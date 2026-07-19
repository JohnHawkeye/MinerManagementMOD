using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using MinerManagementMOD.Market;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;

namespace MinerManagementMOD.Systems
{
    public class MarketSystem : ModSystem
    {
        // 市場データ
        private static readonly Dictionary<int, MarketEntry> market = new();

        // 今日の市場は更新済みか
        private static bool marketUpdatedToday;


        public override void OnWorldLoad()
        {
            market.Clear();

            GenerateMarket();

            // ワールド読み込み時は今日の市場は生成済み
            marketUpdatedToday = true;
        }

        public override void OnWorldUnload()
        {
            market.Clear();
            marketUpdatedToday = false;
        }

        public override void PreUpdateWorld()
        {
            // 夜になったら翌日の更新準備
            if (!Main.dayTime)
            {
                marketUpdatedToday = false;
                MerchantSpawnSystem.RemoveMerchant();
                return;
            }

            // 朝4:30頃（最初の約1秒）
            if (!marketUpdatedToday && Main.time < 60)
            {
                GenerateMarket();
                MerchantSystem.GenerateMerchant();
                MerchantSpawnSystem.SpawnMerchant();

                marketUpdatedToday = true;

                Main.NewText("本日の市場価格が更新されました！");
                Main.NewText(MerchantSystem.MerchantName);
            }
        }

        /// <summary>
        /// 本日の市場価格を生成する
        /// </summary>
        public static void GenerateMarket()
        {
            Dictionary<int, MarketEntry> oldMarket = new Dictionary<int, MarketEntry>(market);

            market.Clear();

            foreach (int itemType in MarketDatabase.TradeItems)
            {
                MarketEntry entry = new MarketEntry(itemType);

                MarketEntry oldEntry;

                if (oldMarket.TryGetValue(itemType, out oldEntry))
                    entry.YesterdayRate = oldEntry.Rate;
                else
                    entry.YesterdayRate = 1f;

                float change = Main.rand.NextFloat(-0.08f, 0.08f);
                entry.Rate = MathHelper.Clamp(entry.YesterdayRate + change, 0.8f, 1.8f);

                market[itemType] = entry;
            }
        }

        /// <summary>
        /// 市場データ取得
        /// </summary>
        public static MarketEntry GetEntry(int itemType)
        {
            MarketEntry entry;

            if (market.TryGetValue(itemType, out entry))
                return entry;

            return null;
        }

        /// <summary>
        /// 買取倍率取得
        /// </summary>
        public static float GetRate(int itemType)
        {
            MarketEntry entry = GetEntry(itemType);

            if (entry == null)
                return 1f;

            return entry.Rate;
        }

        /// <summary>
        /// 現在の市場価格で売却額を取得
        /// </summary>
        public static long GetSellPrice(Item item)
        {
            if (item == null || item.IsAir)
                return 0;

            // Terraria標準売値
            long basePrice = item.value / 5;

            // 市場倍率
            float rate = GetRate(item.type);

            return (long)(basePrice * rate * item.stack);
        }

        /// <summary>
        /// 買取倍率変更
        /// </summary>
        public static void SetRate(int itemType, float rate)
        {
            MarketEntry entry = GetEntry(itemType);

            if (entry == null)
                return;

            entry.Rate = MathHelper.Clamp(rate, 0.5f, 3.0f);
        }

        /// <summary>
        /// 市場に存在するか
        /// </summary>
        public static bool Contains(int itemType)
        {
            return market.ContainsKey(itemType);
        }

        /// <summary>
        /// 市場一覧取得
        /// </summary>
        public static IEnumerable<MarketEntry> GetAllEntries()
        {
            return market.Values;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["MarketEntries"] = market.Values.ToList();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            market.Clear();

            if (tag.ContainsKey("MarketEntries"))
            {
                IList<MarketEntry> list = tag.GetList<MarketEntry>("MarketEntries");

                foreach (MarketEntry entry in list)
                {
                    market[entry.ItemType] = entry;
                }
            }
            else
            {
                GenerateMarket();
            }

            marketUpdatedToday = true;
        }
        public static string GetTop5Summary()
        {
            if (market.Count == 0)
                return "今日は取引できる商品がありません。";

            var top5 = market.Values
                .OrderByDescending(x => x.Rate)
                .Take(5);

            System.Text.StringBuilder sb = new();

            sb.AppendLine("【本日の高値ランキング】");
            sb.AppendLine();

            int rank = 1;

            foreach (MarketEntry entry in top5)
            {
                float diff = (entry.Rate - entry.YesterdayRate) * 100f;

                string change;
                string itemName = Lang.GetItemNameValue(entry.ItemType);

                if (diff > 0)
                    change = $"▲+{diff:F0}%";
                else if (diff < 0)
                    change = $"▼{diff:F0}%";
                else
                    change = "-0%";

                sb.AppendLine($"{rank}. {itemName,-18}　{entry.Rate * 100f,5:F0}% {change}");
                rank++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// アイテムを市場価格で売却する
        /// </summary>
        public static bool SellItem(Player player, Item item)
        {
            if (player == null || item == null || item.IsAir)
                return false;

            long money = GetSellPrice(item);

            // 売値が0なら売却しない
            if (money <= 0)
                return false;

            GiveCoins(player, money);

            // アイテム削除
            item.TurnToAir();

            // 効果音
            SoundEngine.PlaySound(SoundID.Coins);

            return true;
        }

        /// <summary>
        /// 指定金額を貨幣に分解してプレイヤーへ渡す
        /// </summary>
        public static void GiveCoins(Player player, long value)
        {
            if (value <= 0)
                return;

            IEntitySource source = player.GetSource_Misc("MarketSell");

            int platinum = (int)(value / 1000000);
            value %= 1000000;

            int gold = (int)(value / 10000);
            value %= 10000;

            int silver = (int)(value / 100);
            int copper = (int)(value % 100);

            if (platinum > 0)
                player.QuickSpawnItem(source, ItemID.PlatinumCoin, platinum);

            if (gold > 0)
                player.QuickSpawnItem(source, ItemID.GoldCoin, gold);

            if (silver > 0)
                player.QuickSpawnItem(source, ItemID.SilverCoin, silver);

            if (copper > 0)
                player.QuickSpawnItem(source, ItemID.CopperCoin, copper);
        }
    }
}