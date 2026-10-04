using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Mounts;

namespace MinerManagementMOD.Items
{
    public class MossHornetMountItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = false;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Lime;

            // モスホーネットマウント
            Item.mountType = ModContent.MountType<MossHornetMount>();
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 20)
                .AddIngredient(ItemID.JungleSpores, 10)
                .AddIngredient(ItemID.Emerald, 10)
                .AddIngredient(ItemID.TatteredBeeWing, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}