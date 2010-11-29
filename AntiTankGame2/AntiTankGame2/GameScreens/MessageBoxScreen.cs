using System;

using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine.GameComponents;
using NccEngine2;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Textures;

namespace AntiTankGame2.GameScreens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        private const string Texture = ContentConstants.MessageBoxTexureName;
        private string message;

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message)
            : this(message, true)
        { }

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {

            var usageText = string.Format(Strings.UsageText, Environment.NewLine);

            if (includeUsageText)
            {
                this.message = message + usageText;
            }
            else
            {
                this.message = message;
            }
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            TextureManager.AddTexture(new NccTexture(ContentConstants.MessageBoxTexurePath), Texture);
        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, Input input)
        {
            if (input.MenuSelect)
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                {
                    Accepted(this, EventArgs.Empty);
                }
                ExitScreen();
            }
            else if (input.MenuCancel)
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                {
                    Cancelled(this, EventArgs.Empty);
                }
                ExitScreen();
            }
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            // ReSharper disable AccessToStaticMemberViaDerivedType
            var viewport = EngineManager.Device.Viewport;
            // ReSharper restore AccessToStaticMemberViaDerivedType
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = ScreenManager.Font.MeasureString(message);
            var textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            var color = new Color(255, 255, 255, TransitionAlpha);

            //Bug Place HERE
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (TextureManager.GetTexture(Texture).ReadyToRender)
            {
                // Draw the background rectangle.
                ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(Texture).BaseTexture as Texture2D, backgroundRectangle, color);
            }
            // Draw the message box text.
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message, textPosition, color);

            ScreenManager.SpriteBatch.End();
        }
    }
}