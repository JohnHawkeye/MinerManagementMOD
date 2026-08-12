using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Projectiles;

namespace MinerManagementMOD.Items
{
    public class DrillClusterItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            // 射撃武器として設定
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 18;           // EoW撃破後くらいの目安
            Item.knockBack = 4f;
            Item.crit = 4;

            Item.shoot = ModContent.ProjectileType<DrillClusterProjectile>();
            Item.shootSpeed = 6f; // ゆっくり飛ばす

            Item.useAmmo = ModContent.ItemType<DrillRocket>();  // 弾不要
            Item.noMelee = false;        // 武器としてダメージ判定を持たせる

            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 50);
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
            // マウス方向へ、プレイヤー中心から2ブロック(32px)先で発射
            Vector2 direction = velocity.SafeNormalize(Vector2.UnitX);
            Vector2 spawnPosition = player.Center + direction * 32f;

            Projectile.NewProjectile(
                source,
                spawnPosition,
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI
            );

            return false; // デフォルトの発射処理は行わない
        }

        public override void AddRecipes()
        {
            // シャドウスケール版(コラプション)
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient<GoldMinerCoin>(10)
                .AddTile(TileID.Anvils)
                .Register();

            // ティッシュサンプル版(クリムゾン)
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient<GoldMinerCoin>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}