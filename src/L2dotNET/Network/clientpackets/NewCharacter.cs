﻿using System.Collections.Generic;
using System.Linq;
using L2dotNET.Network.serverpackets;
using L2dotNET.tables;
using L2dotNET.templates;

namespace L2dotNET.Network.clientpackets
{
    class NewCharacter : PacketBase
    {
        private readonly GameClient _client;

        public NewCharacter(Packet packet, GameClient client)
        {
            _client = client;
        }

        public override void RunImpl()
        {
            Dictionary<int, PcTemplate> dict = CharTemplateTable.Instance.Templates;
            List<PcTemplate> pcTemp = dict.Select((t, i) => dict.SingleOrDefault(x => x.Key == i).Value).ToList();

            _client.SendPacket(new CharTemplates(pcTemp));
        }
    }
}