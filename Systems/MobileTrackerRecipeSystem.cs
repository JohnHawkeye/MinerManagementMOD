using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Systems
{
    public class MobileTrackerRecipeSystem : ModSystem
    {
        // モバイルトラッカーを所持しているか確認
        public static bool HasMobileTracker(Player player)
        {
            int trackerType = ModContent.ItemType<Items.MobileTracker>();

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];

                if (item != null &&
                    !item.IsAir &&
                    item.type == trackerType &&
                    item.stack > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // モバイルトラッカーによる特殊条件
        public static readonly Condition MobileTrackerCondition =
            new Condition(
                Language.GetOrRegister(
                    "Mods.MinerManagementMOD.Conditions.MobileTracker"
                ),
                () => HasMobileTracker(Main.LocalPlayer)
            );

        public override void AddRecipes()
        {
            // 線路50個
            Recipe.Create(ItemID.MinecartTrack, 50)
                .AddRecipeGroup("IronBar", 1)
                .AddIngredient(ItemID.Wood, 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.Wire, 5)
                .AddRecipeGroup("CopperOrTin", 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.RedPressurePlate, 5)
                .AddRecipeGroup("CopperOrTin", 1)
                .AddRecipeGroup("IronOrLeadBar", 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.PressureTrack)
                .AddIngredient(ItemID.MinecartTrack, 1)
                .AddIngredient(ItemID.RedPressurePlate, 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ModContent.ItemType<HoppingTrackItem>())
                .AddIngredient(ItemID.MinecartTrack, 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.Switch)
                .AddIngredient(ItemID.DirtBlock, 50)
                .AddRecipeGroup("CopperOrTin", 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.Lever)
                .AddIngredient(ItemID.DirtBlock, 50)
                .AddRecipeGroup("CopperOrTin", 1)
                .AddRecipeGroup("IronOrLeadBar", 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.Actuator, 5)
                .AddRecipeGroup("SilverOrTungstenBar", 1)
                .AddRecipeGroup("IronOrLeadBar", 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.Teleporter)
                .AddRecipeGroup("GoldOrPlatinumBar", 1)
                .AddRecipeGroup("IronOrLeadBar", 1)
                .AddCondition(MobileTrackerCondition)
                .Register();

            Recipe.Create(ItemID.MulticolorWrench)
                .AddRecipeGroup("IronOrLeadBar", 5)
                .AddCondition(MobileTrackerCondition)
                .Register();

        }
    }
}