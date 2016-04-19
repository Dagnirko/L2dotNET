﻿using L2dotNET.Game.network.l2send;

namespace L2dotNET.Game.network.l2recv
{
    class RequestShowMiniMap : GameServerNetworkRequest
    {
        public RequestShowMiniMap(GameClient client, byte[] data)
        {
            base.makeme(client, data);
        }

        public override void read()
        {
            // do nothing
        }

        public override void run()
        {
            Client.sendPacket(new ShowMiniMap());
        }
    }
}
