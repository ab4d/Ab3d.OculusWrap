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
    /// Description used to create a texture swap chain.
    /// </summary>
    /// <see cref="OvrWrap.CreateTextureSwapChainDX"/>
    /// <see cref="OvrWrap.CreateTextureSwapChainGL"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureSwapChainDesc
    {
        /// <summary>
        /// Type
        /// </summary>
        public TextureType			Type;
        /// <summary>
        /// Format
        /// </summary>
        public TextureFormat		Format;

        /// <summary>
        /// Must be 6 for ovrTexture_Cube, 1 for other types.
        /// </summary>
        public int					ArraySize;

        /// <summary>
        /// Width
        /// </summary>
        public int					Width;

        /// <summary>
        /// Height
        /// </summary>
        public int					Height;

        /// <summary>
        /// MipLevels
        /// </summary>
        public int					MipLevels;

        /// <summary>
        /// Only supported with depth textures
        /// </summary>
        public int					SampleCount;

        /// <summary>
        /// Not buffered in a chain. For images that don't change
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] // Marshal byte to bool (0 = false, all other = true)
        public bool				    StaticImage;

        /// <summary>
        /// ovrTextureMiscFlags
        /// </summary>
        public TextureMiscFlags		MiscFlags;

        /// <summary>
        /// ovrTextureBindFlags. Not used for GL.
        /// </summary>
        public TextureBindFlags		BindFlags;
    }
}