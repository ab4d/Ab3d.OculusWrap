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

namespace Ab3d.OculusWrap
{
    public struct CameraIntrinsics
    {
        /// <summary>
        /// Time in seconds from last change to the parameters
        /// </summary>
        public double LastChangedTime;

        /// <summary>
        /// Angles of all 4 sides of viewport
        /// </summary>
        public FovPort FOVPort;

        /// <summary>
        /// Near plane of the virtual camera used to match the external camera
        /// </summary>
        public float VirtualNearPlaneDistanceMeters;

        /// <summary>
        /// Far plane of the virtual camera used to match the external camera
        /// </summary>
        public float VirtualFarPlaneDistanceMeters;

        /// <summary>
        /// Height in pixels of image sensor
        /// </summary>
        public Sizei ImageSensorPixelResolution;

        /// <summary>
        /// The lens distortion matrix of camera
        /// </summary>
        public Matrix4f LensDistortionMatrix;

        /// <summary>
        /// How often, in seconds, the exposure is taken
        /// </summary>
        public double ExposurePeriodSeconds;

        /// <summary>
        /// length of the exposure time
        /// </summary>
        public double ExposureDurationSeconds;
    }
}