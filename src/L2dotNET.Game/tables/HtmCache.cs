﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.IO.Compression;
using System.Linq;
using log4net;

namespace L2dotNET.Game.tables
{
    public class HtmCache
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HtmCache));
        private static volatile HtmCache instance;
        private static object syncRoot = new object();

        private List<L2Html> htmCache;
        private List<string> htmFiles;

        public static HtmCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new HtmCache();
                        }
                    }
                }

                return instance;
            }
        }

        public HtmCache()
        {
            
        }

        public void Initialize()
        {
            htmCache = new List<L2Html>();
            htmFiles = DirSearch("./html/");
            BuildHtmCache();
            log.Info($"HtmCache: Cache Built. Loaded { htmCache.Count } files.");
        }

        public void BuildHtmCache()
        {
            foreach(string file in htmFiles)
            {
                string content = File.ReadAllText(file, Encoding.UTF8);
                content = content.Replace("\r\n", "\n");
                htmCache.Add(new L2Html(Path.GetFileNameWithoutExtension(file), content, file));
            }
        }

        public string GetHtmByFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return string.Empty;

            string content = htmCache.FirstOrDefault(x => x.Filename.Equals(filename, StringComparison.InvariantCultureIgnoreCase)).Content;

            return content;
        }

        private List<string> DirSearch(string sDir)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (Exception excpt)
            {
                log.Error(excpt.Message);
            }

            return files;
        }
    }

    public class L2Html
    {
        public string Filename { get; set; }
        public string Content { get; set; }
        public string Filepath { get; set; }

        public L2Html(string filename, string content, string filepath)
        {
            Filename = filename;
            Content = content;
            Filepath = filepath.Replace("\\", "/");
        }
    }
}
