﻿using L2dotNET.GameService.Model.player;
using L2dotNET.GameService.Model.skills2;

namespace L2dotNET.GameService.Commands.Admin
{
    class AdminAddSkill : AAdminCommand
    {
        public AdminAddSkill()
        {
            Cmd = "setskill";
        }

        protected internal override void Use(L2Player admin, string alias)
        {
            //setskill [skill_id] [skill_lvl] -- дает скилл [skill_id] уровня [skill_lvl] выбранному чару
            int id = int.Parse(alias.Split(' ')[1]);
            int lvl = int.Parse(alias.Split(' ')[2]);

            TSkill skill = TSkillTable.Instance.Get(id, lvl);

            if (skill == null)
                admin.sendMessage("Skill " + id + "/" + lvl + " is missing.");
            else
            {
                L2Player target;
                if (admin.CurrentTarget != null && admin.CurrentTarget is L2Player)
                {
                    target = (L2Player)admin.CurrentTarget;
                }
                else
                    target = admin;

                target.addSkill(skill, true, true);
                admin.sendMessage(target.Name + " has received skill " + id + "/" + lvl);
            }
        }
    }
}