using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace MinerManagementMOD.Systems
{
    public class MinerCoinCurrencySystem : ModSystem
    {
        public static int CopperCurrencyID { get; internal set; }
        public static int SilverCurrencyID { get; internal set; }
        public static int GoldCurrencyID { get; internal set; }
        public static int PlatinumCurrencyID { get; internal set; }

        public override void Load()
        {
            CopperCurrencyID = CustomCurrencyManager.RegisterCurrency(
                new CopperMinerCoinCurrency()
                );
            SilverCurrencyID = CustomCurrencyManager.RegisterCurrency(
                new SilverMinerCoinCurrency()
                );
            GoldCurrencyID = CustomCurrencyManager.RegisterCurrency(
                new GoldMinerCoinCurrency()
                );
            PlatinumCurrencyID = CustomCurrencyManager.RegisterCurrency(
                new PlatinumMinerCoinCurrency()
                );

        }

        public override void Unload()
        {
            CopperCurrencyID = 0;
            SilverCurrencyID = 0;
            GoldCurrencyID = 0;
            PlatinumCurrencyID = 0;
        }
    }
}