using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Commands
{
    public class ResetTreasureBookCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "resettreasure";

        public override string Usage => "/resettreasure";

        public override string Description =>
            "トレジャー図鑑を初期化します。";


        public override void Action(
            CommandCaller caller,
            string input,
            string[] args)
        {
            TreasureBookSystem.ResetAll();

            Main.NewText(
                "トレジャー図鑑を初期化しました。",
                Color.Yellow);
        }
    }
}