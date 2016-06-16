using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace fr34kyn01535.Kits
{
    public class KitsConfiguration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "Kit")]
        public List<Kit> Kits;
        public int GlobalCooldown;

        public void LoadDefaults()
        {
            GlobalCooldown = 10;
            Kits = new List<Kit>() {
                new Kit() { Cooldown = 10, Name = "Survival", XP = 0,Items = new List<KitItem>() { new KitItem(245, 1), new KitItem(81, 2), new KitItem(16, 1) }},
                new Kit() { Cooldown = 10, Name = "Brute Force", XP = 0,Money = 30, Vehicle = 57,Items = new List<KitItem>() { new KitItem(112, 1), new KitItem(113, 3), new KitItem(254, 3) }},
                new Kit() { Cooldown = 10, Name = "Watcher", XP = 200,Money=-20, Items = new List<KitItem>() { new KitItem(109, 1), new KitItem(111, 3), new KitItem(236, 1) }}
            };
        }
    }

    public class Kit
    {
        public Kit() { }

        public string Name;
        public uint? XP = null;
        public decimal? Money = null;
        public ushort? Vehicle = null;

        [XmlArrayItem(ElementName = "Item")]
        public List<KitItem> Items;

        public int? Cooldown = null;
    }

    public class KitItem{

        public KitItem(){ }

        public KitItem(ushort itemId, byte amount)
        {
            ItemId = itemId;
            Amount = amount;
        }

        [XmlAttribute("id")]
        public ushort ItemId;

        [XmlAttribute("amount")]
        public byte Amount;
    }
}
