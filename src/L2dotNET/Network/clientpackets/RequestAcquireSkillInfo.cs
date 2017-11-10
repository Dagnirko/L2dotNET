﻿using System.Collections.Generic;
using L2dotNET.model.player;
using L2dotNET.Network.serverpackets;

namespace L2dotNET.Network.clientpackets
{
    class RequestAcquireSkillInfo : PacketBase
    {
        private readonly GameClient _client;
        private readonly int _id;
        private readonly int _level;
        private readonly int _skillType;

        public RequestAcquireSkillInfo(Packet packet, GameClient client)
        {
            _client = client;
            _id = packet.ReadInt();
            _level = packet.ReadInt();
            _skillType = packet.ReadInt();
        }

        public override void RunImpl()
        {
            L2Player player = _client.CurrentPlayer;
            
        }
    }
}