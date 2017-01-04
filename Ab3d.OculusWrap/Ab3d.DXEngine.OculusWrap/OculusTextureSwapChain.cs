// ----------------------------------------------------------------
// <copyright file="OculusTextureSwapChain.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using System.Runtime.ExceptionServices;
using Ab3d.OculusWrap;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Ab3d.DXEngine.OculusWrap
{
    internal class OculusTextureSwapChain : IDisposable
    {
        private struct TextureItem
        {
            public Texture2D Texture;
            public Texture2DDescription TextureDescription;
            public Texture2D DepthBuffer;
            public DepthStencilView DepthStencilView;
            public RenderTargetView RTView;
            public ShaderResourceView SRView;
        }

        private OvrWrap _ovr;
        private IntPtr _sessionPtr;
        private TextureItem[] _textures;
        private Sizei _size;
        private Sizei _viewportSize;
        private ViewportF _viewport;
        private int _currentIndex;
        private IntPtr _textureSwapChainPtr;

        public IntPtr TextureSwapChainPtr
        {
            get { return _textureSwapChainPtr; }
        }

        public Texture2D CurrentTexture
        {
            get { return _textures[_currentIndex].Texture; }
        }

        public Texture2DDescription CurrentTextureDescription
        {
            get { return _textures[_currentIndex].TextureDescription; }
        }

        public RenderTargetView CurrentRTView
        {
            get { return _textures[_currentIndex].RTView; }
        }

        public ShaderResourceView CurrentSRView
        {
            get { return _textures[_currentIndex].SRView; }
        }

        public DepthStencilView CurrentDepthStencilView
        {
            get { return _textures[_currentIndex].DepthStencilView; }
        }

        public Sizei Size
        {
            get { return _size; }
        }

        public Sizei ViewportSize
        {
            get { return _viewportSize; }
        }

        public ViewportF Viewport
        {
            get { return _viewport; }
        }

        public OculusTextureSwapChain(OvrWrap ovr, 
                                      IntPtr sessionPtr, 
                                      SharpDX.Direct3D11.Device device, 
                                      EyeType eye, 
                                      Format format, 
                                      Sizei size, 
                                      bool createDepthStencilView = false, 
                                      bool isDebugDevice = false)
        {
            _ovr = ovr;
            _sessionPtr = sessionPtr;
            _size = size;
            _viewportSize = size;
            _viewport = new ViewportF(0.0f, 0.0f, (float) size.Width, (float) size.Height);

            Format srgbFormat = GetSRgbFormat(format);

            TextureFormat textureFormat = SharpDXHelpers.GetTextureFormat(srgbFormat);
            TextureSwapChainDesc swapChainDesc = new TextureSwapChainDesc()
            {
                ArraySize = 1,
                BindFlags = TextureBindFlags.DX_RenderTarget,
                Format = textureFormat,
                Height = _size.Height,
                MipLevels = 1,
                MiscFlags = TextureMiscFlags.DX_Typeless,
                SampleCount = 1,
                Width = _size.Width
            };

            Texture2DDescription description1 = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R24G8_Typeless,
                Height = _size.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                Width = _size.Width
            };

            ShaderResourceViewDescription description2 = new ShaderResourceViewDescription()
            {
                Format = srgbFormat,
                Dimension = ShaderResourceViewDimension.Texture2D
            };

            description2.Texture2D.MipLevels = 1;

            // Create a texture swap chain, which will contain the textures to render to, for the current eye.
            var result = _ovr.CreateTextureSwapChainDX(_sessionPtr, device.NativePointer, ref swapChainDesc, out _textureSwapChainPtr);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException("Error creating Oculus TextureSwapChain: " + lastError.ErrorString, lastError.Result);
            }


            int length;
            result = _ovr.GetTextureSwapChainLength(_sessionPtr, _textureSwapChainPtr, out length);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException("Failed to retrieve the number of buffers of the created swap chain: " + lastError.ErrorString, lastError.Result);
            }


            _textures = new TextureItem[length];

            for (int index = 0; index < length; ++index)
            {
                IntPtr bufferPtr;
                result = _ovr.GetTextureSwapChainBufferDX(_sessionPtr, _textureSwapChainPtr, index, typeof(Texture2D).GUID, out bufferPtr);

                if (result < Ab3d.OculusWrap.Result.Success)
                {
                    var lastError = _ovr.GetLastErrorInfo();
                    throw new OvrException("Failed to retrieve a texture from the created swap chain: " + lastError.ErrorString, lastError.Result);
                }

                Texture2D texture2D1 = new Texture2D(bufferPtr);
                Texture2D texture2D2 = null;
                DepthStencilView depthStencilView = null;

                if (createDepthStencilView)
                {
                    texture2D2 = new Texture2D(device, description1);
                    depthStencilView = new DepthStencilView(device, texture2D2, new DepthStencilViewDescription()
                        {
                            Flags = DepthStencilViewFlags.None,
                            Dimension = DepthStencilViewDimension.Texture2D,
                            Format = Format.D24_UNorm_S8_UInt
                        });
                }

                _textures[index] = new TextureItem()
                {
                    Texture = texture2D1,
                    TextureDescription = texture2D1.Description,
                    DepthBuffer = texture2D2,
                    DepthStencilView = depthStencilView,
                    RTView = new RenderTargetView(device, texture2D1, new RenderTargetViewDescription()
                        {
                            Format = format,
                            Dimension = RenderTargetViewDimension.Texture2D
                        }),
                    SRView = new ShaderResourceView(device, texture2D1, description2)
                };


                if (isDebugDevice)
                {
                    var eyeTextAndIndex = eye.ToString() + index.ToString();

                    _textures[index].Texture.DebugName = "OculusBackBuffer" + eyeTextAndIndex;
                    _textures[index].RTView.DebugName = "OculusRT" + eyeTextAndIndex;
                    _textures[index].SRView.DebugName = "OculusSR" + eyeTextAndIndex;
                    _textures[index].DepthBuffer.DebugName = "OculusDepthBuffer" + eyeTextAndIndex;
                    _textures[index].DepthStencilView.DebugName = "OculusDepthStencilView" + eyeTextAndIndex;
                }
            }
        }

        public void Commit()
        {
            var result = _ovr.CommitTextureSwapChain(_sessionPtr, _textureSwapChainPtr);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException("Commit failed: " + lastError.ErrorString, lastError.Result);
            }

            result = _ovr.GetTextureSwapChainCurrentIndex(_sessionPtr, _textureSwapChainPtr, out _currentIndex);

            if (result < Ab3d.OculusWrap.Result.Success)
            {
                var lastError = _ovr.GetLastErrorInfo();
                throw new OvrException(lastError.ErrorString, lastError.Result);
            }
        }

        internal static Format GetSRgbFormat(Format format)
        {
            if (FormatHelper.IsSRgb(format))
                return format;

            if (format <= Format.BC2_UNorm)
            {
                if (format == Format.R8G8B8A8_UNorm)
                    return Format.R8G8B8A8_UNorm_SRgb;
                if (format == Format.BC1_UNorm)
                    return Format.BC1_UNorm_SRgb;
                if (format == Format.BC2_UNorm)
                    return Format.BC2_UNorm_SRgb;
            }
            else if (format <= Format.B8G8R8A8_UNorm)
            {
                if (format == Format.BC3_UNorm)
                    return Format.BC3_UNorm_SRgb;
                if (format == Format.B8G8R8A8_UNorm)
                    return Format.B8G8R8A8_UNorm_SRgb;
            }
            else
            {
                if (format == Format.B8G8R8X8_UNorm)
                    return Format.B8G8R8X8_UNorm_SRgb;
                if (format == Format.BC7_UNorm)
                    return Format.BC7_UNorm_SRgb;
            }

            throw new ArgumentException(string.Format("Unsupported texture format '{0}'", (object)format), "format");
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        [HandleProcessCorruptedStateExceptions]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            for (int i = 0; i < this._textures.Length; ++i)
            {
                _textures[i].RTView.Dispose();
                _textures[i].SRView.Dispose();

                if (_textures[i].DepthBuffer != null)
                    _textures[i].DepthBuffer.Dispose();

                if (_textures[i].DepthStencilView != null)
                    _textures[i].DepthStencilView.Dispose();
            }

            _textures = null;
            _currentIndex = -1;

            _size = new Sizei();
            _viewport = new ViewportF();

            _ovr = null;
            _sessionPtr = IntPtr.Zero;
            _textureSwapChainPtr = IntPtr.Zero;
        }
    }
}