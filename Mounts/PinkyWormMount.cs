using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Mounts
{
    public class PinkyWormMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            // ============================================
            // 基本設定
            // ============================================

            MountData.buff =
                ModContent.BuffType<Buffs.PinkyWormMountBuff>();

            // 通常の地上移動は使わない
            MountData.runSpeed = 0f;
            MountData.acceleration = 0f;

            // ジャンプ禁止
            MountData.jumpHeight = 0;
            MountData.jumpSpeed = 0f;

            // 落下ダメージなし
            MountData.fallDamage = 0f;

            // 通常の飛行処理も使わない
            MountData.flightTimeMax = 0;

            MountData.blockExtraJumps = true;

            // ============================================
            // プレイヤー位置調整
            // ============================================

            MountData.heightBoost = 0;

            MountData.totalFrames = 1;

            MountData.playerYOffsets = new int[]
            {
                0
            };

            MountData.standingFrameCount = 1;
            MountData.runningFrameCount = 1;

            MountData.inAirFrameCount = 1;
            MountData.flyingFrameCount = 1;

            MountData.bodyFrame = 0;

            // 通常のマウント画像は使用しない
            // ワーム本体は PinkyWormDrawLayer で描画する。
        }

        public override void SetMount(
            Player player,
            ref bool skipDust)
        {
            base.SetMount(
                player,
                ref skipDust
            );

            var worm =
                player.GetModPlayer<PinkyWormPlayer>();

            worm.StartWorm();

            skipDust = true;
        }

        public override void Dismount(
            Player player,
            ref bool skipDust)
        {
            var worm =
                player.GetModPlayer<PinkyWormPlayer>();

            worm.StopWorm();

            skipDust = true;

            base.Dismount(
                player,
                ref skipDust
            );
        }

        public override void UpdateEffects(
            Player player)
        {
            var worm =
                player.GetModPlayer<PinkyWormPlayer>();

            if (!worm.PinkyWormActive)
                return;

            // ============================================
            // 発光
            // ============================================

            Lighting.AddLight(
                player.Center,
                0.85f,
                0.20f,
                0.55f
            );

            // ============================================
            // 移動中のエフェクト
            // ============================================

            if (worm.IsMoving)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(
                        player.position,
                        player.width,
                        player.height,
                        DustID.PinkTorch
                    );

                    dust.noGravity = true;
                    dust.velocity *= 0.25f;
                    dust.scale = 0.7f;
                }
            }
        }
    }
}