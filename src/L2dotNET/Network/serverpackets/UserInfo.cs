﻿using L2dotNET.model.inventory;
using L2dotNET.model.player;
using L2dotNET.model.skills2;

namespace L2dotNET.Network.serverpackets
{
    class UserInfo : GameserverPacket
    {
        private readonly L2Player _player;

        public UserInfo(L2Player player)
        {
            _player = player;
        }

        //TODO: Simplify method body
        public override void Write()
        {
            WriteByte(0x04);

            WriteInt(_player.X);
            WriteInt(_player.Y);
            WriteInt(_player.Z);
            WriteInt(_player.Heading);
            WriteInt(_player.ObjId);

            WriteString(_player.Name);

            WriteInt((int)_player.BaseClass.ClassId.ClassRace);
            WriteInt((int)_player.Sex);
            WriteInt((int)_player.BaseClass.ClassId.Id);
            WriteInt(_player.Level);
            WriteLong(_player.Exp);

            WriteInt(_player.Str);
            WriteInt(_player.Dex);
            WriteInt(_player.Con);
            WriteInt(_player.Int);
            WriteInt(_player.Wit);
            WriteInt(_player.Men);

            WriteInt(_player.MaxHp); //max hp
            WriteInt(_player.CurHp);
            WriteInt(_player.MaxMp); //max mp
            WriteInt(_player.CurMp);
            WriteInt(_player.Sp);
            WriteInt(_player.CurrentWeight);
            WriteInt(_player.CharacterStat.GetStat(EffectType.BMaxWeight));

            WriteInt(_player.Inventory.GetPaperdollItem(Inventory.PaperdollRhand) != null ? 40 : 20); // 20 no weapon, 40 weapon equipped

            for (byte id = 0; id < Inventory.PaperdollTotalslots; id++)
                WriteInt(_player.Inventory.Paperdoll[id]?.ObjId ?? 0);

            for (byte id = 0; id < Inventory.PaperdollTotalslots; id++)
                WriteInt(_player.Inventory.Paperdoll[id]?.Template?.ItemId ?? 0);

            // c6 new h's
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteInt(0x00); //player.Inventory.getPaperdollAugmentId(InvPC.EQUIPITEM_RHand));
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteInt(0x00); //player.Inventory.getPaperdollAugmentId(InvPC.EQUIPITEM_LHand));
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);
            WriteShort(0x00);

            double atkspd = _player.CharacterStat.GetStat(EffectType.BAttackSpd);
            WriteInt(_player.CharacterStat.GetStat(EffectType.PPhysicalAttack));
            WriteInt(atkspd);
            WriteInt(_player.CharacterStat.GetStat(EffectType.PPhysicalDefense));
            WriteInt(_player.CharacterStat.GetStat(EffectType.BEvasion));
            WriteInt(_player.CharacterStat.GetStat(EffectType.BAccuracy));
            WriteInt(_player.CharacterStat.GetStat(EffectType.BCriticalRate));
            WriteInt(_player.CharacterStat.GetStat(EffectType.PMagicalAttack));
            WriteInt(_player.CharacterStat.GetStat(EffectType.BCastingSpd));
            WriteInt(atkspd); //? еще раз?
            WriteInt(_player.CharacterStat.GetStat(EffectType.PMagicalDefense));

            WriteInt(_player.PvPStatus);
            WriteInt(_player.Karma);

            double spd = _player.CharacterStat.GetStat(EffectType.PSpeed);

            double anim = (spd * 1f) / 130;
            //double anim2 = (1.1) * atkspd / 300;
            double runSpd = spd / anim;
            double walkSpd = (spd * .8) / anim;

            WriteInt(runSpd);
            WriteInt(walkSpd);
            WriteInt(50); // swimspeed
            WriteInt(50); // swimspeed
            WriteInt(0); //?
            WriteInt(0); //?
            WriteInt(runSpd); //fly run
            WriteInt(walkSpd); //fly walk ?
            WriteDouble(1); //run speed multiplier
            WriteDouble(1); //atk speed multiplier

            WriteDouble(_player.Radius);
            WriteDouble(_player.Height);

            WriteInt((int)_player.HairStyleId);
            WriteInt((int)_player.HairColor);
            WriteInt((int)_player.Face);
            WriteInt(_player.Builder);

            WriteString(_player.Title);

            WriteInt(_player.ClanId);
            WriteInt(_player.ClanCrestId);
            WriteInt(_player.AllianceId);
            WriteInt(_player.AllianceCrestId);

            WriteInt(_player.Sstt); //_relation
            WriteByte(_player.MountType);
            WriteByte(_player.GetPrivateStoreType()); //
            WriteByte(_player.PCreateItem > 0 ? 1 : 0);
            WriteInt(_player.PkKills);
            WriteInt(_player.PvpKills);

            WriteShort(_player.Cubics.Count);

            _player.Cubics.ForEach(cub => WriteShort(cub.Template.Id));

            WriteByte(0); //1-isInPartyMatchRoom

            WriteInt(_player.AbnormalBitMask);

            //byte flymode = 0;

            //if (player.TransformID > 0)
            //    flymode = player.Transform.Template.MoveMode;

            WriteByte(0x00);

            WriteInt(_player.ClanPrivs);

            WriteShort(_player.RecHave); //c2  recommendations remaining
            WriteShort(_player.RecLeft); //c2  recommendations received
            WriteInt(_player.MountType > 0 ? _player.MountedTemplate.NpcId + 1000000 : 0); //moun t npcid
            WriteShort(_player.ItemLimitInventory);

            WriteInt((int)_player.ActiveClass.ClassId.Id);
            WriteInt(0); // special effects? circles around player...
            WriteInt(_player.CurCp); //max cp
            WriteInt(_player.CurCp);
            WriteByte(_player.GetEnchantValue());
            WriteByte(_player.TeamId);
            WriteInt(0);//_player.GetClanCrestLargeId()
            WriteByte(_player.Noblesse);

            byte hero = _player.Heroic;
            WriteByte(hero);

            WriteByte(_player.IsFishing() ? 0x01 : 0x00); //Fishing Mode
            WriteInt(_player.GetFishx()); //fishing x
            WriteInt(_player.GetFishy()); //fishing y
            WriteInt(_player.GetFishz()); //fishing z
            WriteInt(_player.GetNameColor());

            WriteByte(_player.IsRunning);

            WriteInt(0);//_player.ClanRank()
            WriteInt(_player.ClanType);

            WriteInt(_player.GetTitleColor());
            WriteInt(_player.CursedWeaponLevel);
        }
    }
}