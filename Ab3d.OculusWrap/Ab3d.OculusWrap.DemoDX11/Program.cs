using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ab3d.OculusWrap;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using ovrSession = System.IntPtr;
using ovrTextureSwapChain = System.IntPtr;
using ovrMirrorTexture = System.IntPtr;
using Result = Ab3d.OculusWrap.Result;

namespace Ab3d.OculusWrap.DemoDX11
{
    /// <summary>
    /// SharpDX MiniCube Direct3D 11 Sample
    /// </summary>
    static class Program
    {
        private static void Main()
        {
			RenderForm form = new RenderForm("OculusWrap SharpDX demo");

            IntPtr sessionPtr;
			InputLayout				inputLayout					= null;
			Buffer					contantBuffer				= null;
			Buffer					vertexBuffer				= null;
			ShaderSignature			shaderSignature				= null;
			PixelShader				pixelShader					= null;
			ShaderBytecode			pixelShaderByteCode			= null;
			VertexShader			vertexShader				= null;
			ShaderBytecode			vertexShaderByteCode		= null;
			Texture2D				mirrorTextureD3D			= null;
			EyeTexture[]			eyeTextures					= null;
			DeviceContext			immediateContext			= null;
			DepthStencilState		depthStencilState			= null;
			DepthStencilView		depthStencilView			= null;
			Texture2D				depthBuffer					= null;
			RenderTargetView		backBufferRenderTargetView	= null;
			Texture2D				backBuffer					= null;
			SharpDX.DXGI.SwapChain	swapChain					= null;
			Factory					factory						= null;
			MirrorTexture			mirrorTexture				= null;
			Guid					textureInterfaceId			= new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c"); // Interface ID of the Direct3D Texture2D interface.

            Result result;

            OvrWrap OVR = OvrWrap.Create();

            // Define initialization parameters with debug flag.
            InitParams initializationParameters = new InitParams();
			initializationParameters.Flags = InitFlags.Debug | InitFlags.RequestVersion;
            initializationParameters.RequestedMinorVersion = 17;

            // Initialize the Oculus runtime.
            string errorReason = null;
            try
            {
                result = OVR.Initialize(initializationParameters);

                if (result < Result.Success)
                    errorReason = result.ToString();
            }
            catch (Exception ex)
            {
                errorReason = ex.Message;
            }

			if (errorReason != null)
			{
				MessageBox.Show("Failed to initialize the Oculus runtime library:\r\n" + errorReason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

            // Use the head mounted display.
            sessionPtr = IntPtr.Zero;
            var graphicsLuid = new GraphicsLuid();
            result = OVR.Create(ref sessionPtr, ref graphicsLuid);
            if (result < Result.Success)
            { 
				MessageBox.Show("The HMD is not enabled: " + result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

            var hmdDesc = OVR.GetHmdDesc(sessionPtr);


            try
			{
				// Create a set of layers to submit.
				eyeTextures = new EyeTexture[2];

				// Create DirectX drawing device.
				SharpDX.Direct3D11.Device device = new Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug);

                // Create DirectX Graphics Interface factory, used to create the swap chain.
                factory = new SharpDX.DXGI.Factory4();

				immediateContext = device.ImmediateContext;

				// Define the properties of the swap chain.
				SwapChainDescription swapChainDescription						= new SwapChainDescription();
				swapChainDescription.BufferCount								= 1;
				swapChainDescription.IsWindowed									= true;
				swapChainDescription.OutputHandle								= form.Handle;
				swapChainDescription.SampleDescription							= new SampleDescription(1, 0);
				swapChainDescription.Usage										= Usage.RenderTargetOutput | Usage.ShaderInput;
				swapChainDescription.SwapEffect									= SwapEffect.Sequential;
				swapChainDescription.Flags										= SwapChainFlags.AllowModeSwitch;
				swapChainDescription.ModeDescription.Width						= form.Width;
				swapChainDescription.ModeDescription.Height						= form.Height;
				swapChainDescription.ModeDescription.Format						= Format.R8G8B8A8_UNorm;
				swapChainDescription.ModeDescription.RefreshRate.Numerator		= 0;
				swapChainDescription.ModeDescription.RefreshRate.Denominator	= 1;

                // Create the swap chain.
                swapChain = new SwapChain(factory, device, swapChainDescription);

				// Retrieve the back buffer of the swap chain.
				backBuffer					= swapChain.GetBackBuffer<Texture2D>(0);	
				backBufferRenderTargetView	= new RenderTargetView(device, backBuffer);	

				// Create a depth buffer, using the same width and height as the back buffer.
				Texture2DDescription depthBufferDescription = new Texture2DDescription();
				depthBufferDescription.Format				= Format.D32_Float;
				depthBufferDescription.ArraySize			= 1;
				depthBufferDescription.MipLevels			= 1;
				depthBufferDescription.Width				= form.Width;
				depthBufferDescription.Height				= form.Height;
				depthBufferDescription.SampleDescription	= new SampleDescription(1, 0);
				depthBufferDescription.Usage				= ResourceUsage.Default;
				depthBufferDescription.BindFlags			= BindFlags.DepthStencil;
				depthBufferDescription.CpuAccessFlags		= CpuAccessFlags.None;
				depthBufferDescription.OptionFlags			= ResourceOptionFlags.None;

				// Define how the depth buffer will be used to filter out objects, based on their distance from the viewer.
				DepthStencilStateDescription depthStencilStateDescription	= new DepthStencilStateDescription();
				depthStencilStateDescription.IsDepthEnabled					= true;
				depthStencilStateDescription.DepthComparison				= Comparison.Less;
				depthStencilStateDescription.DepthWriteMask					= DepthWriteMask.Zero;

				// Create the depth buffer.
				depthBuffer		  = new Texture2D(device, depthBufferDescription);
				depthStencilView  = new DepthStencilView(device, depthBuffer);	
				depthStencilState = new DepthStencilState(device, depthStencilStateDescription);

				var viewport = new Viewport(0, 0, hmdDesc.Resolution.Width, hmdDesc.Resolution.Height, 0.0f, 1.0f);

				immediateContext.OutputMerger.SetDepthStencilState(depthStencilState);
				immediateContext.OutputMerger.SetRenderTargets(depthStencilView, backBufferRenderTargetView);
				immediateContext.Rasterizer.SetViewport(viewport);

				// Retrieve the DXGI device, in order to set the maximum frame latency.
				using(SharpDX.DXGI.Device1 dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device1>())
				{
					dxgiDevice.MaximumFrameLatency = 1;
				}

			    var layerEyeFov = new LayerEyeFov();
                layerEyeFov.Header.Type = LayerType.EyeFov;
                layerEyeFov.Header.Flags = LayerFlags.None;

                for (int eyeIndex=0; eyeIndex<2; eyeIndex++)
				{
					EyeType eye = (EyeType)eyeIndex;
					var eyeTexture = new EyeTexture();
					eyeTextures[eyeIndex] = eyeTexture;

					// Retrieve size and position of the texture for the current eye.
					eyeTexture.FieldOfView				= hmdDesc.DefaultEyeFov[eyeIndex];
					eyeTexture.TextureSize				= OVR.GetFovTextureSize(sessionPtr, eye, hmdDesc.DefaultEyeFov[eyeIndex], 1.0f);
				    eyeTexture.RenderDescription        = OVR.GetRenderDesc(sessionPtr, eye, hmdDesc.DefaultEyeFov[eyeIndex]);
                    eyeTexture.HmdToEyeViewOffset		= eyeTexture.RenderDescription.HmdToEyePose.Position;
					eyeTexture.ViewportSize.Position	= new Vector2i(0, 0);
					eyeTexture.ViewportSize.Size		= eyeTexture.TextureSize;
					eyeTexture.Viewport					= new Viewport(0, 0, eyeTexture.TextureSize.Width, eyeTexture.TextureSize.Height, 0.0f, 1.0f);

					// Define a texture at the size recommended for the eye texture.
					eyeTexture.Texture2DDescription						= new Texture2DDescription();
					eyeTexture.Texture2DDescription.Width				= eyeTexture.TextureSize.Width;
					eyeTexture.Texture2DDescription.Height				= eyeTexture.TextureSize.Height;
					eyeTexture.Texture2DDescription.ArraySize			= 1;
					eyeTexture.Texture2DDescription.MipLevels			= 1;
					eyeTexture.Texture2DDescription.Format				= Format.R8G8B8A8_UNorm;
					eyeTexture.Texture2DDescription.SampleDescription	= new SampleDescription(1, 0);
					eyeTexture.Texture2DDescription.Usage				= ResourceUsage.Default;
					eyeTexture.Texture2DDescription.CpuAccessFlags		= CpuAccessFlags.None;
					eyeTexture.Texture2DDescription.BindFlags			= BindFlags.ShaderResource | BindFlags.RenderTarget;

					// Convert the SharpDX texture description to the Oculus texture swap chain description.
					TextureSwapChainDesc textureSwapChainDesc = SharpDXHelpers.CreateTextureSwapChainDescription(eyeTexture.Texture2DDescription);

                    // Create a texture swap chain, which will contain the textures to render to, for the current eye.
				    IntPtr textureSwapChainPtr;

                    result = OVR.CreateTextureSwapChainDX(sessionPtr, device.NativePointer, ref textureSwapChainDesc, out textureSwapChainPtr);
					WriteErrorDetails(OVR, result, "Failed to create swap chain.");

                    eyeTexture.SwapTextureSet = new TextureSwapChain(OVR, sessionPtr, textureSwapChainPtr);


                    // Retrieve the number of buffers of the created swap chain.
                    int textureSwapChainBufferCount;
					result = eyeTexture.SwapTextureSet.GetLength(out textureSwapChainBufferCount);
					WriteErrorDetails(OVR, result, "Failed to retrieve the number of buffers of the created swap chain.");

					// Create room for each DirectX texture in the SwapTextureSet.
					eyeTexture.Textures				= new Texture2D[textureSwapChainBufferCount];
					eyeTexture.RenderTargetViews	= new RenderTargetView[textureSwapChainBufferCount];

					// Create a texture 2D and a render target view, for each unmanaged texture contained in the SwapTextureSet.
					for (int textureIndex=0; textureIndex<textureSwapChainBufferCount; textureIndex++)
					{
						// Retrieve the Direct3D texture contained in the Oculus TextureSwapChainBuffer.
						IntPtr	swapChainTextureComPtr = IntPtr.Zero;
						result = eyeTexture.SwapTextureSet.GetBufferDX(textureIndex, textureInterfaceId, out swapChainTextureComPtr);
						WriteErrorDetails(OVR, result, "Failed to retrieve a texture from the created swap chain.");

						// Create a managed Texture2D, based on the unmanaged texture pointer.
						eyeTexture.Textures[textureIndex] = new Texture2D(swapChainTextureComPtr);

						// Create a render target view for the current Texture2D.
						eyeTexture.RenderTargetViews[textureIndex]	= new RenderTargetView(device, eyeTexture.Textures[textureIndex]);
					}

					// Define the depth buffer, at the size recommended for the eye texture.
					eyeTexture.DepthBufferDescription					= new Texture2DDescription();
					eyeTexture.DepthBufferDescription.Format			= Format.D32_Float;
					eyeTexture.DepthBufferDescription.Width				= eyeTexture.TextureSize.Width;
					eyeTexture.DepthBufferDescription.Height			= eyeTexture.TextureSize.Height;
					eyeTexture.DepthBufferDescription.ArraySize			= 1;
					eyeTexture.DepthBufferDescription.MipLevels			= 1;
					eyeTexture.DepthBufferDescription.SampleDescription	= new SampleDescription(1, 0);
					eyeTexture.DepthBufferDescription.Usage				= ResourceUsage.Default;
					eyeTexture.DepthBufferDescription.BindFlags			= BindFlags.DepthStencil;
					eyeTexture.DepthBufferDescription.CpuAccessFlags	= CpuAccessFlags.None;
					eyeTexture.DepthBufferDescription.OptionFlags		= ResourceOptionFlags.None;

					// Create the depth buffer.
					eyeTexture.DepthBuffer		= new Texture2D(device, eyeTexture.DepthBufferDescription);
					eyeTexture.DepthStencilView	= new DepthStencilView(device, eyeTexture.DepthBuffer);

					// Specify the texture to show on the HMD.
				    if (eyeIndex == 0)
				    {
				        layerEyeFov.ColorTextureLeft = eyeTexture.SwapTextureSet.TextureSwapChainPtr;
				        layerEyeFov.ViewportLeft.Position = new Vector2i(0, 0);
				        layerEyeFov.ViewportLeft.Size = eyeTexture.TextureSize;
				        layerEyeFov.FovLeft = eyeTexture.FieldOfView;
				    }
				    else
				    {
                        layerEyeFov.ColorTextureRight = eyeTexture.SwapTextureSet.TextureSwapChainPtr;
                        layerEyeFov.ViewportRight.Position = new Vector2i(0, 0);
                        layerEyeFov.ViewportRight.Size = eyeTexture.TextureSize;
                        layerEyeFov.FovRight = eyeTexture.FieldOfView;
                    }
				}

				MirrorTextureDesc mirrorTextureDescription	= new MirrorTextureDesc();
				mirrorTextureDescription.Format					= TextureFormat.R8G8B8A8_UNorm_SRgb;
				mirrorTextureDescription.Width					= form.Width;
				mirrorTextureDescription.Height					= form.Height;
				mirrorTextureDescription.MiscFlags				= TextureMiscFlags.None;

				// Create the texture used to display the rendered result on the computer monitor.
			    IntPtr mirrorTexturePtr;
                result = OVR.CreateMirrorTextureDX(sessionPtr, device.NativePointer, ref mirrorTextureDescription, out mirrorTexturePtr);
				WriteErrorDetails(OVR, result, "Failed to create mirror texture.");

                mirrorTexture = new MirrorTexture(OVR, sessionPtr, mirrorTexturePtr);


				// Retrieve the Direct3D texture contained in the Oculus MirrorTexture.
				IntPtr	mirrorTextureComPtr = IntPtr.Zero;
				result = mirrorTexture.GetBufferDX(textureInterfaceId, out mirrorTextureComPtr);
				WriteErrorDetails(OVR, result, "Failed to retrieve the texture from the created mirror texture buffer.");

				// Create a managed Texture2D, based on the unmanaged texture pointer.
				mirrorTextureD3D = new Texture2D(mirrorTextureComPtr);

				#region Vertex and pixel shader
				// Create vertex shader.
				vertexShaderByteCode	= ShaderBytecode.CompileFromFile("Shaders.fx", "VertexShaderPositionColor", "vs_4_0");
				vertexShader			= new VertexShader(device, vertexShaderByteCode);

				// Create pixel shader.
				pixelShaderByteCode		= ShaderBytecode.CompileFromFile("Shaders.fx", "PixelShaderPositionColor", "ps_4_0");
				pixelShader				= new PixelShader(device, pixelShaderByteCode);
            
				shaderSignature			= ShaderSignature.GetInputSignature(vertexShaderByteCode);

				// Specify that each vertex consists of a single vertex position and color.
				InputElement[] inputElements = new InputElement[]
				{
					new InputElement("POSITION",	0, Format.R32G32B32A32_Float, 0, 0),
					new InputElement("COLOR",		0, Format.R32G32B32A32_Float, 16, 0)
				};

				// Define an input layout to be passed to the vertex shader.
				inputLayout = new InputLayout(device, shaderSignature, inputElements);

				// Create a vertex buffer, containing our 3D model.
				vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, m_vertices);

				// Create a constant buffer, to contain our WorldViewProjection matrix, that will be passed to the vertex shader.
				contantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

				// Setup the immediate context to use the shaders and model we defined.
				immediateContext.InputAssembler.InputLayout			= inputLayout;
				immediateContext.InputAssembler.PrimitiveTopology	= PrimitiveTopology.TriangleList;
				immediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, sizeof(float)*4*2, 0));
				immediateContext.VertexShader.SetConstantBuffer(0, contantBuffer);
				immediateContext.VertexShader.Set(vertexShader);
				immediateContext.PixelShader.Set(pixelShader);
				#endregion

				DateTime startTime = DateTime.Now;
				Vector3  position  = new Vector3(0, 0, -1);

				#region Render loop
				RenderLoop.Run(form, () =>
				{
					Vector3f[] hmdToEyeViewOffsets = {eyeTextures[0].HmdToEyeViewOffset, eyeTextures[1].HmdToEyeViewOffset};
					double displayMidpoint = OVR.GetPredictedDisplayTime(sessionPtr, 0);
					TrackingState trackingState = OVR.GetTrackingState(sessionPtr, displayMidpoint, true);
					Posef[] eyePoses = new Posef[2];
				
					// Calculate the position and orientation of each eye.
					OVR.CalcEyePoses(trackingState.HeadPose.ThePose, hmdToEyeViewOffsets, ref eyePoses);

					float timeSinceStart = (float)(DateTime.Now - startTime).TotalSeconds;

					for(int eyeIndex = 0; eyeIndex < 2; eyeIndex ++)
					{
						EyeType eye	= (EyeType)eyeIndex;
						EyeTexture eyeTexture = eyeTextures[eyeIndex];

                        if (eyeIndex == 0)
						    layerEyeFov.RenderPoseLeft = eyePoses[0];
                        else
                            layerEyeFov.RenderPoseRight = eyePoses[1];

                        // Update the render description at each frame, as the HmdToEyeOffset can change at runtime.
                        eyeTexture.RenderDescription = OVR.GetRenderDesc(sessionPtr, eye, hmdDesc.DefaultEyeFov[eyeIndex]);

						// Retrieve the index of the active texture
						int textureIndex;
				        result = eyeTexture.SwapTextureSet.GetCurrentIndex(out textureIndex);
						WriteErrorDetails(OVR, result, "Failed to retrieve texture swap chain current index.");

						immediateContext.OutputMerger.SetRenderTargets(eyeTexture.DepthStencilView, eyeTexture.RenderTargetViews[textureIndex]);
						immediateContext.ClearRenderTargetView(eyeTexture.RenderTargetViews[textureIndex], Color.Black);
						immediateContext.ClearDepthStencilView(eyeTexture.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
						immediateContext.Rasterizer.SetViewport(eyeTexture.Viewport);

						// Retrieve the eye rotation quaternion and use it to calculate the LookAt direction and the LookUp direction.
						Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
						Matrix     rotationMatrix     = Matrix.RotationQuaternion(rotationQuaternion);
						Vector3    lookUp             = Vector3.Transform(new Vector3(0, -1, 0), rotationMatrix).ToVector3();
						Vector3    lookAt             = Vector3.Transform(new Vector3(0, 0, 1), rotationMatrix).ToVector3();

						Vector3    viewPosition       = position - eyePoses[eyeIndex].Position.ToVector3();

						Matrix world      = Matrix.Scaling(0.1f) * Matrix.RotationX(timeSinceStart/10f) * Matrix.RotationY(timeSinceStart*2/10f) * Matrix.RotationZ(timeSinceStart*3/10f);
                        Matrix viewMatrix = Matrix.LookAtLH(viewPosition, viewPosition+lookAt, lookUp); 

						Matrix projectionMatrix = OVR.Matrix4f_Projection(eyeTexture.FieldOfView, 0.1f, 100.0f, ProjectionModifier.LeftHanded).ToMatrix();
						projectionMatrix.Transpose();

						Matrix worldViewProjection = world * viewMatrix * projectionMatrix;
						worldViewProjection.Transpose();

						// Update the transformation matrix.
						immediateContext.UpdateSubresource(ref worldViewProjection, contantBuffer);

						// Draw the cube
						immediateContext.Draw(m_vertices.Length/2, 0);

						// Commits any pending changes to the TextureSwapChain, and advances its current index
						result = eyeTexture.SwapTextureSet.Commit();
						WriteErrorDetails(OVR, result, "Failed to commit the swap chain texture.");
					}

                    
                    result = OVR.SubmitFrame(sessionPtr, 0L, IntPtr.Zero, ref layerEyeFov);
					WriteErrorDetails(OVR, result, "Failed to submit the frame of the current layers.");

                    immediateContext.CopyResource(mirrorTextureD3D, backBuffer);
                    swapChain.Present(0, PresentFlags.None);
                });
				#endregion
			}
			finally
			{
				if(immediateContext != null)
				{
					immediateContext.ClearState();
					immediateContext.Flush();
				}

				// Release all resources
				Dispose(inputLayout);
				Dispose(contantBuffer);
				Dispose(vertexBuffer);
				Dispose(shaderSignature);
				Dispose(pixelShader);
				Dispose(pixelShaderByteCode);
				Dispose(vertexShader);
				Dispose(vertexShaderByteCode);
				Dispose(mirrorTextureD3D);
				Dispose(mirrorTexture);
				Dispose(eyeTextures[0]);
				Dispose(eyeTextures[1]);
				Dispose(immediateContext);
				Dispose(depthStencilState);
				Dispose(depthStencilView);
				Dispose(depthBuffer);
				Dispose(backBufferRenderTargetView);
				Dispose(backBuffer);
				Dispose(swapChain);
				Dispose(factory);

                // Disposing the device, before the hmd, will cause the hmd to fail when disposing.
                // Disposing the device, after the hmd, will cause the dispose of the device to fail.
                // It looks as if the hmd steals ownership of the device and destroys it, when it's shutting down.
                // device.Dispose();
                OVR.Destroy(sessionPtr);
			}
        }

        /// <summary>
        /// Write out any error details received from the Oculus SDK, into the debug output window.
        /// 
        /// Please note that writing text to the debug output window is a slow operation and will affect performance,
        /// if too many messages are written in a short timespan.
        /// </summary>
        /// <param name="OVR">OvrWrap object for which the error occurred.</param>
        /// <param name="result">Error code to write in the debug text.</param>
        /// <param name="message">Error message to include in the debug text.</param>
        public static void WriteErrorDetails(OvrWrap OVR, Result result, string message)
		{
			if(result >= Result.Success)
				return;

			// Retrieve the error message from the last occurring error.
			ErrorInfo errorInformation = OVR.GetLastErrorInfo();

			string formattedMessage = string.Format("{0}. \nMessage: {1} (Error code={2})", message, errorInformation.ErrorString, errorInformation.Result);
			Trace.WriteLine(formattedMessage);
			MessageBox.Show(formattedMessage, message);

			throw new Exception(formattedMessage);
		}

		/// <summary>
		/// Dispose the specified object, unless it's a null object.
		/// </summary>
		/// <param name="disposable">Object to dispose.</param>
		public static void Dispose(IDisposable disposable)
		{
			if(disposable != null)
				disposable.Dispose();
		}

		static Vector4[]	m_vertices = new Vector4[]
		{
			// Near
			new Vector4( 1,  1, -1, 1), new Vector4(1, 0, 0, 1),	
			new Vector4( 1, -1, -1, 1), new Vector4(1, 0, 0, 1),	
			new Vector4(-1, -1, -1, 1), new Vector4(1, 0, 0, 1),	
			new Vector4(-1,  1, -1, 1), new Vector4(1, 0, 0, 1),	
			new Vector4( 1,  1, -1, 1), new Vector4(1, 0, 0, 1),	
			new Vector4(-1, -1, -1, 1), new Vector4(1, 0, 0, 1),	
			
			// Far
			new Vector4(-1, -1,  1, 1), new Vector4(0, 1, 0, 1),	
			new Vector4( 1, -1,  1, 1), new Vector4(0, 1, 0, 1),	
			new Vector4( 1,  1,  1, 1), new Vector4(0, 1, 0, 1),	
			new Vector4( 1,  1,  1, 1), new Vector4(0, 1, 0, 1),	
			new Vector4(-1,  1,  1, 1), new Vector4(0, 1, 0, 1),	
			new Vector4(-1, -1,  1, 1), new Vector4(0, 1, 0, 1),	

			// Left
			new Vector4(-1,  1,  1, 1), new Vector4(0, 0, 1, 1),	
			new Vector4(-1,  1, -1, 1), new Vector4(0, 0, 1, 1),	
			new Vector4(-1, -1, -1, 1), new Vector4(0, 0, 1, 1),	
			new Vector4(-1, -1, -1, 1), new Vector4(0, 0, 1, 1),	
			new Vector4(-1, -1,  1, 1), new Vector4(0, 0, 1, 1),	
			new Vector4(-1,  1,  1, 1), new Vector4(0, 0, 1, 1),	

			// Right
			new Vector4( 1, -1, -1, 1), new Vector4(1, 1, 0, 1),	
			new Vector4( 1,  1, -1, 1), new Vector4(1, 1, 0, 1),	
			new Vector4( 1,  1,  1, 1), new Vector4(1, 1, 0, 1),	
			new Vector4( 1,  1,  1, 1), new Vector4(1, 1, 0, 1),	
			new Vector4( 1, -1,  1, 1), new Vector4(1, 1, 0, 1),	
			new Vector4( 1, -1, -1, 1), new Vector4(1, 1, 0, 1),	

			// Bottom
			new Vector4(-1, -1, -1, 1), new Vector4(1, 0, 1, 1),	
			new Vector4( 1, -1, -1, 1), new Vector4(1, 0, 1, 1),	
			new Vector4( 1, -1,  1, 1), new Vector4(1, 0, 1, 1),	
			new Vector4( 1, -1,  1, 1), new Vector4(1, 0, 1, 1),	
			new Vector4(-1, -1,  1, 1), new Vector4(1, 0, 1, 1),	
			new Vector4(-1, -1, -1, 1), new Vector4(1, 0, 1, 1),	

			// Top
			new Vector4( 1,  1,  1, 1), new Vector4(0, 1, 1, 1),	
			new Vector4( 1,  1, -1, 1), new Vector4(0, 1, 1, 1),	
			new Vector4(-1,  1, -1, 1), new Vector4(0, 1, 1, 1),	
			new Vector4(-1,  1, -1, 1), new Vector4(0, 1, 1, 1),	
			new Vector4(-1,  1,  1, 1), new Vector4(0, 1, 1, 1),
			new Vector4( 1,  1,  1, 1), new Vector4(0, 1, 1, 1)	
		};
	}
}
