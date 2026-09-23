using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Projectiles;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;

namespace MinerManagementMOD.Common.Players
{
    public class MossHornetPlayer : ModPlayer
    {
        // =========================================================
        // Settings
        // =========================================================

        // 最大飛行速度
        private const float MaxSpeed = 8f;

        // 加速度
        private const float Acceleration = 0.35f;

        // 無操作時の減速
        private const float BrakeAmount = 0.20f;

        // 毒針
        private const int StingerDamage = 80;

        private const float StingerKnockBack = 2f;

        private const float StingerSpeed = 14f;

        // 15フレームに1発
        private const int StingerCooldownMax = 15;

        // =========================================================
        // Active
        // =========================================================

        public bool MossHornetActive;

        // =========================================================
        // Movement
        // =========================================================

        private Vector2 hornetPosition;

        private Vector2 hornetVelocity;

        // =========================================================
        // Stinger
        // =========================================================

        private int stingerCooldown;

        // =========================================================
        // Initialize
        // =========================================================

        public override void Initialize()
        {
            MossHornetActive = false;

            hornetPosition =
                Vector2.Zero;

            hornetVelocity =
                Vector2.Zero;

            stingerCooldown = 0;
        }

        // =========================================================
        // Mount start
        // =========================================================

        public void StartMossHornet()
        {
            MossHornetActive = true;

            hornetPosition =
                Player.position;

            hornetVelocity =
                Vector2.Zero;

            stingerCooldown = 0;

            Player.gravity = 0f;
            Player.velocity = Vector2.Zero;

            Player.noFallDmg = true;
        }

        // =========================================================
        // Mount end
        // =========================================================

        public void StopMossHornet()
        {
            MossHornetActive = false;

            hornetVelocity =
                Vector2.Zero;

            Player.velocity =
                Vector2.Zero;

            Player.gravity = 0f;

            stingerCooldown = 0;
        }

        // =========================================================
        // PreUpdate
        // =========================================================

        public override void PreUpdate()
        {
            if (!MossHornetActive)
                return;

            if (Player.dead)
            {
                hornetVelocity =
                    Vector2.Zero;

                return;
            }

            // 専用座標をPlayerへ反映
            Player.position =
                hornetPosition;

            // 重力なし
            Player.gravity = 0f;

            // 落下ダメージなし
            Player.noFallDmg = true;

            // 通常のPlayer速度は使用しない
            Player.velocity =
                Vector2.Zero;

            // 毒針クールダウン
            if (stingerCooldown > 0)
            {
                stingerCooldown--;
            }
        }

        // =========================================================
        // PreUpdateMovement
        // =========================================================

        public override void PreUpdateMovement()
        {
            if (!MossHornetActive)
                return;

            // Terraria標準の歩行・ジャンプを停止
            Player.velocity =
                Vector2.Zero;

            Player.gravity = 0f;

            Player.oldVelocity =
                Vector2.Zero;
        }

        // =========================================================
        // PostUpdate
        // =========================================================

        public override void PostUpdate()
        {
            if (!MossHornetActive)
                return;

            if (Player.dead)
                return;

            Player.gravity = 0f;
            Player.noFallDmg = true;

            // =====================================================
            // 移動入力
            // =====================================================

            Vector2 input =
                Vector2.Zero;

            if (Player.controlLeft)
                input.X -= 1f;

            if (Player.controlRight)
                input.X += 1f;

            if (Player.controlUp)
                input.Y -= 1f;

            if (Player.controlDown)
                input.Y += 1f;

            // =====================================================
            // 加速
            // =====================================================

            if (input != Vector2.Zero)
            {
                input.Normalize();

                hornetVelocity +=
                    input * Acceleration;

                // 最大速度
                if (hornetVelocity.Length() >
                    MaxSpeed)
                {
                    hornetVelocity =
                        Vector2.Normalize(
                            hornetVelocity
                        ) * MaxSpeed;
                }
            }
            else
            {
                // =================================================
                // 無操作時の減速
                // =================================================

                hornetVelocity *=
                    (1f - BrakeAmount);

                if (hornetVelocity.Length() <
                    0.05f)
                {
                    hornetVelocity =
                        Vector2.Zero;
                }
            }

            // =====================================================
            // タイル衝突
            // =====================================================

            hornetVelocity =
                Collision.TileCollision(
                    hornetPosition,
                    hornetVelocity,
                    Player.width,
                    Player.height,
                    false,
                    false,
                    (int)Player.gravDir
                );

            // =====================================================
            // 実際に移動
            // =====================================================

            hornetPosition +=
                hornetVelocity;

            Player.position =
                hornetPosition;

            // =====================================================
            // プレイヤーの向き
            // =====================================================

            if (hornetVelocity.X > 0.05f)
            {
                Player.direction = 1;
            }
            else if (hornetVelocity.X < -0.05f)
            {
                Player.direction = -1;
            }

            // =====================================================
            // 通常Playerの速度は使用しない
            // =====================================================

            Player.velocity =
                Vector2.Zero;

            // =====================================================
            // 毒針
            // =====================================================

            TryFireStinger();
        }

        // =========================================================
        // Stinger
        // =========================================================

        private void TryFireStinger()
        {
            if (!MossHornetActive)
                return;

            // 右クリックを押している間
            if (!Main.mouseLeft)
                return;

            // クールダウン
            if (stingerCooldown > 0)
                return;

            // マウス方向
            Vector2 direction =
                Main.MouseWorld -
                Player.Center;

            if (direction == Vector2.Zero)
                return;

            direction.Normalize();

            // 毒針の出現位置
            Vector2 spawnPosition =
                Player.Center +
                direction * 26f;

            Vector2 velocity =
                direction *
                StingerSpeed;

            // 自分のクライアントだけが発射する
            if (Player.whoAmI != Main.myPlayer)
                return;

            Projectile.NewProjectile(
                Player.GetSource_Misc(
                    "MossHornetStinger"
                ),
                spawnPosition,
                velocity,
                ModContent.ProjectileType<
                    MossHornetStinger>(),
                StingerDamage,
                StingerKnockBack,
                Player.whoAmI
            );

            SoundEngine.PlaySound(
                SoundID.Item17,
                Player.Center
            );

            stingerCooldown =
                StingerCooldownMax;
        }

        // =========================================================
        // Item use restriction
        // =========================================================

        public override bool CanUseItem(
            Item item)
        {
            if (MossHornetActive)
            {
                // 通常武器を使用禁止
                if (item.damage > 0)
                    return false;
            }

            return true;
        }
        public override void HideDrawLayers(
            PlayerDrawSet drawInfo)
        {
            if (!MossHornetActive)
                return;

            PlayerDrawLayer hornetLayer =
                ModContent.GetInstance<
                    Systems.MossHornetDrawLayer>();

            foreach (PlayerDrawLayer layer
                     in PlayerDrawLayerLoader.Layers)
            {
                if (layer == hornetLayer)
                    continue;

                layer.Hide();
            }
        }
    }
}