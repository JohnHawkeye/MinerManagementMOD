using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD
{
    public class RecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup rottenOrVerebrae = new RecipeGroup(
                () => "Rotten or Verebrae",
                ItemID.RottenChunk,
                ItemID.Vertebrae
);

            RecipeGroup.RegisterGroup(
                "RottenOrBertebrae",
                rottenOrVerebrae
            );

            //iron or lead
            RecipeGroup ironOrLead = new RecipeGroup(
                () => "Iron or Lead Bar",
                ItemID.IronBar,
                ItemID.LeadBar
);

            RecipeGroup.RegisterGroup(
                "IronOrLeadBar",
                ironOrLead
            );

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

            //
            RecipeGroup mythrilOrOrichalcumBar = new RecipeGroup(
                () => "Mythril or Orichalcum Bar",
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            );

            RecipeGroup.RegisterGroup(
                "MythrilOrOrichalcumBar",
                mythrilOrOrichalcumBar
            );


            //
            RecipeGroup titanOrAdamanBar = new RecipeGroup(
                () => "Titanium or Adamantite Bar",
                ItemID.TitaniumBar,
                ItemID.AdamantiteBar
            );

            RecipeGroup.RegisterGroup(
                "TitaniumOrAdamantiteBar",
                titanOrAdamanBar
            );
        }
    }
}