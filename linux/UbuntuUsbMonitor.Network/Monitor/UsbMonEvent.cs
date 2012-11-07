using System;

namespace DevTest.Monitor
{
	/// <summary>
	/// Usb mon event.
	/// </summary>
	public class UsbMonEvent : EventArgs
	{
		public UsbMonEvent () : base ()
		{

		}

		/// <summary>
		/// Count of events in buffer
		/// </summary>
		/// <value>
		/// The count of events
		/// </value>
		internal int Count { get; set; }
	}
}

