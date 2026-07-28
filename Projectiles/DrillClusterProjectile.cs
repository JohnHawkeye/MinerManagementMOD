using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Helpers;
using Terraria.Audio;

namespace MinerManagementMOD.Projectiles
{
    public class DrillClusterProjectile : ModProjectile
    {
        private Vector2 startPosition;

        private const float MaxTravelDistance = 144f; // 3ブロック分
        private const float FlySpeed = 3f;            // ゆっくり飛ぶ速度

        private int soundTimer = 0;
        private const int SoundInterval = 15;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged; // 射撃武器のダメージ計算に準拠

            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1; // 敵を貫通し続ける(採掘しながら進むドリルのため)
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                startPosition = Projectile.Center;
                Projectile.localAI[0] = 1;

                // 発射直後、速度をゆっくりに固定(マウス方向は維持)
                Projectile.velocity =
                    Projectile.velocity.SafeNormalize(Vector2.UnitX) * FlySpeed;
            }
            PlayDrillSound();
            MineTiles();

            float distance = Vector2.Distance(startPosition, Projectile.Center);

            // 3ブロック分進んだら消滅
            if (distance > MaxTravelDistance)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            // 消滅時にも火花を散らす
            SpawnSparks(Projectile.Center, 12);
        }

        private void MineTiles()
        {
            // タイル破壊は所有クライアントのみ
            if (Projectile.owner != Main.myPlayer)
                return;

            Rectangle area = Projectile.Hitbox;

            int startX = area.Left / 16;
            int endX = area.Right / 16;
            int startY = area.Top / 16;
            int endY = area.Bottom / 16;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    // ヘルストーンまでの鉱物に限定(ハードモード鉱石は不可)
                    if (!MiningHelper.CanMine(tile.TileType, allowHardmodeOre: false))
                        continue;

                    WorldGen.KillTile(x, y, false, false, false);
                    SpawnSparks(new Vector2(x * 16 + 8, y * 16 + 8), 3);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1);
                    }
                }
            }
        }

        private void SpawnSparks(Vector2 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Dust spark = Dust.NewDustDirect(
                    position,
                    4, 4,
                    DustID.Torch,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f)
                );

                spark.noGravity = Main.rand.NextBool();
                spark.scale = Main.rand.NextFloat(0.8f, 1.3f);
            }
        }
        
        private void PlayDrillSound()
        {
            soundTimer++;

            if (soundTimer >= SoundInterval)
            {
                soundTimer = 0;

                SoundEngine.PlaySound(
                    SoundID.Item23, // ドリル系の駆動音(Drax/Pickaxe Axeなどで使用)
                    Projectile.Center
                );
            }
        }
    }
}