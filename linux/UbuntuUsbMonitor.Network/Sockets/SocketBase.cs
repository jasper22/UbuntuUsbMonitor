// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using ZeroMQ;

namespace DevTest.Sockets
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
        protected abstract OpStatus AfterSend();

        internal virtual string Endpoint {get { return address; }}

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}

