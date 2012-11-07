using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZeroMQ;
using UbuntuUsbMonitor.Network.Socket;
using UbuntuUsbMonitor.Network.Monitor;

namespace UbuntuUsbMonitor.Network
{
    public class NetworkKernel : IDisposable
    {
        private string endpoint;
        private PacketReceiver packetReceiver = null;

        //public EventHandler<NewPacketEventArgs> OnNewPacket;
        public event EventHandler<UsbMonPacketEventArgs> OnNewPacketReceived;

        public NetworkKernel() : this (string.Empty)
        {
        
        }

        public NetworkKernel(string Endpoint = "")
        {
            endpoint = Endpoint;
            packetReceiver = new PacketReceiver(endpoint);
            packetReceiver.OnNewPacketReceived += (s, e) => {
                if (OnNewPacketReceived != null)
                    OnNewPacketReceived(this, new UsbMonPacketEventArgs(e.Data.Serialize()));
                System.Diagnostics.Trace.WriteLine("[NetworkKernel] Received packet size: " + e.Data.Length.ToString());
            };

            packetReceiver.Start();
        }

        internal string Endpoint
        {
            get {return endpoint;}
            set
            {
                throw(new NotImplementedException("Can not bind dynamically"));
            }
        }


        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
