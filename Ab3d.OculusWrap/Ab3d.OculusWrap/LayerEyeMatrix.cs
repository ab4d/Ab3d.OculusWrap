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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Ab3d.OculusWrap
{
    /// <summary>
    /// Describes a layer that specifies a monoscopic or stereoscopic view.
    /// This uses a direct 3x4 matrix to map from view space to the UV coordinates.
    /// It is essentially the same thing as ovrLayerEyeFov but using a much
    /// lower level. This is mainly to provide compatibility with specific apps.
    /// Unless the application really requires this flexibility, it is usually better
    /// to use ovrLayerEyeFov.
    ///
    /// Three options exist with respect to mono/stereo texture usage:
    ///    - ColorTextureLeft and ColorTextureRight contain the left and right stereo renderings, respectively.
    ///      ViewportLeft and ViewportRight refer to ColorTextureLeft and ColorTextureRight, respectively.
    ///    - ColorTextureLeft contains both the left and right renderings, ColorTextureRight is null,
    ///      and ViewportLeft and ViewportRight refer to sub-rects with ColorTextureLeft.
    ///    - ColorTextureLeft contains a single monoscopic rendering, and ViewportLeft and
    ///      ViewportRight both refer to that rendering.
    /// </summary>
    /// <see cref="OvrWrap.SubmitFrame(IntPtr, long, IntPtr, ref LayerEyeFov)"/>
    /// <see cref="OvrWrap.SubmitFrame(IntPtr, long, IntPtr, IntPtr, int)"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct LayerEyeMatrix
    {
        /// <summary>
        /// Header.Type must be ovrLayerType_EyeMatrix.
        /// </summary>
        public LayerHeader Header;


        /// <summary>
        /// TextureSwapChains for the left eye respectively.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr ColorTextureLeft;

        /// <summary>
        /// TextureSwapChains for the right eye respectively.
        /// Can be null for cases described above.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr ColorTextureRight;


        /// <summary>
        /// Specifies the ColorTexture sub-rect UV coordinates.
        /// Both ViewportLeft and ViewportRight must be valid.
        /// </summary>
        public Recti ViewportLeft;

        /// <summary>
        /// Specifies the ColorTexture sub-rect UV coordinates.
        /// Both ViewportLeft and ViewportRight must be valid.
        /// </summary>
        public Recti ViewportRight;


        /// <summary>
        /// Specifies the position and orientation of each eye view, with the position specified in meters.
        /// RenderPose will typically be the value returned from ovr_CalcEyePoses,
        /// but can be different in special cases if a different head pose is used for rendering.
        /// </summary>
        public Posef RenderPoseLeft;

        /// <summary>
        /// Specifies the position and orientation of each eye view, with the position specified in meters.
        /// RenderPose will typically be the value returned from ovr_CalcEyePoses,
        /// but can be different in special cases if a different head pose is used for rendering.
        /// </summary>
        public Posef RenderPoseRight;


        /// <summary>
        /// Specifies the mapping from a view-space vector
        /// to a UV coordinate on the textures given above.
        /// P = (x,y,z,1)*Matrix
        /// TexU  = P.x/P.z
        /// TexV  = P.y/P.z
        /// </summary>
        public Matrix4f MatrixLeft;

        /// <summary>
        /// Specifies the mapping from a view-space vector
        /// to a UV coordinate on the textures given above.
        /// P = (x,y,z,1)*Matrix
        /// TexU  = P.x/P.z
        /// TexV  = P.y/P.z
        /// </summary>
        public Matrix4f MatrixRight;


        /// <summary>
        /// Specifies the timestamp when the source ovrPosef (used in calculating RenderPose)
        /// was sampled from the SDK. Typically retrieved by calling ovr_GetTimeInSeconds
        /// around the instant the application calls ovr_GetTrackingState
        /// The main purpose for this is to accurately track app tracking latency.
        /// </summary>
        public double SensorSampleTime;
    };
}