using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.Projectiles
{
    public class GuardSwordProjectile : ModProjectile
    {
        private int swingTimer;

        private const int SwingTime = 20;


        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;

            Projectile.friendly = true;

            Projectile.penetrate = -1;

            Projectile.timeLeft = SwingTime;

            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;

            Projectile.DamageType =
                DamageClass.Generic;
        }


        public override void AI()
        {
            int npcID =
                (int)Projectile.ai[0];


            if (npcID < 0 ||
                npcID >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }


            NPC npc =
                Main.npc[npcID];


            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }


            swingTimer++;


            float progress =
                swingTimer /
                (float)SwingTime;


            float angle;


            if (Projectile.ai[1] == 1)
            {
                angle =
                    MathHelper.Lerp(
                        -2.5f,
                        1.2f,
                        progress);
            }
            else
            {
                angle =
                    MathHelper.Lerp(
                        2.5f,
                        -1.2f,
                        progress);
            }


            Projectile.rotation = angle;


            Vector2 offset =
                Projectile.rotation.ToRotationVector2()
                * 45f;


            Projectile.Center =
                npc.Center + offset;


            Projectile.direction =
                npc.direction;


            Projectile.spriteDirection =
                npc.direction;


            Projectile.rotation =
                angle;


            if (swingTimer == 8)
            {
                SoundEngine.PlaySound(
                    SoundID.Item1,
                    Projectile.Center);
            }
        }



        public override bool? CanHitNPC(NPC target)
        {
            return target.friendly ?
                false :
                true;
        }



        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            Projectile.damage = 0;
        }



        public override bool PreDraw(
            ref Color lightColor)
        {
            Texture2D texture =
                ModContent.Request<Texture2D>(
                "MinerManagementMOD/Projectiles/GuardSwordProjectile")
                .Value;


            Main.EntitySpriteDraw(
                texture,
                Projectile.Center
                - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                new Vector2(8f,texture.Height/2f),
                1f,
                Projectile.spriteDirection == -1 ?
                SpriteEffects.FlipHorizontally :
                SpriteEffects.None);


            return false;
        }
    }
}