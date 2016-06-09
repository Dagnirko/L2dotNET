﻿using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Model.Skills;

namespace L2dotNET.GameService.Network.Clientpackets
{
    class RequestDispel : GameServerNetworkRequest
    {
        private int ownerId;
        private int skillId;
        private int skillLv;

        public RequestDispel(GameClient client, byte[] data)
        {
            makeme(client, data, 2);
        }

        public override void read()
        {
            ownerId = readD();
            skillId = readD();
            skillLv = readD();
        }

        public override void run()
        {
            L2Player player = Client.CurrentPlayer;

            if (ownerId != player.ObjID)
            {
                player.sendActionFailed();
                return;
            }

            AbnormalEffect avestop = null;
            foreach (AbnormalEffect ave in player._effects)
            {
                if ((ave.id != skillId) && (ave.lvl != skillLv))
                    continue;

                if ((ave.skill.debuff == 1) && (ave.skill.is_magic > 1))
                    break;

                avestop = ave;
                break;
            }

            if (avestop == null)
            {
                player.sendActionFailed();
                return;
            }

            lock (player._effects)
            {
                avestop.forcedStop(true, true);
                player._effects.Remove(avestop);
            }
        }
    }
}