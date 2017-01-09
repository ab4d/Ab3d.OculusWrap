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
    /// The bindings required for texture swap chain.
    ///
    /// All texture swap chains are automatically bindable as shader
    /// input resources since the Oculus runtime needs this to read them.
    /// </summary>
    /// <see cref="TextureSwapChainDesc"/>
    [Flags]
    public enum TextureBindFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// The application can write into the chain with pixel shader.
        /// </summary>
        DX_RenderTarget = 0x0001,

        /// <summary>
        /// The application can write to the chain with compute shader.
        /// </summary>
        DX_UnorderedAccess = 0x0002,

        /// <summary>
        /// The chain buffers can be bound as depth and/or stencil buffers.
        /// </summary>
        DX_DepthStencil = 0x0004,
    }
}