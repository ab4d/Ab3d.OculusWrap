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
    /// Specifies the coordinate system TrackingState returns tracking poses in.
    /// Used with ovr_SetTrackingOriginType()
    /// </summary>
    public enum TrackingOrigin
    {
        /// <summary>
        /// Tracking system origin reported at eye (HMD) height
        /// 
        /// Prefer using this origin when your application requires
        /// matching user's current physical head pose to a virtual head pose
        /// without any regards to a the height of the floor. Cockpit-based,
        /// or 3rd-person experiences are ideal candidates.
        /// 
        /// When used, all poses in TrackingState are reported as an offset
        /// transform from the profile calibrated or recentered HMD pose.
        /// It is recommended that apps using this origin type call ovr_RecenterTrackingOrigin
        /// prior to starting the VR experience, but notify the user before doing so
        /// to make sure the user is in a comfortable pose, facing a comfortable
        /// direction.
        /// </summary>
        EyeLevel = 0,

        /// <summary>
        /// Tracking system origin reported at floor height
        /// 
        /// Prefer using this origin when your application requires the
        /// physical floor height to match the virtual floor height, such as
        /// standing experiences.
        /// 
        /// When used, all poses in TrackingState are reported as an offset
        /// transform from the profile calibrated floor pose. Calling ovr_RecenterTrackingOrigin
        /// will recenter the X &amp; Z axes as well as yaw, but the Y-axis (i.e. height) will continue
        /// to be reported using the floor height as the origin for all poses.
        /// </summary>
        FloorLevel = 1,

        /// <summary>
        /// Count of enumerated elements.
        /// </summary>
        Count = 2,
    }
}