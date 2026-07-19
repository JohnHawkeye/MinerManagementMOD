using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Common.NPCs;

namespace MinerManagementMOD.Systems
{
    public class MerchantSystem : ModSystem
    {
        /// <summary>
        /// 今日商人が来ているか
        /// </summary>
        public static bool MerchantPresent { get; private set; }

        /// <summary>
        /// 商人の名前
        /// </summary>
        public static string MerchantName { get; private set; } = "";

        /// <summary>
        /// 商人の見た目ID
        /// </summary>
        public static int MerchantStyle { get; private set; }

        public static int MerchantNPCIndex = -1;


        public static bool MerchantAlive
        {
            get
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.active &&
                        npc.GetGlobalNPC<MinerGlobalNPC>()
                        .IsMarketMerchant)
                    {
                        MerchantNPCIndex = i;
                        return true;
                    }
                }

                return false;
            }
        }

        public override void OnWorldLoad()
        {
            Reset();
        }

        public override void OnWorldUnload()
        {
            Reset();
        }

        /// <summary>
        /// 今日の商人を決定する
        /// </summary>
        public static void GenerateMerchant()
        {
            MerchantPresent = true;

            MerchantStyle = Main.rand.Next(6);

            string[] names =
            {
                "ガストン",
                "ロイド",
                "モーリス",
                "アラン",
                "ゲイル",
                "フレッド"
            };

            MerchantName = names[Main.rand.Next(names.Length)];
        }

        /// <summary>
        /// 帰宅
        /// </summary>
        public static void RemoveMerchant()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active &&
                    npc.GetGlobalNPC<MinerGlobalNPC>()
                    .IsMarketMerchant)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }

            MerchantPresent = false;
            MerchantNPCIndex = -1;
        }

        public static void Reset()
        {
            MerchantPresent = false;
            MerchantName = "";
            MerchantStyle = 0;
            MerchantNPCIndex = -1;
        }


    }
}