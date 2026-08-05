using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class TopazCrabShell : ModProjectile
    {
        public override string Texture =>
            "MinerManagementMOD/Assets/NPCs/TopazCrab_dbA";

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 90;

            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.rotation +=
                Projectile.velocity.X * 0.05f;

            Projectile.velocity.Y += 0.25f;
        }

        public override bool PreDraw(
            ref Color lightColor)
        {
            string texturePath =
                "MinerManagementMOD/Assets/NPCs/TopazCrab_dbA";

            switch ((int)Projectile.ai[0])
            {
                case 1:
                    texturePath =
                        "MinerManagementMOD/Assets/NPCs/TopazCrab_dbB";
                    break;

                case 2:
                    texturePath =
                        "MinerManagementMOD/Assets/NPCs/TopazCrab_dbC";
                    break;

                case 3:
                    texturePath =
                        "MinerManagementMOD/Assets/NPCs/TopazCrab_dbD";
                    break;
            }

            Texture2D texture =
                ModContent.Request<Texture2D>(
                    texturePath).Value;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                texture.Size() / 2f,
                1f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}