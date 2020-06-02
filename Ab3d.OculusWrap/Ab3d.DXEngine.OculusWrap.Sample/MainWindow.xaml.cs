using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Cameras;
using Ab3d.Common;
using Ab3d.Common.Cameras;
using Ab3d.Controls;
using Ab3d.DirectX;
using Ab3d.DirectX.Controls;
using Ab3d.OculusWrap;
using Ab3d.Visuals;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using Result = Ab3d.OculusWrap.Result;


// IMPORTANT:
// This sample shows how to use WPF window to show Oculus VR with Ab3d.DXEngine.
// Because by default WPF limits frame rate to 60 FPS, the sample is using
// some tricks (calling DXewportView.Refresh method from background thread) - see RenderAt90Fps.
//
// If do not need to show other WPF content in the window,
// then it is recommended to use WinForms window instead of WPF window.
// This works in much more similar way as the official Oculus sample in C++.
// See Ab3d.DXEngine.OculusWrap.WinForms.Sample for more.

namespace Ab3d.DXEngine.OculusWrap.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const bool RenderAt90Fps = true; // When true, a background worker is used to force rendering at 90 FPS; when false a standard WPF's Rendering event is used to render at app. 60 FPS

        public const bool UseOculusRift = true; // When false, no Oculus device is initialized and we have standard DXEngine 3D rendering



        private OvrWrap _ovr;

        private DXDevice _dxDevice;

        private Viewport3D _viewport3D;
        private DXViewportView _dxViewportView;

        private FirstPersonCamera _camera;
        private XInputCameraController _xInputCameraController;

        private volatile OculusWrapVirtualRealityProvider _oculusRiftVirtualRealityProvider;
        private VarianceShadowRenderingProvider _varianceShadowRenderingProvider;

        private int _framesCount;
        private double _renderTime;
        private int _lastFpsMeterSecond = -1;
        private bool _isFirstSecond = true;
        private TimeSpan _lastRenderTime;

        private string _originalWindowTitle;

        private MeshGeometry3D _leftControllerMesh;

        private ModelVisual3D _leftControllerVisual3D;
        private QuaternionRotation3D _leftControllerQuaternionRotation3D;
        private RotateTransform3D _leftControllerBodyRotateTransform3D;
        private TranslateTransform3D _leftControllerTranslateTransform3D;

        private ModelVisual3D _rightControllerVisual3D;
        private QuaternionRotation3D _rightControllerQuaternionRotation3D;
        private RotateTransform3D _rightControllerBodyRotateTransform3D;
        private TranslateTransform3D _rightControllerTranslateTransform3D;

        private System.Windows.Media.Media3D.Material _controllerMaterial;


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                if (UseOculusRift)
                {
                    // Create Oculus OVR Wrapper
                    _ovr = OvrWrap.Create();

                    // Check if OVR service is running
                    var detectResult = _ovr.Detect(0);

                    if (!detectResult.IsOculusServiceRunning)
                    {
                        MessageBox.Show("Oculus service is not running", "Oculus error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Check if Head Mounter Display is connected
                    if (!detectResult.IsOculusHMDConnected)
                    {
                        MessageBox.Show("Oculus HMD (Head Mounter Display) is not connected", "Oculus error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                InitializeOvrAndDirectX();
            };

            this.Closing += delegate (object sender, CancelEventArgs args)
            {
                if (!RenderAt90Fps) 
                    CompositionTarget.Rendering -= CompositionTargetOnRendering; // Stop rendering at 60 FPS

                if (_xInputCameraController != null)
                    _xInputCameraController.StopCheckingController();

                Dispose();
            };
        }

        #region InitializeOvrAndDirectX
        private void InitializeOvrAndDirectX()
        {
            if (UseOculusRift)
            {
                // Initializing Oculus VR is very simple when using OculusWrapVirtualRealityProvider
                // First we create an instance of OculusWrapVirtualRealityProvider
                _oculusRiftVirtualRealityProvider = new OculusWrapVirtualRealityProvider(_ovr,  multisamplingCount: 4);

                try
                {
                    // Then we initialize Oculus OVR and create a new DXDevice that uses the same adapter (graphic card) as Oculus Rift
                    _dxDevice = _oculusRiftVirtualRealityProvider.InitializeOvrAndDXDevice(requestedOculusSdkMinorVersion: 17);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to initialize the Oculus runtime library.\r\nError: " + ex.Message, "Oculus error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string ovrVersionString = _ovr.GetVersionString();
                _originalWindowTitle = string.Format("DXEngine OculusWrap Sample (OVR v{0})", ovrVersionString);
                this.Title = _originalWindowTitle;


                // Reset tracking origin at startup
                _ovr.RecenterTrackingOrigin(_oculusRiftVirtualRealityProvider.SessionPtr);
            }
            else
            {
                // Create DXDevice that will be used to create DXViewportView
                var dxDeviceConfiguration = new DXDeviceConfiguration();
                dxDeviceConfiguration.DriverType = DriverType.Hardware;
                dxDeviceConfiguration.SupportedFeatureLevels = new FeatureLevel[] { FeatureLevel.Level_11_0 }; // Oculus requires at least feature level 11.0

                _dxDevice = new DXDevice(dxDeviceConfiguration);
                _dxDevice.InitializeDevice();

                _originalWindowTitle = this.Title;
            }



            // Create WPF's Viewport3D
            _viewport3D = new Viewport3D();

            // Create DXViewportView - a control that will enable DirectX 11 rendering of standard WPF 3D content defined in Viewport3D.
            // We use a specified DXDevice that was created by the _oculusRiftVirtualRealityProvider.InitializeOvrAndDXDevice (this way the same adapter is used by Oculus and DXEngine).
            _dxViewportView = new DXViewportView(_dxDevice, _viewport3D);
            
            _dxViewportView.BackgroundColor = Colors.Aqua;

            // Currently DXEngine support showing Oculus mirror texture only with DirectXOverlay presentation type (not with DirectXImage)
            _dxViewportView.PresentationType = DXView.PresentationTypes.DirectXOverlay;

            if (UseOculusRift)
            {
                // The _dxViewportView will show Oculus mirrow window.
                // The mirror window can be any size, for this sample we use 1/2 the HMD resolution.
                _dxViewportView.Width = _oculusRiftVirtualRealityProvider.HmdDescription.Resolution.Width / 2.0;
                _dxViewportView.Height = _oculusRiftVirtualRealityProvider.HmdDescription.Resolution.Height / 2.0;
            }


            // When the DXViewportView is initialized, we set the _oculusRiftVirtualRealityProvider to the DXScene object
            _dxViewportView.DXSceneInitialized += delegate (object sender, EventArgs args)
            {
                if (_dxViewportView.UsedGraphicsProfile.DriverType != GraphicsProfile.DriverTypes.Wpf3D &&
                    _dxViewportView.DXScene != null &&
                    _oculusRiftVirtualRealityProvider != null)
                {
                    // Initialize Virtual reality rendering
                    _dxViewportView.DXScene.InitializeVirtualRealityRendering(_oculusRiftVirtualRealityProvider);


                    // Initialized shadow rendering (see Ab3d.DXEngine.Wpf.Samples project - DXEngine/ShadowRenderingSample for more info
                    _varianceShadowRenderingProvider = new VarianceShadowRenderingProvider()
                    {
                        ShadowMapSize = 1024,
                        ShadowDepthBluringSize = 2,
                        ShadowThreshold = 0.2f
                    };

                    _dxViewportView.DXScene.InitializeShadowRendering(_varianceShadowRenderingProvider);
                }
            };


            // Enable collecting rendering statistics (see _dxViewportView.DXScene.Statistics class)
            DXDiagnostics.IsCollectingStatistics = true;

            // Subscribe to SceneRendered to collect FPS statistics
            _dxViewportView.SceneRendered += DXViewportViewOnSceneRendered;

            // Add _dxViewportView to the RootGrid
            // Before that we resize the window to be big enough to show the mirrored texture
            this.Width = _dxViewportView.Width + 30;
            this.Height = _dxViewportView.Height + 50;

            RootGrid.Children.Add(_dxViewportView);


            // Create FirstPersonCamera
            _camera = new FirstPersonCamera()
            {
                TargetViewport3D = _viewport3D,
                Position = new Point3D(0, 1, 4),
                Heading = 0,
                Attitude = 0,
                ShowCameraLight = ShowCameraLightType.Never
            };

            RootGrid.Children.Add(_camera);


            // Initialize XBOX controller that will control the FirstPersonCamera
            _xInputCameraController = new XInputCameraController()
            {
                TargetCamera = _camera,
                RotationSpeed = 120,                          // rotation: 120 degrees per second
                MovementSpeed = 2,                            // movement: 2 meters per second
                RotateOnlyHorizontally = true,                // do not rotate up and down (changing attitude) with controller - this is done only HMD
                MoveVerticallyWithDPadButtons = true,
            };

            _xInputCameraController.StartCheckingController();


            // Now we can create our sample 3D scene
            CreateSceneObjects();

            // Add lights
            var lightsVisual3D = new ModelVisual3D();
            var lightsGroup = new Model3DGroup();

            var directionalLight = new DirectionalLight(Colors.White, new Vector3D(0.5, -0.3, -0.3));
            directionalLight.SetDXAttribute(DXAttributeType.IsCastingShadow, true); // Set this light to cast shadow
            lightsGroup.Children.Add(directionalLight);

            var ambientLight = new AmbientLight(System.Windows.Media.Color.FromRgb(30, 30, 30));
            lightsGroup.Children.Add(ambientLight);

            lightsVisual3D.Content = lightsGroup;
            _viewport3D.Children.Add(lightsVisual3D);


            // Start rendering
            if (RenderAt90Fps)
            {
                // WPF do not support rendering at more the 60 FPS.
                // But with a trick where a rendering loop is created in a background thread, it is possible to achieve more than 60 FPS.
                // In case of submitting frames to Oculus Rift at higher FPS, the ovr.SubmitFrame method will limit rendering to 90 FPS.
                // 
                // NOTE:
                // When using DXEngine, it is also possible to render the scene in a background thread. 
                // This requires that the 3D scene is also created in the background thread and that the events and other messages are 
                // passed between UI and background thread in a thread safe way. This is too complicated for this simple sample project.
                // To see one possible implementation of background rendering, see the BackgroundRenderingSample in the Ab3d.DXEngine.Wpf.Samples project.
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (object sender, DoWorkEventArgs args) =>
                {
                    // Create an action that will be called by Dispatcher
                    var refreshDXEngineAction = new Action(() =>
                    {
                        UpdateScene();

                        // Render DXEngine's 3D scene again
                        if (_dxViewportView != null)
                            _dxViewportView.Refresh();
                    });

                    while (_dxViewportView != null && !_dxViewportView.IsDisposed) // Render until window is closed
                    {
                        if (_oculusRiftVirtualRealityProvider != null && _oculusRiftVirtualRealityProvider.LastSessionStatus.ShouldQuit) // Stop rendering - this will call RunWorkerCompleted where we can quit the application
                            break;

                        // Sleep for 1 ms to allow WPF tasks to complete (for example handling XBOX controller events)
                        System.Threading.Thread.Sleep(1);

                        // Call Refresh to render the DXEngine's scene
                        // This is a synchronous call and will wait until the scene is rendered. 
                        // Because Oculus is limited to 90 fps, the call to ovr.SubmitFrame will limit rendering to 90 FPS.
                        Dispatcher.Invoke(refreshDXEngineAction); 
                    }
                };

                backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
                {
                    if (_oculusRiftVirtualRealityProvider != null && _oculusRiftVirtualRealityProvider.LastSessionStatus.ShouldQuit)
                        this.Close(); // Exit the application
                };

                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                // Subscribe to WPF rendering event (called approximately 60 times per second)
                CompositionTarget.Rendering += CompositionTargetOnRendering;
            }
        }
        #endregion

        #region Update
        // All scene objects updates should be done here
        private void UpdateScene()
        {
            ProcessTouchControllers();
        }

        private void UpdateTitleFpsMeter()
        {
            // We count number of rendered frames for each second.
            _framesCount++;

            // We also store the sum of the time required by DXEngine to render one frame.
            // This will be used to show average render time and the theoretical FPS (by DXEngine) in the window title
            if (_dxViewportView.DXScene != null && _dxViewportView.DXScene.Statistics != null)
            {
                // The Statistics.TotalRenderTimeMs time also contains time where the code waits for ovr.SubmitFrame method.
                // This method takes most of the time because it wait until 90 FPS sync.
                // To get accurate DXEngine rendering time we need to sum the sub-process times.
                // In the future version of DXEngine the VR time will be stored separately so we could substract it from the TotalRenderTimeMs.
                //
                //_renderTime += _dxViewportView.DXScene.Statistics.TotalRenderTimeMs;
                _renderTime += _dxViewportView.DXScene.Statistics.UpdateTimeMs +
                               _dxViewportView.DXScene.Statistics.PrepareRenderTimeMs +
                               _dxViewportView.DXScene.Statistics.RenderShadowsMs +
                               _dxViewportView.DXScene.Statistics.DrawRenderTimeMs +
                               _dxViewportView.DXScene.Statistics.PostProcessingRenderTimeMs +
                               _dxViewportView.DXScene.Statistics.CompleteRenderTimeMs;
            }

            // At the beginning of the next second, we show statistics in the window title
            int currentSecond = DateTime.Now.Second;

            if (_lastFpsMeterSecond == -1)
            {
                _lastFpsMeterSecond = currentSecond;
            }
            else if (currentSecond != _lastFpsMeterSecond)
            {
                // If we are here, then a new second has begun
                if (_isFirstSecond)
                {
                    // We start measuring in the middle of the first second so the result for the first second is not correct - do not show it
                    _isFirstSecond = false;
                }
                else
                {
                    string newTitle = string.Format("{0}  {1} FPS", _originalWindowTitle, _framesCount);
                    if (_renderTime > 0)
                    {
                        double averageRenderTime = _renderTime / _framesCount;
                        newTitle += string.Format("  DXEngine render time: {0:0.00}ms => {1:0} FPS", averageRenderTime, 1000.0 / averageRenderTime); // Show theoretical FPS from render time
                    }

                    this.Title = newTitle;
                }

                _framesCount = 0;
                _renderTime = 0;
                _lastFpsMeterSecond = currentSecond;
            }
        }

        // This method is called approximately 60 times per second but only when RenderAt90Fps const is false
        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            // It's possible for Rendering to call back twice in the same frame.
            // So only render when we haven't already rendered in this frame.
            var renderingEventArgs = eventArgs as System.Windows.Media.RenderingEventArgs;
            if (renderingEventArgs != null)
            {
                if (renderingEventArgs.RenderingTime == _lastRenderTime)
                    return;

                _lastRenderTime = renderingEventArgs.RenderingTime;
            }

            if (_dxViewportView == null || _dxViewportView.IsDisposed)
                return; // Window closed

            if (_oculusRiftVirtualRealityProvider != null && _oculusRiftVirtualRealityProvider.LastSessionStatus.ShouldQuit)
                this.Close(); // Exit the application

            if (_oculusRiftVirtualRealityProvider == null)
                return;


            UpdateScene();

            // Render the scene again
            _dxViewportView.Refresh();
        }

        // This method is called each time DXEngine frame is rendered
        private void DXViewportViewOnSceneRendered(object sender, EventArgs eventArgs)
        {
            // Measure FPS
            UpdateTitleFpsMeter();
        }
        #endregion

        #region CreateSceneObjects
        private void CreateSceneObjects()
        {
            // NOTE: For VR all units must be in meters
            
            var rootVisual3D = new ModelVisual3D();

            // NOTE that the size of the scene will affect the quality of the shadows (bigger scene requite bigger shadow map)
            var floorBox = new BoxVisual3D()
            {
                CenterPosition = new Point3D(0, -0.5, 0),
                Size = new Size3D(10, 1, 10),                  // 10 x 1 x 10 meters
                Material = new DiffuseMaterial(Brushes.Green)
            };

            rootVisual3D.Children.Add(floorBox);

            double centerX = 0;
            double centerZ = 0;
            double circleRadius = 3;
            double boxesHeight = 1.3;

            var grayMaterial = new DiffuseMaterial(Brushes.Gray);
            var goldMaterial = new MaterialGroup();
            goldMaterial.Children.Add(new DiffuseMaterial(Brushes.Gold));
            goldMaterial.Children.Add(new SpecularMaterial(Brushes.White, 16));

            // Create spheres on top of boxes that are organized in a circle
            for (int a = 0; a < 360; a += 36)
            {
                double rad = SharpDX.MathUtil.DegreesToRadians(a + 18);
                double x = Math.Sin(rad) * circleRadius + centerX;
                double z = Math.Cos(rad) * circleRadius + centerZ;

                var boxVisual3D = new BoxVisual3D()
                {
                    CenterPosition = new Point3D(x, boxesHeight * 0.5, z),
                    Size = new Size3D(0.2, boxesHeight, 0.2),
                    Material = grayMaterial
                };

                var sphereVisual3D = new SphereVisual3D()
                {
                    CenterPosition = new Point3D(x, boxesHeight + 0.1, z),
                    Radius = 0.1,
                    Material = goldMaterial
                };

                rootVisual3D.Children.Add(boxVisual3D);
                rootVisual3D.Children.Add(sphereVisual3D);
            }


            // Read dragon model from obj file into Model3D object
            string dragonFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\dragon_vrip_res3.obj");

            var readerObj = new Ab3d.ReaderObj();
            var dragonModel3D = readerObj.ReadModel3D(dragonFileName);

            // Scale the model
            dragonModel3D.Transform = new ScaleTransform3D(10, 10, 10);

            Ab3d.Utilities.ModelUtils.ChangeMaterial(dragonModel3D, newMaterial: goldMaterial, newBackMaterial: null);

            // Add it to the scene
            var modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = dragonModel3D;

            rootVisual3D.Children.Add(modelVisual3D);


            // Add another box that will represet a base for the dragon model
            var dragonBaseBox = new BoxVisual3D()
            {
                CenterPosition = new Point3D(0, 0.27, 0),
                Size = new Size3D(2.2, 0.54, 1),
                Material = grayMaterial
            };

            rootVisual3D.Children.Add(dragonBaseBox);

            _viewport3D.Children.Clear();
            _viewport3D.Children.Add(rootVisual3D);
        }
        #endregion

        #region Touch controller handling
        private void ProcessTouchControllers()
        {
            if (_oculusRiftVirtualRealityProvider == null)
                return;

            var sessionPtr = _oculusRiftVirtualRealityProvider.SessionPtr;
            var connectedControllerTypes = _ovr.GetConnectedControllerTypes(sessionPtr);

            if (connectedControllerTypes.HasFlag(ControllerType.LTouch)) // Is Left touch controller connected?
            {
                if (_leftControllerVisual3D == null) // If controller is not yet visible, show it
                {
                    EnsureLeftControllerVisual3D();
                    _viewport3D.Children.Add(_leftControllerVisual3D);
                }
            }
            else
            {
                if (_leftControllerVisual3D != null) // If controller is visible, hide it
                {
                    _viewport3D.Children.Remove(_leftControllerVisual3D);
                    _leftControllerVisual3D = null;
                }
            }


            if (connectedControllerTypes.HasFlag(ControllerType.RTouch)) // Is Right touch controller connected?
            {
                if (_rightControllerVisual3D == null) // If controller is not yet visible, show it
                {
                    EnsureRightControllerVisual3D();
                    _viewport3D.Children.Add(_rightControllerVisual3D);
                }
            }
            else
            {
                if (_rightControllerVisual3D != null) // If controller is visible, hide it
                {
                    _viewport3D.Children.Remove(_rightControllerVisual3D);
                    _rightControllerVisual3D = null;
                }
            }


            // If any controller is visible update its transformation
            if (_leftControllerVisual3D != null || _rightControllerVisual3D != null)
            {
                double displayMidpoint = _ovr.GetPredictedDisplayTime(sessionPtr, 0);
                TrackingState trackingState = _ovr.GetTrackingState(sessionPtr, displayMidpoint, true);

                var cameraPosition = _camera.GetCameraPosition();

                if (_leftControllerVisual3D != null)
                {
                    var handPose = trackingState.HandPoses[0].ThePose;
                    var handPosePosition = handPose.Position;

                    // Update transformations that are already assigned to _leftControllerVisual3D

                    // First rotate the controller based on its rotation in our hand
                    _leftControllerQuaternionRotation3D.Quaternion = new Quaternion(handPose.Orientation.X, handPose.Orientation.Y, handPose.Orientation.Z, handPose.Orientation.W); // NOTE: Quaternion is struct so no GC "harm" is done here

                    // Now rotate because of our body rotation (the amount of rotation is defined in the _camera.Heading)
                    // We also need to adjust the center of rotation
                    
                    _leftControllerBodyRotateTransform3D.CenterX = -handPosePosition.X;
                    _leftControllerBodyRotateTransform3D.CenterY = -handPosePosition.Y;
                    _leftControllerBodyRotateTransform3D.CenterZ = -handPosePosition.Z;
                    ((AxisAngleRotation3D)_leftControllerBodyRotateTransform3D.Rotation).Angle = -_camera.Heading;

                    // Finally move the controller model for the hand pose offset + body offset
                    _leftControllerTranslateTransform3D.OffsetX = cameraPosition.X + handPosePosition.X;
                    _leftControllerTranslateTransform3D.OffsetY = cameraPosition.Y + handPosePosition.Y;
                    _leftControllerTranslateTransform3D.OffsetZ = cameraPosition.Z + handPosePosition.Z;
                }

                if (_rightControllerVisual3D != null)
                {
                    var handPose = trackingState.HandPoses[1].ThePose;
                    var handPosePosition = handPose.Position;

                    // Update transformations that are already assigned to _rightControllerVisual3D

                    // First rotate the controller based on its rotation in our hand
                    _rightControllerQuaternionRotation3D.Quaternion = new Quaternion(handPose.Orientation.X, handPose.Orientation.Y, handPose.Orientation.Z, handPose.Orientation.W); // NOTE: Quaternion is struct so no GC "harm" is done here

                    // Now rotate because of our body rotation (the amount of rotation is defined in the _camera.Heading)
                    // We also need to adjust the center of rotation
                    
                    _rightControllerBodyRotateTransform3D.CenterX = -handPosePosition.X;
                    _rightControllerBodyRotateTransform3D.CenterY = -handPosePosition.Y;
                    _rightControllerBodyRotateTransform3D.CenterZ = -handPosePosition.Z;
                    ((AxisAngleRotation3D)_rightControllerBodyRotateTransform3D.Rotation).Angle = -_camera.Heading;

                    // Finally move the controller model for the hand pose offset + body offset
                    _rightControllerTranslateTransform3D.OffsetX = cameraPosition.X + handPosePosition.X;
                    _rightControllerTranslateTransform3D.OffsetY = cameraPosition.Y + handPosePosition.Y;
                    _rightControllerTranslateTransform3D.OffsetZ = cameraPosition.Z + handPosePosition.Z;
                }



                // Check the state of the controller Thicksticks and move or rotate accordingly
                var leftControllerInputState  = new InputState();
                var rightControllerInputState = new InputState();
                _ovr.GetInputState(sessionPtr, ControllerType.LTouch, ref leftControllerInputState);
                _ovr.GetInputState(sessionPtr, ControllerType.RTouch, ref rightControllerInputState);

                // Change camera angle
                _camera.Heading += rightControllerInputState.Thumbstick[1].X * _xInputCameraController.RotationSpeed;

                // Now move the camera (use strafe directions)
                double dx = leftControllerInputState.Thumbstick[0].X * _xInputCameraController.MovementSpeed;
                double dy = leftControllerInputState.Thumbstick[0].Y * _xInputCameraController.MovementSpeed;

                // strafeDirection is perpendicular to LookDirection and UpDirection
                var strafeDirection = Vector3D.CrossProduct(_camera.LookDirection, _camera.UpDirection);
                strafeDirection.Normalize();

                Vector3D movementVector = strafeDirection * dx; // move left / right

                Vector3D usedLookDirection = _camera.LookDirection;
                if (_xInputCameraController.MoveOnlyHorizontally)
                    usedLookDirection.Y = 0; // Zero y direction when we move only horizontally

                usedLookDirection.Normalize();

                movementVector += usedLookDirection * dy; // move forward / backward

                _camera.Offset += movementVector;
            }
        }

        private void EnsureLeftControllerMesh()
        {
            // Controller mash is get from: ovr_sdk_win_1.17.0_public\OculusSDK\Samples\OculusWorldDemo\Assets\Tuscany\LeftController.xml
            // This model is also used to render the right controller. It is only flipped on x axis (see OculusWorldDemoApp::RenderControllers)
            _leftControllerMesh = (MeshGeometry3D)this.FindResource("LeftTouchController");

            if (_leftControllerMesh == null)
                throw new Exception("Cannot find the LeftTouchController MeshGeometry3D - it is defined in the App.xaml");

            // We also create the material here
            _controllerMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(20, 20, 20))); // Almost back Diffuse material
        }

        private void EnsureLeftControllerVisual3D()
        {
            if (_leftControllerVisual3D != null)
                return;

            EnsureLeftControllerMesh();

            var controllerModel3D = new GeometryModel3D(_leftControllerMesh, _controllerMaterial);
            controllerModel3D.BackMaterial = _controllerMaterial;

            // Add transformations
            _leftControllerQuaternionRotation3D = new QuaternionRotation3D();
            _leftControllerBodyRotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0)); // rotate around y (up) axis
            _leftControllerTranslateTransform3D = new TranslateTransform3D();

            var transform3DGroup = new Transform3DGroup();
            transform3DGroup.Children.Add(new RotateTransform3D(_leftControllerQuaternionRotation3D));
            transform3DGroup.Children.Add(_leftControllerBodyRotateTransform3D);
            transform3DGroup.Children.Add(_leftControllerTranslateTransform3D);

            controllerModel3D.Transform = transform3DGroup;

            // Finally create ModelVisualD that can be added to Viewport3D's Children
            _leftControllerVisual3D = controllerModel3D.CreateModelVisual3D();
        }

        private void EnsureRightControllerVisual3D()
        {
            if (_rightControllerVisual3D != null)
                return;


            // Right controller model is same as left controller but flipped around x axis.
            // 
            // Because flipping x axis also change the winding order of triangles, this would lead to showing the model with rendering the inside of the triangles.
            // To fix that we need to swap the order of triangles with swapping first and second triangle indice.

            EnsureLeftControllerMesh();
           

            // Flip x position
            var leftPositions = _leftControllerMesh.Positions;
            int count = leftPositions.Count;

            var rightPositions = new Point3DCollection(count);
            for (int i = 0; i < count; i++)
            {
                var leftPosition = leftPositions[i];
                rightPositions.Add(new Point3D(-leftPosition.X, leftPosition.Y, leftPosition.Z));
            }

            // swap 1st and 2nd triangle indice 
            var leftTriangleIndices = _leftControllerMesh.TriangleIndices;
            count = leftTriangleIndices.Count;

            var rightTriangleIndices = new Int32Collection(count);
            for (int i = 0; i < count; i+=3)
            {
                rightTriangleIndices.Add(leftTriangleIndices[i + 1]); 
                rightTriangleIndices.Add(leftTriangleIndices[i + 0]);
                rightTriangleIndices.Add(leftTriangleIndices[i + 2]);
            }


            var rightControllerMesh = new MeshGeometry3D()
            {
                Positions = rightPositions,
                TriangleIndices = rightTriangleIndices
            };

            // Now we can crate the GeometryModel3D and ModelVisual3D
            var controllerModel3D = new GeometryModel3D(rightControllerMesh, _controllerMaterial);
            controllerModel3D.BackMaterial = _controllerMaterial;

            // Add transformations
            _rightControllerQuaternionRotation3D = new QuaternionRotation3D();
            _rightControllerBodyRotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0)); // rotate around y (up) axis
            _rightControllerTranslateTransform3D = new TranslateTransform3D();

            var transform3DGroup = new Transform3DGroup();
            transform3DGroup.Children.Add(new RotateTransform3D(_rightControllerQuaternionRotation3D));
            transform3DGroup.Children.Add(_rightControllerBodyRotateTransform3D);
            transform3DGroup.Children.Add(_rightControllerTranslateTransform3D);

            controllerModel3D.Transform = transform3DGroup;

            // Finally create ModelVisualD that can be added to Viewport3D's Children
            _rightControllerVisual3D = controllerModel3D.CreateModelVisual3D();
        }
        #endregion

        #region Dispose
        private void DisposeOculusRiftVirtualRealityProvider()
        {
            if (_oculusRiftVirtualRealityProvider != null)
            {
                _oculusRiftVirtualRealityProvider.Dispose();
                _oculusRiftVirtualRealityProvider = null;
            }

            _ovr = null;
        }

        private void Dispose()
        {
            DisposeOculusRiftVirtualRealityProvider();

            if (_dxViewportView != null)
            {
                _dxViewportView.Dispose();
                _dxViewportView = null;
            }

            if (_dxDevice != null)
            {
                _dxDevice.Dispose();
                _dxDevice = null;
            }
        }
        #endregion
    }
}

