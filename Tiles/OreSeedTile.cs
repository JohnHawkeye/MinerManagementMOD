using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.TileEntities;

namespace MinerManagementMOD.Tiles
{
    public class OreSeedTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileLighted[Type] = true;

            Main.tileFrameImportant[Type] = true;

            DustType = DustID.Stone;
            HitSound = SoundID.Tink;

            AddMapEntry(
                new Color(110, 110, 110),
                CreateMapEntryName()
            );
        }

        public override void ModifyLight(
            int i,
            int j,
            ref float r,
            ref float g,
            ref float b)
        {
            r = 0.35f;
            g = 0.35f;
            b = 0.35f;
        }

        public override bool CanPlace(int i, int j)
        {
            // 通常のブロック配置は禁止。
            // OreSeedアイテムからのみ生成する。
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return true;
        }

        public override void KillTile(
            int i,
            int j,
            ref bool fail,
            ref bool effectOnly,
            ref bool noItem)
        {
            // OreSeedTileが破壊されたら
            // 対応するTileEntityも削除する。
            if (!fail && !effectOnly)
            {
                OreSeedTileEntity.RemoveAt(i, j);
            }

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(
                    -1,
                    i,
                    j,
                    1
                );
            }
        }

        public override IEnumerable<Item> GetItemDrops(
            int i,
            int j)
        {
            yield return new Item(
                ModContent.ItemType<Items.OreSeed>()
            );
        }
    }
}