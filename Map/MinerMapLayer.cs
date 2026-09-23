using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;
using Terraria.UI;

namespace MinerManagementMOD.Map
{
    public class MinerMapLayer : ModMapLayer
    {
        // ==========================================
        // マップアイコン
        // ==========================================

        private Texture2D BaseIcon
        {
            get
            {
                return ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Map/MinerBaseMap"
                ).Value;
            }
        }

        private Texture2D OutpostIcon
        {
            get
            {
                return ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Map/MinerOutpostMap"
                ).Value;
            }
        }


        // ==========================================
        // マップ描画
        // ==========================================

        public override void Draw(
            ref MapOverlayDrawContext context,
            ref string text)
        {
            // ==========================================
            // Miner Base
            // ==========================================

            if (MinerStationSystem.MinerBasePosition.X >= 0 &&
                MinerStationSystem.MinerBasePosition.Y >= 0)
            {
                DrawStation(
                    ref context,
                    MinerStationSystem.MinerBasePosition,
                    BaseIcon,
                    "Miner Base",
                    ref text,
                    false
                );
            }


            // ==========================================
            // Miner Outpost
            // ==========================================

            foreach (Point16 position in
                     MinerStationSystem.OutpostPositions)
            {
                DrawStation(
                    ref context,
                    position,
                    OutpostIcon,
                    "Miner Outpost",
                    ref text,
                    true
                );
            }
        }


        // ==========================================
        // Station描画
        // ==========================================

        private void DrawStation(
            ref MapOverlayDrawContext context,
            Point16 position,
            Texture2D texture,
            string stationName,
            ref string text,
            bool isOutpost)
        {
            // ==========================================
            // Tile座標
            //
            // positionは2×4タイルの左上
            // ==========================================

            Vector2 mapPosition = new Vector2(
                position.X +1f,
                position.Y+2f
            );


            // ==========================================
            // 画像を1フレームとして扱う
            // ==========================================

            SpriteFrame frame = new SpriteFrame(
                1,
                1,
                0,
                0
            );


            // ==========================================
            // マップへ描画
            // ==========================================

            MapOverlayDrawContext.DrawResult result =
                context.Draw(
                    texture,
                    mapPosition,
                    Color.White,
                    frame,
                    1f,
                    1.2f,
                    Alignment.Center
                );


            // ==========================================
            // マウスオーバー
            // ==========================================

            if (result.IsMouseOver)
            {
                text = stationName;


                // ==========================================
                // Outpost選択中のみクリック可能
                // ==========================================

                if (isOutpost &&
                    MinerStationSystem.SelectingOutpost &&
                    Main.mouseLeft &&
                    Main.mouseLeftRelease)
                {
                    // ======================================
                    // 選択モード終了
                    // ======================================

                    MinerStationSystem.SelectingOutpost = false;


                    // ======================================
                    // マップを閉じる
                    // ======================================

                    Main.mapFullscreen = false;


                    // ======================================
                    // Outpostへテレポート
                    //
                    // positionは2×4タイルの左上
                    //
                    // 中央：
                    // X = +16px
                    // Y = +32px
                    // ======================================

                    Vector2 teleportPosition =
                        new Vector2(
                            position.X * 16f + 16f,
                            position.Y * 16f + 32f
                        );


                    Main.LocalPlayer.Teleport(
                        teleportPosition,
                        0
                    );
                }
            }
        }
    }
}