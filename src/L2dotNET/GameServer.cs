﻿using System;
using System.Net;
using System.Net.Sockets;
using log4net;
using L2dotNET.controllers;
using L2dotNET.Handlers;
using L2dotNET.managers;
using L2dotNET.model.items;
using L2dotNET.model.npcs.ai;
using L2dotNET.model.quests;
using L2dotNET.Network;
using L2dotNET.Network.loginauth;
using L2dotNET.tables;
using L2dotNET.tables.multisell;
using L2dotNET.Utility;
using L2dotNET.world;
using Ninject;

namespace L2dotNET
{
    public class GameServer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameServer));

        protected TcpListener GameServerListener;

        public static IKernel Kernel { get; set; }

        public void Start()
        {
            Config.Config.Instance.Initialize();

            PreReqValidation.Instance.Initialize();

            CharTemplateTable.Instance.Initialize();

            NetworkBlock.Instance.Initialize();
            GameTime.Instance.Initialize();

            IdFactory.Instance.Initialize();

            L2World.Instance.Initialize();

            MapRegionTable.Instance.Initialize();
            ZoneTable.Instance.Initialize();

            ItemTable.Instance.Initialize();
            ItemHandler.Instance.Initialize();

            NpcTable.Instance.Initialize();
            NpcData.Instance.Initialize();

            MultiSell.Instance.Initialize();
            Capsule.Instance.Initialize();
            RecipeTable.Instance.Initialize();

            AiManager.Instance.Initialize();

            BlowFishKeygen.GenerateKeys();

            AdminCommandHandler.Instance.Initialize();

            QuestManager.Instance.Initialize();

            AnnouncementManager.Instance.Initialize();

            AllianceTable.Instance.Initialize();
            ClanTable.Instance.Initialize();

            StaticObjTable.Instance.Initialize();
            //SpawnTable.getInstance().Spawn();
            StructureTable.Instance.Initialize();

            HtmCache.Instance.Initialize();

           // PluginManager.Instance.Initialize(this);

            AuthThread.Instance.Initialize();

            GameServerListener = new TcpListener(IPAddress.Any, Config.Config.Instance.ServerConfig.Port);

            try
            {
                GameServerListener.Start();
            }
            catch (SocketException ex)
            {
                Log.Error($"Socket Error: '{ex.SocketErrorCode}'. Message: '{ex.Message}' (Error Code: '{ex.NativeErrorCode}')");
                Log.Info("Press ENTER to exit...");
                Console.Read();
                Environment.Exit(0);
            }

            Log.Info($"Listening Gameservers on port {Config.Config.Instance.ServerConfig.Port}");

            while (true)
            {
                TcpClient clientSocket = GameServerListener.AcceptTcpClient();
                Accept(clientSocket);
            }
        }

        private void Accept(TcpClient clientSocket)
        {
            ClientManager.Instance.AddClient(clientSocket);
        }
    }
}