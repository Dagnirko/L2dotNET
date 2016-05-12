﻿using System;
using L2dotNET.GameService.tables;

namespace L2dotNET.GameService.network.l2recv
{
    class RequestTutorialPassCmdToServer : GameServerNetworkRequest
    {
        public RequestTutorialPassCmdToServer(GameClient client, byte[] data)
        {
            base.makeme(client, data);
        }

        private string _alias;
        public override void read()
        {
            _alias = readS();
            if (_alias.Contains("\n"))
                _alias = _alias.Replace("\n", "");
        }

        public override void run()
        {
            L2Player player = getClient().CurrentPlayer;

            if (player._p_block_act == 1)
            {
                player.sendActionFailed();
                return;
            }

            if (_alias.StartsWith("menu_select?"))
            {
                _alias = _alias.Replace(" ", "");
                string x1 = _alias.Split('?')[1];
                string[] x2 = x1.Split('&');
                int ask = int.Parse(x2[0].Substring(4));
                int reply = int.Parse(x2[1].Substring(6));

                //  npc.onDialog(player, ask, reply);
            }
            else if (_alias.StartsWith("admin?"))
            {
                if (player.ViewingAdminPage == 0)
                {
                    player.sendActionFailed();
                    return;
                }

                if (_alias.Contains("tp"))
                {
                    string[] coord = _alias.Split(' ');
                    int x, y, z;
                    if (!int.TryParse(coord[1], out x) || !int.TryParse(coord[2], out y) || !int.TryParse(coord[3], out z))
                    {
                        player.sendMessage("Only numbers allowed in box.");
                        return;
                    }

                    AdminAccess.Instance.ProcessBypassTp(player, x, y, z);
                }
                else
                {
                    string x1 = _alias.Split('?')[1];
                    string[] x2 = x1.Split('&');
                    int ask = int.Parse(x2[0].Substring(4));
                    int reply = int.Parse(x2[1].Substring(6));

                    AdminAccess.Instance.ProcessBypass(player, ask, reply);
                }
            }
        }
    }
}
