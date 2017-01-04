using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;

namespace Ab3d.OculusWrap.DemoDX11
{
    /// <summary>
    /// Contains all the fields used by each eye.
    /// </summary>
    public class EyeTexture : IDisposable
    {
        public Texture2DDescription Texture2DDescription;
        public TextureSwapChain SwapTextureSet;
        public Texture2D[] Textures;
        public RenderTargetView[] RenderTargetViews;
        public Texture2DDescription DepthBufferDescription;
        public Texture2D DepthBuffer;
        public Viewport Viewport;
        public DepthStencilView DepthStencilView;
        public FovPort FieldOfView;
        public Sizei TextureSize;
        public Recti ViewportSize;
        public EyeRenderDesc RenderDescription;
        public Vector3f HmdToEyeViewOffset;

        #region IDisposable Members
        /// <summary>
        /// Dispose contained fields.
        /// </summary>
        public void Dispose()
        {
            if(SwapTextureSet != null)
            {
                SwapTextureSet.Dispose();
                SwapTextureSet = null;
            }

            if(Textures != null)
            {
                foreach(Texture2D texture in Textures)
                    texture.Dispose();

                Textures = null;
            }

            if(RenderTargetViews != null)
            {
                foreach(RenderTargetView renderTargetView in RenderTargetViews)
                    renderTargetView.Dispose();

                RenderTargetViews = null;
            }

            if(DepthBuffer != null)
            {
                DepthBuffer.Dispose();
                DepthBuffer = null;
            }

            if(DepthStencilView != null)
            {
                DepthStencilView.Dispose();
                DepthStencilView = null;
            }
        }
        #endregion
    }
}