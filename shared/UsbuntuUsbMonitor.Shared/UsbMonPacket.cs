using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbuntuUsbMonitor.Shared
{
    [Serializable]
    public class UsbMonPacket : IDisposable
    {
        public UsbMonPacket()
        {

        }

        public UsbMonPacket(UsbMonPacketStruct RawPacket)
        {
            this.URB_ID = Convert.ToInt64(RawPacket.id);
            this.DeviceID = Convert.ToByte(RawPacket.devnum);
            this.EndpointID = Convert.ToByte(RawPacket.epnum);
            this.BudID = Convert.ToByte(RawPacket.t_busnum);

            //
            // Offsets from here: http://www.kernel.org/doc/Documentation/usb/usbmon.txt
        }

        public long URB_ID { get; set; }

        public byte DeviceID { get; set; }

        public byte EndpointID { get; set; }

        public byte BudID { get; set; }


        #region IDisposable implementation
        public void Dispose()
        {
            // Nothing to dispose
        }
        #endregion
    }
}
