﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using L2dotNET.LoginService.GSCommunication;
using L2dotNET.LoginService.Managers;
using Ninject;

namespace L2dotNET.LoginService
{
    class LoginServer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginServer));

        private TcpListener _listener;

        public static IKernel Kernel { get; set; }

        public void Start()
        {
            CheckRunningProcesses();

            Config.Config.Instance.Initialize();
            PreReqValidation.Instance.Initialize();
            ClientManager.Instance.Initialize();
            ServerThreadPool.Instance.Initialize();
            NetworkRedirect.Instance.Initialize();

            _listener = new TcpListener(IPAddress.Parse(Config.Config.Instance.ServerConfig.Host), Config.Config.Instance.ServerConfig.LoginPort);

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

            Log.Info($"Auth server listening clients at {Config.Config.Instance.ServerConfig.Host}:{Config.Config.Instance.ServerConfig.LoginPort}");
            new Thread(ServerThreadPool.Instance.Start).Start();

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

        private void CheckRunningProcesses()
        {
            if (!Process.GetProcessesByName("L2dotNET.LoginService").Any())
                return;

            Log.Fatal("A L2dotNET.LoginService process is already running!");
            Log.Info("Press ENTER to exit...");
            Console.Read();
            Environment.Exit(0);
        }
    }
}