﻿using System.Collections.Generic;
using L2dotNET.GameService.model.items;
using L2dotNET.GameService.model.npcs;
using L2dotNET.GameService.network.l2send;
using L2dotNET.GameService.tables;
using L2dotNET.GameService.world;
using L2dotNET.GameService.model.playable.petai;
using L2dotNET.GameService.tools;

namespace L2dotNET.GameService.model.playable
{
    public class L2Summon : L2Character
    {
        public L2Player Owner;
        public NpcTemplate Template;
        public int CurrentTime;
        public int MaxTime;
        public int StatusSP;
        public long StatusExp;
        public L2Item ControlItem;

        public L2Summon()
        {
            ObjectSummonType = 1;
            ObjID = IdFactory.Instance.nextId();
        }

        public override void onAction(L2Player player)
        {
            player.sendMessage(asString());

            player.ChangeTarget(this);
        }

        public override void NotifyAction(L2Player player)
        {
            if (Owner != null && Owner.ObjID == player.ObjID)
            {
                player.sendPacket(new PetStatusShow(ObjectSummonType));
            }
        }

        public virtual void setTemplate(NpcTemplate template)
        {
            Template = template;
            CStatsInit();
            CharacterStat.setTemplate(template);
            CurHP = CharacterStat.getStat(skills2.TEffectType.b_max_hp);
            MaxTime = 1200; //20 минут
            CurrentTime = MaxTime;
            Level = template.Level;
        }

        public int NpcId
        {
            get { return Template.NpcId; }
        }

        public int NpcHashId
        {
            get { return Template.NpcId + 1000000; }
        }

        public override void broadcastUserInfo()
        {
            foreach (L2Object obj in knownObjects.Values)
                if (obj is L2Player)
                    obj.sendPacket(new PetInfo(this));
        }

        public byte getPvPStatus()
        {
            if (Owner == null)
                return 0;

            return Owner.PvPStatus;
        }

        public int getKarma()
        {
            if (Owner == null)
                return 0;

            return Owner.Karma;
        }

        public virtual long getExpToLevelUp()
        {
            return 0;
        }

        public virtual long getExpCurrentLevel()
        {
            return 0;
        }

        public virtual int CurrentWeight()
        {
            return 0;
        }

        public virtual int MaxWeight()
        {
            return 0;
        }

        public virtual short IsMountable()
        {
            return 0;
        }

        public virtual int getForm()
        {
            return 0;
        }

        private bool IsSpawned = false;
        public void SpawmMe()
        {
            X = Owner.X;
            Y = Owner.Y;
            Z = Owner.Z;
            Heading = Owner.Heading;

            Owner.sendPacket(new PetStatusUpdate(this));

            L2World.Instance.AddObject(this);//to add pet
            IsSpawned = true;
            onSpawn();

            StartRegeneration();

            AICharacter = new SA_Standart(this);
            AICharacter.Enable();
        }

        public void setOwner(L2Player owner)
        {
            Owner = owner;
            owner.Summon = this;

            if (owner.Party != null)
                owner.Party.broadcastToMembers(new ExPartyPetWindowAdd(this));

            Title = owner.Name;
        }

        public virtual void unSummon()
        {
            AICharacter.Disable();

            Owner.sendPacket(new PetDelete(ObjectSummonType, ObjID));

            if (Owner.Party != null)
                Owner.Party.broadcastToMembers(new ExPartyPetWindowDelete(ObjID, Owner.ObjID, Name));

            Owner.Summon = null;
            this.deleteMe();
        }

        public bool isTeleporting = false;
        public double ConsumeExp = 30.0;
        // 0 - teleport, 1 - default, 2 - summoned
        public byte AppearMethod()
        {
            if (!IsSpawned)
                return 2;

            if (isTeleporting)
                return 0;

            return 1;
        }

        public virtual void ChangeNode()
        {
            AICharacter.ChangeFollowStatus();
        }

        public virtual void Attack()
        {

        }

        public virtual void Stop()
        {
            NotifyStopMove(false, true);
        }

        public virtual void Move()
        {
            if (Owner.CurrentTarget == null)
                return;

            double dis = Calcs.calculateDistance(this, Owner.CurrentTarget, true);

            if (dis > 40 && dis < 2300)
            {
                if (!cantMove())
                    MoveTo(Owner.CurrentTarget.X, Owner.CurrentTarget.Y, Owner.CurrentTarget.Z);
            }
        }

        public override L2Character[] getPartyCharacters()
        {
            List<L2Character> chars = new List<L2Character>();
            chars.Add(this);
            if (Owner != null)
                chars.Add(Owner);

            if (Owner.Party != null)
            {
                if (chars == null)
                    chars = new List<L2Character>();

                foreach (L2Player pl in Owner.Party.Members)
                {
                    if (pl.ObjID == Owner.ObjID)
                        continue;

                    chars.Add(pl);

                    if (pl.Summon != null)
                        chars.Add(pl.Summon);
                }
            }

            return chars.ToArray();
        }

        public override void updateAbnormalEffect()
        {
            if (Owner == null || Owner.Party == null)
                return;

            if (_effects.Count == 0)
                return;

            PartySpelled p = new PartySpelled(this);
            List<AbnormalEffect> nulled = new List<AbnormalEffect>();
            foreach (AbnormalEffect ei in _effects)
            {
                if (ei != null)
                {
                    if (ei.active == 1)
                    {
                        p.addIcon(ei.id, ei.lvl, ei.getTime());
                    }
                    else
                        nulled.Add(ei);
                }
            }

            lock (_effects)
                foreach (AbnormalEffect ei in nulled)
                    _effects.Remove(ei);

            nulled.Clear();
            Owner.Party.broadcastToMembers(p);
        }


        public override string asString()
        {
            return "L2Summon:" + ObjID + "";
        }
    }
}
