using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;

namespace UbuntuUsbMonitor.Network.Socket
{
    internal abstract class ReplaySocket : SocketBase, IDisposable
    {
        private ZmqMessage zOkMessage = null;

        internal ReplaySocket(string Address)
            : base(Address)
        {
            zOkMessage = new ZmqMessage();
            zOkMessage.Append(System.Text.Encoding.Unicode.GetBytes("Ok"));
        }

        protected virtual void OnMessageReceived(byte [] Message)
        {

        }

        protected override OpStatus BeforeSend()
        {
#if DEBUG
            System.Diagnostics.Trace.WriteLine("[ReplaySocket] Receive message on thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
#endif
            ZmqMessage receivedMessage = Socket.ReceiveMessage();
            if (Socket.ReceiveStatus == ReceiveStatus.Received)
            {
                byte[] data = receivedMessage.SelectMany((frame) => { return frame.Buffer; }).ToArray();
                OnMessageReceived(data);
            }

            return Socket.ReceiveStatus.Convert();
        }

        protected override OpStatus SendCore(ZmqMessage Message)
        {
#if DEBUG
            System.Diagnostics.Trace.WriteLine("[ReplaySocket] Send message on thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
#endif

            SendStatus status = Socket.SendMessage(Message);
            return status.Convert();
        }

        protected override OpStatus AfterSend(ZmqMessage Message)
        {
            return OpStatus.Unknown;
        }

        internal OpStatus DoWork()
        {
            OpStatus status = BeforeSend();
            if (status == OpStatus.Success)
                status = SendCore(zOkMessage);

            return status;
        }

        protected abstract void Start();

        protected abstract void Stop();

        public void Dispose()
        {
            base.Dispose();
            // Nothing more to dispose
        }
    }
}
