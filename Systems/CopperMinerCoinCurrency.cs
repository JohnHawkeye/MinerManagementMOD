using Terraria.GameContent.UI;
using Terraria.ModLoader;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Systems
{
    public class CopperMinerCoinCurrency : CustomCurrencySingleCoin
    {
        public CopperMinerCoinCurrency()
            : base(ModContent.ItemType<CopperMinerCoin>(), 999999L)
        {
            CurrencyTextKey =
                "Mods.MinerManagementMOD.Currency.CopperMinerCoin";
        }
    }
}