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
    /// OvrWrap32 is an instance of base <see cref="OvrWrap"/> class that is created for x86 process. See remarks of the base class for more info.
    /// </summary>
    public sealed partial class OvrWrap32 : OvrWrap
    {
        internal const string _ovrDllName = "LibOVRRT32_1.dll";

        /// <inheritdoc />
        public override string OvrDllName { get { return _ovrDllName; } }

        // OvrWrap32 can be created only from static OvrWrap.Create or OvrWrap.Create32 methods
        internal OvrWrap32()
        {
        }


        /// <inheritdoc />
        public override HmdDesc GetHmdDesc(IntPtr sessionPtr)
        {
            HmdDesc hmdDesc;
            SafeNativeMethods.ovr_GetHmdDesc32(out hmdDesc, sessionPtr);

            return hmdDesc;
        }
    }
}