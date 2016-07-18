﻿using L2dotNET.GameService.World;
using L2dotNET.Network;

namespace L2dotNET.GameService.Network.Serverpackets
{
    class TargetUnselected : GameserverPacket
    {
        private readonly int _id;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public TargetUnselected(L2Object obj)
        {
            _id = obj.ObjId;
            _x = obj.X;
            _y = obj.Y;
            _z = obj.Z;
        }

        public override void Write()
        {
            WriteByte(0x2a);
            WriteInt(_id);
            WriteInt(_x);
            WriteInt(_y);
            WriteInt(_z);
        }
    }
}