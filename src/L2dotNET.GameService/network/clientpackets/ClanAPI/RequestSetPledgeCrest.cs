﻿using L2dotNET.GameService.Model.Communities;
using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Network.Serverpackets;

namespace L2dotNET.GameService.Network.Clientpackets.ClanAPI
{
    class RequestSetPledgeCrest : GameServerNetworkRequest
    {
        private int _size;
        private byte[] _picture;

        public RequestSetPledgeCrest(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            _size = readD();

            if ((_size > 0) && (_size <= 256))
                _picture = readB(_size);
        }

        public override void run()
        {
            L2Player player = getClient().CurrentPlayer;

            if (player.ClanId == 0)
            {
                player.SendActionFailed();
                return;
            }

            L2Clan clan = player.Clan;

            if (clan.Level < 3)
            {
                player.SendSystemMessage(SystemMessage.SystemMessageId.CLAN_LVL_3_NEEDED_TO_SET_CREST);
                player.SendActionFailed();
                return;
            }

            if (clan.IsDissolving())
            {
                player.SendSystemMessage(SystemMessage.SystemMessageId.CANNOT_SET_CREST_WHILE_DISSOLUTION_IN_PROGRESS);
                player.SendActionFailed();
                return;
            }

            if ((_size < 0) || (_size > 256))
            {
                player.SendSystemMessage(SystemMessage.SystemMessageId.CAN_ONLY_REGISTER_16_12_PX_256_COLOR_BMP_FILES);
                player.SendActionFailed();
                return;
            }

            if ((player.ClanPrivs & L2Clan.CP_CL_REGISTER_CREST) != L2Clan.CP_CL_REGISTER_CREST)
            {
                player.SendSystemMessage(SystemMessage.SystemMessageId.NOT_AUTHORIZED_TO_BESTOW_RIGHTS);
                player.SendActionFailed();
                return;
            }

            clan.updateCrest(_size, _picture);
        }
    }
}