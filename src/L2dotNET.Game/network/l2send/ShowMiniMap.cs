﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2dotNET.Game.network.l2send
{
    class ShowMiniMap : GameServerNetworkPacket
    {
        protected internal override void write()
        {
            writeC(0xa3);
            writeD(1665);
            writeD(0);//SevenSigns.getInstance().getCurrentPeriod());
        }
    }
}
