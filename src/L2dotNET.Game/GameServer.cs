﻿using L2dotNET.Game.controllers;
using L2dotNET.Game.crypt;
using L2dotNET.Game.geo;
using L2dotNET.Game.Managers;
using L2dotNET.Game.model.events;
using L2dotNET.Game.model.items;
using L2dotNET.Game.model.npcs.ai;
using L2dotNET.Game.model.quests;
using L2dotNET.Game.network;
using L2dotNET.Game.network.loginauth;
using L2dotNET.Game.tables;
using L2dotNET.Game.tables.multisell;
using L2dotNET.Game.world;
using log4net;
using Ninject;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace L2dotNET.Game
{
    class GameServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GameServer));

        protected TcpListener GameServerListener;

        public static IKernel Kernel { get; set; }

        public GameServer()
        { }

        public void Start()
        {
            Console.Title = "L2dotNET GameServer";

            Config.Instance.Initialize();

            CharTemplateTable.Instance.Initialize();

            NetworkBlock.Instance.Initialize();
            GameTime.Instance.Initialize();

            IdFactory.Instance.Initialize();

            L2World.Instance.Initialize();

            MapRegionTable.Instance.Initialize();
            ZoneTable.Instance.Initialize();

            NpcTable.Instance.Initialize();
            NpcData.Instance.Initialize();
            //SpawnTable.Instance.Initialize();

            //TSkillTable.Instance.Initialize();
            ItemTable.getInstance();
            ItemHandler.Instance.Initialize();

            MultiSell.Instance.Initialize();
            Capsule.Instance.Initialize();
            RecipeTable.Instance.Initialize();

            AIManager.Instance.Initialize();

            BlowFishKeygen.GenerateKeys();

            AdminAccess.Instance.Initialize();

            QuestManager.getInstance();

            AnnouncementManager.Instance.Initialize();

            AllianceTable.getInstance();
            ClanTable.getInstance();

            log.Info("NpcServer: ");
            StaticObjTable.Instance.Initialize();
            MonsterRace.Instance.Initialize();
            //SpawnTable.getInstance().Spawn();
            StructureTable.Instance.Initialize();

            HtmCache.Instance.Initialize();

            AuthThread.Instance.Initialize();

            Geodata.Initialize();

            GameServerListener = new TcpListener(IPAddress.Any, Config.Instance.serverConfig.Port);

            try { GameServerListener.Start(); }
            catch (SocketException ex)
            {
                log.Error($"Socket Error: '{ ex.SocketErrorCode }'. Message: '{ ex.Message }' (Error Code: '{ ex.NativeErrorCode }')");
                log.Info($"Press ENTER to exit...");
                Console.Read();
                Environment.Exit(0);
            }

            log.Info($"Listening Gameservers on port { Config.Instance.serverConfig.Port }");

            TcpClient clientSocket = default(TcpClient);
            while (true)
            {
                clientSocket = GameServerListener.AcceptTcpClient();
                Accept(clientSocket);
            }
        }

        private void Accept(TcpClient clientSocket)
        {
            ClientManager.Instance.addClient(clientSocket);
        }
    }
}
