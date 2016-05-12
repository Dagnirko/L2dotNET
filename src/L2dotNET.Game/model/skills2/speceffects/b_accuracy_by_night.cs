﻿using L2dotNET.GameService.model.skills2.effects;
using L2dotNET.GameService.network.l2send;

namespace L2dotNET.GameService.model.skills2.speceffects
{
    public class b_accuracy_by_night : TSpecEffect
    {
        TEffect effect;
        public b_accuracy_by_night(double value, int skillId, int lvl)
        {
            this.value = value;
            effect = new b_accuracy();
            effect.HashID = skillId * 65536 + lvl;
            effect.SkillId = skillId;
            effect.SkillLv = lvl;
            effect.build("st +"+value);
        }

        public override void OnStartNight(L2Player player)
        {
            //It is now midnight and the effect of $s1 can be felt.
            player.sendPacket(new SystemMessage(1131).AddSkillName(effect.SkillId, effect.SkillLv));
            player.addStat(effect);
        }

        public override void OnStartDay(L2Player player)
        {
            //It is dawn and the effect of $s1 will now disappear.
            player.sendPacket(new SystemMessage(1132).AddSkillName(effect.SkillId, effect.SkillLv));
            player.removeStat(effect);
        }
    }
}
