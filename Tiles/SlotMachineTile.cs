using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Tiles
{
    public class SlotMachineTile : ModTile
    {
        public override string Texture
            => "MinerManagementMOD/Assets/Tiles/SlotMachine";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.Origin = new Point16(0, 3);

            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
                16
            };

            TileObjectData.addTile(Type);

            AddMapEntry(
                new Color(50, 80, 150),
                CreateMapEntryName()
            );
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            Tile tile = Main.tile[i, j];

            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            int chairX = left;
            int chairY = top + 3;

            player.Bottom = new Vector2(
                chairX * 16 + 8,
                chairY * 16
            );

            ModContent.GetInstance<SlotMachineUISystem>().Show(
                chairX,chairY
            );

            return true;
        }
    }
}