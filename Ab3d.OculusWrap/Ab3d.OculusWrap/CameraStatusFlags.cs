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
    public enum CameraStatusFlags
    {
        /// <summary>
        /// Initial state of camera
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Bit set when the camera is connected to the system
        /// </summary>
        Connected = 0x1,

        /// <summary>
        /// Bit set when the camera is undergoing calibration
        /// </summary>
        Calibrating = 0x2,

        /// <summary>
        /// Bit set when the camera has tried and failed calibration
        /// </summary>
        CalibrationFailed = 0x4,

        /// <summary>
        /// Bit set when the camera has tried and passed calibration
        /// </summary>
        Calibrated = 0x8,
    }
}