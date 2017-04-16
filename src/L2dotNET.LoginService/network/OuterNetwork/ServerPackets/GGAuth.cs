﻿using L2dotNET.Network;

namespace L2dotNET.LoginService.Network.OuterNetwork.ServerPackets
{
    static class GGAuth
    {
        /// <summary>
        /// Packet opcode.
        /// </summary>
        private const byte Opcode = 0x0b;

        internal static Packet ToPacket(LoginClient client)
        {
            Packet p = new Packet(Opcode);
            p.WriteInt(client.Key.SessionId);
            p.WriteByteArray(new byte[4]);
            return p;
        }
    }
}