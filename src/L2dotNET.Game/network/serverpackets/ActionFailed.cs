﻿namespace L2dotNET.GameService.network.serverpackets
{
    class ActionFailed : GameServerNetworkPacket
    {
        protected internal override void write()
        {
            writeC(0x25);
        }
    }
}