using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class MossHornetStinger : ModProjectile
    {
        // =========================================================
        // Settings
        // =========================================================

        // 最大飛行距離
        private const float MaxDistance = 900f;

        // Acid Venom持続時間
        // 5秒
        private const int AcidVenomTime = 300;

        // =========================================================
        // Initialize
        // =========================================================

        private Vector2 startPosition;

        // =========================================================
        // SetDefaults
        // =========================================================

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 4;

            Projectile.aiStyle = 0;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType =
                DamageClass.Generic;

            Projectile.penetrate = -1;

            Projectile.tileCollide = true;

            Projectile.ignoreWater = true;

            Projectile.timeLeft = 180;

            Projectile.light = 0f;
        }

        // =========================================================
        // Spawn
        // =========================================================

        public override void OnSpawn(
            Terraria.DataStructures.IEntitySource source)
        {
            startPosition =
                Projectile.Center;

            Projectile.rotation =
                Projectile.velocity.ToRotation();
        }

        // =========================================================
        // AI
        // =========================================================

        public override void AI()
        {
            // 重力なし
            // velocityは一切変更しない

            Projectile.rotation =
                Projectile.velocity.ToRotation();

            // =====================================================
            // 一定距離で消滅
            // =====================================================

            if (Vector2.Distance(
                    startPosition,
                    Projectile.Center) >=
                MaxDistance)
            {
                Projectile.Kill();
                return;
            }
        }

        // =========================================================
        // Hit NPC
        // =========================================================

        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            // Acid Venom
            // 内部IDは BuffID.Venom
            target.AddBuff(
                BuffID.Venom,
                AcidVenomTime
            );
        }
    }
}