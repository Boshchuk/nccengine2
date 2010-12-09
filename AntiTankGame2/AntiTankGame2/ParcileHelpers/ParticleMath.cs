using System;
using Microsoft.Xna.Framework;

namespace AntiTankGame2.ParcileHelpers
{
    /// <summary>
    /// Used to support simple particle system calculations
    /// </summary>
    public static class ParticleMath
    {
        // Random number generator for the fire effect.
        static readonly Random Random = new Random();

        /// <summary>
        /// Helper used by the UpdateFire method. Chooses a random location
        /// around a circle, at which a fire particle will be created.
        /// </summary>
        public static Vector2 RandomPointOnCircle2D()
        {
            const float radius = 30;
            const float height = 40;

            var angle = Random.NextDouble() * Math.PI * 2;

            var x = (float)Math.Cos(angle);
            var y = (float)Math.Sin(angle);

            return new Vector2(x * radius, y * radius + height);
        }

        /// <summary>
        /// Helper used by the UpdateFire method. Chooses a random location
        /// around a circle, at which a fire particle will be created.
        /// </summary>
        public static Vector3 RandomPointOnCircle()
        {
            const float radius = 30;
            const float height = 40;

            var angle = Random.NextDouble() * Math.PI * 2;

            var x = (float)Math.Cos(angle);
            var y = (float)Math.Sin(angle);

            return new Vector3(x * radius, y * radius + height, 0);
        }
    }
}