using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Items.Treasure
{
    public abstract class TreasureBase : ModItem
    {
        /// <summary>
        /// トレジャーID
        /// </summary>
        public abstract int TreasureID { get; }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 1;

            Item.value = Item.buyPrice(gold: 1);

            Item.rare = Terraria.ID.ItemRarityID.White;
        }

        public override void SetStaticDefaults()
        {
            TreasureBookSystem.RegisterTreasureItem(
                TreasureID,
                Type);
        }

        public override void UpdateInventory(Player player)
        {
            // 初回入手時のみ登録
            if (TreasureBookSystem.Register(TreasureID))
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.NewText($"図鑑に登録されました！ ({TreasureID}/128)",
                        Microsoft.Xna.Framework.Color.Gold);
                }
            }
        }
    }
}