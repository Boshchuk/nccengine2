using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Graphics.Screens.Menu
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class MenuEntry
    {
        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        internal void OnSelectEntry()
        {
            if (Selected != null)
            {
                Selected(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
            {
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            }
            else
            {
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
            }
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            var color = isSelected ? Color.Yellow : Color.White;

            // Pulsate the size of the selected menu entry.
            var time = gameTime.TotalGameTime.TotalSeconds;

            var pulsate = (float)Math.Sin(time * 6) + 1;

            var scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.

            // ReSharper disable PossibleLossOfFraction
            var origin = new Vector2(0, ScreenManager.Font.LineSpacing / 2);
            // ReSharper restore PossibleLossOfFraction

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, Text, new Vector2(position.X + 1, position.Y + 1), Color.Black, 0, origin, scale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, Text, position, color, 0, origin, scale, SpriteEffects.None, 0);

        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public static int GetHeight(MenuScreen screen)
        {
            return ScreenManager.Font.LineSpacing;
        }
    }

}