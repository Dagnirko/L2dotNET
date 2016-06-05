﻿using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Timers;
using L2dotNET.GameService.network;
using L2dotNET.GameService.network.serverpackets;

namespace L2dotNET.GameService.Model.player
{
    [Synchronization]
    public class L2Party
    {
        public LinkedList<L2Player> Members;
        private L2Player player;
        public int itemDistribution;
        public L2Player leader;

        public L2Party(L2Player player)
        {
            Members = new LinkedList<L2Player>();
            Members.AddLast(player);
            leader = player;
            this.player = player;
            this.itemDistribution = player.itemDistribution;
        }

        public double[] bonusExp = new double[] { 1, 1.30, 1.39, 1.50, 1.54, 1.58, 1.63, 1.67, 1.71 };
        public short[] LOOT_SYSSTRINGS = { 487, 488, 798, 799, 800 };

        public byte ITEM_LOOTER = 0;
        public byte ITEM_RANDOM = 1;
        public byte ITEM_RANDOM_SPOIL = 2;
        public byte ITEM_ORDER = 3;
        public byte ITEM_ORDER_SPOIL = 4;

        public short VoteId = -1;

        public void addMember(L2Player player, bool onCreate = false)
        {
            Members.AddLast(player);

            if (!onCreate)
                player.sendPacket(new PartySmallWindowAll(this));
            else
                broadcastToMembers(new PartySmallWindowAll(this));

            player.Party = this;

            SystemMessage sm = new SystemMessage(SystemMessage.SystemMessageId.YOU_JOINED_S1_PARTY);
            sm.AddPlayerName(leader.Name);
            player.sendPacket(sm);

            sm = new SystemMessage(SystemMessage.SystemMessageId.S1_JOINED_PARTY);
            sm.AddPlayerName(leader.Name);
            broadcastToMembers(sm, player.ObjID);
        }

        public void broadcastToMembers(GameServerNetworkPacket pk)
        {
            foreach (L2Player pl in Members)
                pl.sendPacket(pk);
        }

        public void broadcastToMembers(GameServerNetworkPacket pk, int except)
        {
            foreach (L2Player pl in Members)
                if (pl.ObjID != except)
                    pl.sendPacket(pk);
        }

        private byte votesOnStart = 0,
                     votesVoted = 0;
        private Timer voteTimer = null;
        private SortedList<int, byte> votes = null;

        public void VoteForLootChange(byte mode)
        {
            VoteId = mode;
            broadcastToMembers(new ExAskModifyPartyLooting(leader.Name, mode));
            SystemMessage sm = new SystemMessage(SystemMessage.SystemMessageId.REQUESTING_APPROVAL_FOR_CHANGING_PARTY_LOOT_TO_S1);
            sm.AddSysStr(LOOT_SYSSTRINGS[mode]);
            leader.sendPacket(sm);

            votesOnStart = (byte)Members.Count;

            voteTimer = new Timer();
            voteTimer.Interval = 30000;
            voteTimer.Enabled = true;
            voteTimer.Elapsed += new ElapsedEventHandler(voteTimer_Elapsed);
            votes = new SortedList<int, byte>(votesOnStart);
        }

        private void voteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            FinishVoting();
        }

        public void AnswerLootVote(L2Player player, byte answer)
        {
            votesVoted++;

            if (!votes.ContainsKey(player.ObjID))
                votes.Add(player.ObjID, answer);

            if (votes.Count == votesOnStart)
                FinishVoting();
        }

        private void FinishVoting()
        {
            voteTimer.Enabled = false;

            double half = votesOnStart * 0.5;
            byte agreed = 0;
            foreach (byte vote in votes.Values)
                if (vote == 1)
                    agreed++;

            votes.Clear();
            SystemMessage sm;
            if (agreed > half)
            {
                sm = new SystemMessage(SystemMessage.SystemMessageId.PARTY_LOOT_WAS_CHANGED_TO_S1);
                sm.AddSysStr(LOOT_SYSSTRINGS[VoteId]);
                itemDistribution = VoteId;
            }
            else
            {
                sm = new SystemMessage(SystemMessage.SystemMessageId.PARTY_LOOT_CHANGE_WAS_CANCELLED);
                VoteId = -1;
            }

            broadcastToMembers(sm);
            broadcastToMembers(new ExSetPartyLooting(VoteId));
            VoteId = -1;
        }

        public void Leave(L2Player player)
        {
            if (leader.ObjID == player.ObjID)
            {
                if (Members.Count > 2)
                {
                    kick(player);

                    leader = Members.First.Value;
                    SystemMessage sm = new SystemMessage(SystemMessage.SystemMessageId.S1_HAS_BECOME_A_PARTY_LEADER);
                    sm.AddPlayerName(leader.Name);
                    broadcastToMembers(sm);

                    broadcastToMembers(new PartySmallWindowDeleteAll());
                    broadcastToMembers(new PartySmallWindowAll(this));
                }
                else
                {
                    foreach (L2Player pl in Members)
                    {
                        pl.sendSystemMessage(SystemMessage.SystemMessageId.PARTY_DISPERSED);
                        pl.sendPacket(new PartySmallWindowDeleteAll());
                        pl.Party = null;
                    }

                    Members.Clear();

                    if (voteTimer != null && voteTimer.Enabled)
                        voteTimer.Enabled = false;
                }
            }
            else
                kick(player);
        }

        private void kick(L2Player player, short msg1 = 200, short msg2 = 108)
        {
            lock (Members)
                Members.Remove(player);

            if (Members.Count > 2)
            {
                player.sendSystemMessage(SystemMessage.SystemMessageId.YOU_LEFT_PARTY);
                player.sendPacket(new PartySmallWindowDeleteAll());
                player.Party = null;

                SystemMessage sm = new SystemMessage(SystemMessage.SystemMessageId.S1_LEFT_PARTY);
                sm.AddPlayerName(player.Name);
                broadcastToMembers(sm);
                broadcastToMembers(new PartySmallWindowDelete(player.ObjID, player.Name));

                if (player.Summon != null)
                    broadcastToMembers(new ExPartyPetWindowDelete(player.Summon.ObjID, player.ObjID, player.Summon.Name));
            }
            else
            {
                foreach (L2Player pl in Members)
                {
                    pl.sendSystemMessage(SystemMessage.SystemMessageId.PARTY_DISPERSED);
                    pl.sendPacket(new PartySmallWindowDeleteAll());
                    pl.Party = null;
                }

                Members.Clear();
            }
        }

        public void Expel(string name)
        {
            L2Player player = null;

            foreach (L2Player pl in Members)
                if (pl.Name.Equals(name))
                {
                    player = pl;
                    break;
                }

            if (player == null)
            {
                player.sendSystemMessage(SystemMessage.SystemMessageId.FAILED_TO_EXPEL_THE_PARTY_MEMBER);
                player.sendActionFailed();
                return;
            }

            kick(player, (int)SystemMessage.SystemMessageId.HAVE_BEEN_EXPELLED_FROM_PARTY, (int)SystemMessage.SystemMessageId.S1_WAS_EXPELLED_FROM_PARTY);
        }
    }
}