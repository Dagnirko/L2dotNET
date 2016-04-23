﻿using L2dotNET.Game.model.skills2;

namespace L2dotNET.Game.network.l2send
{
    class PartySmallWindowUpdate : GameServerNetworkPacket
    {
        private L2Player member;
        public PartySmallWindowUpdate(L2Player member)
        {
            this.member = member;
        }

        protected internal override void write()
        {
            writeC(0x52);
            writeD(member.ObjID);
            writeS(member.Name);
            writeD(member.CurCP);
            writeD(member.CharacterStat.getStat(TEffectType.b_max_cp));
            writeD(member.CurHP);
            writeD(member.CharacterStat.getStat(TEffectType.b_max_hp));
            writeD(member.CurMP);
            writeD(member.CharacterStat.getStat(TEffectType.b_max_mp));
            writeD(member.Level);
            writeD(member.ActiveClass.id);
        }
    }
}
