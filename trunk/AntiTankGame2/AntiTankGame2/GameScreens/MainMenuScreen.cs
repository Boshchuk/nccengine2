using System;
using AntiTankGame2.Localization;
using NccEngine2;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Screens.Menu;

namespace AntiTankGame2.GameScreens
{
    public class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// The main menu screen is the first thing displayed when the game starts up.
        /// </summary>
        public MainMenuScreen()
            : base( Strings.MainMenu)
        {
            var playGameManuEntry = new MenuEntry(Strings.PlayGame);
            var optionsMenuEntry = new MenuEntry(Strings.OptionsMenu);
            var exitMenuEntry = new MenuEntry(Strings.ExitMenu);

            playGameManuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameManuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        static void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(true, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        static void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            //ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            var message = Strings.AreYouWantToExit;

            var messageBox = new MessageBoxScreen(message);
            messageBox.Accepted += ExitMessageBoxAccepted;
            ScreenManager.AddScreen(messageBox);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            EngineManager.Game.Exit();
        }
    }
}