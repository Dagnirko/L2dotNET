﻿using L2dotNET.GameService.Commands;
using L2dotNET.GameService.managers;
using L2dotNET.GameService.model.npcs;
using L2dotNET.GameService.model.structures.conq;
using L2dotNET.GameService.model.vehicles;
using L2dotNET.GameService.model.zones.forms;
using L2dotNET.GameService.network.l2send;
using L2dotNET.GameService.tables;
using L2dotNET.GameService.tables.multisell;
using L2dotNET.GameService.world;
using log4net;
using System;
using System.Threading;

namespace L2dotNET.GameService.Command
{
    class AdminRange : AAdminCommand
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminRange));

        public AdminRange()
        {
            Cmd = "range";
        }
        ZoneNPoly np;
        protected internal override void Use(L2Player admin, string alias)
        {
          //  double dis = Calcs.calculateDistance(admin, admin._currentTarget, true);
          //  admin.sendMessage("dis "+dis);

          //  foreach (L2SkillCoolTime ct in admin._reuse.Values)
          //  {
          //      ct.forcedStop();
          //  }
          //  admin._reuse.Clear();
          //  admin.updateReuse();

            string s = alias.Split(' ')[1];

            if (s == "1")
            {
                admin.AbnormalBitMaskEvent = int.Parse(alias.Split(' ')[2]);
                admin.updateAbnormalEventEffect();
            }
            else if (s == "2")
            {
                int listid = int.Parse(alias.Split(' ')[2]);
                MultiSell.Instance.ShowList(admin, null, listid);
            }
            else if (s == "4")
            {
                FortressOfTheDead d = new FortressOfTheDead();
                d.start();
            }
            else if (s == "5")
            {
                if (np == null)
                {
                    int[] x = new int[] { -81166, -80913, -81952, -82554 };
                    int[] y = new int[] { 245118, 246031, 246551, 245619 };
                    np = new ZoneNPoly(x, y, -3727, -3727);
                }

                int count = int.Parse(alias.Split(' ')[2]);

                for (int i = 0; i < count; i++)
                {
                    int[] rloc = this.rndXYZ();
                   // NpcTable.getInstance().spawnNpc("lector", rloc[0], rloc[1], rloc[3], new Random().Next(65000));
                }

            }
            
           // admin._privateStoreType = byte.Parse(alias.Split(' ')[1]);
           // admin.broadcastUserInfo();
            

           // int val = int.Parse(alias.Split(' ')[1]);

          //  StatusUpdate su = new StatusUpdate(admin);
          //  su.add(val, 5000);
          //  admin.sendPacket(su);
        }
        System.Timers.Timer t;
        L2Player adm;

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //List<int> l = new List<int>();
            //l.Add(adm.ObjID);
            //adm.broadcastPacket(new MagicSkillLaunched(adm, l, 261, 1));
            //t.Enabled = false;
            adm.sendMessage("END!");
        }

        public int[] rndXYZ()
        {
            int i; int[] p = new int[4];
            Random rn = new Random();
            for (i = 0; i < 200; i++)
            {
                p[0] = rn.Next(np.minX, np.maxX);
                p[1] = rn.Next(np.minY, np.maxY);
                log.Info($"rnd xy { p[0] } { p[1] }");
                if (np.isInsideZone(p[0], p[1]))
                {
                    double curdistance = -1;
                    p[2] = np.getLowZ() + 10;
                    p[3] = np.getHighZ();

                    for (i = 0; i < np._x.Length; i++)
                    {
                        int p1x = np._x[i];
                        int p1y = np._y[i];
                        long dx = p1x - p[0], dy = p1y - p[1];
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        if (curdistance == -1 || distance < curdistance)
                        {
                            curdistance = distance;
                            p[2] = np._z1 + 10;
                        }
                    }

                    return p;
                }
            }

            return p;
        }

        public void read()
        {
            Thread.Sleep(1000);
           // _admin.validateVisibleObjects(_admin);
        }
    }
}
