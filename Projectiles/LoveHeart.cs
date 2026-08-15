using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class LoveHeart : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;

            Projectile.timeLeft = 60; // 約1秒
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            // ゆっくり上へ
            Projectile.velocity *= 0.96f;
            Projectile.velocity.Y -= 0.02f;

            // 徐々に透明になる
            Projectile.alpha += 4;

            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            // 少し大きくなる
            Projectile.scale += 0.003f;
        }
    }
}