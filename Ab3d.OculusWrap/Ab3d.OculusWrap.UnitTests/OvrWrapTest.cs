using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Ab3d.OculusWrap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ab3d.OculusWrap.UnitTests
{
	[TestClass]
	public class OvrWrapTest
    {
        /// <summary>
        /// Interface to Oculus runtime methods.
        /// </summary>
        protected OvrWrap OVR { get; set; }

        /// <summary>
        /// Initialize the Oculus VR runtime.
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
            OVR = OvrWrap.Create();

            InitParams initializationParameters = new InitParams();
            initializationParameters.Flags = InitFlags.Debug | InitFlags.RequestVersion;
            initializationParameters.ConnectionTimeoutMS = 0;
            initializationParameters.RequestedMinorVersion = 17;
            initializationParameters.LogCallback = LogCallback;

            Result result = OVR.Initialize(initializationParameters);
            Assert.IsTrue(result >= Result.Success);
        }

        /// <summary>
        /// Write a any log messages to the trace output.
        /// </summary>
        /// <param name="level">Error level of the message.</param>
        /// <param name="message">Message to display in the trace output.</param>
        protected void LogCallback(IntPtr userData, LogLevel level, string message)
        {
            string formattedMessage = string.Format("[{0}] {1}", level.ToString(), message);
            System.Diagnostics.Trace.WriteLine(formattedMessage);
        }

        /// <summary>
        /// Shut down the Oculus VR runtime.
        /// </summary>
        [TestCleanup]
        public void Shutdown()
        {
            OVR.Shutdown();
            OVR = null;
        }
        
        /// <summary>
        /// Creates the session used in the unit tests in this class.
        /// </summary>
        /// <returns>Created unmanaged session object.</returns>
        private IntPtr CreateSession()
		{
			var sessionPtr = IntPtr.Zero;
			var graphicsLuid = new GraphicsLuid();

            Result result = OVR.Create(ref sessionPtr, ref graphicsLuid);

			Assert.IsTrue(result >= Result.Success, "Failed to create a session. This usually indicates that the HMD is turned off.");
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "No session returned, even though create call succeeded.");

			return sessionPtr;
		}

		[TestMethod]
		public void Detect()
		{
			DetectResult result = OVR.Detect(0);

			Assert.AreEqual(result.IsOculusServiceRunning, true, "The Oculus service isn't running.");
			Assert.AreEqual(result.IsOculusHMDConnected, true, "The HMD isn't connected.");
		}

		[TestMethod]
		public void GetLastErrorInfo()
		{
			IntPtr session	= CreateSession();

		    var initParams = new InitParams(InitFlags.RequestVersion, 999);
		    var result = OVR.Initialize(initParams);
            
            ErrorInfo errorInfo = OVR.GetLastErrorInfo();
			Assert.IsTrue(errorInfo.Result == result);
			Assert.IsTrue(!string.IsNullOrEmpty(errorInfo.ErrorString) && errorInfo.ErrorString.Contains(".999")); // ErrorString == "Cannot reinitialize LibOVRRT with a different version. Newly requested major.minor version: 1.999; Current version: 1.17"
        }

		[TestMethod]
		public void GetVersionString()
		{
			string versionString = OVR.GetVersionString(); // "1.17.0"
            Assert.IsNotNull(versionString);

            Version version;
            Assert.IsTrue(Version.TryParse(versionString, out version)); // Check if we can parse the version text
		}

		[TestMethod]
		public void TraceMessage()
		{
			string message = "OculusWrapTest.TraceMessage unit test method ran successfully.";

			int length = OVR.TraceMessage(LogLevel.Info, message);
			Assert.AreEqual(message.Length, length);
		}

		/// <remarks>
		/// Remember to turn on the Rift headset, before running this test.
		/// </remarks>
		[TestMethod]
		public void Session_Create()
		{
            var sessionPtr = IntPtr.Zero;
            var graphicsLuid = new GraphicsLuid();

            Result result = OVR.Create(ref sessionPtr, ref graphicsLuid);

            Assert.IsTrue(result >= Result.Success, "Failed to create a session. This usually indicates that the HMD is turned off.");
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "No session returned, even though create call succeeded.");
			
			HmdDesc hmdDesc = OVR.GetHmdDesc(sessionPtr);

			Assert.IsTrue(hmdDesc.ProductName.StartsWith("Oculus"));

			Assert.AreEqual("Oculus VR", hmdDesc.Manufacturer);

			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].LeftTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].LeftTan < 2f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].RightTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].RightTan < 2f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].UpTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].UpTan < 2f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].DownTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[0].DownTan < 2f);

			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].LeftTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].LeftTan < 2f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].RightTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].RightTan < 2f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].UpTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].UpTan < 2f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].DownTan > 0.7f);
			Assert.IsTrue(hmdDesc.DefaultEyeFov[1].DownTan < 2f);
		}

		/// <remarks>
		/// Remember to turn on the Rift headset, before running this test.
		/// </remarks>
		[TestMethod]
		public void Session_Destroy()
		{
			IntPtr sessionPtr	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			OVR.Destroy(sessionPtr);
		}

		[TestMethod]
		public void Session_GetTrackerCount()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			int count = OVR.GetTrackerCount(sessionPtr);

		    Assert.IsTrue(count >= 0 && count < 99, "invalid number of sensors.");

            ErrorInfo errorInfo = OVR.GetLastErrorInfo();
		    Assert.AreEqual(errorInfo.Result, Result.Success);
		}

		[TestMethod]
		public void Session_GetTrackerDesc()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			TrackerDesc trackerDescription = OVR.GetTrackerDesc(sessionPtr, 0);
			Assert.IsTrue(trackerDescription.FrustumNearZInMeters > 0f);
			Assert.IsTrue(trackerDescription.FrustumNearZInMeters < 100f);
			Assert.IsTrue(trackerDescription.FrustumFarZInMeters > 0f);
			Assert.IsTrue(trackerDescription.FrustumFarZInMeters < 100f);
			Assert.IsTrue(trackerDescription.FrustumHFovInRadians > Math.PI/4);
			Assert.IsTrue(trackerDescription.FrustumHFovInRadians < Math.PI);	
			Assert.IsTrue(trackerDescription.FrustumVFovInRadians > Math.PI/4);
			Assert.IsTrue(trackerDescription.FrustumVFovInRadians < Math.PI);
		}

		[TestMethod]
		public void Session_GetSessionStatus()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			SessionStatus sessionStatus = new SessionStatus();
			Result result = OVR.GetSessionStatus(sessionPtr, ref sessionStatus);
			Assert.IsTrue(result >= Result.Success);
			Assert.IsTrue(sessionStatus.HmdPresent);
		}

		[TestMethod]
		public void Session_GetSetTrackingOriginType()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			Result result = OVR.SetTrackingOriginType(sessionPtr, TrackingOrigin.EyeLevel);
			Assert.IsTrue(result >= Result.Success);

			TrackingOrigin trackingOrigin = OVR.GetTrackingOriginType(sessionPtr);
			Assert.AreEqual(TrackingOrigin.EyeLevel, trackingOrigin);

			result = OVR.SetTrackingOriginType(sessionPtr, TrackingOrigin.FloorLevel);
			Assert.IsTrue(result >= Result.Success);

			trackingOrigin = OVR.GetTrackingOriginType(sessionPtr);
			Assert.AreEqual(TrackingOrigin.FloorLevel, trackingOrigin);
		}

		[TestMethod]
		public void Session_RecenterTrackingOrigin()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			Result result = OVR.RecenterTrackingOrigin(sessionPtr);

			if(result == Result.InvalidHeadsetOrientation)
				Assert.Fail("The headset couldn't be seen by the tracker.");

			Assert.IsTrue(result >= Result.Success);
		}

		[TestMethod]
		public void Session_ClearShouldRecenterFlag()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			OVR.ClearShouldRecenterFlag(sessionPtr);
		}

		[TestMethod]
		public void Session_GetTrackerPose()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr, "Failed to create session.");

			TrackerPose trackerPose = OVR.GetTrackerPose(sessionPtr, 0);

			Assert.IsTrue(trackerPose.LeveledPose.Position.X > -2f && trackerPose.LeveledPose.Position.X < 2f);
			Assert.IsTrue(trackerPose.LeveledPose.Position.Y > -2f && trackerPose.LeveledPose.Position.Y < 2f);
			Assert.IsTrue(trackerPose.LeveledPose.Position.Z > -2f && trackerPose.LeveledPose.Position.Z < 2f);

			Assert.IsTrue(trackerPose.LeveledPose.Orientation.X > -2f && trackerPose.LeveledPose.Orientation.X < 2f);
			Assert.IsTrue(trackerPose.LeveledPose.Orientation.Y > -2f && trackerPose.LeveledPose.Orientation.Y < 2f);
			Assert.IsTrue(trackerPose.LeveledPose.Orientation.Z > -2f && trackerPose.LeveledPose.Orientation.Z < 2f);
			Assert.IsTrue(trackerPose.LeveledPose.Orientation.W > -2f && trackerPose.LeveledPose.Orientation.W < 2f);

			Assert.IsTrue(trackerPose.Pose.Position.X > -2f && trackerPose.Pose.Position.X < 2f);
			Assert.IsTrue(trackerPose.Pose.Position.Y > -2f && trackerPose.Pose.Position.Y < 2f);
			Assert.IsTrue(trackerPose.Pose.Position.Z > -2f && trackerPose.Pose.Position.Z < 2f);

			Assert.IsTrue(trackerPose.Pose.Orientation.X > -2f && trackerPose.Pose.Orientation.X < 2f);
			Assert.IsTrue(trackerPose.Pose.Orientation.Y > -2f && trackerPose.Pose.Orientation.Y < 2f);
			Assert.IsTrue(trackerPose.Pose.Orientation.Z > -2f && trackerPose.Pose.Orientation.Z < 2f);
			Assert.IsTrue(trackerPose.Pose.Orientation.W > -2f && trackerPose.Pose.Orientation.W < 2f);

			Assert.AreNotEqual(TrackerFlags.None, trackerPose.TrackerFlags, "The tracker is not connected.");
		}
		
		[TestMethod]
		public void Session_GetTrackingState()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			TrackingState trackingState = OVR.GetTrackingState(sessionPtr, 0.0, true);

			Assert.AreNotEqual(0, trackingState.HeadPose.ThePose.Orientation.X);
			Assert.AreNotEqual(0, trackingState.HeadPose.ThePose.Orientation.Y);
			Assert.AreNotEqual(0, trackingState.HeadPose.ThePose.Orientation.Z);
			Assert.AreNotEqual(1, trackingState.HeadPose.ThePose.Orientation.W);
		}

		[TestMethod]
		public void Session_GetInputState()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			InputState inputState = new InputState();
			Result result = OVR.GetInputState(sessionPtr, ControllerType.XBox, ref inputState);

			if(result == Result.DeviceUnavailable)
				Assert.Fail("No XBox controller was available.");

			Assert.IsTrue(result >= Result.Success, "Failed to retrieve the input state of the XBox controller.");
		}

		[TestMethod]
		public void Session_GetConnectedControllerTypes()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			ControllerType controllerType = OVR.GetConnectedControllerTypes(sessionPtr);
			
            // We assert that at least one controller is present
			Assert.IsTrue(controllerType.HasFlag(ControllerType.Remote) || controllerType.HasFlag(ControllerType.XBox) || controllerType.HasFlag(ControllerType.LTouch) || controllerType.HasFlag(ControllerType.RTouch), 
                "No Oculus controller is available - please connect any Oculus controller for this test to pass.");
        }

		[TestMethod]
		public void Session_SetControllerVibration()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			Result result = OVR.SetControllerVibration(sessionPtr, ControllerType.Touch, 0.5f, 0.75f);
			
			// TODO: Enable again when touch controllers are attached.
			//Assert.IsTrue(result >= OVRost.Result.Success, "Failed to set controller vibration on the touch controller.");
		}

		[TestMethod]
		public void Session_GetFovTextureSize()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			FovPort fieldOfView	= new FovPort();
			fieldOfView.DownTan				= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.UpTan				= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.LeftTan				= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			fieldOfView.RightTan			= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.

			Sizei size = OVR.GetFovTextureSize(sessionPtr, EyeType.Left, fieldOfView, 1.0f);
            
            //Assert.AreEqual(1099, size.Width);
            //Assert.AreEqual(635, size.Height);

            //Assert.AreEqual(1586, size.Width);
            //Assert.AreEqual(915, size.Height);

            // The following is reported on 1.17:
            Assert.AreEqual(1600, size.Width);
            Assert.AreEqual(928, size.Height);
        }

		[TestMethod]
		public void Session_GetRenderDesc()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			// Define field of view (This is used for both left and right eye).
			FovPort fieldOfView	= new FovPort();
			fieldOfView.DownTan				= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.UpTan				= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.LeftTan				= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			fieldOfView.RightTan			= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.

			EyeRenderDesc renderDesc = OVR.GetRenderDesc(sessionPtr, EyeType.Left, fieldOfView);
			Assert.AreEqual(renderDesc.Fov, renderDesc.Fov);

            // Test that the GetRenderDesc is returning the correct eye position (or that the workaround is working)
            Assert.AreNotEqual(renderDesc.HmdToEyePose.Position.X, 0.0f);
		}

		[TestMethod]
		public void Session_GetPredictedDisplayTime()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			double frameMidPoint = OVR.GetPredictedDisplayTime(sessionPtr, 0);
			Assert.IsTrue(frameMidPoint >= 0);
		}

		[TestMethod]
		public void GetTimeInSeconds()
		{
			double time = OVR.GetTimeInSeconds();
			Assert.AreNotEqual(0, time);
		}

		[TestMethod]
		public void ovrMatrix4f_Projection()
		{
			// Define field of view (This is used for both left and right eye).
			FovPort fieldOfView = new FovPort();

			fieldOfView.DownTan		= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.UpTan		= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.LeftTan		= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			fieldOfView.RightTan	= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			
			// Create left handed projection matrix.
			Matrix4f matrix = OVR.Matrix4f_Projection(fieldOfView, 0.1f, 1, 0);
			Assert.IsTrue(matrix.M11 != 0);
			Assert.IsTrue(matrix.M22 != 0);
			Assert.IsTrue(matrix.M33 != 0);
		}

		[TestMethod]
		public void TimewarpProjectionDesc_FromProjection()
		{
			// Define field of view (This is used for both left and right eye).
			FovPort fieldOfView = new FovPort();

			fieldOfView.DownTan		= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.UpTan		= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.LeftTan		= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			fieldOfView.RightTan	= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			
			// Create left handed projection matrix, used to create a timewarp projection description.
			Matrix4f matrix = OVR.Matrix4f_Projection(fieldOfView, 0.1f, 1, 0);

			TimewarpProjectionDesc timewarpProjectionDesc = OVR.TimewarpProjectionDesc_FromProjection(matrix, ProjectionModifier.None);
			Assert.IsTrue(timewarpProjectionDesc.Projection22 != 0);
			Assert.IsTrue(timewarpProjectionDesc.Projection23 != 0);
			Assert.IsTrue(timewarpProjectionDesc.Projection32 != 0);
		}

		[TestMethod]
		public void ovrMatrix4f_OrthoSubProjection()
		{
			Matrix4f projectionMatrix = new Matrix4f();
			projectionMatrix.M11	= 1;
			projectionMatrix.M22	= 1;
			projectionMatrix.M33	= 1;
			projectionMatrix.M44	= 1;

			Vector2f vector = new Vector2f();
			vector.X = 1;
			vector.Y = 1;

			Matrix4f matrix = OVR.Matrix4f_OrthoSubProjection(projectionMatrix, vector, 1f, 1f);
			Assert.AreEqual( 1, matrix.M11);
			Assert.AreEqual(-1, matrix.M22);
			Assert.AreEqual( 0, matrix.M33);
			Assert.AreEqual( 1, matrix.M44);
		}

		[TestMethod]
		public void CalcEyePoses()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			var hmdToEyeViewOffset = new Vector3f[2];
			var	eyePoses = new Posef[2];
			var headPose = new Posef();

			hmdToEyeViewOffset[0].X	= -0.1f;
			hmdToEyeViewOffset[1].X	= 0.1f;

			// Define a head position looking forward, from a position where the head is 1.75 meters above the ground.
			headPose.Orientation.W = 1;
			headPose.Position.Y = 1.75f;

            OVR.CalcEyePoses(headPose, hmdToEyeViewOffset, ref eyePoses);

			Assert.AreEqual(headPose.Orientation.X, eyePoses[0].Orientation.X);
			Assert.AreEqual(headPose.Orientation.Y, eyePoses[0].Orientation.Y);
			Assert.AreEqual(headPose.Orientation.Z, eyePoses[0].Orientation.Z);
			Assert.AreEqual(headPose.Orientation.W, eyePoses[0].Orientation.W);
			Assert.AreEqual(hmdToEyeViewOffset[0].X, eyePoses[0].Position.X);
			Assert.AreEqual(headPose.Position.Y, eyePoses[0].Position.Y);
			Assert.AreEqual(0, eyePoses[0].Position.Z);

			Assert.AreEqual(headPose.Orientation.X, eyePoses[1].Orientation.X);
			Assert.AreEqual(headPose.Orientation.Y, eyePoses[1].Orientation.Y);
			Assert.AreEqual(headPose.Orientation.Z, eyePoses[1].Orientation.Z);
			Assert.AreEqual(headPose.Orientation.W, eyePoses[1].Orientation.W);
			Assert.AreEqual(hmdToEyeViewOffset[1].X, eyePoses[1].Position.X);
			Assert.AreEqual(headPose.Position.Y, eyePoses[1].Position.Y);
			Assert.AreEqual(0, eyePoses[1].Position.Z);
		}

		[TestMethod]
		public void Session_GetEyePoses()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			var hmdToEyeViewOffset = new Vector3f[2];
			var poses = new Posef[2];
			double sensorSampleTime;

			hmdToEyeViewOffset[0].X	= -0.1f;
			hmdToEyeViewOffset[1].X	= 0.1f;

            OVR.GetEyePoses(sessionPtr, 0, true, hmdToEyeViewOffset, ref poses, out sensorSampleTime);

			Assert.AreNotEqual(0, poses[0].Orientation.W);
			Assert.AreNotEqual(0, poses[0].Position.X);
			Assert.AreNotEqual(0, poses[0].Position.Y);
			Assert.AreNotEqual(0, poses[0].Position.Z);
			Assert.AreNotEqual(0, poses[1].Orientation.W);
			Assert.AreNotEqual(0, poses[1].Position.X);
			Assert.AreNotEqual(0, poses[1].Position.Y);
			Assert.AreNotEqual(0, poses[1].Position.Z);
			Assert.AreNotEqual(0, hmdToEyeViewOffset[0].X);
			Assert.AreEqual(0, hmdToEyeViewOffset[0].Y);
			Assert.AreEqual(0, hmdToEyeViewOffset[0].Z);
			Assert.AreNotEqual(0, hmdToEyeViewOffset[1].X);
			Assert.AreEqual(0, hmdToEyeViewOffset[1].Y);
			Assert.AreEqual(0, hmdToEyeViewOffset[1].Z);
		}

		[TestMethod]
		public void Session_Posef_FlipHandedness()
		{
			Posef pose			= new Posef();
			Posef resultPose	= new Posef();

			pose.Position		= new Vector3f(1, 1, 1);
			pose.Orientation	= new Quaternionf(1, 1, 1, 1);

			OVR.Posef_FlipHandedness(ref pose, ref resultPose);

			Assert.AreEqual(pose.Position.X, -resultPose.Position.X);
			Assert.AreEqual(pose.Position.Y, resultPose.Position.Y);
			Assert.AreEqual(pose.Position.Z, resultPose.Position.Z);

			Assert.AreEqual(-pose.Orientation.X, resultPose.Orientation.X);
			Assert.AreEqual(pose.Orientation.Y, resultPose.Orientation.Y);
			Assert.AreEqual(pose.Orientation.Z, resultPose.Orientation.Z);
			Assert.AreEqual(-pose.Orientation.W, resultPose.Orientation.W);
		}

		[TestMethod]
		public void Session_GetBool()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

		    string parameterName = "DummyValue";

            bool result = OVR.GetBool(sessionPtr, parameterName, false);
			Assert.AreEqual(false, result); // Check if we get the fallback value

            result = OVR.GetBool(sessionPtr, parameterName, true);
			Assert.AreEqual(true, result);

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }

		[TestMethod]
		public void Session_SetBool()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			bool success = OVR.SetBool(sessionPtr, OvrWrap.OVR_DEBUG_HUD_STEREO_GUIDE_INFO_ENABLE, true);
			Assert.IsTrue(success);
        }

		[TestMethod]
		public void Session_GetInt()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

		    string parameterName = "DummyValue";
            int result = OVR.GetInt(sessionPtr, parameterName, 2);
			Assert.AreEqual(2, result);

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }

		[TestMethod]
		public void Session_SetInt()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

		    string parameterName = "DummyValue";
            bool success = OVR.SetInt(sessionPtr, "DummyValue", 1);
		    Assert.IsFalse(success); // This should not succeed because of invalid parameter name

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }

		[TestMethod]
		public void ovr_GetFloat()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			float result = OVR.GetFloat(sessionPtr, "DummyValue", 2f);
			Assert.AreEqual(2f, result);
		}

		[TestMethod]
		public void Session_SetFloat()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

		    string parameterName = "DummyValue";
            bool success = OVR.SetFloat(sessionPtr, parameterName, 2f);
			Assert.IsFalse(success); // This should not succeed because of invalid parameter name

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }

		[TestMethod]
		public void Session_GetFloatArray()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			float[] values = new float[1];
		    string parameterName = "DummyValue";

            int result = OVR.GetFloatArray(sessionPtr, parameterName, values, values.Length);
			Assert.AreEqual(0, result);

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }

		[TestMethod]
		public void Session_SetFloatArray()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			float[] values = new float[1];
		    string parameterName = "DummyValue";

            bool success = OVR.SetFloatArray(sessionPtr, parameterName, values, values.Length);
			Assert.IsFalse(success);

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }
		
		[TestMethod]
		public void Session_GetString()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

		    string parameterName = "DummyValue";

            string result = OVR.GetString(sessionPtr, parameterName, string.Empty);
			Assert.IsNotNull(result);

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }

		[TestMethod]
		public void Session_SetString()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

		    string parameterName = "DummyValue";

            bool success = OVR.SetString(sessionPtr, parameterName, "New value");
			Assert.IsFalse(success);

		    ErrorInfo errorInfo = OVR.GetLastErrorInfo();

		    Assert.AreEqual(errorInfo.Result, Result.InvalidParameter);
		    Assert.IsTrue(errorInfo.ErrorString.Contains(parameterName)); // ovr_SetFloat: invalid property name "DummyValue".
        }



        [TestMethod]
        public void Session_IdentifyClient()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            Result result = OVR.IdentifyClient("EngineName: OculusWrapTest\nEngineVersion: 1.0\nEngineEditor: false");
            Assert.IsTrue(result >= Result.Success, "Failed to call IdentifyClient");
        }

        [TestMethod]
        public void Session_GetTouchHapticsDesc()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            TouchHapticsDesc touchHapticsDesc = OVR.GetTouchHapticsDesc(sessionPtr, ControllerType.LTouch);

            Assert.IsTrue(touchHapticsDesc.SampleRateHz > 0 &&
                          touchHapticsDesc.QueueMinSizeToAvoidStarvation > 0 &&
                          touchHapticsDesc.SampleSizeInBytes > 0 &&
                          touchHapticsDesc.SubmitMaxSamples > 0 &&
                          touchHapticsDesc.SubmitMinSamples > 0 &&
                          touchHapticsDesc.SubmitOptimalSamples > 0,
                          "Invalid value in TouchHapticsDesc get from GetTouchHapticsDesc");


            touchHapticsDesc = OVR.GetTouchHapticsDesc(sessionPtr, ControllerType.RTouch);

            Assert.IsTrue(touchHapticsDesc.SampleRateHz > 0 &&
                          touchHapticsDesc.QueueMinSizeToAvoidStarvation > 0 &&
                          touchHapticsDesc.SampleSizeInBytes > 0 &&
                          touchHapticsDesc.SubmitMaxSamples > 0 &&
                          touchHapticsDesc.SubmitMinSamples > 0 &&
                          touchHapticsDesc.SubmitOptimalSamples > 0,
                          "Invalid value in TouchHapticsDesc get from GetTouchHapticsDesc");
        }

        [TestMethod]
        public void Session_SubmitControllerVibration()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            byte[] samples = { 0, 64, 128, 255 };
            var gcHandle = GCHandle.Alloc(samples, GCHandleType.Pinned);

            var hapticsBuffer = new HapticsBuffer()
            {
                Samples = gcHandle.AddrOfPinnedObject(),
                SamplesCount = samples.Length,
                SubmitMode = HapticsBufferSubmitMode.Enqueue
            };

            Result result = OVR.SubmitControllerVibration(sessionPtr, ControllerType.LTouch, hapticsBuffer);

            gcHandle.Free();

            Assert.IsTrue(result >= Result.Success, "Failed to SubmitControllerVibration");
        }

        [TestMethod]
        public void Session_GetControllerVibrationState()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var hapticsPlaybackState = new HapticsPlaybackState();

            Result result = OVR.GetControllerVibrationState(sessionPtr, ControllerType.LTouch, ref hapticsPlaybackState);

            if (result == Result.DeviceUnavailable)
                Assert.Fail("Touch controller was available.");

            Assert.IsTrue(result >= Result.Success, "Failed to call GetControllerVibrationState");
        }

        [TestMethod]
        public void Session_TestBoundary()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var boundaryTestResult = new BoundaryTestResult();

            Result result = OVR.TestBoundary(sessionPtr, TrackedDeviceType.HMD, BoundaryType.PlayArea, ref boundaryTestResult);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call TestBoundary");

            if (result != Result.Success_DeviceUnavailable)
            {
                // Check that we get a valid data back (we assume the values bigger than 10 meters are not valid)
                Assert.IsTrue(boundaryTestResult.ClosestDistance > -10 && boundaryTestResult.ClosestDistance < 10, "Invalid ClosestDistance");

                Assert.IsTrue(-10 < boundaryTestResult.ClosestPoint.X && boundaryTestResult.ClosestPoint.X < 10 &&
                              -10 < boundaryTestResult.ClosestPoint.Y && boundaryTestResult.ClosestPoint.Y < 10 &&
                              -10 < boundaryTestResult.ClosestPoint.Z && boundaryTestResult.ClosestPoint.Z < 10,
                              "Invalid ClosestPoint");

                Assert.IsTrue(boundaryTestResult.ClosestPointNormal.X + boundaryTestResult.ClosestPointNormal.Y + boundaryTestResult.ClosestPointNormal.Z < 0.01, // should be 1
                              "Invalid ClosestPointNormal");
            }
        }

        [TestMethod]
        public void Session_TestBoundaryPoint()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var boundaryTestResult = new BoundaryTestResult();

            Result result = OVR.TestBoundaryPoint(sessionPtr, new Vector3f(1, 1, 1), BoundaryType.PlayArea, ref boundaryTestResult);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call TestBoundaryPoint");

            if (result != Result.Success_DeviceUnavailable)
            {
                // Check that we get a valid data back (we assume the values bigger than 10 meters are not valid)
                Assert.IsTrue(boundaryTestResult.ClosestDistance > -10 && boundaryTestResult.ClosestDistance < 10, "Invalid ClosestDistance");

                Assert.IsTrue(-10 < boundaryTestResult.ClosestPoint.X && boundaryTestResult.ClosestPoint.X < 10 &&
                              -10 < boundaryTestResult.ClosestPoint.Y && boundaryTestResult.ClosestPoint.Y < 10 &&
                              -10 < boundaryTestResult.ClosestPoint.Z && boundaryTestResult.ClosestPoint.Z < 10,
                              "Invalid ClosestPoint");

                Assert.IsTrue(boundaryTestResult.ClosestPointNormal.X + boundaryTestResult.ClosestPointNormal.Y + boundaryTestResult.ClosestPointNormal.Z < 0.01, // should be 1
                              "Invalid ClosestPointNormal");
            }
        }

        [TestMethod]
        public void Session_SetBoundaryLookAndFeel()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var boundaryLookAndFeel = new BoundaryLookAndFeel()
            {
                Color = new Colorf(1, 0, 0, 1)
            };

            Result result = OVR.SetBoundaryLookAndFeel(sessionPtr, ref boundaryLookAndFeel);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call SetBoundaryLookAndFeel");
        }

        [TestMethod]
        public void Session_ResetBoundaryLookAndFeel()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            Result result = OVR.ResetBoundaryLookAndFeel(sessionPtr);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call ResetBoundaryLookAndFeel");
        }

        [TestMethod]
        public void Session_GetBoundaryGeometry()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            Vector3f[] points;
            Result result = OVR.GetBoundaryGeometry(sessionPtr, BoundaryType.PlayArea, out points);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call GetBoundaryGeometry");

            Assert.IsTrue(points != null && points.Length > 0, "BoundaryGeometry points array is empty");

            foreach (var onePoint in points)
            {
                Assert.IsTrue(onePoint.X > -10 && onePoint.X < 10 &&
                              onePoint.Y > -10 && onePoint.Y < 10 &&
                              onePoint.Z > -10 && onePoint.Z < 10,
                              "Invalid point in BoundaryGeometry");
            }
        }
        
        [TestMethod]
        public void Session_GetBoundaryDimensions()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            Vector3f dimensions;
            Result result = OVR.GetBoundaryDimensions(sessionPtr, BoundaryType.PlayArea, out dimensions);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call GetBoundaryDimensions");

            Assert.IsTrue(dimensions.X > 0  && dimensions.X < 10 &&
                          dimensions.Y >= 0 && dimensions.Y < 10 &&
                          dimensions.Z > 0  && dimensions.Z < 10,
                          "Invalid BoundaryDimensions");
        }
        
        [TestMethod]
        public void Session_GetBoundaryVisible()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            bool isVisible;
            Result result = OVR.GetBoundaryVisible(sessionPtr, out isVisible);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call GetBoundaryVisible");
        }

        [TestMethod]
        public void Session_RequestBoundaryVisible()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            Result result = OVR.RequestBoundaryVisible(sessionPtr, true);

            Assert.IsFalse(result == Result.Success_BoundaryInvalid, "Boundaries are not set on Oculus. Boundaries require Oculus Touch. Please run a full setup in Oculus settings to set up boundaries before runing the test.");

            Assert.IsTrue(result >= Result.Success, "Failed to call GetBoundaryVisible");
        }

        [TestMethod]
        public void Session_GetPerfStats()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var outStats = new PerfStats();
            Result result = OVR.GetPerfStats(sessionPtr, ref outStats);

            Assert.IsTrue(result >= Result.Success, "Failed to call GetPerfStats");
        }

        [TestMethod]
        public void Session_ResetPerfStats()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            Result result = OVR.ResetPerfStats(sessionPtr);

            Assert.IsTrue(result >= Result.Success, "Failed to call ResetPerfStats");
        }

        [TestMethod]
        public void Session_SpecifyTrackingOrigin()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var posef = new Posef()
            {
                Position = new Vector3f(0, 0, 0),
                Orientation = new Quaternionf(0, 1, 0, 0)
            };

            Result result = OVR.SpecifyTrackingOrigin(sessionPtr, posef);

            Assert.IsTrue(result >= Result.Success, "Failed to call SpecifyTrackingOrigin");
        }

        [TestMethod]
        public void Session_GetDevicePoses()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            var deviceTypes = new TrackedDeviceType[1];
            deviceTypes[0] = TrackedDeviceType.HMD;

            var outDevicePoses = new PoseStatef[1];

            //var deviceTypes = new TrackedDeviceType[3];
            //deviceTypes[0] = TrackedDeviceType.HMD;
            //deviceTypes[1] = TrackedDeviceType.LTouch;
            //deviceTypes[2] = TrackedDeviceType.RTouch;

            //var outDevicePoses = new PoseStatef[3]; 

            Result result = OVR.GetDevicePoses(sessionPtr, deviceTypes, 0, outDevicePoses);

            Assert.IsTrue(result >= Result.Success || result == Result.LostTracking, "Failed to call GetDevicePoses");
        }

        [TestMethod]
        public void Session_GetExternalCameras()
        {
            IntPtr sessionPtr = CreateSession();
            Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            ExternalCamera[] externalCameras;
            Result result = OVR.GetExternalCameras(sessionPtr, out externalCameras);

            Assert.IsTrue(result >= Result.Success || result == Result.NoExternalCameraInfo, "Failed to call GetExternalCameras");
        }

        [TestMethod]
        public void Session_SetExternalCameraProperties()
        {
            // This comment is commented because it creates a new external camera that is persistent after one session ends.
            // In v1.17 there seems to be no way to delete this camera - even when passing IntPtr.Zero to ovr_SetExternalCameraProperties

            //IntPtr sessionPtr = CreateSession();
            //Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

            //var cameraExtrinsics = new CameraExtrinsics()
            //{
            //    RelativePose = new Posef() {Orientation = new Quaternionf(0, 1, 0, 45)}
            //};

            //var cameraIntrinsics = new CameraIntrinsics()
            //{
            //    VirtualFarPlaneDistanceMeters = 10f,
            //    VirtualNearPlaneDistanceMeters = 0.125f
            //};

            //Result result = OVR.SetExternalCameraProperties(sessionPtr, "OculusWrapTestCamera", ref cameraIntrinsics, ref cameraExtrinsics);

            //Assert.IsTrue(result >= Result.Success, "Failed to call SetExternalCameraProperties");


            //ExternalCamera[] externalCameras;
            //result = OVR.GetExternalCameras(sessionPtr, out externalCameras);

            //Assert.IsTrue(result >= Result.Success, "Failed to call GetExternalCameras");

            //Assert.IsTrue(externalCameras != null && externalCameras.Length == 1);

            //Assert.AreEqual(externalCameras[0].Extrinsics.RelativePose.Orientation.Y, 1f);
            //Assert.AreEqual(externalCameras[0].Extrinsics.RelativePose.Orientation.W, 45f);
            //Assert.AreEqual(externalCameras[0].Intrinsics.VirtualNearPlaneDistanceMeters, 0.125f);
            //Assert.AreEqual(externalCameras[0].Intrinsics.VirtualFarPlaneDistanceMeters, 10f);
        }
    }
}
