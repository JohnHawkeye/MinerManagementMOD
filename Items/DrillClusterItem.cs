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

            // 射撃武器
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 18;
            Item.knockBack = 4f;
            Item.crit = 4;

            // デフォルトProjectile
            Item.shoot = ModContent.ProjectileType<DrillClusterProjectile>();
            Item.shootSpeed = 6f;

            // 基準となる弾薬タイプ
            Item.useAmmo = ModContent.ItemType<DrillRocket>();

            Item.noMelee = false;

            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
        }

        // =========================================================
        // DrillRocket / DrillRocket2 の両方を弾薬として認識させる
        // =========================================================
        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            int drillRocketType =
                ModContent.ItemType<DrillRocket>();

            int drillRocket2Type =
                ModContent.ItemType<DrillRocket2>();

            if (ammo.type == drillRocketType ||
                ammo.type == drillRocket2Type)
            {
                return true;
            }

            return false;
        }

        // =========================================================
        // 弾薬の種類によってProjectileを変更
        // =========================================================
        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            int projectileType;

            // -----------------------------------------------------
            // 実際に選択された弾薬を確認
            // -----------------------------------------------------
            if (source.AmmoItemIdUsed ==
                ModContent.ItemType<DrillRocket2>())
            {
                // ハードモード用
                projectileType =
                    ModContent.ProjectileType<DrillClusterProjectile2>();
            }
            else
            {
                // 通常版
                projectileType =
                    ModContent.ProjectileType<DrillClusterProjectile>();
            }

            // -----------------------------------------------------
            // マウス方向
            // -----------------------------------------------------
            Vector2 direction =
                velocity.SafeNormalize(Vector2.UnitX);

            // プレイヤー中心から2ブロック先
            Vector2 spawnPosition =
                player.Center + direction * 32f;

            // -----------------------------------------------------
            // Projectile発射
            // -----------------------------------------------------
            Projectile.NewProjectile(
                source,
                spawnPosition,
                velocity,
                projectileType,
                damage,
                knockback,
                player.whoAmI
            );

            // デフォルト発射を停止
            return false;
        }

        // =========================================================
        // レシピ
        // =========================================================
        public override void AddRecipes()
        {
            // -----------------------------------------------------
            // シャドウスケール版（コラプション）
            // -----------------------------------------------------
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient<GoldMinerCoin>(10)
                .AddTile(TileID.Anvils)
                .Register();

            // -----------------------------------------------------
            // ティッシュサンプル版（クリムゾン）
            // -----------------------------------------------------
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