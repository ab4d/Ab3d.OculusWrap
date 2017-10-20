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
using System.Runtime.InteropServices;
using System.Security;

namespace Ab3d.OculusWrap
{
    // NOTE:
    // This file is the same as OvrWrap64.Interop.cs except that it is part of OvrWrap32 partial class.
    // This way maintenance of the library is much easier because all the changes need to be done only in one place
    // and then the file content can be easily copied to the OvrWrap64 class.
    //
    // The reason why there need to be two files is that the _ovrDllName that is used in the DllImport must be constant.

    public sealed partial class OvrWrap32
    {
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_Detect", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern DetectResult ovr_Detect(int timeoutMilliseconds);

            /// <summary>
            /// Initializes all Oculus functionality.
            /// </summary>
            /// <param name="parameters">
            /// Initialize with extra parameters.
            /// Pass 0 to initialize with default parameters, suitable for released games.
            /// </param>
            /// <remarks>
            /// Library init/shutdown, must be called around all other OVR code.
            /// No other functions calls besides ovr_InitializeRenderingShim are allowed
            /// before ovr_Initialize succeeds or after ovr_Shutdown.
            /// 
            /// LibOVRRT shared library search order:
            ///      1) Current working directory (often the same as the application directory).
            ///      2) Module directory (usually the same as the application directory, but not if the module is a separate shared library).
            ///      3) Application directory
            ///      4) Development directory (only if OVR_ENABLE_DEVELOPER_SEARCH is enabled, which is off by default).
            ///      5) Standard OS shared library search location(s) (OS-specific).
            /// </remarks>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_Initialize", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_Initialize(InitParams parameters = null);

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
            /// <param name="errorInfo">The last ErrorInfo for the current thread.</param>
            /// <remarks>
            /// Allocate an ErrorInfo and pass this as errorInfo argument.
            /// </remarks>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetLastErrorInfo", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_GetLastErrorInfo(out ErrorInfo errorInfo);

            /// <summary>
            /// Returns version string representing libOVR version. Static, so
            /// string remains valid for app lifespan
            /// </summary>
            /// <remarks>
            /// Use Marshal.PtrToStringAnsi() to retrieve version string.
            /// </remarks>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetVersionString", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr ovr_GetVersionString();

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_TraceMessage", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ovr_TraceMessage(int level, string message);

            /// <summary>
            /// Shuts down all Oculus functionality.
            /// </summary>
            /// <remarks>
            /// No API functions may be called after ovr_Shutdown except ovr_Initialize.
            /// </remarks>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_Shutdown", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_Shutdown();

            /// <summary>
            /// Returns information about the current HMD.
            /// 
            /// ovr_Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
            /// will be reported as None.
            /// 
            /// Please note: This method will should only be called by a 32 bit process. 
            /// </summary>
            /// <param name="sessionPtr">
            /// Specifies an IntPtr previously returned by ovr_Create, else NULL in which
            /// case this function detects whether an HMD is present and returns its info if so.
            /// </param>
            /// <param name="result">
            /// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is ovr_None then
            /// no HMD is present.
            /// </param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetHmdDesc", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_GetHmdDesc32(out HmdDesc result, IntPtr sessionPtr);

            /// <summary>
            /// Returns information about the current HMD.
            /// 
            /// ovr_Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
            /// will be reported as None.
            /// 
            /// Please note: This method will should only be called by a 64 bit process. 
            /// </summary>
            /// <param name="sessionPtr">
            /// Specifies an IntPtr previously returned by ovr_Create, else NULL in which
            /// case this function detects whether an HMD is present and returns its info if so.
            /// </param>
            /// <param name="result">
            /// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is ovr_None then
            /// no HMD is present.
            /// </param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetHmdDesc", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_GetHmdDesc64(out HmdDesc64 result, IntPtr sessionPtr);

            /// <summary>
            /// Returns the number of sensors. 
            ///
            /// The number of sensors may change at any time, so this function should be called before use 
            /// as opposed to once on startup.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <returns>Returns unsigned int count.</returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTrackerCount", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ovr_GetTrackerCount(IntPtr sessionPtr);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTrackerDesc", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern TrackerDesc ovr_GetTrackerDesc(IntPtr sessionPtr, uint trackerDescIndex);

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
            /// IntPtr sessionPtr;
            /// ovrGraphicsLuid luid;
            /// ovrResult result = ovr_Create(ref session, ref luid);
            /// if(OVR_FAILURE(result))
            /// ...
            /// </code>
            /// </example>
            /// <see cref="Destroy"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_Create", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_Create(ref IntPtr sessionPtr, ref GraphicsLuid pLuid);

            /// <summary>
            /// Destroys the HMD.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_Destroy", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_Destroy(IntPtr sessionPtr);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetSessionStatus", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetSessionStatus(IntPtr sessionPtr, ref SessionStatus sessionStatus);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetTrackingOriginType", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SetTrackingOriginType(IntPtr sessionPtr, TrackingOrigin origin);

            /// <summary>
            /// Gets the tracking origin state
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
            /// <see cref="TrackingOrigin"/>
            /// <see cref="SetTrackingOriginType"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTrackingOriginType", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern TrackingOrigin ovr_GetTrackingOriginType(IntPtr sessionPtr);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_RecenterTrackingOrigin", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_RecenterTrackingOrigin(IntPtr sessionPtr);

            /// <summary>
            /// Allows manually tweaking the sensor position and orientation.
            ///
            /// This function is similar to ovr_RecenterTrackingOrigin in that it modifies the
            /// (x,y,z) positional components and the yaw orientation component of the tracking space for
            /// the HMD and controllers.
            ///
            /// While ovr_RecenterTrackingOrigin resets the tracking origin in reference to the HMD's
            /// current pose, ovr_SpecifyTrackingOrigin allows the caller to explicitly specify a transform
            /// for the tracking origin. This transform is expected to be an offset to the most recent
            /// recentered origin, so calling this function repeatedly with the same originPose will keep
            /// nudging the recentered origin in that direction.
            ///
            /// There are several use cases for this function. For example, if the application decides to
            /// limit the yaw, or translation of the recentered pose instead of directly using the HMD pose
            /// the application can query the current tracking state via ovr_GetTrackingState, and apply
            /// some limitations to the HMD pose because feeding this pose back into this function.
            /// Similarly, this can be used to "adjust the seating position" incrementally in apps that
            /// feature seated experiences such as cockpit-based games.
            ///
            /// This function can emulate ovr_RecenterTrackingOrigin as such:
            ///     ovrTrackingState ts = ovr_GetTrackingState(session, 0.0, ovrFalse);
            ///     ovr_SpecifyTrackingOrigin(session, ts.HeadPose.ThePose);
            ///
            /// The roll and pitch orientation components are determined by gravity and cannot be redefined.
            /// If you are using ovrTrackerPoses then you will need to call ovr_GetTrackerPose after
            /// this, because the sensor position(s) will change as a result of this.
            ///
            /// For more info, see the notes on each ovrTrackingOrigin enumeration to understand how
            /// recenter will vary slightly in its behavior based on the current ovrTrackingOrigin setting.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="originPose">originPose Specifies a pose that will be used to transform the current tracking origin.</param>
            /// <returns>
            /// Returns an ovrResult indicating success or failure. In the case of failure, use
            /// ovr_GetLastErrorInfo to get more information. Return values include but aren't limited to:
            /// - ovrSuccess: Completed successfully.
            /// - ovrError_InvalidParameter: The heading direction in originPose was invalid,
            /// such as facing vertically. This can happen if the caller is directly feeding the pose
            /// of a position-tracked device such as an HMD or controller into this function.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SpecifyTrackingOrigin", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SpecifyTrackingOrigin(IntPtr sessionPtr, Posef originPose);

            /// <summary>
            /// Clears the ShouldRecenter status bit in IntPtrStatus.
            ///
            /// Clears the ShouldRecenter status bit in IntPtrStatus, allowing further recenter 
            /// requests to be detected. Since this is automatically done by ovr_RecenterTrackingOrigin,
            /// this is only needs to be called when application is doing its own re-centering.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_ClearShouldRecenterFlag", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_ClearShouldRecenterFlag(IntPtr sessionPtr);

            /// <summary>
            /// Returns the ovrTrackerPose for the given sensor.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
            /// <returns>
            /// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
            /// </returns>
            /// <see cref="GetTrackerCount"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTrackerPose", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern TrackerPose ovr_GetTrackerPose(IntPtr sessionPtr, uint trackerPoseIndex);

            /// <summary>
            /// Returns the most recent input state for controllers, without positional tracking info.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="controllerType">Specifies which controller the input will be returned for.</param>
            /// <param name="inputState">Input state that will be filled in.</param>
            /// <returns>Returns Result.Success if the new state was successfully obtained.</returns>
            /// <see cref="ControllerType"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetInputState", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetInputState(IntPtr sessionPtr, ControllerType controllerType, ref InputState inputState);

            /// <summary>
            /// Returns controller types connected to the system OR'ed together.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <returns>A bitmask of ControllerTypes connected to the system.</returns>
            /// <see cref="ControllerType"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetConnectedControllerTypes", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern ControllerType ovr_GetConnectedControllerTypes(IntPtr sessionPtr);

            /// <summary>
            /// Returns tracking state reading based on the specified absolute system time.
            ///
            /// Pass an absTime value of 0.0 to request the most recent sensor reading. In this case
            /// both PredictedPose and SamplePose will have the same value.
            ///
            /// This may also be used for more refined timing of front buffer rendering logic, and so on.
            /// This may be called by multiple threads.
            /// </summary>
            /// <param name="result">Returns the TrackingState that is predicted for the given absTime.</param>
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
            /// <see cref="TrackingState"/>
            /// <see cref="GetEyePoses"/>
            /// <see cref="GetTimeInSeconds"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTrackingState", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_GetTrackingState(out TrackingState result, IntPtr sessionPtr, double absTime, Byte latencyMarker);

            /// Returns an array of poses, where each pose matches a device type provided by the deviceTypes
            /// array parameter.
            /// <param name="sessionPtr">Specifies an ovrSession previously returned by ovr_Create.</param>
            /// <param name="deviceTypes">Array of device types to query for their poses.</param>
            /// <param name="deviceCount">deviceCount Number of queried poses. This number must match the length of the outDevicePoses and deviceTypes array.</param>
            /// <param name="absTime">Specifies the absolute future time to predict the return ovrTrackingState value. Use 0 to request the most recent tracking state.</param>
            /// <param name="outDevicePoses">Array of poses, one for each device type in deviceTypes arrays (size must match the size of deviceTypes array).</param>
            /// <returns>Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true upon success.</returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetDevicePoses", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetDevicePoses(IntPtr sessionPtr, [MarshalAs(UnmanagedType.LPArray)] TrackedDeviceType[] deviceTypes, int deviceCount, double absTime, [MarshalAs(UnmanagedType.LPArray), In, Out] PoseStatef[] outDevicePoses);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetControllerVibration", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SetControllerVibration(IntPtr sessionPtr, ControllerType controllerType, float frequency, float amplitude);

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
            /// <param name="out_Length">Returns the number of buffers in the specified chain.</param>
            /// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
            /// <see cref="CreateTextureSwapChainDX"/>
            /// <see cref="CreateTextureSwapChainGL"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTextureSwapChainLength", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetTextureSwapChainLength(IntPtr sessionPtr, IntPtr chain, out int out_Length);

            /// <summary>
            /// Gets the current index in an IntPtr.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="chain">Specifies the IntPtr for which the index should be retrieved.</param>
            /// <param name="out_Index">Returns the current (free) index in specified chain.</param>
            /// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
            /// <see cref="CreateTextureSwapChainDX"/>
            /// <see cref="CreateTextureSwapChainGL"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTextureSwapChainCurrentIndex", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetTextureSwapChainCurrentIndex(IntPtr sessionPtr, IntPtr chain, out int out_Index);

            /// <summary>
            /// Gets the description of the buffers in an IntPtr
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="chain">Specifies the IntPtr for which the description should be retrieved.</param>
            /// <param name="out_Desc">Returns the description of the specified chain.</param>
            /// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
            /// <see cref="CreateTextureSwapChainDX"/>
            /// <see cref="CreateTextureSwapChainGL"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTextureSwapChainDesc", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetTextureSwapChainDesc(IntPtr sessionPtr, IntPtr chain, [In, Out] ref TextureSwapChainDesc out_Desc);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_CommitTextureSwapChain", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_CommitTextureSwapChain(IntPtr sessionPtr, IntPtr chain);

            /// <summary>
            /// Destroys an IntPtr and frees all the resources associated with it.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="chain">Specifies the IntPtr to destroy. If it is null then this function has no effect.</param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_DestroyTextureSwapChain", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_DestroyTextureSwapChain(IntPtr sessionPtr, IntPtr chain);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_DestroyMirrorTexture", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_DestroyMirrorTexture(IntPtr sessionPtr, IntPtr mirrorTexturePtr);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetFovTextureSize", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Sizei ovr_GetFovTextureSize(IntPtr sessionPtr, EyeType eye, FovPort fov, float pixelsPerDisplayPixel);


            // NOTE: In Oculus SDK 1.17 the EyeRenderDesc was changed. The get the new struct we need to call the ovr_GetRenderDesc2 method.
            // See https://forums.oculus.com/developer/discussion/comment/562711#Comment_562711

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetRenderDesc2", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern EyeRenderDesc ovr_GetRenderDesc(IntPtr sessionPtr, EyeType eyeType, FovPort fov);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SubmitFrame", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SubmitFrame(IntPtr sessionPtr, Int64 frameIndex, IntPtr viewScaleDesc, IntPtr layerPtrList, int layerCount);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetPredictedDisplayTime", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern double ovr_GetPredictedDisplayTime(IntPtr sessionPtr, Int64 frameIndex);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTimeInSeconds", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern double ovr_GetTimeInSeconds();

            /// <summary>
            /// Reads a boolean property.
            /// </summary>
            /// <param name="sessionPtr">
            /// Specifies an IntPtr previously returned by ovr_Create.
            /// </param>
            /// <param name="propertyName">
            /// The name of the property, which needs to be valid for only the call.
            /// </param>
            /// <param name="defaultVal">
            /// Specifes the value to return if the property couldn't be read.
            /// </param>
            /// <returns>
            /// Returns the property interpreted as a boolean value. 
            /// Returns defaultVal if the property doesn't exist.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetBool", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Byte ovr_GetBool(IntPtr sessionPtr, string propertyName, Byte defaultVal);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetBool", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Byte ovr_SetBool(IntPtr sessionPtr, string propertyName, Byte value);

            /// <summary>
            /// Reads an integer property.
            /// </summary>
            /// <param name="sessionPtr">
            /// Specifies an IntPtr previously returned by ovr_Create.
            /// </param>
            /// <param name="propertyName">
            /// The name of the property, which needs to be valid for only the call.
            /// </param>
            /// <param name="defaultVal">
            /// Specifes the value to return if the property couldn't be read.
            /// </param>
            /// <returns>
            /// Returns the property interpreted as an integer value. 
            /// Returns defaultVal if the property doesn't exist.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetInt", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ovr_GetInt(IntPtr sessionPtr, string propertyName, int defaultVal);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetInt", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Byte ovr_SetInt(IntPtr sessionPtr, string propertyName, int value);

            /// <summary>
            /// Reads a float property.
            /// </summary>
            /// <param name="sessionPtr">
            /// Specifies an IntPtr previously returned by ovr_Create.
            /// </param>
            /// <param name="propertyName">
            /// The name of the property, which needs to be valid for only the call.
            /// </param>
            /// <param name="defaultVal">
            /// Specifes the value to return if the property couldn't be read.
            /// </param>
            /// <returns>
            /// Returns the property interpreted as an float value. 
            /// Returns defaultVal if the property doesn't exist.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetFloat", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern float ovr_GetFloat(IntPtr sessionPtr, string propertyName, float defaultVal);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetFloat", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Byte ovr_SetFloat(IntPtr sessionPtr, string propertyName, float value);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetFloatArray", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ovr_GetFloatArray(IntPtr sessionPtr, string propertyName, [MarshalAs(UnmanagedType.LPArray)] float[] values, int valuesCapacity);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetFloatArray", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Byte ovr_SetFloatArray(IntPtr sessionPtr, string propertyName, [MarshalAs(UnmanagedType.LPArray)] float[] values, int valuesSize);

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
            /// <param name="defaultVal">
            /// Specifes the value to return if the property couldn't be read.
            /// </param>
            /// <returns>
            /// Returns the string property if it exists. 
            /// 
            /// Otherwise returns defaultVal, which can be specified as null.
            /// The return memory is guaranteed to be valid until next call to ovr_GetString or 
            /// until the HMD is destroyed, whichever occurs first.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetString", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr ovr_GetString(IntPtr sessionPtr, string propertyName, string defaultVal);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetString", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Byte ovr_SetString(IntPtr sessionPtr, string propertyName, string value);

            /// <summary>
            /// Used to generate projection from ovrEyeDesc::Fov.
            /// </summary>
            /// <param name="fov">
            /// Specifies the ovrFovPort to use.
            /// </param>
            /// <param name="znear">
            /// Distance to near Z limit.
            /// </param>
            /// <param name="zfar">
            /// Distance to far Z limit.
            /// </param>
            /// <param name="projectionModFlags">
            /// A combination of the ProjectionModifier flags.
            /// </param>
            /// <returns>
            /// Returns the calculated projection matrix.
            /// </returns>
            /// <see cref="ProjectionModifier"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovrMatrix4f_Projection", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Matrix4f ovrMatrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags);

            /// <summary>
            /// Extracts the required data from the result of ovrMatrix4f_Projection.
            /// </summary>
            /// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
            /// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
            /// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
            /// <see cref="TimewarpProjectionDesc"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovrTimewarpProjectionDesc_FromProjection", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern TimewarpProjectionDesc ovrTimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovrMatrix4f_OrthoSubProjection", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Matrix4f ovrMatrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX);

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
            /// <param name="outEyePoses">
            /// If outEyePoses are used for rendering, they should be passed to 
            /// ovr_SubmitFrame in LayerEyeFov.RenderPose or LayerEyeFovDepth.RenderPose.
            /// </param>     
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_CalcEyePoses", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_CalcEyePoses(Posef headPose, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] Vector3f[] hmdToEyeOffset, IntPtr outEyePoses);

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
            /// <param name="outEyePoses">
            /// The predicted eye poses.
            /// </param>
            /// <param name="outSensorSampleTime">
            /// The time when this function was called. 
            /// May be null, in which case it is ignored.
            /// </param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetEyePoses", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovr_GetEyePoses(IntPtr sessionPtr, Int64 frameIndex, Byte latencyMarker, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] Vector3f[] hmdToEyeOffset, IntPtr outEyePoses,
                ref double outSensorSampleTime);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovrPosef_FlipHandedness", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void ovrPosef_FlipHandedness(ref Posef inPose, [In, Out] ref Posef outPose);

            /// <summary>
            /// Create Texture Swap Chain suitable for use with Direct3D 11 and 12.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="d3dPtr">
            /// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue 
            /// which must be the same one the application renders to the eye textures with.</param>
            /// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
            /// <param name="out_TextureSwapChain">
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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_CreateTextureSwapChainDX", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_CreateTextureSwapChainDX(IntPtr sessionPtr, IntPtr d3dPtr, ref TextureSwapChainDesc desc, [In, Out] ref IntPtr out_TextureSwapChain);

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
            /// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
            /// <returns>
            /// Returns an ovrResult indicating success or failure. In the case of failure, use 
            /// ovr_GetLastErrorInfo to get more information.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTextureSwapChainBufferDX", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetTextureSwapChainBufferDX(IntPtr sessionPtr, IntPtr chain, int index, Guid iid, [In, Out] ref IntPtr out_Buffer);

            /// <summary>
            /// Create Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
            ///
            /// A second call to ovr_CreateMirrorTextureDX for a given IntPtr before destroying the first one
            /// is not supported and will result in an error return.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="d3dPtr">
            /// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue
            /// which must be the same one the application renders to the textures with.
            /// </param>
            /// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
            /// <param name="out_MirrorTexture">
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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_CreateMirrorTextureDX", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_CreateMirrorTextureDX(IntPtr sessionPtr, IntPtr d3dPtr, ref MirrorTextureDesc desc, [In, Out] ref IntPtr out_MirrorTexture);

            /// <summary>
            /// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="mirrorTexturePtr">Specifies an IntPtr previously returned by ovr_CreateMirrorTextureDX</param>
            /// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
            /// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
            /// <returns>
            /// Returns an ovrResult indicating success or failure. In the case of failure, use 
            /// ovr_GetLastErrorInfo to get more information.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetMirrorTextureBufferDX", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetMirrorTextureBufferDX(IntPtr sessionPtr, IntPtr mirrorTexturePtr, Guid iid, [In, Out] ref IntPtr out_Buffer);

            /// <summary>
            /// Creates a TextureSwapChain suitable for use with OpenGL.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="desc">Specifies the requested texture properties. See notes for more info about texture format.</param>
            /// <param name="out_TextureSwapChain">
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
            /// rendering. Furthermore, the app should then make sure "glEnable(GL_FRAMEBUFFER_SRGB);" is called before rendering
            /// into these textures. Even though it is not recommended, if the application would like to treat the texture as a linear
            /// format and do linear-to-gamma conversion in GLSL, then the application can avoid calling "glEnable(GL_FRAMEBUFFER_SRGB);",
            /// but should still pass in an sRGB variant for the format. Failure to do so will cause the distortion compositor
            /// to apply incorrect gamma conversions leading to gamma-curve artifacts.		
            /// </remarks>
            /// <see cref="GetTextureSwapChainLength"/>
            /// <see cref="GetTextureSwapChainCurrentIndex"/>
            /// <see cref="GetTextureSwapChainDesc"/>
            /// <see cref="GetTextureSwapChainBufferGL"/>
            /// <see cref="DestroyTextureSwapChain"/>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_CreateTextureSwapChainGL", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_CreateTextureSwapChainGL(IntPtr sessionPtr, TextureSwapChainDesc desc, [Out] out IntPtr out_TextureSwapChain);

            /// <summary>
            /// Get a specific buffer within the chain as a GL texture name
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="chain">Specifies an IntPtr previously returned by ovr_CreateTextureSwapChainGL</param>
            /// <param name="index">
            /// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength)
            /// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
            /// </param>
            /// <param name="out_TexId">Returns the GL texture object name associated with the specific index requested</param>
            /// <returns>
            /// Returns an ovrResult indicating success or failure. In the case of failure, use 
            /// ovr_GetLastErrorInfo to get more information.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTextureSwapChainBufferGL", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetTextureSwapChainBufferGL(IntPtr sessionPtr, IntPtr chain, int index, [Out] out uint out_TexId);

            /// <summary>
            /// Creates a Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
            ///
            /// A second call to ovr_CreateMirrorTextureGL for a given IntPtr before destroying the first one
            /// is not supported and will result in an error return.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="desc">Specifies the requested mirror texture description.</param>
            /// <param name="out_MirrorTexture">
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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_CreateMirrorTextureGL", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_CreateMirrorTextureGL(IntPtr sessionPtr, MirrorTextureDesc desc, [Out] out IntPtr out_MirrorTexture);

            /// <summary>
            /// Get a the underlying buffer as a GL texture name
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="mirrorTexturePtr">Specifies an IntPtr previously returned by ovr_CreateMirrorTextureGL</param>
            /// <param name="out_TexId">Specifies the GL texture object name associated with the mirror texture</param>
            /// <returns>
            /// Returns an ovrResult indicating success or failure. In the case of failure, use 
            /// ovr_GetLastErrorInfo to get more information.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetMirrorTextureBufferGL", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetMirrorTextureBufferGL(IntPtr sessionPtr, IntPtr mirrorTexturePtr, [Out] out uint out_TexId);

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
            /// <returns>
            /// Returns an ovrResult indicating success or failure. In the case of failure, use 
            /// ovr_GetLastErrorInfo to get more information.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_IdentifyClient", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_IdentifyClient(string identity);

            /// <summary>
            /// Gets information about Haptics engine for the specified Touch controller.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="controllerType">The controller to retrieve the information from.</param>
            /// <returns>
            /// Returns an TouchHapticsDesc.
            /// </returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetTouchHapticsDesc", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern TouchHapticsDesc ovr_GetTouchHapticsDesc(IntPtr sessionPtr, ControllerType controllerType);

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
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SubmitControllerVibration", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SubmitControllerVibration(IntPtr sessionPtr, ControllerType controllerType, HapticsBuffer buffer);

            /// <summary>
            /// Gets the Haptics engine playback state of a specific Touch controller.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="controllerType">The controller to retrieve the information from.</param>
            /// <param name="out_State">State of the haptics engine.</param>
            /// <returns>
            /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///          upon success. Return values include but aren't limited to:
            ///      - ovrSuccess: The call succeeded and a result was returned.
            ///      - ovrSuccess_DeviceUnavailable: The call succeeded but the device referred to by controllerType is not available.
            ///  </returns>   
            /// <seealso cref="HapticsPlaybackState"/> 
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetControllerVibrationState", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetControllerVibrationState(IntPtr sessionPtr, ControllerType controllerType, ref HapticsPlaybackState out_State);

            /// <summary>
            /// Tests collision/proximity of position tracked devices (e.g. HMD and/or Touch) against the Boundary System.
            /// Note: this method is similar to ovr_BoundaryTestPoint but can be more precise as it may take into account device acceleration/momentum.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="deviceBitmask">Bitmask of one or more tracked devices to test.</param>
            /// <param name="boundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea.</param>
            /// <param name="out_TestResult">Result of collision/proximity test, contains information such as distance and closest point.</param>
            /// <returns>
            /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///         upon success. Return values include but aren't limited to:
            ///     - ovrSuccess: The call succeeded and a result was returned.
            ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
            ///     - ovrSuccess_DeviceUnavailable: The call succeeded but the device referred to by deviceBitmask is not available.
            /// </returns>   
            /// <seealso cref="BoundaryTestResult"/> 
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_TestBoundary", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_TestBoundary(IntPtr sessionPtr, TrackedDeviceType deviceBitmask, BoundaryType boundaryType, ref BoundaryTestResult out_TestResult);

            /// <summary>
            /// Tests collision/proximity of a 3D point against the Boundary System.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="point">3D point to test.</param>
            /// <param name="singleBoundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea to test against</param>
            /// <param name="out_TestResult">Result of collision/proximity test, contains information such as distance and closest point.</param>
            /// <returns>
            /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///         upon success. Return values include but aren't limited to:
            ///     - ovrSuccess: The call succeeded and a result was returned.
            ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
            /// </returns>   
            /// <seealso cref="BoundaryTestResult"/>         
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_TestBoundaryPoint", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_TestBoundaryPoint(IntPtr sessionPtr, ref Vector3f point, BoundaryType singleBoundaryType, ref BoundaryTestResult out_TestResult);

            /// <summary>
            /// Sets the look and feel of the Boundary System.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="lookAndFeel">Look and feel parameters.</param>
            /// <returns>
            /// Returns ovrSuccess upon success.
            /// </returns>   
            /// <seealso cref="BoundaryLookAndFeel"/>   
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetBoundaryLookAndFeel", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SetBoundaryLookAndFeel(IntPtr sessionPtr, ref BoundaryLookAndFeel lookAndFeel);

            /// <summary>
            /// Resets the look and feel of the Boundary System to its default state.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <returns>
            /// Returns ovrSuccess upon success.
            /// </returns>
            /// <seealso cref="BoundaryLookAndFeel"/>     
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_ResetBoundaryLookAndFeel", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_ResetBoundaryLookAndFeel(IntPtr sessionPtr);

            /// <summary>
            /// Gets the geometry of the Boundary System's "play area" or "outer boundary" as 3D floor points.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="boundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea.</param>
            /// <param name="out_floorPointPtr">IntPtr to array of 3D points (in clockwise order) defining the boundary at floor height (can be NULL to retrieve only the number of points).</param>
            /// <param name="out_FloorPointsCount"> Number of 3D points returned in the array.</param>
            /// <returns>
            /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///         upon success. Return values include but aren't limited to:
            ///     - ovrSuccess: The call succeeded and a result was returned.
            ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
            /// </returns>   
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetBoundaryGeometry", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetBoundaryGeometry(IntPtr sessionPtr, BoundaryType boundaryType, IntPtr out_floorPointPtr, ref int out_FloorPointsCount);

            /// <summary>
            /// Gets the dimension of the Boundary System's "play area" or "outer boundary".
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="boundaryType">Must be either ovrBoundary_Outer or ovrBoundary_PlayArea.</param>
            /// <param name="out_Dimensions">Dimensions of the axis aligned bounding box that encloses the area in meters (width, height and length).</param>
            /// <returns>
            /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///         upon success. Return values include but aren't limited to:
            ///     - ovrSuccess: The call succeeded and a result was returned.
            ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
            /// </returns>   
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetBoundaryDimensions", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetBoundaryDimensions(IntPtr sessionPtr, BoundaryType boundaryType, float[] out_Dimensions);

            /// <summary>
            /// Returns if the boundary is currently visible.
            /// Note: visibility is false if the user has turned off boundaries, otherwise, it's true if the app has requested 
            /// boundaries to be visible or if any tracked device is currently triggering it. This may not exactly match rendering 
            /// due to fade-in and fade-out effects.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="out_IsVisible">ovrTrue, if the boundary is visible.</param>
            /// <returns>
            /// Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///         upon success. Return values include but aren't limited to:
            ///     - ovrSuccess: Result was successful and a result was returned.
            ///     - ovrSuccess_BoundaryInvalid: The call succeeded but the result is not a valid boundary due to not being set up.
            /// </returns>   
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetBoundaryVisible", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetBoundaryVisible(IntPtr sessionPtr, [In, Out] ref Byte out_IsVisible);

            /// <summary>
            /// Requests boundary to be visible.
            /// </summary>
            /// <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="visible">forces the outer boundary to be visible. An application can't force it to be invisible, but can cancel its request by passing false.</param>
            /// <returns>
            /// Returns ovrSuccess upon success.
            /// </returns>   
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_RequestBoundaryVisible", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_RequestBoundaryVisible(IntPtr sessionPtr, Byte visible);

            ///  <summary>
            ///  Retrieves performance stats for the VR app as well as the SDK compositor.
            /// 
            ///  If the app calling this function is not the one in focus (i.e. not visible in the HMD), then
            ///  outStats will be zero'd out.
            ///  New stats are populated after each successive call to ovr_SubmitFrame. So the app should call
            ///  this function on the same thread it calls ovr_SubmitFrame, preferably immediately
            ///  afterwards.
            ///  </summary>
            ///  <param name="sessionPtr">Specifies an IntPtr previously returned by ovr_Create.</param>
            /// <param name="outStats">Contains the performance stats for the application and SDK compositor</param>
            /// <returns>
            ///  Returns an ovrResult for which OVR_SUCCESS(result) is false upon error and true
            ///         upon success.
            ///  </returns> 
            /// <seealso cref="PerfStats"/>  
            /// <seealso cref="PerfStatsPerCompositorFrame"/>  
            /// <seealso cref="ovr_ResetPerfStats"/>  
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetPerfStats", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetPerfStats(IntPtr sessionPtr, [In, Out] ref PerfStats outStats);

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
            /// <seealso cref="ovr_GetPerfStats"/>          
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_ResetPerfStats", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_ResetPerfStats(IntPtr sessionPtr);

            /// <summary>
            /// Returns the number of camera properties of all cameras
            /// </summary>
            /// <param name="sessionPtr">session Specifies an ovrSession previously returned by ovr_Create.</param>
            /// <param name="cameras">cameras Pointer to the array. If null and the provided array capacity is sufficient, will return ovrError_NullArrayPointer.</param>
            /// <param name="inoutCameraCount">inoutCameraCount Supply the array capacity, will return the actual # of cameras defined. If *inoutCameraCount is too small, will return ovrError_InsufficientArraySize.</param>
            /// <returns>Returns the ids of external cameras the system knows about. Returns ovrError_NoExternalCameraInfo if there is not any eternal camera information.</returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_GetExternalCameras", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_GetExternalCameras(IntPtr sessionPtr, [In, Out] ExternalCamera[] cameras, ref int inoutCameraCount);

            /// <summary>
            /// Sets the camera intrinsics and/or extrinsics stored for the cameraName camera Names must be less then 32 characters and null-terminated.
            /// </summary>
            /// <param name="sessionPtr">session Specifies an ovrSession previously returned by ovr_Create.</param>
            /// <param name="name">Specifies which camera to set the intrinsics or extrinsics for</param>
            /// <param name="intrinsics">Contains the intrinsic parameters to set, can be null</param>
            /// <param name="extrinsics">Contains the extrinsic parameters to set, can be null</param>
            /// <returns>Returns ovrSuccess or an ovrError code</returns>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(_ovrDllName, EntryPoint = "ovr_SetExternalCameraProperties", SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
            internal static extern Result ovr_SetExternalCameraProperties(IntPtr sessionPtr, string name, ref CameraIntrinsics intrinsics, ref CameraExtrinsics extrinsics);
        }
    }
}