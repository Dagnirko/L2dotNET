﻿
namespace L2dotNET.GameService.model.skills2.conds
{
    public class can_summon_cubic : TSkillCond
    {
        public override bool CanUse(L2Player player, TSkill skill)
        {
            //int len = player.cubics.Count;
            //int max = (int)player.CharacterStat.getStat(TEffectType.p_cubic_mastery);
            //if (max == 0)
            //    max = 1;

            //return !(len + 1 > max);

            return true;
        }
    }
}
