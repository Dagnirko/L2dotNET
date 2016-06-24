﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using L2dotNET.LoginService.Network;
using L2dotNET.Utility;

namespace L2dotNET.LoginService.Managers
{
    class NetworkRedirect
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkRedirect));
        private static volatile NetworkRedirect instance;
        private static readonly object syncRoot = new object();

        protected List<NetRedClass> redirects = new List<NetRedClass>();
        public NetRedClass GlobalRedirection { get; set; }

        public static NetworkRedirect Instance
        {
            get
            {
                if (instance == null)
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new NetworkRedirect();
                    }

                return instance;
            }
        }

        public NetworkRedirect() { }

        public void Initialize()
        {
            using (StreamReader reader = new StreamReader(new FileInfo(@"sq\server_redirect.txt").FullName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine() ?? string.Empty;
                    if ((line.Length == 0) || line.StartsWithIgnoreCase("//"))
                        continue;

                    NetRedClass i = new NetRedClass();
                    string[] sp = line.Split(' ');
                    i.serverId = short.Parse(sp[0]);
                    i.mask = sp[1];
                    i.setRedirect(sp[2]);

                    if (i.serverId == -1)
                        GlobalRedirection = i;
                    else
                        redirects.Add(i);
                }
            }

            log.Info($"NetworkRedirect: {redirects.Count} redirects. Global is {(GlobalRedirection == null ? "disabled" : "enabled")}");
        }

        public byte[] GetRedirect(LoginClient client, short serverId)
        {
            if (GlobalRedirection != null)
            {
                string[] a = client.Address.ToString().Split(':')[0].Split('.'),
                         b = GlobalRedirection.mask.Split('.');
                byte[] d = new byte[4];
                for (byte c = 0; c < 4; c++)
                {
                    d[c] = 0;

                    if (b[c] == "*")
                        d[c] = 1;
                    else if (b[c] == a[c])
                        d[c] = 1;
                    else if (b[c].Contains("/"))
                    {
                        byte n = byte.Parse(b[c].Split('/')[0]),
                             x = byte.Parse(b[c].Split('/')[1]);
                        byte t = byte.Parse(a[c]);
                        d[c] = ((t >= n) && (t <= x)) ? (byte)1 : (byte)0;
                    }
                }

                if (d.Min() == 1)
                {
                    log.Info($"Redirecting client to global {GlobalRedirection.redirect} on #{serverId}");
                    return GlobalRedirection.redirectBits;
                }
            }
            else
            {
                if (redirects.Count == 0)
                    return null;

                foreach (NetRedClass nr in redirects.Where(nr => nr.serverId == serverId))
                {
                    string[] a = client.Address.ToString().Split(':')[0].Split('.'),
                             b = nr.mask.Split('.');
                    byte[] d = new byte[4];
                    for (byte c = 0; c < 4; c++)
                    {
                        d[c] = 0;

                        if (b[c] == "*")
                            d[c] = 1;
                        else if (b[c] == a[c])
                            d[c] = 1;
                        else if (b[c].Contains("/"))
                        {
                            byte n = byte.Parse(b[c].Split('/')[0]),
                                 x = byte.Parse(b[c].Split('/')[1]);
                            byte t = byte.Parse(a[c]);
                            d[c] = ((t >= n) && (t <= x)) ? (byte)1 : (byte)0;
                        }
                    }

                    if (d.Min() == 1)
                    {
                        log.Info($"Redirecting client to {nr.redirect} on #{serverId}");
                        return nr.redirectBits;
                    }
                }
            }

            return null;
        }
    }
}