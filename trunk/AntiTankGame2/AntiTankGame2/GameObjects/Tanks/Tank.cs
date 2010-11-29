using Microsoft.Xna.Framework;

namespace AntiTankGame2.GameObjects.Tanks
{
    /// <summary>
    /// Simle tank model
    /// </summary>
    public sealed class Tank : BaseTank
    {
        #region construtcors
        public Tank()
        {
            Position = Vector3.Zero;
        }

        public Tank(Vector3 newPosition)
        {
            Position = newPosition;
            Scale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        #endregion

    }
}