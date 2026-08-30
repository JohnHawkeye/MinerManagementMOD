using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Systems
{
    public class DungeonMapSystem : ModSystem
    {
        // ============================================================
        // 設定
        // ============================================================

        // 1フレームに処理するタイル数
        private const int TilesPerFrame = 5000;

        // ダンジョン探索範囲
        //
        // Main.dungeonX / Main.dungeonY を中心として、
        // かなり広めに探索する。
        //
        // これは「マップ公開範囲」ではなく、
        // 「ダンジョン構造を探す範囲」。
        private const int SearchRadiusX = 1200;
        private const int SearchRadiusY = 1200;

        // ダンジョン構造から少しだけ外側を含める。
        //
        // 0なら完全に構造だけ。
        // 1～2程度なら壁際の空間も自然に表示できる。
        private const int RevealPadding = 3;

        // ============================================================
        // 状態
        // ============================================================

        private static bool revealing;

        private static int ownerPlayer = -1;

        // 探索中
        private static int searchCurrentX;
        private static int searchCurrentY;

        private static int searchStartX;
        private static int searchEndX;

        private static int searchStartY;
        private static int searchEndY;

        private static long searchTotalTiles;
        private static long searchProcessedTiles;

        // ダンジョン実範囲
        private static int dungeonMinX;
        private static int dungeonMaxX;

        private static int dungeonMinY;
        private static int dungeonMaxY;

        private static bool dungeonFound;

        // マップ公開中
        private static int revealCurrentX;
        private static int revealCurrentY;

        private static int revealStartX;
        private static int revealEndX;

        private static int revealStartY;
        private static int revealEndY;

        private static long revealTotalTiles;
        private static long revealProcessedTiles;

        private static int effectTimer;

        // ============================================================
        // 状態
        // ============================================================

        private enum State
        {
            None,
            Searching,
            Revealing
        }

        private static State state;

        // ============================================================
        // 外部参照
        // ============================================================

        public static bool IsRevealing => revealing;

        public static float Progress
        {
            get
            {
                if (state == State.Searching)
                {
                    if (searchTotalTiles <= 0)
                        return 0f;

                    return MathHelper.Clamp(
                        (float)searchProcessedTiles /
                        searchTotalTiles,
                        0f,
                        1f
                    ) * 0.3f;
                }

                if (state == State.Revealing)
                {
                    if (revealTotalTiles <= 0)
                        return 0.3f;

                    return 0.3f +
                        MathHelper.Clamp(
                            (float)revealProcessedTiles /
                            revealTotalTiles,
                            0f,
                            1f
                        ) * 0.7f;
                }

                return 0f;
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

            state = State.None;

            searchCurrentX = 0;
            searchCurrentY = 0;

            searchStartX = 0;
            searchEndX = 0;

            searchStartY = 0;
            searchEndY = 0;

            searchTotalTiles = 0;
            searchProcessedTiles = 0;

            dungeonMinX = 0;
            dungeonMaxX = 0;

            dungeonMinY = 0;
            dungeonMaxY = 0;

            dungeonFound = false;

            revealCurrentX = 0;
            revealCurrentY = 0;

            revealStartX = 0;
            revealEndX = 0;

            revealStartY = 0;
            revealEndY = 0;

            revealTotalTiles = 0;
            revealProcessedTiles = 0;

            effectTimer = 0;
        }

        // ============================================================
        // 開始
        // ============================================================

        public static void StartReveal(Player player)
        {
            if (revealing)
                return;

            if (Main.dedServ)
                return;

            // --------------------------------------------------------
            // ダンジョンが存在するか
            // --------------------------------------------------------

            if (Main.dungeonX <= 0 ||
                Main.dungeonX >= Main.maxTilesX)
            {
                Main.NewText(
                    "ダンジョンの位置を取得できませんでした。",
                    Color.OrangeRed
                );

                return;
            }

            if (Main.dungeonY <= 0 ||
                Main.dungeonY >= Main.maxTilesY)
            {
                Main.NewText(
                    "ダンジョンの位置を取得できませんでした。",
                    Color.OrangeRed
                );

                return;
            }

            ownerPlayer = player.whoAmI;

            // ========================================================
            // 探索範囲
            // ========================================================

            searchStartX =
                Math.Max(
                    0,
                    Main.dungeonX - SearchRadiusX
                );

            searchEndX =
                Math.Min(
                    Main.maxTilesX - 1,
                    Main.dungeonX + SearchRadiusX
                );

            searchStartY =
                Math.Max(
                    0,
                    Main.dungeonY - SearchRadiusY
                );

            searchEndY =
                Math.Min(
                    Main.maxTilesY - 1,
                    Main.dungeonY + SearchRadiusY
                );

            searchCurrentX = searchStartX;
            searchCurrentY = searchStartY;

            searchProcessedTiles = 0;

            searchTotalTiles =
                (long)(searchEndX - searchStartX + 1) *
                (searchEndY - searchStartY + 1);

            dungeonFound = false;

            dungeonMinX = Main.maxTilesX;
            dungeonMaxX = 0;

            dungeonMinY = Main.maxTilesY;
            dungeonMaxY = 0;

            effectTimer = 0;

            state = State.Searching;

            revealing = true;

            // ========================================================
            // 開始音
            // ========================================================

            SoundEngine.PlaySound(
                SoundID.Item29,
                player.Center
            );

            Main.NewText(
                "ダンジョンの構造を解析しています……",
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
            // 探索
            // ========================================================

            if (state == State.Searching)
            {
                ProcessDungeonSearch();
            }
            else if (state == State.Revealing)
            {
                ProcessDungeonReveal();
            }

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
        // ダンジョン探索
        // ============================================================

        private static void ProcessDungeonSearch()
        {
            int processedThisFrame = 0;

            while (
                processedThisFrame < TilesPerFrame &&
                searchCurrentY <= searchEndY)
            {
                CheckDungeonTile(
                    searchCurrentX,
                    searchCurrentY
                );

                searchProcessedTiles++;
                processedThisFrame++;

                searchCurrentX++;

                if (searchCurrentX > searchEndX)
                {
                    searchCurrentX = searchStartX;
                    searchCurrentY++;
                }
            }

            // --------------------------------------------------------
            // 探索完了
            // --------------------------------------------------------

            if (searchCurrentY > searchEndY)
            {
                FinishDungeonSearch();
            }
        }

        // ============================================================
        // 1タイルをダンジョン判定
        // ============================================================

        private static void CheckDungeonTile(
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

            Tile tile = Main.tile[x, y];

            if (tile == null)
                return;

            bool dungeonTile =
                IsDungeonTile(tile.TileType);

            bool dungeonWall =
                IsDungeonWall(tile.WallType);

            if (!dungeonTile && !dungeonWall)
                return;

            dungeonFound = true;

            if (x < dungeonMinX)
                dungeonMinX = x;

            if (x > dungeonMaxX)
                dungeonMaxX = x;

            if (y < dungeonMinY)
                dungeonMinY = y;

            if (y > dungeonMaxY)
                dungeonMaxY = y;
        }

        // ============================================================
        // ダンジョンタイル判定
        // ============================================================

        private static bool IsDungeonTile(ushort tileType)
        {
            return
                tileType == TileID.BlueDungeonBrick ||
                tileType == TileID.GreenDungeonBrick ||
                tileType == TileID.PinkDungeonBrick;
        }

        // ============================================================
        // ダンジョン壁判定
        // ============================================================

        private static bool IsDungeonWall(ushort wallType)
        {
            return
                // ========================================================
                // Brick
                // ========================================================

                wallType == WallID.BlueDungeonUnsafe ||
                wallType == WallID.GreenDungeonUnsafe ||
                wallType == WallID.PinkDungeonUnsafe ||

                // ========================================================
                // Slab
                // ========================================================

                wallType == WallID.BlueDungeonSlabUnsafe ||
                wallType == WallID.GreenDungeonSlabUnsafe ||
                wallType == WallID.PinkDungeonSlabUnsafe ||

                // ========================================================
                // Tiled
                // ========================================================

                wallType == WallID.BlueDungeonTileUnsafe ||
                wallType == WallID.GreenDungeonTileUnsafe ||
                wallType == WallID.PinkDungeonTileUnsafe;
        }

        // ============================================================
        // 探索完了
        // ============================================================

        private static void FinishDungeonSearch()
        {
            if (!dungeonFound)
            {
                Main.NewText(
                    "ダンジョンを検出できませんでした。",
                    Color.OrangeRed
                );

                Reset();

                return;
            }

            // ========================================================
            // 解析範囲に少しだけ余裕を持たせる
            // ========================================================

            revealStartX =
                Math.Max(
                    0,
                    dungeonMinX - RevealPadding
                );

            revealEndX =
                Math.Min(
                    Main.maxTilesX - 1,
                    dungeonMaxX + RevealPadding
                );

            revealStartY =
                Math.Max(
                    0,
                    dungeonMinY - RevealPadding
                );

            revealEndY =
                Math.Min(
                    Main.maxTilesY - 1,
                    dungeonMaxY + RevealPadding
                );

            revealCurrentX = revealStartX;
            revealCurrentY = revealStartY;

            revealProcessedTiles = 0;

            revealTotalTiles =
                (long)(revealEndX - revealStartX + 1) *
                (revealEndY - revealStartY + 1);

            state = State.Revealing;

            Main.NewText(
                "ダンジョン内部をマップに展開しています……",
                Color.LightSkyBlue
            );
        }

        // ============================================================
        // マップ公開処理
        // ============================================================

        private static void ProcessDungeonReveal()
        {
            int processedThisFrame = 0;

            while (
                processedThisFrame < TilesPerFrame &&
                revealCurrentY <= revealEndY)
            {
                RevealDungeonArea(
                    revealCurrentX,
                    revealCurrentY
                );

                revealProcessedTiles++;
                processedThisFrame++;

                revealCurrentX++;

                if (revealCurrentX > revealEndX)
                {
                    revealCurrentX = revealStartX;
                    revealCurrentY++;
                }
            }

            if (revealCurrentY > revealEndY)
            {
                FinishReveal();
            }
        }

        // ============================================================
        // ダンジョン内部をマップへ登録
        // ============================================================

        private static void RevealDungeonArea(
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

            Tile tile = Main.tile[x, y];

            if (tile == null)
                return;

            // ============================================================
            // ダンジョンタイル
            // ============================================================

            bool dungeonTile =
                IsDungeonTile(tile.TileType);

            // ============================================================
            // ダンジョン壁
            //
            // ダンジョン内部の空間は、
            // 背景壁によって「ダンジョン内部」であることを判定できる。
            //
            // そのため、周囲1マスだけを見る必要はない。
            // ============================================================

            bool dungeonWall =
                IsDungeonWall(tile.WallType);

            // ============================================================
            // ダンジョンそのものだけを公開
            // ============================================================

            if (dungeonTile || dungeonWall)
            {
                RevealTile(x, y);
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

            // ========================================================
            // Lighting.GetColor() は使用しない
            //
            // ダンジョン内部が暗くても、
            // 完全公開されたマップとして表示する。
            // ========================================================

            Main.Map.Update(
                x,
                y,
                255
            );

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
            // マップ表示を即時更新
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
                "ダンジョン全体の地図が明らかになりました！",
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

            string text;

            if (state == State.Searching)
            {
                text =
                    $"ダンジョンを解析中... {progress * 100f:0.0}%";
            }
            else
            {
                text =
                    $"ダンジョン地図を展開中... {progress * 100f:0.0}%";
            }

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

            // ========================================================
            // プログレスバー
            // ========================================================

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

            // ========================================================
            // ダンジョン位置
            // ========================================================

            if (dungeonFound)
            {
                string range =
                    $"X {dungeonMinX}～{dungeonMaxX}  " +
                    $"Y {dungeonMinY}～{dungeonMaxY}";

                Vector2 rangeSize =
                    Terraria.GameContent.FontAssets.MouseText.Value
                        .MeasureString(range);

                Utils.DrawBorderString(
                    spriteBatch,
                    range,
                    new Vector2(
                        Main.screenWidth / 2f -
                        rangeSize.X / 2f,
                        145f
                    ),
                    Color.LightGray
                );
            }
        }
    }
}