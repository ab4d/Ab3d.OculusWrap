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
    public sealed partial class OvrWrap32 : OvrWrap
    {
        internal const string _ovrDllName = "LibOVRRT32_1.dll";

        /// <summary>
        /// Filename of the DllOVR wrapper file, which wraps the LibOvr.lib in a dll.
        /// </summary>
        public override string OvrDllName { get { return _ovrDllName; } }

        // OvrWrap32 can be created only from static OvrWrap.Create or OvrWrap.Create32 methods
        internal OvrWrap32()
        {
        }


        /// <summary>
        /// Returns information about the current HMD.
        /// 
        /// Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
        /// will be reported as None.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by Create, else NULL in which
        /// case this function detects whether an HMD is present and returns its info if so.
        /// </param>
        /// <returns>
        /// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is None then no HMD is present.
        /// </returns>
        public override HmdDesc GetHmdDesc(IntPtr sessionPtr)
        {
            HmdDesc hmdDesc;
            SafeNativeMethods.ovr_GetHmdDesc32(out hmdDesc, sessionPtr);

            return hmdDesc;
        }
    }
}