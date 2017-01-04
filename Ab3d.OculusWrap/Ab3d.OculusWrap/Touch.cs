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

namespace Ab3d.OculusWrap
{
    /// <summary>
    /// Describes touch input types.
    /// These values map to capacitive touch values reported ovrInputState::Touch.
    /// Some of these values are mapped to button bits for consistency.
    /// </summary>
    public enum Touch
    {
        A              = Button.A,
        B              = Button.B,
        RThumb         = Button.RThumb,
        RThumbRest     = 0x00000008,
        RIndexTrigger  = 0x00000010,

        /// <summary>
        /// Bit mask of all the button touches on the right controller
        /// </summary>
        RButtonMask    = A | B | RThumb | RThumbRest | RIndexTrigger,

        X              = Button.X,
        Y              = Button.Y,
        LThumb         = Button.LThumb,
        LThumbRest     = 0x00000800,
        LIndexTrigger  = 0x00001000,

        /// <summary>
        /// Bit mask of all the button touches on the left controller
        /// </summary>
        LButtonMask    = X | Y | LThumb | LThumbRest | LIndexTrigger,

        // Finger pose state 
        // Derived internally based on distance, proximity to sensors and filtering.
        RIndexPointing = 0x00000020,
        RThumbUp       = 0x00000040,    

        /// <summary>
        /// Bit mask of all right controller poses
        /// </summary>
        RPoseMask      = RIndexPointing | RThumbUp,

        LIndexPointing = 0x00002000,
        LThumbUp       = 0x00004000,

        /// <summary>
        /// Bit mask of all left controller poses
        /// </summary>
        LPoseMask      = LIndexPointing | LThumbUp,
    }
}