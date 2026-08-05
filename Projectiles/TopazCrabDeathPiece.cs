using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class TopazCrabDeathPiece : ModProjectile
    {

        public override string Texture =>
            "MinerManagementMOD/Assets/NPCs/TopazCrab_deathA";


        private int Piece =>
            (int)Projectile.ai[0];


        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;

            Projectile.timeLeft = 180;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }


        public override void AI()
        {

            Projectile.rotation += 0.15f;


            Projectile.velocity.Y += 0.25f;


            Lighting.AddLight(
                Projectile.Center,
                1f,
                0.7f,
                0.2f);


            if(Main.rand.NextBool(3))
            {
                Dust dust =
                    Dust.NewDustPerfect(
                        Projectile.Center,
                        DustID.GemTopaz,
                        Main.rand.NextVector2Circular(2f,2f),
                        100,
                        Color.White,
                        1.5f);


                dust.noGravity = true;
            }
        }


        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {

            switch(Piece)
            {
                case 0:
                    Projectile.netUpdate = true;
                    break;

                case 1:
                    Projectile.netUpdate = true;
                    break;

                case 2:
                    Projectile.netUpdate = true;
                    break;

                case 3:
                    Projectile.netUpdate = true;
                    break;
            }

        }


        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }


        public override string ToString()
        {
            return Piece switch
            {
                0 => "TopazCrab_deathA",
                1 => "TopazCrab_deathB",
                2 => "TopazCrab_deathC",
                _ => "TopazCrab_deathD"
            };
        }
    }
}