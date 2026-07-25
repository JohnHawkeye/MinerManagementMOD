using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class BuriedBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1);

            Item.consumable = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen, player.Center);

            int roll = Main.rand.Next(1000);

            // ライフポーション
            if (roll < 100)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.LesserHealingPotion);
            }
            // マナポーション
            else if (roll < 200)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.LesserManaPotion);
            }
            // カッパーコイン
            else if (roll < 330)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.CopperCoin, Main.rand.Next(25, 36));
            }
            // ビルダーポーション
            else if (roll < 400)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.BuilderPotion);
            }
            // マイニングポーション
            else if (roll < 470)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.MiningPotion);
            }
            // 危険察知ポーション
            else if (roll < 540)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.TrapsightPotion);
            }
            // 鉱石探知ポーション
            else if (roll < 610)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.SpelunkerPotion);
            }
            // 夜目ポーション
            else if (roll < 680)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.NightOwlPotion);
            }
            // 発光ポーション
            else if (roll < 740)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.ShinePotion);
            }
            // 爆弾
            else if (roll < 800)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Bomb, Main.rand.Next(3, 7));
            }
            // グロウスティック
            else if (roll < 850)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Glowstick, Main.rand.Next(20, 41));
            }
            // ロープ
            else if (roll < 900)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Rope, Main.rand.Next(50, 101));
            }
            // 松明
            else if (roll < 940)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Torch, Main.rand.Next(25, 36));
            }
            // 金インゴット
            else if (roll < 965)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.GoldBar, Main.rand.Next(2, 5));
            }
            // 金の鍵
            else if (roll < 980)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.GoldenKey);
            }
            // ダイヤモンド
            else if (roll < 990)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Diamond);
            }
            // ゴールドピッケル
            else if (roll < 996)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.GoldPickaxe);
            }
            // カエル（8匹）
            else if (roll < 998)
            {
                SoundEngine.PlaySound(SoundID.Item16, player.Center);

                for (int i = 0; i < 8; i++)
                {
                    NPC.NewNPC(
                        player.GetSource_OpenItem(Item.type),
                        (int)player.Center.X + Main.rand.Next(-40, 41),
                        (int)player.Center.Y,
                        NPCID.Frog);
                }
            }
            // コウモリ（4匹）
            else
            {
                SoundEngine.PlaySound(SoundID.Item16, player.Center);

                for (int i = 0; i < 4; i++)
                {
                    NPC.NewNPC(
                        player.GetSource_OpenItem(Item.type),
                        (int)player.Center.X + Main.rand.Next(-40, 41),
                        (int)player.Center.Y,
                        NPCID.CaveBat);
                }
            }
        }
    }
}