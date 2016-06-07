﻿using System.Linq;
using L2dotNET.GameService.Model.Npcs.Decor;
using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Model.Structures;
using L2dotNET.GameService.Network.Serverpackets;

namespace L2dotNET.GameService.Model.Npcs
{
    class L2Doormen : L2Npc
    {
        private readonly Hideout hideout;

        public L2Doormen(HideoutTemplate hideout)
        {
            this.hideout = (Hideout)hideout;
            structureControlled = true;
        }

        public override void NotifyAction(L2Player player)
        {
            if (hideout.ownerId == player.ClanId)
            {
                NpcHtmlMessage htm = new NpcHtmlMessage(player, "agitjanitorhi.htm", ObjID);
                htm.replace("<?my_pledge_name?>", player.Clan.Name);
                player.sendPacket(htm);
            }
        }

        public override void onDialog(L2Player player, int ask, int reply)
        {
            player.FolkNpc = this;

            switch (ask)
            {
                case 0:
                    NotifyAction(player);
                    break;
                case -203:
                    switch (reply)
                    {
                        case 1: //open ch doors
                            foreach (L2Door door in hideout.doors.Where(door => door.Closed != 0))
                            {
                                door.Closed = 0;
                                door.broadcastUserInfo();
                            }

                            player.sendPacket(new NpcHtmlMessage(player, "AgitJanitorAfterDoorOpen.htm", ObjID));
                            break;
                        case 2: //close
                            foreach (L2Door door in hideout.doors.Where(door => door.Closed != 1))
                            {
                                door.Closed = 1;
                                door.broadcastUserInfo();
                            }

                            player.sendPacket(new NpcHtmlMessage(player, "AgitJanitorAfterDoorClose.htm", ObjID));
                            break;
                    }
                    break;
            }
        }

        public override string asString()
        {
            return "L2Doormen:" + Template.NpcId + "; id " + ObjID + "; " + hideout.ID;
        }
    }
}