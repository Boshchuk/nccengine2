using System;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.Graphics.Screens;

namespace AntiTankGame2.GameScreens
{
    public class LoadingScreen : GameScreen
    {
        bool loadingIsSlow;
        bool otherScreensAreGone;

        readonly GameScreen[] screensToLoad;

        private LoadingScreen(bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(bool loadingIsSlow, params GameScreen[] screensToLoad)
        {
            foreach (var screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            var loadingScreen = new LoadingScreen(loadingIsSlow, screensToLoad);

            ScreenManager.AddScreen(loadingScreen);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocusParameter, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocusParameter, coveredByOtherScreen);

            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }

                EngineManager.Game.ResetElapsedTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) && (ScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            if (loadingIsSlow)
            {
                var message = Strings.Loading;

                // Center the text in the viewport.
                
                var viewport = BaseEngine.Device.Viewport;
          
                var viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = ScreenManager.Font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize)/2;

                var color = new Color(255, 255, 255, TransitionAlpha);

                //Note Drawing
                ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,null,null,null);
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message, textPosition, color);
                ScreenManager.SpriteBatch.End();
            }
        }
    }

}