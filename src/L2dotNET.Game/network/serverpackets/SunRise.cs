﻿namespace L2dotNET.GameService.network.l2send
{
    class SunRise : GameServerNetworkPacket
    {
        protected internal override void write()
        {
            writeC(0x1c);
        }
    }
}