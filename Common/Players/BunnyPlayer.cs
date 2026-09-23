using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Common.Players
{
    public class BunnyPlayer : ModPlayer
    {
        // カッパーマイナーコインを一度でも入手したか
        public bool HasObtainedCopperMinerCoin { get; private set; }

        public override bool OnPickup(Item item)
        {
            // カッパーマイナーコインを拾った
            if (item.type == ModContent.ItemType<CopperMinerCoin>())
            {
                HasObtainedCopperMinerCoin = true;
            }

            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            if (HasObtainedCopperMinerCoin)
            {
                tag["HasObtainedCopperMinerCoin"] = true;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            HasObtainedCopperMinerCoin =
                tag.GetBool("HasObtainedCopperMinerCoin");
        }
    }
}