﻿
namespace L2dotNET.GameService.network.l2send
{
    class CharCreateOk : GameServerNetworkPacket
    {
        protected internal override void write()
        {
            writeC(0x19);
            writeD(0x01);
        }
    }
}
