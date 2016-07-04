﻿namespace L2dotNET.GameService.Network.Serverpackets
{
    class KeyPacket : GameServerNetworkPacket
    {
        private readonly byte[] key;
        private byte next;

        public KeyPacket(GameClient client, byte n)
        {
            key = client.EnableCrypt();
            next = n;
        }

        protected internal override void write()
        {
            writeC(0x00);
            writeC(0x01);
            writeB(key);
            writeD(0x01);
            writeD(0x01);
        }
    }
}