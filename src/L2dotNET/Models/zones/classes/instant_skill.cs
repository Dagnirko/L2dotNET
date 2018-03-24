﻿using System.Timers;
using L2dotNET.Models.npcs;
using L2dotNET.Models.player;
using L2dotNET.tables;

namespace L2dotNET.Models.zones.classes
{
    class instant_skill : L2Zone
    {
        public instant_skill()
        {
            ZoneId = IdFactory.Instance.NextId();
        }

        public override void OnInit()
        {
            Enabled = Template.DefaultStatus;

            if (Enabled && (Template.UnitTick > 0))
                StartTimer();
        }

        public override void OnTimerAction(object sender, ElapsedEventArgs e)
        {
            if (ObjectsInside.Count == 0)
                return;

            foreach (L2Object o in ObjectsInside.Values)
            {
                if (o is L2Player)
                {
                    if (Template.Target == ZoneTemplate.ZoneTarget.Npc)
                        continue;

                   // affect((L2Character)o);
                }
                else
                {
                    if (!(o is L2Warrior))
                        continue;

                    if ((Template.Target == ZoneTemplate.ZoneTarget.Pc) || (Template.Target == ZoneTemplate.ZoneTarget.OnlyPc))
                        continue;

                   // affect((L2Character)o);
                }
            }
        }
        
        public override void OnEnter(L2Object obj)
        {
            if (!Enabled)
                return;

            base.OnEnter(obj);

            obj.OnEnterZone(this);
        }

        public override void OnExit(L2Object obj, bool cls)
        {
            if (!Enabled)
                return;

            base.OnExit(obj, cls);

            obj.OnExitZone(this, cls);
        }
    }
}