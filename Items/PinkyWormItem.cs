using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class PinkyWormItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;

            // マウントアイテム
            Item.mountType = ModContent.MountType<Mounts.PinkyWormMount>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PinkGel, 20)
                .AddIngredient(ItemID.Worm, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}