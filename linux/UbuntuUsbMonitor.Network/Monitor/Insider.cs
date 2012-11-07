using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace DevTest.Monitor
{
	internal sealed class Insider : IDisposable
	{
		private readonly string devAddress;	// = @"/dev/usbmon1";
		private SafeUnixHandle safeHandle;
		private const int MON_IOC_MAGIC	= 0x92;
		private readonly int REQUEST_NEXT_DATA_LENGHT = BitShifter._IO(MON_IOC_MAGIC, 1);
		private readonly int MON_IO_REQUEST_RING_SIZE = BitShifter._IO(MON_IOC_MAGIC, 5);	// Current buffer size
		private readonly int MON_IO_SET_RING_SIZE	=	BitShifter._IO(MON_IOC_MAGIC, 4);	// Set buffer size
		private const int USBMON_MAX_SIZE = 65536;

		private PacketManager packetManager;

		public event EventHandler<UsbMonEvent> OnNewEvents;

		internal Insider (string DevicePath = @"/dev/usbmon1")
		{
			devAddress = DevicePath;

			try 
			{
				safeHandle = PInvoke.Open (devAddress, Convert.ToUInt32 (FileMode.Open), Convert.ToInt32 (FileAccess.Read));
                if (safeHandle.IsInvalid == true)
                {
                    throw(new System.Security.SecurityException("Only 'root' could open access to UsbMonitor device"));
                }

				packetManager = new PacketManager(safeHandle);
				packetManager.OnNewEvents += (sender, e) => {
					if (this.OnNewEvents != null)
						this.OnNewEvents(this, e);
				};
			} 
			catch (Exception exp_gen) 
			{
				//throw(new UnixIOException(PInvoke.Strerror(System.Runtime.InteropServices.Marshal.GetExceptionCode()),exp_gen));
                throw;
			}
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			if (safeHandle != null)
				safeHandle.Close();

			safeHandle = null;
		}
		#endregion

		/// <summary>
		/// Gets the size of the next event 'data' block (in bytes)
		/// </summary>
		/// <value>
		/// The size of the next event 'data' block (in bytes)
		/// </value>
		/// <remarks>
		/// This call returns the length of data in the next event. Note that majority of
		/// events contain no data, so if this call returns zero, it does not mean that
		/// no events are available.
		/// </remarks>
		internal int NextEventDataSize 
		{
			get 
			{
				return PInvoke.Ioctl(safeHandle, REQUEST_NEXT_DATA_LENGHT);
			}
		}

		/// <summary>
		/// Gets or sets the size of the buffer (int bytes)
		/// </summary>
		/// <value>
		/// The size of the buffer.
		/// </value>
		/// <remarks>Buffer size in bytes</remarks>
		internal int BufferSize 
		{
			get 
			{
				int res = 0;
				res = PInvoke.Ioctl(safeHandle, MON_IO_REQUEST_RING_SIZE);
				return res;
			}
			set
			{
				int setSize = value;
				int res = PInvoke.Ioctl(safeHandle, MON_IO_SET_RING_SIZE, setSize);
				if (res != 0)
					throw(new ApplicationException("Error here may be ?"));
			}
		}

		internal IEnumerable<UsbMonPacket> Packets 
		{
			get{ return packetManager;}
		}

		private sealed class PacketManager : IEnumerable<UsbMonPacket>, IEnumerator<UsbMonPacket>
		{
			private SafeUnixHandle safeHandle2;
			private readonly int MON_IOCX_GETX_PACKET	=	BitShifter._IOW(MON_IOC_MAGIC, 6, 12);		// 12 = SizeOf(mon_get_arg)
			private readonly int REQUEST_STATUS = BitShifter._IOR(MON_IOC_MAGIC, 3, Marshal.SizeOf(typeof(Mon_bin_stats)));

			internal event EventHandler<UsbMonEvent> OnNewEvents;

			private System.Threading.Timer watchEventsTimer;

			internal PacketManager(SafeUnixHandle SafeHandle)
			{
				safeHandle2 = SafeHandle;
				watchEventsTimer= new Timer(_ => {
					int events = EventQueuedCount();
					if (events > 0)
						if (OnNewEvents != null)
							OnNewEvents(this, new UsbMonEvent() { Count = events });
				}
				, null
				, TimeSpan.FromSeconds(0)	// start immideatly
				, TimeSpan.FromSeconds(1));
			}

			#region IEnumerable implementation
			public IEnumerator GetEnumerator ()
			{
				return this;
			}
			#endregion

			#region IEnumerable implementation
			IEnumerator<UsbMonPacket> IEnumerable<UsbMonPacket>.GetEnumerator ()
			{
				return this;
			}
			#endregion

			#region IDisposable implementation
			public void Dispose ()
			{
				//throw new System.NotImplementedException ();
			}
			#endregion

			private int EventQueuedCount ()
			{
				Mon_bin_stats returnedStruct = new Mon_bin_stats ();

				try 
				{
					int res = PInvoke.Ioctl (safeHandle2, REQUEST_STATUS, ref returnedStruct);
					if (res == 0) 
                    {
						return  Convert.ToInt32(returnedStruct.queued);
					} 
                    else
						throw(new ApplicationException ("Error here may be ?"));
				} 
				catch (Exception exp_gen) 
				{
					throw(new ApplicationException("Error occurred while trying to request Ioctl. Error is: " + exp_gen.ToString(), exp_gen));
				}
			}

			#region IEnumerator implementation
			public bool MoveNext ()
			{
				return EventQueuedCount() > 0;
			}

			public void Reset ()
			{
				// Do nothing
			}

			public object Current 
			{
				get {return GetPacket();}
			}
			#endregion

			#region IEnumerator implementation
			UsbMonPacket IEnumerator<UsbMonPacket>.Current 
			{
				get {return GetPacket();}
			}
			#endregion

			private UsbMonPacket GetPacket ()
			{
				mon_get_arg struct1 = new mon_get_arg ();
				int res =0;

				try 
				{
					struct1.alloc = USBMON_MAX_SIZE;
					struct1.data = Marshal.AllocHGlobal (USBMON_MAX_SIZE);
					struct1.hdr = Marshal.AllocHGlobal (48);

					res = PInvoke.Ioctl (this.safeHandle2, MON_IOCX_GETX_PACKET, ref struct1);
					if (res == 0)
					{
						UsbMonPacketStruct packet = new UsbMonPacketStruct ();
						packet = (UsbMonPacketStruct)Marshal.PtrToStructure (struct1.hdr, typeof(UsbMonPacketStruct));
						return new UsbMonPacket(packet);
					}
					else
					{
						throw(new ApplicationException("Error occurred in IoCtrl(). Function return: " + res.ToString()));
					}
				} 
				catch (Exception exp_gen) 
				{
					throw(new ApplicationException("Error occurred while trying to access UsbMonitor", exp_gen));
				} 
				finally 
				{
					Marshal.FreeHGlobal(struct1.data);
					Marshal.FreeHGlobal(struct1.hdr);
				}

			}
		}
	}
}

