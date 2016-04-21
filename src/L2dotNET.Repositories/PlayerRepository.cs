﻿using L2dotNET.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace L2dotNET.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        internal IDbConnection db;

        public PlayerRepository()
        {
            this.db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnectionString"].ToString()); // to be changed
        }

        //DummyMethod
        public int GetDeviceIdByPlayerName(string name)
        {
            return this.db.Query<int>("SELECT DeviceId FROM Players WHERE Name=@name", new { name = name }).SingleOrDefault();
        }
    }
}
