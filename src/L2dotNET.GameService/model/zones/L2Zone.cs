﻿using System.Collections.Generic;
using System.Timers;
using L2dotNET.GameService.Model.Playable;
using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Model.Zones.Forms;
using L2dotNET.GameService.Network;
using L2dotNET.GameService.World;

namespace L2dotNET.GameService.Model.Zones
{
    public class L2Zone
    {
        public ZoneForm Territory;
        public int ZoneID;
        public string _zonePch;
        public bool _enabled = false;
        public ZoneTemplate Template;
        public int InstanceID = -1;
        public L2Object NpcCenter;

        public SortedList<int, L2Object> ObjectsInside = new SortedList<int, L2Object>();

        public virtual void onEnter(L2Object obj)
        {
            if (!ObjectsInside.ContainsKey(obj.ObjId))
                ObjectsInside.Add(obj.ObjId, obj);
        }

        public void broadcastPacket(GameServerNetworkPacket pk)
        {
            foreach (L2Object obj in ObjectsInside.Values)
                if (obj is L2Player)
                    obj.SendPacket(pk);
                else if (obj is L2Summon)
                    ((L2Summon)obj).SendPacket(pk);
        }

        public virtual void onExit(L2Object obj, bool cls)
        {
            if (cls)
                lock (ObjectsInside)
                {
                    if (ObjectsInside.ContainsKey(obj.ObjId))
                        ObjectsInside.Remove(obj.ObjId);
                }
        }

        public virtual void onDie(L2Character obj, L2Character killer) { }

        public virtual void onKill(L2Character obj, L2Character target) { }

        public Timer _action;

        public virtual void startTimer()
        {
            _action = new Timer(Template._unit_tick * 1000);
            _action.Elapsed += new ElapsedEventHandler(onTimerAction);
            _action.Interval = Template._unit_tick * 1000;
            _action.Enabled = true;
        }

        public virtual void stopTimer() { }

        public virtual void onTimerAction(object sender, ElapsedEventArgs e) { }

        public virtual void onInit() { }

        private Timer _selfDestruct;
        public int[] CylinderCenter;
        public string Name;

        public void SelfDestruct(int sec)
        {
            _selfDestruct = new Timer(sec * 1000);
            _selfDestruct.Elapsed += new ElapsedEventHandler(desctructTime);
            _selfDestruct.Enabled = true;
        }

        private void desctructTime(object sender, ElapsedEventArgs e)
        {
            _selfDestruct.Enabled = false;

            NpcCenter.DeleteMe();

            foreach (L2Object o in ObjectsInside.Values)
                onExit(o, false);

            ObjectsInside.Clear();

            L2WorldRegion region = L2World.Instance.GetRegion(CylinderCenter[0], CylinderCenter[1]);
            if (region != null)
            {
                // region._zoneManager.remZone(this);
            }
        }
    }
}