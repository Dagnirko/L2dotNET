﻿using L2dotNET.GameService.World;

namespace L2dotNET.GameService.Network.Serverpackets
{
    class CharMoveToLocation : GameserverPacket
    {
        private readonly L2Object _obj;

        public CharMoveToLocation(L2Object obj)
        {
            _obj = obj;
        }

        protected internal override void Write()
        {
            WriteByte(0x01);

            WriteInt(_obj.ObjId);

            WriteInt(_obj.DestX);
            WriteInt(_obj.DestY);
            WriteInt(_obj.DestZ);

            WriteInt(_obj.X);
            WriteInt(_obj.Y);
            WriteInt(_obj.Z);
        }
    }
}