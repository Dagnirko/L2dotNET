﻿using System;
using L2dotNET.world;

namespace L2dotNET.model.skills2.effects
{
    public class TargetCancel : Effect
    {
        public TargetCancel()
        {
            Type = EffectType.ITargetCancel;
        }

        private int _rate;

        public override void Build(string str)
        {
            string[] v = str.Split(' ');
            _rate = int.Parse(v[1]);
        }

        public override EffectResult OnStart(L2Character caster, L2Character target)
        {
            if (new Random().Next(100) < _rate)
                target.SetTarget(null);

            return Nothing;
        }
    }
}