using Terraria.GameContent.UI;
using Terraria.ModLoader;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Systems
{
    public class SilverMinerCoinCurrency : CustomCurrencySingleCoin
    {
        public SilverMinerCoinCurrency()
            : base(ModContent.ItemType<SilverMinerCoin>(), 999999L)
        {
            CurrencyTextKey =
                "Mods.MinerManagementMOD.Currency.SilverMinerCoin";
        }
    }
}