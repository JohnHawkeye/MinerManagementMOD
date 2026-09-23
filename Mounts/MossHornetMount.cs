using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Mounts
{
    public class MossHornetMount : ModMount
    {

         public override string Texture
            => "MinerManagementMOD/Assets/Mounts/MossHornetMount";

        public override void SetStaticDefaults()
        {
            // =====================================================
            // Mount Buff
            // =====================================================

            MountData.buff =
                ModContent.BuffType<
                    Buffs.MossHornetMountBuff>();

            // =====================================================
            // Movement
            // =====================================================

            // 地上走行は使用しない
            MountData.runSpeed = 0f;
            MountData.acceleration = 0f;

            // ジャンプは使用しない
            MountData.jumpHeight = 0;
            MountData.jumpSpeed = 0f;

            // 落下ダメージなし
            MountData.fallDamage = 0f;

            // 飛行時間
            // 実質無制限
            MountData.flightTimeMax = 999999;

            // 追加ジャンプ禁止
            MountData.blockExtraJumps = true;

            MountData.heightBoost = 0;

            // =====================================================
            // Animation
            // =====================================================

            // 48 × 47 の2フレーム
            MountData.totalFrames = 2;

            MountData.playerYOffsets =
                new int[]
                {
                    0,
                    0
                };

            MountData.standingFrameCount = 2;
            MountData.runningFrameCount = 2;
            MountData.inAirFrameCount = 2;
            MountData.flyingFrameCount = 2;

            MountData.bodyFrame = 0;
        }

        // =========================================================
        // Mount start
        // =========================================================

        public override void SetMount(
            Player player,
            ref bool skipDust)
        {
            base.SetMount(
                player,
                ref skipDust
            );

            var hornet =
                player.GetModPlayer<
                    MossHornetPlayer>();

            hornet.StartMossHornet();

            skipDust = true;
        }

        // =========================================================
        // Mount end
        // =========================================================

        public override void Dismount(
            Player player,
            ref bool skipDust)
        {
            var hornet =
                player.GetModPlayer<
                    MossHornetPlayer>();

            hornet.StopMossHornet();

            skipDust = true;

            base.Dismount(
                player,
                ref skipDust
            );
        }

        // =========================================================
        // Update effects
        // =========================================================

        public override void UpdateEffects(
            Player player)
        {
            var hornet =
                player.GetModPlayer<
                    MossHornetPlayer>();

            if (!hornet.MossHornetActive)
                return;

        }
    }
}