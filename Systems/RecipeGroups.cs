using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD
{
    public class RecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            //
            RecipeGroup rottenOrVerebrae = new RecipeGroup(
                () => "Rotten or Verebrae",
                ItemID.RottenChunk,
                ItemID.Vertebrae
            );
            RecipeGroup.RegisterGroup(
                "RottenOrBertebrae",
                rottenOrVerebrae
            );

            //copper or tin
            RecipeGroup copperOrTin = new RecipeGroup(
                () => "Copper or Tin Bar",
                ItemID.CopperBar,
                ItemID.TinBar
            );
            RecipeGroup.RegisterGroup(
                "CopperOrTin",
                copperOrTin
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

            //silver or tungsten
            RecipeGroup silverOrTungsten = new RecipeGroup(
                () => "Silver or Tungsten Bar",
                ItemID.SilverBar,
                ItemID.TungstenBar
            );
            RecipeGroup.RegisterGroup(
                "SilverOrTungstenBar",
                silverOrTungsten
            );

            //gold or platinum
            RecipeGroup goldOrPlatinum = new RecipeGroup(
                () => "Gold or Platinum Bar",
                ItemID.GoldBar,
                ItemID.PlatinumBar
            );
            RecipeGroup.RegisterGroup(
                "GoldOrPlatinumBar",
                goldOrPlatinum
            );


            //cobalt palladium
            RecipeGroup cobaltOrPalladium = new RecipeGroup(
                () => "Cobalt or Palladium Bar",
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            );
            RecipeGroup.RegisterGroup(
                "CobaltOrPalladium",
                cobaltOrPalladium
            );

            //Mithril Orichalcum
            RecipeGroup mythrilOrOrichalcumBar = new RecipeGroup(
                () => "Mythril or Orichalcum Bar",
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            );
            RecipeGroup.RegisterGroup(
                "MythrilOrOrichalcumBar",
                mythrilOrOrichalcumBar
            );

            //Titanium Adaman
            RecipeGroup titanOrAdamanBar = new RecipeGroup(
                () => "Titanium or Adamantite Bar",
                ItemID.TitaniumBar,
                ItemID.AdamantiteBar
            );
            RecipeGroup.RegisterGroup(
                "TitaniumOrAdamantiteBar",
                titanOrAdamanBar
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