﻿namespace L2dotNET.GameService.Network.Serverpackets
{
    class ExPutEnchantSupportItemResult : GameserverPacket
    {
        private readonly int _result;

        public ExPutEnchantSupportItemResult(int result = 0)
        {
            _result = result;
        }

        protected internal override void Write()
        {
            WriteByte(0xfe);
            WriteShort(0x82);
            WriteInt(_result);
        }
    }
}