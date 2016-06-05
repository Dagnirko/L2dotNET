﻿using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Model.Skills2;
using L2dotNET.GameService.Network.Serverpackets;

namespace L2dotNET.GameService.Model.Npcs.Cubic.Data
{
    public class heal_cubic : CubicTemplate
    {
        public heal_cubic(int id, byte lv, int skillId, int skilllv, int duration = 900)
        {
            this.id = id;
            this.level = lv;
            this.skill1 = TSkillTable.Instance.Get(skillId, skilllv);
            this.delay = 13;
            this.duration = duration;
            this.max_count = 20;
        }

        public override int AiActionTask(L2Player owner)
        {
            if (owner.Dead || owner.CurHP / owner.MaxHP > 0.9)
                return 0;

            owner.addAbnormal(skill1, owner, true, false);
            owner.broadcastPacket(new MagicSkillUse(owner, owner, skill1, skill1.skill_hit_time));
            return 1;
        }
    }
}