using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Market
{
    public static class MarketDatabase
    {
        public static readonly List<int> TradeItems =
        [
            // 通常鉱石
            ItemID.CopperOre,
            ItemID.TinOre,
            ItemID.IronOre,
            ItemID.LeadOre,
            ItemID.SilverOre,
            ItemID.TungstenOre,
            ItemID.GoldOre,
            ItemID.PlatinumOre,

            // ハードモード鉱石
            ItemID.CobaltOre,
            ItemID.PalladiumOre,
            ItemID.MythrilOre,
            ItemID.OrichalcumOre,
            ItemID.AdamantiteOre,
            ItemID.TitaniumOre,

            // 邪悪鉱石
            ItemID.DemoniteOre,
            ItemID.CrimtaneOre,

            // ジャングル
            ItemID.ChlorophyteOre,

            // エンドゲーム
            //ItemID.LunarOre,

            ItemID.Hellstone,
            ItemID.Meteorite, 
            ItemID.Obsidian,

            // 宝石
            ItemID.Amethyst,
            ItemID.Topaz,
            ItemID.Sapphire,
            ItemID.Emerald,
            ItemID.Ruby,
            ItemID.Diamond,
            ItemID.Amber
        ];
        
        public static bool CanTrade(Item item)
        {
            return item != null &&
                   !item.IsAir &&
                   TradeItems.Contains(item.type);
        }
    }
    
}