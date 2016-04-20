﻿using L2dotNET.Game.model.items;
using System.Collections.Generic;

namespace L2dotNET.Game.network.l2send
{
    class SellList : GameServerNetworkPacket
    {
        List<L2Item> _sells = new List<L2Item>();
        private long _adena;
        public SellList(L2Player player, int npcObj)
        {
            foreach (L2Item item in player.getAllItems())
            {
                if (item.Template.is_trade == 0 || item.AugmentationID > 0 || item._isEquipped == 1)
                    continue;

                if (item.Template.Type == ItemTemplate.L2ItemType.asset)
                    continue;

                _sells.Add(item);
            }

            _adena = player.getAdena();
        }

        protected internal override void write()
        {
            writeC(0x10);
            writeD(_adena);
            writeD(0);
            writeH(_sells.Count);

            foreach (L2Item item in _sells)
            {
                writeD(item.ObjID);
                writeD(item.Template.ItemID);
                writeQ(item.Count);

                writeH(item.Template.Type2());
                writeH(item.Template.Type1());
                writeD(item.Template.BodyPartId());

                writeH(item.Enchant);
                writeH(item.Template.Type2());
                writeH(0x00);
                writeD((int)(item.Template.Price * 0.5));
            }
        }
    }
}
