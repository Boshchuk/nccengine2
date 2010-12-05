using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NccPcGamePad.DualShock;
#if !XBOX

#endif

namespace NccEngine2.GameComponents.NccInput
{
    public class Input : GameComponent
    {
        public KeyboardState CurrentKeyboardState;

        public GamePadState CurrentGamePadState;

        public KeyboardState LastKeyboardState;

        public GamePadState LastGamePadState;

        /// <summary>
        /// Keys pressed last frame, for comparison if a key was just pressed.
        /// </summary>
        private static List<Keys> keysPressedLastFrame = new List<Keys>();


#if !XBOX
        public MouseState CurrentMouseState;

        public MouseState LastMouseState;

        private Point lastMouseLocation;

        public Vector2 MouseMoved { get; private set; }

        public PcDualShockState CurrentSimpleGamePadState;
        public PcDualShockState LastSimpleGamePadState;

        private static bool wasError; //By default  no Error

#endif

        public Input(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePadState = CurrentGamePadState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);

#if !XBOX
                LastMouseState = CurrentMouseState;

                CurrentMouseState = Mouse.GetState();

                LastSimpleGamePadState = CurrentSimpleGamePadState;

                //#warning может плодить ошибки если джойстик не подключен... //TODO придумать решение проблемыс октлюченным контролером
                if (!wasError)
                {
                    try
                    {
       // #if !DEGUG
                            //TODO check the type of geme pad...
                            CurrentSimpleGamePadState = PcDualShock.GetState(PlayerIndex.One);

      //  #endif
                    }
                    catch (Exception) //If Any exception
                    {
        #if !XBOX
                            wasError = true;
        #endif
                    }
                }


                MouseMoved = new Vector2(LastMouseState.X - CurrentMouseState.X, LastMouseState.Y - CurrentMouseState.Y);
                lastMouseLocation = new Point(CurrentMouseState.X, CurrentMouseState.Y);
#endif
        }

        /// <summary>
        /// Checks for a "menu up" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
#if !XBOX
 (CurrentSimpleGamePadState.DPad.Up == ButtonState.Pressed &&
 LastGamePadState.DPad.Up == ButtonState.Released) ||
#endif
 (CurrentGamePadState.DPad.Up == ButtonState.Pressed &&
                        LastGamePadState.DPad.Up == ButtonState.Released) ||
                       (CurrentGamePadState.ThumbSticks.Left.Y > 0 &&
                        LastGamePadState.ThumbSticks.Left.Y <= 0);
            }
        }

        /// <summary>
        /// Checks for a "menu down" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
#if !XBOX
 (CurrentSimpleGamePadState.DPad.Down == ButtonState.Pressed &&
                     LastGamePadState.DPad.Down == ButtonState.Released) ||
                     (CurrentSimpleGamePadState.ThumbSticks.Left.Y < 0 && LastSimpleGamePadState.ThumbSticks.Left.Y >= 0) ||
#endif
 (CurrentGamePadState.DPad.Down == ButtonState.Pressed &&
                        LastGamePadState.DPad.Down == ButtonState.Released) ||
                       (CurrentGamePadState.ThumbSticks.Left.Y < 0 &&
                        LastGamePadState.ThumbSticks.Left.Y >= 0);
            }
        }

        /// <summary>
        /// Checks for a "menu select" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       (CurrentGamePadState.Buttons.A == ButtonState.Pressed &&
                        LastGamePadState.Buttons.A == ButtonState.Released) ||
#if !XBOX
 (CurrentSimpleGamePadState.Buttons.Start == ButtonState.Pressed && LastSimpleGamePadState.Buttons.Start == ButtonState.Released) ||
#endif

 (CurrentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Start == ButtonState.Released);
            }
        }

        /// <summary>
        /// Checks for a "menu cancel" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePadState.Buttons.B == ButtonState.Pressed &&
                        LastGamePadState.Buttons.B == ButtonState.Released) ||

                       (CurrentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Back == ButtonState.Released);
            }
        }

        /// <summary>
        /// Checks for a "pause the game" input action (on either keyboard or gamepad).
        /// </summary>
        public bool ExitGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Back == ButtonState.Released);
            }
        }
        private bool pausedGame;
        public bool PauseGame
        {
            get
            {
                if (IsNewKeyPress(Keys.Space) ||
#if !XBOX
 (CurrentSimpleGamePadState.Buttons.Start == ButtonState.Pressed && LastSimpleGamePadState.Buttons.Start == ButtonState.Released) ||
#endif
 (CurrentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Start == ButtonState.Released))
                    pausedGame = !pausedGame;
                return pausedGame;
            }
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Keyboard key just pressed
        /// </summary>
        /// <returns>Bool</returns>
        public static bool KeyboardKeyJustPressed(Keys key)
        {

            return Keyboard.GetState()./* CurrentKeyboardState.*/IsKeyDown(key) &&
                keysPressedLastFrame.Contains(key) == false;
        } // KeyboardSpaceJustPressed

    }
}