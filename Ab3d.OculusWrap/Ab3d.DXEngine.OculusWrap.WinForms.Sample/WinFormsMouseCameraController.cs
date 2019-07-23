using System;
using System.Windows;
using System.Windows.Forms;
using Ab3d.Controls;

namespace Ab3d.DXEngine.OculusWrap.WinForms.Sample
{
    /// <summary>
    /// WinFormsMouseCameraController is a MouseCameraController from Ab3d.PowerToys library that can work with Windows Forms.
    /// It can be created with a Form or a Control.
    /// </summary>
    public class WinFormsMouseCameraController : MouseCameraController
    {
        private Control _usedControl;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control">control of type ContainerControl. Can be a Form (for example SharpDX.Windows.RenderForm) or a UserControl (for example SharpDX.Windows.RenderControl) or any other object derived from ContainerControl.</param>
        public WinFormsMouseCameraController(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            Initialize(control);
        }

        private void Initialize(Control containerControl)
        {
            _usedControl = containerControl;

            containerControl.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                Point mousePosition = new Point(e.X, e.Y);

                OnMouseMove(mousePosition,
                            (e.Button & MouseButtons.Left) != 0,
                            (e.Button & MouseButtons.Middle) != 0,
                            (e.Button & MouseButtons.Right) != 0,
                            System.Windows.Input.Keyboard.Modifiers);
            };

            containerControl.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                Point mousePosition = new Point(e.X, e.Y);

                OnMouseButtonDown(mousePosition,
                                  (e.Button & MouseButtons.Left) != 0,
                                  (e.Button & MouseButtons.Middle) != 0,
                                  (e.Button & MouseButtons.Right) != 0,
                                  System.Windows.Input.Keyboard.Modifiers);
            };

            containerControl.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                Point mousePosition = new Point(e.X, e.Y);

                OnMouseButtonUp(mousePosition,
                                (e.Button & MouseButtons.Left) != 0,
                                (e.Button & MouseButtons.Middle) != 0,
                                (e.Button & MouseButtons.Right) != 0,
                                System.Windows.Input.Keyboard.Modifiers);
            };

            containerControl.MouseWheel += delegate (object sender, MouseEventArgs e)
            {
                Point mousePosition = new Point(e.X, e.Y);

                OnMouseWheel(e.Delta, mousePosition);
            };
        }

        /// <inheritdoc />
        protected override Size GetEventsSourceElementSize()
        {
            return new Size(_usedControl.Size.Width, _usedControl.Size.Height);
        }
    }
}