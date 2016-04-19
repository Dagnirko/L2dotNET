﻿
namespace L2dotNET.Game.network.l2send
{
    class ExPutEnchantTargetItemResult : GameServerNetworkPacket
    {
        private int result;
        public ExPutEnchantTargetItemResult(int result = 0)
        {
            this.result = result;
        }

        protected internal override void write()
        {
            writeC(0xfe);
            writeH(0x81);
            writeD(result);
        }
    }
}
