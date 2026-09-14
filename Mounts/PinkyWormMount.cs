using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Mounts
{
    public class PinkyWormMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.buff =
                ModContent.BuffType<
                    Buffs.PinkyWormMountBuff>();

            MountData.runSpeed = 0f;
            MountData.acceleration = 0f;

            MountData.jumpHeight = 0;
            MountData.jumpSpeed = 0f;

            MountData.fallDamage = 0f;

            MountData.flightTimeMax = 0;

            MountData.blockExtraJumps = true;

            MountData.heightBoost = 0;

            MountData.totalFrames = 1;

            MountData.playerYOffsets =
                new int[]
                {
                    0
                };

            MountData.standingFrameCount = 1;
            MountData.runningFrameCount = 1;
            MountData.inAirFrameCount = 1;
            MountData.flyingFrameCount = 1;

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

            var worm =
                player.GetModPlayer<
                    PinkyWormPlayer>();

            worm.StartWorm();

            skipDust = true;
        }

        // =========================================================
        // Mount end
        // =========================================================

        public override void Dismount(
            Player player,
            ref bool skipDust)
        {
            var worm =
                player.GetModPlayer<
                    PinkyWormPlayer>();

            worm.StopWorm();

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
            var worm =
                player.GetModPlayer<
                    PinkyWormPlayer>();

            if (!worm.PinkyWormActive)
                return;

            Lighting.AddLight(
                player.Center,
                0.85f,
                0.20f,
                0.55f
            );

            if (worm.IsMoving)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust =
                        Dust.NewDustDirect(
                            player.position,
                            player.width,
                            player.height,
                            DustID.PinkTorch
                        );

                    dust.noGravity = true;

                    dust.velocity *=
                        0.25f;

                    dust.scale = 0.7f;
                }
            }
        }
    }
}
