using System;
using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.Graphics.FX.Particles
{
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    public class Projectile
    {
        #region Constants

        const float TrailParticlesPerSecond = 200;
        const int NumExplosionParticles = 30;
        const int NumExplosionSmokeParticles = 50;
        const float ProjectileLifespan = 1.5f;
        const float SidewaysVelocityRange = 60;
        const float VerticalVelocityRange = 40;
        const float Gravity = 15;

        #endregion

        #region Fields

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleEmitter trailEmitter;

        Vector3 position;
        Vector3 velocity;
        float age;

        static Random random = new Random();

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(ParticleSystem explosionParticles,
                          ParticleSystem explosionSmokeParticles,
                          ParticleSystem projectileTrailParticles)
        {
            this.explosionParticles = explosionParticles;
            this.explosionSmokeParticles = explosionSmokeParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            position = Vector3.Zero;

            velocity.X = (float)(random.NextDouble() - 0.5) * SidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() + 0.5) * VerticalVelocityRange;
            velocity.Z = (float)(random.NextDouble() - 0.5) * SidewaysVelocityRange;

            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(projectileTrailParticles,
                                               TrailParticlesPerSecond, position);
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Simple projectile physics.
            position += velocity * elapsedTime;
            velocity.Y -= elapsedTime * Gravity;
            age += elapsedTime;

            // Update the particle emitter, which will create our particle trail.
            trailEmitter.Update(gameTime, position);

            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.
            if (age > ProjectileLifespan)
            {
                for (int i = 0; i < NumExplosionParticles; i++)
                    explosionParticles.AddParticle(position, velocity);

                for (int i = 0; i < NumExplosionSmokeParticles; i++)
                    explosionSmokeParticles.AddParticle(position, velocity);

                return false;
            }

            return true;
        }
    }
}
