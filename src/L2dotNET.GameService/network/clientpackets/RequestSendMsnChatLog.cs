﻿namespace L2dotNET.GameService.Network.Clientpackets
{
    class RequestSendMsnChatLog : GameServerNetworkRequest
    {
        public RequestSendMsnChatLog(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        private string _text,
                       _email;
        private int _type;

        public override void read()
        {
            _text = readS();
            _email = readS();
            _type = readD();
        }

        public override void run()
        {
            //            L2Player player = getClient()._player;

            //todo log
        }
    }
}