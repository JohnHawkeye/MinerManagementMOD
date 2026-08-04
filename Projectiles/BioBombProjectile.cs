using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.Projectiles
{
    public class BioBombProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            int tileX = (int)(Projectile.Center.X / 16);
            int tileY = (int)(Projectile.Center.Y / 16);

            SpawnExplosionEffect();
            SpawnBiomeTiles(tileX, tileY);
        }

        private void SpawnBiomeTiles(int tileX, int tileY)
        {
            Player player = Main.player[Projectile.owner];

            ushort tileType;

            // -------------------------
            // バイオーム判定
            // -------------------------
            if (player.ZoneUnderworldHeight)
            {
                tileType = TileID.Ash;
            }
            else if (player.ZoneSnow)
            {
                tileType = TileID.SnowBlock;
            }
            else if (player.ZoneDesert || player.ZoneBeach)
            {
                tileType = TileID.Sand;
            }
            else if (player.ZoneJungle || player.ZoneGlowshroom)
            {
                tileType = TileID.Mud;
            }
            else if (player.ZoneCorrupt)
            {
                tileType = TileID.Ebonstone;
            }
            else if (player.ZoneCrimson)
            {
                tileType = TileID.Crimstone;
            }
            else if (player.ZoneHallow)
            {
                tileType = TileID.Pearlstone;
            }
            else if (player.ZoneDungeon)
            {
                tileType = TileID.Dirt;
            }
            else if (player.ZoneRockLayerHeight)
            {
                tileType = TileID.Stone;
            }
            else
            {
                tileType = TileID.Dirt;
            }

            // -------------------------
            // 範囲生成
            // -------------------------
            int radius = 3;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y > radius * radius)
                        continue;

                    int tx = tileX + x;
                    int ty = tileY + y;

                    if (!WorldGen.InWorld(tx, ty))
                        continue;

                    Tile tile = Main.tile[tx, ty];

                    if (tile == null)
                        continue;

                    bool blocked = false;

                    // -------------------------
                    // プレイヤーチェック
                    // -------------------------
                    foreach (Player p in Main.player)
                    {
                        if (p.active)
                        {
                            if (Vector2.Distance(p.Center, new Vector2(tx * 16, ty * 16)) < 16f)
                            {
                                blocked = true;
                                break;
                            }
                        }
                    }

                    // -------------------------
                    // 何もいなければ生成
                    // -------------------------
                    if (!blocked && !Main.tile[tx, ty].HasTile)
                    {
                        WorldGen.PlaceTile(tx, ty, tileType, true, true);
                    }
                }
            }
        }
        private void SpawnExplosionEffect()
        {
            // 光
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    150,
                    default,
                    1.5f
                );
            }

            // 火花系
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(
                    Projectile.Center,
                    0,
                    0,
                    DustID.Smoke,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    100,
                    default,
                    2f
                );
            }

            // 光点
            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.2f);
        }
    }
}