//#define TRY

using System;
using AntiTankGame2.GameScreens;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.Graphics.Screens;

#if !XBOX
using System.Windows.Forms;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

#endif

namespace AntiTankGame2
{
#if WINDOWS || XBOX
  
    static class Program
    {

        static void Main()
        {
//#if DEBUG
           // StartUnitTests();
//#else
            StartGame();
//#endif
        }
        #region StartGame

        private static void StartGame()
        {
#if !XBOX360
#if TRY
            try
            {
#endif
#endif
                using (var game = new EngineManager("Game"))
                {
                    EngineManager.Game = game;
                    SetupScene();
                    game.Run();
#if TRY
                }
#endif
#if !XBOX360
            }
            //catch (NoSuitableGraphicsDeviceException)
            //{
            //    MessageBox.Show(Strings.SystemRequirements, Strings.GameName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //  }
            //    //  catch (Microsoft.Xna.Framework.Graphics. OutOfVideoMemoryException)
            //    //  {
            //    MessageBox.Show(string.Format(Strings.ErrorOutOfVideoMemory, Environment.NewLine), Strings.GameName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
#if TRY
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Strings.GameName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
#endif
        }

        private static void SetupScene()
        {
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }

        #endregion
    }

#endif
}
