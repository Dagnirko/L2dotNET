﻿namespace L2dotNET.GameService.Network.Serverpackets
{
    class AskJoinParty : GameServerNetworkPacket
    {
        private readonly string asker;
        private readonly int itemDistribution;

        public AskJoinParty(string asker, int itemDistribution)
        {
            this.asker = asker;
            this.itemDistribution = itemDistribution;
        }

        protected internal override void write()
        {
            writeC(0x39);
            writeS(asker);
            writeD(itemDistribution);
        }
    }
}