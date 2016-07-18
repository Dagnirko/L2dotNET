﻿using L2dotNET.GameService.World;
using L2dotNET.Network;

namespace L2dotNET.GameService.Network.Serverpackets
{
    class StopMove : GameserverPacket
    {
        private readonly int _id;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;
        private readonly int _h;

        public StopMove(L2Character cha)
        {
            _id = cha.ObjId;
            _x = cha.X;
            _y = cha.Y;
            _z = cha.Z;
            _h = cha.Heading;
        }

        public override void Write()
        {
            WriteByte(0x47);
            WriteInt(_id);
            WriteInt(_x);
            WriteInt(_y);
            WriteInt(_z);
            WriteInt(_h);
        }
    }
}