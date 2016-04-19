﻿using L2dotNET.Game.tables;

namespace L2dotNET.Game.model.zones.classes
{
    class battle_zone : L2Zone
    {
        public battle_zone()
        {
            ZoneID = IdFactory.getInstance().nextId();
            _enabled = true;
        }

        public override void onEnter(world.L2Object obj)
        {
            if (!_enabled)
                return;

            base.onEnter(obj);

            obj.onEnterZone(this);
        }

        public override void onExit(world.L2Object obj, bool cls)
        {
            if (!_enabled)
                return;

            base.onExit(obj, cls);

            obj.onExitZone(this, cls);
        }
    }
}
