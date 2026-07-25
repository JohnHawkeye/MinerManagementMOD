using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class MiningMachineItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item1;

            Item.noMelee = true;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold:10);

            Item.shoot = ModContent.ProjectileType<Projectiles.MiningMachineProjectile>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player,
            Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            Projectile.NewProjectile(
                source,
                Main.MouseWorld,
                Vector2.Zero,
                type,
                0,
                0,
                player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            // 銅バージョン
            CreateRecipe()
                .AddIngredient(ItemID.CopperBar, 10)
                .AddIngredient(ItemID.Sapphire, 1)
                .AddIngredient(ItemID.GoldCoin, 10)
                .AddTile(TileID.Anvils)
                .Register();

            // ブリキバージョン
            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 10)
                .AddIngredient(ItemID.Sapphire, 1)
                .AddIngredient(ItemID.GoldCoin, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}