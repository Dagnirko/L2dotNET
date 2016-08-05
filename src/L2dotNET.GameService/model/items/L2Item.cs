﻿using System;
using System.Linq;
using System.Threading.Tasks;
using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Network.Serverpackets;
using L2dotNET.GameService.Tables;
using L2dotNET.GameService.Templates;
using L2dotNET.GameService.Tools;
using L2dotNET.GameService.World;
using L2dotNET.Models;
using L2dotNET.Services.Contracts;
using Ninject;

namespace L2dotNET.GameService.Model.Items
{
    public class L2Item : L2Object
    {
        [Inject]
        public IItemService ItemService => GameServer.Kernel.Get<IItemService>();

        public ItemTemplate Template;
        public int Count;
        public short IsEquipped;
        public int Enchant;
        public int AugmentationId = 0;
        public int Durability;
        public ItemLocation Location;
        public int PaperdollSlot = -1;
        public int PetId = -1;
        public int Dropper;
        public int SlotLocation = 0;

        public bool ExistsInDb { get; set; }
        public int OwnerId { get; set; }

        public short AttrAttackType = -2;
        public short AttrAttackValue = 0;

        public bool Blocked = false;
        public bool TempBlock = false;

        public L2Item(ItemTemplate template)
        {
            ObjId = IdFactory.Instance.NextId();
            Template = template;
            Count = 1;
            Location = ItemLocation.Void;
        }

        public void GenId()
        {
            ObjId = IdFactory.Instance.NextId();
        }


        public void ChangeCount(int count, L2Player creator)
        {
            if (count == 0)
                return;

            if (count > 0 && Count > int.MaxValue - count)
                Count = int.MaxValue;
            else
                Count = Count + count;

            if (Count < 0)
                Count = 0;
        }

        /** Enumeration of locations for item */

        public enum ItemLocation
        {
            Void,
            Inventory,
            Paperdoll,
            Warehouse,
            Clanwh,
            Pet,
            PetEquip,
            Lease,
            Freight
        }

        public void Unequip(L2Player owner)
        {
            IsEquipped = 0;
            PaperdollSlot = -1;

            owner.RemoveStats(this);
        }

        public void Equip(L2Player owner)
        {
            IsEquipped = 1;

            Location = ItemLocation.Paperdoll;

            owner.AddStats(this);
        }

        public void NotifyStats(L2Player owner)
        {
            owner.AddStats(this);
        }

        private void TryEquipSecondary(L2Player owner) { }

        public void DropMe(int x, int y, int z, L2Character dropper, L2Character killer, int seconds)
        {
            X = x;
            Y = y;
            Z = z;
            DropItem pk = new DropItem(this);
            if (dropper != null)
                Dropper = dropper.ObjId;

            Location = ItemLocation.Void;

            killer?.AddKnownObject(this, pk, true);

            L2World.Instance.AddObject(this);
        }

        public void DropMe(int x, int y, int z)
        {
            DropMe(x, y, z, null, null, 0);
        }

        public override void OnAction(L2Player player)
        {
            double dis = Calcs.CalculateDistance(this, player, true);
            player.SendMessage($"{AsString()} dis {(int)dis}");
            if (dis < 80)
            {
                foreach (L2Player o in KnownObjects.Values.OfType<L2Player>())
                {
                    o.SendPacket(new GetItem(player.ObjId, ObjId, X, Y, Z));
                    o.SendPacket(new DeleteObject(ObjId));
                }

                player.OnPickUp(this);

                L2World.Instance.RemoveObject(this);
            }
            else
                player.TryMoveTo(X, Y, Z);
        }

        public override void OnForcedAttack(L2Player player)
        {
            player.SendActionFailed();
        }

        public void UpdateDatabase()
        {
            if (ExistsInDb)
            {
                //if(OwnerId == 0 || Location == ItemLocation.Void || (Count == 0 && Location != ItemLocation.Lease))
                //    RemoveFromDb();
                //else
                //    UpdateInDb();
            }
            else
            {
               // if (OwnerId == 0 || Location == ItemLocation.Void || (Count == 0 && Location != ItemLocation.Lease))
               //     return;

                InsertInDb();
            }
        }

        private void InsertInDb()
        {
            ItemModel model = new ItemModel
            {
                ObjectId = ObjId,
                ItemId = Template.ItemId,
                Count = Count,
                CustomType1 = CustomType1,
                CustomType2 = CustomType2,
                Enchant = Enchant,
                LocationData = SlotLocation,
                Location = Enum.GetName(typeof(ItemLocation), Location),
                OwnerId = OwnerId,
                ManaLeft = 0,
                Time = 0,
                TimeOfUse = null
            };

            ItemService.InsertNewItem(model);
        }

        private bool _lifeTimeEndEnabled;
        private DateTime _lifeTimeEndTime;
        public int CustomType1;
        public int CustomType2;
        public bool Soulshot = false,
                    Spiritshot = false,
                    BlessSpiritshot = false;

        public int LifeTimeEnd()
        {
            if (!_lifeTimeEndEnabled)
                return -9999;

            TimeSpan ts = _lifeTimeEndTime - DateTime.Now;
            return (int)ts.TotalSeconds;
        }

        public void AddLimitedHour(int hours)
        {
            if (_lifeTimeEndEnabled)
                _lifeTimeEndTime = _lifeTimeEndTime.AddHours(hours);
            else
            {
                _lifeTimeEndEnabled = true;
                _lifeTimeEndTime = DateTime.Now.AddHours(hours);
            }
        }

        public void SetLimitedHour(string str)
        {
            if (str == "-1")
                return;

            string[] x1 = str.Split(' ');
            int yy = Convert.ToInt32(x1[0].Split('-')[0]);
            int mm = Convert.ToInt32(x1[0].Split('-')[1]);
            int dd = Convert.ToInt32(x1[0].Split('-')[2]);
            int hh = Convert.ToInt32(x1[1].Split('-')[0]);
            int m = Convert.ToInt32(x1[1].Split('-')[1]);
            int ss = Convert.ToInt32(x1[1].Split('-')[2]);

            DateTime dt = new DateTime(yy, mm, dd, hh, m, ss);
            if (dt <= DateTime.Now)
                return;

            _lifeTimeEndEnabled = true;
            _lifeTimeEndTime = dt;
            //TODO delete me
        }
        
        public override string AsString()
        {
            return $"L2Item:{Template.ItemId}; count {Count}; enchant {Enchant}; id {ObjId}";
        }

        public bool NotForTrade()
        {
            return !Template.Tradable || (AugmentationId > 0) || (IsEquipped == 1);
        }

        public bool NotForSale()
        {
            return !Template.Tradable || (IsEquipped == 1);
        }
    }
}