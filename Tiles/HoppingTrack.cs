using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Tiles
{
    // ==================================================
    // HoppingTrack
    //
    // 実際の線路ではなく、Wallとして設置するセンサー。
    //
    // 普通のトロッコ線路には一切干渉しない。
    //
    // トロッコに乗ったプレイヤーがセンサーに触れると、
    // 約6ブロック分、上方向へ跳ね上がる。
    // ==================================================

    public class HoppingTrack : ModWall
    {
        // ==========================================
        // ジャンプ設定
        // ==========================================

        // 何ブロック上へ跳ねるか
        private const float JumpHeightBlocks = 8f;

        // 1ブロック = 16px
        private const float JumpHeightPixels =
            JumpHeightBlocks * 16f;

        // Terrariaの標準重力
        private const float Gravity = 0.4f;

        // 再発動防止時間
        // 60 = 約1秒
        private const int CooldownTime = 30;


        // ==========================================
        // センサー画像
        // ==========================================

        private static Asset<Texture2D> SensorTexture;


        // ==========================================
        // Wall設定
        // ==========================================

        public override void SetStaticDefaults()
        {
            AddMapEntry(
                new Color(180, 120, 40)
            );

            // 壊したときのダスト
            DustType = DustID.WoodFurniture;
        }


        // ==========================================
        // センサー画像を描画
        // ==========================================

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D sensorTexture =
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Tiles/HoppingTrack"
                ).Value;

            Vector2 zero = Main.drawToScreen
                ? Vector2.Zero
                : new Vector2(Main.offScreenRange);

            Vector2 position =
                new Vector2(i * 16f, j * 16f)
                - Main.screenPosition
                + zero;

            spriteBatch.Draw(
                sensorTexture,
                position,
                Color.White
            );
        }


        // ==================================================
        // プレイヤー側処理
        // ==================================================

        public class HoppingTrackPlayer : ModPlayer
        {
            // ------------------------------------------
            // 再発動防止時間
            // ------------------------------------------

            public int HoppingCooldown;


            // ------------------------------------------
            // 前フレームにセンサーへ触れていたか
            // ------------------------------------------

            private bool WasTouchingSensor;


            // ==========================================
            // 毎フレーム処理
            // ==========================================

            public override void PostUpdate()
            {
                // --------------------------------------
                // クールダウン減少
                // --------------------------------------

                if (HoppingCooldown > 0)
                {
                    HoppingCooldown--;
                }


                // --------------------------------------
                // トロッコに乗っているか？
                // --------------------------------------

                if (!Player.mount.Active ||
                    !Player.mount.Cart)
                {
                    WasTouchingSensor = false;
                    return;
                }


                // --------------------------------------
                // センサーに触れているか？
                // --------------------------------------

                bool touching =
                    IsTouchingHoppingSensor(Player);


                // --------------------------------------
                // センサーへ新しく接触した瞬間
                // --------------------------------------

                if (touching &&
                    !WasTouchingSensor &&
                    HoppingCooldown <= 0)
                {
                    JumpPlayer(Player);

                    HoppingCooldown =
                        CooldownTime;
                }


                // 現在の接触状態を保存
                WasTouchingSensor = touching;
            }


            // ==========================================
            // センサー接触判定
            // ==========================================

            private static bool IsTouchingHoppingSensor(
                Player player)
            {
                Rectangle hitbox =
                    player.Hitbox;


                // --------------------------------------
                // プレイヤー周辺のWallだけ調べる
                // --------------------------------------

                int startX =
                    Math.Max(
                        0,
                        hitbox.Left / 16 - 1
                    );

                int endX =
                    Math.Min(
                        Main.maxTilesX - 1,
                        hitbox.Right / 16 + 1
                    );

                int startY =
                    Math.Max(
                        0,
                        hitbox.Top / 16 - 1
                    );

                int endY =
                    Math.Min(
                        Main.maxTilesY - 1,
                        hitbox.Bottom / 16 + 1
                    );


                int sensorType =
                    ModContent.WallType<HoppingTrack>();


                // --------------------------------------
                // 周囲のWallを確認
                // --------------------------------------

                for (int x = startX;
                     x <= endX;
                     x++)
                {
                    for (int y = startY;
                         y <= endY;
                         y++)
                    {
                        Tile tile =
                            Main.tile[x, y];

                        if (tile == null)
                            continue;


                        // HoppingTrackのWallか？
                        if (tile.WallType != sensorType)
                            continue;


                        // ----------------------------------
                        // 16×16のセンサー領域
                        // ----------------------------------

                        Rectangle sensorRect =
                            new Rectangle(
                                x * 16,
                                y * 16,
                                16,
                                16
                            );


                        // ----------------------------------
                        // プレイヤーが触れた
                        // ----------------------------------

                        if (hitbox.Intersects(
                            sensorRect))
                        {
                            return true;
                        }
                    }
                }


                return false;
            }


            // ==========================================
            // ジャンプ処理
            // ==========================================

            private static void JumpPlayer(
                Player player)
            {
                // --------------------------------------
                // v² = 2gh
                //
                // h = 6 × 16 = 96px
                //
                // v = √(2 × 0.4 × 96)
                //   ≒ 8.76
                // --------------------------------------

                float jumpVelocity =
                    MathF.Sqrt(
                        2f *
                        Gravity *
                        JumpHeightPixels
                    );


                // --------------------------------------
                // 上方向へジャンプ
                // --------------------------------------

                player.velocity.Y =
                    -jumpVelocity;


                // --------------------------------------
                // トロッコから降ろさない
                // --------------------------------------

                // mount.Cartを変更しないため、
                // トロッコに乗った状態を維持する。


                // --------------------------------------
                // ジャンプ時の煙
                // --------------------------------------

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        Dust.NewDust(
                            player.Bottom -
                            new Vector2(8f, 0f),

                            16,
                            4,

                            DustID.Smoke
                        );
                    }
                }
            }
        }
    }
}