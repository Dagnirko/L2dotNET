﻿using L2dotNET.Attributes;
using L2dotNET.Models.items;
using L2dotNET.Models.player;
using L2dotNET.tables;

namespace L2dotNET.Commands.Admin
{
    [Command(CommandName = "summon3")]
    class AdminSpawnItemRange : AAdminCommand
    {
        protected internal override void Use(L2Player admin, string alias)
        {
            int idmin = int.Parse(alias.Split(' ')[1]);
            int idmax = int.Parse(alias.Split(' ')[2]);

            if ((idmax - idmin) > 200)
            {
                admin.SendMessage("Too big id range.");
                return;
            }

            bool x = false;
            for (int i = idmin; i <= idmax; i++)
            {
                ItemTemplate item = ItemTable.Instance.GetItem(i);

                if (item == null)
                    admin.SendMessage($"Item with id {i} not exists.");
                else
                {
                    admin.AddItem(i, 1);
                    x = true;
                }
            }

            if (x)
                admin.SendItemList(true);
        }
    }
}