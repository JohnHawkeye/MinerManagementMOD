using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace MinerManagementMOD.Systems
{
    public static class TreasureEffects
    {
        public static void PlayTreasureOpenEffect(Vector2 worldPosition)
        {
            // キラリーンという効果音
            SoundEngine.PlaySound(SoundID.Item4, worldPosition);

            // 上方向へ伸びる光の柱
            for (int i = 0; i < 50; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-1.2f, 1.2f),
                    Main.rand.NextFloat(-6f, -2.5f));

                Dust dust = Dust.NewDustPerfect(
                    worldPosition,
                    DustID.GoldFlame,
                    velocity,
                    80,
                    Color.Gold,
                    Main.rand.NextFloat(1.2f, 1.8f));

                dust.noGravity = true;
            }

            // 左右へ広がる光
            for (int i = 0; i < 35; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5f, 2.5f);

                Dust dust = Dust.NewDustPerfect(
                    worldPosition,
                    DustID.GoldCoin,
                    velocity,
                    120,
                    Color.White,
                    Main.rand.NextFloat(1f, 1.6f));

                dust.noGravity = true;
            }

            // 星が飛び散る
            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);

                Dust dust = Dust.NewDustPerfect(
                    worldPosition,
                    DustID.YellowStarDust,
                    velocity,
                    80,
                    Color.White,
                    1.8f);

                dust.noGravity = true;
            }
        }
    }
}