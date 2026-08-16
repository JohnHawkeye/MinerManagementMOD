using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Commands
{
    public class TileDebugCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "tiledebug";

        public override string Usage => "/tiledebug";

        public override string Description =>
            "カーソル位置のTile情報を表示します。";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;

            // マウス位置をTile座標に変換
            Point tilePosition = Main.MouseWorld.ToTileCoordinates();

            int x = tilePosition.X;
            int y = tilePosition.Y;

            // ワールド範囲外チェック
            if (x < 0 || x >= Main.maxTilesX ||
                y < 0 || y >= Main.maxTilesY)
            {
                Main.NewText("Tileがワールド範囲外です.", Color.Red);
                return;
            }

            Tile tile = Main.tile[x, y];

            // Tileが存在しない場合
            if (!tile.HasTile)
            {
                Main.NewText(
                    $"TileDebug: ({x}, {y}) にTileはありません。",
                    Color.Gray
                );

                return;
            }

            Main.NewText("========== Tile Debug ==========", Color.Cyan);

            Main.NewText(
                $"位置: X={x}, Y={y}",
                Color.White
            );

            Main.NewText(
                $"TileType: {tile.TileType}",
                Color.Yellow
            );

            Main.NewText(
                $"TileFrameX: {tile.TileFrameX}",
                Color.LightGreen
            );

            Main.NewText(
                $"TileFrameY: {tile.TileFrameY}",
                Color.LightGreen
            );

            Main.NewText(
                $"IsHalfBlock: {tile.IsHalfBlock}",
                Color.Orange
            );

            Main.NewText(
                $"Slope: {tile.Slope}",
                Color.Orange
            );

            Main.NewText(
                $"HasTile: {tile.HasTile}",
                Color.White
            );

            Main.NewText(
                "================================",
                Color.Cyan
            );
        }
    }
}