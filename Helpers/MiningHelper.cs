using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Helpers
{
    public static class MiningHelper
    {
        public static readonly HashSet<int> MineableTiles =
        new()
        {
            // 土・石
            TileID.Dirt,
            TileID.Stone,
            TileID.RedMoss,
            TileID.GreenMoss,
            TileID.BrownMoss,
            TileID.BlueMoss,
            TileID.PurpleMoss,
            TileID.LavaMoss,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.CrimsonGrass,
            TileID.HallowedGrass,
            TileID.JungleGrass,
            TileID.Mud,
            TileID.MushroomGrass,
            TileID.Ebonstone,
            TileID.Crimstone,
            TileID.Pearlstone,
            TileID.BreakableIce,

            // 砂
            TileID.Sand,
            TileID.Sandstone,
            TileID.HardenedSand,
            TileID.HallowHardenedSand,
            TileID.CorruptHardenedSand,
            TileID.CrimsonHardenedSand,
            TileID.DesertFossil,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Pearlsand,

            // 雪・氷
            TileID.SnowBlock,
            TileID.IceBlock,
            TileID.CorruptIce,
            TileID.FleshIce,
            TileID.HallowedIce,

            // 灰・シルト・化石
            TileID.Ash,
            TileID.Silt,
            TileID.Slush,
            TileID.DesertFossil,

            // 粘土・蜂蜜ブロック
            TileID.ClayBlock,
            TileID.Hive,

            // 通常鉱石
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Gold,
            TileID.Platinum,
            TileID.Granite,
            TileID.Marble,

            // ハードモード鉱石
            TileID.Cobalt,
            TileID.Palladium,
            TileID.Mythril,
            TileID.Orichalcum,
            TileID.Adamantite,
            TileID.Titanium,

            // その他鉱石
            TileID.Demonite,
            TileID.Crimtane,
            TileID.Hellstone,
            TileID.Meteorite,
            TileID.Chlorophyte,

            // 宝石
            TileID.Amethyst,
            TileID.Topaz,
            TileID.Sapphire,
            TileID.Emerald,
            TileID.Ruby,
            TileID.Diamond,

            // ライフクリスタル・ライフフルーツ
            TileID.Heart,
            TileID.LifeFruit
        };


        // ハードモード限定鉱石(ヘルストーンより上位)
        public static readonly HashSet<int> HardmodeOnlyTiles = new()
            {
                TileID.Cobalt,
                TileID.Palladium,
                TileID.Mythril,
                TileID.Orichalcum,
                TileID.Adamantite,
                TileID.Titanium,
                TileID.Chlorophyte
            };

        public static bool CanMine(int tileType,bool allowHardmodeOre = true)
        {
            if(!MineableTiles.Contains(tileType))
                return false;

            if(!allowHardmodeOre && HardmodeOnlyTiles.Contains(tileType))
                return false;

            return true;
        }

    }
}