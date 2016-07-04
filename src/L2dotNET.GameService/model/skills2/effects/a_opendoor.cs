﻿using System;
using L2dotNET.GameService.Model.Npcs.Decor;
using L2dotNET.GameService.Network.Serverpackets;
using L2dotNET.GameService.World;

namespace L2dotNET.GameService.Model.Skills2.Effects
{
    class a_opendoor : TEffect
    {
        private int level,
                    rate;

        public override void build(string str)
        {
            level = Convert.ToInt32(str.Split(' ')[1]);
            rate = Convert.ToInt32(str.Split(' ')[2]);
        }

        public override TEffectResult onStart(L2Character caster, L2Character target)
        {
            if (target is L2Door)
            {
                L2Door door = (L2Door)target;
                if (door.Level <= level)
                    if (new Random().Next(100) < rate)
                        door.OpenForTime();
                    else
                        caster.SendSystemMessage(SystemMessage.SystemMessageId.FAILED_TO_UNLOCK_DOOR);
                else
                    caster.SendSystemMessage(SystemMessage.SystemMessageId.FAILED_TO_UNLOCK_DOOR);
            }
            else
            {
                caster.SendSystemMessage(SystemMessage.SystemMessageId.TARGET_IS_INCORRECT);
            }

            return nothing;
        }

        public override bool canUse(L2Character caster)
        {
            L2Object target = caster.CurrentTarget;
            if (target is L2Door)
            {
                L2Door door = target as L2Door;
                if (door.Closed == 0)
                {
                    caster.SendSystemMessage(SystemMessage.SystemMessageId.TARGET_IS_INCORRECT);
                    return false;
                }

                if (!door.UnlockSkill)
                {
                    caster.SendSystemMessage(SystemMessage.SystemMessageId.UNABLE_TO_UNLOCK_DOOR);
                    return false;
                }
            }
            else
            {
                caster.SendSystemMessage(SystemMessage.SystemMessageId.TARGET_IS_INCORRECT);
                return false;
            }

            return true;
        }
    }
}