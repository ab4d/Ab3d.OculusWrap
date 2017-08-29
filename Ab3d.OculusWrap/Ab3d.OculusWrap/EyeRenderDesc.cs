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

using System.Runtime.InteropServices;

namespace Ab3d.OculusWrap
{
    /// <summary>
    /// Rendering information for each eye. Computed by ovr_GetRenderDesc() based on the
    /// specified FOV. Note that the rendering viewport is not included
    /// here as it can be specified separately and modified per frame by
    /// passing different Viewport values in the layer structure.
    /// </summary>
    /// <see cref="OvrWrap.GetRenderDesc"/>
    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct EyeRenderDesc
    {
        /// <summary>
        /// The eye index to which this instance corresponds.
        /// </summary>
        public EyeType  Eye;

        /// <summary>
        /// The field of view.
        /// </summary>
        public FovPort	Fov;

        /// <summary>
        /// Distortion viewport.
        /// </summary>
        public Recti	DistortedViewport; 	        

        /// <summary>
        /// How many display pixels will fit in tan(angle) = 1.
        /// </summary>
        public Vector2f	PixelsPerTanAngleAtCenter;

        // Before version 1.17 the EyeRenderDesc contained HmdToEyeOffset instead of HmdToEyePose
        ///// <summary>
        ///// Translation of each eye, in meters.
        ///// </summary>
        //public Vector3f	HmdToEyeOffset;

        /// <summary>
        /// Transform of eye from the HMD center, in meters.
        /// </summary>
        public Posef HmdToEyePose;
    }
}