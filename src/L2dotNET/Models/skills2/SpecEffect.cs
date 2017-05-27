﻿using L2dotNET.model.items;
using L2dotNET.model.player;
using L2dotNET.world;

namespace L2dotNET.model.skills2
{
    public class SpecEffect
    {
        public double Value;
        public bool Mul = false;

        public virtual void OnStartMoving(L2Player player) { }

        public virtual void OnStopMoving(L2Player player) { }

        public virtual void OnStartNight(L2Player player) { }

        public virtual void OnStartDay(L2Player player) { }

        public virtual void OnReceiveDamage(L2Player player, L2Character attacker) { }

        public virtual void OnGaveDamage(L2Player player, L2Character target, bool crit) { }

        public virtual void OnCastSkill(L2Player player, L2Character target, Skill skill) { }

        public virtual void OnStand(L2Player player) { }

        public virtual void OnSit(L2Player player) { }

        public virtual void OnEquipChest(L2Player player, L2Item item) { }

        public virtual void OnUnEquipChest(L2Player player) { }
    }
}