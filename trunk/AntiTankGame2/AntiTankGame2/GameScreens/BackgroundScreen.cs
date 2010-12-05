using System;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Textures;

namespace AntiTankGame2.GameScreens
{
    public class BackgroundScreen : GameScreen
    {
        private const string BackGroundTextureName = ContentConstants.BackgroundTexureName;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            // ReSharper disable AccessToStaticMemberViaDerivedType
            EngineManager.Bloom.Visible = false;
            // ReSharper restore AccessToStaticMemberViaDerivedType
        }

        public override void LoadContent()
        {
            base.LoadContent();

            TextureManager.AddTexture(new NccTexture(ContentConstants.BackgroundTexurePath), BackGroundTextureName);
        }
        /*
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        */
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
            // ReSharper disable AccessToStaticMemberViaDerivedType
            var viewport = EngineManager.Device.Viewport;
            // ReSharper restore AccessToStaticMemberViaDerivedType

            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;
            //BUG
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,null,null,null,null);

            if (TextureManager.GetTexture(BackGroundTextureName).ReadyToRender)
            {
                ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(BackGroundTextureName).BaseTexture as Texture2D, fullscreen, new Color(fade, fade, fade));
            }

            ScreenManager.SpriteBatch.End();
        }
    }

}