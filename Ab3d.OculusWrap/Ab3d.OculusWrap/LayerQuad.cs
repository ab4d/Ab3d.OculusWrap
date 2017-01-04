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
    /// Describes a layer of Quad type, which is a single quad in world or viewer space.
    /// It is used for ovrLayerType_Quad. This type of layer represents a single
    /// object placed in the world and not a stereo view of the world itself.
    ///
    /// A typical use of ovrLayerType_Quad is to draw a television screen in a room
    /// that for some reason is more convenient to draw as a layer than as part of the main
    /// view in layer 0. For example, it could implement a 3D popup GUI that is drawn at a
    /// higher resolution than layer 0 to improve fidelity of the GUI.
    ///
    /// Quad layers are visible from both sides; they are not back-face culled.
    /// </summary>
    /// <see cref="OvrWrap.SubmitFrame(IntPtr, long, IntPtr, ref LayerEyeFov)"/>
    /// <see cref="OvrWrap.SubmitFrame(IntPtr, long, IntPtr, IntPtr, int)"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct LayerQuad
    {
        /// <summary>
        /// Header.Type must be ovrLayerType_Quad.
        /// </summary>
        public LayerHeader		Header;

        /// <summary>
        /// Contains a single image, never with any stereo view.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr			ColorTexture;

        /// <summary>
        /// Specifies the ColorTexture sub-rect UV coordinates.
        /// </summary>
        public Recti			Viewport;

        /// <summary>
        /// Specifies the orientation and position of the center point of a Quad layer type.
        /// The supplied direction is the vector perpendicular to the quad.
        /// The position is in real-world meters (not the application's virtual world,
        /// the physical world the user is in) and is relative to the "zero" position
        /// set by ovr_RecenterTrackingOrigin unless the ovrLayerFlag_HeadLocked flag is used.
        /// </summary>
        public Posef			QuadPoseCenter;

        /// <summary>
        /// Width and height (respectively) of the quad in meters.
        /// </summary>
        public Vector2f			QuadSize;
    }
}