// Copyright (c) 2017 AB4D d.o.o.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Based on OculusWrap project created by MortInfinite and licensed as Ms-PL (https://oculuswrap.codeplex.com/)

using System;

namespace Ab3d.OculusWrap
{
    /// <summary>
    /// Describes button input types.
    /// Button inputs are combined; that is they will be reported as pressed if they are 
    /// pressed on either one of the two devices.
    /// The ovrButton_Up/Down/Left/Right map to both XBox D-Pad and directional buttons.
    /// The ovrButton_Enter and ovrButton_Return map to Start and Back controller buttons, respectively.
    /// </summary>
    [Flags]
    public enum Button
    {
        /// <summary>
        /// A
        /// </summary>
        A         = 0x00000001,

        /// <summary>
        /// B
        /// </summary>
        B         = 0x00000002,

        /// <summary>
        /// RThumb
        /// </summary>
        RThumb = 0x00000004,

        /// <summary>
        /// RShoulder
        /// </summary>
        RShoulder = 0x00000008,

        /// <summary>
        /// Bit mask of all buttons on the right Touch controller
        /// </summary>
        RMask     = A | B | RThumb | RShoulder,

        /// <summary>
        /// X
        /// </summary>
        X         = 0x00000100,

        /// <summary>
        /// Y
        /// </summary>
        Y         = 0x00000200,

        /// <summary>
        /// LThumb
        /// </summary>
        LThumb = 0x00000400,

        /// <summary>
        /// LShoulder
        /// </summary>
        LShoulder = 0x00000800,

        /// <summary>
        /// Bit mask of all buttons on the left Touch controller
        /// </summary>
        LMask     = X | Y | LThumb | LShoulder,

        // Navigation through DPad.

        /// <summary>
        /// DPad Up
        /// </summary>
        Up = 0x00010000,

        /// <summary>
        /// DPad Down
        /// </summary>
        Down = 0x00020000,

        /// <summary>
        /// DPad Left
        /// </summary>
        Left = 0x00040000,

        /// <summary>
        /// DPad Right
        /// </summary>
        Right = 0x00080000,

        /// <summary>
        /// Start on XBox controller.
        /// </summary>
        Enter = 0x00100000, 

        /// <summary>
        /// Back on Xbox controller.
        /// </summary>
        Back = 0x00200000,

        /// <summary>
        /// Volume button on Oculus Remote. Not present on XBox or Touch controllers.
        /// </summary>
        VolUp = 0x00400000,

        /// <summary>
        /// Volume button on Oculus Remote. Not present on XBox or Touch controllers.
        /// </summary>
        VolDown = 0x00800000,

        /// <summary>
        /// Home button on XBox controllers. Oculus button on Touch controllers and Oculus Remote.
        /// </summary>
        Home = 0x01000000,

        /// <summary>
        /// Bit mask of all buttons that are for private usage by Oculus
        /// </summary>
        Private = VolUp | VolDown | Home,
    }
}