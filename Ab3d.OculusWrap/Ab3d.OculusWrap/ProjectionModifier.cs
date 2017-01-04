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
    /// Enumerates modifications to the projection matrix based on the application's needs.
    /// </summary>
    /// <see cref="OvrWrap.Matrix4f_Projection"/>
    public enum ProjectionModifier
    {
        /// <summary>
        /// Use for generating a default projection matrix that is:
        /// * Right-handed.
        /// * Near depth values stored in the depth buffer are smaller than far depth values.
        /// * Both near and far are explicitly defined.
        /// * With a clipping range that is (0 to w).
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Enable if using left-handed transformations in your application.
        /// </summary>
        LeftHanded = 0x01,

        /// <summary>
        /// After the projection transform is applied, far values stored in the depth buffer will be less than closer depth values.
        /// NOTE: Enable only if the application is using a floating-point depth buffer for proper precision.
        /// </summary>
        FarLessThanNear = 0x02,

        /// <summary>
        /// When this flag is used, the zfar value pushed into ovrMatrix4f_Projection() will be ignored
        /// NOTE: Enable only if ovrProjection_FarLessThanNear is also enabled where the far clipping plane will be pushed to infinity.
        /// </summary>
        FarClipAtInfinity = 0x04,

        /// <summary>
        /// Enable if the application is rendering with OpenGL and expects a projection matrix with a clipping range of (-w to w).
        /// Ignore this flag if your application already handles the conversion from D3D range (0 to w) to OpenGL.
        /// </summary>
        ClipRangeOpenGL = 0x08
    }
}