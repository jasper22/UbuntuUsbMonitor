using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbuntuUsbMonitor.Shared;

namespace UbuntuUsbMonitor.Network.Monitor
{
    public class UsbMonPacketEventArgs : EventArgs
    {
        private UsbMonPacket packet = null;

        public UsbMonPacketEventArgs (UsbMonPacket Packet) : base ()
        {
            packet = Packet;
        }

        public UsbMonPacket Packet
        {
            get { return packet; }
            set{packet = value;}
        }
    }
}
