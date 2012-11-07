using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.ConstrainedExecution;

namespace DevTest
{
	/// <summary>
	/// Native low-level functions
	/// </summary>
	internal static class PInvoke
	{
		/// <summary>
		/// Close the specified handle.
		/// </summary>
		/// <param name='handle'>
		/// Handle.
		/// </param>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
	    internal static extern int Close(IntPtr handle);

		[DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
		internal static extern int Ioctl(SafeUnixHandle handle, int request);

		[DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
		internal static extern int Ioctl(SafeUnixHandle handle, int request, [In, Out] ref Mon_bin_stats Status);

		[DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
		internal static extern int Ioctl(SafeUnixHandle handle, int request, [In, Out] ref mon_get_arg NextPacket);


		[DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
		internal static extern int Ioctl(SafeUnixHandle handle, int request, int BufferSizeBytes);


//	    [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
//	    internal static extern int Ioctl(SafeUnixHandle handle, uint request, ref Capability capability);

	    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
	    internal static extern SafeUnixHandle Open(string path, uint flag, int mode);

	    internal static string Strerror(int error)
	    {
	        try
	        {
	            var buffer = new StringBuilder(256);
	            var result = Strerror(error, buffer, (ulong)buffer.Capacity);
	            return (result != -1) ? buffer.ToString() : null;
	        }
	        catch (EntryPointNotFoundException)
	        {
	            return null;
	        }
	    }

	    [DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_strerror_r", SetLastError = true)]
	    private static extern int Strerror(int error, [Out] StringBuilder buffer, ulong length);
	}


	/// <summary>
	/// Mon_bin_stats.
	/// </summary>
	/// <remarks>sizeof(Mon_bin_stats) = 8 bytes</remarks>
	[StructLayout(LayoutKind.Sequential)]
	internal struct Mon_bin_stats
	{
		/// <summary>
		/// The member "queued" refers to the number of events currently queued in the buffer (and not to the number 
		/// of events processed since the last reset).
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public uint queued;

		/// <summary>
		/// The member "dropped" is the number of events lost since the last call to MON_IOCG_STATS.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public uint dropped;
	}
}



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
	[MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
	public byte [] setup;
}

/// <summary>
/// Mon_get_arg.
/// </summary>
/// <remarks>Sizeof this struct = 12 bytes</remarks>
[StructLayout(LayoutKind.Sequential)]
public struct mon_get_arg
{

	/// <summary>
	/// Pointer to <see cref="UsbMonPacketStruct"/>
	/// </summary>
	/// <remarks>Size const = 48 bytes</remarks>
	internal IntPtr hdr;

	/// <summary>
	/// The data.
	/// </summary>
	internal IntPtr data;

	/// <summary>
	/// Size of data
	/// </summary>
	internal int alloc;
}
