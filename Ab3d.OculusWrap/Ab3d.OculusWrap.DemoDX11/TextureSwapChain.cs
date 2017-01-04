using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ab3d.OculusWrap.DemoDX11
{
	/// <summary>
	/// Wrapper for the TextureSwapChain type.
	/// </summary>
	public class TextureSwapChain : IDisposable
	{
        private IntPtr _sessionPtr;

        private OvrWrap _ovr;

        /// <summary>
        /// Pointer to unmanaged SwapTextureSet.
        /// </summary>
        public IntPtr TextureSwapChainPtr { get; private set; }

        /// <summary>
        /// Describes if the object has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Notifies subscribers when this object has been disposed.
        /// </summary>
        public event Action<TextureSwapChain> Disposed;

        /// <summary>
        /// Creates a new TextureSwapChain.
        /// </summary>
        /// <param name="ovr">Interface to Oculus runtime methods.</param>
        /// <param name="sessionPtr">Session of the Hmd owning this texture swap chain.</param>
        /// <param name="textureSwapChainPtr">Unmanaged texture swap chain.</param>
        public TextureSwapChain(OvrWrap ovr, IntPtr sessionPtr, IntPtr textureSwapChainPtr)
		{
			if(ovr == null)
				throw new ArgumentNullException("ovr");

			if(sessionPtr == IntPtr.Zero)
				throw new ArgumentNullException("sessionPtr");

            if (textureSwapChainPtr == IntPtr.Zero)
				throw new ArgumentNullException("textureSwapChainPtr");

            _ovr = ovr;
			_sessionPtr = sessionPtr;
			TextureSwapChainPtr = textureSwapChainPtr;
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

			if(TextureSwapChainPtr != IntPtr.Zero)
			{
				_ovr.DestroyTextureSwapChain(_sessionPtr, TextureSwapChainPtr);
				TextureSwapChainPtr = IntPtr.Zero;

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
		/// Gets the number of buffers in the TextureSwapChain.
		/// </summary>
		/// <param name="length">Returns the number of buffers in the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		public Result GetLength(out int length)
		{
			if (IsDisposed)
				throw new ObjectDisposedException("TextureSwapChain");

			return _ovr.GetTextureSwapChainLength(_sessionPtr, TextureSwapChainPtr, out length);
		}

		/// <summary>
		/// Gets the current index in the TextureSwapChain.
		/// </summary>
		/// <param name="index">Returns the current (free) index in specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		public Result GetCurrentIndex(out int index)
		{
			if (IsDisposed)
				throw new ObjectDisposedException("TextureSwapChain");

			return _ovr.GetTextureSwapChainCurrentIndex(_sessionPtr, TextureSwapChainPtr, out index);
		}

		/// <summary>
		/// Gets the description of the buffers in the TextureSwapChain
		/// </summary>
		/// <param name="textureSwapChainDescription">Returns the description of the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		public Result GetDescription(out TextureSwapChainDesc textureSwapChainDescription)
		{
			if (IsDisposed)
				throw new ObjectDisposedException("TextureSwapChain");

			TextureSwapChainDesc textureSwapChainDesc = new TextureSwapChainDesc();

			Result result = _ovr.GetTextureSwapChainDesc(_sessionPtr, TextureSwapChainPtr, ref textureSwapChainDesc);
			textureSwapChainDescription = textureSwapChainDesc;

			return result;
		}

		/// <summary>
		/// Commits any pending changes to a TextureSwapChain, and advances its current index
		/// </summary>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error.
		/// Failures include but aren't limited to:
		///   - Result.TextureSwapChainFull: ovr_CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
		/// </returns>
		public Result Commit()
		{
			if(IsDisposed)
				throw new ObjectDisposedException("TextureSwapChain");

			return _ovr.CommitTextureSwapChain(_sessionPtr, TextureSwapChainPtr);
		}

		/// <summary>
		/// Get a specific buffer within the chain as any compatible COM interface (similar to QueryInterface)
		/// </summary>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see GetTextureSwapChainLength),
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex).
		/// </param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public Result GetBufferDX(int index, Guid iid, out IntPtr buffer)
		{
			return _ovr.GetTextureSwapChainBufferDX(_sessionPtr, TextureSwapChainPtr, index, iid, out buffer);
		}

		/// <summary>
		/// Get a specific buffer within the chain as a GL texture name
		/// </summary>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see GetTextureSwapChainLength)
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
		/// </param>
		/// <param name="textureId">Returns the GL texture object name associated with the specific index requested</param>
		/// <returns>
		/// Returns an Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public Result GetBufferGL(int index, out uint textureId)
		{
			return _ovr.GetTextureSwapChainBufferGL(_sessionPtr, TextureSwapChainPtr, index, out textureId);
		}

		#endregion
	}
}
