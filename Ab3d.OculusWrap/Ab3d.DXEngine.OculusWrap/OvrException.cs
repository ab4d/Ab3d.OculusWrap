// ----------------------------------------------------------------
// <copyright file="OvrException.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using Ab3d.OculusWrap;

namespace Ab3d.DXEngine.OculusWrap
{
    /// <summary>
    /// OvrException describes an exception that occured in Oculus OVR library.
    /// </summary>
    public class OvrException : Exception
    {
        /// <summary>
        /// Gets exception result details
        /// </summary>
        public Result Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">message</param>
        public OvrException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="result">Result</param>
        public OvrException(string message, Result result)
            : base(message)
        {
            Result = result;
        }
    }
}