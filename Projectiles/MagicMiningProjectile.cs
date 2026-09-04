using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Helpers;
using System.Collections.Generic;
using Terraria.Audio;

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

        private HashSet<Point> processedExposedGems = new HashSet<Point>();

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
            int centerX = (int)(Projectile.Center.X / 16f);
            int centerY = (int)(Projectile.Center.Y / 16f);

            int tileRadius = (int)(radius / 16f) + 1;

            // =========================================================
            // ① 破壊前にExposedGemsを記録
            //    ただし、このProjectileで一度記録したものは除外
            // =========================================================

            List<(Point tilePosition, Vector2 worldPosition, int gemItem)> exposedGems
                = new List<(Point, Vector2, int)>();

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

                    // ExposedGemsではない
                    if (tile.TileType != TileID.ExposedGems)
                        continue;

                    Point tilePosition = new Point(x, y);

                    // このProjectileですでに記録済みなら無視
                    if (processedExposedGems.Contains(tilePosition))
                        continue;

                    int gemItem = GetGemItem(tile);

                    if (gemItem == -1)
                        continue;

                    exposedGems.Add(
                        (tilePosition, tileCenter, gemItem));

                    // ★このProjectileでは二度と処理しない
                    processedExposedGems.Add(tilePosition);
                }
            }

            // =========================================================
            // ② 通常のタイル破壊
            // =========================================================

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

                    // 採掘可能か判定
                    bool allowHardmodeOre = MiningLevel >= 2;

                    if (!MiningHelper.CanMine(
                        tile.TileType,
                        allowHardmodeOre))
                    {
                        continue;
                    }

                    // 通常の宝石タイル判定
                    int gemItem = GetGemItem(tile);

                    // ExposedGemsはここでは処理しない
                    bool isExposedGem =
                        tile.TileType == TileID.ExposedGems;

                    // タイルを破壊
                    WorldGen.KillTile(
                        x,
                        y,
                        false,
                        false,
                        false);

                    // 通常の宝石タイルのみ
                    if (gemItem != -1 && !isExposedGem)
                    {
                        Vector2 dropPosition =
                            new Vector2(
                                x * 16f + 8f,
                                y * 16f + 8f);

                        Item.NewItem(
                            null,
                            new Rectangle(
                                x * 16,
                                y * 16,
                                16,
                                16),
                            gemItem,
                            4);

                        CreateGemBonusEffect(
                            dropPosition,
                            gemItem);
                    }
                }
            }

            // =========================================================
            // ③ 破壊後にExposedGemsを確認
            // =========================================================

            foreach (var exposedGem in exposedGems)
            {
                Tile tileAfter =
                    Framing.GetTileSafely(
                        exposedGem.tilePosition.X,
                        exposedGem.tilePosition.Y);

                // ExposedGemが実際に消えていた場合のみボーナス
                if (!tileAfter.HasTile ||
                    tileAfter.TileType != TileID.ExposedGems)
                {
                    Item.NewItem(
                        null,
                        new Rectangle(
                            (int)exposedGem.worldPosition.X - 8,
                            (int)exposedGem.worldPosition.Y - 8,
                            16,
                            16),
                        exposedGem.gemItem,
                        4);

                    CreateGemBonusEffect(
                        exposedGem.worldPosition,
                        exposedGem.gemItem);
                }
            }
        }

        private void CreateGemBonusEffect(
            Vector2 position,
            int gemItem)
        {
            Color gemColor;

            switch (gemItem)
            {
                case ItemID.Amethyst:
                    gemColor = new Color(180, 80, 255);
                    break;

                case ItemID.Topaz:
                    gemColor = new Color(255, 220, 60);
                    break;

                case ItemID.Sapphire:
                    gemColor = new Color(70, 140, 255);
                    break;

                case ItemID.Emerald:
                    gemColor = new Color(60, 255, 120);
                    break;

                case ItemID.Ruby:
                    gemColor = new Color(255, 70, 70);
                    break;

                case ItemID.Diamond:
                    gemColor = new Color(180, 240, 255);
                    break;

                case ItemID.Amber:
                    gemColor = new Color(255, 160, 40);
                    break;

                default:
                    return;
            }

            // ---------------------------------------------
            // 発光
            // ---------------------------------------------

            Lighting.AddLight(
                position,
                gemColor.R / 255f * 0.8f,
                gemColor.G / 255f * 0.8f,
                gemColor.B / 255f * 0.8f);

            // ---------------------------------------------
            // キラキラしたDust
            // ---------------------------------------------

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    position,
                    DustID.GemAmethyst,
                    Main.rand.NextVector2Circular(2.5f, 2.5f));

                dust.color = gemColor;
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.3f);
            }

            // ---------------------------------------------
            // 白いキラキラを少し混ぜる
            // ---------------------------------------------

            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    position,
                    DustID.WhiteTorch,
                    Main.rand.NextVector2Circular(3f, 3f));

                dust.noGravity = true;
                dust.scale = 0.8f;
            }

            SoundEngine.PlaySound(
                SoundID.ResearchComplete, new Vector2(position.X , position.Y ));

            CombatText.NewText(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    16,
                    16
                ),
                Color.Gold,
                "Lucky!"
            );
        }

        private int GetGemItem(Tile tile)
        {
            ushort tileType = tile.TileType;

            switch (tileType)
            {
                case TileID.Amethyst:
                    return ItemID.Amethyst;

                case TileID.Topaz:
                    return ItemID.Topaz;

                case TileID.Sapphire:
                    return ItemID.Sapphire;

                case TileID.Emerald:
                    return ItemID.Emerald;

                case TileID.Ruby:
                    return ItemID.Ruby;

                case TileID.Diamond:
                    return ItemID.Diamond;

                case TileID.ExposedGems:

                    int gemStyle = tile.TileFrameX / 18;

                    switch (gemStyle)
                    {
                        case 0:
                            return ItemID.Amethyst;
                        case 1:
                            return ItemID.Topaz;
                        case 2:
                            return ItemID.Sapphire;
                        case 3:
                            return ItemID.Emerald;
                        case 4:
                            return ItemID.Ruby;
                        case 5:
                            return ItemID.Diamond;
                        case 6:
                            return ItemID.Amber;
                        default:
                            return -1;
                    }
                default: return -1;
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