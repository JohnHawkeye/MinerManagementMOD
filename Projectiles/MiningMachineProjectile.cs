using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class MiningMachineProjectile : ModProjectile
    {
        private enum MachineState
        {
            Waiting,
            Mining,
            Idle
        }

        private MachineState state = MachineState.Waiting;

        private int timer;
        private int miningTimer;
        private int soundTimer;
        private int sparkTimer;
        private bool pickedUp;

        private Point? currentTarget;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 48;

            Projectile.timeLeft = 36000;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Lighting.AddLight(
                Projectile.Center,
                0.25f,   // 赤
                0.45f,   // 緑
                0.80f);  // 青

            Projectile.velocity = Vector2.Zero;

            timer++;

            //----------------------------------------------------
            // 待機中
            //----------------------------------------------------

            if (state == MachineState.Waiting)
            {
                Projectile.frame = 0;

                if (timer >= 120)
                {
                    timer = 0;
                    state = MachineState.Mining;
                }

                return;
            }

            //------------------
            //Idle state
            //-------------------------
            if (state == MachineState.Idle)
            {
                // 停止中は1コマ目
                Projectile.frame = 0;

                // 動かない
                Projectile.velocity = Vector2.Zero;

                // ピッケル回収判定だけ行う
                CheckRightClickPickup();

                return;
            }

            //----------------------------------------------------
            // 採掘中アニメ
            //----------------------------------------------------

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;

                Projectile.frame++;

                if (Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            //----------------------------------------------------
            // ドリル音
            //----------------------------------------------------

            soundTimer++;

            if (soundTimer >= 18)
            {
                soundTimer = 0;

                SoundEngine.PlaySound(
                    SoundID.Item23,
                    Projectile.Center);
            }

            //----------------------------------------------------
            // 掘削
            //----------------------------------------------------

            miningTimer++;

            if (miningTimer >= 20)
            {
                miningTimer = 0;

                MineNearestOre();
            }

            //----------------------------------------------------
            // 掘削中火花
            //----------------------------------------------------

            if (currentTarget.HasValue)
            {
                sparkTimer++;

                if (sparkTimer >= 3)
                {
                    sparkTimer = 0;

                    Vector2 dustPos =
                        new Vector2(
                            currentTarget.Value.X * 16 + 8,
                            currentTarget.Value.Y * 16 + 8);

                    Dust d = Dust.NewDustPerfect(
                        dustPos,
                        DustID.Electric);

                    d.velocity =
                        Main.rand.NextVector2Circular(1.5f, 1.5f);

                    d.noGravity = true;
                }
            }
            CheckRightClickPickup();
        }

        private void CheckRightClickPickup()
        {
            Player player = Main.player[Projectile.owner];

            // 右クリックしていない
            if (!Main.mouseRight)
                return;

            // インベントリを開いているときは無効
            if (Main.playerInventory)
                return;

            // プレイヤーとの距離
            if (Vector2.Distance(player.Center, Projectile.Center) > 80f)
                return;

            // マウスが掘削機の上にあるか
            if (!Projectile.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                return;

            pickedUp = true;

            Projectile.Kill();
        }

        private void MineNearestOre()
        {
            int centerX = (int)(Projectile.Center.X / 16);
            int centerY = (int)(Projectile.Center.Y / 16);

            Point? target = null;

            float bestDistance = float.MaxValue;

            for (int x = centerX - 6; x <= centerX + 6; x++)
            {
                for (int y = centerY - 6; y <= centerY + 6; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    int dx = x - centerX;
                    int dy = y - centerY;

                    if (dx * dx + dy * dy > 36)
                        continue;

                    Tile tile = Main.tile[x, y];

                    if (tile == null || !tile.HasTile)
                        continue;

                    if (!CanMine(tile.TileType))
                        continue;

                    float distance =
                        Vector2.Distance(
                            new Vector2(centerX, centerY),
                            new Vector2(x, y));

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        target = new Point(x, y);
                    }
                }
            }

            currentTarget = target;

            if (!target.HasValue)
            {
                currentTarget = null;
                state = MachineState.Idle;
                Projectile.frame = 0;
                return;
            }

            WorldGen.KillTile(
                target.Value.X,
                target.Value.Y);

            if (!Main.tile[target.Value.X, target.Value.Y].HasTile)
            {
                Vector2 dustPos =
                    new Vector2(
                        target.Value.X * 16 + 8,
                        target.Value.Y * 16 + 8);

                // 火花
                for (int i = 0; i < 12; i++)
                {
                    Dust d = Dust.NewDustPerfect(
                        dustPos,
                        DustID.Electric);

                    d.velocity *= 2.5f;
                    d.noGravity = true;
                }

                // 石粉
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(
                        dustPos,
                        DustID.Stone);
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(
                        -1,
                        target.Value.X,
                        target.Value.Y);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (!pickedUp)
                return;

            Item.NewItem(
                Projectile.GetSource_Death(),
                Projectile.getRect(),
                ModContent.ItemType<Items.MiningMachineItem>());
        }

        private bool CanMine(ushort tileType)
        {
            switch (tileType)
            {
                case TileID.Copper:
                case TileID.Tin:

                case TileID.Iron:
                case TileID.Lead:

                case TileID.Silver:
                case TileID.Tungsten:

                case TileID.Gold:
                case TileID.Platinum:

                case TileID.Amethyst:
                case TileID.Topaz:
                case TileID.Sapphire:
                case TileID.Emerald:
                case TileID.Ruby:
                case TileID.Diamond:

                    return true;
            }

            return false;
        }
    }
}