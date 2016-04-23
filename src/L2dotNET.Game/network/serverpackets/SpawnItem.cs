﻿using L2dotNET.Game.model.items;

namespace L2dotNET.Game.network.l2send
{
    class SpawnItem : GameServerNetworkPacket
    {
        private L2Item _item;
        public SpawnItem(L2Item item)
        {
            _item = item;
        }

        protected internal override void write()
        {
            writeC(0x0b);
            writeD(_item.ObjID);
            writeD(_item.Template.ItemID);
            writeD(_item.X);
            writeD(_item.Y);
            writeD(_item.Z);
            writeD(_item.Template.isStackable() ? 1 : 0);
            writeD((int)_item.Count);
            writeD(0); // ?
        }
    }
}
