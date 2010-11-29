using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.Graphics.Textures;

namespace NccEngine2.GameComponents.Graphics.Screens
{
    public class ScreenManager : DrawableGameComponent
    {
        private static List<GameScreen> Screens = new List<GameScreen>();
        private static List<GameScreen> ScreensToUpdate = new List<GameScreen>();

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public static GameScreen[] GetScreens()
        {
            return Screens.ToArray();
        }

        /// <summary>
        /// Is the ScreenManagers Initialized, used for test cases and setup of Effects.
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public static SpriteFont Font { get; private set; }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            TraceEnabled = true;
            Enabled = true;
        }

        static ScreenManager()
        {
            Initialized = false;
        }

        protected override void LoadContent()
        {
          //  base.LoadContent();

            SpriteBatch = new SpriteBatch(BaseEngine.Device);

            Font = BaseEngine.ContentManager.Load<SpriteFont>("Content/Fonts/LocalizatedFont");
            TextureManager.AddTexture(new NccTexture("Content/Textures/blank"), "blank");

            foreach (var screen in Screens)
            {
                screen.LoadContent();
            }
          
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (var screen in Screens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Initializes each screen and the screen manager itself.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Initialized = true;
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            
            BaseEngine.Input.Update();
            

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            ScreensToUpdate.Clear();

            foreach (var screen in Screens)
            {
                ScreensToUpdate.Add(screen);
            }
            var otherScreenHasFocus = !Game.IsActive;
            var coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (ScreensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var screen = ScreensToUpdate[ScreensToUpdate.Count - 1];

                ScreensToUpdate.RemoveAt(ScreensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, BaseEngine.Input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                    {
                        coveredByOtherScreen = true;
                    }
                }
            }

            // Print debug trace?
            if (TraceEnabled)
            {
                TraceScreens();
            }
        }

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        private static void TraceScreens()
        {
            Trace.WriteLine(string.Join(", ", Screens.Select(screen => screen.GetType().Name).ToArray()));
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (var screen in Screens)
            {
                if (screen == null) continue;
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                screen.Draw(gameTime);
            }

            foreach (var screen in Screens)
            {
                if (screen != null)
                {
                    if (screen.ScreenState == ScreenState.Hidden)
                        continue;
                    screen.PostUIDraw(gameTime);
                }
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public static void AddScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to load content.
            Screens.Add(screen);
            if (Initialized)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public static void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.

            if (Initialized)
            {
                screen.UnloadContent();
            }

            Screens.Remove(screen);
            ScreensToUpdate.Remove(screen);
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public static void FadeBackBufferToBlack(int alpha)
        {
            var viewport = BaseEngine.Device.Viewport;
           
            //TODO CHEK FOR BUG
            //BUG can make error
            //SpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,SaveStateMode.SaveState);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);//when pause

            SpriteBatch.Draw(TextureManager.GetTexture("blank").BaseTexture as Texture2D, new Rectangle(0, 0, viewport.Width, viewport.Height),new Color(0, 0, 0, (byte)alpha));

            SpriteBatch.End();
        }
    }
}