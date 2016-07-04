﻿using System;
using System.Runtime.Remoting.Contexts;
using log4net;
using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Network;
using L2dotNET.GameService.Network.Serverpackets;
using L2dotNET.GameService.World;

namespace L2dotNET.GameService.Controllers
{
    [Synchronization]
    public class GameTime
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameTime));

        private static volatile GameTime _instance;
        private static readonly object SyncRoot = new object();

        public static GameTime Instance
        {
            get
            {
                if (_instance == null)
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new GameTime();
                    }

                return _instance;
            }
        }

        private int _time;
        private readonly GameServerNetworkPacket _dayPk = new SunRise();
        private readonly GameServerNetworkPacket _nightPk = new SunSet();
        private System.Timers.Timer _timeController;
        public DateTime ServerStartUp;
        public static bool Night;

        private const int SecDay = 10800,
                          SecNight = 3600,
                          SecHour = 600,
                          SecDn = 14400;
        private const int SecScale = 1800;

        public GameTime() { }

        public void Initialize()
        {
            ServerStartUp = DateTime.Now;
            _time = 5800 + 0; // 10800 18:00 вечер
            _timeController = new System.Timers.Timer();
            _timeController.Interval = 1000;
            _timeController.Enabled = true;
            _timeController.Elapsed += new System.Timers.ElapsedEventHandler(ActionTime);

            Log.Info("GameTime Controller: started 18:00 PM.");
        }

        private void ActionTime(object sender, System.Timers.ElapsedEventArgs e)
        {
            _time++;

            switch (_time)
            {
                case SecDay + SecScale: // 21:00
                    NotifyStartNight();
                    break;
                case SecScale: // 03:00
                    NotifyStartDay();
                    break;
            }

            if (_time == SecDn)
                _time = 0;
        }

        private void NotifyStartDay()
        {
            Night = false;

            foreach (L2Player p in L2World.Instance.GetPlayers())
                p.NotifyDayChange(_dayPk);
        }

        private void NotifyStartNight()
        {
            Night = true;

            foreach (L2Player p in L2World.Instance.GetPlayers())
                p.NotifyDayChange(_nightPk);
        }

        public void EnterWorld(L2Player p)
        {
            p.NotifyDayChange(Night ? _nightPk : _dayPk);
        }

        public void ShowInfo(L2Player player)
        {
            DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0).AddSeconds(_time * 6);

            SystemMessage sm = new SystemMessage(Night ? SystemMessage.SystemMessageId.TIME_S1_S2_IN_THE_NIGHT : SystemMessage.SystemMessageId.TIME_S1_S2_IN_THE_DAY);
            sm.AddString(dt.Hour < 10 ? "0" + dt.Hour : "" + dt.Hour);
            string str = dt.Minute < 10 ? "0" + dt.Minute : "" + dt.Minute;
            str += ":";
            str += dt.Second < 10 ? "0" + dt.Second : "" + dt.Second;
            sm.AddString(str);
            player.SendPacket(sm);
        }
    }
}