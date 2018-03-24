﻿using System;
using System.Net;
using System.Net.Sockets;
using log4net;
using L2dotNET.Controllers;
using L2dotNET.Handlers;
using L2dotNET.managers;
using L2dotNET.Models.items;
using L2dotNET.Network;
using L2dotNET.Network.loginauth;
using L2dotNET.tables;
using L2dotNET.Utility;
using L2dotNET.world;
using Ninject;

namespace L2dotNET
{
    public class GameServer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameServer));

        private TcpListener _listener;

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
            Capsule.Instance.Initialize();
            
            BlowFishKeygen.GenerateKeys();

            AdminCommandHandler.Instance.Initialize();

            AnnouncementManager.Instance.Initialize();

            StaticObjTable.Instance.Initialize();
            SpawnTable.Instance.Initialize();

            HtmCache.Instance.Initialize();

            // PluginManager.Instance.Initialize(this);

            AuthThread.Instance.Initialize();

            _listener = new TcpListener(IPAddress.Any, Config.Config.Instance.ServerConfig.Port);

            try
            {
                _listener.Start();
            }
            catch (SocketException ex)
            {
                Log.Error($"Socket Error: '{ex.SocketErrorCode}'. Message: '{ex.Message}' (Error Code: '{ex.NativeErrorCode}')");
                Log.Info("Press ENTER to exit...");
                Console.Read();
                Environment.Exit(0);
            }

            Log.Info($"Listening Gameservers on port {Config.Config.Instance.ServerConfig.Port}");

            WaitForClients();
        }

        private void WaitForClients()
        {
            _listener.BeginAcceptTcpClient(OnClientConnected, null);
        }

        private void OnClientConnected(IAsyncResult asyncResult)
        {
            TcpClient clientSocket = _listener.EndAcceptTcpClient(asyncResult);

            Log.Info($"Received connection request from: {clientSocket.Client.RemoteEndPoint}");

            AcceptClient(clientSocket);

            WaitForClients();
        }

        /// <summary>Handle Client Request</summary>
        private void AcceptClient(TcpClient clientSocket)
        {
            ClientManager.Instance.AddClient(clientSocket);
        }
    }
}