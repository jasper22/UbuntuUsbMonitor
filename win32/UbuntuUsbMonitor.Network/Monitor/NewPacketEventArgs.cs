using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbuntuUsbMonitor.Network.Monitor
{
    internal class NewPacketEventArgs : EventArgs
    {
        private byte[] data;

        internal NewPacketEventArgs(byte [] Data) : base ()
        {
            data = new byte[Data.Length];
            Array.Copy(Data, data, data.Length);
        }

        internal byte [] Data
        {
            get { return data; }
        }
    }
}
