using Terraria.GameContent.UI;
using Terraria.ModLoader;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Systems
{
    public class PlatinumMinerCoinCurrency : CustomCurrencySingleCoin
    {
        public PlatinumMinerCoinCurrency()
            : base(ModContent.ItemType<PlatinumMinerCoin>(), 999999L)
        {
            CurrencyTextKey =
                "Mods.MinerManagementMOD.Currency.PlatinumMinerCoin";
        }
    }
}