// ----------------------------------------------------------------
// <copyright file="XInputControllerButtonChangedEventArgs.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using SharpDX.XInput;

namespace Ab3d.Controls
{
    public delegate void XInputControllerButtonChangedHandler(object sender, XInputControllerButtonChangedEventArgs e);

    public class XInputControllerButtonChangedEventArgs : EventArgs
    {
        public GamepadButtonFlags OldButtonFlags { get; private set; }
        public GamepadButtonFlags NewButtonFlags { get; private set; }

        public bool IsHandled { get; set; }

        public XInputControllerButtonChangedEventArgs(GamepadButtonFlags oldButtonFlags, GamepadButtonFlags newButtonFlags)
        {
            OldButtonFlags = oldButtonFlags;
            NewButtonFlags = newButtonFlags;

            IsHandled = false;
        }
    }
}