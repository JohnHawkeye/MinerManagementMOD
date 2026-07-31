using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace MinerManagementMOD.Items
{
    public class MagicHand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.noMelee = true;

            Item.DamageType = DamageClass.Generic;

            Item.shoot = ModContent.ProjectileType<Projectiles.MagicHandProjectile>();
            Item.shootSpeed = 16f;

            Item.UseSound = SoundID.Item8;

            Item.rare = ItemRarityID.Blue;
        }


        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {

            Projectile.NewProjectile(
                source,
                player.Center,
                velocity,
                type,
                0,
                0,
                player.whoAmI
            );


            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient<GoldMinerCoin>(10);
            recipe.AddTile(TileID.Anvils);

            recipe.Register();
        }
    }
}