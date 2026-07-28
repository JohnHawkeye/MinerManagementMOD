using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Commands
{
    public class ResetMinersCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "resetminers";

        public override string Usage => "/resetminers";

        public override string Description =>
            "鉱夫名簿を初期化します。";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            MinerRosterSystem.ResetAll();

            Main.NewText(
                "鉱夫名簿を初期化しました。",
                Color.Yellow);
        }
    }
}