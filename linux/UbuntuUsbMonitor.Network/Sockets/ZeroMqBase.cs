// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using System.Threading;
using ZeroMQ;
using System.Threading.Tasks;

namespace DevTest.Sockets
{
    internal class ZeroMqBase : RequestSocket, IDisposable
    {
        private static ZmqContext zContext = null;
        private static ZmqSocket zSocket = null;
        private string address;
        private CancellationTokenSource tokenSource;


        internal ZeroMqBase(string Address) : base(Address)
        {
            address = Address;
            tokenSource = new CancellationTokenSource();
        }


        private ZmqSocket CreateSocket()
        {
            if (zSocket != null)
                return zSocket;

            zContext = ZmqContext.Create();
            zSocket = zContext.CreateSocket(SocketType.REQ);
            zSocket.Connect(Endpoint);

#if DEBUG
            System.Console.WriteLine("[ZeroMqBase] Context created on thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
#endif

            return zSocket;
        }

                
        public void Dispose()
        {
            base.Dispose();

            if (zContext != null)
            {
                zContext.Terminate();
                zContext.Dispose();
            }

            zContext = null;
        }


        protected override ZmqSocket Socket
        {
            get {return CreateSocket(); }
            set
            {
                throw new NotImplementedException();
            }
        }

        internal OpStatus Send(byte [] Data)
        {
            ZmqMessage message = new ZmqMessage();
            message.Append(Data);
            return base.DoWork(message);
        }
    }
}

/*
 *         internal ZeroMqBase(string Address, SocketType TypeOfSocket)
        {
//            tokenSource = new CancellationTokenSource();

            address = Address;
            typeOfSocket = TypeOfSocket;
        }       

        #region IDisposable implementation
        public void Dispose()
        {
            if (zContext != null)
            {
                zContext.Terminate();
                zContext.Dispose();
            }

            zContext = null;
#if DEBUG
//            System.Console.WriteLine("[ZeroMqBase] Disposed");
#endif
        }
        #endregion

        private ZmqSocket CreateSocket()
        {
            if (this.zSocket != null)
                return zSocket;

            zContext = ZmqContext.Create();
#if DEBUG
//            System.Console.WriteLine("[ZeroMqBase] Context was created on " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()  + " thread ID ");
#endif

            zSocket = zContext.CreateSocket(typeOfSocket);
            try
            {
                zSocket.Connect(address);
#if DEBUG
//                System.Console.WriteLine("[ZeroMqBase] Connected to: " + address);
#endif
                return zSocket;
            }
            catch(Exception exp_gen)
            {
                if (zContext != null)
                {
                    zContext.Terminate();
                    zContext.Dispose();
                }

                zContext = null;
                throw;
            }

        }

        protected abstract OpStatus OnBeforeSend(ZmqSocket Socket);

        protected abstract OpStatus OnSendComplete(ZmqSocket Socket);

        protected virtual OpStatus SendMessage(byte[] Data)
        {
            if (zSocket == null)
                zSocket = CreateSocket();

#if DEBUG
            //System.Console.WriteLine("[ZeroMqBase] Context gonna be used " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()  + " thread ID ");
#endif

            OpStatus firstStep = OnBeforeSend(this.zSocket);
            if (firstStep == OpStatus.Success)
            {
                SendStatus stat = SendStatus.None;
                ZmqMessage message = new ZmqMessage();
                message.Append(Data);

#if DEBUG
                System.Console.WriteLine("[ZeroMqBase] Current thread is " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()  + " thread ID ");
#endif

                try
                {
                    stat = zSocket.SendMessage(message);
                }
                catch(Exception exp_gen)
                {
                    System.Console.WriteLine("Error here");
                }
#if DEBUG
                System.Console.WriteLine("[ZeroMqBase] Packet was send. Status is: " + stat.ToString());
#endif

                if (stat != SendStatus.Sent)
                    return stat.Convert();


                //return OnSendComplete(this.zSocket);
                ZmqMessage okMessage = null;

                try
                {
                    System.Console.WriteLine("[ZeroMqBase] Before packet receive");
                    okMessage = zSocket.ReceiveMessage();
                    System.Console.WriteLine("[ZeroMqBase] Packet was received back. Status is: " + zSocket.ReceiveStatus.ToString());
                    return zSocket.ReceiveStatus.Convert();
                }
                catch(Exception exp_gen)
                {
                    System.Console.WriteLine("Error here: " + exp_gen.ToString());
                    throw;
                }
            }
            else
                return firstStep;
        }
*/