﻿namespace L2dotNET.Network.serverpackets
{
    class MyTargetSelected : GameserverPacket
    {
        private readonly int _targetId;
        private readonly short _color;

        public MyTargetSelected(int target, int color)
        {
            _targetId = target;
            _color = (short)color;
        }

        public override void Write()
        {
            WriteByte(0xa6);
            WriteInt(_targetId);
            WriteShort(_color);
        }
    }
}