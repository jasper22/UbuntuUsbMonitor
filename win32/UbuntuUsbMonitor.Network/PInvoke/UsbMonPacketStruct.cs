
namespace UbuntuUsbMonitor.Network.PInvoke
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Usb mon packet struct.
    /// </summary>
    /// <remarks>sizeof(UsbMonPacketStruct) should be equal 48</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct UsbMonPacketStruct
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        /// <remarks>0: URB ID - from submission to callback</remarks>
        // [MarshalAs(UnmanagedType.U8)]
        public ulong id;

        /// <summary>
        /// The ttype.
        /// </summary>
        /// <remarks>8: Same as text; extensible</remarks>
        //[MarshalAs(UnmanagedType.U2)]
        public char ttype;

        /// <summary>
        /// The xfer_type (ISO (0), Intr, Control, Bulk (3))
        /// </summary>
        /// <remarks>9: ISO (0), Intr, Control, Bulk (3)</remarks>
        //[MarshalAs(UnmanagedType.U2)]
        public char xfer_type;

        /// <summary>
        /// The endpoint (Endpoint number and transfer direction)
        /// </summary>
        /// <remarks>10: Endpoint number and transfer direction</remarks>
        //[MarshalAs(UnmanagedType.U2)]
        public char epnum;

        /// <summary>
        /// Device address
        /// </summary>
        /// <remarks>11: Device address</remarks>
        //[MarshalAs(UnmanagedType.U2)]
        public char devnum;

        /// <summary>
        /// Bus number
        /// </summary>
        /// <remarks>12: Bus number</remarks>
        //[MarshalAs(UnmanagedType.U2)]
        public short t_busnum;

        /// <summary>
        /// The flag_setup.
        /// </summary>
        /// <remarks>14: Same as text</remarks>
        //[MarshalAs(UnmanagedType.U1)]
        public char flag_setup;

        /// <summary>
        /// The flag_data.
        /// </summary>
        /// <remarks>15: Same as text (binary 0 is ok)</remarks>
        //[MarshalAs(UnmanagedType.U1)]
        public char flag_data;

        /// <summary>
        /// The ts_sec.
        /// </summary>
        /// <remarks>16: GetTimeOfDay</remarks>
        //[MarshalAs(UnmanagedType.U8)]
        public long ts_sec;

        /// <summary>
        /// The ts_usec.
        /// </summary>
        /// <remarks>24: GetTimeOfDay</remarks>
        //[MarshalAs(UnmanagedType.U4)]
        public int ts_usec;

        /// <summary>
        /// The status.
        /// </summary>
        public int status;

        /// <summary>
        /// Length of data (submitted or actual)
        /// </summary>
        /// <remarks>32: Length of data (submitted or actual)</remarks>
        public uint lenght;

        /// <summary>
        /// Delivered length
        /// </summary>
        /// <remarks>36: Delivered length</remarks>
        public uint lenght_cap;

        /// <summary>
        /// The setup.
        /// </summary>
        /// <remarks>40: Only for Control S-type</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] setup;
    }
}
