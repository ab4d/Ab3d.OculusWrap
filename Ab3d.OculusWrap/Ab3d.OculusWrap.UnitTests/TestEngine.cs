using System;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Ab3d.OculusWrap.UnitTests
{
	/// <summary>
	/// Very primitive graphics engine, used for passing Direct3D pointers to the OVR methods.
	/// </summary>
	public class TestEngine	:IDisposable
	{
		public TestEngine(int width, int height)
		{
			if(width == 0)
				throw new ArgumentOutOfRangeException("width", "The width argument is not allowed to be 0.");
			if(height == 0)
				throw new ArgumentOutOfRangeException("height", "The height argument is not allowed to be 0.");

			// Create a window for the Direct3D device.
			Window		= new System.Windows.Forms.Form();

			// Define the swap chain we want.
			SwapChainDescription swapChainDescription						= new SwapChainDescription();
			swapChainDescription.BufferCount								= 2;
			swapChainDescription.IsWindowed									= true;
			swapChainDescription.OutputHandle								= Window.Handle;
			swapChainDescription.SampleDescription							= new SampleDescription(1, 0);
			swapChainDescription.Usage										= Usage.RenderTargetOutput;
			swapChainDescription.SwapEffect									= SwapEffect.Discard;
			swapChainDescription.ModeDescription.Width						= width;
			swapChainDescription.ModeDescription.Height						= height;
			swapChainDescription.ModeDescription.Format						= Format.R8G8B8A8_UNorm;
			swapChainDescription.ModeDescription.RefreshRate.Numerator		= 0;
			swapChainDescription.ModeDescription.RefreshRate.Denominator	= 1;

			// Create device and swap chain.
			SharpDX.Direct3D11.Device	device;
			SharpDX.DXGI.SwapChain		swapChain;
			SharpDX.Direct3D11.Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.Debug, swapChainDescription, out device, out swapChain);
			Device		= device;
			SwapChain	= swapChain;

			// Create render target view.
			BackBuffer			= swapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>(0);
			RenderTargetView	= new RenderTargetView(device, BackBuffer);

			// Define a texture at the same size as the render target.
			Texture2DDescription texture2DDescription	= new Texture2DDescription();
			texture2DDescription.Width					= width;
			texture2DDescription.Height					= height;
			texture2DDescription.ArraySize				= 1;
			texture2DDescription.MipLevels				= 1;
			texture2DDescription.Format					= Format.R8G8B8A8_UNorm;
			texture2DDescription.SampleDescription		= new SampleDescription(1, 0);
			texture2DDescription.BindFlags				= BindFlags.ShaderResource | BindFlags.RenderTarget;
			texture2DDescription.Usage					= ResourceUsage.Default;
			texture2DDescription.CpuAccessFlags			= CpuAccessFlags.None;
			Texture2DDescription						= texture2DDescription;

			// Create a texture at the same size as the render target.
			Texture = new Texture2D(Device, texture2DDescription);

			// Create a shader resource view for the texture.
			TextureShaderResourceView = new ShaderResourceView(device, Texture);
		}

		#region IDisposable Members
		public void Dispose()
		{
			Window.Dispose();
			Device.Dispose();
			SwapChain.Dispose();
			BackBuffer.Dispose();
			RenderTargetView.Dispose();
			Texture.Dispose();
			TextureShaderResourceView.Dispose();
		}
		#endregion
		
		public System.Windows.Forms.Form	Window					{get; protected set;}
		public SharpDX.Direct3D11.Device	Device					{get; protected set;}
		public SharpDX.DXGI.SwapChain		SwapChain				{get; protected set;}
		public Texture2DDescription			Texture2DDescription	{get; protected set;}
		public Texture2D					BackBuffer				{get; protected set;}
		public Texture2D					Texture					{get; protected set;}
		public RenderTargetView				RenderTargetView		{get; protected set;}
		public ShaderResourceView			TextureShaderResourceView	{get; protected set;}
	}
}
