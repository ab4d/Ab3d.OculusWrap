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
    /// Debug HUD is provided to help developers gauge and debug the fidelity of their app's
    /// stereo rendering characteristics. Using the provided quad and crosshair guides, 
    /// the developer can verify various aspects such as VR tracking units (e.g. meters),
    /// stereo camera-parallax properties (e.g. making sure objects at infinity are rendered
    /// with the proper separation), measuring VR geometry sizes and distances and more.
    ///
    /// The app can modify the visual properties of the stereo guide (i.e. quad, crosshair)
    /// using the ovr_SetFloatArray function. For a list of tweakable properties,
    /// see the OVR_DEBUG_HUD_STEREO_GUIDE_* keys in the OVR_CAPI_Keys.h header file.
    /// </summary>
    /// <example>
    /// ovrDebugHudStereoMode DebugHudMode = ovrDebugHudStereo.QuadWithCrosshair;
    /// ovr_SetInt(Hmd, OVR_DEBUG_HUD_STEREO_MODE, (int)DebugHudMode);
    /// </example>
    public enum DebugHudStereoMode
    {
        /// <summary>
        /// Turns off the Stereo Debug HUD
        /// </summary>
        Off                 = 0,

        /// <summary>
        /// Renders Quad in world for Stereo Debugging
        /// </summary>
        Quad                = 1,

        /// <summary>
        /// Renders Quad+crosshair in world for Stereo Debugging
        /// </summary>
        QuadWithCrosshair   = 2,

        /// <summary>
        /// Renders screen-space crosshair at infinity for Stereo Debugging
        /// </summary>
        CrosshairAtInfinity = 3,
    }
}