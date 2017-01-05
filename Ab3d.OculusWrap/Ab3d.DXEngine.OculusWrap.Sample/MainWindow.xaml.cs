using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Cameras;
using Ab3d.Common.Cameras;
using Ab3d.Controls;
using Ab3d.DirectX;
using Ab3d.DirectX.Controls;
using Ab3d.OculusWrap;
using Ab3d.Visuals;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using Result = Ab3d.OculusWrap.Result;

namespace Ab3d.DXEngine.OculusWrap.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const bool UseOculusRift = true; // When false, no Oculus device is initialized and we have standard DXEngine 3D rendering

        public const bool RenderAt90Fps = true; // When true, a background worker is used to force rendering at 90 FPS; when false a standard WPF's Rendering event is used to render at app. 60 FPS


        private OvrWrap _ovr;

        private DXDevice _dxDevice;

        private Viewport3D _viewport3D;
        private DXViewportView _dxViewportView;

        private FirstPersonCamera _camera;
        private XInputCameraController _xInputCameraController;

        private volatile OculusWrapVirtualRealityProvider _oculusRiftVirtualRealityProvider;

        private string _originalWindowTitle;

        private int _framesCount;
        private double _renderTime;
        private int _lastFpsMeterSecond = -1;
        private bool _isFirstSecond = true;
        private TimeSpan _lastRenderTime;
        

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
                    _xInputCameraController.Stop();

                Dispose();
            };
        }

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
                    _dxDevice = _oculusRiftVirtualRealityProvider.InitializeOvrAndDXDevice(requestedOculusSdkMinorVersion: 10);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to initialize the Oculus runtime library.\r\nError: " + ex.Message, "Oculus error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string ovrVersionString = _ovr.GetVersionString();
                _originalWindowTitle = string.Format("DXEngine OculusWrap Sample (OVR v{0})", ovrVersionString);
                this.Title = _originalWindowTitle;
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
                    _dxViewportView.DXScene.InitializeVirtualRealityRendering(_oculusRiftVirtualRealityProvider);
                }
            };


            // Enable collecting rendering statistics (see _dxViewportView.DXScene.Statistics class)
            DXDiagnostics.IsCollectingStatistics = true;

            // Subscribe to SceneRendered to collect FPS statistics
            _dxViewportView.SceneRendered += DXViewportViewOnSceneRendered;

            // Add _dxViewportView to the RootGrid
            // Before that we resize the window to be big enough to show the mirrored texture
            this.Width = _dxViewportView.Width + 100;
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
            _xInputCameraController = new XInputCameraController();
            _xInputCameraController.TargetCamera = _camera;
            _xInputCameraController.MovementSpeed = 0.01;

            // We handle the rotation by ourself to prevent rotating the camera up and down - this is done only by HMD
            _xInputCameraController.RightThumbChanged += delegate(object sender, XInputControllerThumbChangedEventArgs e)
            {
                // Apply only horizontal rotation
                _camera.Heading += e.NormalizedX * _xInputCameraController.RotationSpeed;

                // Mark the event as handled
                e.IsHandled = true;
            };

            _xInputCameraController.Start(); // Start listening to events

            // IMPORTANT:
            // We call _xInputCameraController.Stop() method in this.Closing event



            // Now we can create our sample 3D scene
            CreateSceneObjects();

            // Add lights
            var lightsVisual3D = new ModelVisual3D();
            var lightsGroup = new Model3DGroup();

            var directionalLight = new DirectionalLight(Colors.White, new Vector3D(1, -0.3, 0));
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
                // In case of sumbiting frames to Oculus Rift, the ovr.SubmitFrame method will limit rendering to 90 FPS.
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

        // This method is called approximately 60 times per second
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

            // Render the scene again
            _dxViewportView.Refresh(); 
        }

        // This method is called each time DXEngine frame is rendered
        private void DXViewportViewOnSceneRendered(object sender, EventArgs eventArgs)
        {
            // Measure FPS
            UpdateTitleFpsMeter();
        }

        private void UpdateTitleFpsMeter()
        {
            // We count number of rendered frames for each second.
            // At the beginning of the next second, we show the results in window title

            _framesCount++;

            if (_dxViewportView.DXScene != null && _dxViewportView.DXScene.Statistics != null)
                _renderTime += _dxViewportView.DXScene.Statistics.TotalRenderTimeMs;


            int currentSecond = DateTime.Now.Second;

            if (_lastFpsMeterSecond == -1)
            {
                _lastFpsMeterSecond = currentSecond;
            }
            else if (currentSecond != _lastFpsMeterSecond)
            {
                // We start measuring in the middle of the first second so the result for the first second is not correct - do not show it
                if (_isFirstSecond)
                {
                    _isFirstSecond = false;
                }
                else
                {
                    string newTitle = string.Format("{0}  {1} FPS", _originalWindowTitle, _framesCount);
                    if (_renderTime > 0)
                    {
                        double averageRenderTime = _renderTime / _framesCount;
                        newTitle += string.Format("  DXRenderTime: {0:0.0}ms => {1:0} FPS", averageRenderTime, 1000.0 / averageRenderTime); // Show theoretical FPS from render time
                    }

                    this.Title = newTitle;
                }

                _framesCount = 0;
                _renderTime = 0;
                _lastFpsMeterSecond = currentSecond;
            }
        }

        private void CreateSceneObjects()
        {
            // NOTE: For VR all units must be in meters

            var rootVisual3D = new ModelVisual3D();

            var floorBox = new BoxVisual3D()
            {
                CenterPosition = new Point3D(0, -0.5, 0),
                Size = new Size3D(15, 1, 15),
                Material = new DiffuseMaterial(Brushes.Green)
            };

            rootVisual3D.Children.Add(floorBox);


            double centerX = 0;
            double centerZ = 0;
            double circleRadius = 2;
            double boxesHeight = 1.3;

            var boxMaterial = new DiffuseMaterial(Brushes.Gray);
            var sphereMaterial = new MaterialGroup();
            sphereMaterial.Children.Add(new DiffuseMaterial(Brushes.Gold));
            sphereMaterial.Children.Add(new SpecularMaterial(Brushes.White, 16));

            // Create spheres on top of boxes that are organized in a circle
            for (int a = 0; a < 360; a += 36)
            {
                double rad = SharpDX.MathUtil.DegreesToRadians(a);
                double x = Math.Sin(rad) * circleRadius + centerX;
                double z = Math.Cos(rad) * circleRadius + centerZ;

                var boxVisual3D = new BoxVisual3D()
                {
                    CenterPosition = new Point3D(x, boxesHeight * 0.5, z),
                    Size = new Size3D(0.2, boxesHeight, 0.2),
                    Material = boxMaterial
                };

                var sphereVisual3D = new SphereVisual3D()
                {
                    CenterPosition = new Point3D(x, boxesHeight + 0.1, z),
                    Radius = 0.1,
                    Material = sphereMaterial
                };

                rootVisual3D.Children.Add(boxVisual3D);
                rootVisual3D.Children.Add(sphereVisual3D);
            }

            _viewport3D.Children.Clear();
            _viewport3D.Children.Add(rootVisual3D);
        }


        private void Dispose()
        {
            _ovr = null;

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
    }
}

