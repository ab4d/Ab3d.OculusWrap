// ----------------------------------------------------------------
// <copyright file="OculusWrapVirtualRealityProvider.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ab3d.DirectX;
using Ab3d.DirectX.Cameras;
using Ab3d.OculusWrap;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Result = Ab3d.OculusWrap.Result;

namespace Ab3d.DXEngine.OculusWrap
{
    /// <summary>
    /// OculusWrapVirtualRealityProvider is a VirtualRealityProvider that can render the 3D scene for Oculus Rift head mounted display.
    /// </summary>
    public class OculusWrapVirtualRealityProvider : StereoscopicVirtualRealityProvider
    {
        private OculusTextureSwapChain[] _eyeTextureSwapChains;

        private ChangeBackBufferRenderingStep _resetViewportRenderingStep;

        private readonly OvrWrap _ovr;
        private IntPtr _sessionPtr;

        private readonly int _multisamplingCount;

        private HmdDesc _hmdDesc;

        private float _pixelsPerDisplayPixel = 1.0f;
        private LayerEyeFov _layerShared;

        private EyeRenderDesc[] _eyeRenderDesc = new EyeRenderDesc[2];
        private Vector3f[] _hmdToEyeOffset = new Vector3f[2];
        private Posef[] _eyePoses = new Posef[2];

        private MirrorTextureDesc _mirrorTextureDesc;
        private OculusMirrorTexture _mirrorTexture;

        private Texture2D _msaaBackBuffer;
        private Texture2DDescription _msaaBackBufferDescription;
        private RenderTargetView _msaaBackBufferRenderTargetView;
        private DepthStencilView _msaaDepthStencilView;

        private uint _frameIndex;

        private MatrixCamera _matrixCamera;
        private SessionStatus _sessionStatus;
        private Texture2D _mirrorTextureDX;
        private double _sensorSampleTime;

        private Vector3[] _sceneBoundingCorners;

        /// <summary>
        /// Gets the OVR session as IntPtr
        /// </summary>
        public IntPtr SessionPtr { get { return _sessionPtr; } }
        
        /// <summary>
        /// Gets the description of the HMD (Head Mounted Display)
        /// </summary>
        public HmdDesc HmdDescription { get { return _hmdDesc; } }

        /// <summary>
        /// Gets last OVR session status.
        /// </summary>
        public SessionStatus LastSessionStatus { get { return _sessionStatus; } }

        /// <summary>
        /// Constructor with already initialized HMD (Head Mounted Display) - its pointer is specified with ovrSessionPtr.
        /// </summary>
        /// <param name="ovr">OculusWrap.Wrap</param>
        /// <param name="ovrSessionPtr">Session IntPtr get by call to OculusWrap.Create method</param>
        /// <param name="multisamplingCount">multisamplingCount (0 - no multisampling; default value is 4)</param>
        public OculusWrapVirtualRealityProvider(OvrWrap ovr, IntPtr ovrSessionPtr, int multisamplingCount = 4)
            : base(0, 0, prepareRenderTargetsForEachEye: true)
        {
            if (ovr == null) throw new ArgumentNullException(nameof(ovr));

            _ovr = ovr;
            _sessionPtr = ovrSessionPtr;
            _multisamplingCount = multisamplingCount;

            if (ovrSessionPtr != IntPtr.Zero)
                _hmdDesc = ovr.GetHmdDesc(ovrSessionPtr);
        }

        /// <summary>
        /// Constructor without initialized HMD (Head Mounted Display). This constructor requires calling <see cref="InitializeOvrAndDXDevice"/> method to complete the initialization of HMD.
        /// </summary>
        /// <param name="ovr">OculusWrap.Wrap</param>
        /// <param name="multisamplingCount">multisamplingCount (0 - no multisampling; default value is 4)</param>
        public OculusWrapVirtualRealityProvider(OvrWrap ovr, int multisamplingCount = 4)
            : this(ovr, IntPtr.Zero, multisamplingCount)
        {
        }

        /// <summary>
        /// InitializeOvrAndDXDevice method initializes the Oculus OVR and creates a new DXDevice that uses the same adapter (graphic card) as Oculus Rift.
        /// User need to dispose the created DXDevice when it is not needed any more.
        /// This method can throw exceptions in case initialization of OVR or DirectX fails.
        /// </summary>
        /// <param name="requestedOculusSdkMinorVersion">minimal version of Oculus SKD that is required for this application (default value is 17)</param>
        /// <returns>Created DXDevice that needs to be disposed by the user</returns>
        public DXDevice InitializeOvrAndDXDevice(int requestedOculusSdkMinorVersion = 17)
        {
            if (_sessionPtr != IntPtr.Zero)
                throw new Exception("InitializeOvrAndDXDevice cannot be called after the sessionPtr was already set.");

            
            // Define initialization parameters with debug flag.
            var initializationParameters = new InitParams();

            // In Oculus SDK 1.8 and newer versions, it is required to specify to which version the application is build
            initializationParameters.Flags = InitFlags.RequestVersion;
            initializationParameters.RequestedMinorVersion = (uint)requestedOculusSdkMinorVersion;

            // Initialize the Oculus runtime.
            var result = _ovr.Initialize(initializationParameters);

            if (result < Result.Success)
                throw new OvrException("Failed to initialize the Oculus runtime library.", result);


            // Use the head mounted display.
            var adapterLuid = new GraphicsLuid();
            result = _ovr.Create(ref _sessionPtr, ref adapterLuid);

            if (result < Result.Success)
                throw new OvrException("Oculus Rift not detected", result);


            _hmdDesc = _ovr.GetHmdDesc(_sessionPtr);


            // Get adapter (graphics card) used by Oculus
            Adapter1 hmdAdapter;
            if (adapterLuid.Reserved != null && adapterLuid.Reserved.Length == 4)
            {
                var allSystemAdapters = DXDevice.GetAllSystemAdapters();

                long adapterUid = Convert.ToInt64(adapterLuid.Reserved); // adapterLuid.Reserved is byte array
                hmdAdapter = allSystemAdapters.FirstOrDefault(a => a.Description1.Luid == adapterUid);
            }
            else
            {
                hmdAdapter = null;
            }

            // Create DXEngine's DirectX Device with the same adapter (graphics card) that is used for Oculus Rift
            var dxDeviceConfiguration = new DXDeviceConfiguration();

            if (hmdAdapter != null)
                dxDeviceConfiguration.Adapter = hmdAdapter;

            dxDeviceConfiguration.DriverType = DriverType.Hardware;
            dxDeviceConfiguration.SupportedFeatureLevels = new FeatureLevel[] {FeatureLevel.Level_11_0};

            // Create DirectX device
            var dxDevice = new DXDevice(dxDeviceConfiguration);
            dxDevice.InitializeDevice();

            return dxDevice;
        }

        /// <summary>
        /// CreateResources is called when the VirtualRealityProvider is initialized and should create the DirectX resources.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>CreateResources</b> is called when the VirtualRealityProvider is initialized and should create the DirectX resources.
        /// </para>
        /// <para>
        /// This method is called after this virtual reality provider is registered with calling the <see cref="DXScene.InitializeVirtualRealityRendering"/> method. 
        /// This method then calls the <see cref="DXSceneResource.InitializeResources"/> and the <see cref="VirtualRealityProviderBase.OnInitializeResources"/>.
        /// OnInitializeResources calls the this CreateResources method and then <see cref="VirtualRealityProviderBase.InitializeRenderingSteps"/> method.
        /// </para>
        /// <para>
        /// This method usually creates pixel shaders and constant buffers.
        /// Other resources (back buffers and views) are usually created in <see cref="VirtualRealityProviderBase.UpdateRenderingContext"/> where the size of the current back buffer is compared with the size of back buffers for virtual reality.
        /// </para>
        /// </remarks>
        /// <param name="dxScene">parent DXScene</param>
        protected override void CreateResources(DXScene dxScene)
        {
            if (_eyeTextureSwapChains != null)
            {
                if (_eyeTextureSwapChains[0] != null)
                    _eyeTextureSwapChains[0].Dispose();

                if (_eyeTextureSwapChains[1] != null)
                    _eyeTextureSwapChains[1].Dispose();
            }
            else
            {
                _eyeTextureSwapChains = new OculusTextureSwapChain[2];
            }

            _eyeTextureSwapChains[0] = new OculusTextureSwapChain(_ovr, 
                                                                  _sessionPtr, 
                                                                  dxScene.Device, 
                                                                  EyeType.Left, 
                                                                  Format.B8G8R8A8_UNorm_SRgb, 
                                                                  _ovr.GetFovTextureSize(_sessionPtr, EyeType.Left, _hmdDesc.DefaultEyeFov[0], 1.0f), 
                                                                  createDepthStencilView: true, 
                                                                  isDebugDevice: dxScene.DXDevice.IsDebugDevice);

            _eyeTextureSwapChains[1] = new OculusTextureSwapChain(_ovr, 
                                                                  _sessionPtr, 
                                                                  dxScene.Device, 
                                                                  EyeType.Left, 
                                                                  Format.B8G8R8A8_UNorm_SRgb, 
                                                                  _ovr.GetFovTextureSize(_sessionPtr, EyeType.Right, _hmdDesc.DefaultEyeFov[1], 1.0f), 
                                                                  createDepthStencilView: true, 
                                                                  isDebugDevice: dxScene.DXDevice.IsDebugDevice);


            _layerShared = new LayerEyeFov();
            _layerShared.Header = new LayerHeader()
            {
                Type = LayerType.EyeFov,
                Flags = LayerFlags.HighQuality
            };

            // Specify the texture to show on the HMD.
            _layerShared.ColorTextureLeft  = _eyeTextureSwapChains[0].TextureSwapChainPtr;
            _layerShared.ColorTextureRight = _eyeTextureSwapChains[1].TextureSwapChainPtr;

            _layerShared.ViewportLeft.Position = new Vector2i(0, 0);
            _layerShared.ViewportLeft.Size = _eyeTextureSwapChains[0].Size;

            _layerShared.ViewportRight.Position = new Vector2i(0, 0);
            _layerShared.ViewportRight.Size = _eyeTextureSwapChains[1].Size;

            _layerShared.FovLeft  = _hmdDesc.DefaultEyeFov[0];
            _layerShared.FovRight = _hmdDesc.DefaultEyeFov[1];


            _eyeRenderDesc[0] = _ovr.GetRenderDesc(_sessionPtr, EyeType.Left, _hmdDesc.DefaultEyeFov[0]);
            _hmdToEyeOffset[1] = _eyeRenderDesc[1].HmdToEyePose.Position;

            _eyeRenderDesc[1] = _ovr.GetRenderDesc(_sessionPtr, EyeType.Right, _hmdDesc.DefaultEyeFov[1]);
            _hmdToEyeOffset[1] = _eyeRenderDesc[1].HmdToEyePose.Position;


            // Create MSAA back buffer if needed
            UpdateMsaaBackBuffer(_eyeTextureSwapChains[0].Size.Width, _eyeTextureSwapChains[0].Size.Height, _multisamplingCount);


            _mirrorTextureDesc = new MirrorTextureDesc()
            {
                Format = SharpDXHelpers.GetTextureFormat(dxScene.BackBufferDescription.Format),
                Height = dxScene.BackBufferDescription.Height,
                MiscFlags = dxScene.BackBufferDescription.MipLevels != 1 ? TextureMiscFlags.AllowGenerateMips : TextureMiscFlags.None,
                Width = dxScene.BackBufferDescription.Width
            };

            // FloorLevel will give tracking poses where the floor height is 0
            _ovr.SetTrackingOriginType(_sessionPtr, TrackingOrigin.EyeLevel);

            IntPtr mirrorTexturePtr;
            var result = _ovr.CreateMirrorTextureDX(_sessionPtr, dxScene.Device.NativePointer, ref _mirrorTextureDesc, out mirrorTexturePtr);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException("Failed to create Oculus mirror texture: " + lastError.ErrorString, lastError.Result);
            }

            _mirrorTexture = new OculusMirrorTexture(_ovr, _sessionPtr, mirrorTexturePtr);

            // Retrieve the Direct3D texture contained in the Oculus MirrorTexture.
            IntPtr mirrorTextureComPtr;
            result = _mirrorTexture.GetBufferDX(typeof(Texture2D).GUID, out mirrorTextureComPtr);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException("Failed to retrieve the texture from the created mirror texture buffer: " + lastError.ErrorString, lastError.Result);
            }

            // Create a managed Texture2D, based on the unmanaged texture pointer.
            _mirrorTextureDX = new Texture2D(mirrorTextureComPtr);

            if (dxScene.DXDevice.IsDebugDevice)
                _mirrorTextureDX.DebugName = "OculusMirrorTexture";


            // To prevent DirectX from rendering more then one frame in the background, 
            // we need to set the MaximumFrameLatency to 1.
            // This prevents occasional dropped frames in Oculus Rift.
            var dxgiDevice = dxScene.Device.QueryInterface<SharpDX.DXGI.Device1>();
            if (dxgiDevice != null)
            {
                dxgiDevice.MaximumFrameLatency = 1;
                dxgiDevice.Dispose();
            }

            _frameIndex = 0;

            _matrixCamera = new MatrixCamera();
        }

        /// <summary>
        /// InitializeRenderingSteps is called when the VirtualRealityProvider is initialized and should add customer rendering steps to the DXScene.RenderingSteps list.
        /// See remarks for more into.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>InitializeRenderingSteps</b> is called when the VirtualRealityProvider is initialized and should add customer rendering steps to the DXScene.RenderingSteps list.
        /// </para>
        /// <para>
        /// Usually the virtual reality rendering provider adds 3 rendering steps to existing rendering step:<br/>
        /// 1) <see cref="BeginVirtualRealityRenderingStep"/> is added before DXScene.DefaultPrepareRenderTargetsRenderingStep (prepares the rendering context for the currently rendered eys)<br/>
        /// 2) <see cref="RenderingStepsLoop"/> is added after DXScene.DefaultResolveMultisampledBackBufferRenderingStep (this renders the scene again for the other eye - jumps to BeginVirtualRealityRenderingStep)<br/>
        /// 3) <see cref="SimpleResolveStereoscopicImagesRenderingStep"/> or similar step is added after RenderingStepsLoop (to render post-process effects after the VR resolve) or befor DXScene.DefaultCompleteRenderingStep (to render post-process effects before the VS resolve).
        /// </para>
        /// <para>
        /// This method usually also created the pixel shaders and constant buffers.
        /// Other resources (back buffers and views) are usually created in <see cref="VirtualRealityProviderBase.UpdateRenderingContext"/> where the size of the current back buffer is compared with the size of back buffers for virtual reality.
        /// </para>
        /// <para>
        /// It is recommended that the created rendering steps are protected or public with private setter.
        /// This way a derived class can override the InitializeRenderingSteps method and add the created rendering steps in some other was to the DXScene.RenderingSteps.
        /// </para>        
        /// </remarks>
        /// <param name="dxScene">parent DXScene</param>
        protected override void InitializeRenderingSteps(DXScene dxScene)
        {
            // Call base class to:
            // Create and add beginVirtualRealityRenderingStep 
            // Create and add renderingStepsLoop
            base.InitializeRenderingSteps(dxScene);

            if (_resetViewportRenderingStep != null)
            {
                dxScene.RenderingSteps.Remove(_resetViewportRenderingStep);
                _resetViewportRenderingStep.Dispose();
            }


            // After both eyes were rendered, we need to reset the Viewport back to full screen
            // This can be done with adding the ChangeBackBufferRenderingStep after the renderingStepsLoop (after both eyes are rendered)
            // ChangeBackBufferRenderingStep is usually used to change current back buffer and its views, but it can be also used to change only Viewport.
            // Here we only create an instance of ChangeBackBufferRenderingStep and add it to RenderingSteps.
            // In the UpdateRenderingContext (below) we will set the NewViewport property to the size of the FinalBackBuffer
            _resetViewportRenderingStep = new ChangeBackBufferRenderingStep("ResetViewportRenderingStep", "Resets the Viewport from split screen viewport to the final full screen viewport");

            dxScene.RenderingSteps.AddAfter(dxScene.DefaultResolveBackBufferRenderingStep, _resetViewportRenderingStep);


            if (renderingStepsLoop != null)
                dxScene.RenderingSteps.Remove(renderingStepsLoop);

            // We need to call _textureSwapChain.Commit() after image for each eye is rendered

            // We create a loop in rendering steps with adding a RenderingStepsLoop (this is the last step in the loop)
            // The loop begins with beginVirtualRealityRenderingStep (when the loop is repeated, the execution goes back to beginVirtualRealityRenderingStep step)
            // The RenderingStepsLoop also requires a loopPredicate that determines if the loop should repeat (returns true) or exit (returns false).
            renderingStepsLoop = new RenderingStepsLoop("RepeatVirtualRealityLoop",
                beginLoopRenderingStep: beginVirtualRealityRenderingStep,
                loopPredicate: (RenderingContext r) =>
                {
                    // This predicate is executed when with the RenderingStepsLoop execution.
                    // It returns true in case the rendering loop should repeat itself, or false when it should exit.
                    // As seen from the return statement below, we repeat the rendering loop when the stereoscopic rendering is enabled and when we have rendered the left eye
                    var currentEye = r.VirtualRealityContext.CurrentEye;

                    if (_eyeTextureSwapChains != null)
                    {
                        // Update the _sessionStatus before rendering the frame
                        if (currentEye == Eye.Left)
                        {
                            UpdateSessionStatus();

                            if (_sessionStatus.ShouldRecenter)
                                _ovr.RecenterTrackingOrigin(_sessionPtr);
                        }

                        if (_sessionStatus.IsVisible) // We should submit OVR frames only when VR has focus
                        {
                            int eyeIndex = currentEye == Eye.Left ? 0 : 1;

                            _eyeTextureSwapChains[eyeIndex].Commit();

                            if (currentEye == Eye.Right)
                            {
                                _layerShared.Header.Type = LayerType.EyeFov;
                                _layerShared.Header.Flags = LayerFlags.None;

                                _layerShared.ColorTextureLeft = _eyeTextureSwapChains[0].TextureSwapChainPtr;
                                _layerShared.ViewportLeft = new Recti(new Vector2i(0, 0), new Sizei(_eyeTextureSwapChains[0].ViewportSize.Width, _eyeTextureSwapChains[0].ViewportSize.Height));
                                _layerShared.FovLeft = _hmdDesc.DefaultEyeFov[0];
                                _layerShared.RenderPoseLeft = _eyePoses[0];

                                _layerShared.ColorTextureRight = _eyeTextureSwapChains[1].TextureSwapChainPtr;
                                _layerShared.ViewportRight = new Recti(new Vector2i(0, 0), new Sizei(_eyeTextureSwapChains[1].ViewportSize.Width, _eyeTextureSwapChains[1].ViewportSize.Height));
                                _layerShared.FovRight = _hmdDesc.DefaultEyeFov[1];
                                _layerShared.RenderPoseRight = _eyePoses[1];

                                _layerShared.SensorSampleTime = _sensorSampleTime;

                                var result = _ovr.SubmitFrame(_sessionPtr, _frameIndex, IntPtr.Zero, ref _layerShared);

                                if (result < Ab3d.OculusWrap.Result.Success)
                                {
                                    var lastError = _ovr.GetLastErrorInfo();
                                    throw new OvrException("Failed to sumbit frame: " + result);
                                }

                                _frameIndex++;
                            }
                        }

                        if (_mirrorTextureDesc.Width == r.FinalBackBufferDescription.Width && _mirrorTextureDesc.Height == r.FinalBackBufferDescription.Height)
                            r.DeviceContext.CopyResource(_mirrorTextureDX, r.FinalBackBuffer);
                    }

                    // Repeat the rendering loop when the stereoscopic rendering is enabled and when we have rendered the left eye
                    return (this.IsEnabled &&
                            r.VirtualRealityContext != null &&
                            currentEye == Eye.Left);
                });

            dxScene.RenderingSteps.AddAfter(dxScene.DefaultResolveBackBufferRenderingStep, renderingStepsLoop);
        }

        /// <summary>
        /// UpdateRenderingContext is called from the BeginVirtualRealityRenderingStep and should update the properties in the RenderingContext according to the current eye.
        /// See remarks for more info about the usual tasks that are preformed in this method.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>UpdateRenderingContext</b> is called from the BeginVirtualRealityRenderingStep and should update the properties in the RenderingContext according to the current eye.
        /// </para>
        /// <para>
        /// This method is usually called from the <see cref="BeginVirtualRealityRenderingStep"/> (when the virtual reality provider is enabled).
        /// </para>
        /// <para>
        /// Usually this method does the following:<br/>
        /// - Creates the back buffers and views that are needed for rendering 
        /// (the back buffers are also recreated if the size of <see cref="RenderingContext.CurrentBackBufferDescription"/> is different from the size of previously created back buffers).<br/>
        /// - Updates the <see cref="VirtualRealityContext.CurrentEye"/> property.<br/>
        /// - Sets the <see cref="RenderingContext.UsedCamera"/> property to a <see cref="StereoscopicCamera"/><br/>
        /// - Calls <see cref="RenderingContext.SetBackBuffer"/> method and sets the new back buffers.<br/>
        /// - Updates <see cref="ResolveMultisampledBackBufferRenderingStep.DestinationBuffer"/> on the <see cref="DXScene.DefaultResolveMultisampledBackBufferRenderingStep"/> and sets it to the eye texture.
        /// </para>
        /// </remarks>
        /// <param name="renderingContext">RenderingContext</param>
        /// <param name="isNewFrame">true if no eye was yet rendered for the current frame; false if the first eye was already rendered for the current frame and we need to render the second eye</param>
        public override void UpdateRenderingContext(RenderingContext renderingContext, bool isNewFrame)
        {
            // This code is executed inside BeginVirtualRealityRenderingStep before all the objects are rendered.

            // Base method does:
            // - sets the virtualRealityContext.CurrentEye based on the isNewFrame parameter: isNewFrame == true => LeftEye else RightEye
            // - ensures that stereoscopicCamera is created and sets its properties for the current eye and based on the current EyeSeparation, Parallax and InvertLeftRightView
            // - sets renderingContext.UsedCamera = stereoscopicCamera
            base.UpdateRenderingContext(renderingContext, isNewFrame);


            var virtualRealityContext = renderingContext.VirtualRealityContext;

            Eye currentEye = virtualRealityContext.CurrentEye;
            EyeType ovrEye = currentEye == Eye.Left ? EyeType.Left : EyeType.Right;

            int eyeIndex = currentEye == Eye.Left ? 0 : 1;


            FovPort defaultEyeFov = _hmdDesc.DefaultEyeFov[eyeIndex];
            var idealSize = _ovr.GetFovTextureSize(_sessionPtr, ovrEye, defaultEyeFov, _pixelsPerDisplayPixel);

            // When we render this frame for the first time 
            // we also check that all the required resources are created
            // Check if we need to create or recreate the RenderTargetViews and DepthStencilViews
            if (isNewFrame &&
                (_eyeTextureSwapChains[eyeIndex] == null ||
                 _eyeTextureSwapChains[eyeIndex].Size.Width != idealSize.Width ||
                 _eyeTextureSwapChains[eyeIndex].Size.Height != idealSize.Height))
            {
                CreateResources(renderingContext.DXScene);
            }

            if (isNewFrame)
                _ovr.GetEyePoses(_sessionPtr, 0L, true, _hmdToEyeOffset, ref _eyePoses, out _sensorSampleTime);

            var camera = renderingContext.DXScene.Camera;


            // From OculusRoomTiny main.cpp #221

            //Get the pose information
            var eyeQuat = SharpDXHelpers.ToQuaternion(_eyePoses[eyeIndex].Orientation);
            var eyePos  = SharpDXHelpers.ToVector3(_eyePoses[eyeIndex].Position);

            // Get view and projection matrices for the Rift camera
            Vector3 cameraPosition = camera.GetCameraPosition();
            Matrix cameraRotationMatrix = camera.View;
            cameraRotationMatrix.M41 = 0; // Remove translation
            cameraRotationMatrix.M42 = 0;
            cameraRotationMatrix.M43 = 0;

            cameraRotationMatrix.Invert(); // Invert to get rotation matrix

            Vector4 rotatedEyePos4 = Vector3.Transform(eyePos, cameraRotationMatrix);
            var rotatedEyePos = new Vector3(rotatedEyePos4.X, rotatedEyePos4.Y, rotatedEyePos4.Z);

            var finalCameraPosition = cameraPosition + rotatedEyePos;

            var eyeQuaternionMatrix = Matrix.RotationQuaternion(eyeQuat);
            var finalRotationMatrix = eyeQuaternionMatrix * cameraRotationMatrix;

            Vector4 lookDirection4 = Vector3.Transform(new Vector3(0, 0, -1), finalRotationMatrix);
            var lookDirection = new Vector3(lookDirection4.X, lookDirection4.Y, lookDirection4.Z);

            Vector4 upDirection4 = Vector3.Transform(Vector3.UnitY, finalRotationMatrix);
            var upDirection = new Vector3(upDirection4.X, upDirection4.Y, upDirection4.Z);

            var viewMatrix = Matrix.LookAtRH(finalCameraPosition, finalCameraPosition + lookDirection, upDirection);


            // Calculate optimal camera near and far planes
            // For this we need all 8 corners of the scene's bounding box and view matrix
            if (_sceneBoundingCorners == null)
                _sceneBoundingCorners = new Vector3[8]; // create array here and reuse it on every frame (to prevent garbage)

            renderingContext.DXScene.RootNode.Bounds.BoundingBox.GetCorners(_sceneBoundingCorners);

            float zNear, zFar;
            DXScene.CalculateCameraPlanes(ref viewMatrix, isOrthographicProjection: false, isRightHandedCoordinateSystem: true, boundingCorners: _sceneBoundingCorners, adjustValues: true, 
                                          zNear: out zNear, zFar: out zFar);


            var eyeRenderDesc = _ovr.GetRenderDesc(_sessionPtr, ovrEye, _hmdDesc.DefaultEyeFov[eyeIndex]);

            var projectionMatrix = _ovr.Matrix4f_Projection(eyeRenderDesc.Fov, zNear, zFar, ProjectionModifier.None).ToMatrix();
            projectionMatrix.Transpose();

            _matrixCamera.Projection = projectionMatrix;
            _matrixCamera.View = viewMatrix;
            _matrixCamera.SetCameraPosition(finalCameraPosition);

            renderingContext.UsedCamera = _matrixCamera;


            // Change the current viewport
            renderingContext.CurrentViewport = _eyeTextureSwapChains[eyeIndex].Viewport;
            renderingContext.DeviceContext.Rasterizer.SetViewport(renderingContext.CurrentViewport);

            if (_msaaBackBuffer == null)
            {
                renderingContext.SetBackBuffer(backBuffer: _eyeTextureSwapChains[eyeIndex].CurrentTexture,
                                               backBufferDescription: _eyeTextureSwapChains[eyeIndex].CurrentTextureDescription,
                                               renderTargetView: _eyeTextureSwapChains[eyeIndex].CurrentRTView,
                                               depthStencilView: _eyeTextureSwapChains[eyeIndex].CurrentDepthStencilView,
                                               bindNewRenderTargetsToDeviceContext: false, // Do not bind new buffers because this is done in the next rendering step - PrepareRenderTargetsRenderingStep
                                               currentSupersamplingCount: 1); 
            }
            else
            {
                // MSAA
                renderingContext.SetBackBuffer(backBuffer: _msaaBackBuffer,
                                               backBufferDescription: _msaaBackBufferDescription,
                                               renderTargetView: _msaaBackBufferRenderTargetView,
                                               depthStencilView: _msaaDepthStencilView,
                                               bindNewRenderTargetsToDeviceContext: false, // Do not bind new buffers because this is done in the next rendering step - PrepareRenderTargetsRenderingStep
                                               currentSupersamplingCount: 1); 

                renderingContext.DXScene.DefaultResolveBackBufferRenderingStep.SetCustomDestinationBuffer(_eyeTextureSwapChains[eyeIndex].CurrentTexture, _eyeTextureSwapChains[eyeIndex].CurrentTextureDescription, _eyeTextureSwapChains[eyeIndex].CurrentRTView);
            }


            // When we render this frame for the first time set the NewViewport on the ChangeBackBufferRenderingStep to resets the Viewport from split screen viewport to the final full screen viewport 
            if (isNewFrame && _resetViewportRenderingStep != null)
            {
                int backBufferWidth = renderingContext.FinalBackBufferDescription.Width;
                int backBufferHeight = renderingContext.FinalBackBufferDescription.Height;

                _resetViewportRenderingStep.NewViewport = new ViewportF(0, 0, backBufferWidth, backBufferHeight);
            }
        }

        /// <summary>
        /// UpdateSessionStatus updates the <see cref="LastSessionStatus"/> property value.
        /// </summary>
        public void UpdateSessionStatus()
        {
            var result = _ovr.GetSessionStatus(_sessionPtr, ref _sessionStatus);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException("Failed to get session status: " + lastError.ErrorString, lastError.Result);
            }
        }

        // Create or dispose the MSAA buffers
        private void UpdateMsaaBackBuffer(int width, int height, int multisamplingCount)
        {
            if (multisamplingCount <= 0 || width <= 0 || height <= 0)
            {
                DiposeMsaaBuffers();
                return;
            }

            if (_msaaBackBuffer != null && _msaaBackBufferDescription.Width == width &&
                                           _msaaBackBufferDescription.Height == height &&
                                           _msaaBackBufferDescription.SampleDescription.Count == multisamplingCount)
            {
                // We already have the correct MSAA back buffer
                return;
            }


            // Dispose existing buffers
            DiposeMsaaBuffers();

            var sampleDescription = new SampleDescription(multisamplingCount, 0);

            var dxDevice = parentDXScene.DXDevice;

            _msaaBackBufferDescription = dxDevice.CreateTexture2DDescription(width, height, sampleDescription,
                                                                             isRenderTarget: true,
                                                                             isSharedResource: false,
                                                                             isStagingTexture: false,
                                                                             isShaderResource: false,
                                                                             format: Format.B8G8R8A8_UNorm);

            _msaaBackBuffer = dxDevice.CreateTexture2D(_msaaBackBufferDescription);
            _msaaBackBufferRenderTargetView = new RenderTargetView(dxDevice.Device, _msaaBackBuffer);
            _msaaDepthStencilView = dxDevice.CreateDepthStencilView(width, height, sampleDescription, DXDevice.StandardDepthStencilFormat);
        }
        
        private void DiposeMsaaBuffers()
        {
            DisposeHelper.DisposeAndNullify(ref _msaaBackBuffer);
            DisposeHelper.DisposeAndNullify(ref _msaaDepthStencilView);
            DisposeHelper.DisposeAndNullify(ref _msaaBackBufferRenderTargetView);

            _msaaBackBufferDescription = new Texture2DDescription(); // Reset the description values
        }

        /// <summary>
        /// Dispose the created resource and removes the added rendering steps.
        /// </summary>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                DiposeMsaaBuffers();

                if (_mirrorTexture != null)
                {
                    _mirrorTexture.Dispose();
                    _mirrorTexture = null;

                    _mirrorTextureDesc = new MirrorTextureDesc();
                }

                if (_eyeTextureSwapChains != null)
                {
                    _eyeTextureSwapChains[0].Dispose();
                    _eyeTextureSwapChains[1].Dispose();

                    _eyeTextureSwapChains = null;
                }

                if (_resetViewportRenderingStep != null)
                {
                    if (parentDXScene != null)
                        parentDXScene.RenderingSteps.Remove(_resetViewportRenderingStep);

                    _resetViewportRenderingStep.Dispose();
                    _resetViewportRenderingStep = null;
                }

                if (renderingStepsLoop != null)
                {
                    if (parentDXScene != null)
                        parentDXScene.RenderingSteps.Remove(renderingStepsLoop);

                    renderingStepsLoop.Dispose();
                    renderingStepsLoop = null;
                }

                if (parentDXScene != null)
                    parentDXScene.DefaultResolveBackBufferRenderingStep.ClearCustomDestinationBuffer(); // Set to previous value
            }

            base.Dispose(isDisposing);
        }
    }
}