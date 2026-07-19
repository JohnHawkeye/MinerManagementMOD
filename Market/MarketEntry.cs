using System;
using Terraria.ModLoader.IO;

namespace MinerManagementMOD.Market
{
    public class MarketEntry : TagSerializable
    {
        public int ItemType;

        // 今日の倍率
        public float Rate = 1f;

        // 昨日の倍率
        public float YesterdayRate = 1f;

        // 高騰・暴落フラグ
        public bool IsHot;
        public bool IsCrash;

        public MarketEntry()
        {
        }

        public MarketEntry(int itemType)
        {
            ItemType = itemType;
        }

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["ItemType"] = ItemType,
                ["Rate"] = Rate,
                ["YesterdayRate"] = YesterdayRate,
                ["IsHot"] = IsHot,
                ["IsCrash"] = IsCrash
            };
        }

        public static readonly Func<TagCompound, MarketEntry> DESERIALIZER = tag =>
        {
            return new MarketEntry
            {
                ItemType = tag.GetInt("ItemType"),
                Rate = tag.GetFloat("Rate"),
                YesterdayRate = tag.GetFloat("YesterdayRate"),
                IsHot = tag.GetBool("IsHot"),
                IsCrash = tag.GetBool("IsCrash")
            };
        };
    }
}