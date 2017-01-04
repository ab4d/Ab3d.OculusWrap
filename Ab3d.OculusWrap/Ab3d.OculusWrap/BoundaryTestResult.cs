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
    /// Provides boundary test information
    /// </summary>	
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundaryTestResult
    {
        /// <summary>	
        /// True if the boundary system is being triggered. Note that due to fade in/out effects this may not exactly match visibility.
        /// </summary>	
        [MarshalAs(UnmanagedType.U1)] // Marshal byte to bool (0 = false, all other = true)
        public bool IsTriggering;

        /// <summary>	
        /// Distance to the closest play area or outer boundary surface.
        /// </summary>	
        public float ClosestDistance;

        /// <summary>	
        /// Closest point on the boundary surface.
        /// </summary>	
        public Vector3f ClosestPoint;

        /// <summary>	
        /// Unit surface normal of the closest boundary surface.
        /// </summary>	
        public Vector3f ClosestPointNormal;
    }
}