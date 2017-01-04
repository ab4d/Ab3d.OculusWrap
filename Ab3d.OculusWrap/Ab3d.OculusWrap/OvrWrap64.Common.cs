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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Ab3d.OculusWrap
{
    // This file contains methods that are the same for OvrWrap32 and OvrWrap64 (but call different P/Invoke ovr methods defined in SafeNativeMethods)
    // If a method needs different marshaling or is otherwise different for 32-bit and 64-bit platform, 
    // then it should be defined in the OvrWrap32.Base.cs or OvrWrap64.Base.cs file.

    // NOTE:
    // This file is the same as OvrWrap32.Common.cs except that it is part of OvrWrap64 partial class.
    // This way maintenance of the library is much easier because all the changes need to be done only in one place
    // and then the file content can be easily copied to the OvrWrap32 class.

    public sealed partial class OvrWrap64
    {
        // The Oculus SDK is full of missing comments.
        // Ignore warnings regarding missing comments, in this class.
#pragma warning disable 1591

        #region Oculus SDK methods
        /// <summary>
        /// Detects Oculus Runtime and Device Status
        ///
        /// Checks for Oculus Runtime and Oculus HMD device status without loading the LibOVRRT
        /// shared library.  This may be called before ovr_Initialize() to help decide whether or
        /// not to initialize LibOVR.
        /// </summary>
        /// <param name="timeoutMilliseconds">Specifies a timeout to wait for HMD to be attached or 0 to poll.</param>
        /// <returns>Returns a DetectResult object indicating the result of detection.</returns>
        /// <see cref="DetectResult"/>
        public override DetectResult Detect(int timeoutMilliseconds)
        {
            return SafeNativeMethods.ovr_Detect(timeoutMilliseconds);
        }

        /// <summary>
        /// Initializes all Oculus functionality.
        /// </summary>
        /// <param name="parameters">
        /// Specifies custom initialization options. May be NULL to indicate default options when
        /// using the CAPI shim. If you are directly calling the LibOVRRT version of ovr_Initialize
        /// in the LibOVRRT DLL then this must be valid and include ovrInit_RequestVersion.
        /// </param>
        /// <remarks>
        /// Library init/shutdown, must be called around all other OVR code.
        /// No other functions calls besides ovr_InitializeRenderingShim are allowed
        /// before ovr_Initialize succeeds or after ovr_Shutdown.
        /// 
        /// LibOVRRT shared library search order:
        ///      1) Current working directory (often the same as the application directory).<br/>
        ///      2) Module directory (usually the same as the application directory, but not if the module is a separate shared library).<br/>
        ///      3) Application directory<br/>
        ///      4) Development directory (only if OVR_ENABLE_DEVELOPER_SEARCH is enabled, which is off by default).<br/>
        ///      5) Standard OS shared library search location(s) (OS-specific).<br/>
        /// </remarks>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use
        /// ovr_GetLastErrorInfo to get more information. Example failed results include:
        ///     - ovrError_Initialize: Generic initialization error.<br/>
        ///     - ovrError_LibLoad: Couldn't load LibOVRRT.<br/>
        ///     - ovrError_LibVersion: LibOVRRT version incompatibility.<br/>
        ///     - ovrError_ServiceConnection: Couldn't connect to the OVR Service.<br/>
        ///     - ovrError_ServiceVersion: OVR Service version incompatibility.<br/>
        ///     - ovrError_IncompatibleOS: The operating system version is incompatible.<br/>
        ///     - ovrError_DisplayInit: Unable to initialize the HMD display.<br/>
        ///     - ovrError_ServerStart:  Unable to start the server. Is it already running?<br/>
        ///     - ovrError_Reinitialization: Attempted to re-initialize with a different version.
        /// </returns>
		public override Result Initialize(InitParams parameters = null)
        {
            return SafeNativeMethods.ovr_Initialize(parameters);
        }

        /// <summary>
        /// Returns information about the most recent failed return value by the
        /// current thread for this library.
        /// 
        /// This function itself can never generate an error.
        /// The last error is never cleared by LibOVR, but will be overwritten by new errors.
        /// Do not use this call to determine if there was an error in the last API 
        /// call as successful API calls don't clear the last ErrorInfo.
        /// To avoid any inconsistency, ovr_GetLastErrorInfo should be called immediately
        /// after an API function that returned a failed ovrResult, with no other API
        /// functions called in the interim.
        /// </summary>
        /// <remarks>
        /// Allocate an ErrorInfo and pass this as errorInfo argument.
        /// </remarks>
        /// <returns>The last ErrorInfo for the current thread.</returns>
        public override ErrorInfo GetLastErrorInfo()
        {
            ErrorInfo errorInfo;
            SafeNativeMethods.ovr_GetLastErrorInfo(out errorInfo);

            return errorInfo;
        }

        /// <summary>
        /// Returns version string representing libOVR version. Static, so
        /// string remains valid for app lifespan
        /// </summary>
        /// <returns>
        /// Version string
        /// </returns>
        public override string GetVersionString()
        {
            IntPtr stringPtr = SafeNativeMethods.ovr_GetVersionString();
            string resultString = Marshal.PtrToStringAnsi(stringPtr);
            return resultString;
        }

        /// <summary>
        /// Send a message string to the system tracing mechanism if enabled (currently Event Tracing for Windows)
        /// </summary>
        /// <param name="level">
        /// One of the ovrLogLevel constants.
        /// </param>
        /// <param name="message">
        /// A UTF8-encoded null-terminated string.
        /// </param>
        /// <returns>
        /// Returns the length of the message, or -1 if message is too large
        /// </returns>
        public override int TraceMessage(LogLevel level, string message)
        {
            return SafeNativeMethods.ovr_TraceMessage((int)level, message);
        }

        /// <summary>
        /// Shuts down all Oculus functionality.
        /// </summary>
        /// <remarks>
        /// No API functions may be called after ovr_Shutdown except ovr_Initialize.
        /// </remarks>
        public override void Shutdown()
        {
            SafeNativeMethods.ovr_Shutdown();
        }

        /// <summary>
        /// Returns the number of sensors. 
        ///
        /// The number of sensors may change at any time, so this function should be called before use 
        /// as opposed to once on startup.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <returns>Returns unsigned int count.</returns>
        public override uint GetTrackerCount(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_GetTrackerCount(sessionPtr);
        }

        /// <summary>
        /// Returns a given sensor description.
        ///
        /// It's possible that sensor desc [0] may indicate a unconnnected or non-pose tracked sensor, but 
        /// sensor desc [1] may be connected.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="trackerDescIndex">
        /// Specifies a sensor index. The valid indexes are in the range of 0 to the sensor count returned by ovr_GetTrackerCount.
        /// </param>
        /// <returns>An empty ovrTrackerDesc will be returned if trackerDescIndex is out of range.</returns>
        /// <see cref="TrackerDesc"/>
        /// <see cref="GetTrackerCount"/>
        public override TrackerDesc GetTrackerDesc(IntPtr sessionPtr, uint trackerDescIndex)
        {
            return SafeNativeMethods.ovr_GetTrackerDesc(sessionPtr, trackerDescIndex);
        }

        /// <summary>
        /// Creates a handle to a VR session.
        /// 
        /// Upon success the returned IntPtr must be eventually freed with ovr_Destroy when it is no longer needed.
        /// A second call to ovr_Create will result in an error return value if the previous Hmd has not been destroyed.
        /// </summary>
        /// <param name="sessionPtr">
        /// Provides a pointer to an IntPtr which will be written to upon success.
        /// </param>
        /// <param name="pLuid">
        /// Provides a system specific graphics adapter identifier that locates which
        /// graphics adapter has the HMD attached. This must match the adapter used by the application
        /// or no rendering output will be possible. This is important for stability on multi-adapter systems. An
        /// application that simply chooses the default adapter will not run reliably on multi-adapter systems.
        /// </param>
        /// <remarks>
        /// Call Marshal.PtrToStructure(...) to convert the IntPtr to the OVR.ovrHmd type.
        /// </remarks>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. Upon failure
        /// the returned pHmd will be null.
        /// </returns>
        /// <example>
        /// <code>
        /// IntPtr sessionPtr
        /// ovrGraphicsLuid luid
        /// ovrResult result = ovr_Create(ref session, ref luid)
        /// if(OVR_FAILURE(result))
        /// ...
        /// </code>
        /// </example>
        /// <see cref="Destroy"/>
        public override Result Create(ref IntPtr sessionPtr, ref GraphicsLuid pLuid)
        {
            return SafeNativeMethods.ovr_Create(ref sessionPtr, ref pLuid);
        }

        /// <summary>
        /// Destroys the HMD.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        public override void Destroy(IntPtr sessionPtr)
        {
            SafeNativeMethods.ovr_Destroy(sessionPtr);
        }

        /// <summary>
        /// Returns status information for the application.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="sessionStatus">Provides a SessionStatus that is filled in.</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use ovr_GetLastErrorInfo 
        /// to get more information.
        /// Return values include but aren't limited to:
        /// - Result.Success: Completed successfully.
        /// - Result.ServiceConnection: The service connection was lost and the application must destroy the session.
        /// </returns>
        public override Result GetSessionStatus(IntPtr sessionPtr, ref SessionStatus sessionStatus)
        {
            return SafeNativeMethods.ovr_GetSessionStatus(sessionPtr, ref sessionStatus);
        }

        /// <summary>
        /// Sets the tracking origin type
        ///
        /// When the tracking origin is changed, all of the calls that either provide
        /// or accept ovrPosef will use the new tracking origin provided.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="origin">Specifies an ovrTrackingOrigin to be used for all ovrPosef</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. 
        /// In the case of failure, use ovr_GetLastErrorInfo to get more information.
        /// </returns>
        /// <see cref="TrackingOrigin"/>
        /// <see cref="GetTrackingOriginType"/>
        public override Result SetTrackingOriginType(IntPtr sessionPtr, TrackingOrigin origin)
        {
            return SafeNativeMethods.ovr_SetTrackingOriginType(sessionPtr, origin);
        }

        /// <summary>
        /// Gets the tracking origin state
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
        /// <see cref="TrackingOrigin"/>
        /// <see cref="SetTrackingOriginType"/>
        public override TrackingOrigin GetTrackingOriginType(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_GetTrackingOriginType(sessionPtr);
        }

        /// <summary>
        /// Re-centers the sensor position and orientation.
        ///
        /// This resets the (x,y,z) positional components and the yaw orientation component.
        /// The Roll and pitch orientation components are always determined by gravity and cannot
        /// be redefined. All future tracking will report values relative to this new reference position.
        /// If you are using ovrTrackerPoses then you will need to call ovr_GetTrackerPose after 
        /// this, because the sensor position(s) will change as a result of this.
        /// 
        /// The headset cannot be facing vertically upward or downward but rather must be roughly
        /// level otherwise this function will fail with ovrError_InvalidHeadsetOrientation.
        ///
        /// For more info, see the notes on each ovrTrackingOrigin enumeration to understand how
        /// recenter will vary slightly in its behavior based on the current ovrTrackingOrigin setting.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use
        /// ovr_GetLastErrorInfo to get more information. Return values include but aren't limited to:
        /// - Result.Success: Completed successfully.
        /// - Result.InvalidHeadsetOrientation: The headset was facing an invalid direction when attempting recentering, 
        ///   such as facing vertically.
        /// </returns>
        /// <see cref="TrackingOrigin"/>
        /// <see cref="GetTrackerPose"/>
        public override Result RecenterTrackingOrigin(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_RecenterTrackingOrigin(sessionPtr);
        }

        /// <summary>
        /// Clears the ShouldRecenter status bit in IntPtrStatus.
        ///
        /// Clears the ShouldRecenter status bit in IntPtrStatus, allowing further recenter 
        /// requests to be detected. Since this is automatically done by ovr_RecenterTrackingOrigin,
        /// this is only needs to be called when application is doing its own re-centering.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        public override void ClearShouldRecenterFlag(IntPtr sessionPtr)
        {
            SafeNativeMethods.ovr_ClearShouldRecenterFlag(sessionPtr);
        }

        /// <summary>
        /// Returns the ovrTrackerPose for the given sensor.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
        /// <returns>
        /// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
        /// </returns>
        /// <see cref="GetTrackerCount"/>
        public override TrackerPose GetTrackerPose(IntPtr sessionPtr, uint trackerPoseIndex)
        {
            return SafeNativeMethods.ovr_GetTrackerPose(sessionPtr, trackerPoseIndex);
        }

        /// <summary>
        /// Returns the most recent input state for controllers, without positional tracking info.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="controllerType">Specifies which controller the input will be returned for.</param>
        /// <param name="inputState">Input state that will be filled in.</param>
        /// <returns>Returns Result.Success if the new state was successfully obtained.</returns>
        /// <see cref="ControllerType"/>
        public override Result GetInputState(IntPtr sessionPtr, ControllerType controllerType, ref InputState inputState)
        {
            return SafeNativeMethods.ovr_GetInputState(sessionPtr, controllerType, ref inputState);
        }

        /// <summary>
        /// Returns controller types connected to the system OR'ed together.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <returns>A bitmask of ControllerTypes connected to the system.</returns>
        /// <see cref="ControllerType"/>
        public override ControllerType GetConnectedControllerTypes(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_GetConnectedControllerTypes(sessionPtr);
        }

        /// <summary>
        /// Returns tracking state reading based on the specified absolute system time.
        ///
        /// Pass an absTime value of 0.0 to request the most recent sensor reading. In this case
        /// both PredictedPose and SamplePose will have the same value.
        ///
        /// This may also be used for more refined timing of front buffer rendering logic, and so on.
        /// This may be called by multiple threads.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="absTime">
        /// Specifies the absolute future time to predict the return
        /// TrackingState value. Use 0 to request the most recent tracking state.
        /// </param>
        /// <param name="latencyMarker">
        /// Specifies that this call is the point in time where
        /// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
        /// provides "SensorSampleTimestamp", that will override the value stored here.
        /// </param>
        /// <returns>Returns the TrackingState that is predicted for the given absTime.</returns>
        /// <see cref="TrackingState"/>
        /// <see cref="GetEyePoses"/>
        /// <see cref="GetTimeInSeconds"/>
        public override TrackingState GetTrackingState(IntPtr sessionPtr, double absTime, bool latencyMarker)
        {
            TrackingState trackingState;
            SafeNativeMethods.ovr_GetTrackingState(out trackingState, sessionPtr, absTime, latencyMarker ? (byte)1 : (byte)0);

            return trackingState;
        }

        /// <summary>
        /// Turns on vibration of the given controller.
        ///
        /// To disable vibration, call ovr_SetControllerVibration with an amplitude of 0.
        /// Vibration automatically stops after a nominal amount of time, so if you want vibration 
        /// to be continuous over multiple seconds then you need to call this function periodically.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="controllerType">Specifies controllers to apply the vibration to.</param>
        /// <param name="frequency">
        /// Specifies a vibration frequency in the range of 0.0 to 1.0. 
        /// Currently the only valid values are 0.0, 0.5, and 1.0 and other values will
        /// be clamped to one of these.
        /// </param>
        /// <param name="amplitude">Specifies a vibration amplitude in the range of 0.0 to 1.0.</param>
        /// <returns>Returns ovrSuccess upon success.</returns>
        /// <see cref="ControllerType"/>
        public override Result SetControllerVibration(IntPtr sessionPtr, ControllerType controllerType, float frequency, float amplitude)
        {
            return SafeNativeMethods.ovr_SetControllerVibration(sessionPtr, controllerType, frequency, amplitude);
        }

        // SDK Distortion Rendering
        //
        // All of rendering functions including the configure and frame functions
        // are not thread safe. It is OK to use ConfigureRendering on one thread and handle
        // frames on another thread, but explicit synchronization must be done since
        // functions that depend on configured state are not reentrant.
        //
        // These functions support rendering of distortion by the SDK.

        // IntPtr creation is rendering API-specific.
        // ovr_CreateTextureSwapChainDX and ovr_CreateTextureSwapChainGL can be found in the
        // rendering API-specific headers, such as OVR_CAPI_D3D.h and OVR_CAPI_GL.h


        /// <summary>
        /// Gets the number of buffers in an IntPtr.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies the IntPtr for which the length should be retrieved.</param>
        /// <param name="length">Returns the number of buffers in the specified chain.</param>
        /// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
        /// <see cref="CreateTextureSwapChainDX"/>
        /// <see cref="CreateTextureSwapChainGL"/>
        public override Result GetTextureSwapChainLength(IntPtr sessionPtr, IntPtr chain, out int length)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainLength(sessionPtr, chain, out length);
        }

        /// <summary>
        /// Gets the current index in an IntPtr.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies the IntPtr for which the index should be retrieved.</param>
        /// <param name="index">Returns the current (free) index in specified chain.</param>
        /// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
        /// <see cref="CreateTextureSwapChainDX"/>
        /// <see cref="CreateTextureSwapChainGL"/>
        public override Result GetTextureSwapChainCurrentIndex(IntPtr sessionPtr, IntPtr chain, out int index)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainCurrentIndex(sessionPtr, chain, out index);
        }

        /// <summary>
        /// Gets the description of the buffers in an IntPtr
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies the IntPtr for which the description should be retrieved.</param>
        /// <param name="desc">Returns the description of the specified chain.</param>
        /// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
        /// <see cref="CreateTextureSwapChainDX"/>
        /// <see cref="CreateTextureSwapChainGL"/>
        public override Result GetTextureSwapChainDesc(IntPtr sessionPtr, IntPtr chain, ref TextureSwapChainDesc desc)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainDesc(sessionPtr, chain, ref desc);
        }

        /// <summary>
        /// Commits any pending changes to an IntPtr, and advances its current index
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies the IntPtr to commit.</param>
        /// <returns>
        /// Returns an ovrResult for which the return code is negative upon error.
        /// Failures include but aren't limited to:
        ///   - Result.TextureSwapChainFull: ovr_CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
        /// </returns>
        /// <see cref="CreateTextureSwapChainDX"/>
        /// <see cref="CreateTextureSwapChainGL"/>
        public override Result CommitTextureSwapChain(IntPtr sessionPtr, IntPtr chain)
        {
            return SafeNativeMethods.ovr_CommitTextureSwapChain(sessionPtr, chain);
        }

        /// <summary>
        /// Destroys an IntPtr and frees all the resources associated with it.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies the IntPtr to destroy. If it is null then this function has no effect.</param>
        public override void DestroyTextureSwapChain(IntPtr sessionPtr, IntPtr chain)
        {
            SafeNativeMethods.ovr_DestroyTextureSwapChain(sessionPtr, chain);
        }

        // MirrorTexture creation is rendering API-specific.

        /// <summary>
        /// Destroys a mirror texture previously created by one of the mirror texture creation functions.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="mirrorTexturePtr">
        /// Specifies the ovrTexture to destroy. If it is null then this function has no effect.
        /// </param>
        /// <see cref="CreateMirrorTextureDX"/>
        /// <see cref="CreateMirrorTextureGL"/>
        public override void DestroyMirrorTexture(IntPtr sessionPtr, IntPtr mirrorTexturePtr)
        {
            SafeNativeMethods.ovr_DestroyMirrorTexture(sessionPtr, mirrorTexturePtr);
        }

        /// <summary>
        /// Calculates the recommended viewport size for rendering a given eye within the HMD
        /// with a given FOV cone. 
        /// 
        /// Higher FOV will generally require larger textures to maintain quality.
        /// Apps packing multiple eye views together on the same texture should ensure there are
        /// at least 8 pixels of padding between them to prevent texture filtering and chromatic
        /// aberration causing images to leak between the two eye views.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="eye">
        /// Specifies which eye (left or right) to calculate for.
        /// </param>
        /// <param name="fov">
        /// Specifies the ovrFovPort to use.
        /// </param>
        /// <param name="pixelsPerDisplayPixel">
        /// pixelsPerDisplayPixel Specifies the ratio of the number of render target pixels 
        /// to display pixels at the center of distortion. 1.0 is the default value. Lower
        /// values can improve performance, higher values give improved quality.
        /// </param>
        /// <returns>
        /// Returns the texture width and height size.
        /// </returns>
        public override Sizei GetFovTextureSize(IntPtr sessionPtr, EyeType eye, FovPort fov, float pixelsPerDisplayPixel)
        {
            return SafeNativeMethods.ovr_GetFovTextureSize(sessionPtr, eye, fov, pixelsPerDisplayPixel);
        }

        /// <summary>
        /// Computes the distortion viewport, view adjust, and other rendering parameters for
        /// the specified eye.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="eyeType">
        /// Specifies which eye (left or right) for which to perform calculations.
        /// </param>
        /// <param name="fov">
        /// Specifies the FovPort to use.
        /// </param>
        /// <returns>
        /// Returns the computed EyeRenderDesc for the given eyeType and field of view.
        /// </returns>
        /// <see cref="EyeRenderDesc"/>
        public override EyeRenderDesc GetRenderDesc(IntPtr sessionPtr, EyeType eyeType, FovPort fov)
        {
            return SafeNativeMethods.ovr_GetRenderDesc(sessionPtr, eyeType, fov);
        }

        /// <summary>
        /// Submits layers for distortion and display.
        /// 
        /// ovr_SubmitFrame triggers distortion and processing which might happen asynchronously. 
        /// The function will return when there is room in the submission queue and surfaces
        /// are available. Distortion might or might not have completed.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="frameIndex">
        /// Specifies the targeted application frame index, or 0 to refer to one frame 
        /// after the last time ovr_SubmitFrame was called.
        /// </param>
        /// <param name="viewScaleDesc">
        /// Provides additional information needed only if layerPtrList contains
        /// an ovrLayerType_Quad. If null, a default version is used based on the current configuration and a 1.0 world scale.
        /// </param>
        /// <param name="layerPtrList">
        /// Specifies a list of ovrLayer pointers, which can include null entries to
        /// indicate that any previously shown layer at that index is to not be displayed.
        /// Each layer header must be a part of a layer structure such as ovrLayerEyeFov or ovrLayerQuad,
        /// with Header.Type identifying its type. A null layerPtrList entry in the array indicates the 
        /// absence of the given layer.
        /// </param>
        /// <param name="layerCount">
        /// Indicates the number of valid elements in layerPtrList. The maximum supported layerCount 
        /// is not currently specified, but may be specified in a future version.
        /// </param>
        /// <returns>
        /// Returns an ovrResult for which the return code is negative upon error and positive
        /// upon success. Return values include but aren't limited to:
        ///     - Result.Success: rendering completed successfully.
        ///     - Result.NotVisible: rendering completed successfully but was not displayed on the HMD,
        ///       usually because another application currently has ownership of the HMD. Applications receiving
        ///       this result should stop rendering new content, but continue to call ovr_SubmitFrame periodically
        ///       until it returns a value other than ovrSuccess_NotVisible.
        ///     - Result.DisplayLost: The session has become invalid (such as due to a device removal)
        ///       and the shared resources need to be released (ovr_DestroyTextureSwapChain), the session needs to
        ///       destroyed (ovr_Destroy) and recreated (ovr_Create), and new resources need to be created
        ///       (ovr_CreateTextureSwapChainXXX). The application's existing private graphics resources do not
        ///       need to be recreated unless the new ovr_Create call returns a different GraphicsLuid.
        ///     - Result.TextureSwapChainInvalid: The IntPtr is in an incomplete or inconsistent state. 
        ///       Ensure ovr_CommitTextureSwapChain was called at least once first.
        /// </returns>
        /// <remarks>
        /// layerPtrList must contain an array of pointers. 
        /// Each pointer must point to an object, which starts with a an LayerHeader property.
        /// </remarks>
        /// <see cref="GetPredictedDisplayTime"/>
        /// <see cref="ViewScaleDesc"/>
        /// <see cref="LayerHeader"/>
        public override Result SubmitFrame(IntPtr sessionPtr, Int64 frameIndex, IntPtr viewScaleDesc, IntPtr layerPtrList, int layerCount)
        {
            return SafeNativeMethods.ovr_SubmitFrame(sessionPtr, frameIndex, viewScaleDesc, layerPtrList, layerCount);
        }

        // Gets the ovrFrameTiming for the given frame index.
        //
        // The application should increment frameIndex for each successively targeted frame,
        // and pass that index to any relevent OVR functions that need to apply to the frame
        // identified by that index.
        //
        // This function is thread-safe and allows for multiple application threads to target
        // their processing to the same displayed frame.

        /// <summary>
        /// Gets the time of the specified frame midpoint.
        ///
        /// Predicts the time at which the given frame will be displayed. The predicted time 
        /// is the middle of the time period during which the corresponding eye images will 
        /// be displayed. 
        /// 
        /// The application should increment frameIndex for each successively targeted frame,
        /// and pass that index to any relevent OVR functions that need to apply to the frame
        /// identified by that index. 
        /// 
        /// This function is thread-safe and allows for multiple application threads to target 
        /// their processing to the same displayed frame.
        /// 
        /// In the even that prediction fails due to various reasons (e.g. the display being off
        /// or app has yet to present any frames), the return value will be current CPU time.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="frameIndex">
        /// Identifies the frame the caller wishes to target.
        /// A value of zero returns the next frame index.
        /// </param>
        /// <returns>
        /// Returns the absolute frame midpoint time for the given frameIndex.
        /// </returns>
        /// <see cref="GetTimeInSeconds"/>
        public override double GetPredictedDisplayTime(IntPtr sessionPtr, Int64 frameIndex)
        {
            return SafeNativeMethods.ovr_GetPredictedDisplayTime(sessionPtr, frameIndex);
        }

        /// <summary>
        /// Returns global, absolute high-resolution time in seconds. 
        ///
        /// The time frame of reference for this function is not specified and should not be
        /// depended upon.
        /// </summary>
        /// <returns>
        /// Returns seconds as a floating point value.
        /// </returns>
        /// <see cref="PoseStatef"/>
        public override double GetTimeInSeconds()
        {
            return SafeNativeMethods.ovr_GetTimeInSeconds();
        }

        /// <summary>
        /// Reads a boolean property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="defaultValue">
        /// Specifes the value to return if the property couldn't be read.
        /// </param>
        /// <returns>
        /// Returns the property interpreted as a boolean value. 
        /// Returns defaultValue if the property doesn't exist.
        /// </returns>
        public override bool GetBool(IntPtr sessionPtr, string propertyName, bool defaultValue)
        {
            byte result = SafeNativeMethods.ovr_GetBool(sessionPtr, propertyName, defaultValue ? (byte)1 : (byte)0);

            return result != 0;
        }

        /// <summary>
        /// Writes or creates a boolean property.
        /// If the property wasn't previously a boolean property, it is changed to a boolean property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        /// <returns>
        /// Returns true if successful, otherwise false. 
        /// A false result should only occur if the property name is empty or if the property is read-only.
        /// </returns>
        public override bool SetBool(IntPtr sessionPtr, string propertyName, bool value)
        {
            byte result = SafeNativeMethods.ovr_SetBool(sessionPtr, propertyName, value ? (byte)1 : (byte)0);
            return result != 0;
        }

        /// <summary>
        /// Reads an integer property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="defaultValue">
        /// Specifes the value to return if the property couldn't be read.
        /// </param>
        /// <returns>
        /// Returns the property interpreted as an integer value. 
        /// Returns defaultValue if the property doesn't exist.
        /// </returns>
        public override int GetInt(IntPtr sessionPtr, string propertyName, int defaultValue)
        {
            return SafeNativeMethods.ovr_GetInt(sessionPtr, propertyName, defaultValue);
        }

        /// <summary>
        /// Writes or creates an integer property.
        /// 
        /// If the property wasn't previously an integer property, it is changed to an integer property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        /// <returns>
        /// Returns true if successful, otherwise false. 
        /// A false result should only occur if the property name is empty or if the property is read-only.
        /// </returns>
        public override bool SetInt(IntPtr sessionPtr, string propertyName, int value)
        {
            byte result = SafeNativeMethods.ovr_SetInt(sessionPtr, propertyName, value);
            return result != 0;
        }

        /// <summary>
        /// Reads a float property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="defaultValue">
        /// Specifes the value to return if the property couldn't be read.
        /// </param>
        /// <returns>
        /// Returns the property interpreted as an float value. 
        /// Returns defaultValue if the property doesn't exist.
        /// </returns>
        public override float GetFloat(IntPtr sessionPtr, string propertyName, float defaultValue)
        {
            return SafeNativeMethods.ovr_GetFloat(sessionPtr, propertyName, defaultValue);
        }

        /// <summary>
        /// Writes or creates a float property.
        /// 
        /// If the property wasn't previously a float property, it's changed to a float property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        /// <returns>
        /// Returns true if successful, otherwise false. 
        /// A false result should only occur if the property name is empty or if the property is read-only.
        /// </returns>
        public override bool SetFloat(IntPtr sessionPtr, string propertyName, float value)
        {
            byte result = SafeNativeMethods.ovr_SetFloat(sessionPtr, propertyName, value);
            return result != 0;
        }

        /// <summary>
        /// Reads a float array property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="values">
        /// An array of float to write to.
        /// </param>
        /// <param name="valuesCapacity">
        /// Specifies the maximum number of elements to write to the values array.
        /// </param>
        /// <returns>
        /// Returns the number of elements read, or 0 if property doesn't exist or is empty.
        /// </returns>
        public override int GetFloatArray(IntPtr sessionPtr, string propertyName, float[] values, int valuesCapacity)
        {
            return SafeNativeMethods.ovr_GetFloatArray(sessionPtr, propertyName, values, valuesCapacity);
        }

        /// <summary>
        /// Writes or creates a float array property.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="values">
        /// An array of float to write from.
        /// </param>
        /// <param name="valuesSize">
        /// Specifies the number of elements to write.
        /// </param>
        /// <returns>
        /// Returns true if successful, otherwise false. 
        /// A false result should only occur if the property name is empty or if the property is read-only.
        /// </returns>
        public override bool SetFloatArray(IntPtr sessionPtr, string propertyName, float[] values, int valuesSize)
        {
            byte result = SafeNativeMethods.ovr_SetFloatArray(sessionPtr, propertyName, values, valuesSize);
            return result != 0;
        }

        /// <summary>
        /// Reads a string property.
        /// Strings are UTF8-encoded and null-terminated.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="defaultValue">
        /// Specifes the value to return if the property couldn't be read.
        /// </param>
        /// <returns>
        /// Returns the string property if it exists. 
        /// 
        /// Otherwise returns defaultValue, which can be specified as null.
        /// The return memory is guaranteed to be valid until next call to ovr_GetString or 
        /// until the HMD is destroyed, whichever occurs first.
        /// </returns>
        public override string GetString(IntPtr sessionPtr, string propertyName, string defaultValue)
        {
            IntPtr resultPtr = SafeNativeMethods.ovr_GetString(sessionPtr, propertyName, defaultValue);
            string result = Marshal.PtrToStringAnsi(resultPtr);

            return result;
        }

        /// <summary>
        /// Writes or creates a string property.
        /// Strings are UTF8-encoded and null-terminated.
        /// </summary>
        /// <param name="sessionPtr">
        /// Specifies an IntPtr previously returned by ovr_Create.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property, which needs to be valid for only the call.
        /// </param>
        /// <param name="value">
        /// The string property, which only needs to be valid for the duration of the call.
        /// </param>
        /// <returns>
        /// Returns true if successful, otherwise false. 
        /// 
        /// A false result should only occur if the property name is empty or if the property is read-only.
        /// </returns>
        public override bool SetString(IntPtr sessionPtr, string propertyName, string value)
        {
            byte result = SafeNativeMethods.ovr_SetString(sessionPtr, propertyName, value);
            return result != 0;
        }

        /// <summary>
        /// Used to generate projection from ovrEyeDesc::Fov.
        /// </summary>
        /// <param name="fov">
        /// Specifies the ovrFovPort to use.
        /// </param>
        /// <param name="zNear">
        /// Distance to near Z limit.
        /// </param>
        /// <param name="zFar">
        /// Distance to far Z limit.
        /// </param>
        /// <param name="projectionModFlags">
        /// A combination of the ProjectionModifier flags.
        /// </param>
        /// <returns>
        /// Returns the calculated projection matrix.
        /// </returns>
        /// <see cref="ProjectionModifier"/>
        public override Matrix4f Matrix4f_Projection(FovPort fov, float zNear, float zFar, ProjectionModifier projectionModFlags)
        {
            return SafeNativeMethods.ovrMatrix4f_Projection(fov, zNear, zFar, projectionModFlags);
        }

        /// <summary>
        /// Extracts the required data from the result of ovrMatrix4f_Projection.
        /// </summary>
        /// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
        /// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
        /// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
        /// <see cref="TimewarpProjectionDesc"/>
        public override TimewarpProjectionDesc TimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags)
        {
            return SafeNativeMethods.ovrTimewarpProjectionDesc_FromProjection(projection, projectionModFlags);
        }

        /// <summary>
        /// Generates an orthographic sub-projection.
        ///
        /// Used for 2D rendering, Y is down.
        /// </summary>
        /// <param name="projection">
        /// The perspective matrix that the orthographic matrix is derived from.
        /// </param>
        /// <param name="orthoScale">
        /// Equal to 1.0f / pixelsPerTanAngleAtCenter.
        /// </param>
        /// <param name="orthoDistance">
        /// Equal to the distance from the camera in meters, such as 0.8m.
        /// </param>
        /// <param name="hmdToEyeOffsetX">
        /// Specifies the offset of the eye from the center.
        /// </param>
        /// <returns>
        /// Returns the calculated projection matrix.
        /// </returns>
        public override Matrix4f Matrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX)
        {
            return SafeNativeMethods.ovrMatrix4f_OrthoSubProjection(projection, orthoScale, orthoDistance, hmdToEyeOffsetX);
        }

        /// <summary>
        /// Computes offset eye poses based on headPose returned by TrackingState.
        /// </summary>
        /// <param name="headPose">
        /// Indicates the HMD position and orientation to use for the calculation.
        /// </param>
        /// <param name="hmdToEyeOffset">
        /// Can EyeRenderDesc.HmdToEyeOffset returned from 
        /// ovr_GetRenderDesc. For monoscopic rendering, use a vector that is the average 
        /// of the two vectors for both eyes.
        /// </param>
        /// <param name="eyePoses">
        /// If outEyePoses are used for rendering, they should be passed to 
        /// ovr_SubmitFrame in LayerEyeFov.RenderPose or LayerEyeFovDepth.RenderPose.
        /// </param>   
        public override void CalcEyePoses(Posef headPose, Vector3f[] hmdToEyeOffset, ref Posef[] eyePoses)
        {
            if (eyePoses == null)
                throw new ArgumentNullException("eyePoses");

            if (eyePoses.Length != 2)
                throw new ArgumentException("eyePoses array should have 2 Posef items", "eyePoses");


            GCHandle posesHandle = GCHandle.Alloc(eyePoses, GCHandleType.Pinned);
            try
            {
                SafeNativeMethods.ovr_CalcEyePoses(headPose, hmdToEyeOffset, posesHandle.AddrOfPinnedObject());
            }
            finally
            {
                posesHandle.Free();
            }
        }

        /// <summary>
        /// Returns the predicted head pose in HmdTrackingState and offset eye poses in outEyePoses. 
        /// 
        /// This is a thread-safe function where caller should increment frameIndex with every frame
        /// and pass that index where applicable to functions called on the rendering thread.
        /// Assuming outEyePoses are used for rendering, it should be passed as a part of ovrLayerEyeFov.
        /// The caller does not need to worry about applying HmdToEyeOffset to the returned outEyePoses variables.
        /// </summary>
        /// <param name="sessionPtr">IntPtr previously returned by ovr_Create.</param>
        /// <param name="frameIndex">
        /// Specifies the targeted frame index, or 0 to refer to one frame after 
        /// the last time ovr_SubmitFrame was called.
        /// </param>
        /// <param name="latencyMarker">
        /// Specifies that this call is the point in time where
        /// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
        /// provides "SensorSampleTimestamp", that will override the value stored here.
        /// </param>
        /// <param name="hmdToEyeOffset">
        /// Can be EyeRenderDesc.HmdToEyeOffset returned from ovr_GetRenderDesc. 
        /// For monoscopic rendering, use a vector that is the average of the two vectors for both eyes.
        /// </param>
        /// <param name="eyePoses">
        /// The predicted eye poses.
        /// </param>
        /// <param name="sensorSampleTime">
        /// The time when this function was called. 
        /// May be null, in which case it is ignored.
        /// </param>
        public override void GetEyePoses(IntPtr sessionPtr, long frameIndex, bool latencyMarker, Vector3f[] hmdToEyeOffset, ref Posef[] eyePoses, out double sensorSampleTime)
        {
            if (eyePoses == null)
                throw new ArgumentNullException("eyePoses");

            if (eyePoses.Length != 2)
                throw new ArgumentException("eyePoses array should have 2 Posef items", "eyePoses");


            GCHandle posesHandle = GCHandle.Alloc(eyePoses, GCHandleType.Pinned);
            try
            {
                sensorSampleTime = 0;
                SafeNativeMethods.ovr_GetEyePoses(sessionPtr, frameIndex, latencyMarker ? (byte)1 : (byte)0, hmdToEyeOffset, posesHandle.AddrOfPinnedObject(), ref sensorSampleTime);
            }
            finally
            {
                posesHandle.Free();
            }
        }

        /// <summary>
        /// Tracking poses provided by the SDK come in a right-handed coordinate system. If an application
        /// is passing in ovrProjection_LeftHanded into ovrMatrix4f_Projection, then it should also use
        /// this function to flip the HMD tracking poses to be left-handed.
        ///
        /// While this utility function is intended to convert a left-handed ovrPosef into a right-handed
        /// coordinate system, it will also work for converting right-handed to left-handed since the
        /// flip operation is the same for both cases.
        /// </summary>
        /// <param name="inPose">inPose that is right-handed</param>
        /// <param name="outPose">outPose that is requested to be left-handed (can be the same pointer to inPose)</param>
        public override void Posef_FlipHandedness(ref Posef inPose, ref Posef outPose)
        {
            SafeNativeMethods.ovrPosef_FlipHandedness(ref inPose, ref outPose);
        }

        /// <summary>
        /// Create Texture Swap Chain suitable for use with Direct3D 11 and 12.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="d3dDevicePtr">
        /// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue 
        /// which must be the same one the application renders to the eye textures with.</param>
        /// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
        /// <param name="textureSwapChainPtr">
        /// Returns the created IntPtr, which will be valid upon a successful return value, else it will be null.
        /// This texture chain must be eventually destroyed via ovr_DestroyTextureSwapChain before destroying the HMD with ovr_Destroy.
        /// </param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        /// <remarks>
        /// The texture format provided in desc should be thought of as the format the distortion-compositor will use for the
        /// ShaderResourceView when reading the contents of the texture. To that end, it is highly recommended that the application
        /// requests texture swapchain formats that are in sRGB-space (e.g. OVR_FORMAT_R8G8B8A8_UNORM_SRGB) as the compositor
        /// does sRGB-correct rendering. As such, the compositor relies on the GPU's hardware sampler to do the sRGB-to-linear
        /// conversion. If the application still prefers to render to a linear format (e.g. OVR_FORMAT_R8G8B8A8_UNORM) while handling the
        /// linear-to-gamma conversion via HLSL code, then the application must still request the corresponding sRGB format and also use
        /// the ovrTextureMisc_DX_Typeless flag in the IntPtrDesc's Flag field. This will allow the application to create
        /// a RenderTargetView that is the desired linear format while the compositor continues to treat it as sRGB. Failure to do so
        /// will cause the compositor to apply unexpected gamma conversions leading to gamma-curve artifacts. The ovrTextureMisc_DX_Typeless
        /// flag for depth buffer formats (e.g. OVR_FORMAT_D32_FLOAT) is ignored as they are always converted to be typeless.
        /// </remarks>
        /// <see cref="GetTextureSwapChainLength"/>
        /// <see cref="GetTextureSwapChainCurrentIndex"/>
        /// <see cref="GetTextureSwapChainDesc"/>
        /// <see cref="GetTextureSwapChainBufferDX"/>
        /// <see cref="DestroyTextureSwapChain"/>
        public override Result CreateTextureSwapChainDX(IntPtr sessionPtr, IntPtr d3dDevicePtr, ref TextureSwapChainDesc desc, out IntPtr textureSwapChainPtr)
        {
            textureSwapChainPtr = IntPtr.Zero;
            return SafeNativeMethods.ovr_CreateTextureSwapChainDX(sessionPtr, d3dDevicePtr, ref desc, ref textureSwapChainPtr);
        }

        /// <summary>
        /// Get a specific buffer within the chain as any compatible COM interface (similar to QueryInterface)
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies an IntPtr previously returned by ovr_CreateTextureSwapChainDX</param>
        /// <param name="index">
        /// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength),
        /// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex).
        /// </param>
        /// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
        /// <param name="buffer">Returns the COM interface pointer retrieved.</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        public override Result GetTextureSwapChainBufferDX(IntPtr sessionPtr, IntPtr chain, int index, Guid iid, out IntPtr buffer)
        {
            buffer = IntPtr.Zero;
            return SafeNativeMethods.ovr_GetTextureSwapChainBufferDX(sessionPtr, chain, index, iid, ref buffer);
        }

        /// <summary>
        /// Create Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
        ///
        /// A second call to ovr_CreateMirrorTextureDX for a given IntPtr before destroying the first one
        /// is not supported and will result in an error return.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="d3dDevicePtr">
        /// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue
        /// which must be the same one the application renders to the textures with.
        /// </param>
        /// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
        /// <param name="mirrorTexturePtr">
        /// Returns the created IntPtr, which will be valid upon a successful return value, else it will be null.
        /// This texture must be eventually destroyed via ovr_DestroyMirrorTexture before destroying the HMD with ovr_Destroy.
        /// </param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        /// <remarks>
        /// The texture format provided in desc should be thought of as the format the compositor will use for the RenderTargetView when
        /// writing into mirror texture. To that end, it is highly recommended that the application requests a mirror texture format that is
        /// in sRGB-space (e.g. OVR.TextureFormat.R8G8B8A8_UNORM_SRGB) as the compositor does sRGB-correct rendering. If however the application wants
        /// to still read the mirror texture as a linear format (e.g. OVR.TextureFormat.OVR_FORMAT_R8G8B8A8_UNORM) and handle the sRGB-to-linear conversion in
        /// HLSL code, then it is recommended the application still requests an sRGB format and also use the ovrTextureMisc_DX_Typeless flag in the
        /// IntPtrDesc's Flags field. This will allow the application to bind a ShaderResourceView that is a linear format while the
        /// compositor continues to treat is as sRGB. Failure to do so will cause the compositor to apply unexpected gamma conversions leading to 
        /// gamma-curve artifacts.
        /// </remarks>
        /// <see cref="GetMirrorTextureBufferDX"/>
        /// <see cref="DestroyMirrorTexture"/>
        public override Result CreateMirrorTextureDX(IntPtr sessionPtr, IntPtr d3dDevicePtr, ref MirrorTextureDesc desc, out IntPtr mirrorTexturePtr)
        {
            mirrorTexturePtr = IntPtr.Zero;
            return SafeNativeMethods.ovr_CreateMirrorTextureDX(sessionPtr, d3dDevicePtr, ref desc, ref mirrorTexturePtr);
        }

        /// <summary>
        /// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="mirrorTexturePtr">Specifies an IntPtr previously returned by ovr_CreateMirrorTextureDX</param>
        /// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
        /// <param name="buffer">Returns the COM interface pointer retrieved.</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        public override Result GetMirrorTextureBufferDX(IntPtr sessionPtr, IntPtr mirrorTexturePtr, Guid iid, out IntPtr buffer)
        {
            buffer = IntPtr.Zero;
            return SafeNativeMethods.ovr_GetMirrorTextureBufferDX(sessionPtr, mirrorTexturePtr, iid, ref buffer);
        }

        /// <summary>
        /// Creates a TextureSwapChain suitable for use with OpenGL.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="desc">Specifies the requested texture properties. See notes for more info about texture format.</param>
        /// <param name="textureSwapChainPtr">
        /// Returns the created IntPtr, which will be valid upon
        /// a successful return value, else it will be null. This texture swap chain must be eventually
        /// destroyed via ovr_DestroyTextureSwapChain before destroying the HMD with ovr_Destroy.
        /// </param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        /// <remarks>
        /// The format provided should be thought of as the format the distortion compositor will use when reading
        /// the contents of the texture. To that end, it is highly recommended that the application requests texture swap chain
        /// formats that are in sRGB-space (e.g. Format.R8G8B8A8_UNORM_SRGB) as the distortion compositor does sRGB-correct
        /// rendering. Furthermore, the app should then make sure "glEnable(GL_FRAMEBUFFER_SRGB)" is called before rendering
        /// into these textures. Even though it is not recommended, if the application would like to treat the texture as a linear
        /// format and do linear-to-gamma conversion in GLSL, then the application can avoid calling "glEnable(GL_FRAMEBUFFER_SRGB)",
        /// but should still pass in an sRGB variant for the format. Failure to do so will cause the distortion compositor
        /// to apply incorrect gamma conversions leading to gamma-curve artifacts.		
        /// </remarks>
        /// <see cref="GetTextureSwapChainLength"/>
        /// <see cref="GetTextureSwapChainCurrentIndex"/>
        /// <see cref="GetTextureSwapChainDesc"/>
        /// <see cref="GetTextureSwapChainBufferGL"/>
        /// <see cref="DestroyTextureSwapChain"/>
        public override Result CreateTextureSwapChainGL(IntPtr sessionPtr, TextureSwapChainDesc desc, out IntPtr textureSwapChainPtr)
        {
            return SafeNativeMethods.ovr_CreateTextureSwapChainGL(sessionPtr, desc, out textureSwapChainPtr);
        }

        /// <summary>
        /// Get a specific buffer within the chain as a GL texture name
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="chain">Specifies an IntPtr previously returned by ovr_CreateTextureSwapChainGL</param>
        /// <param name="index">
        /// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength)
        /// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
        /// </param>
        /// <param name="textureId">Returns the GL texture object name associated with the specific index requested</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        public override Result GetTextureSwapChainBufferGL(IntPtr sessionPtr, IntPtr chain, int index, out uint textureId)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainBufferGL(sessionPtr, chain, index, out textureId);
        }

        /// <summary>
        /// Creates a Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
        ///
        /// A second call to ovr_CreateMirrorTextureGL for a given IntPtr before destroying the first one
        /// is not supported and will result in an error return.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="desc">Specifies the requested mirror texture description.</param>
        /// <param name="mirrorTexturePtr">
        /// Specifies the created IntPtr, which will be valid upon a successful return value, else it will be null.
        /// This texture must be eventually destroyed via ovr_DestroyMirrorTexture before destroying the HMD with ovr_Destroy.
        /// </param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        /// <remarks>
        /// The format provided should be thought of as the format the distortion compositor will use when writing into the mirror
        /// texture. It is highly recommended that mirror textures are requested as sRGB formats because the distortion compositor
        /// does sRGB-correct rendering. If the application requests a non-sRGB format (e.g. R8G8B8A8_UNORM) as the mirror texture,
        /// then the application might have to apply a manual linear-to-gamma conversion when reading from the mirror texture.
        /// Failure to do so can result in incorrect gamma conversions leading to gamma-curve artifacts and color banding.
        /// </remarks>
        /// <see cref="GetMirrorTextureBufferGL"/>
        /// <see cref="DestroyMirrorTexture"/>
        public override Result CreateMirrorTextureGL(IntPtr sessionPtr, MirrorTextureDesc desc, out IntPtr mirrorTexturePtr)
        {
            return SafeNativeMethods.ovr_CreateMirrorTextureGL(sessionPtr, desc, out mirrorTexturePtr);
        }

        /// <summary>
        /// Get a the underlying buffer as a GL texture name
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="mirrorTexturePtr">Specifies an IntPtr previously returned by ovr_CreateMirrorTextureGL</param>
        /// <param name="textureId">Specifies the GL texture object name associated with the mirror texture</param>
        /// <returns>
        /// Returns an ovrResult indicating success or failure. In the case of failure, use 
        /// ovr_GetLastErrorInfo to get more information.
        /// </returns>
        public override Result GetMirrorTextureBufferGL(IntPtr sessionPtr, IntPtr mirrorTexturePtr, out uint textureId)
        {
            return SafeNativeMethods.ovr_GetMirrorTextureBufferGL(sessionPtr, mirrorTexturePtr, out textureId);
        }

        /// <summary>
        /// Identify client application info.
        /// </summary>
        /// <remarks>
        /// The string is one or more newline-delimited lines of optional info
        /// indicating engine name, engine version, engine plugin name, engine plugin
        /// version, engine editor. The order of the lines is not relevant. Individual
        /// lines are optional. A newline is not necessary at the end of the last line.
        /// Call after ovr_Initialize and before the first call to ovr_Create.
        /// Each value is limited to 20 characters. Key names such as 'EngineName:'
        /// 'EngineVersion:' do not count towards this limit.
        /// </remarks>
        /// <param name="identity">
        /// Specifies one or more newline-delimited lines of optional info:<br/>
        ///             EngineName: %s\n<br/>
        ///             EngineVersion: %s\n<br/>
        ///             EnginePluginName: %s\n<br/>
        ///             EnginePluginVersion: %s\n<br/>
        ///             EngineEditor: ('true' or 'false')\n
        /// </param>
        public override Result IdentifyClient(string identity)
        {
            return SafeNativeMethods.ovr_IdentifyClient(identity);
        }

        /// <summary>
        /// Gets information about Haptics engine for the specified Touch controller.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="controllerType">The controller to retrieve the information from.</param>
        /// <returns>
        /// Returns an TouchHapticsDesc.
        /// </returns>
        public override TouchHapticsDesc GetTouchHapticsDesc(IntPtr sessionPtr, ControllerType controllerType)
        {
            return SafeNativeMethods.ovr_GetTouchHapticsDesc(sessionPtr, controllerType);
        }

        /// <summary>
        /// Submits a Haptics buffer (used for vibration) to Touch (only) controllers.
        /// Note: ovr_SubmitControllerVibration cannot be used interchangeably with ovr_SetControllerVibration.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="controllerType">The controller to retrieve the information from.</param>
        /// <param name="buffer">Haptics buffer containing amplitude samples to be played.</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///          upon success. Return values include but aren't limited to:
        ///      - ovrSuccess: The call succeeded and a result was returned.
        ///      - ovrSuccess_DeviceUnavailable: The call succeeded but the device referred to by controllerType is not available.
        /// </returns> 
        /// <seealso cref="HapticsBuffer"/>       
        public override Result SubmitControllerVibration(IntPtr sessionPtr, ControllerType controllerType, HapticsBuffer buffer)
        {
            return SafeNativeMethods.ovr_SubmitControllerVibration(sessionPtr, controllerType, buffer);
        }
        /// <summary>
        /// Gets the Haptics engine playback state of a specific Touch controller.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="controllerType">The controller to retrieve the information from.</param>
        /// <param name="state">state of the haptics engine.</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///          upon success. Return values include but aren't limited to:
        ///      - ovrSuccess: The call succeeded and a result was returned.
        ///      - ovrSuccess_DeviceUnavailable: The call succeeded but the device referred to by controllerType is not available.
        ///  </returns>   
        /// <seealso cref="HapticsPlaybackState"/> 
        public override Result GetControllerVibrationState(IntPtr sessionPtr, ControllerType controllerType, ref HapticsPlaybackState state)
        {
            return SafeNativeMethods.ovr_GetControllerVibrationState(sessionPtr, controllerType, ref state);
        }
        /// <summary>
        /// Tests collision/proximity of position tracked devices (e.g. HMD and/or Touch) against the Boundary System.
        /// Note: this method is similar to ovr_BoundaryTestPoint but can be more precise as it may take into account device acceleration/momentum.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="deviceBitmask">Bitmask of one or more tracked devices to test.</param>
        /// <param name="boundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea.</param>
        /// <param name="testResult">Result of collision/proximity test, contains information such as distance and closest point.</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success. Return values include but aren't limited to:
        ///     - ovrSuccess: The call succeeded and a result was returned.
        ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
        ///     - ovrSuccess_DeviceUnavailable: The call succeeded but the device referred to by deviceBitmask is not available.
        /// </returns>   
        /// <seealso cref="BoundaryTestResult"/> 
        public override Result TestBoundary(IntPtr sessionPtr, TrackedDeviceType deviceBitmask, BoundaryType boundaryType, ref BoundaryTestResult testResult)
        {
            return SafeNativeMethods.ovr_TestBoundary(sessionPtr, deviceBitmask, boundaryType, ref testResult);
        }
        /// <summary>
        /// Tests collision/proximity of a 3D point against the Boundary System.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="point">3D point to test.</param>
        /// <param name="singleBoundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea to test against</param>
        /// <param name="testResult">Result of collision/proximity test, contains information such as distance and closest point.</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success. Return values include but aren't limited to:
        ///     - ovrSuccess: The call succeeded and a result was returned.
        ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
        /// </returns>   
        /// <seealso cref="BoundaryTestResult"/>         
        public override Result TestBoundaryPoint(IntPtr sessionPtr, Vector3f point, BoundaryType singleBoundaryType, ref BoundaryTestResult testResult)
        {
            return SafeNativeMethods.ovr_TestBoundaryPoint(sessionPtr, ref point, singleBoundaryType, ref testResult);
        }
        /// <summary>
        /// Sets the look and feel of the Boundary System.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="lookAndFeel">Look and feel parameters.</param>
        /// <returns>
        /// Returns ovrSuccess upon success.
        /// </returns>   
        /// <seealso cref="BoundaryLookAndFeel"/>   
        public override Result SetBoundaryLookAndFeel(IntPtr sessionPtr, ref BoundaryLookAndFeel lookAndFeel)
        {
            return SafeNativeMethods.ovr_SetBoundaryLookAndFeel(sessionPtr, ref lookAndFeel);
        }
        /// <summary>
        /// Resets the look and feel of the Boundary System to its default state.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <returns>
        /// Returns ovrSuccess upon success.
        /// </returns>
        /// <seealso cref="BoundaryLookAndFeel"/>     
        public override Result ResetBoundaryLookAndFeel(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_ResetBoundaryLookAndFeel(sessionPtr);
        }

        /// <summary>
        /// Gets the geometry of the Boundary System's "play area" or "outer boundary" as 3D floor points.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="boundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea.</param>
        /// <param name="floorPoints">Array of 3D points (in clockwise order) defining the boundary at floor height</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success. Return values include but aren't limited to:
        ///     - ovrSuccess: The call succeeded and a result was returned.
        ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
        /// </returns>   
        public override Result GetBoundaryGeometry(IntPtr sessionPtr, BoundaryType boundaryType, out Vector3f[] floorPoints)
        {
            int out_FloorPointsCount = 0;

            // In first call we get number of points back (called with floorPointPtr == Zero)
            var result = SafeNativeMethods.ovr_GetBoundaryGeometry(sessionPtr, boundaryType, IntPtr.Zero, ref out_FloorPointsCount);

            if (result != Result.Success || out_FloorPointsCount == 0)
            {
                floorPoints = null;
                return result;
            }

            floorPoints = new Vector3f[out_FloorPointsCount];

            GCHandle gcHandle = GCHandle.Alloc(floorPoints, GCHandleType.Pinned);

            try
            {
                result = SafeNativeMethods.ovr_GetBoundaryGeometry(sessionPtr, boundaryType, gcHandle.AddrOfPinnedObject(), ref out_FloorPointsCount);
            }
            finally
            {
                gcHandle.Free();
            }

            return result;
        }

        /// <summary>
        /// Gets the dimension of the Boundary System's "play area" or "outer boundary".
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="boundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea.</param>
        /// <param name="dimensions">Dimensions of the axis aligned bounding box that encloses the area in meters (width, height and length).</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success. Return values include but aren't limited to:
        ///     - ovrSuccess: The call succeeded and a result was returned.
        ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
        /// </returns>   
        public override Result GetBoundaryDimensions(IntPtr sessionPtr, BoundaryType boundaryType, out Vector3f dimensions)
        {
            var dimensionValues = new float[3];
            var result = SafeNativeMethods.ovr_GetBoundaryDimensions(sessionPtr, boundaryType, dimensionValues);

            dimensions = new Vector3f(dimensionValues[0], dimensionValues[1], dimensionValues[2]);

            return result;
        }
        /// <summary>
        /// Returns if the boundary is currently visible.
        /// Note: visibility is false if the user has turned off boundaries, otherwise, it's true if the app has requested 
        /// boundaries to be visible or if any tracked device is currently triggering it. This may not exactly match rendering 
        /// due to fade-in and fade-out effects.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="isVisible">ovrTrue, if the boundary is visible.</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success. Return values include but aren't limited to:
        ///     - ovrSuccess: Result was successful and a result was returned.
        ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
        /// </returns>   
        public override Result GetBoundaryVisible(IntPtr sessionPtr, out bool isVisible)
        {
            Byte ovrIsVisible = 0;
            var result = SafeNativeMethods.ovr_GetBoundaryVisible(sessionPtr, ref ovrIsVisible);

            isVisible = (ovrIsVisible != 0);

            return result;
        }
        /// <summary>
        /// Requests boundary to be visible.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="visible">forces the outer boundary to be visible. An application can't force it to be invisible, but can cancel its request by passing false.</param>
        /// <returns>
        /// Returns ovrSuccess upon success.
        /// </returns>   
        public override Result RequestBoundaryVisible(IntPtr sessionPtr, bool visible)
        {
            return SafeNativeMethods.ovr_RequestBoundaryVisible(sessionPtr, visible ? (Byte)1 : (Byte)0);
        }

        ///  <summary>
        ///  Retrieves performance stats for the VR app as well as the SDK compositor.
        /// 
        ///  If the app calling this function is not the one in focus (i.e. not visible in the HMD), then
        ///  stats will be zero'd out.
        ///  New stats are populated after each successive call to ovr_SubmitFrame. So the app should call
        ///  this function on the same thread it calls ovr_SubmitFrame, preferably immediately
        ///  afterwards.
        ///  </summary>
        ///  <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <param name="stats">Contains the performance stats for the application and SDK compositor</param>
        /// <returns>
        ///  Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success.
        ///  </returns> 
        /// <seealso cref="PerfStats"/>  
        /// <seealso cref="PerfStatsPerCompositorFrame"/>  
        /// <seealso cref="ResetPerfStats"/>  
        public override Result GetPerfStats(IntPtr sessionPtr, ref PerfStats stats)
        {
            return SafeNativeMethods.ovr_GetPerfStats(sessionPtr, ref stats);
        }

        /// <summary>
        /// Resets the accumulated stats reported in each ovrPerfStatsPerCompositorFrame back to zero.
        ///
        /// Only the integer values such as HmdVsyncIndex, AppDroppedFrameCount etc. will be reset
        /// as the other fields such as AppMotionToPhotonLatency are independent timing values updated
        /// per-frame.
        /// </summary>
        /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
        /// <returns>
        /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
        ///         upon success.
        /// </returns>   
        /// <seealso cref="PerfStats"/>  
        /// <seealso cref="PerfStatsPerCompositorFrame"/>  
        /// <seealso cref="GetPerfStats"/>          
        public override Result ResetPerfStats(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_ResetPerfStats(sessionPtr);
        }
        #endregion        
    }
}