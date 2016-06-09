﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using log4net;
using L2dotNET.LoginService.Model;
using L2dotNET.LoginService.Network;
using L2dotNET.Models;
using L2dotNET.Services.Contracts;
using Ninject;

namespace L2dotNET.LoginService.GSCommunication
{
    public class ServerThreadPool
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerThreadPool));

        [Inject]
        public IServerService serverService
        {
            get { return LoginServer.Kernel.Get<IServerService>(); }
        }

        private static volatile ServerThreadPool instance;
        private static readonly object syncRoot = new object();

        public List<L2Server> servers = new List<L2Server>();

        public static ServerThreadPool Instance
        {
            get
            {
                if (instance == null)
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServerThreadPool();
                    }

                return instance;
            }
        }

        public ServerThreadPool() { }

        public void Initialize()
        {
            List<ServerModel> serverModels = serverService.GetServerList();

            foreach (ServerModel curServ in serverModels)
            {
                L2Server server = new L2Server();
                server.Id = (byte)curServ.Id;
                server.Info = curServ.Name;
                server.Code = curServ.Code;
                servers.Add(server);
            }

            log.Info($"GameServerThread: loaded {servers.Count} servers");
        }

        public L2Server Get(short serverId)
        {
            return servers.FirstOrDefault(s => s.Id == serverId);
        }

        protected TcpListener listener;

        public void Start()
        {
            listener = new TcpListener(IPAddress.Parse(Config.Config.Instance.serverConfig.Host), Config.Config.Instance.serverConfig.GSPort);
            listener.Start();
            log.Info($"Auth server listening gameservers at {Config.Config.Instance.serverConfig.Host}:{Config.Config.Instance.serverConfig.GSPort}");
            while (true)
                VerifyClient(listener.AcceptTcpClient());
        }

        private void VerifyClient(TcpClient client)
        {
            ServerThread st = new ServerThread();
            st.ReadData(client, this);
        }

        public void Shutdown(byte id)
        {
            foreach (L2Server s in servers.Where(s => s.Id == id))
            {
                if (s.Thread != null)
                    s.Thread.Stop();

                s.Thread = null;
                log.Warn($"ServerThread: #{id} shutted down");
                break;
            }
        }

        public bool LoggedAlready(string account)
        {
            foreach (L2Server srv in servers.Where(srv => (srv.Thread != null) && srv.Thread.LoggedAlready(account)))
            {
                srv.Thread.KickAccount(account);
                return true;
            }

            return false;
        }

        public void SendPlayer(byte serverId, LoginClient client, string time)
        {
            foreach (L2Server srv in servers.Where(srv => (srv.Id == serverId) && (srv.Thread != null)))
            {
                srv.Thread.SendPlayer(client, time);
                break;
            }
        }
    }
}