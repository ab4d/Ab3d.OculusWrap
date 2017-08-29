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
    public struct CameraExtrinsics
    {
        /// <summary>
        /// Time in seconds from last change to the parameters.
        /// For instance, if the pose changes, or a camera exposure happens, this struct will be updated.
        /// </summary>
        public double LastChangedTimeSeconds;

        /// <summary>
        /// Current Status of the camera, a mix of bits from ovrCameraStatusFlags
        /// </summary>
        public uint CameraStatusFlags;

        /// <summary>
        /// Which Tracked device, if any, is the camera rigidly attached to
        /// If set to ovrTrackedDevice_None, then the camera is not attached to a tracked object.
        /// If the external camera moves while unattached (i.e. set to ovrTrackedDevice_None), its Pose
        /// won't be updated
        /// </summary>
        public TrackedDeviceType AttachedToDevice;

        /// <summary>
        /// The relative Pose of the External Camera.
        /// If AttachedToDevice is ovrTrackedDevice_None, then this is a absolute pose in tracking space
        /// </summary>
        public Posef RelativePose;

        /// <summary>
        /// The time, in seconds, when the last successful exposure was taken
        /// </summary>
        public double LastExposureTimeSeconds;

        /// <summary>
        /// Estimated exposure latency to get from the exposure time to the system
        /// </summary>
        public double ExposureLatencySeconds;

        /// <summary>
        /// Additional latency to get from the exposure time of the real camera to match the render time
        /// of the virtual camera
        /// </summary>
        public double AdditionalLatencySeconds;
    }
}