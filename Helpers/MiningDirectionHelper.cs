using Microsoft.Xna.Framework;

namespace MinerManagementMOD.Helpers
{
    public static class MiningDirectionHelper
    {
        public static int GetDirection(Vector2 playerPosition, Vector2 tilePosition)
        {
            if (tilePosition.X > playerPosition.X)
            {
                return 1; //右
            }
            else
            {
                return -1; //左
            }
        }
    }
}