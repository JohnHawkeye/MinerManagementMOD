using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD
{
    public class RecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup cobaltOrPalladium = new RecipeGroup(
                () => "Cobalt or Palladium Bar",
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            );

            RecipeGroup.RegisterGroup(
                "CobaltOrPalladium",
                cobaltOrPalladium
            );

            // デモナイトまたはクリムタン
            RecipeGroup demoniteOrCrimtane = new RecipeGroup(
                () => "Demonite or Crimtane Bar",
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
            );

            RecipeGroup.RegisterGroup(
                "DemoniteOrCrimtane",
                demoniteOrCrimtane
            );
        }
    }
}