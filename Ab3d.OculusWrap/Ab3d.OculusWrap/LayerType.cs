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
    /// Describes layer types that can be passed to ovr_SubmitFrame.
    /// Each layer type has an associated struct, such as ovrLayerEyeFov.
    /// </summary>
    /// <see cref="LayerHeader"/>
    public enum LayerType
    {
        /// <summary>
        /// Layer is disabled.
        /// </summary>
        Disabled       = 0,

        /// <summary>
        /// Described by LayerEyeFov.
        /// </summary>
        EyeFov         = 1,

        /// <summary>
        /// Described by LayerQuad. 
        /// 
        /// Previously called QuadInWorld.
        /// </summary>
        Quad           = 3,

        // enum 4 used to be ovrLayerType_QuadHeadLocked. 
        // Instead, use ovrLayerType_Quad with ovrLayerFlag_HeadLocked.

        /// <summary>
        /// Described by LayerEyeMatrix.
        /// </summary>
        EyeMatrix      = 5,
    }
}