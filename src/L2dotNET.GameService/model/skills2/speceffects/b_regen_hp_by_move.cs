﻿using L2dotNET.GameService.Model.Player;

namespace L2dotNET.GameService.Model.Skills2.SpecEffects
{
    public class b_regen_hp_by_move : TSpecEffect
    {
        public b_regen_hp_by_move(double value)
        {
            this.value = value;
        }

        public override void OnStartMoving(L2Player player)
        {
            player.CharacterStat.SpecBonusRegHP += value;
            player.SendMessage("reg hp inc to " + player.CharacterStat.getStat(TEffectType.b_reg_hp));
        }

        public override void OnStopMoving(L2Player player)
        {
            player.CharacterStat.SpecBonusRegHP -= value;
            player.SendMessage("reg hp lowered to " + player.CharacterStat.getStat(TEffectType.b_reg_hp));
        }
    }
}