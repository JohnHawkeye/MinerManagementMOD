using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Tiles;
using MinerManagementMOD.TileEntities;

namespace MinerManagementMOD.Items
{
    public class OreSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 999;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Green;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;

            Item.useTurn = true;
            Item.autoReuse = false;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            int x = Player.tileTargetX;
            int y = Player.tileTargetY;

            if (!WorldGen.InWorld(x, y, 1))
                return false;

            Tile tile = Main.tile[x, y];

            // Stoneブロックにのみ植えられる
            if (!tile.HasTile || tile.TileType != TileID.Stone)
                return false;

            // 念のため既存TileEntityも確認
            if (OreSeedTileEntity.ExistsAt(x, y))
                return false;

            return true;
        }

        public override bool? UseItem(Player player)
        {
            int x = Player.tileTargetX;
            int y = Player.tileTargetY;

            if (!WorldGen.InWorld(x, y, 1))
                return false;

            Tile tile = Main.tile[x, y];

            // 念のため再確認
            if (!tile.HasTile || tile.TileType != TileID.Stone)
                return false;

            // マルチプレイヤーではサーバー側で処理
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            // Stone → OreSeedTile
            tile.TileType = (ushort)ModContent.TileType<OreSeedTile>();
            tile.HasTile = true;

            tile.Slope = SlopeType.Solid;
            tile.IsHalfBlock = false;

            tile.TileFrameX = 0;
            tile.TileFrameY = 0;

            WorldGen.SquareTileFrame(x, y);

            // TileEntityを生成して初期化
            OreSeedTileEntity.PlaceAt(x, y);

            // サーバーからクライアントへタイル同期
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(
                    -1,
                    x,
                    y,
                    1
                );
            }

            return true;
        }
    }
}