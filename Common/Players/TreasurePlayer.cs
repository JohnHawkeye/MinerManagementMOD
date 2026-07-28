using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Items.Treasure;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Players
{
    public class TreasurePlayer : ModPlayer
    {
        private int previousChest = -1;

        public override void PostUpdate()
        {
            // 新しく宝箱を開いた瞬間だけ
            if (Player.chest != previousChest)
            {
                previousChest = Player.chest;

                if (Player.chest != -1)
                {
                    CheckTreasureChest(Player.chest);
                }
            }
        }

        private void CheckTreasureChest(int chestIndex)
        {
            if(chestIndex < 0 || chestIndex >= Main.chest.Length)
                return;
                
            Chest chest = Main.chest[chestIndex];

            if (chest == null)
                return;

            if(TreasureWorldData.OpenedTreasureChests.Contains(chestIndex))
                return;

            foreach (Item item in chest.item)
            {
                if (item.ModItem is TreasureBase)
                {
                    Vector2 position = new Vector2(
                        chest.x * 16 + 16,
                        chest.y * 16 + 16);

                    TreasureEffects.PlayTreasureOpenEffect(position);

                    TreasureWorldData.OpenedTreasureChests.Add(chestIndex);

                    break;
                }
            }
        }
    }
}