﻿using L2dotNET.Game.model.npcs;
using L2dotNET.Game.world;

namespace L2dotNET.Game.network.l2recv
{
    class AttackRequest : GameServerNetworkRequest
    {
        public AttackRequest(GameClient client, byte[] data)
        {
            base.makeme(client, data);
        }

        private int _objectId;
        private int _originX;
        private int _originY;
        private int _originZ;
        private int _attackId;

        public override void read()
        {
            _objectId = readD();
            _originX = readD();
            _originY = readD();
            _originZ = readD();
            _attackId = readC(); 	 // 0 for simple click   1 for shift-click
        }

        public override void run()
        {
            L2Player player = getClient().CurrentPlayer;

            if (player._p_block_act == 1)
            {
                player.sendActionFailed();
                return;
            }

            L2Object obj = null;

            if (_objectId == player.ObjID)
            {
                player.sendSystemMessage(51);//You cannot use this on yourself.
                player.sendActionFailed();
                return;
            }
            else
                obj = player.knownObjects[_objectId];

            if (obj == null)
            {
                player.sendActionFailed();
                return;
            }

            //if (obj is L2Citizen)
            //{
            //    if (((L2Citizen)obj).Template._can_be_attacked == 0)
            //    {
            //        player.sendSystemMessage(144);//That is the incorrect target.
            //        player.sendActionFailed();
            //        return;
            //    }
            //}

            obj.onForcedAttack(player);
        }
    }
}
