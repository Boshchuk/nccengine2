//#define MENUMUSIC

using System;
using AntiTankGame2.GameObjects.Tanks;
using AntiTankGame2.GameObjects.Terrain;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.Audio;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Textures;
using NccEngine2.GameComponents.Scene;

namespace AntiTankGame2.GameScreens
{
    public class BackgroundScreen : GameScreen
    {
        private const string BackGroundTextureName = ContentConstants.BackgroundTexureName;

        private Texture2D skyRocketTexture;
        private readonly Vector2 skyRoketPos = new Vector2(0,0);
        
        Matrix matrix;


        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            
            BaseEngine.Bloom.Visible = false;

            //EngineManager.Game.IsMouseVisible = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            skyRocketTexture = BaseEngine.ContentManager.Load<Texture2D>("Content/Textures/LensFlare/flare2");
            
            TextureManager.AddTexture(new NccTexture(ContentConstants.BackgroundTexurePath), BackGroundTextureName);
            
            var model = new BigTank();
            SceneGraphManager.AddObject(model);

            var plane = new SimplePlane {Scale = new Vector3(5, 5, 5), Position = new Vector3(0, 0, 0)};
            SceneGraphManager.AddObject(plane);
            SceneGraphManager.LoadContent();

#if MENUMUSIC
            BaseEngine.AudioManager.LoadSong("MenuTheme", "MenuTheme");
            AudioManager.PlayMusic("MenuTheme");
#endif

        }

        public override void UnloadContent()
        {
            SceneGraphManager.UnloadContent();
        }
       
        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocusParameter, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocusParameter, false);
        }

        
        public override void Draw(GameTime gameTime)
        {
            //var viewport = BaseEngine.Device.Viewport;
            //var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            //var fade = TransitionAlpha;
            CameraManager.SetAllCamerasProjectionMatrix(BaseEngine.AspectRatio);
            
           
            Matrix.CreateRotationY(MathHelper.ToRadians(270) , out matrix);

            CameraManager.ActiveCamera.View = matrix;
            CameraManager.ActiveCamera.View =  matrix* Matrix.CreateTranslation(100,-100,-600) ;
            
            BaseEngine.Device.Clear(Color.Black);

            //Note drawing perf
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
            //if (TextureManager.GetTexture(BackGroundTextureName).ReadyToRender)
            //{
            //    ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(BackGroundTextureName).BaseTexture as Texture2D, fullscreen, new Color(fade, fade, fade));
            //}
            ScreenManager.SpriteBatch.Draw(skyRocketTexture, skyRoketPos,  Color.White );

            ScreenManager.SpriteBatch.End();
            
            SceneGraphManager.Draw(gameTime);
          
        }
    }

}