﻿
namespace L2dotNET.Game.network.l2send
{
    class ExPledgeCrestLarge : GameServerNetworkPacket
    {
        private int id;
        private byte[] picture;
        public ExPledgeCrestLarge(int id, byte[] picture)
        {
            this.id = id;
            if(picture == null)
                picture = new byte[0];

            this.picture = picture;
        }

        protected internal override void write()
        {
            writeC(0xfe);
            writeH(0x1b);

            writeD(0x00); //???
            writeD(id);
            writeD(picture.Length);
            writeB(picture);
        }
    }
}
