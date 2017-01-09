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
    /// 4x4 Matrix used in Oculus SDK
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4f
    {
        /// <summary>
        /// M11
        /// </summary>
        public float M11;
        /// <summary>
        /// M12
        /// </summary>
        public float M12;
        /// <summary>
        /// M13
        /// </summary>
        public float M13;
        /// <summary>
        /// M14
        /// </summary>
        public float M14;
        /// <summary>
        /// M21
        /// </summary>
        public float M21;
        /// <summary>
        /// M22
        /// </summary>
        public float M22;
        /// <summary>
        /// M23
        /// </summary>
        public float M23;
        /// <summary>
        /// M24
        /// </summary>
        public float M24;
        /// <summary>
        /// M31
        /// </summary>
        public float M31;
        /// <summary>
        /// M32
        /// </summary>
        public float M32;
        /// <summary>
        /// M33
        /// </summary>
        public float M33;
        /// <summary>
        /// M34
        /// </summary>
        public float M34;
        /// <summary>
        /// M41
        /// </summary>
        public float M41;
        /// <summary>
        /// M42
        /// </summary>
        public float M42;
        /// <summary>
        /// M43
        /// </summary>
        public float M43;
        /// <summary>
        /// M44
        /// </summary>
        public float M44;
    }
}