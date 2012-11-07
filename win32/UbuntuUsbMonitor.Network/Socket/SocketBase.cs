using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;


namespace UbuntuUsbMonitor.Network.Socket
{
    internal abstract class SocketBase : IDisposable
    {
        private string address;

        internal SocketBase(string Address)
        {
            address = Address;
        }
        
        protected abstract ZeroMQ.ZmqSocket Socket { get; set; }

        protected abstract OpStatus BeforeSend();
        protected abstract OpStatus SendCore(ZmqMessage Message);
        protected abstract OpStatus AfterSend(ZmqMessage Message);

        internal virtual string Endpoint {get { return address; }}

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
