using System;
using Humanizer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MinerManagementMOD.Systems
{
    public class TreasureBookSystem : ModSystem
    {
        public const int TreasureCount = 128;

        // 発見済みフラグ
        public static bool[] Discovered = new bool[TreasureCount];
        public static int[] TreasureItemTypes = new int[TreasureCount];

        /// <summary>
        /// 全データ初期化
        /// </summary>
        public static void ResetAll()
        {
            Discovered = new bool[TreasureCount];
        }

        public static void RegisterTreasureItem(
            int treasureID,
            int itemType)
        {
            int index = ToIndex(treasureID);

            if (index < 0 || index >= TreasureCount)
                return;


            TreasureItemTypes[index] = itemType;
            //Main.NewText($"登録: {treasureID} = {itemType}");
        }

        public override void OnWorldLoad()
        {
            ResetAll();
        }

        public override void OnWorldUnload()
        {
            ResetAll();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["TreasureDiscovered"] = Discovered;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("TreasureDiscovered"))
            {
                Discovered = tag.Get<bool[]>("TreasureDiscovered");
            }

            // 古いセーブデータ対策
            if (Discovered == null || Discovered.Length != TreasureCount)
            {
                ResetAll();
            }
        }

        /// <summary>
        /// 図鑑登録
        /// </summary>
        public static bool Register(int treasureID)
        {
            int index = ToIndex(treasureID);

            if (index < 0 || index >= TreasureCount)
                return false;

            if (Discovered[index])
                return false;

            Discovered[index] = true;
            return true;
        }

        /// <summary>
        /// 登録済み？
        /// </summary>
        public static bool IsDiscovered(int treasureID)
        {
            int index = ToIndex(treasureID);

            if (index < 0 || index >= TreasureCount)
                return false;

            return Discovered[index];
        }

        /// <summary>
        /// 登録数
        /// </summary>
        public static int GetDiscoveredCount()
        {
            int count = 0;

            foreach (bool discovered in Discovered)
            {
                if (discovered)
                    count++;
            }

            return count;
        }

        private static int ToIndex(int treasureID)
        {
            return treasureID - 1;
        }

        public static int GetItemType(int treasureID)
        {
            int index = ToIndex(treasureID);

            if (index < 0 || index >= TreasureCount)
                return 0;

            return TreasureItemTypes[index];
        }

        public static void DebugReset()
        {
            Discovered = new bool[TreasureCount];

            Main.NewText(
                "トレジャー図鑑をリセットしました。",
                Microsoft.Xna.Framework.Color.Yellow);
        }
    }
}