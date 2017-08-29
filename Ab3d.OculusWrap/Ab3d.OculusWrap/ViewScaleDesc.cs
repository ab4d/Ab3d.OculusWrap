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
    /// Contains the data necessary to properly calculate position info for various layer types.<br/>
    /// - HmdToEyePose is the same value pair provided in ovrEyeRenderDesc.<br/>
    /// - HmdSpaceToWorldScaleInMeters is used to scale player motion into in-application units.<br/>
    ///   In other words, it is how big an in-application unit is in the player's physical meters.
    ///   For example, if the application uses inches as its units then HmdSpaceToWorldScaleInMeters
    ///   would be 0.0254.<br/>
    ///   Note that if you are scaling the player in size, this must also scale. So if your application
    ///   units are inches, but you're shrinking the player to half their normal size, then
    ///   HmdSpaceToWorldScaleInMeters would be 0.0254*2.0.
    /// <seealso cref="EyeRenderDesc"/>, OvrWrap.SubmitFrame
    /// </summary>
    /// <see cref="EyeRenderDesc"/>
    /// <see cref="OvrWrap.SubmitFrame(IntPtr, long, IntPtr, ref LayerEyeFov)"/>
    /// <see cref="OvrWrap.SubmitFrame(IntPtr, long, IntPtr, IntPtr, int)"/>
    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct ViewScaleDesc
    {
        // Before version 1.17 the EyeRenderDesc contained HmdToEyeOffset instead of HmdToEyePose
        ///// <summary>
        ///// Translation of each eye.
        ///// 
        ///// The same value pair provided in EyeRenderDesc.
        ///// </summary>
        //public Vector3f[] HmdToEyeOffset;


        // We cannot marshal array of Posef structs, so there are two fields instead of array of two Posef structs

        /// <summary>
        /// Transform of first eye from the HMD center, in meters.
        /// </summary>
        public Posef HmdToEyePose0;
        
        /// <summary>
        /// Transform of second eye from the HMD center, in meters.
        /// </summary>
        public Posef HmdToEyePose1;


			
        /// <summary>
        /// Ratio of viewer units to meter units.
        /// 
        /// Used to scale player motion into in-application units.
        /// In other words, it is how big an in-application unit is in the player's physical meters.
        /// For example, if the application uses inches as its units then HmdSpaceToWorldScaleInMeters would be 0.0254.
        /// Note that if you are scaling the player in size, this must also scale. So if your application
        /// units are inches, but you're shrinking the player to half their normal size, then
        /// HmdSpaceToWorldScaleInMeters would be 0.0254*2.0.
        /// </summary>
        public float		HmdSpaceToWorldScaleInMeters;
    }
}