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
using System.Runtime.InteropServices;

namespace Ab3d.OculusWrap
{
    /// <summary>
    /// Specifies status information for the current session.
    /// </summary>
    /// <see cref="OvrWrap.GetSessionStatus"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct SessionStatus
    {
        /// <summary>
        /// True if the process has VR focus and thus is visible in the HMD.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] // Marshal byte to bool (0 = false, all other = true)
        public bool IsVisible;

        /// <summary>
        /// True if an HMD is present.
        /// </summary>       
        [MarshalAs(UnmanagedType.U1)]
        public bool HmdPresent;

        /// <summary>
        /// True if the HMD is on the user's head.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool HmdMounted;

        /// <summary>
        /// True if the session is in a display-lost state. See ovr_SubmitFrame.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool DisplayLost;

        /// <summary>
        /// True if the application should initiate shutdown.    
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool ShouldQuit;

        /// <summary>
        /// True if UX has requested re-centering. 
        /// Must call ovr_ClearShouldRecenterFlag or ovr_RecenterTrackingOrigin.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool ShouldRecenter;

        [MarshalAs(UnmanagedType.U1)]
        public bool Internal0;

        [MarshalAs(UnmanagedType.U1)]
        public bool Internal1;
    }
}