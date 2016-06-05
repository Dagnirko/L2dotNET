﻿using L2dotNET.GameService.model.npcs;

namespace L2dotNET.GameService.model.quests.data
{
    class _0012_Secret_Meeting_With_Varka_Silenos : QuestOrigin
    {
        const int guard_cadmon = 31296;
        const int trader_helmut = 31258;
        const int herald_naran = 31378;

        const int q_cargo_for_barka = 7232;

        public _0012_Secret_Meeting_With_Varka_Silenos()
        {
            questId = 12;
            questName = "Secret Meeting With Varka Silenos";
            startNpc = guard_cadmon;
            talkNpcs = new int[] { startNpc, trader_helmut, herald_naran };
            actItems = new int[] { q_cargo_for_barka };
        }

        public override void tryAccept(L2Player player, L2Npc npc)
        {
            if (player.Level >= 74)
                player.ShowHtm("guard_cadmon_q0012_0101.htm", npc);
            else
            {
                player.ShowHtm("guard_cadmon_q0012_0103.htm", npc);
            }
        }

        public override void onAccept(L2Player player, L2Npc npc)
        {
            player.questAccept(new QuestInfo(this));
            player.ShowHtm("guard_cadmon_q0012_0104.htm", npc);
        }

        public override void onTalkToNpcQM(L2Player player, L2Npc npc, int reply)
        {
            //todo
        }

        public override void onTalkToNpc(L2Player player, L2Npc npc, int cond)
        {
            int npcId = npc.Template.NpcId;
            string htmltext = no_action_required;
            if (npcId == guard_cadmon)
            {
                switch (cond)
                {
                    case 1:
                        htmltext = "guard_cadmon_q0012_0105.htm";
                        break;
                }
            }
            else if (npcId == trader_helmut)
            {
                switch (cond)
                {
                    case 1:
                        htmltext = "trader_helmut_q0012_0101.htm";
                        break;
                    case 2:
                        htmltext = "trader_helmut_q0012_0202.htm";
                        break;
                }
            }
            else if (npcId == herald_naran)
            {
                switch (cond)
                {
                    case 2:
                        if (player.hasItem(q_cargo_for_barka))
                            htmltext = "herald_naran_q0012_0201.htm";
                        break;
                }
            }

            player.ShowHtm(htmltext, npc);
        }


        public override void onEarnItem(L2Player player, int cond, int id)
        {
            //todo
        }
    }
}

