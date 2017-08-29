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
    /// Contains the performance stats for a given SDK compositor frame
    /// All of the int fields can be reset via the ovr_ResetPerfStats call.
    /// </summary>	
    [StructLayout(LayoutKind.Sequential)]
    public struct PerfStatsPerCompositorFrame
    {
        /// <summary>	
        /// Vsync Frame Index - increments with each HMD vertical synchronization signal (i.e. vsync or refresh rate)
        /// If the compositor drops a frame, expect this value to increment more than 1 at a time.
        /// </summary>	
        public int HmdVsyncIndex;

        /// <summary>	
        /// Index that increments with each successive ovr_SubmitFrame call	
        /// </summary>	
        public int AppFrameIndex;

        /// <summary>	
        /// If the app fails to call ovr_SubmitFrame on time, then expect this value to increment with each missed frame
        /// </summary>	
        public int AppDroppedFrameCount;

        /// <summary>	
        /// Motion-to-photon latency for the application
        /// This value is calculated by either using the SensorSampleTime provided for the ovrLayerEyeFov or if that
        /// is not available, then the call to ovr_GetTrackingState which has latencyMarker set to ovrTrue
        /// </summary>	
        public float AppMotionToPhotonLatency;

        /// <summary>	
        /// Amount of queue-ahead in seconds provided to the app based on performance and overlap of CPU and GPU utilization
        /// A value of 0.0 would mean the CPU and GPU workload is being completed in 1 frame's worth of time, while
        /// 11 ms (on the CV1) of queue ahead would indicate that the app's CPU workload for the next frame is
        /// overlapping the app's GPU workload for the current frame.
        /// </summary>	
        public float AppQueueAheadTime;

        /// <summary>	
        /// Amount of time in seconds spent on the CPU by the app's render-thread that calls ovr_SubmitFrame
        /// Measured as elapsed time between from when app regains control from ovr_SubmitFrame to the next time the app
        /// calls ovr_SubmitFrame.
        /// </summary>	
        public float AppCpuElapsedTime;

        /// <summary>	
        /// Amount of time in seconds spent on the GPU by the app
        /// Measured as elapsed time between each ovr_SubmitFrame call using GPU timing queries.	
        /// </summary>	
        public float AppGpuElapsedTime;

        /// <summary>	
        /// Index that increments each time the SDK compositor completes a distortion and timewarp pass
        /// Since the compositor operates asynchronously, even if the app calls ovr_SubmitFrame too late,
        /// the compositor will kick off for each vsync.	
        /// </summary>	
        public int CompositorFrameIndex;

        /// <summary>	
        /// Increments each time the SDK compositor fails to complete in time
        /// This is not tied to the app's performance, but failure to complete can be tied to other factors
        /// such as OS capabilities, overall available hardware cycles to execute the compositor in time
        /// and other factors outside of the app's control.	
        /// </summary>	
        public int CompositorDroppedFrameCount;

        /// <summary>	
        /// Motion-to-photon latency of the SDK compositor in seconds
        /// This is the latency of timewarp which corrects the higher app latency as well as dropped app frames.	
        /// </summary>	
        public float CompositorLatency;

        /// <summary>	
        /// The amount of time in seconds spent on the CPU by the SDK compositor. Unless the VR app is utilizing
        /// all of the CPU cores at their peak performance, there is a good chance the compositor CPU times
        /// will not affect the app's CPU performance in a major way.	
        /// </summary>	
        public float CompositorCpuElapsedTime;

        /// <summary>	
        /// The amount of time in seconds spent on the GPU by the SDK compositor. Any time spent on the compositor
        /// will eat away from the available GPU time for the app.
        /// </summary>	
        public float CompositorGpuElapsedTime;

        /// <summary>	
        /// The amount of time in seconds spent from the point the CPU kicks off the compositor to the point in time
        /// the compositor completes the distortion and timewarp on the GPU. In the event the GPU time is not
        /// available, expect this value to be -1.0f	
        /// </summary>	
        public float CompositorCpuStartToGpuEndElapsedTime;

        /// <summary>	
        /// The amount of time in seconds left after the compositor is done on the GPU to the associated V-Sync time.
        /// In the event the GPU time is not available, expect this value to be -1.0f
        /// </summary>	
        public float CompositorGpuEndToVsyncElapsedTime;


        ///
        /// Async Spacewarp stats (ASW)
        ///

        /// <summary>	
        /// Will be true if ASW is active for the given frame such that the application is being forced
        /// into half the frame-rate while the compositor continues to run at full frame-rate.
        /// </summary>	
        [MarshalAs(UnmanagedType.U1)]
        public bool AswIsActive;

        /// <summary>	
        /// Increments each time ASW it activated where the app was forced in and out of
        /// half-rate rendering.
        /// </summary>	
        public int AswActivatedToggleCount;

        /// <summary>	
        /// Accumulates the number of frames presented by the compositor which had extrapolated
        /// ASW frames presented.
        /// </summary>	
        public int AswPresentedFrameCount;

        /// <summary>	
        /// Accumulates the number of frames that the compositor tried to present when ASW is
        /// active but failed.
        /// </summary>	
        public int AswFailedFrameCount;
    }
}