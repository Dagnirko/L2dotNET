﻿using L2dotNET.Network;

namespace L2dotNET.GameService.Network.Serverpackets
{
    class NetPing : GameserverPacket
    {
        private readonly int _request;

        public NetPing(int request)
        {
            _request = request;
        }

        public override void Write()
        {
            WriteByte(0xd9);
            WriteInt(_request);
        }
    }
}