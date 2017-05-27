﻿using System;
using L2dotNET.model.npcs.decor;
using L2dotNET.Network.serverpackets;
using L2dotNET.world;

namespace L2dotNET.model.skills2.effects
{
    class AOpendoor : Effect
    {
        private int _level,
                    _rate;

        public override void Build(string str)
        {
            _level = Convert.ToInt32(str.Split(' ')[1]);
            _rate = Convert.ToInt32(str.Split(' ')[2]);
        }

        public override EffectResult OnStart(L2Character caster, L2Character target)
        {
            if (target is L2Door)
            {
                L2Door door = (L2Door)target;
                if (door.Level <= _level)
                {
                    if (new Random().Next(100) < _rate)
                        door.OpenForTime();
                    else
                        caster.SendSystemMessage(SystemMessage.SystemMessageId.FailedToUnlockDoor);
                }
                else
                    caster.SendSystemMessage(SystemMessage.SystemMessageId.FailedToUnlockDoor);
            }
            else
                caster.SendSystemMessage(SystemMessage.SystemMessageId.TargetIsIncorrect);

            return Nothing;
        }

        public override bool CanUse(L2Character caster)
        {
            L2Object target = caster.Target;
            if (target is L2Door)
            {
                L2Door door = (L2Door)target;
                if (door.Closed == 0)
                {
                    caster.SendSystemMessage(SystemMessage.SystemMessageId.TargetIsIncorrect);
                    return false;
                }

                if (door.UnlockSkill)
                    return true;

                caster.SendSystemMessage(SystemMessage.SystemMessageId.UnableToUnlockDoor);
                return false;
            }

            caster.SendSystemMessage(SystemMessage.SystemMessageId.TargetIsIncorrect);
            return false;
        }
    }
}