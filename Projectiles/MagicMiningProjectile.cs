using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Helpers;

namespace MinerManagementMOD.Projectiles
{
    public class MagicMiningProjectile : ModProjectile
    {
        public int MiningLevel;
        // 飛行中のサイズ
        private const float StartSize = 32f;

        // 最大半径2ブロック = 64px
        private const float MaxRadius = 32f;

        // 拡大時間 1秒
        private const int ExpandTime = 60;

        // 消滅時間 2秒
        private const int FadeTime = 120;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;

            Projectile.timeLeft = 300;

            Projectile.netImportant = true;


        }

        public override void AI()
        {
            AnimateProjectile();
            // -------------------------------------------------
            // ai[0] = ターゲットX
            // ai[1] = ターゲットY
            // -------------------------------------------------

            Vector2 targetPosition =
                new Vector2(
                    Projectile.ai[0],
                    Projectile.ai[1]);

            // -------------------------------------------------
            // まだターゲットに到達していない
            // -------------------------------------------------

            if (Projectile.ai[2] == 0f)
            {
                Vector2 difference =
                    targetPosition - Projectile.Center;

                float distance = difference.Length();

                // ターゲット到達
                if (distance <= Projectile.velocity.Length() + 2f)
                {
                    Projectile.Center = targetPosition;
                    Projectile.velocity = Vector2.Zero;

                    // 拡大開始
                    Projectile.ai[2] = 1f;
                    Projectile.localAI[0] = 0f;

                    return;
                }

                // ターゲット方向へ飛行
                difference.Normalize();

                Projectile.velocity =
                    difference * 10f;

                return;
            }

            // -------------------------------------------------
            // 拡大フェーズ
            // -------------------------------------------------

            if (Projectile.ai[2] == 1f)
            {
                Projectile.velocity = Vector2.Zero;

                Projectile.localAI[0]++;

                float progress =
                    Projectile.localAI[0] / ExpandTime;

                progress =
                    MathHelper.Clamp(progress, 0f, 1f);

                float radius =
                    MathHelper.Lerp(
                        StartSize / 2f,
                        MaxRadius,
                        progress);

                Projectile.width =
                    (int)(radius * 2f);

                Projectile.height =
                    (int)(radius * 2f);

                Projectile.Center = targetPosition;

                // 範囲内を採掘
                MineTiles(radius);

                // 1秒経過
                if (Projectile.localAI[0] >= ExpandTime)
                {
                    Projectile.ai[2] = 2f;
                    Projectile.localAI[0] = 0f;
                }

                return;
            }

            // -------------------------------------------------
            // 消滅フェーズ
            // -------------------------------------------------

            if (Projectile.ai[2] == 2f)
            {
                Projectile.velocity = Vector2.Zero;

                Projectile.localAI[0]++;

                float progress =
                    Projectile.localAI[0] / FadeTime;

                progress =
                    MathHelper.Clamp(progress, 0f, 1f);

                Projectile.alpha =
                    (int)MathHelper.Lerp(
                        0f,
                        255f,
                        progress);

                if (Projectile.localAI[0] >= FadeTime)
                {
                    Projectile.Kill();
                }
            }
        }

        private void MineTiles(float radius)
        {
            int centerX =
                (int)(Projectile.Center.X / 16f);

            int centerY =
                (int)(Projectile.Center.Y / 16f);

            int tileRadius =
                (int)(radius / 16f) + 1;

            for (int x = centerX - tileRadius;
                 x <= centerX + tileRadius;
                 x++)
            {
                for (int y = centerY - tileRadius;
                     y <= centerY + tileRadius;
                     y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    Vector2 tileCenter =
                        new Vector2(
                            x * 16f + 8f,
                            y * 16f + 8f);

                    if (Vector2.Distance(
                        Projectile.Center,
                        tileCenter) > radius)
                    {
                        continue;
                    }

                    Tile tile =
                        Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    // MiningHelperで採掘可能か判定

                    bool allowHardmodeOre = MiningLevel >= 2;

                    if (!MiningHelper.CanMine(tile.TileType, allowHardmodeOre))
                    {
                        continue;
                    }

                    // タイルを破壊
                    WorldGen.KillTile(
                        x,
                        y,
                        false,
                        false,
                        false);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture =
                Terraria.GameContent.TextureAssets.Projectile[
                    Projectile.type].Value;

            // 4枚の縦並びなので、1フレームの高さを取得
            int frameHeight =
                texture.Height / 4;

            Rectangle sourceRectangle =
                new Rectangle(
                    0,
                    frameHeight * Projectile.frame,
                    texture.Width,
                    frameHeight);

            // 1フレームの中央を原点にする
            Vector2 origin =
                new Vector2(
                    texture.Width / 2f,
                    frameHeight / 2f);

            // 32px → 現在のProjectileサイズに合わせて拡大
            float scale =
                Projectile.width / 32f;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                Color.White * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        private void AnimateProjectile()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;

                Projectile.frame++;

                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}