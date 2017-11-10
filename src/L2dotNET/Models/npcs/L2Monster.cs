﻿using L2dotNET.model.npcs;
using L2dotNET.templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2dotNET.model.player;
using L2dotNET.Network.serverpackets;
using log4net.Core;
using log4net;
using L2dotNET.world;
using System.Timers;

namespace L2dotNET.Models.npcs
{
    class L2Monster : L2Npc
    {

        private readonly ILog Log = LogManager.GetLogger(typeof(L2Monster));

        private Timer CorpseTimer;

        public L2Monster(int objectId, NpcTemplate template) : base(objectId, template)
        {
            Template = template;
            Name = template.Name;
            //CStatsInit();
            CurHp = Template.BaseHpMax;
            CurCp = 100;
            CurMp = Template.BaseMpMax;
            MaxCp = 100;
            MaxHp = (int)Template.BaseHpMax;
            MaxMp = (int)Template.BaseMpMax;
        }

        public override void OnAction(L2Player player)
        {
            if (player.Target != this) {
                player.SetTarget(this);
                player.SendPacket(new MyTargetSelected(ObjId, 0));
                return;
            }
            player.MoveTo(X, Y, Z);
            player.SendPacket(new MoveToPawn(player, this, 150));

            player.DoAttack(this);
        }

        public override void DoDie(L2Character killer)
        {
            //Check For Exp
            if(killer is L2Player)
            {
                ((L2Player)killer).AddExpSp(this.Template.Exp, this.Template.Sp, true);
            }
            if (Template.CorpseTime <= 0)
            { return; }
            CorpseTimer = new Timer(Template.CorpseTime * 1000);
            CorpseTimer.Elapsed += new ElapsedEventHandler(RemoveCorpse);
            CorpseTimer.Start();
        }

        private void RemoveCorpse(object sender, ElapsedEventArgs e)
        {
            CorpseTimer.Stop();
            CorpseTimer.Enabled = false;
            BroadcastPacket(new DeleteObject(ObjId));
            L2World.Instance.RemoveObject(this);
        }
    }
}
