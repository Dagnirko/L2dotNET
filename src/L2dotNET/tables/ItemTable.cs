﻿using System;
using System.Collections.Generic;
using log4net;
using L2dotNET.Enums;
using L2dotNET.model.items;
using L2dotNET.model.player;
using L2dotNET.Models;
using L2dotNET.Services.Contracts;
using L2dotNET.templates;
using L2dotNET.Utility;
using L2dotNET.world;
using Ninject;

namespace L2dotNET.tables
{
    public class ItemTable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ItemTable));

        [Inject]
        public IItemService ItemService => GameServer.Kernel.Get<IItemService>();

        internal ItemTemplate GetItem(int id)
        {
            if (Armors.ContainsKey(id))
                return Armors[id];

            if (Weapons.ContainsKey(id))
                return Weapons[id];

            if (EtcItems.ContainsKey(id))
                return EtcItems[id];

            return null;
        }

        private static volatile ItemTable _instance;
        private static readonly object SyncRoot = new object();
        public Dictionary<string, int> Slots = new Dictionary<string, int>();
        public Dictionary<int, Armor> Armors = new Dictionary<int, Armor>();
        public Dictionary<int, Weapon> Weapons = new Dictionary<int, Weapon>();
        public Dictionary<int, EtcItem> EtcItems = new Dictionary<int, EtcItem>();

        public static ItemTable Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new ItemTable();
                }

                return _instance;
            }
        }

        public void Initialize()
        {
            Slots.Add("chest", ItemTemplate.SlotChest);
            Slots.Add("fullarmor", ItemTemplate.SlotFullArmor);
            Slots.Add("alldress", ItemTemplate.SlotAlldress);
            Slots.Add("head", ItemTemplate.SlotHead);
            Slots.Add("hair", ItemTemplate.SlotHair);
            Slots.Add("face", ItemTemplate.SlotFace);
            Slots.Add("dhair", ItemTemplate.SlotHairall);
            Slots.Add("underwear", ItemTemplate.SlotUnderwear);
            Slots.Add("back", ItemTemplate.SlotBack);
            Slots.Add("neck", ItemTemplate.SlotNeck);
            Slots.Add("legs", ItemTemplate.SlotLegs);
            Slots.Add("feet", ItemTemplate.SlotFeet);
            Slots.Add("gloves", ItemTemplate.SlotGloves);
            Slots.Add("chest,legs", ItemTemplate.SlotChest | ItemTemplate.SlotLegs);
            Slots.Add("rhand", ItemTemplate.SlotRHand);
            Slots.Add("lhand", ItemTemplate.SlotLHand);
            Slots.Add("lrhand", ItemTemplate.SlotLrHand);
            Slots.Add("rear,lear", ItemTemplate.SlotREar | ItemTemplate.SlotLEar);
            Slots.Add("rfinger,lfinger", ItemTemplate.SlotRFinger | ItemTemplate.SlotLFinger);
            Slots.Add("none", ItemTemplate.SlotNone);
            Slots.Add("wolf", ItemTemplate.SlotWolf);
            Slots.Add("hatchling", ItemTemplate.SlotHatchling);
            Slots.Add("strider", ItemTemplate.SlotStrider);
            Slots.Add("babypet", ItemTemplate.SlotBabypet);

            LoadArmorModels();
            LoadWeaponModels();
            LoadEtcItemModels();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Log.Info($"ItemTable: Loaded #{Armors.Count} armors, #{Weapons.Count} weapons and #{EtcItems.Count} etc items.");
        }

        public L2Item CreateItem(int itemId, int count, L2Player actor)
        {
            L2Item item = new L2Item(Instance.GetItem(itemId));

            L2World.Instance.AddObject(item);

            if (item.Template.Stackable && (count > 1))
                item.Count = count;

            return item;
        }

        private void LoadArmorModels()
        {
            Dictionary<int, ArmorModel> armorsModels = ItemService.GetAllArmorModelsDict();
            foreach (KeyValuePair<int, ArmorModel> modelPair in armorsModels)
            {
                StatsSet set = new StatsSet();
                ArmorModel model = modelPair.Value;
                Armor armor = new Armor(set)
                {
                    Type = Utilz.GetEnumFromString(model.ArmorType, ArmorTypeId.None),
                    ItemId = model.ItemId,
                    Name = model.Name,
                    BodyPart = Slots[model.BodyPart],
                    Sellable = model.Sellable,
                    Dropable = model.Dropable,
                    Destroyable = model.Destroyable,
                    Tradable = model.Tradeable,
                    Weight = model.Weight,
                    Duration = model.Duration
                };
                Armors.Add(modelPair.Key, armor);
            }
        }

        private void LoadEtcItemModels()
        {
            Dictionary<int, EtcItemModel> etcItemModels = ItemService.GetAllEtcItemModelsDict();
            foreach (KeyValuePair<int, EtcItemModel> modelPair in etcItemModels)
            {
                StatsSet set = new StatsSet();
                EtcItemModel model = modelPair.Value;
                EtcItem etcItem = new EtcItem(set)
                {
                    Type = Utilz.GetEnumFromString(model.ItemType, EtcItemTypeId.None),
                    ItemId = model.ItemId,
                    Name = model.Name,
                    Sellable = model.Sellable,
                    Dropable = model.Dropable,
                    Destroyable = model.Destroyable,
                    Tradable = model.Tradeable,
                    Weight = model.Weight,
                    Duration = model.Duration
                };
                EtcItems.Add(modelPair.Key, etcItem);
            }
        }

        private void LoadWeaponModels()
        {
            Dictionary<int, WeaponModel> weaponModels = ItemService.GetAllWeaponModelsDict();
            foreach (KeyValuePair<int, WeaponModel> modelPair in weaponModels)
            {
                StatsSet set = new StatsSet();
                WeaponModel model = modelPair.Value;
                Weapon weapon = new Weapon(set)
                {
                    Type = Utilz.GetEnumFromString(model.WeaponType, WeaponTypeId.None),
                    ItemId = model.ItemId,
                    Name = model.Name,
                    BodyPart = Slots[model.BodyPart],
                    Sellable = model.Sellable,
                    Dropable = model.Dropable,
                    Destroyable = model.Destroyable,
                    Tradable = model.Tradeable,
                    Weight = model.Weight,
                    Duration = model.Duration,
                    ReferencePrice = model.Price,
                    SpiritshotCount = model.Spiritshots,
                    SoulshotCount = model.Soulshots,
                    PDam = model.Pdam,
                    RndDam = model.RndDam,
                    Critical = model.Critical,
                    HitModifier = model.HitModify,
                    AvoidModifier = model.AvoidModify,
                    ShieldDef = model.ShieldDef,
                    ShieldDefRate = model.ShieldDefRate,
                    AtkSpeed = model.AtkSpeed,
                    MpConsume = model.MpConsume,
                    MDam = model.Mdam
                };
                Weapons.Add(modelPair.Key, weapon);
            }
        }
    }
}