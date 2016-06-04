﻿using System.Collections.Generic;
using L2dotNET.GameService.model.items;

namespace L2dotNET.GameService.model.skills2.conds
{
    class Cond_EquipWeapon : TSkillCond
    {
        private List<string> allowed = new List<string>();

        public void add(string mask)
        {
            if (!allowed.Contains(mask))
                allowed.Add(mask);
        }

        public override bool CanUse(L2Player player, TSkill skill)
        {

            L2Item item = player.Inventory.getWeapon();

            if (item != null)
            {
                foreach (string mask in allowed)
                    if (mask.Equals(item.Template.WeaponType.ToString()))
                        return true;
            }

            return false;

        }
    }
}
