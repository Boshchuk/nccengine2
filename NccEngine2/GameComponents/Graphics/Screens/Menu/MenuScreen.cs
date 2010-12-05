using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.NccInput;

namespace NccEngine2.GameComponents.Graphics.Screens.Menu
{
    public abstract class MenuScreen : GameScreen
    {
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        /// <summary>
        /// Gets the list of menu entry strings, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        int selectedEntry;
        string menuTitle;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, Input input)
        {
            // Move to the previous menu entry?
            if (input.MenuUp)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                {
                    selectedEntry = menuEntries.Count - 1;
                }
            }

            // Move to the next menu entry?
            if (input.MenuDown)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu?
            if (input.MenuSelect)
            {
                OnSelectEntry(selectedEntry);
            }
            else if (input.MenuCancel)
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Notifies derived classes that a menu entry has been chosen.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }

        /// <summary>
        /// Notifies derived classes that the menu has been cancelled.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocusParameter,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocusParameter, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (var i = 0; i < menuEntries.Count; i++)
            {
                var isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            var position = new Vector2(100, 150);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
            {
                position.X -= transitionOffset * 256;
            }
            else
            {
                position.X += transitionOffset*512;
            }

            // Draw the menu title.
            var titlePosition = new Vector2(426, 80);
            var titleOrigin = ScreenManager.Font.MeasureString(menuTitle) / 2;
            var titleColor = new Color(192, 192, 192, TransitionAlpha);
            var titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;
            //BUG HERE BUG PLACE
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,null,null,null /*SaveStateMode.SaveState*/);

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += MenuEntry.GetHeight(this);
            }

            

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, menuTitle, titlePosition, titleColor, 0,titleOrigin, titleScale, SpriteEffects.None, 0);

            ScreenManager.SpriteBatch.End();
        }
    }
}