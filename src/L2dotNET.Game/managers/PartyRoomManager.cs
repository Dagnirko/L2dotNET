﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2dotNET.Game.model.player;

namespace L2dotNET.Game.managers
{
    public class PartyRoomManager
    {
        private static PartyRoomManager m = new PartyRoomManager();

        public static PartyRoomManager getInstance()
        {
            return m;
        }

        public SortedList<int, L2PartyRoom> _rooms = new SortedList<int, L2PartyRoom>();
        public static int _idFactory = 20;

        public L2PartyRoom newRoom(L2Player player, int _roomId, int _maxMembers, int _minLevel, int _maxLevel, int _lootDist, string _roomTitle)
        {
            L2PartyRoom room = new L2PartyRoom();
            room._roomId = _roomId;
            room._maxMembers = _maxMembers;
            room._minLevel = _minLevel;
            room._maxLevel = _maxLevel;
            room._lootDist = _lootDist;
            room._title = _roomTitle;
            room._leaderId = player.ObjID;
            _idFactory++;
            _rooms.Add(_roomId, room);

            return room;
        }
    }
}
