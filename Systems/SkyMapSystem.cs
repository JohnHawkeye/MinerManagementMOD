using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace MinerManagementMOD.Systems
{
    public class SkyMapSystem : ModSystem
    {
        // ============================================================
        // 設定
        // ============================================================

        // 1フレームに処理するタイル数
        //
        // 5000なら比較的軽い。
        // 大型ワールドでも処理を分散する。
        private const int TilesPerFrame = 5000;

        // ============================================================
        // 状態
        // ============================================================

        private static bool revealing;

        private static int ownerPlayer = -1;

        private static int currentX;
        private static int currentY;

        private static int startY;
        private static int endY;

        private static long totalTiles;
        private static long processedTiles;

        private static int effectTimer;

        // ============================================================
        // 外部参照
        // ============================================================

        public static bool IsRevealing => revealing;

        public static float Progress
        {
            get
            {
                if (totalTiles <= 0)
                    return 0f;

                return MathHelper.Clamp(
                    (float)processedTiles / totalTiles,
                    0f,
                    1f
                );
            }
        }

        // ============================================================
        // ワールドロード
        // ============================================================

        public override void OnWorldLoad()
        {
            Reset();
        }

        public override void OnWorldUnload()
        {
            Reset();
        }

        private static void Reset()
        {
            revealing = false;

            ownerPlayer = -1;

            currentX = 0;
            currentY = 0;

            startY = 0;
            endY = 0;

            totalTiles = 0;
            processedTiles = 0;

            effectTimer = 0;
        }

        // ============================================================
        // 開始
        // ============================================================

        public static void StartReveal(Player player)
        {
            if (revealing)
                return;

            // Dedicated Serverでは実行しない
            if (Main.dedServ)
                return;

            ownerPlayer = player.whoAmI;

            // ========================================================
            // 「空全体」を対象にする
            //
            // 上端は0
            // 下端はworldSurface
            // ========================================================

            GetSkyIslandHeightRange(out startY, out endY);

            
            // 安全対策
            startY = Math.Max(
                0,
                startY
            );

            endY = Math.Min(
                Main.maxTilesY - 1,
                endY
            );

            // ========================================================
            // 処理開始地点
            // ========================================================

            currentX = 0;
            currentY = startY;

            processedTiles = 0;

            totalTiles =
                (long)Main.maxTilesX *
                (endY - startY + 1);

            effectTimer = 0;

            revealing = true;

            // ========================================================
            // 開始音
            // ========================================================

            SoundEngine.PlaySound(
                SoundID.Item29,
                player.Center
            );

            Main.NewText(
                "空島の地図を展開しています……",
                Color.LightSkyBlue
            );
        }

        // ============================================================
        // 毎フレーム
        // ============================================================

        public override void PostUpdateEverything()
        {
            if (!revealing)
                return;

            if (Main.dedServ)
                return;

            if (ownerPlayer < 0 ||
                ownerPlayer >= Main.maxPlayers)
            {
                Reset();
                return;
            }

            Player player =
                Main.player[ownerPlayer];

            if (!player.active)
            {
                Reset();
                return;
            }

            // ========================================================
            // マップ処理
            // ========================================================

            ProcessTiles();

            // ========================================================
            // エフェクト
            // ========================================================

            effectTimer++;

            if (effectTimer >= 4)
            {
                effectTimer = 0;

                CreateProgressEffect();
            }
        }

        // ============================================================
        // タイル処理
        // ============================================================

        private static void ProcessTiles()
        {
            int processedThisFrame = 0;

            while (
                processedThisFrame < TilesPerFrame &&
                currentY <= endY)
            {
                RevealTile(
                    currentX,
                    currentY
                );

                processedTiles++;
                processedThisFrame++;

                currentX++;

                if (currentX >= Main.maxTilesX)
                {
                    currentX = 0;
                    currentY++;
                }
            }

            if (currentY > endY)
            {
                FinishReveal();
            }
        }

        // ============================================================
        // 1タイルをマップへ登録
        // ============================================================

        private static void RevealTile(
            int x,
            int y)
        {
            if (x < 0 ||
                x >= Main.maxTilesX ||
                y < 0 ||
                y >= Main.maxTilesY)
            {
                return;
            }

            // --------------------------------------------------------
            // 現在のワールドタイル
            // --------------------------------------------------------

            Tile tile = Main.tile[x, y];

            // --------------------------------------------------------
            // 重要
            //
            // Lighting.GetColor() は使用しない。
            //
            // 未探索の空島内部やSpace付近は暗くなるため、
            // それをそのままマップに登録すると、
            // 「明らかにしたのに黒い」状態になる。
            //
            // 今回は地図を完全に明らかにするため、
            // 明るさを255として登録する。
            // --------------------------------------------------------

            Main.Map.Update(
                x,
                y,
                255
            );

            // --------------------------------------------------------
            // MapTileの更新を確実に反映
            // --------------------------------------------------------

            Main.Map.UpdateType(
                x,
                y
            );
        }

        // ============================================================
        // 完了
        // ============================================================

        private static void FinishReveal()
        {
            if (ownerPlayer < 0 ||
                ownerPlayer >= Main.maxPlayers)
            {
                Reset();
                return;
            }

            Player player =
                Main.player[ownerPlayer];

            // ========================================================
            // ★ マップ表示を即時更新
            // ========================================================

            Main.sectionManager.ClearMapDraw();

            // ========================================================
            // マップ保存
            // ========================================================

            Main.Map.Save();

            // ========================================================
            // 完了音
            // ========================================================

            SoundEngine.PlaySound(
                SoundID.Item4,
                player.Center
            );

            // ========================================================
            // 完了エフェクト
            // ========================================================

            for (int i = 0; i < 60; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(
                        5f,
                        5f
                    );

                Dust dust =
                    Dust.NewDustPerfect(
                        player.Center,
                        DustID.GoldFlame,
                        velocity
                    );

                dust.noGravity = true;

                dust.scale =
                    Main.rand.NextFloat(
                        0.8f,
                        1.6f
                    );
            }

            Lighting.AddLight(
                player.Center,
                1.0f,
                0.9f,
                0.5f
            );

            Main.NewText(
                "空全体の地図が明らかになりました！",
                Color.Gold
            );

            Reset();
        }

        // ============================================================
        // 進捗エフェクト
        // ============================================================

        private static void CreateProgressEffect()
        {
            if (ownerPlayer < 0)
                return;

            Player player =
                Main.player[ownerPlayer];

            if (!player.active)
                return;

            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPosition =
                    player.Center +
                    new Vector2(
                        Main.rand.NextFloat(-60f, 60f),
                        Main.rand.NextFloat(-50f, 20f)
                    );

                Dust dust =
                    Dust.NewDustPerfect(
                        spawnPosition,
                        DustID.Cloud,
                        new Vector2(
                            Main.rand.NextFloat(-0.5f, 0.5f),
                            Main.rand.NextFloat(-1.5f, -0.5f)
                        )
                    );

                dust.noGravity = true;

                dust.scale =
                    Main.rand.NextFloat(
                        0.8f,
                        1.4f
                    );
            }
        }

        // ============================================================
        // UI
        // ============================================================

        public override void PostDrawInterface(
            SpriteBatch spriteBatch)
        {
            if (!revealing)
                return;

            if (Main.dedServ)
                return;

            if (ownerPlayer != Main.myPlayer)
                return;

            DrawProgressUI(
                spriteBatch
            );
        }

        // ============================================================
        // プログレスUI
        // ============================================================

        private static void DrawProgressUI(
            SpriteBatch spriteBatch)
        {
            float progress = Progress;

            string text =
                $"空全体を解析中... {progress * 100f:0.0}%";

            Vector2 textSize =
                Terraria.GameContent.FontAssets.MouseText.Value
                    .MeasureString(text);

            Vector2 position =
                new Vector2(
                    Main.screenWidth / 2f,
                    80f
                );

            Utils.DrawBorderString(
                spriteBatch,
                text,
                position -
                new Vector2(
                    textSize.X / 2f,
                    0f
                ),
                Color.White
            );

            // --------------------------------------------------------
            // プログレスバー
            // --------------------------------------------------------

            string bar =
                new string(
                    '█',
                    Math.Max(
                        1,
                        (int)(30f * progress)
                    )
                );

            Utils.DrawBorderString(
                spriteBatch,
                bar,
                new Vector2(
                    Main.screenWidth / 2f - 150f,
                    115f
                ),
                Color.LightSkyBlue
            );

            // --------------------------------------------------------
            // 高度
            // --------------------------------------------------------

            string altitude =
                $"高度 {currentY} / {endY}";

            Vector2 altitudeSize =
                Terraria.GameContent.FontAssets.MouseText.Value
                    .MeasureString(altitude);

            Utils.DrawBorderString(
                spriteBatch,
                altitude,
                new Vector2(
                    Main.screenWidth / 2f -
                    altitudeSize.X / 2f,
                    145f
                ),
                Color.LightGray
            );
        }
        private static void GetSkyIslandHeightRange(
    out int startY,
    out int endY)
        {
            // Terrariaの通常ワールドサイズ
            //
            // Small  : 4200 x 1200
            // Medium : 6400 x 1800
            // Large  : 8400 x 2400

            if (Main.maxTilesX <= 4200)
            {
                // Small
                startY = 0;
                endY = 310;
            }
            else if (Main.maxTilesX <= 6400)
            {
                // Medium
                startY = 0;
                endY = 410;
            }
            else
            {
                // Large
                startY = 0;
                endY = 610;
            }

            // ワールド外に出ないようにする
            startY = Utils.Clamp(
                startY,
                0,
                Main.maxTilesY - 1
            );

            endY = Utils.Clamp(
                endY,
                startY,
                Main.maxTilesY - 1
            );
        }
    }
}