﻿using L2dotNET.Network;

namespace L2dotNET.GameService.Network.Serverpackets
{
    class ExCursedWeaponList : GameserverPacket
    {
        private readonly int[] _ids;

        public ExCursedWeaponList(int[] ids)
        {
            _ids = ids;
        }

        public override void Write()
        {
            WriteByte(0xfe);
            WriteShort(0x46);
            WriteInt(_ids.Length);

            foreach (int id in _ids)
                WriteInt(id);
        }
    }
}