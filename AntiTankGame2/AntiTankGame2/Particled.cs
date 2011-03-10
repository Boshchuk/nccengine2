using System;
using System.Collections.Generic;
using AntiTankGame2.Localization;
using AntiTankGame2.ParcileHelpers;
using Microsoft.Xna.Framework;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Graphics.FX.Particles;

namespace AntiTankGame2
{
// ReSharper disable UnusedMember.Global
    public class Particled : EngineManager

    {

        #region Particles

// ReSharper disable FieldCanBeMadeReadOnly.Local
        private Vector3 tempSmokePos = Vector3.Zero;
// ReSharper restore FieldCanBeMadeReadOnly.Local

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleSystem projectileTrailParticles;
        ParticleSystem smokePlumeParticles;
        ParticleSystem fireParticles;

        ParticleState currentState = ParticleState.Explosions;

        // The explosions effect works by firing projectiles up into the
        // air, so we need to keep track of all the active projectiles.
        readonly List<Projectile> projectiles = new List<Projectile>();

        TimeSpan timeToNextProjectile = TimeSpan.Zero;

        private void UpdateRarticles(GameTime gameTime)
        {
            switch (currentState)
            {
                case ParticleState.Explosions:
                    {
                        UpdateExplosions(gameTime);
                        break;
                    }
                case ParticleState.SmokePlume:
                    {
                        UpdateSmokePlume();
                        break;
                    }

                case ParticleState.RingOfFire:
                    {
                        UpdateFire();
                        break;
                    }
            }

            UpdateProjectiles(gameTime);
        }

        /// <summary>
        /// Helper for updating the list of active projectiles.
        /// </summary>
        void UpdateProjectiles(GameTime gameTime)
        {
            var i = 0;

            while (i < projectiles.Count)
            {
                if (!projectiles[i].Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    projectiles.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }

        /// <summary>
        /// Helper for updating the explosions effect.
        /// </summary>
        void UpdateExplosions(GameTime gameTime)
        {
            timeToNextProjectile -= gameTime.ElapsedGameTime;

            if (timeToNextProjectile > TimeSpan.Zero) return;
            // Create a new projectile once per second. The real work of moving
            // and creating particles is handled inside the Projectile class.
            projectiles.Add(new Projectile(explosionParticles, explosionSmokeParticles, projectileTrailParticles));

            timeToNextProjectile += TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Helper for updating the smoke plume effect.
        /// </summary>
        void UpdateSmokePlume()
        {
            // This is trivial: we just create one new smoke particle per frame.
            smokePlumeParticles.AddParticle(/*Vector3.Zero*/ tempSmokePos, Vector3.Zero);
        }

        /// <summary>
        /// Helper for updating the fire effect.
        /// </summary>
        void UpdateFire()
        {
            const int fireParticlesPerFrame = 20;

            // Create a number of fire particles, randomly positioned around a circle.
            for (var i = 0; i < fireParticlesPerFrame; i++)
            {
                fireParticles.AddParticle(ParticleMath.RandomPointOnCircle(), Vector3.Zero);
            }

            // Create one smoke particle per frmae, too.
            smokePlumeParticles.AddParticle(ParticleMath.RandomPointOnCircle(), Vector3.Zero);
        }

        #endregion

        public Particled(string unitTestName) : base(unitTestName)
        {
        }

        protected override void Initialize()
        {
            #region init particles

            //// Construct our particle system components.
            explosionParticles = new ParticleSystem(Game, ContentManager, ContentConstants.ExplosionSettings);
            explosionSmokeParticles = new ParticleSystem(Game, ContentManager, ContentConstants.ExplosionSmokeSettings);
            projectileTrailParticles = new ParticleSystem(Game, ContentManager, ContentConstants.ProjectileTrailSettings);
            smokePlumeParticles = new ParticleSystem(Game, ContentManager, ContentConstants.SmokePlumeSettings);
            fireParticles = new ParticleSystem(Game, ContentManager, ContentConstants.FireSettings);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system components.
            Game.Components.Add(explosionParticles);
            Game.Components.Add(explosionSmokeParticles);
            Game.Components.Add(projectileTrailParticles);
            Game.Components.Add(smokePlumeParticles);
            Game.Components.Add(fireParticles);
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            UpdateRarticles(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            explosionParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
            explosionSmokeParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
            projectileTrailParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
            smokePlumeParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
            fireParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
        }
    }
// ReSharper restore UnusedMember.Global
}