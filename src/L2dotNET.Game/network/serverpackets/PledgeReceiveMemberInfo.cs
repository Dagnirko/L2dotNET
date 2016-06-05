﻿using L2dotNET.GameService.Model.communities;

namespace L2dotNET.GameService.network.serverpackets
{
    class PledgeReceiveMemberInfo : GameServerNetworkPacket
    {
        private readonly ClanMember Member;

        public PledgeReceiveMemberInfo(ClanMember cm)
        {
            Member = cm;
        }

        protected internal override void write()
        {
            writeC(0xfe);
            writeH(0x3e);

            writeD(Member.ClanType);
            writeS(Member.Name);
            writeS(Member.NickName);
            writeD(Member.ClanPrivs);
            writeS(Member._pledgeTypeName);
            writeS(Member._ownerName); // name of this member's apprentice/sponsor
        }
    }
}