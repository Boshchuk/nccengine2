#define DRAWPARTICLE
#define LENSFLARE
#define USEBLOOM

#region Using Statement

using System;
using System.Collections.Generic;
using AntiTankGame2.GameLogic;
using AntiTankGame2.GameObjects;
using AntiTankGame2.GameObjects.Tanks;
using AntiTankGame2.GameObjects.TestLogic;
using AntiTankGame2.GameObjects.Trees;
using AntiTankGame2.Localization;
using AntiTankGame2.ParcileHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NccEngine2;
using NccEngine2.GameComponents.Audio;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Graphics.FX.Bloom;
using NccEngine2.GameComponents.Graphics.FX.Particles;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Textures;
using NccEngine2.GameComponents.Models.Terrain;
using NccEngine2.GameComponents.NccInput;
using NccEngine2.GameComponents.Scene;
using NccEngine2.GameComponents.Scene.SceneObject;
using NccEngine2.GameDebugTools;

#endregion

namespace AntiTankGame2.GameScreens
{
    /// <summary>
    /// Class where gameplay logic must be consctructed
    /// </summary>
    public class GameplayScreen : GameScreen 
    {
        #region fields

        private static double delta;

#pragma warning disable 649
        private HeightMapInfo heightMapInfo;
#pragma warning restore 649

        private TankHeight targetTank;
        private HeightTree tree;
        private EndPoint endPoint;
        private Vector3 lastendPointPos;
        private EndPoint roket;

        private bool col2;

        private bool wasSound;

        private const int CrossBarTextureHeight = 586;
        private const int CrossBarTextureWidth = 586;
        private const int CrossBarTextureHeightHalf = 293;
        private const int CrossBarTextureWidthHalf = 293;

        //precalculatet optimization
        static readonly Vector2 Pos = new Vector2(BaseEngine.Width / 2 - CrossBarTextureWidthHalf, BaseEngine.Height / 2 - CrossBarTextureHeightHalf);
        static readonly Rectangle Dest1 = new Rectangle(0, 0, BaseEngine.Width, BaseEngine.Height / 2 - CrossBarTextureHeightHalf);
        static readonly Rectangle Dest2 = new Rectangle(0, BaseEngine.Height / 2 - CrossBarTextureHeightHalf, BaseEngine.Width / 2 - CrossBarTextureWidthHalf, BaseEngine.Height);
        static readonly Rectangle Dest3 = new Rectangle(BaseEngine.Height / 2 - CrossBarTextureWidthHalf, BaseEngine.Height / 2 - CrossBarTextureHeightHalf + CrossBarTextureHeight, BaseEngine.Width, BaseEngine.Height);
        static readonly Rectangle Dest4 = new Rectangle(BaseEngine.Width / 2 - CrossBarTextureWidthHalf + CrossBarTextureWidth, BaseEngine.Height / 2 - CrossBarTextureHeightHalf, BaseEngine.Width, BaseEngine.Height);
        
        Vector2 textStartPosition = new Vector2(10, 20);
        private readonly Color fontColor;

        private bool drawCross = true;
        private bool lens;

        private int bloomSettingsIndex;

        private Texture2D crossBarTexture;
        private Texture2D blackTexture;

        private Texture2D readyTexture;

        /// <summary>
        /// Is roket in move
        /// </summary>
        private bool zapusk;

        #endregion
#if DRAWPARTICLE
        #region Particles

        private Vector3 tempSmokePos = Vector3.Zero;

        readonly ParticleSystem explosionParticles;
        readonly ParticleSystem explosionSmokeParticles;
        readonly ParticleSystem projectileTrailParticles;
        readonly ParticleSystem smokePlumeParticles;
        readonly ParticleSystem fireParticles;

        ParticleState currentState = ParticleState.SmokePlume;

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
#endif
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            fontColor = new Color(255, 255, 255, TransitionAlpha);
           // SetupCamera();

            //Stop music if it played before
            AudioManager.StopMusic();

#if (DRAWPARTICLE)
            #region init particles
            //// Construct our particle system components.
            explosionParticles = new ParticleSystem(EngineManager.Game, BaseEngine.ContentManager, ContentConstants.ExplosionSettings);
            explosionSmokeParticles = new ParticleSystem(EngineManager.Game, BaseEngine.ContentManager, ContentConstants.ExplosionSmokeSettings);
            projectileTrailParticles = new ParticleSystem(EngineManager.Game, BaseEngine.ContentManager, ContentConstants.ProjectileTrailSettings);
            smokePlumeParticles = new ParticleSystem(EngineManager.Game, BaseEngine.ContentManager, ContentConstants.SmokePlumeSettings);
            fireParticles = new ParticleSystem(EngineManager.Game, BaseEngine.ContentManager, ContentConstants.FireSettings);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system components.
            EngineManager.Game.Components.Add(explosionParticles);
            EngineManager.Game.Components.Add(explosionSmokeParticles);
            EngineManager.Game.Components.Add(projectileTrailParticles);
            EngineManager.Game.Components.Add(smokePlumeParticles);
            EngineManager.Game.Components.Add(fireParticles);
            #endregion
#endif
#if HIDEF
            BaseEngine.LensFlareComponent.MakeDraw = true;
#endif
        }

        private static void SetupCamera()
        {
            #region camera setup

            CameraManager.SetActiveCamera(CameraManager.CameraNumber.Default);
            CameraManager.ActiveCamera.Position = new Vector3(-1624.0f, -608.0f, -1654.0f);

            CameraManager.SetCamerasFrustum(0.1f, 41000.0f, BaseEngine.AspectRatio);
            CameraManager.ActiveCamera.RotateY((30));

            #endregion
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
           //SceneGraphManager.DrawDebugger = true;
            SetupCamera();

            TextureManager.AddTexture(new NccTexture(ContentConstants.CrossbarTexturePath), ContentConstants.CrossbarName); // crossbar init
            TextureManager.AddTexture(new NccTexture(ContentConstants.BlackRactangleTexturePath), ContentConstants.BlackRactangeleName); // black rectangle int
            TextureManager.AddTexture(new NccTexture("Content/Textures/Ready"), "Ready");

            readyTexture = TextureManager.GetTexture("Ready").BaseTexture as Texture2D;
            crossBarTexture = TextureManager.GetTexture(ContentConstants.CrossbarName).BaseTexture as Texture2D;
            blackTexture = TextureManager.GetTexture(ContentConstants.BlackRactangeleName).BaseTexture as Texture2D;

            var heightMapTerrain = new HeightMapTerrain();
            SceneGraphManager.AddObject(heightMapTerrain);

            targetTank = new TankHeight(heightMapInfo, new Vector3(800, 730, 95))
                             {FacingDirection = MathHelper.ToRadians(-110)};
            SceneGraphManager.AddObject(targetTank);

            endPoint = new EndPoint { Position = new Vector3(100.0f, 0.0f, 0.0f), Scale = new Vector3(20f, 20f, 20f) };
            SceneGraphManager.AddObject(endPoint);
            
            //roket = new EndPoint { Position = new Vector3(-1624, -590, -1654), Scale = new Vector3(20f, 20f, 20f) };
            roket = new EndPoint { Position = new Vector3(800, 740, 105), Scale = new Vector3(20f, 20f, 20f) };
            SceneGraphManager.AddObject(roket);

            var skySphere = new NccSkySphere {Position = Vector3.Zero};
            SceneGraphManager.AddObject(skySphere);


            tree = new /*BaseTree*/ HeightTree(heightMapInfo, new Vector3(800, 740, 105));
            ;
                            //{FacingDirection = MathHelper.ToRadians(-110)};
            SceneGraphManager.AddObject(tree);

            BaseEngine.AudioManager.LoadSound("Start", "Start");

            BaseEngine.Bloom.Visible = false;
            SceneGraphManager.LoadContent();

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            EngineManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            SceneGraphManager.UnloadContent();
            GC.Collect();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocusParameter, bool coveredByOtherScreen)
        {
           
#if(DRAWPARTICLE)
            if (SceneGraphManager.Paused != true)
            {
                if (col2)
                {
                    UpdateRarticles(gameTime);
                }
            }
#endif
            base.Update(gameTime, otherScreenHasFocusParameter, coveredByOtherScreen);
            delta = gameTime.ElapsedGameTime.TotalSeconds;
         
            HandleCube();

           // Collsison(roket, endPoint);

            col2 = Int(roket, targetTank);

            if (!zapusk) return;
            if (!col2)
            {
                roket.Position = RocketHelper.RocketPos(gameTime, roket.Position, endPoint.Position, lastendPointPos);
            }
            else
            {
                if (!wasSound)
                {
                    //BaseEngine.AudioManager.Play3DSound("Content/Sounds/Start", false, roket);
                    AudioManager.PlaySound("Start");
                    tempSmokePos = targetTank.Position;

                    SceneGraphManager.ClearSelectedObject(roket);

                    wasSound = true;
                }
            }
        }

        //private static bool Collsison(INccSceneObject rocket, INccSceneObject target)
        //{
        //    var rock = new BoundingSphere(rocket.Position, 10);
        //    var targ = new BoundingSphere(target.Position, 10);

        //    return rock.Intersects(targ);
        //}

        private static bool Int(INccSceneObject rocket, INccSceneObject target)
        {
            var rock = new BoundingSphere(rocket.Position, 10);
            var targ = new BoundingSphere(target.Position, 100);

            return rock.Intersects(targ);
        }


        public override void HandleInput(GameTime gameTime, Input input)
        {
            if (BaseEngine.DebugSystem.DebugCommandUI.UIState == DebugCommandUI.State.Opened) return;
            //delta = gameTime.ElapsedGameTime.TotalSeconds;
            if (input.ExitGame)
            {
                ScreenManager.AddScreen(new PauseMenuScreen());
                SceneGraphManager.Paused = true;
            }
            else
            {

                if (input.CurrentKeyboardState.IsKeyDown(Keys.E))
                {
                    zapusk = true;
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.X))
                {
                    //TODO shake here
                    //  camera.Shake(25f, 2f);
                    CameraManager.ActiveCamera.Shake(25f,2f);
                }


                #region BloomHandle

                input.LastKeyboardState = input.CurrentKeyboardState;
                input.LastGamePadState = input.CurrentGamePadState;

                input.CurrentKeyboardState = Keyboard.GetState();
                input.CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
#if USEBLOOM


                // Switch to the next bloom settings preset?
                if (input.CurrentKeyboardState.IsKeyDown(Keys.F5))
                {

                    bloomSettingsIndex = (bloomSettingsIndex + 1)%BloomSettings.PresetSettings.Length;
                    BloomComponent.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                    BaseEngine.Bloom.Visible = true;

                }

                // Toggle bloom on or off?
                if (input.CurrentKeyboardState.IsKeyDown(Keys.F6))
                {
                    BaseEngine.Bloom.Visible = !BaseEngine.Bloom.Visible;
                }

                // Cycle through the intermediate buffer debug display modes?
                if (input.CurrentKeyboardState.IsKeyDown(Keys.F7))
                {
                    BaseEngine.Bloom.Visible = true;
                    BaseEngine.Bloom.ShowBuffer++;

                    if (BaseEngine.Bloom.ShowBuffer > BloomComponent.IntermediateBuffer.FinalResult)
                    {
                        BaseEngine.Bloom.ShowBuffer = 0;
                    }
                }

#endif

                #endregion

                #region particcles
#if DRAWPARTICLE

                #region Partilces Handle

                // Check for changing the active particle effect.
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Space))
                {
                    currentState++;

                    if (currentState > ParticleState.RingOfFire)
                    {
                        currentState = 0;
                    }
                }

                #endregion

                const int move = 15;

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Up))
                {
                    tempSmokePos = new Vector3(tempSmokePos.X+move,tempSmokePos.Y,tempSmokePos.Z);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Down))
                {
                    tempSmokePos = new Vector3(tempSmokePos.X - move, tempSmokePos.Y, tempSmokePos.Z);
                }
                //------------------------------------------------------
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
                {
                    tempSmokePos = new Vector3(tempSmokePos.X, tempSmokePos.Y, tempSmokePos.Z - move);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
                {
                    tempSmokePos = new Vector3(tempSmokePos.X , tempSmokePos.Y, tempSmokePos.Z + move);
                }
                //---------------------------------------
                if (input.CurrentKeyboardState.IsKeyDown(Keys.OemPlus))
                {
                    tempSmokePos = new Vector3(tempSmokePos.X, tempSmokePos.Y+move, tempSmokePos.Z );
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.OemMinus))
                {
                    tempSmokePos = new Vector3(tempSmokePos.X, tempSmokePos.Y-move, tempSmokePos.Z);
                }

#endif

                #endregion
                #region Blend

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Y))
                {
                    BaseEngine.SetCurrentAlphaMode(BaseEngine.AlphaMode.DisableAlpha);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.U))
                {
                    BaseEngine.SetCurrentAlphaMode(BaseEngine.AlphaMode.Default);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.I))
                {
                    BaseEngine.SetCurrentAlphaMode(BaseEngine.AlphaMode.SourceAlphaOne);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.O))
                {
                    BaseEngine.SetCurrentAlphaMode(BaseEngine.AlphaMode.OneOne);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.R))
                {
                    BaseEngine.RestorSamplerState();
                }


                #endregion

                #region camera handle

                const float streifSpeed = 1000;

                if (input.PauseGame)
                {
                    SceneGraphManager.Paused = true;
                }
                else
                {
                    SceneGraphManager.HandleInput(gameTime, input);
                }

                const float cubeMovment = 10;

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
                {
                    endPoint.Position = new Vector3(endPoint.Position.X - cubeMovment, endPoint.Position.Y,
                                                    endPoint.Position.Z);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
                {
                    endPoint.Position = new Vector3(endPoint.Position.X + cubeMovment, endPoint.Position.Y,
                                                    endPoint.Position.Z);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Down))
                {
                    endPoint.Position = new Vector3(endPoint.Position.X, endPoint.Position.Y - cubeMovment,
                                                    endPoint.Position.Z);
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Up))
                {
                    endPoint.Position = new Vector3(endPoint.Position.X, endPoint.Position.Y + cubeMovment,
                                                    endPoint.Position.Z);
                }


                if (input.CurrentKeyboardState.IsKeyDown(Keys.F1))
                {
                    CameraManager.SetActiveCamera(CameraManager.CameraNumber.Default);
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.F2))
                {
                    CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThreeNumber);
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Q))
                {
                    CameraManager.ActiveCamera.Translate(new Vector3(0, streifSpeed*(float) delta, 0.0f));
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Z))
                {
                    CameraManager.ActiveCamera.Translate(new Vector3(0, -streifSpeed*(float) delta, 0.0f));
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.W))
                {
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 0, streifSpeed*(float) delta));
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
                {
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 0, -streifSpeed*(float) delta));
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.D))
                {
                    CameraManager.ActiveCamera.Translate(new Vector3(-streifSpeed*(float) delta, 0, 0));
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
                {
                    CameraManager.ActiveCamera.Translate(new Vector3(streifSpeed*(float) delta, 0, 0));
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.F2))
                {
                    drawCross = !drawCross;
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.F3))
                {
                    lens = !lens;
                }

                if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    CameraManager.ActiveCamera.RotateX(input.MouseMoved.Y);
                    CameraManager.ActiveCamera.RotateY(input.MouseMoved.X);
                }

                #endregion

                #region Joy stick suck

#if !XBOX
                const bool dualshock = true;
                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                if (dualshock)
                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
                {
                    CameraManager.ActiveCamera.RotateX(input.CurrentSimpleGamePadState.ThumbSticks.Right.Y);

                    CameraManager.ActiveCamera.RotateY(-input.CurrentSimpleGamePadState.ThumbSticks.Right.X);
                }


                const float pistonCompensatorProcY = 0.05f;
                const float pistonCompensatorProcX = 0.05f;



                var paramY = input.CurrentSimpleGamePadState.ThumbSticks.Left.Y;
                var paramX = input.CurrentSimpleGamePadState.ThumbSticks.Left.X;

                if ((paramY > pistonCompensatorProcY) || (paramY < -pistonCompensatorProcY))
                {
                    CameraManager.ActiveCamera.RotateX(input.CurrentSimpleGamePadState.ThumbSticks.Left.Y);
                }
                if ((paramX > pistonCompensatorProcX) || (paramX < -pistonCompensatorProcX))
                {
                    CameraManager.ActiveCamera.RotateY(-input.CurrentSimpleGamePadState.ThumbSticks.Left.X);
                }

#endif

                #endregion
            }
        }

        public override void Draw(GameTime gameTime)
        {
            BaseEngine.RestorSamplerState();
            BaseEngine.Bloom.BeginDraw();

            // TODO make optimization
           // BaseEngine.Device.Clear(BaseEngine.BackgroundColor);
           // SceneGraphManager.DrawCulling(gameTime);
         
            BaseEngine.Device.Clear(Color.Black);
#if (DRAWPARTICLE)
            {
                #region draw particle
                
                    //var view = CameraManager.ActiveCamera.View;
                    //var projection = CameraManager.ActiveCamera.Projection;

                    // Pass camera matrices through to the particle system components.
                    explosionParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
                    explosionSmokeParticles.SetCamera(CameraManager.ActiveCamera.View,
                                                      CameraManager.ActiveCamera.Projection);
                    projectileTrailParticles.SetCamera(CameraManager.ActiveCamera.View,
                                                       CameraManager.ActiveCamera.Projection);
                    smokePlumeParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
                    fireParticles.SetCamera(CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
                

                #endregion
            }
#endif
            SceneGraphManager.Draw(gameTime);
            
            base.Draw(gameTime);

#if (LENSFLARE)
#if HIDEF
            BaseEngine.LensFlareComponent.MakeDraw = !SceneGraphManager.Paused;
            
            BaseEngine.LensFlareComponent.Projection = CameraManager.ActiveCamera.Projection;
            BaseEngine.LensFlareComponent.View = CameraManager.ActiveCamera.View;
#endif 
#endif
        }

        public override void PostUIDraw(GameTime gameTime)
        {
            base.PostUIDraw(gameTime);

            if (SceneGraphManager.Paused != true)
            {
               
                if (drawCross)
                {
                    DrawHUDCrossBar();
                }
            }
            
            //var cameraMessage = string.Format("cam pos x{0} y{1} z{2}", CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Y, CameraManager.ActiveCamera.Position.Z);
            //var rocketPos = string.Format("rocket pos x{0} y{1} z{2} radius{3}", roket.Position.X, roket.Position.Y, roket.Position.Z,roket.ModelRadius);

            //var col = string.Format("collision was: {0} zapusk {1}", col2,zapusk);
            //var cameraAngle = string.Format("Angle hor {0} vert {1}", MathHelper.ToDegrees(CameraManager.ActiveCamera.Yaw), MathHelper.ToDegrees(CameraManager.ActiveCamera.Pitch));
            //var bloomInfo = string.Format("F5 = settings ({0}{1}F6 = toggle bloom ({2}){1}F7 = show buffer ({3})", EngineManager.Bloom.Settings.Name, Environment.NewLine, (EngineManager.Bloom.Visible ? "on" : "off"), EngineManager.Bloom.ShowBuffer);
            //var particleMessage = string.Format("Current effect: {0}!!!{1}Hit space bar to switch.",currentState,Environment.NewLine);
            var cameraMessage = string.Format("cameraPos x{0} y{1} z{2}",
                CameraManager.ActiveCamera.Position.X,
                CameraManager.ActiveCamera.Position.Y,
                CameraManager.ActiveCamera.Position.Z);

            //var particleMessage = string.Format("Particle pos : {0}",smokePlumeParticles.);
            var mess = string.Format("x{0} y{1} z{2}",tree.Position.X,tree.Position.Y,tree.Position.Z);
            //var mess = string.Format(GC.CollectionCount(0) + " " + GC.CollectionCount(1));

            #region SpriteBatch Drawing
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend/*AlphaBlend*//*,SaveStateMode.SaveState*/);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, cameraMessage, new Vector2(textStartPosition.X, textStartPosition.Y + 30), fontColor);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, mess, new Vector2(textStartPosition.X, textStartPosition.Y + 60), fontColor);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, col, new Vector2(textStartPosition.X, textStartPosition.Y + 120), fontColor);
            //// //ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, particleMessage, new Vector2(textPosition.X, textPosition.Y + 120), color);
            //// //ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, cameraAngle, new Vector2(textPosition.X, textPosition.Y + 90), color);
            //// //ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, bloomInfo, new Vector2(textPosition.X+550, textPosition.Y ), color);
            ScreenManager.SpriteBatch.End();
            #endregion

        }

        private void DrawHUDCrossBar()
        {
            ScreenManager.SpriteBatch.Begin(/*SpriteSortMode.Texture, BlendState.NonPremultiplied, null, null, null*/);

            ScreenManager.SpriteBatch.Draw(blackTexture, Dest1, Color.White);
            ScreenManager.SpriteBatch.Draw(blackTexture, Dest2, Color.White);
            ScreenManager.SpriteBatch.Draw(blackTexture, Dest3, Color.White);
            ScreenManager.SpriteBatch.Draw(blackTexture, Dest4, Color.White);
            ScreenManager.SpriteBatch.Draw(crossBarTexture, Pos, Color.White);
            ScreenManager.SpriteBatch.Draw(readyTexture, new Vector2(BaseEngine.Width / 2 - 9, BaseEngine.Height - 19), Color.White);
            ScreenManager.SpriteBatch.End();
        }

        #region target lock logic


        private void HandleCube()
        {
            lastendPointPos = endPoint.Position;
            endPoint.Position = AbstractTargetCoordinates();
        }

        /// <summary>
        /// Возвращает координаты, той точки, до которой может долететь ракета...
        /// </summary>
        /// <returns></returns>
        private static Vector3 AbstractTargetCoordinates()
        {
            var result = Vector3.Zero;

            // Vector3.
            const int radius = 8000;

            var heihgt = -(float)Math.Sin(CameraManager.ActiveCamera.Pitch) * radius;

            //****** VERTICAL PLANE ***********************************
            result.Y = CameraManager.ActiveCamera.Position.Y + heihgt;
            var projectionLine = radius * (float)Math.Cos(CameraManager.ActiveCamera.Pitch);
            //**********************************************************
            //******  HORIZONTAL PLANE ***********************************************
            result.X = CameraManager.ActiveCamera.Position.X + (float)Math.Sin((((CameraManager.ActiveCamera.Yaw)))) * projectionLine;

            result.Z = CameraManager.ActiveCamera.Position.Z + (float)Math.Cos((((CameraManager.ActiveCamera.Yaw)))) * projectionLine;
            //**************************************************************************

            return result;
        }
        #endregion
    }
}
