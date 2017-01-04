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
    /// Identifies flags used by LayerHeader and which are passed to ovr_SubmitFrame.
    /// </summary>
    /// <see cref="LayerHeader"/>
    [Flags]
    public enum LayerFlags
    {
        /// <summary>
        /// No layer flags specified.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// HighQuality enables 4x anisotropic sampling during the composition of the layer.
        /// The benefits are mostly visible at the periphery for high-frequency &amp; high-contrast visuals.
        /// For best results consider combining this flag with an IntPtr that has mipmaps and
        /// instead of using arbitrary sized textures, prefer texture sizes that are powers-of-two.
        /// Actual rendered viewport and doesn't necessarily have to fill the whole texture.
        /// </summary>
        HighQuality = 0x01,

        /// <summary>
        /// TextureOriginAtBottomLeft: the opposite is TopLeft.
        /// 
        /// Generally this is false for D3D, true for OpenGL.
        /// </summary>
        TextureOriginAtBottomLeft = 0x02,

        /// <summary>
        /// Mark this surface as "headlocked", which means it is specified
        /// relative to the HMD and moves with it, rather than being specified
        /// relative to sensor/torso space and remaining still while the head moves.
        /// 
        /// What used to be ovrLayerType_QuadHeadLocked is now LayerType.Quad plus this flag.
        /// However the flag can be applied to any layer type to achieve a similar effect.
        /// </summary>
        HeadLocked = 0x04
    }
}