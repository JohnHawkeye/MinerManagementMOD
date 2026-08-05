using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.Items
{
    public class BioBombItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item19;

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(copper: 50);
            Item.consumable = true;
            Item.maxStack = 999;

            Item.shoot = ModContent.ProjectileType<Projectiles.BioBombProjectile>();
            Item.shootSpeed = 8f;

            Item.value = Item.buyPrice(0, 0, 10);
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player player)
        {

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(50);

            recipe.AddIngredient(ItemID.MeteoriteBar, 1);
            recipe.AddIngredient(ItemID.DirtBlock, 100);
            recipe.AddIngredient(ItemID.StoneBlock, 100);
            recipe.AddIngredient(ItemID.Bomb, 50);
            recipe.AddIngredient<CopperMinerCoin>(10);
            recipe.AddTile(TileID.Anvils);

            recipe.Register();
        }
    }
}