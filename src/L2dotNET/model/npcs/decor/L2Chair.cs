﻿using L2dotNET.tables;
using L2dotNET.templates;

namespace L2dotNET.model.npcs.decor
{
    public sealed class L2Chair : L2StaticObject
    {
        public bool IsUsedAlready = false;

        public L2Chair(int objectId, CharTemplate template) : base(objectId, template)
        {
            Closed = 0;
            MaxHp = 0;
            CurHp = 0;
        }

        public override string AsString()
        {
            return $"L2Chair:{ObjId} {StaticId} {ClanID}";
        }
    }
}