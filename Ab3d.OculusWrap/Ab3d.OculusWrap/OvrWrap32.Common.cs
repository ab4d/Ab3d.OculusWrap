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
    // This file is the same as OvrWrap64.Common.cs except that it is part of OvrWrap32 partial class.
    // This way maintenance of the library is much easier because all the changes need to be done only in one place
    // and then the file content can be easily copied to the OvrWrap64 class.

    public sealed partial class OvrWrap32
    {
        // The Oculus SDK is full of missing comments.
        // Ignore warnings regarding missing comments, in this class.
#pragma warning disable 1591

        #region Oculus SDK methods

        /// <inheritdoc />
        public override DetectResult Detect(int timeoutMilliseconds)
        {
            return SafeNativeMethods.ovr_Detect(timeoutMilliseconds);
        }

        /// <inheritdoc />
		public override Result Initialize(InitParams parameters = null)
        {
            return SafeNativeMethods.ovr_Initialize(parameters);
        }

        /// <inheritdoc />
        public override ErrorInfo GetLastErrorInfo()
        {
            ErrorInfo errorInfo;
            SafeNativeMethods.ovr_GetLastErrorInfo(out errorInfo);

            return errorInfo;
        }

        /// <inheritdoc />
        public override string GetVersionString()
        {
            IntPtr stringPtr = SafeNativeMethods.ovr_GetVersionString();
            string resultString = Marshal.PtrToStringAnsi(stringPtr);
            return resultString;
        }

        /// <inheritdoc />
        public override int TraceMessage(LogLevel level, string message)
        {
            return SafeNativeMethods.ovr_TraceMessage((int)level, message);
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            SafeNativeMethods.ovr_Shutdown();
        }

        /// <inheritdoc />
        public override int GetTrackerCount(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_GetTrackerCount(sessionPtr);
        }

        /// <inheritdoc />
        public override TrackerDesc GetTrackerDesc(IntPtr sessionPtr, uint trackerDescIndex)
        {
            return SafeNativeMethods.ovr_GetTrackerDesc(sessionPtr, trackerDescIndex);
        }

        /// <inheritdoc />
        public override Result Create(ref IntPtr sessionPtr, ref GraphicsLuid pLuid)
        {
            return SafeNativeMethods.ovr_Create(ref sessionPtr, ref pLuid);
        }

        /// <inheritdoc />
        public override void Destroy(IntPtr sessionPtr)
        {
            SafeNativeMethods.ovr_Destroy(sessionPtr);
        }

        /// <inheritdoc />
        public override Result GetSessionStatus(IntPtr sessionPtr, ref SessionStatus sessionStatus)
        {
            return SafeNativeMethods.ovr_GetSessionStatus(sessionPtr, ref sessionStatus);
        }

        /// <inheritdoc />
        public override Result SetTrackingOriginType(IntPtr sessionPtr, TrackingOrigin origin)
        {
            return SafeNativeMethods.ovr_SetTrackingOriginType(sessionPtr, origin);
        }

        /// <inheritdoc />
        public override TrackingOrigin GetTrackingOriginType(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_GetTrackingOriginType(sessionPtr);
        }

        /// <inheritdoc />
        public override Result RecenterTrackingOrigin(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_RecenterTrackingOrigin(sessionPtr);
        }

        /// <inheritdoc />
        public override Result SpecifyTrackingOrigin(IntPtr sessionPtr, Posef originPose)
        {
            return SafeNativeMethods.ovr_SpecifyTrackingOrigin(sessionPtr, originPose);
        }

        /// <inheritdoc />
        public override void ClearShouldRecenterFlag(IntPtr sessionPtr)
        {
            SafeNativeMethods.ovr_ClearShouldRecenterFlag(sessionPtr);
        }

        /// <inheritdoc />
        public override TrackerPose GetTrackerPose(IntPtr sessionPtr, uint trackerPoseIndex)
        {
            return SafeNativeMethods.ovr_GetTrackerPose(sessionPtr, trackerPoseIndex);
        }

        /// <inheritdoc />
        public override Result GetInputState(IntPtr sessionPtr, ControllerType controllerType, ref InputState inputState)
        {
            return SafeNativeMethods.ovr_GetInputState(sessionPtr, controllerType, ref inputState);
        }

        /// <inheritdoc />
        public override ControllerType GetConnectedControllerTypes(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_GetConnectedControllerTypes(sessionPtr);
        }

        /// <inheritdoc />
        public override TrackingState GetTrackingState(IntPtr sessionPtr, double absTime, bool latencyMarker)
        {
            TrackingState trackingState;
            SafeNativeMethods.ovr_GetTrackingState(out trackingState, sessionPtr, absTime, latencyMarker ? (byte)1 : (byte)0);

            return trackingState;
        }

        /// <inheritdoc />
        public override Result GetDevicePoses(IntPtr sessionPtr, TrackedDeviceType[] deviceTypes, double absTime, PoseStatef[] outDevicePoses)
        {
            if (deviceTypes == null) throw new ArgumentNullException(nameof(deviceTypes));
            if (outDevicePoses == null) throw new ArgumentNullException(nameof(outDevicePoses));

            if (deviceTypes.Length != outDevicePoses.Length) throw new ArgumentException("Size of deviceTypes array must be the same as size of outDevicePoses array");

            return SafeNativeMethods.ovr_GetDevicePoses(sessionPtr, deviceTypes, deviceTypes.Length, absTime, outDevicePoses);
        }

        /// <inheritdoc />
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


        /// <inheritdoc />
        public override Result GetTextureSwapChainLength(IntPtr sessionPtr, IntPtr chain, out int length)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainLength(sessionPtr, chain, out length);
        }

        /// <inheritdoc />
        public override Result GetTextureSwapChainCurrentIndex(IntPtr sessionPtr, IntPtr chain, out int index)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainCurrentIndex(sessionPtr, chain, out index);
        }

        /// <inheritdoc />
        public override Result GetTextureSwapChainDesc(IntPtr sessionPtr, IntPtr chain, ref TextureSwapChainDesc desc)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainDesc(sessionPtr, chain, ref desc);
        }

        /// <inheritdoc />
        public override Result CommitTextureSwapChain(IntPtr sessionPtr, IntPtr chain)
        {
            return SafeNativeMethods.ovr_CommitTextureSwapChain(sessionPtr, chain);
        }

        /// <inheritdoc />
        public override void DestroyTextureSwapChain(IntPtr sessionPtr, IntPtr chain)
        {
            SafeNativeMethods.ovr_DestroyTextureSwapChain(sessionPtr, chain);
        }

        // MirrorTexture creation is rendering API-specific.

        /// <inheritdoc />
        public override void DestroyMirrorTexture(IntPtr sessionPtr, IntPtr mirrorTexturePtr)
        {
            SafeNativeMethods.ovr_DestroyMirrorTexture(sessionPtr, mirrorTexturePtr);
        }

        /// <inheritdoc />
        public override Sizei GetFovTextureSize(IntPtr sessionPtr, EyeType eye, FovPort fov, float pixelsPerDisplayPixel)
        {
            return SafeNativeMethods.ovr_GetFovTextureSize(sessionPtr, eye, fov, pixelsPerDisplayPixel);
        }

        /// <inheritdoc />
        public override EyeRenderDesc GetRenderDesc(IntPtr sessionPtr, EyeType eyeType, FovPort fov)
        {
            return SafeNativeMethods.ovr_GetRenderDesc(sessionPtr, eyeType, fov);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override double GetPredictedDisplayTime(IntPtr sessionPtr, Int64 frameIndex)
        {
            return SafeNativeMethods.ovr_GetPredictedDisplayTime(sessionPtr, frameIndex);
        }

        /// <inheritdoc />
        public override double GetTimeInSeconds()
        {
            return SafeNativeMethods.ovr_GetTimeInSeconds();
        }

        /// <inheritdoc />
        public override bool GetBool(IntPtr sessionPtr, string propertyName, bool defaultValue)
        {
            byte result = SafeNativeMethods.ovr_GetBool(sessionPtr, propertyName, defaultValue ? (byte)1 : (byte)0);
            return result != 0;
        }

        /// <inheritdoc />
        public override bool SetBool(IntPtr sessionPtr, string propertyName, bool value)
        {
            byte result = SafeNativeMethods.ovr_SetBool(sessionPtr, propertyName, value ? (byte)1 : (byte)0);
            return result != 0;
        }

        /// <inheritdoc />
        public override int GetInt(IntPtr sessionPtr, string propertyName, int defaultValue)
        {
            return SafeNativeMethods.ovr_GetInt(sessionPtr, propertyName, defaultValue);
        }

        /// <inheritdoc />
        public override bool SetInt(IntPtr sessionPtr, string propertyName, int value)
        {
            byte result = SafeNativeMethods.ovr_SetInt(sessionPtr, propertyName, value);
            return result != 0;
        }

        /// <inheritdoc />
        public override float GetFloat(IntPtr sessionPtr, string propertyName, float defaultValue)
        {
            return SafeNativeMethods.ovr_GetFloat(sessionPtr, propertyName, defaultValue);
        }

        /// <inheritdoc />
        public override bool SetFloat(IntPtr sessionPtr, string propertyName, float value)
        {
            byte result = SafeNativeMethods.ovr_SetFloat(sessionPtr, propertyName, value);
            return result != 0;
        }

        /// <inheritdoc />
        public override int GetFloatArray(IntPtr sessionPtr, string propertyName, float[] values, int valuesCapacity)
        {
            return SafeNativeMethods.ovr_GetFloatArray(sessionPtr, propertyName, values, valuesCapacity);
        }

        /// <inheritdoc />
        public override bool SetFloatArray(IntPtr sessionPtr, string propertyName, float[] values, int valuesSize)
        {
            byte result = SafeNativeMethods.ovr_SetFloatArray(sessionPtr, propertyName, values, valuesSize);
            return result != 0;
        }

        /// <inheritdoc />
        public override string GetString(IntPtr sessionPtr, string propertyName, string defaultValue)
        {
            IntPtr resultPtr = SafeNativeMethods.ovr_GetString(sessionPtr, propertyName, defaultValue);
            string result = Marshal.PtrToStringAnsi(resultPtr);

            return result;
        }

        /// <inheritdoc />
        public override bool SetString(IntPtr sessionPtr, string propertyName, string value)
        {
            byte result = SafeNativeMethods.ovr_SetString(sessionPtr, propertyName, value);
            return result != 0;
        }

        /// <inheritdoc />
        public override Matrix4f Matrix4f_Projection(FovPort fov, float zNear, float zFar, ProjectionModifier projectionModFlags)
        {
            return SafeNativeMethods.ovrMatrix4f_Projection(fov, zNear, zFar, projectionModFlags);
        }

        /// <inheritdoc />
        public override TimewarpProjectionDesc TimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags)
        {
            return SafeNativeMethods.ovrTimewarpProjectionDesc_FromProjection(projection, projectionModFlags);
        }

        /// <inheritdoc />
        public override Matrix4f Matrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX)
        {
            return SafeNativeMethods.ovrMatrix4f_OrthoSubProjection(projection, orthoScale, orthoDistance, hmdToEyeOffsetX);
        }

        /// <inheritdoc />  
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override void Posef_FlipHandedness(ref Posef inPose, ref Posef outPose)
        {
            SafeNativeMethods.ovrPosef_FlipHandedness(ref inPose, ref outPose);
        }

        /// <inheritdoc />
        public override Result CreateTextureSwapChainDX(IntPtr sessionPtr, IntPtr d3dDevicePtr, ref TextureSwapChainDesc desc, out IntPtr textureSwapChainPtr)
        {
            textureSwapChainPtr = IntPtr.Zero;
            return SafeNativeMethods.ovr_CreateTextureSwapChainDX(sessionPtr, d3dDevicePtr, ref desc, ref textureSwapChainPtr);
        }

        /// <inheritdoc />
        public override Result GetTextureSwapChainBufferDX(IntPtr sessionPtr, IntPtr chain, int index, Guid iid, out IntPtr buffer)
        {
            buffer = IntPtr.Zero;
            return SafeNativeMethods.ovr_GetTextureSwapChainBufferDX(sessionPtr, chain, index, iid, ref buffer);
        }

        /// <inheritdoc />
        public override Result CreateMirrorTextureDX(IntPtr sessionPtr, IntPtr d3dDevicePtr, ref MirrorTextureDesc desc, out IntPtr mirrorTexturePtr)
        {
            mirrorTexturePtr = IntPtr.Zero;
            return SafeNativeMethods.ovr_CreateMirrorTextureDX(sessionPtr, d3dDevicePtr, ref desc, ref mirrorTexturePtr);
        }

        /// <inheritdoc />
        public override Result GetMirrorTextureBufferDX(IntPtr sessionPtr, IntPtr mirrorTexturePtr, Guid iid, out IntPtr buffer)
        {
            buffer = IntPtr.Zero;
            return SafeNativeMethods.ovr_GetMirrorTextureBufferDX(sessionPtr, mirrorTexturePtr, iid, ref buffer);
        }

        /// <inheritdoc />
        public override Result CreateTextureSwapChainGL(IntPtr sessionPtr, TextureSwapChainDesc desc, out IntPtr textureSwapChainPtr)
        {
            return SafeNativeMethods.ovr_CreateTextureSwapChainGL(sessionPtr, desc, out textureSwapChainPtr);
        }

        /// <inheritdoc />
        public override Result GetTextureSwapChainBufferGL(IntPtr sessionPtr, IntPtr chain, int index, out uint textureId)
        {
            return SafeNativeMethods.ovr_GetTextureSwapChainBufferGL(sessionPtr, chain, index, out textureId);
        }

        /// <inheritdoc />
        public override Result CreateMirrorTextureGL(IntPtr sessionPtr, MirrorTextureDesc desc, out IntPtr mirrorTexturePtr)
        {
            return SafeNativeMethods.ovr_CreateMirrorTextureGL(sessionPtr, desc, out mirrorTexturePtr);
        }

        /// <inheritdoc />
        public override Result GetMirrorTextureBufferGL(IntPtr sessionPtr, IntPtr mirrorTexturePtr, out uint textureId)
        {
            return SafeNativeMethods.ovr_GetMirrorTextureBufferGL(sessionPtr, mirrorTexturePtr, out textureId);
        }

        /// <inheritdoc />
        public override Result IdentifyClient(string identity)
        {
            return SafeNativeMethods.ovr_IdentifyClient(identity);
        }

        /// <inheritdoc />
        public override TouchHapticsDesc GetTouchHapticsDesc(IntPtr sessionPtr, ControllerType controllerType)
        {
            return SafeNativeMethods.ovr_GetTouchHapticsDesc(sessionPtr, controllerType);
        }

        /// <inheritdoc />   
        public override Result SubmitControllerVibration(IntPtr sessionPtr, ControllerType controllerType, HapticsBuffer buffer)
        {
            return SafeNativeMethods.ovr_SubmitControllerVibration(sessionPtr, controllerType, buffer);
        }

        /// <inheritdoc />
        public override Result GetControllerVibrationState(IntPtr sessionPtr, ControllerType controllerType, ref HapticsPlaybackState state)
        {
            return SafeNativeMethods.ovr_GetControllerVibrationState(sessionPtr, controllerType, ref state);
        }

        /// <inheritdoc />
        public override Result TestBoundary(IntPtr sessionPtr, TrackedDeviceType deviceBitmask, BoundaryType boundaryType, ref BoundaryTestResult testResult)
        {
            return SafeNativeMethods.ovr_TestBoundary(sessionPtr, deviceBitmask, boundaryType, ref testResult);
        }

        /// <inheritdoc />        
        public override Result TestBoundaryPoint(IntPtr sessionPtr, Vector3f point, BoundaryType singleBoundaryType, ref BoundaryTestResult testResult)
        {
            return SafeNativeMethods.ovr_TestBoundaryPoint(sessionPtr, ref point, singleBoundaryType, ref testResult);
        }

        /// <inheritdoc />   
        public override Result SetBoundaryLookAndFeel(IntPtr sessionPtr, ref BoundaryLookAndFeel lookAndFeel)
        {
            return SafeNativeMethods.ovr_SetBoundaryLookAndFeel(sessionPtr, ref lookAndFeel);
        }

        /// <inheritdoc />     
        public override Result ResetBoundaryLookAndFeel(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_ResetBoundaryLookAndFeel(sessionPtr);
        }

        /// <inheritdoc />  
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

        /// <inheritdoc />  
        public override Result GetBoundaryDimensions(IntPtr sessionPtr, BoundaryType boundaryType, out Vector3f dimensions)
        {
            var dimensionValues = new float[3];
            var result = SafeNativeMethods.ovr_GetBoundaryDimensions(sessionPtr, boundaryType, dimensionValues);

            dimensions = new Vector3f(dimensionValues[0], dimensionValues[1], dimensionValues[2]);

            return result;
        }

        /// <inheritdoc /> 
        public override Result GetBoundaryVisible(IntPtr sessionPtr, out bool isVisible)
        {
            Byte ovrIsVisible = 0;
            var result = SafeNativeMethods.ovr_GetBoundaryVisible(sessionPtr, ref ovrIsVisible);

            isVisible = (ovrIsVisible != 0);

            return result;
        }

        /// <inheritdoc />   
        public override Result RequestBoundaryVisible(IntPtr sessionPtr, bool visible)
        {
            return SafeNativeMethods.ovr_RequestBoundaryVisible(sessionPtr, visible ? (Byte)1 : (Byte)0);
        }

        /// <inheritdoc /> 
        public override Result GetPerfStats(IntPtr sessionPtr, ref PerfStats stats)
        {
            return SafeNativeMethods.ovr_GetPerfStats(sessionPtr, ref stats);
        }

        /// <inheritdoc />          
        public override Result ResetPerfStats(IntPtr sessionPtr)
        {
            return SafeNativeMethods.ovr_ResetPerfStats(sessionPtr);
        }

        /// <inheritdoc />     
        public override Result GetExternalCameras(IntPtr sessionPtr, out ExternalCamera[] cameras)
        {
            int arraySize = 0;
            Result result = Result.InsufficientArraySize;
            cameras = null;

            while (result == Result.InsufficientArraySize)
            {
                arraySize++;
                cameras = new ExternalCamera[arraySize];

                result = SafeNativeMethods.ovr_GetExternalCameras(sessionPtr, cameras, ref arraySize);
            }

            if (result == Result.NoExternalCameraInfo)
                cameras = null;

            return result;
        }

        /// <summary>
        /// Sets the camera intrinsics and/or extrinsics stored for the cameraName camera Names must be less then 32 characters and null-terminated.
        /// </summary>
        /// <param name="sessionPtr">session Specifies an ovrSession previously returned by ovr_Create.</param>
        /// <param name="name">Specifies which camera to set the intrinsics or extrinsics for</param>
        /// <param name="intrinsics">Contains the intrinsic parameters to set, can be null</param>
        /// <param name="extrinsics">Contains the extrinsic parameters to set, can be null</param>
        /// <returns>Returns ovrSuccess or an ovrError code</returns>
        public override Result SetExternalCameraProperties(IntPtr sessionPtr, string name, ref CameraIntrinsics intrinsics, ref CameraExtrinsics extrinsics)
        {
            return SafeNativeMethods.ovr_SetExternalCameraProperties(sessionPtr, name, ref intrinsics, ref extrinsics);
        }
        #endregion        
    }
}