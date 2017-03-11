﻿using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using log4net;
using L2dotNET.Models;
using L2dotNET.Repositories.Contracts;
using MySql.Data.MySqlClient;

namespace L2dotNET.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerRepository));

        internal IDbConnection Db;

        public SkillRepository()
        {
            Db = new MySqlConnection(ConfigurationManager.ConnectionStrings["PrimaryConnection"].ToString());
        }

        public List<SkillResponseModel> GetPlayerSkills(int charID)
        {
            try
            {
                const string sql = @"select ownerId as CharObjId, id as skillId, lvl as SkillLvl, iclass as ClassId from user_skills where ownerId = @char_obj_id";
                return Db.Query<SkillResponseModel>(sql, new { char_obj_id = charID }).ToList();
            }
            catch (MySqlException ex)
            {
                Log.Error($"Method: {nameof(GetPlayerSkills)}. Message: '{ex.Message}' (Error Number: '{ex.Number}')");
                return new List<SkillResponseModel>();
            }
        }
    }
}