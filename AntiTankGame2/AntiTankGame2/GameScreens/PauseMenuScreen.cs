using System;

using AntiTankGame2.Localization;
using NccEngine2;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Screens.Menu;

namespace AntiTankGame2.GameScreens
{
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Конструктор
        /// </summary>
        public PauseMenuScreen()
            : base(Strings.PauseMenu)
        {

            IsPopup = true;

            //создание меню
            var resumeGameMenuEntry = new MenuEntry(Strings.ResumeGame);
            var backToMainMenuEntry = new MenuEntry(Strings.AbortThisGame);
            var quitGameMenuEntry = new MenuEntry(Strings.QuitGame);

            // привязка событий к обработчикам

            resumeGameMenuEntry.Selected += OnCancel;
            backToMainMenuEntry.Selected += AbortThisGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // создание веток к меню

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(backToMainMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

        }
        #endregion

        #region HandleInput

        static void AbortThisGameMenuEntrySelected(object sender, EventArgs e)
        {

            //TODO коректрно почитстиь ресурсы, так как начиная игру заново тормозит несустветно
            var message = Strings.AreYouWantToAbort;
            var confirmQuitMBox = new MessageBoxScreen(message);

            confirmQuitMBox.Accepted += ConfirmAbortCurrentGameAccepted;

            ScreenManager.AddScreen(confirmQuitMBox);
        }

        static void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            var message = Strings.AreYouWantToExit;
            var confirmQuitMBox = new MessageBoxScreen(message);

            confirmQuitMBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMBox);
        }

        /// <summary>
        /// Если пользователь предпочитает завершить игру вообще...
        /// </summary>
        static void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            EngineManager.Game.Exit();
        }

        static void ConfirmAbortCurrentGameAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(false, new BackgroundScreen(), new MainMenuScreen());
        }
        #endregion

        #region Draw
        /// <summary>
        /// Рисует экран паузы
        /// и останавлвает геймплей
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            BaseEngine.Bloom.Enabled = false;
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            base.Draw(gameTime);
        }
        #endregion
    }

}