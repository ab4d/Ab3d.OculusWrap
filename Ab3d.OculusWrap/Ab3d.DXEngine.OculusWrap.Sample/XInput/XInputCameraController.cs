// ----------------------------------------------------------------
// <copyright file="XInputCameraController.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Controls;
using Ab3d.Utilities;
using SharpDX.XInput;

// XInputCameraController is not yet fully implemented.
// It will be part of the next version of Ab3d.PowerToys library.

namespace Ab3d.Controls
{
    /// <summary>
    /// XInputCameraController is a camera controller that can be used to control the Ab3d.PowerToys camera with game controller that support XInput (for example XBOX game controller).
    /// </summary>
    public class XInputCameraController : BaseControllerControl, ICompositionRenderingSubscriber
    {
        private Controller _controller;

        private DateTime _lastConnectionCheckTime;

        private bool _isRenderingEventSubscribed;

        private bool _isLastButtonsChangeHandled;

        private GamepadButtonFlags _lastButtonFlags;

        public bool IsControllerConnected { get; private set; }

        public short LeftThumbDeadZone = Gamepad.LeftThumbDeadZone;
        public short RightThumbDeadZone = Gamepad.RightThumbDeadZone;

        public TimeSpan ControllerConnectedCheckInterval = TimeSpan.FromSeconds(2);

        public double MovementSpeed = 1.0;
        public double RotationSpeed = 1.0;

        public bool InvertHeadingRotationDirection = false;
        public bool InvertAttitudeRotationDirection = false;

        public bool MoveOnlyHorizontally = true;

        public bool MoveVerticallyWithDPadButtons = true;

        public event EventHandler IsConnectedChanged;

        public event XInputControllerThumbChangedHandler LeftThumbChanged;
        public event XInputControllerThumbChangedHandler RightThumbChanged;

        public event XInputControllerButtonChangedHandler ButtonChanged;


        public XInputCameraController()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // This will allow to select controls defined after MouseCameraController in Designer - for example the camera
                this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                this.Width = 0;
                this.Height = 0;
            }

            this.IsHitTestVisible = false;

            _controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.One);

            this.Loaded += delegate(object sender, RoutedEventArgs args)
            {
                Start();
            };

            this.Unloaded += delegate(object sender, RoutedEventArgs args)
            {
                Stop();
            };
        }

        public void Start()
        {
            if (_isRenderingEventSubscribed)
                return;

            Ab3d.Utilities.CompositionRenderingHelper.Instance.Subscribe(this);
            _isRenderingEventSubscribed = true;
        }

        public void Stop()
        {
            if (!_isRenderingEventSubscribed)
                return;

            Ab3d.Utilities.CompositionRenderingHelper.Instance.Unsubscribe(this);
            _isRenderingEventSubscribed = false;
        }

        void ICompositionRenderingSubscriber.OnRendering(EventArgs e)
        {
            if (!IsControllerConnected)
            {
                // Based on MSDN article, we should not check for newly connected controller on each frame - https://msdn.microsoft.com/en-us/library/windows/desktop/ee417001(v=vs.85).aspx
                // This should be done only every few seconds instead

                // We do that here every 2 seconds

                var now = DateTime.Now;
                if ((now - _lastConnectionCheckTime) < ControllerConnectedCheckInterval)
                    return;

                _lastConnectionCheckTime = now;
            }

            State state;
            bool isConnected = _controller.GetState(out state);

            if (isConnected != IsControllerConnected)
            {
                _lastConnectionCheckTime = DateTime.Now;
                IsControllerConnected = isConnected;

                OnIsConnectedChanged();

                if (!isConnected) // Stop processing when the controller is disconnected
                    return;
            }


            // If we are here then the controller is connected

            // Normalize the input values and check for dead zones (based on MSDN article - see link before)

            // NOTE:
            // We use int for x and y instead of short (as defined in Gamepad struct) because when the min short value (Math.Abs is used on -32768, the produces +32768 which is an overflow because max short is +32767)

            int x = state.Gamepad.LeftThumbX;
            int y = state.Gamepad.LeftThumbY;

            // Ensure LeftThumbDeadZone is in correct interval
            if (LeftThumbDeadZone < 0)
                LeftThumbDeadZone = 0;
            else if (LeftThumbDeadZone > 32767)
                LeftThumbDeadZone = 32767;

            if (Math.Abs(x) > LeftThumbDeadZone || Math.Abs(y) > LeftThumbDeadZone)
            {
                if (x < 0)
                    x += LeftThumbDeadZone;
                else
                    x -= LeftThumbDeadZone;

                if (y < 0)
                    y += LeftThumbDeadZone;
                else
                    y -= LeftThumbDeadZone;

                double xNormalized = (double)x / (double)(32767 - LeftThumbDeadZone);
                double yNormalized = (double)y / (double)(32767 - LeftThumbDeadZone);

                var thumbChangedEventArgs = new XInputControllerThumbChangedEventArgs(x, y, xNormalized, yNormalized);
                OnLeftThumbChanged(thumbChangedEventArgs);

                // If the subscriber to LeftThumbChanged has already handled the change of the left thumb, then we do not need to do that
                if (!thumbChangedEventArgs.IsHandled)
                {
                    double dx = xNormalized * MovementSpeed;
                    double dy = yNormalized * MovementSpeed;


                    // strafeDirection is perpendicular to LookDirection and UpDirection
                    var strafeDirection = Vector3D.CrossProduct(TargetCamera.LookDirection, TargetCamera.UpDirection);
                    strafeDirection.Normalize();

                    Vector3D cameraOffsetDiff = strafeDirection * dx;


                    Vector3D usedLookDirection = TargetCamera.LookDirection;
                    if (MoveOnlyHorizontally)
                        usedLookDirection.Y = 0; // Zero y direction when we move only horizontally

                    usedLookDirection.Normalize();

                    cameraOffsetDiff += usedLookDirection * dy;

                    TargetCamera.Offset += cameraOffsetDiff;
                }
            }
            

            // Ensure RightThumbDeadZone is in correct interval
            if (RightThumbDeadZone < 0)
                RightThumbDeadZone = 0;
            else if (RightThumbDeadZone > 32767)
                RightThumbDeadZone = 32767;

            x = state.Gamepad.RightThumbX;
            y = state.Gamepad.RightThumbY;

            if (Math.Abs(x) > RightThumbDeadZone || Math.Abs(y) > RightThumbDeadZone)
            {
                if (InvertHeadingRotationDirection)
                    x = -x;

                if (x < 0)
                    x += RightThumbDeadZone;
                else
                    x -= RightThumbDeadZone;

                if (y < 0)
                    y += RightThumbDeadZone;
                else
                    y -= RightThumbDeadZone;

                double xNormalized = (double)x / (double)(32767 - RightThumbDeadZone);
                double yNormalized = (double)y / (double)(32767 - RightThumbDeadZone);

                var thumbChangedEventArgs = new XInputControllerThumbChangedEventArgs(x, y, xNormalized, yNormalized);
                OnRightThumbChanged(thumbChangedEventArgs);

                // If the subscriber to LeftThumbChanged has already handled the change of the left thumb, then we do not need to do that
                if (!thumbChangedEventArgs.IsHandled)
                {
                    if (InvertHeadingRotationDirection)
                        xNormalized = -xNormalized;

                    if (!InvertAttitudeRotationDirection)
                        yNormalized = -yNormalized;

                    var sphericalCamera = TargetCamera as Ab3d.Cameras.SphericalCamera;
                    if (sphericalCamera != null)
                    {
                        sphericalCamera.Heading += xNormalized * RotationSpeed;
                        sphericalCamera.Attitude += yNormalized * RotationSpeed;
                    }
                }
            }



            var newButtonsFlags = state.Gamepad.Buttons;
            if (newButtonsFlags != _lastButtonFlags)
            {
                var buttonChangedEventArgs = new XInputControllerButtonChangedEventArgs(_lastButtonFlags, newButtonsFlags);

                _lastButtonFlags = newButtonsFlags;

                OnButtonChanged(buttonChangedEventArgs);
                _isLastButtonsChangeHandled = buttonChangedEventArgs.IsHandled;
            }

            // If the change of buttons was handled in the previous ButtonChanged event, then we do not handle it here
            if (!_isLastButtonsChangeHandled)
            {
                if (MoveVerticallyWithDPadButtons)
                {
                    if ((newButtonsFlags & GamepadButtonFlags.DPadUp) != 0)
                    {
                        Vector3D upVector;

                        if (MoveOnlyHorizontally)
                        {
                            upVector = new Vector3D(0, MovementSpeed, 0);
                        }
                        else
                        {
                            upVector = TargetCamera.UpDirection;
                            upVector.Normalize();

                            upVector *= MovementSpeed;
                        }

                        TargetCamera.Offset += upVector;
                    }

                    if ((newButtonsFlags & GamepadButtonFlags.DPadDown) != 0)
                    {
                        Vector3D downVector;

                        if (MoveOnlyHorizontally)
                        {
                            downVector = new Vector3D(0, -MovementSpeed, 0);
                        }
                        else
                        {
                            downVector = TargetCamera.UpDirection;
                            downVector.Normalize();

                            downVector *= -MovementSpeed;
                        }

                        TargetCamera.Offset += downVector;
                    }
                }
            }
        }



        protected void OnIsConnectedChanged()
        {
            if (IsConnectedChanged != null)
                IsConnectedChanged(this, null);
        }

        protected void OnLeftThumbChanged(XInputControllerThumbChangedEventArgs e)
        {
            if (LeftThumbChanged != null)
                LeftThumbChanged(this, e);
        }

        protected void OnRightThumbChanged(XInputControllerThumbChangedEventArgs e)
        {
            if (RightThumbChanged != null)
                RightThumbChanged(this, e);
        }

        protected void OnButtonChanged(XInputControllerButtonChangedEventArgs e)
        {
            if (ButtonChanged != null)
                ButtonChanged(this, e);
        }
    }
}