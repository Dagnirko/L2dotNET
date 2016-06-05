﻿namespace L2dotNET.GameService.network.l2send
{
    class GetItem : GameServerNetworkPacket
    {
        private readonly int _id;
        private readonly int _itemId;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public GetItem(int id, int itemId, int x, int y, int z)
        {
            _id = id;
            _itemId = itemId;
            _x = x;
            _y = y;
            _z = z;
        }

        protected internal override void write()
        {
            writeC(0x0d);
            writeD(_id);
            writeD(_itemId);
            writeD(_x);
            writeD(_y);
            writeD(_z);
        }
    }
}