﻿using L2dotNET.Game.tables;
using L2dotNET.Game.network.l2send;

namespace L2dotNET.Game.model.zones.classes
{
    class swamp : L2Zone
    {
        public swamp()
        {
            ZoneID = IdFactory.getInstance().nextId();
        }

        public override void onInit()
        {
            base.onInit();
            _enabled = Template.DefaultStatus;
        }

        public override void onEnter(world.L2Object obj)
        {
            if (!_enabled)
                return;

            base.onEnter(obj);

            obj.onEnterZone(this);

            if (obj is L2Player)
            {
                L2Player p = (L2Player)obj;
                p.isInDanger = true;
                p.sendPacket(new EtcStatusUpdate(p));
              //  p._stats.base_p_speed += Template._move_bonus;
                p.broadcastUserInfo();
            }
        }

        public override void onExit(world.L2Object obj, bool cls)
        {
            if (!_enabled)
                return;

            base.onExit(obj, cls);

            obj.onExitZone(this, cls);

            if (obj is L2Player)
            {
                L2Player p = (L2Player)obj;
                p.isInDanger = false;
                p.sendPacket(new EtcStatusUpdate(p));
              //  p._stats.base_p_speed -= Template._move_bonus;
                p.broadcastUserInfo();
            }
        }
    }
}
