using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework.Input;

namespace NccPcGamePad.DirectNcc
{
    public class DirectInputGamepad
    {
        private static List<DirectInputGamepad> gamepads;
        public static List<DirectInputGamepad> Gamepads
        {
            get
            {
                if (gamepads == null)
                {
                    ReloadGamepads();
                }
                return gamepads;
            }
        }

        /// <summary>
        /// Normally for internal use only; call if user has attached new Gamepads,
        /// or detached Gamepads you want discarded
        /// Otherwise, loaded once on first Gamepad request and does not reflect changes in gamepad attachment
        /// TODO: Do this better
        /// </summary>
        public static void ReloadGamepads()
        {
            // gamepads generally misidentified as Joysticks in DirectInput... get both
            
            var gamepadInstanceList = Manager.GetDevices(DeviceType.Gamepad, EnumDevicesFlags.AttachedOnly);
            var joystickInstanceList = Manager.GetDevices(DeviceType.Joystick, EnumDevicesFlags.AttachedOnly);

            gamepads = new List<DirectInputGamepad>(gamepadInstanceList.Count + joystickInstanceList.Count);

            foreach (var gamepad in
                from DeviceInstance deviceInstance in gamepadInstanceList
                select new DirectInputGamepad(deviceInstance.InstanceGuid))
            {
                gamepads.Add(gamepad);
            }
            foreach (var gamepad in
                from DeviceInstance deviceInstance in joystickInstanceList
                select new DirectInputGamepad(deviceInstance.InstanceGuid))
            {
                gamepads.Add(gamepad);
            }
        }


        public Device Device { get; protected set; }

        protected DirectInputGamepad(Guid gamepadInstanceGuid)
        {
            Device = new Device(gamepadInstanceGuid);
            Device.SetDataFormat(DeviceDataFormat.Joystick);
            Device.Acquire();
        }

        public DirectInputThumbSticks ThumbSticks
        {
            get { return new DirectInputThumbSticks(Device); }
        }

        public DirectInputDPad DPad
        {
            get
            {
                var currentJoystickState = Device.CurrentJoystickState;
                return new DirectInputDPad(currentJoystickState.GetPointOfView()[0]);	// note that there could be a total of 4 DPads on the PC
            }
        }

        public DirectInputButtons Buttons
        {
            get { return new DirectInputButtons(Device); }
        }

        #region Diagnostics
        public string DiagnosticsThumbSticks
        {
            get
            {
                return
                  
                string.Format("X {0} Y{1} X{2} Y{3}",
                              Math.Round(ThumbSticks.Left.X, 4),
                              Math.Round(ThumbSticks.Left.Y, 4),
                              Math.Round(ThumbSticks.Right.X, 4),
                              Math.Round(ThumbSticks.Right.Y, 4));
            }
        }

        public string DiagnosticsRawGamepadData
        {
            get
            {
                return
                 
                string.Format("X{0} Y{1} Z{2} Rx{3} Ry{4} Rz{5} pov[0]{6}",
                                Device.CurrentJoystickState.X,
                                Device.CurrentJoystickState.Y,
                                Device.CurrentJoystickState.Z,
                                Device.CurrentJoystickState.Rx,
                                Device.CurrentJoystickState.Ry,
                                Device.CurrentJoystickState.Rz,
                                Device.CurrentJoystickState.GetPointOfView()[0]
                    )
;

            }
        }

        public string DiagnosticsButtons
        {
            get
            {
                var stringBuilder = new System.Text.StringBuilder();

                var i = 0;
                foreach (var buttonState in Buttons.List)
                {
                    stringBuilder.Append(i);
                    stringBuilder.Append("=");
                    stringBuilder.Append((buttonState == ButtonState.Pressed ? "1" : "0"));
                    stringBuilder.Append(" ");
                    i++;
                }

                return stringBuilder.ToString();
            }
        }
        #endregion
    }
}