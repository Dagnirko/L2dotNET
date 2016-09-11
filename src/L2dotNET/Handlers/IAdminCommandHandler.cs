﻿using L2dotNET.Commands;
using L2dotNET.Commands.Admin;
using L2dotNET.model.player;

namespace L2dotNET.Handlers
{
    public interface IAdminCommandHandler
    {
        void Request(L2Player admin, string alias);

        void Register(object processor);
    }
}