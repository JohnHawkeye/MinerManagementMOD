using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class Detonator : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.UseSound = SoundID.Item37;

            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 50);

            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            int detonated = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (!proj.active)
                    continue;

                if (proj.owner != player.whoAmI)
                    continue;

                int bomb1 = ModContent.ProjectileType<Projectiles.RemoteBombProj>();
                int bomb2 = ModContent.ProjectileType<Projectiles.RemoteBombProj2>();
                if (proj.type != bomb1 && proj.type != bomb2)
                    continue;

                proj.Kill();
                detonated++;
            }

            if (detonated > 0)
            {
                CombatText.NewText(
                    player.Hitbox,
                    Microsoft.Xna.Framework.Color.Orange,
                    $"起爆！ ({detonated})"
                );
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wire, 15)
                .AddIngredient(ItemID.IronBar, 5)
                .AddIngredient(ItemID.Ruby, 1)
                .AddIngredient<GoldMinerCoin>(10)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Wire, 15)
                .AddIngredient(ItemID.LeadBar, 5)
                .AddIngredient(ItemID.Ruby, 1)
                .AddIngredient<GoldMinerCoin>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}