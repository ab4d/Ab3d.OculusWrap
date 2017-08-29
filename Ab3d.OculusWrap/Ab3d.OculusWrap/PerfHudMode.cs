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
    /// <summary>
    /// Performance HUD enables the HMD user to see information critical to
    /// the real-time operation of the VR application such as latency timing,
    /// and CPU &amp; GPU performance metrics
    /// </summary>
    /// <example>
    /// App can toggle performance HUD modes as such:
    /// 
    /// PerfHudMode perfHudMode = PerfHudMode.Hud_LatencyTiming;
    /// ovr_SetInt(Hmd, "PerfHudMode", (int) perfHudMode);
    /// </example>
    public enum PerfHudMode
    {
        /// <summary>
        /// Shows performance summary and headroom
        /// </summary>
        PerfSummary        = 1,

        /// <summary>
        /// Shows latency related timing info
        /// </summary>
        LatencyTiming      = 2,

        /// <summary>
        /// Shows render timing info for application
        /// </summary>
        AppRenderTiming    = 3,

        /// <summary>
        /// Shows render timing info for OVR compositor
        /// </summary>
        CompRenderTiming   = 4,

        /// <summary>
        /// Shows SDK &amp; HMD version Info
        /// </summary>
        VersionInfo        = 5,

        /// <summary>
        /// Shows Async Spacewarp-specific info
        /// </summary>
        ovrPerfHud_AswStats = 6,
    }
}