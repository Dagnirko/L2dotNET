﻿using System;
using L2dotNET.GameService.Model.Player;

namespace L2dotNET.GameService.Managers
{
    class PetitionManager
    {
        private static readonly PetitionManager instance = new PetitionManager();

        public static PetitionManager getInstance()
        {
            return instance;
        }

        internal void petitionlink(L2Player player, string p)
        {
            throw new NotImplementedException();
        }
    }
}