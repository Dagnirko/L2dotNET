﻿using L2dotNET.Auth.data;
using L2dotNET.Auth.gscommunication;
using L2dotNET.Auth.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using log4net;

namespace L2dotNET.Auth
{
    class LoginServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoginServer));

        public LoginServer()
        { }

        protected TcpListener LoginListener;
        public static IKernel Kernel { get; set; }

        public void Start()
        {
            Console.Title = "L2dotNET LoginServer";

            Config.Instance.Initialize();
            ClientManager.Instance.Initialize();
            ServerThreadPool.Instance.Initialize();
            NetworkRedirect.Instance.Initialize();

            LoginListener = new TcpListener(IPAddress.Parse(Config.Instance.serverConfig.Host), Config.Instance.serverConfig.LoginPort);

            bool isListening = false;
            try
            {
                LoginListener.Start();
                isListening = true;
            }
            catch (SocketException ex)
            {
                log.Error($"Socket Error: '{ ex.SocketErrorCode }'. Message: '{ ex.Message }' (Error Code: '{ ex.NativeErrorCode }')");
            }

            if (isListening)
            {
                log.Info($"Auth server listening clients at { Config.Instance.serverConfig.Host }:{ Config.Instance.serverConfig.LoginPort }");
                new System.Threading.Thread(ServerThreadPool.Instance.Start).Start();

                TcpClient clientSocket = default(TcpClient);
                while (true)
                {
                    clientSocket = LoginListener.AcceptTcpClient();
                    AcceptClient(clientSocket);
                }
            }
        }

        private void AcceptClient(TcpClient client)
        {
            ClientManager.Instance.addClient(client);
        }
    }
}
