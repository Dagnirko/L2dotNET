﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using log4net;
using L2dotNET.GameService.Network.LoginAuth.Send;
using L2dotNET.GameService.World;
using L2dotNET.Models;

namespace L2dotNET.GameService.Network.LoginAuth
{
    public class AuthThread
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthThread));
        private static volatile AuthThread instance;
        private static readonly object syncRoot = new object();

        public static AuthThread Instance
        {
            get
            {
                if (instance == null)
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AuthThread();
                    }

                return instance;
            }
        }

        protected TcpClient lclient;
        protected NetworkStream nstream;
        protected System.Timers.Timer ltimer;
        public bool IsConnected;
        private byte[] buffer;

        public string version = "rcs #216";
        public int build = 0;

        public AuthThread() { }

        public void Initialize()
        {
            IsConnected = false;
            try
            {
                lclient = new TcpClient(Config.Config.Instance.ServerConfig.AuthHost, Config.Config.Instance.ServerConfig.AuthPort);
                nstream = lclient.GetStream();
            }
            catch (SocketException)
            {
                log.Warn("Login server is not responding. Retrying");
                if (ltimer == null)
                {
                    ltimer = new System.Timers.Timer();
                    ltimer.Interval = 2000;
                    ltimer.Elapsed += new System.Timers.ElapsedEventHandler(ltimer_Elapsed);
                }

                if (!ltimer.Enabled)
                    ltimer.Enabled = true;

                return;
            }

            if ((ltimer != null) && ltimer.Enabled)
                ltimer.Enabled = false;

            IsConnected = true;

            sendPacket(new Send.LoginAuth());
            sendPacket(new LoginServPing(this));
            new System.Threading.Thread(read).Start();
        }

        private void ltimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Initialize();
        }

        public void read()
        {
            try
            {
                buffer = new byte[2];
                nstream.BeginRead(buffer, 0, 2, new AsyncCallback(OnReceiveCallbackStatic), null);
            }
            catch (Exception e)
            {
                log.Error($"AuthThread: {e.Message}");
                termination();
            }
        }

        private void OnReceiveCallbackStatic(IAsyncResult result)
        {
            try
            {
                int rs = nstream.EndRead(result);
                if (rs > 0)
                {
                    short length = BitConverter.ToInt16(buffer, 0);
                    buffer = new byte[length];
                    nstream.BeginRead(buffer, 0, length, new AsyncCallback(OnReceiveCallback), result.AsyncState);
                }
            }
            catch (Exception e)
            {
                log.Error($"AuthThread: {e.Message}");
                termination();
            }
        }

        private void OnReceiveCallback(IAsyncResult result)
        {
            nstream.EndRead(result);

            byte[] buff = new byte[buffer.Length];
            buffer.CopyTo(buff, 0);

            PacketHandlerAuth.handlePacket(this, buff);

            new System.Threading.Thread(read).Start();
        }

        private void termination()
        {
            if (paused)
                return;

            log.Error("AuthThread: reconnecting...");
            Initialize();
        }

        public void sendPacket(GameServerNetworkPacket pk)
        {
            pk.write();

            List<byte> blist = new List<byte>();
            byte[] db = pk.ToByteArray();

            short len = (short)db.Length;
            blist.AddRange(BitConverter.GetBytes(len));
            blist.AddRange(db);

            nstream.Write(blist.ToArray(), 0, blist.Count);
            nstream.Flush();
        }

        private bool paused;

        public void loginFail(string code)
        {
            paused = true;
            log.Error($"AuthThread: {code}. Please check configuration, server paused.");
            try
            {
                nstream.Close();
                lclient.Close();
            }
            catch (Exception e)
            {
                log.Error($"AuthThread: {e.Message}");
            }
        }

        public void loginOk(string code)
        {
            log.Info($"AuthThread: {code}");
        }

        public void setInGameAccount(string account, bool status = false)
        {
            sendPacket(new AccountInGame(account, status));
        }

        public void UpdatePlayersOnline()
        {
            short cnt = (short)L2World.Instance.GetPlayers().Count;
            sendPacket(new PlayerCount(cnt));
        }

        private readonly SortedList<string, AccountModel> awaitingAccounts = new SortedList<string, AccountModel>();

        public void awaitAccount(AccountModel ta)
        {
            if (awaitingAccounts.ContainsKey(ta.Login))
                awaitingAccounts.Remove(ta.Login);

            awaitingAccounts.Add(ta.Login, ta);
        }

        public AccountModel getTA(string p)
        {
            if (awaitingAccounts.ContainsKey(p))
            {
                AccountModel ta = awaitingAccounts[p];
                awaitingAccounts.Remove(p);
                return ta;
            }

            return null;
        }
    }
}