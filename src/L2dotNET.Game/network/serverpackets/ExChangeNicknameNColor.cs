﻿namespace L2dotNET.GameService.network.l2send
{
    class ExChangeNicknameNColor : GameServerNetworkPacket
    {
        protected internal override void write()
        {
            writeC(0xFE);
            writeH(0x83);
        }
    }
}