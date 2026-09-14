using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using MinerManagementMOD.Buffs;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Systems
{
    public class GreedCompassUISystem : ModSystem
    {
        // ============================================
        // 表示設定
        // ============================================

        // プレイヤー中心から矢印の中心までの距離
        // 128px = 8タイル
        private const float ArrowDistance = 128f;

        // DetectArrow.png の大きさ
        private const float ArrowScale = 1f;

        // ============================================
        // UIレイヤー
        // ============================================

        public override void ModifyInterfaceLayers(
            List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(
                layer => layer.Name.Equals(
                    "Vanilla: Mouse Text"
                )
            );

            if (index == -1)
                return;

            layers.Insert(
                index,
                new LegacyGameInterfaceLayer(
                    "MinerManagementMOD: Greed Compass",
                    delegate
                    {
                        DrawGreedCompass(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.Game
                )
            );
        }

        // ============================================
        // 貪欲のコンパス描画
        // ============================================

        private static void DrawGreedCompass(
            SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            // プレイヤーが存在しない
            if (!player.active || player.dead)
                return;

            // デテクトチェストが付いていなければ表示しない
            if (!player.HasBuff<DetectChestBuff>())
                return;

            GreedCompassPlayer compassPlayer =
                player.GetModPlayer<GreedCompassPlayer>();

            List<GreedCompassPlayer.ChestTarget> targets =
                compassPlayer.GetActiveTargets();

            if (targets.Count == 0)
                return;

            // ========================================
            // DetectArrow画像を取得
            // ========================================

            Texture2D arrowTexture =
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/UI/DetectArrow"
                ).Value;

            // ========================================
            // プレイヤーの画面上の位置
            // ========================================

            Vector2 playerScreenPosition =
                player.Center - Main.screenPosition;

            // ========================================
            // 最大5個の宝箱を処理
            // ========================================

            for (int i = 0; i < targets.Count; i++)
            {
                GreedCompassPlayer.ChestTarget target =
                    targets[i];

                // ------------------------------------
                // プレイヤー → 宝箱の方向
                // ------------------------------------

                Vector2 direction =
                    target.WorldPosition - player.Center;

                if (direction.LengthSquared() <= 1f)
                    continue;

                direction.Normalize();

                // ------------------------------------
                // プレイヤーから128px離れた位置
                // ------------------------------------

                Vector2 arrowPosition =
                    playerScreenPosition +
                    direction * ArrowDistance;

                // ------------------------------------
                // ▶画像を宝箱方向へ回転
                // ------------------------------------
                //
                // DetectArrow.png は右向き「▶」なので、
                // Rotation = 方向ベクトルの角度
                //

                float rotation =
                    direction.ToRotation();

                // ------------------------------------
                // 矢印を描画
                // ------------------------------------

                spriteBatch.Draw(
                    arrowTexture,
                    arrowPosition,
                    null,
                    Color.White,
                    rotation,
                    new Vector2(
                        arrowTexture.Width / 2f,
                        arrowTexture.Height / 2f
                    ),
                    ArrowScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}