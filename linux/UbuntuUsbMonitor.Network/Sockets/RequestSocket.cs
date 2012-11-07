// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using ZeroMQ;

namespace DevTest.Sockets
{
    internal abstract class RequestSocket : SocketBase, IDisposable
    {
        private ZmqMessage zOkMessage = null;

        internal RequestSocket(string Address)
            : base(Address)
        {
            zOkMessage = new ZmqMessage();
            zOkMessage.Append(System.Text.Encoding.Unicode.GetBytes("Ok"));
        }

        #region implemented abstract members of DevTest.Sockets.SocketBase
        protected override OpStatus BeforeSend()
        {
            return OpStatus.Success;
        }

        protected override OpStatus SendCore(ZmqMessage Message)
        {
            System.Console.WriteLine("[RequestSocket] Message will be send on thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            SendStatus status = Socket.SendMessage(Message);
            return status.Convert();
        }

        protected override OpStatus AfterSend()
        {
            System.Console.WriteLine("[RequestSocket] Message will be received on thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            ZmqMessage okMessage = Socket.ReceiveMessage();
            return Socket.ReceiveStatus.Convert();
        }
        #endregion


       internal OpStatus DoWork(ZmqMessage Message)
        {
            OpStatus status = SendCore(Message);
            if (status == OpStatus.Success)
                return AfterSend();

            return status;
        }

        public void Dispose()
        {
            base.Dispose();
            // Nothing to dispose here
        }
    }
}

