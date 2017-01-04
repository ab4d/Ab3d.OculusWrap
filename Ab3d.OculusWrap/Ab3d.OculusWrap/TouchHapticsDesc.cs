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
    /// Describes the Touch Haptics engine.
    /// Currently, those values will NOT change during a session.
    /// </summary>	
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TouchHapticsDesc
    {
        /// <summary>
        /// Haptics engine frequency/sample-rate, sample time in seconds equals 1.0/sampleRateHz
        /// </summary>
        public int SampleRateHz;

        /// <summary>
        /// Size of each Haptics sample, sample value range is [0, 2^(Bytes*8)-1]
        /// </summary>
        public int SampleSizeInBytes;

        /// <summary>
        /// Queue size that would guarantee Haptics engine would not starve for data.
        /// Make sure size doesn't drop below it for best results.
        /// </summary>
        public int QueueMinSizeToAvoidStarvation;

        /// <summary>
        /// Minimum number of samples that can be sent to Haptics through ovr_SubmitControllerVibration
        /// </summary>
        public int SubmitMinSamples;

        /// <summary>
        /// Maximum number of samples that can be sent to Haptics through ovr_SubmitControllerVibration
        /// </summary>
        public int SubmitMaxSamples;

        /// <summary>
        /// Optimal number of samples that can be sent to Haptics through ovr_SubmitControllerVibration
        /// </summary>
        public int SubmitOptimalSamples;
    }
}