using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

namespace MinerManagementMOD.Systems
{
    public class FakeChestSystem : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
                return;


            // 開いているチェストがあるか
            int chestIndex = player.chest;

            if (chestIndex < 0)
                return;


            Chest chest = Main.chest[chestIndex];

            if (chest == null)
                return;


            CheckFoolChest(chest);
        }

        private void CheckFoolChest(Chest chest)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].type ==
                    ModContent.ItemType<Items.FoolTicket>())
                {
                    ActivateFool(chest, i);
                    return;
                }
            }
        }

        private void ActivateFool(Chest chest, int slot)
        {
            Vector2 position = new Vector2(
                chest.x * 16 + 16,
                chest.y * 16
            );

            // 黄色文字
            CombatText.NewText(
                new Rectangle(
                    (int)position.X - 50,
                    (int)position.Y - 50,
                    100,
                    30),
                Color.Yellow,
                "You FOOL!!"
            );

            // オナラ音
            SoundEngine.PlaySound(
                new SoundStyle("MinerManagementMOD/Assets/Sounds/FoolFart"),
                position
            );

            // 茶色ダスト
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    position + new Vector2(0, 16),
                    32,
                    32,
                    DustID.Smoke
                );

                // 上へ広がるガス
                dust.velocity =
                    new Vector2(
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-4f, -1f)
                    );

                // ふわっと大きめ
                dust.scale =
                    Main.rand.NextFloat(1.5f, 2.5f);

                // ゆっくり消える
                dust.fadeIn = 1.2f;
            }


            // 一度だけ発動
            chest.item[slot].TurnToAir();

            Player player = Main.LocalPlayer;

            player.QuickSpawnItem(
                player.GetSource_Misc("FoolChest"),
                ModContent.ItemType<Items.FoolTicket>(),
                1
            );
        }
    }
}