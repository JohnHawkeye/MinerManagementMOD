using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Systems
{
    public class DeliverySystem : ModSystem
    {
        public static int CurrentQuest = 0;

        public static bool CompletedToday = false;

        public static void GenerateQuest()
        {
            CompletedToday = false;

            //今は固定
            CurrentQuest = 0;
        }

        public static void OpenQuest(Player player)
        {
            if (CompletedToday)
            {
                Main.npcChatText =
                    "今日はもう十分だ。\nまた明日頼むよ。";
                return;
            }

            switch (CurrentQuest)
            {
                case 0:
                    CheckCopperSword100(player);
                    break;
            }
        }

        private static void CheckCopperSword100(Player player)
        {
            
            int itemType = ModContent.ItemType<CopperSword100>();

            // 所持しているか
            if (player.HasItem(itemType))
            {
                // アイテム消費
                player.ConsumeItem(itemType);

                // 報酬
                player.QuickSpawnItem(
                    player.GetSource_GiftOrReward(),
                    ItemID.GoldCoin,
                    50);

                CompletedToday = true;

                Main.npcChatText =
                    "ありがとう！\nこれは報酬だ！";
            }
            else
            {
                Main.npcChatText =
                    "銅の剣×100本が必要です。";
            }
        }
    }
}