using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Systems
{
    public class PinkyWormOrePickupSystem : GlobalItem
    {
        public override bool OnPickup(
            Item item,
            Player player)
        {
            if (item.stack <= 0)
                return true;

            var worm =
                player.GetModPlayer<PinkyWormPlayer>();

            if (!worm.PinkyWormActive)
                return true;

            if (!IsOreItem(item.type))
                return true;

            worm.AddCollectedOre(
                item.stack
            );

            return true;
        }

        private bool IsOreItem(int type)
        {
            switch (type)
            {
                case ItemID.CopperOre:
                case ItemID.TinOre:

                case ItemID.IronOre:
                case ItemID.LeadOre:

                case ItemID.SilverOre:
                case ItemID.TungstenOre:

                case ItemID.GoldOre:
                case ItemID.PlatinumOre:

                case ItemID.Meteorite:

                case ItemID.DemoniteOre:
                case ItemID.CrimtaneOre:

                case ItemID.Hellstone:

                case ItemID.CobaltOre:
                case ItemID.PalladiumOre:

                case ItemID.MythrilOre:
                case ItemID.OrichalcumOre:

                case ItemID.AdamantiteOre:
                case ItemID.TitaniumOre:

                case ItemID.ChlorophyteOre:

                case ItemID.LunarOre:
                    return true;
            }

            return false;
        }
    }
}