using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.Projectiles
{
    public class RemoteBombProj : ModProjectile
    {
        private bool stuck = false;

        private int frameCounter;
        private bool pickedUp;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Explosive[Type] = true;

            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = -1;

            Projectile.timeLeft = 60 * 60 * 30; //30分

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            // アニメーション
            frameCounter++;

            if (frameCounter >= 8)
            {
                frameCounter = 0;

                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

            if (!stuck)
            {
                // 重力
                Projectile.velocity.Y += 0.25f;

                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;

                // 回転
                //Projectile.rotation += Projectile.velocity.X * 0.08f;
            }
            else
            {
                // 貼り付き中
                Projectile.velocity = Vector2.Zero;
            }

            if (Projectile.frame == 2)
            {
                Lighting.AddLight(
                    Projectile.Center,
                    0.8f,   // 赤
                    0.08f,  // 緑
                    0.08f); // 青

                SoundEngine.PlaySound(
                    new SoundStyle("MinerManagementMOD/Assets/Sounds/RemoteBombBeep"),
                    Projectile.Center
                );
            }
            if (stuck)
            {
                CheckRightClickPickup();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (stuck)
                return false;

            stuck = true;

            Projectile.velocity = Vector2.Zero;

            Projectile.tileCollide = false;

            Projectile.netUpdate = true;

            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void PrepareBombToBlow()
        {
            // 爆風の当たり判定
            Projectile.Resize(128, 128);

            // 爆発中だけダメージを与える
            Projectile.damage = 100;
            Projectile.knockBack = 8f;
        }

        public override void OnKill(int timeLeft)
        {
            if (pickedUp)
            {
                Item.NewItem(
                    Projectile.GetSource_Death(),
                    Projectile.getRect(),
                    ModContent.ItemType<Items.RemoteBomb>());

                return;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                int radius = 7;

                Point start = new Point(
                    (int)(Projectile.Center.X / 16f) - radius,
                    (int)(Projectile.Center.Y / 16f) - radius);

                Point end = new Point(
                    (int)(Projectile.Center.X / 16f) + radius,
                    (int)(Projectile.Center.Y / 16f) + radius);

                bool wallSplode = Projectile.ShouldWallExplode(
                    Projectile.Center,
                    radius,
                    start.X,
                    end.X,
                    start.Y,
                    end.Y);

                Projectile.ExplodeTiles(
                    Projectile.Center,
                    radius,
                    start.X,
                    end.X,
                    start.Y,
                    end.Y,
                    wallSplode);
            }
        }

        private void CheckRightClickPickup()
        {
            Player player = Main.player[Projectile.owner];

            // 右クリックしていない
            if (!Main.mouseRight || Main.mouseRightRelease == false)
                return;

            // インベントリ表示中は無効
            if (Main.playerInventory)
                return;

            // 少し近付かないと回収できない
            if (Vector2.Distance(player.Center, Projectile.Center) > 80f)
                return;

            // マウスが爆弾の上にあるか
            Rectangle pickupArea = Projectile.Hitbox;
            pickupArea.Inflate(12,12);

            if (!pickupArea.Contains(Main.MouseWorld.ToPoint()))
                return;

            pickedUp = true;

            SoundEngine.PlaySound(SoundID.Grab, Projectile.Center);

            Projectile.Kill();
        }
    }
}