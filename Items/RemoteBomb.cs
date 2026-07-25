using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace MinerManagementMOD.Items
{
    public class RemoteBomb : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;

            Item.maxStack = 9999;
            Item.consumable = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 25;
            Item.useTime = 25;

            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<Projectiles.RemoteBombProj>();
            Item.shootSpeed = 8f;

            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.Bomb, 5)
                .AddIngredient(ItemID.Wire, 2)
                .AddIngredient(ItemID.Gel, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}