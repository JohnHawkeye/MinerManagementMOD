using Terraria.GameContent.UI;
using Terraria.ModLoader;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Systems
{
    public class GoldMinerCoinCurrency : CustomCurrencySingleCoin
    {
        public GoldMinerCoinCurrency()
            : base(ModContent.ItemType<GoldMinerCoin>(), 999999L)
        {
            CurrencyTextKey =
                "Mods.MinerManagementMOD.Currency.GoldMinerCoin";
        }
    }
}