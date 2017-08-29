using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDX.Direct3D11;

namespace Ab3d.OculusWrap.UnitTests
{
	[TestClass]
	public class OvrWarpDX11Test
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

		/// <summary>
		/// Creates a TextureSwapChainDesc, based on a specified SharpDX texture description.
		/// </summary>
		/// <param name="texture2DDescription">SharpDX texture description.</param>
		/// <returns>TextureSwapChainDesc, based on the SharpDX texture description.</returns>
		private TextureSwapChainDesc CreateTextureSwapChainDescription(Texture2DDescription texture2DDescription)
		{
			TextureSwapChainDesc textureSwapChainDescription = new TextureSwapChainDesc();
			textureSwapChainDescription.Type				 = TextureType.Texture2D;
			textureSwapChainDescription.Format				 = GetTextureFormat(texture2DDescription.Format);
			textureSwapChainDescription.ArraySize			 = texture2DDescription.ArraySize;
			textureSwapChainDescription.Width				 = texture2DDescription.Width;
			textureSwapChainDescription.Height				 = texture2DDescription.Height;
			textureSwapChainDescription.MipLevels			 = texture2DDescription.MipLevels;
			textureSwapChainDescription.SampleCount			 = texture2DDescription.SampleDescription.Count;
			textureSwapChainDescription.StaticImage			 = false;
			textureSwapChainDescription.MiscFlags			 = GetTextureMiscFlags(texture2DDescription, false);
			textureSwapChainDescription.BindFlags			 = GetTextureBindFlags(texture2DDescription.BindFlags);

			return textureSwapChainDescription;
		}

		/// <summary>
		/// Translates a DirectX texture format into an Oculus SDK texture format.
		/// </summary>
		/// <param name="textureFormat">DirectX texture format to translate into an Oculus SDK texture format.</param>
		/// <returns>
		/// Oculus SDK texture format matching the specified textureFormat or TextureFormat.Unknown if a match count not be found.
		/// </returns>
		private TextureFormat GetTextureFormat(SharpDX.DXGI.Format textureFormat)
		{
			switch(textureFormat)
			{
				case SharpDX.DXGI.Format.B5G6R5_UNorm:			return TextureFormat.B5G6R5_UNorm;
				case SharpDX.DXGI.Format.B5G5R5A1_UNorm:		return TextureFormat.B5G5R5A1_UNorm;
				case SharpDX.DXGI.Format.R8G8B8A8_UNorm:		return TextureFormat.R8G8B8A8_UNorm;
				case SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb:	return TextureFormat.R8G8B8A8_UNorm_SRgb;
				case SharpDX.DXGI.Format.B8G8R8A8_UNorm:		return TextureFormat.B8G8R8A8_UNorm;
				case SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb:	return TextureFormat.B8G8R8A8_UNorm_SRgb;
				case SharpDX.DXGI.Format.B8G8R8X8_UNorm:		return TextureFormat.B8G8R8X8_UNorm;
				case SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb:	return TextureFormat.B8G8R8X8_UNorm_SRgb;
				case SharpDX.DXGI.Format.R16G16B16A16_Float:	return TextureFormat.R16G16B16A16_Float;
				case SharpDX.DXGI.Format.D16_UNorm:				return TextureFormat.D16_UNorm;
				case SharpDX.DXGI.Format.D24_UNorm_S8_UInt:		return TextureFormat.D24_UNorm_S8_UInt;
				case SharpDX.DXGI.Format.D32_Float:				return TextureFormat.D32_Float;
				case SharpDX.DXGI.Format.D32_Float_S8X24_UInt:	return TextureFormat.D32_Float_S8X24_UInt;

				case SharpDX.DXGI.Format.R8G8B8A8_Typeless:		return TextureFormat.R8G8B8A8_UNorm;
				case SharpDX.DXGI.Format.R16G16B16A16_Typeless:	return TextureFormat.R16G16B16A16_Float;
	
				default:										return TextureFormat.Unknown;
			}
		}

		/// <summary>
		/// Creates a set of TextureMiscFlags, based on a specified SharpDX texture description and a mip map generation flag.
		/// </summary>
		/// <param name="texture2DDescription">SharpDX texture description.</param>
		/// <param name="allowGenerateMips">
		/// When set, allows generation of the mip chain on the GPU via the GenerateMips
		/// call. This flag requires that RenderTarget binding also be specified.
		/// </param>
		/// <returns>Created TextureMiscFlags, based on the specified SharpDX texture description and mip map generation flag.</returns>
		private TextureMiscFlags GetTextureMiscFlags(Texture2DDescription texture2DDescription, bool allowGenerateMips)
		{
			TextureMiscFlags results = TextureMiscFlags.None;

			if(texture2DDescription.Format == SharpDX.DXGI.Format.R8G8B8A8_Typeless || texture2DDescription.Format == SharpDX.DXGI.Format.R16G16B16A16_Typeless)
				results |= TextureMiscFlags.DX_Typeless;

			if(texture2DDescription.BindFlags.HasFlag(BindFlags.RenderTarget) && allowGenerateMips)
				results |= TextureMiscFlags.AllowGenerateMips;

			return results;
		}

		/// <summary>
		/// Retrieves a list of flags matching the specified DirectX texture binding flags.
		/// </summary>
		/// <param name="bindFlags">DirectX texture binding flags to translate into Oculus SDK texture binding flags.</param>
		/// <returns>Oculus SDK texture binding flags matching the specified bindFlags.</returns>
		private TextureBindFlags GetTextureBindFlags(SharpDX.Direct3D11.BindFlags bindFlags)
		{
			TextureBindFlags result = TextureBindFlags.None;

			if(bindFlags.HasFlag(SharpDX.Direct3D11.BindFlags.DepthStencil))
				result |= TextureBindFlags.DX_DepthStencil;

			if(bindFlags.HasFlag(SharpDX.Direct3D11.BindFlags.RenderTarget))
				result |= TextureBindFlags.DX_RenderTarget;

			if(bindFlags.HasFlag(SharpDX.Direct3D11.BindFlags.UnorderedAccess))
				result |= TextureBindFlags.DX_DepthStencil;

			return result;
		}

		/// <summary>
		/// Creates a simple Direct3D graphics engine, used in unit tests.
		/// </summary>
		/// <param name="session">Existing session used to retrieve the size of the test engine.</param>
		/// <returns>Created test engine.</returns>
		/// <remarks>Remember to dispose the created test engine, after use.</remarks>
		private TestEngine CreateTestEngine(IntPtr session)
		{
			// Define field of view (This is used for both left and right eye).
			FovPort fieldOfView = new FovPort();
			fieldOfView.DownTan		= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.UpTan		= (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.LeftTan		= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			fieldOfView.RightTan	= (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.

			EyeRenderDesc renderDescLeft	= OVR.GetRenderDesc(session, EyeType.Left, fieldOfView);
			EyeRenderDesc renderDescRight	= OVR.GetRenderDesc(session, EyeType.Left, fieldOfView);

			// Determine texture size matching the field of view.
			Sizei sizeLeft		= OVR.GetFovTextureSize(session, EyeType.Left, fieldOfView, 1.0f);
			Sizei sizeRight	= OVR.GetFovTextureSize(session, EyeType.Right, fieldOfView, 1.0f);

			TestEngine testEngine = new TestEngine(sizeLeft.Width+sizeRight.Width, sizeLeft.Height);
			return testEngine;
		}

		/// <summary>
		/// Create a texture swap chain, used in unit tests.
		/// </summary>
		/// <param name="session">Existing session used to create the texture swap chain.</param>
		/// <param name="testEngine">Existing test engine, containing the Direct3D device and texture description to use.</param>
		/// <returns>Created texture swap chain pointer.</returns>
		private IntPtr CreateTextureSwapChain(IntPtr session, TestEngine testEngine)
		{
			// Convert the SharpDX texture description to the native Direct3D texture description.
			TextureSwapChainDesc textureSwapChainDescription = CreateTextureSwapChainDescription(testEngine.Texture2DDescription);

			IntPtr textureSwapChainPtr;
			Result result = OVR.CreateTextureSwapChainDX(session, testEngine.Device.NativePointer, ref textureSwapChainDescription, out textureSwapChainPtr);
			Assert.AreEqual(result, Result.Success);

			return textureSwapChainPtr;
		}

		/// <summary>
		/// Create a mirror texture, used in unit tests.
		/// </summary>
		/// <param name="session">Existing session used to create the mirror texture.</param>
		/// <param name="testEngine">Existing test engine, containing the Direct3D device and texture size to request.</param>
		/// <returns>Created mirror texture pointer.</returns>
		private IntPtr CreateMirrorTexture(IntPtr session, TestEngine testEngine)
		{
			MirrorTextureDesc mirrorTextureDescription = new MirrorTextureDesc();
			mirrorTextureDescription.Format			   = TextureFormat.R8G8B8A8_UNorm_SRgb;
			mirrorTextureDescription.Width			   = testEngine.Window.Width;
			mirrorTextureDescription.Height			   = testEngine.Window.Height;
			mirrorTextureDescription.MiscFlags		   = TextureMiscFlags.None;

			IntPtr mirrorTexturePtr;
			Result result = OVR.CreateMirrorTextureDX(session, testEngine.Device.NativePointer, ref mirrorTextureDescription, out mirrorTexturePtr);
			Assert.IsTrue(result >= Result.Success);
			Assert.AreNotEqual(IntPtr.Zero, mirrorTexturePtr);

			return mirrorTexturePtr;
		}

		[TestMethod]
		public void Session_CreateTextureSwapChainDX()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				// Convert the SharpDX texture description to the native Direct3D texture description.
				TextureSwapChainDesc textureSwapChainDescription = CreateTextureSwapChainDescription(testEngine.Texture2DDescription);

				IntPtr textureSwapChainPtr = IntPtr.Zero;

				try
				{
					Result result = OVR.CreateTextureSwapChainDX(session, testEngine.Device.NativePointer, ref textureSwapChainDescription, out textureSwapChainPtr);
					Assert.IsTrue(result >= Result.Success);
					Assert.AreNotEqual(IntPtr.Zero, textureSwapChainPtr);
				}
				finally
				{
					if(textureSwapChainPtr != IntPtr.Zero)
						OVR.DestroyTextureSwapChain(session, textureSwapChainPtr);
				}

			}
		}

		[TestMethod]
		public void Session_GetTextureSwapChainBufferDX()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr textureSwapChainPtr = IntPtr.Zero;

				try
				{
					textureSwapChainPtr = CreateTextureSwapChain(session, testEngine);

					Guid	textureInterfaceId	= new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
					IntPtr	textureComPtr;
					Result result = OVR.GetTextureSwapChainBufferDX(session, textureSwapChainPtr, 0, textureInterfaceId, out textureComPtr);
					Assert.IsTrue(result >= Result.Success);
					Assert.AreNotEqual(IntPtr.Zero, textureSwapChainPtr);

					Texture2D texture2D = new Texture2D(textureComPtr);
					Assert.IsNotNull(texture2D);
					texture2D.Dispose();
				}
				finally
				{
					if(textureSwapChainPtr != IntPtr.Zero)
						OVR.DestroyTextureSwapChain(session, textureSwapChainPtr);
				}
			}
		}

		[TestMethod]
		public void Session_DestroyTextureSwapChainDX()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr textureSwapChainPtr = CreateTextureSwapChain(session, testEngine);
			
				OVR.DestroyTextureSwapChain(session, textureSwapChainPtr);
			}
		}

		[TestMethod]
		public void Session_GetTextureSwapChainLength()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr textureSwapChainPtr = IntPtr.Zero;

				try
				{
					textureSwapChainPtr = CreateTextureSwapChain(session, testEngine);
			
					int length;
					Result result = OVR.GetTextureSwapChainLength(session, textureSwapChainPtr, out length);
					Assert.IsTrue(result >= Result.Success);
					Assert.IsTrue(length > 0);
				}
				finally
				{
					if(textureSwapChainPtr != IntPtr.Zero)
						OVR.DestroyTextureSwapChain(session, textureSwapChainPtr);
				}
			}
		}

		[TestMethod]
		public void Session_GetTextureSwapChainCurrentIndex()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr textureSwapChainPtr = IntPtr.Zero;

				try
				{
					textureSwapChainPtr = CreateTextureSwapChain(session, testEngine);
			
					int index;
					Result result = OVR.GetTextureSwapChainCurrentIndex(session, textureSwapChainPtr, out index);
					Assert.IsTrue(result >= Result.Success);
					Assert.IsTrue(index >= 0);
					Assert.IsTrue(index < 99);
				}
				finally
				{
					if(textureSwapChainPtr != IntPtr.Zero)
						OVR.DestroyTextureSwapChain(session, textureSwapChainPtr);
				}
			}
		}

		[TestMethod]
		public void Session_GetTextureSwapChainDesc()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				// Convert the SharpDX texture description to the native Direct3D texture description.
				TextureSwapChainDesc textureSwapChainDescription = CreateTextureSwapChainDescription(testEngine.Texture2DDescription);

				IntPtr textureSwapChainPtr;
				Result result = OVR.CreateTextureSwapChainDX(session, testEngine.Device.NativePointer, ref textureSwapChainDescription, out textureSwapChainPtr);
			
				TextureSwapChainDesc textureSwapChainDescriptionResult = new TextureSwapChainDesc();
				result = OVR.GetTextureSwapChainDesc(session, textureSwapChainPtr, ref textureSwapChainDescriptionResult);
				Assert.IsTrue(result >= Result.Success);

				Assert.AreEqual(textureSwapChainDescription.ArraySize,		textureSwapChainDescriptionResult.ArraySize);
				Assert.AreEqual(textureSwapChainDescription.BindFlags,		textureSwapChainDescriptionResult.BindFlags);
				Assert.AreEqual(textureSwapChainDescription.Format,			textureSwapChainDescriptionResult.Format);
				Assert.AreEqual(textureSwapChainDescription.Height,			textureSwapChainDescriptionResult.Height);
				Assert.AreEqual(textureSwapChainDescription.MipLevels,		textureSwapChainDescriptionResult.MipLevels);
				Assert.AreEqual(textureSwapChainDescription.MiscFlags,		textureSwapChainDescriptionResult.MiscFlags);
				Assert.AreEqual(textureSwapChainDescription.SampleCount,	textureSwapChainDescriptionResult.SampleCount);
				Assert.AreEqual(textureSwapChainDescription.StaticImage,	textureSwapChainDescriptionResult.StaticImage);
				Assert.AreEqual(textureSwapChainDescription.Type,			textureSwapChainDescriptionResult.Type);
				Assert.AreEqual(textureSwapChainDescription.Width,			textureSwapChainDescriptionResult.Width);
			}
		}

		[TestMethod]
		public void Session_CommitTextureSwapChain()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr textureSwapChainPtr = IntPtr.Zero;

				try
				{
					textureSwapChainPtr = CreateTextureSwapChain(session, testEngine);
			
					Result result = OVR.CommitTextureSwapChain(session, textureSwapChainPtr);
					Assert.IsTrue(result >= Result.Success);
				}
				finally
				{
					if(textureSwapChainPtr != IntPtr.Zero)
						OVR.DestroyTextureSwapChain(session, textureSwapChainPtr);
				}
			}
		}

		[TestMethod]
		public void Session_CreateMirrorTextureDX()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				MirrorTextureDesc mirrorTextureDescription	= new MirrorTextureDesc();
				mirrorTextureDescription.Format						= TextureFormat.R8G8B8A8_UNorm_SRgb;
				mirrorTextureDescription.Width						= testEngine.Window.Width;
				mirrorTextureDescription.Height						= testEngine.Window.Height;
				mirrorTextureDescription.MiscFlags					= TextureMiscFlags.None;

				IntPtr mirrorTexturePtr;
				Result result = OVR.CreateMirrorTextureDX(session, testEngine.Device.NativePointer, ref mirrorTextureDescription, out mirrorTexturePtr);
				Assert.IsTrue(result >= Result.Success);
				Assert.AreNotEqual(IntPtr.Zero, mirrorTexturePtr);
			}
		}

		[TestMethod]
		public void Session_DestroyMirrorTextureDX()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr mirrorTexturePtr = CreateMirrorTexture(session, testEngine);

				OVR.DestroyMirrorTexture(session, mirrorTexturePtr);
			}
		}

		[TestMethod]
		public void Session_GetMirrorTextureBufferDX()
		{
			IntPtr session	= CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, session);

			using(TestEngine testEngine = CreateTestEngine(session))
			{
				IntPtr mirrorTexturePtr = IntPtr.Zero;

				try
				{
					mirrorTexturePtr = CreateMirrorTexture(session, testEngine);

					Guid	textureInterfaceId	= new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
					IntPtr	textureComPtr;
					Result result = OVR.GetMirrorTextureBufferDX(session, mirrorTexturePtr, textureInterfaceId, out textureComPtr);
					Assert.IsTrue(result >= Result.Success);
					Assert.AreNotEqual(IntPtr.Zero, mirrorTexturePtr);

					Texture2D texture2D = new Texture2D(textureComPtr);
					Assert.IsNotNull(texture2D);
					texture2D.Dispose();
				}
				finally
				{
					if(mirrorTexturePtr != IntPtr.Zero)
						OVR.DestroyMirrorTexture(session, mirrorTexturePtr);
				}
			}
		}       

		[TestMethod]
		public void Session_SubmitFrame()
		{
			IntPtr sessionPtr = CreateSession();
			Assert.AreNotEqual(IntPtr.Zero, sessionPtr);

			// Define field of view (This is used for both left and right eye).
			FovPort fieldOfView	= new FovPort();
			fieldOfView.DownTan	 = (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.UpTan	 = (float) Math.Tan(0.523598776); // 0.523598776 radians = 30 degrees.
			fieldOfView.LeftTan	 = (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.
			fieldOfView.RightTan = (float) Math.Tan(0.785398163); // 0.785398163 radians = 45 degrees.

			EyeRenderDesc renderDescLeft  = OVR.GetRenderDesc(sessionPtr, EyeType.Left, fieldOfView);
			EyeRenderDesc renderDescRight = OVR.GetRenderDesc(sessionPtr, EyeType.Left, fieldOfView);

			var viewScaleDesc = new ViewScaleDesc();
		    viewScaleDesc.HmdToEyePose0 = renderDescLeft.HmdToEyePose;
            viewScaleDesc.HmdToEyePose1 = renderDescRight.HmdToEyePose;
			viewScaleDesc.HmdSpaceToWorldScaleInMeters = 1;

			// Determine texture size matching the field of view.
			Sizei sizeLeft  = OVR.GetFovTextureSize(sessionPtr, EyeType.Left, fieldOfView, 1.0f);
			Sizei sizeRight	= OVR.GetFovTextureSize(sessionPtr, EyeType.Right, fieldOfView, 1.0f);

			var hmdToEyeViewOffset	= new Vector3f[2];
			var poses = new Posef[2];
		    double sensorSampleTime;

			hmdToEyeViewOffset[0].X	= -0.1f;
			hmdToEyeViewOffset[1].X	= 0.1f;

			OVR.GetEyePoses(sessionPtr, 0, true, hmdToEyeViewOffset, ref poses, out sensorSampleTime);

			// Create a set of layers to submit.
			LayerEyeFov	layer	= new LayerEyeFov();
            layer.Header.Type = LayerType.EyeFov;

            Result result;

			using(TestEngine testEngine = CreateTestEngine(sessionPtr))
			{
				try
				{
					// Create a texture for the left eye.
					layer.ColorTextureLeft		 = CreateTextureSwapChain(sessionPtr, testEngine);
					layer.ViewportLeft.Position  = new Vector2i(0, 0);
					layer.ViewportLeft.Size	     = sizeLeft;
					layer.FovLeft                = fieldOfView;
					layer.RenderPoseLeft         = poses[0];

                    // Create a texture for the right eye.
					layer.ColorTextureRight		 = CreateTextureSwapChain(sessionPtr, testEngine);
					layer.ViewportRight.Position = new Vector2i(0, 0);
					layer.ViewportRight.Size	 = sizeLeft;
					layer.FovRight               = fieldOfView;
                    layer.RenderPoseRight        = poses[1];


                    // The created texture swap chain must be committed to the Oculus SDK, before using it in the
                    // call to ovr_SubmitFrame, otherwise ovr_SubmitFrame will fail.
                    result = OVR.CommitTextureSwapChain(sessionPtr, layer.ColorTextureLeft);
                    Assert.IsTrue(result >= Result.Success);

                    result = OVR.CommitTextureSwapChain(sessionPtr, layer.ColorTextureRight);
					Assert.IsTrue(result >= Result.Success);


                    // SubmitFrame requires pointer to an array of pointers to Layer objects
                    var layerPointers = new IntPtr[1];

                    GCHandle layerHandle = GCHandle.Alloc(layer, GCHandleType.Pinned);
                    GCHandle layerPointersHandle = GCHandle.Alloc(layerPointers, GCHandleType.Pinned);

				    layerPointers[0] = layerHandle.AddrOfPinnedObject();

                    result = OVR.SubmitFrame(sessionPtr, 0L, IntPtr.Zero, layerPointersHandle.AddrOfPinnedObject(), 1);
					Assert.IsTrue(result >= Result.Success);

                    layerPointersHandle.Free();
                    layerHandle.Free();
                }
				finally
				{
                    if (layer.ColorTextureLeft != IntPtr.Zero)
                        OVR.DestroyTextureSwapChain(sessionPtr, layer.ColorTextureLeft);

                    if (layer.ColorTextureRight != IntPtr.Zero)
                        OVR.DestroyTextureSwapChain(sessionPtr, layer.ColorTextureRight);
				}
			}
		}
	}
}

