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
    /// This is a complete descriptor of the performance stats provided by the SDK
    /// </summary>	
    /// <seealso cref="OvrWrap.GetPerfStats"/>
    /// <seealso cref="PerfStatsPerCompositorFrame"/>
    public struct PerfStats
    {
        /// <summary>
        /// FrameStatsCount will have a maximum value set by ovrMaxProvidedFrameStats
        /// If the application calls ovr_GetPerfStats at the native refresh rate of the HMD
        /// then FrameStatsCount will be 1. If the app's workload happens to force
        /// ovr_GetPerfStats to be called at a lower rate, then FrameStatsCount will be 2 or more.
        /// If the app does not want to miss any performance data for any frame, it needs to
        /// ensure that it is calling ovr_SubmitFrame and ovr_GetPerfStats at a rate that is at least:
        /// "HMD_refresh_rate / ovrMaxProvidedFrameStats". On the Oculus Rift CV1 HMD, this will
        /// be equal to 18 times per second.
        ///
        /// The performance entries will be ordered in reverse chronological order such that the
        /// first entry will be the most recent one.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OvrWrap.MaxProvidedFrameStats)]
        public PerfStatsPerCompositorFrame[] FrameStats;

        /// <summary>
        /// FrameStatsCount
        /// </summary>
        public int FrameStatsCount;

        /// <summary>
        /// If the app calls ovr_GetPerfStats at less than 18 fps for CV1, then AnyFrameStatsDropped
        /// will be ovrTrue and FrameStatsCount will be equal to ovrMaxProvidedFrameStats.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] // Marshal byte to bool (0 = false, all other = true)
        public bool AnyFrameStatsDropped;

        /// <summary>
        /// AdaptiveGpuPerformanceScale is an edge-filtered value that a caller can use to adjust
        /// the graphics quality of the application to keep the GPU utilization in check. The value
        /// is calculated as: (desired_GPU_utilization / current_GPU_utilization)
        /// As such, when this value is 1.0, the GPU is doing the right amount of work for the app.
        /// Lower values mean the app needs to pull back on the GPU utilization.
        /// If the app is going to directly drive render-target resolution using this value, then
        /// be sure to take the square-root of the value before scaling the resolution with it.
        /// Changing render target resolutions however is one of the many things an app can do
        /// increase or decrease the amount of GPU utilization.
        /// Since AdaptiveGpuPerformanceScale is edge-filtered and does not change rapidly
        /// (i.e. reports non-1.0 values once every couple of seconds) the app can make the
        /// necessary adjustments and then keep watching the value to see if it has been satisfied.	
        /// </summary>
        public float AdaptiveGpuPerformanceScale;

        /// <summary>
        /// Will be true if Async Spacewarp (ASW) is available for this system which is dependent on
        /// several factors such as choice of GPU, OS and debug overrides
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] // Marshal byte to bool (0 = false, all other = true)
        public bool AswIsAvailable;

        /// <summary>
        /// Contains the Process ID of the VR application the stats are being polled for
        /// If an app continues to grab perf stats even when it is not visible, then expect this
        /// value to point to the other VR app that has grabbed focus (i.e. became visible)
        /// </summary>
        public int VisibleProcessId;

    }
}