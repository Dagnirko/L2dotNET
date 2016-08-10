﻿using L2dotNET.model.player;

namespace L2dotNET.Network.clientpackets
{
    class RequestUnEquipItem : PacketBase
    {
        private readonly GameClient _client;
        private readonly int _slotBitType;

        public RequestUnEquipItem(Packet packet, GameClient client)
        {
            _client = client;
            _slotBitType = packet.ReadInt();
        }

        public override void RunImpl()
        {
            L2Player player = _client.CurrentPlayer;

            if (player.PBlockAct == 1)
            {
                player.SendActionFailed();
                return;
            }

            //int dollId = player.Inventory.getPaperdollIdByMask(slotBitType);

            //player.setPaperdoll(dollId, null, true);
            player.BroadcastUserInfo();
        }
    }
}