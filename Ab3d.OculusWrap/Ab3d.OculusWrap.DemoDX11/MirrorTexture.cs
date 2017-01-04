using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ab3d.OculusWrap.DemoDX11
{
	/// <summary>
	/// Wrapper for the MirrorTexture type.
	/// </summary>
	public class MirrorTexture : IDisposable
	{
	    private IntPtr _sessionPtr;

	    private OvrWrap _ovr;

        /// <summary>
        /// Pointer to unmanaged MirrorTexture.
        /// </summary>
        public IntPtr MirrorTexturePtr { get; private set; }

        /// <summary>
        /// Describes if the object has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Notifies subscribers when this object has been disposed.
        /// </summary>
        public event Action<MirrorTexture> Disposed;

        /// <summary>
        /// Creates a new MirrorTexture.
        /// </summary>
        /// <param name="ovr">Interface to Oculus runtime methods.</param>
        /// <param name="sessionPtr">IntPtr of the OVR </param>
        /// <param name="mirrorTexturePtr">Unmanaged mirror texture.</param>
        public MirrorTexture(OvrWrap ovr, IntPtr sessionPtr, IntPtr mirrorTexturePtr)
		{
			if(ovr == null)
				throw new ArgumentNullException("ovr");

            if (sessionPtr == IntPtr.Zero)
				throw new ArgumentNullException("sessionPtr");

			if(mirrorTexturePtr == IntPtr.Zero)
				throw new ArgumentNullException("mirrorTexturePtr");

            _ovr = ovr;
            _sessionPtr = sessionPtr;
			MirrorTexturePtr = mirrorTexturePtr;
		}

		#region IDisposable Members
		/// <summary>
		/// Clean up the allocated HMD.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose pattern implementation of dispose method.
		/// </summary>
		/// <param name="disposing">True if disposing, false if finalizing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(IsDisposed)
				return;

			if(MirrorTexturePtr != IntPtr.Zero)
			{
				_ovr.DestroyMirrorTexture(_sessionPtr, MirrorTexturePtr);
				MirrorTexturePtr = IntPtr.Zero;

				// Notify subscribers that this object has been disposed.
				if(Disposed != null)
					Disposed(this);
			}

			GC.SuppressFinalize(this);

			IsDisposed = true;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
		/// </summary>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns a Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public Result GetBufferDX(Guid iid, out IntPtr buffer)
		{
			return _ovr.GetMirrorTextureBufferDX(_sessionPtr, MirrorTexturePtr, iid, out buffer);
		}

		/// <summary>
		/// Get a the underlying buffer as a GL texture name
		/// </summary>
		/// <param name="textureId">Specifies the GL texture object name associated with the mirror texture</param>
		/// <returns>
		/// Returns an OVRTypes.Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public Result GetBufferGL(out uint textureId)
		{
			return _ovr.GetMirrorTextureBufferGL(_sessionPtr, MirrorTexturePtr, out textureId);
		}

		#endregion
	}
}
