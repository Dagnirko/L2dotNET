﻿using L2dotNET.GameService.Model.Player;

namespace L2dotNET.GameService.Model.Npcs.Ai.Ex
{
    class ai_looser_of_gracia : AITemplate
    {
        public override void onDialog(L2Player player, int ask, int reply, L2Npc npc)
        {
            switch (ask)
            {
                case -1425:
                {
                    switch (reply)
                    {
                        case 1:
                            if (player.Level < 75)
                                player.ShowHtm(fnLowLevel, npc);
                            else if (player.getAdena() >= 150000)
                                player.InstantTeleportWithItem(-149406, 255247, -85, 57, 150000);
                            else
                                player.ShowHtm(fnNotHaveAdena, npc);
                            break;
                    }
                }
                    break;
            }
        }
    }
}