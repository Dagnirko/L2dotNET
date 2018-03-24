﻿using L2dotNET.Models.player;

namespace L2dotNET.Models.items.effects
{
    class Calculator : ItemEffect
    {
        public Calculator()
        {
            Ids = new[] { 4393 }; //Calculator
        }

        public override void UsePlayer(L2Player player, L2Item item)
        {
            player.SendPacket(new Network.serverpackets.Calculator());
        }
    }
}