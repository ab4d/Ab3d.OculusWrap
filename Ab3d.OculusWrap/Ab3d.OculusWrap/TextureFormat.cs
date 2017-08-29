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
    // The Oculus SDK is full of missing comments.
    // Ignore warnings regarding missing comments, in this class.
    #pragma warning disable 1591

    /// <summary>
    /// The format of a texture.
    /// </summary>
    /// <see cref="TextureSwapChainDesc"/>
    public enum TextureFormat
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Not currently supported on PC. Requires a DirectX 11.1 device.
        /// </summary>
        B5G6R5_UNorm = 1,

        /// <summary>
        /// Not currently supported on PC. Requires a DirectX 11.1 device.
        /// </summary>
        B5G5R5A1_UNorm = 2,

        /// <summary>
        /// Not currently supported on PC. Requires a DirectX 11.1 device.
        /// </summary>
        B4G4R4A4_UNorm = 3,

        R8G8B8A8_UNorm = 4,
        R8G8B8A8_UNorm_SRgb = 5,
        B8G8R8A8_UNorm = 6,

        /// <summary>
        /// Not supported for OpenGL applications
        /// </summary>
        B8G8R8A8_UNorm_SRgb = 7,

        /// <summary>
        /// Not supported for OpenGL applications
        /// </summary>
        B8G8R8X8_UNorm = 8,

        /// <summary>
        /// Not supported for OpenGL applications
        /// </summary>
        B8G8R8X8_UNorm_SRgb = 9,

        R16G16B16A16_Float = 10,

        R11G11B10_Float = 25, // Introduced in v1.10

        D16_UNorm = 11,
        D24_UNorm_S8_UInt = 12,
        D32_Float = 13,
        D32_Float_S8X24_UInt = 14,

        // Added in 1.5 compressed formats can be used for static layers
        BC1_UNorm = 15,
        BC1_UNorm_SRgb = 16,
        BC2_UNorm = 17,
        BC2_UNorm_SRgb = 18,
        BC3_UNorm = 19,
        BC3_UNorm_SRgb = 20,
        BC6H_UF16 = 21,
        BC6H_SF16 = 22,
        BC7_UNorm = 23,
        BC7_UNorm_SRgb = 24,
    }

    #pragma warning restore 1591
}