// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using System.Threading;
using ZeroMQ;

namespace DevTest.Sockets
{
    internal class RequestReplaySocket : ZeroMqBase
    {
        public RequestReplaySocket(string Address) : base (Address, SocketType.REQ)
        {

        }

        protected override OpStatus OnBeforeSend(ZmqSocket Socket)
        {
            // Nothing to do in Request/Replay socket
            return OpStatus.Success;
        }

        #region implemented abstract members of DevTest.Native.Sockets.ZeroMqBase
        protected override OpStatus OnSendComplete(ZmqSocket Socket)
        {
            // Request-replay socket (REQ) should immideatly receive message just after 'send'
#if DEBUG
            System.Console.WriteLine("[RequestReplaySocket] Before receiveng the package");
            System.Console.WriteLine("[RequestReplaySocket] Current thread ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
#endif

            ZmqMessage okMessage = Socket.ReceiveMessage(TimeSpan.FromSeconds(1));

#if DEBUG
            System.Console.WriteLine("[RequestReplaySocket] Replay message was received. Status is: " + Socket.ReceiveStatus.ToString());
#endif
            return Socket.ReceiveStatus.Convert();
        }
        #endregion

    }
}

