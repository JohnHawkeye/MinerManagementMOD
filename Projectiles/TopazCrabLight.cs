using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class TopazCrabLight : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.timeLeft = 300;

            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(
                Projectile.Center,
                1.0f,
                0.9f,
                0.3f);
        }
    }
}