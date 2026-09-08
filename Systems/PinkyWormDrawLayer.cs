using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Systems
{
    public class PinkyWormDrawLayer : PlayerDrawLayer
    {
        private Asset<Texture2D> headTexture;
        private Asset<Texture2D> bodyTexture;
        private Asset<Texture2D> tailTexture;


        // =========================================================
        // 表示条件
        // =========================================================

        public override bool GetDefaultVisibility(
            PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer
                .GetModPlayer<PinkyWormPlayer>()
                .PinkyWormActive;
        }


        // =========================================================
        // 描画位置
        // =========================================================
        //
        // 以前はMountFrontの子だったが、
        // HideDrawLayersでMountFrontも非表示になるため、
        // Vanilla描画の最後に配置する。
        // =========================================================

        public override Position GetDefaultPosition()
        {
            return PlayerDrawLayers.AfterLastVanillaLayer;
        }


        // =========================================================
        // 描画
        // =========================================================

        protected override void Draw(
            ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player =
                drawInfo.drawPlayer;

            PinkyWormPlayer worm =
                player.GetModPlayer<PinkyWormPlayer>();

            if (!worm.PinkyWormActive)
                return;

            LoadTextures();

            IReadOnlyList<Vector2> history =
                worm.PositionHistory;

            if (history.Count == 0)
                return;


            // =====================================================
            // 身体
            // =====================================================

            for (int segment =
                     worm.BodySegments - 1;
                 segment >= 0;
                 segment--)
            {
                Vector2 position =
                    GetSegmentPosition(
                        history,
                        segment + 1
                    );

                float rotation =
                    GetSegmentRotation(
                        history,
                        segment + 1
                    );

                DrawWormPart(
                    drawInfo,
                    bodyTexture.Value,
                    position,
                    rotation
                );
            }


            // =====================================================
            // 尻尾
            // =====================================================

            Vector2 tailPosition =
                GetSegmentPosition(
                    history,
                    worm.BodySegments + 1
                );

            float tailRotation =
                GetSegmentRotation(
                    history,
                    worm.BodySegments + 1
                );

            DrawWormPart(
                drawInfo,
                tailTexture.Value,
                tailPosition,
                tailRotation
            );


            // =====================================================
            // 頭
            // =====================================================

            Vector2 headPosition =
                player.Center;

            DrawWormPart(
                drawInfo,
                headTexture.Value,
                headPosition,
                worm.WormRotation
            );
        }


        // =========================================================
        // テクスチャ読み込み
        // =========================================================

        private void LoadTextures()
        {
            headTexture ??=
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/Mounts/PinkyWormHead"
                );

            bodyTexture ??=
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/Mounts/PinkyWormBody"
                );

            tailTexture ??=
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/Mounts/PinkyWormTale"
                );
        }


        // =========================================================
        // 身体節の位置取得
        // =========================================================

        private Vector2 GetSegmentPosition(
            IReadOnlyList<Vector2> history,
            int segment)
        {
            float distance =
                segment *
                PinkyWormPlayer.SegmentDistance;

            int index =
                FindHistoryIndex(
                    history,
                    distance
                );

            if (index >= history.Count)
                index = history.Count - 1;

            return history[index];
        }


        // =========================================================
        // 履歴から距離を検索
        // =========================================================

        private int FindHistoryIndex(
            IReadOnlyList<Vector2> history,
            float distance)
        {
            if (history.Count < 2)
                return 0;

            float accumulated = 0f;

            for (int i = 1;
                 i < history.Count;
                 i++)
            {
                accumulated +=
                    Vector2.Distance(
                        history[i - 1],
                        history[i]
                    );

                if (accumulated >= distance)
                    return i;
            }

            return history.Count - 1;
        }


        // =========================================================
        // 身体節の向き
        // =========================================================

        private float GetSegmentRotation(
            IReadOnlyList<Vector2> history,
            int segment)
        {
            int index =
                FindHistoryIndex(
                    history,
                    segment *
                    PinkyWormPlayer.SegmentDistance
                );

            int next =
                index + 1;

            if (next >= history.Count)
                next = history.Count - 1;

            if (index == next)
                return 0f;

            Vector2 delta =
                history[index] -
                history[next];

            if (delta.LengthSquared() < 0.001f)
                return 0f;

            return delta.ToRotation();
        }


        // =========================================================
        // ワームパーツ描画
        // =========================================================

        private void DrawWormPart(
            PlayerDrawSet drawInfo,
            Texture2D texture,
            Vector2 worldPosition,
            float rotation)
        {
            Vector2 screenPosition =
                worldPosition -
                Main.screenPosition;

            Color drawColor =
                Color.White;

            drawInfo.DrawDataCache.Add(
                new DrawData(
                    texture,
                    screenPosition,
                    null,
                    drawColor,
                    rotation,
                    texture.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0
                )
            );
        }
    }
}