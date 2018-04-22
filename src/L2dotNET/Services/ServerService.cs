﻿using System.Collections.Generic;
using L2dotNET.DataContracts;
using L2dotNET.Repositories.Contracts;
using L2dotNET.Services.Contracts;

namespace L2dotNET.Services
{
    public class ServerService : IServerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<ServerContract> GetServerList()
        {
            return _unitOfWork.ServerRepository.GetServerList();
        }

        public List<int> GetPlayersObjectIdList()
        {
            return _unitOfWork.ServerRepository.GetPlayersObjectIdList();
        }

        public List<int> GetPlayersItemsObjectIdList()
        {
            return _unitOfWork.ServerRepository.GetPlayersItemsObjectIdList();
        }

        public List<AnnouncementContract> GetAnnouncementsList()
        {
            return _unitOfWork.ServerRepository.GetAnnouncementsList();
        }

        public bool CheckDatabaseQuery()
        {
            return _unitOfWork.ServerRepository.CheckDatabaseQuery();
        }

        public List<SpawnlistContract> GetAllSpawns()
        {
            return _unitOfWork.ServerRepository.GetAllSpawns();
        }
    }
}