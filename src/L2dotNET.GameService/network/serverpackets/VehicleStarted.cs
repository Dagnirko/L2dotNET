﻿namespace L2dotNET.GameService.Network.Serverpackets
{
    class VehicleStarted : GameserverPacket
    {
        private readonly int _sId;
        private readonly int _type;

        public VehicleStarted(int sId, int type)
        {
            _sId = sId;
            _type = type;
        }

        protected internal override void Write()
        {
            WriteByte(0xBA);
            WriteInt(_sId);
            WriteInt(_type);
        }
    }
}