using System.Runtime.InteropServices;
using System.Text;

namespace Ab3d.OculusWrap
{
    public struct ExternalCamera
    {
        /// <summary>
        /// Byte array for camera Name string
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] NameBytes;

        /// <summary>
        /// Camera Name
        /// </summary>
        public string Name
        {
            get
            {
                return OvrWrap.GetAsciiString(NameBytes);
            }
        }

        public CameraIntrinsics Intrinsics;
        public CameraExtrinsics Extrinsics;
    }
}