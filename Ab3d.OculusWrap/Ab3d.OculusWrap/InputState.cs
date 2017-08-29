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

using System.Runtime.InteropServices;

namespace Ab3d.OculusWrap
{
    /// <summary>
    /// InputState describes the complete controller input state, including Oculus Touch,
    /// and XBox gamepad. If multiple inputs are connected and used at the same time,
    /// their inputs are combined.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct InputState
    {
        /// <summary>
        /// System type when the controller state was last updated.
        /// </summary>
        public double			TimeInSeconds;

        /// <summary>
        /// Values for buttons described by ovrButton.
        /// </summary>
        public uint				Buttons;

        /// <summary>
        /// Touch values for buttons and sensors as described by ovrTouch.
        /// </summary>
        public uint				Touches;
    
        /// <summary>
        /// Left and right finger trigger values (Hand.Left and Hand.Right), in the range 0.0 to 1.0f.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
        public float[]			IndexTrigger;
    
        /// <summary>
        /// Left and right hand trigger values (Hand.Left and Hand.Right), in the range 0.0 to 1.0f.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
        public float[]			HandTrigger;

        /// <summary>
        /// Horizontal and vertical thumbstick axis values (Hand.Left and Hand.Right), in the range -1.0f to 1.0f.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
        public Vector2f[]		Thumbstick;

        /// <summary>
        /// The type of the controller this state is for.
        /// </summary>
        public ControllerType	ControllerType;

        /// <summary>
        /// Left and right finger trigger values (ovrHand_Left and ovrHand_Right), in the range 0.0 to 1.0f.
        /// Does not apply a deadzone.  Only touch applies a filter.
        /// This has been formally named simply "Trigger". We retain the name IndexTrigger for backwards code compatibility.
        /// User-facing documentation should refer to it as the Trigger.
        /// Added in 1.7
        /// </summary>            
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] IndexTriggerNoDeadzone;

        /// <summary>
        /// Left and right hand trigger values (ovrHand_Left and ovrHand_Right), in the range 0.0 to 1.0f.
        /// Does not apply a deadzone. Only touch applies a filter.
        /// This has been formally named "Grip Button". We retain the name HandTrigger for backwards code compatibility.
        /// User-facing documentation should refer to it as the Grip Button or simply Grip.
        /// Added in 1.7
        /// </summary>   
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] HandTriggerNoDeadzone;

        /// <summary>
        /// Horizontal and vertical thumbstick axis values (ovrHand_Left and ovrHand_Right), in the range -1.0f to 1.0f
        /// Does not apply a deadzone or filter.
        /// Added in 1.7
        /// </summary>   
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector2f[] ThumbstickNoDeadzone;

        /// <summary>
        /// Left and right finger trigger values (ovrHand_Left and ovrHand_Right), in range 0.0 to 1.0f.
        /// No deadzone or filter
        /// This has been formally named "Grip Button". We retain the name HandTrigger for backwards code
        /// compatibility.
        /// User-facing documentation should refer to it as the Grip Button or simply Grip.
        /// </summary>   
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        float[] IndexTriggerRaw;

        /// <summary>
        /// Left and right hand trigger values (ovrHand_Left and ovrHand_Right), in the range 0.0 to 1.0f.
        /// No deadzone or filter
        /// This has been formally named "Grip Button". We retain the name HandTrigger for backwards code
        /// compatibility.
        /// User-facing documentation should refer to it as the Grip Button or simply Grip.
        /// </summary>   
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        float[] HandTriggerRaw;

        /// <summary>
        /// Horizontal and vertical thumbstick axis values (ovrHand_Left and ovrHand_Right), in the range
        /// -1.0f to 1.0f
        /// No deadzone or filter
        /// </summary>   
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector2f[] ThumbstickRaw;
    }
}