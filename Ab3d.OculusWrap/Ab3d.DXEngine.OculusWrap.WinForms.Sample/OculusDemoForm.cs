using System;
using System.IO;
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
using SharpDX.Windows;

namespace Ab3d.DXEngine.OculusWrap.WinForms.Sample
{
    public class OculusDemoForm : IDisposable
    {
        public const bool UseOculusRift = true; // When false, no Oculus device is initialized and we have standard DXEngine 3D rendering


        private const int FormWidth = 800;
        private const int FormHeight = 550;


        private RenderForm _renderForm;

        private OvrWrap _ovr;

        private DXViewportView _dxViewportView;
        private Viewport3D _mainViewport3D;

        private DXDevice _dxDevice;
        private DXScene _dxScene;

        private FirstPersonCamera _camera;
        private XInputCameraController _xInputCameraController;

        private OculusWrapVirtualRealityProvider _oculusRiftVirtualRealityProvider;
        private VarianceShadowRenderingProvider _varianceShadowRenderingProvider;

        private int _framesCount;
        private double _renderTime;
        private int _lastFpsMeterSecond = -1;
        private bool _isFirstSecond = true;

        private string _originalWindowTitle;

        /// <summary>
        /// Create and initialize a new game.
        /// </summary>
        public OculusDemoForm()
        {
            // Set window properties
            _originalWindowTitle = "Ab3d.DXEngine Oculus WinForms Demo";

            _renderForm = new RenderForm(_originalWindowTitle);
            _renderForm.ClientSize = new System.Drawing.Size(FormWidth, FormHeight);
            _renderForm.AllowUserResizing = false;


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
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void Run()
        {
            // Start the render loop
            RenderLoop.Run(_renderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            // UpdateScene(); // If you need to play an animation or update the objects, this can be done in the Update method

            RenderScene();
        }


        private void InitializeOvrAndDirectX()
        {
            if (UseOculusRift)
            {
                // Initializing Oculus VR is very simple when using OculusWrapVirtualRealityProvider
                // First we create an instance of OculusWrapVirtualRealityProvider
                _oculusRiftVirtualRealityProvider = new OculusWrapVirtualRealityProvider(_ovr, multisamplingCount: 4);

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
                _originalWindowTitle += string.Format(" (OVR v{0})", ovrVersionString);
                _renderForm.Text = _originalWindowTitle;


                // Reset tracking origin at startup
                _ovr.RecenterTrackingOrigin(_oculusRiftVirtualRealityProvider.SessionPtr);
            }
            else
            {
                // Create DXDevice that will be used to create DXViewportView
                var dxDeviceConfiguration = new DXDeviceConfiguration();
                dxDeviceConfiguration.DriverType             = DriverType.Hardware;
                dxDeviceConfiguration.SupportedFeatureLevels = new FeatureLevel[] { FeatureLevel.Level_11_0 }; // Oculus requires at least feature level 11.0

                _dxDevice = new DXDevice(dxDeviceConfiguration);
                _dxDevice.InitializeDevice();
            }


            // Now create DXScene object from the RenderForm Handle
            // The DXScene will use SwapChain to show the rendered scene
            _dxScene                 = _dxDevice.CreateDXSceneWithSwapChain(_renderForm.Handle, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height, preferedMultisampleCount: 4);
            _dxScene.BackgroundColor = System.Windows.Media.Colors.LightBlue.ToColor4();
            _dxScene.ShaderQuality   = ShaderQuality.Normal;

            // _mainViewport3D will define the 3D scene. The scene is defined with using WPF's Viewport3D object.
            // Because Viewport3D is used, this means that we can use many helper objects from Ab3d.PowerToys library.
            _mainViewport3D = new Viewport3D();

            // When we use Viewport3D to define the 3D scene, we need to use DXViewportView to render its content in Ab3d.DXEngine.
            // The DXViewportView will convert all 3D objects defined in Viewport3D to DXEngine's SceneNodes.
            // It is also possible to define 3D scene with using only SceneNode and other low level DXEngine objects.
            // See PictureBoxForm sample and samples in "Advanced usage" section in Ab3d.DXEngine.Samples project.
            _dxViewportView = new DXViewportView(_dxScene, _mainViewport3D);

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


            // Initialize Virtual reality rendering
            _dxViewportView.DXScene.InitializeVirtualRealityRendering(_oculusRiftVirtualRealityProvider);


            // Initialized shadow rendering (see Ab3d.DXEngine.Wpf.Samples project - DXEngine/ShadowRenderingSample for more info
            _varianceShadowRenderingProvider = new VarianceShadowRenderingProvider()
            {
                ShadowMapSize          = 1024,
                ShadowDepthBluringSize = 2,
                ShadowThreshold        = 0.2f
            };

            _dxViewportView.DXScene.InitializeShadowRendering(_varianceShadowRenderingProvider);



            // Set DXDiagnostics.CurrentDXView to allow using DXEngineSnoop tool (see https://www.ab4d.com/DirectX/3D/Diagnostics.aspx)
            // Note that CurrentDXView is using WeakReference to prevent rooting the _dxViewportView by a static filed.
            Ab3d.DirectX.DXDiagnostics.CurrentDXView = _dxViewportView;


            // Enable collecting rendering statistics (see _dxViewportView.DXScene.Statistics class)
            DXDiagnostics.IsCollectingStatistics = true;


            // Set DXDiagnostics.CurrentDXView to allow using DXEngineSnoop tool (see https://www.ab4d.com/DirectX/3D/Diagnostics.aspx)
            // Note that CurrentDXView is using WeakReference to prevent rooting the _dxViewportView by a static filed.
            Ab3d.DirectX.DXDiagnostics.CurrentDXView = _dxViewportView;


            // Create FirstPersonCamera
            _camera = new FirstPersonCamera()
            {
                TargetViewport3D = _mainViewport3D,
                Position         = new Point3D(0, 1, 4),
                Heading          = 0,
                Attitude         = 0,
                ShowCameraLight  = ShowCameraLightType.Never
            };

            _camera.Refresh(); // We need to manually call Refresh because this camera is not added to WPF objects tree and therefore its Loaded event is never fired.


            // Initialize XBOX controller that will control the FirstPersonCamera
            _xInputCameraController = new XInputCameraController()
            {
                TargetCamera = _camera,
                RotationSpeed = 120,                          // rotation: 120 degrees per second
                MovementSpeed = 2,                            // movement: 2 meters per second
                RotateOnlyHorizontally = true,                // do not rotate up and down (changing attitude) with controller - this is done only HMD
                MoveVerticallyWithDPadButtons = true,
                AutomaticallyStartCheckingController = false, // We will manually call CheckController method so we need to disable automatic checking (the reason is because we are rendering at higher frame rate then 60 FPS that is use when automatically checking the controller).
            };


            // Create a sample 3D scene
            CreateSceneObjects();


            // Add lights
            var lightsVisual3D = new ModelVisual3D();
            var lightsGroup    = new Model3DGroup();

            var directionalLight = new DirectionalLight(Colors.White, new Vector3D(0.5, -0.3, -0.3));
            directionalLight.SetDXAttribute(DXAttributeType.IsCastingShadow, true); // Set this light to cast shadow
            lightsGroup.Children.Add(directionalLight);

            var ambientLight = new AmbientLight(System.Windows.Media.Color.FromRgb(30, 30, 30));
            lightsGroup.Children.Add(ambientLight);

            lightsVisual3D.Content = lightsGroup;
            _mainViewport3D.Children.Add(lightsVisual3D);


            // And finally render the first frame
            RenderScene();
        }

        private void RenderScene()
        {
            if (_dxDevice == null)
                return;

            if (_oculusRiftVirtualRealityProvider != null && _oculusRiftVirtualRealityProvider.LastSessionStatus.ShouldQuit)
            {
                _renderForm.Close();
                this.Dispose();
                return;
            }

            
            // Check the state of the controller and update the camera when needed
            _xInputCameraController.CheckController(updateTargetCamera: true);


            if (_renderForm.ClientSize.Width != _dxScene.Width || _renderForm.ClientSize.Height != _dxScene.Height)
            {
                // Resize buffers
                _dxScene.Resize(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height);
            }

            // Render the scene with DXEngine.
            // We force rendering event if there are no known changes on the scene (no _dxScene.NotifyChange calls).
            // To render only when there are any know changes, set forceRenderAll to false
            _dxScene.RenderScene(forceRenderAll: true);

            UpdateTitleFpsMeter();
        }


        #region UpdateTitleFpsMeter
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

                    _renderForm.Text = newTitle;
                }

                _framesCount = 0;
                _renderTime = 0;
                _lastFpsMeterSecond = currentSecond;
            }
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

            _mainViewport3D.Children.Clear();
            _mainViewport3D.Children.Add(rootVisual3D);
        }
        #endregion


        #region Dispose
        public void Dispose()
        {
            if (_xInputCameraController != null)
            {
                _xInputCameraController.StopCheckingController();
                _xInputCameraController = null;
            }


            // Dispose created resources:
            if (_dxViewportView != null)
            {
                _dxViewportView.Dispose();
                _dxViewportView = null;
            }

            if (_dxScene != null)
            {
                _dxScene.Dispose();
                _dxScene = null;
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