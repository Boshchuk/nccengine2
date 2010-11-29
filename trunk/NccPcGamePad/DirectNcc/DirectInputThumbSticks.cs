using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework;

namespace NccPcGamePad.DirectNcc
{
    /// <summary>
    /// A struct offering the current positions of as many as 3 thumbsticks on a PC gamepad or joystick
    /// </summary>
    /// <remarks>
    /// For unusual joysticks, these "thumbsticks" may be whatever the hardware-designer imagined;
    /// for example, Right.Y might be a jet-throttle and Right.X might be the rotational position of a steering wheel
    /// In other words, being in the list of Gamepads doesn't mean it looks anything like a Gamepad
    /// </remarks>
    public struct DirectInputThumbSticks
    {
        /// <summary>
        /// Check HasLeft, HasRight, etc before getting these values; will always be 0 if this gamepad lacks the requested thumbstick
        /// </summary>
        public Vector2 Left;
        public Vector2 Right;
        public Vector2 Third;
        //public Microsoft.Xna.Framework.Vector2 Fourth;

        public bool HasLeft;
        public bool HasRight;
        public bool HasThird;
        //public bool HasFourth;

        const float Center = 32767.5f;

        public DirectInputThumbSticks(Device device)
        {
            var joystickState = device.CurrentJoystickState;

            HasLeft = false;
            Left = Vector2.Zero;
            HasRight = false;
            Right = Vector2.Zero;
            HasThird = false;
            Third = Vector2.Zero;

            if (device.Caps.NumberAxes > 0)
            {
                HasLeft = true;
                Left = new Vector2((joystickState.X - Center) / Center, (joystickState.Y - Center) / Center);

                if (device.Caps.NumberAxes > 2)
                {
                    HasRight = true;
                    Right = new Vector2((joystickState.Rz - Center) / Center, (joystickState.Z - Center) / Center);

                    if (device.Caps.NumberAxes > 4)
                    {
                        HasThird = true;
                        Third = new Vector2((joystickState.Rx - Center) / Center, (joystickState.Ry - Center) / Center);
                    }
                }
            }
        }
    }
}