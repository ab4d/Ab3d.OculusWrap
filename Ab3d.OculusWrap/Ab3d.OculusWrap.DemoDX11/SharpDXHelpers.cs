using SharpDX;
using SharpDX.Direct3D11;

namespace Ab3d.OculusWrap.DemoDX11
{
    public static class SharpDXHelpers
    {
        /// <summary>
        /// Convert a Vector4 to a Vector3
        /// </summary>
        /// <param name="vector4">Vector4 to convert to a Vector3.</param>
        /// <returns>Vector3 based on the X, Y and Z coordinates of the Vector4.</returns>
        public static Vector3 ToVector3(this Vector4 vector4)
        {
            return new Vector3(vector4.X, vector4.Y, vector4.Z);
        }

        /// <summary>
        /// Convert an ovrVector3f to SharpDX Vector3.
        /// </summary>
        /// <param name="ovrVector3f">ovrVector3f to convert to a SharpDX Vector3.</param>
        /// <returns>SharpDX Vector3, based on the ovrVector3f.</returns>
        public static Vector3 ToVector3(this Vector3f ovrVector3f)
        {
            return new Vector3(ovrVector3f.X, ovrVector3f.Y, ovrVector3f.Z);
        }

        /// <summary>
        /// Convert an ovrMatrix4f to a SharpDX Matrix.
        /// </summary>
        /// <param name="ovrMatrix4f">ovrMatrix4f to convert to a SharpDX Matrix.</param>
        /// <returns>SharpDX Matrix, based on the ovrMatrix4f.</returns>
        public static Matrix ToMatrix(this Matrix4f ovrMatrix4f)
        {
            return new Matrix(ovrMatrix4f.M11, ovrMatrix4f.M12, ovrMatrix4f.M13, ovrMatrix4f.M14, ovrMatrix4f.M21, ovrMatrix4f.M22, ovrMatrix4f.M23, ovrMatrix4f.M24, ovrMatrix4f.M31, ovrMatrix4f.M32, ovrMatrix4f.M33, ovrMatrix4f.M34, ovrMatrix4f.M41, ovrMatrix4f.M42, ovrMatrix4f.M43, ovrMatrix4f.M44);
        }

        /// <summary>
        /// Converts an ovrQuatf to a SharpDX Quaternion.
        /// </summary>
        public static Quaternion ToQuaternion(Quaternionf ovrQuatf)
        {
            return new Quaternion(ovrQuatf.X, ovrQuatf.Y, ovrQuatf.Z, ovrQuatf.W);
        }

        /// <summary>
        /// Creates a TextureSwapChainDesc, based on a specified SharpDX texture description.
        /// </summary>
        /// <param name="texture2DDescription">SharpDX texture description.</param>
        /// <returns>TextureSwapChainDesc, based on the SharpDX texture description.</returns>
        public static TextureSwapChainDesc CreateTextureSwapChainDescription(Texture2DDescription texture2DDescription)
        {
            TextureSwapChainDesc textureSwapChainDescription	= new TextureSwapChainDesc();
            textureSwapChainDescription.Type							= TextureType.Texture2D;
            textureSwapChainDescription.Format							= GetTextureFormat(texture2DDescription.Format);
            textureSwapChainDescription.ArraySize						= (int)	texture2DDescription.ArraySize;
            textureSwapChainDescription.Width							= (int)	texture2DDescription.Width;
            textureSwapChainDescription.Height							= (int)	texture2DDescription.Height;
            textureSwapChainDescription.MipLevels						= (int)	texture2DDescription.MipLevels;
            textureSwapChainDescription.SampleCount						= (int)	texture2DDescription.SampleDescription.Count;
            textureSwapChainDescription.StaticImage						= false;
            textureSwapChainDescription.MiscFlags						= GetTextureMiscFlags(texture2DDescription, false);
            textureSwapChainDescription.BindFlags						= GetTextureBindFlags(texture2DDescription.BindFlags);

            return textureSwapChainDescription;
        }

        /// <summary>
        /// Translates a DirectX texture format into an Oculus SDK texture format.
        /// </summary>
        /// <param name="textureFormat">DirectX texture format to translate into an Oculus SDK texture format.</param>
        /// <returns>
        /// Oculus SDK texture format matching the specified textureFormat or TextureFormat.Unknown if a match count not be found.
        /// </returns>
        public static TextureFormat GetTextureFormat(SharpDX.DXGI.Format textureFormat)
        {
            switch(textureFormat)
            {
                case SharpDX.DXGI.Format.B5G6R5_UNorm:			return TextureFormat.B5G6R5_UNorm;
                case SharpDX.DXGI.Format.B5G5R5A1_UNorm:		return TextureFormat.B5G5R5A1_UNorm;
                case SharpDX.DXGI.Format.R8G8B8A8_UNorm:		return TextureFormat.R8G8B8A8_UNorm;
                case SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb:	return TextureFormat.R8G8B8A8_UNorm_SRgb;
                case SharpDX.DXGI.Format.B8G8R8A8_UNorm:		return TextureFormat.B8G8R8A8_UNorm;
                case SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb:	return TextureFormat.B8G8R8A8_UNorm_SRgb;
                case SharpDX.DXGI.Format.B8G8R8X8_UNorm:		return TextureFormat.B8G8R8X8_UNorm;
                case SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb:	return TextureFormat.B8G8R8X8_UNorm_SRgb;
                case SharpDX.DXGI.Format.R16G16B16A16_Float:	return TextureFormat.R16G16B16A16_Float;
                case SharpDX.DXGI.Format.D16_UNorm:				return TextureFormat.D16_UNorm;
                case SharpDX.DXGI.Format.D24_UNorm_S8_UInt:		return TextureFormat.D24_UNorm_S8_UInt;
                case SharpDX.DXGI.Format.D32_Float:				return TextureFormat.D32_Float;
                case SharpDX.DXGI.Format.D32_Float_S8X24_UInt:	return TextureFormat.D32_Float_S8X24_UInt;

                case SharpDX.DXGI.Format.R8G8B8A8_Typeless:		return TextureFormat.R8G8B8A8_UNorm;
                case SharpDX.DXGI.Format.R16G16B16A16_Typeless:	return TextureFormat.R16G16B16A16_Float;
	
                default:										return TextureFormat.Unknown;
            }
        }

        /// <summary>
        /// Creates a set of TextureMiscFlags, based on a specified SharpDX texture description and a mip map generation flag.
        /// </summary>
        /// <param name="texture2DDescription">SharpDX texture description.</param>
        /// <param name="allowGenerateMips">
        /// When set, allows generation of the mip chain on the GPU via the GenerateMips
        /// call. This flag requires that RenderTarget binding also be specified.
        /// </param>
        /// <returns>Created TextureMiscFlags, based on the specified SharpDX texture description and mip map generation flag.</returns>
        public static TextureMiscFlags GetTextureMiscFlags(Texture2DDescription texture2DDescription, bool allowGenerateMips)
        {
            TextureMiscFlags results = TextureMiscFlags.None;

            if(texture2DDescription.Format == SharpDX.DXGI.Format.R8G8B8A8_Typeless || texture2DDescription.Format == SharpDX.DXGI.Format.R16G16B16A16_Typeless)
                results |= TextureMiscFlags.DX_Typeless;

            if(texture2DDescription.BindFlags.HasFlag(BindFlags.RenderTarget) && allowGenerateMips)
                results |= TextureMiscFlags.AllowGenerateMips;

            return results;
        }

        /// <summary>
        /// Retrieves a list of flags matching the specified DirectX texture binding flags.
        /// </summary>
        /// <param name="bindFlags">DirectX texture binding flags to translate into Oculus SDK texture binding flags.</param>
        /// <returns>Oculus SDK texture binding flags matching the specified bindFlags.</returns>
        public static TextureBindFlags GetTextureBindFlags(SharpDX.Direct3D11.BindFlags bindFlags)
        {
            TextureBindFlags result = TextureBindFlags.None;

            if(bindFlags.HasFlag(SharpDX.Direct3D11.BindFlags.DepthStencil))
                result |= TextureBindFlags.DX_DepthStencil;

            if(bindFlags.HasFlag(SharpDX.Direct3D11.BindFlags.RenderTarget))
                result |= TextureBindFlags.DX_RenderTarget;

            if(bindFlags.HasFlag(SharpDX.Direct3D11.BindFlags.UnorderedAccess))
                result |= TextureBindFlags.DX_DepthStencil;

            return result;
        }
    }
}