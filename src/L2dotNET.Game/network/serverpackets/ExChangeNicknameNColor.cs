﻿namespace L2dotNET.GameService.Network.Serverpackets
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