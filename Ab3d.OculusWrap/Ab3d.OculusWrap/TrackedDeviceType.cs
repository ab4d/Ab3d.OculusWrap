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
    /// Position tracked devices
    /// </summary>	
    [Flags]
    public enum TrackedDeviceType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// HMD
        /// </summary>
        HMD = 0x0001,

        /// <summary>
        /// LTouch
        /// </summary>
        LTouch = 0x0002,

        /// <summary>
        /// RTouch
        /// </summary>
        RTouch = 0x0004,

        /// <summary>
        /// Touch
        /// </summary>
        Touch = LTouch | RTouch,

        Object0 = 0x0010,
        Object1 = 0x0020,
        Object2 = 0x0040,
        Object3 = 0x0080,
    }
}