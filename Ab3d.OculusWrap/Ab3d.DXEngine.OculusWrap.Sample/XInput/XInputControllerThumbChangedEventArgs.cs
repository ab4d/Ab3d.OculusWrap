// ----------------------------------------------------------------
// <copyright file="XInputControllerThumbChangedEventArgs.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;

namespace Ab3d.Controls
{
    public delegate void XInputControllerThumbChangedHandler(object sender, XInputControllerThumbChangedEventArgs e);

    public class XInputControllerThumbChangedEventArgs : EventArgs
    {
        public int ThumbX { get; private set; }
        public int ThumbY { get; private set; }

        public double NormalizedX { get; private set; }
        public double NormalizedY { get; private set; }

        public bool IsHandled { get; set; }

        public XInputControllerThumbChangedEventArgs(int thumbX, int thumbY, double normalizedX, double normalizedY)
        {
            ThumbX = thumbX;
            ThumbY = thumbY;

            NormalizedX = normalizedX;
            NormalizedY = normalizedY;

            IsHandled = false;
        }
    }
}