using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Projectiles
{
    public class MagicHandProjectile : ModProjectile
    {
        private enum HandState
        {
            Extend,
            Return
        }

        private HandState state = HandState.Extend;
        private Vector2 startPosition;
        private int grabbedItem = -1;
        private bool collected = false;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0)
            {
                startPosition = player.Center;
                Projectile.localAI[0] = 1;
            }

            if (state == HandState.Extend)
            {
                CheckInteract();

                CheckItemGrab();

                float distance =
                    Vector2.Distance(
                        startPosition,
                        Projectile.Center);

                // 最大距離
                if (distance > 400f)
                {
                    state = HandState.Return;
                }
            }

            if (state == HandState.Return)
            {
                // 掴んだアイテムを手に固定
                if (grabbedItem != -1)
                {
                    Item item = Main.item[grabbedItem];

                    if (item.active)
                    {
                        item.Center = Projectile.Center;
                        item.velocity = Vector2.Zero;
                        item.noGrabDelay = 999;
                    }
                }

                Vector2 direction =
                    startPosition -
                    Projectile.Center;

                if (!collected && direction.Length() < 20f)
                {
                    collected = true;
                    // アイテム回収
                    if (grabbedItem != -1)
                    {
                        Item item = Main.item[grabbedItem];

                        if (item.active)
                        {
                            player.QuickSpawnItem(
                                player.GetSource_Misc("MagicHand"),
                                item.type,
                                item.stack
                            );
                            
                            item.TurnToAir();
                        }
                    }

                    Projectile.Kill();
                    return;
                }

                direction.Normalize();

                Projectile.velocity =
                    direction * 18f;

            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void CheckInteract()
        {
            Rectangle area = Projectile.Hitbox;


            int startX = area.Left / 16;
            int endX = area.Right / 16;

            int startY = area.Top / 16;
            int endY = area.Bottom / 16;


            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType == TileID.Containers ||
                    tile.TileType == TileID.Containers2 ||
                    tile.TileType == TileID.FakeContainers ||
                    tile.TileType == TileID.FakeContainers2
                    )
                    {
                        int chestX = x - tile.TileFrameX / 18 % 2;
                        int chestY = y - tile.TileFrameY / 18 % 2;

                        int chestIndex = Chest.FindChest(chestX, chestY);
                        if (chestIndex < 0)
                            continue;

                        if(Chest.IsLocked(chestX,chestY))
                            continue;

                        Player player = Main.player[Projectile.owner];

                        player.OpenChest(chestX, chestY, chestIndex);

                        Wiring.TripWire(chestX, chestY, 2, 2);
                        Wiring.HitSwitch(chestX, chestY);
                        ActivateSuccess();
                        return;
                    }

                    // スイッチ
                    if (tile.TileType == TileID.Switches)
                    {
                        Wiring.HitSwitch(x, y);

                        ActivateSuccess();
                        return;
                    }

                    // レバー
                    if (tile.TileType == TileID.Lever)
                    {
                        Wiring.HitSwitch(x, y);

                        ActivateSuccess();
                        return;
                    }

                    // デトネイター
                    if (tile.TileType == TileID.Detonator)
                    {
                        Wiring.HitSwitch(x, y);
                        ActivateSuccess();
                        return;
                    }


                    // ワイヤー系
                    if (tile.TileType == TileID.PressurePlates)
                    {
                        Wiring.HitSwitch(x, y);
                        ActivateSuccess();
                        return;
                    }
                }
            }
        }

        private void CheckItemGrab()
        {
            if (grabbedItem != -1)
                return;

            foreach (Item item in Main.item)
            {
                if (!item.active)
                    continue;

                // 他の取得処理中のアイテムは無視
                if (item.noGrabDelay > 0)
                    continue;

                float distance =
                    Vector2.Distance(
                        Projectile.Center,
                        item.Center);

                if (distance < 24f)
                {
                    grabbedItem = item.whoAmI;
                    state = HandState.Return;
                    Projectile.velocity = Vector2.Zero;
                    break;
                }
            }
        }

        private void ActivateSuccess()
        {
            Terraria.Audio.SoundEngine.PlaySound(
                SoundID.Mech,
                Projectile.Center
            );


            state = HandState.Return;

            Projectile.velocity = Vector2.Zero;
        }
    }
}