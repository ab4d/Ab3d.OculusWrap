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
    /// Misc flags overriding particular behaviors of a texture swap chain
    /// </summary>
    /// <see cref="TextureSwapChainDesc"/>
    [Flags]
    public enum TextureMiscFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Vulkan and DX only: The underlying texture is created with a TYPELESS equivalent
        /// of the format specified in the texture desc. The SDK will still access the
        /// texture using the format specified in the texture desc, but the app can
        /// create views with different formats if this is specified.
        /// </summary>
        DX_Typeless = 0x0001,

        /// <summary>
        /// DX only: Allow generation of the mip chain on the GPU via the GenerateMips
        /// call. This flag requires that RenderTarget binding also be specified.
        /// </summary>
        AllowGenerateMips = 0x0002,

        /// <summary>
        /// Texture swap chain contains protected content, and requires
        /// HDCP connection in order to display to HMD. Also prevents
        /// mirroring or other redirection of any frame containing this contents
        /// </summary>
        ProtectedContent = 0x0004,
    }
}