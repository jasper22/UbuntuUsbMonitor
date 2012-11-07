
namespace UbuntuUsbMonitor.Network.Socket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ZeroMQ;

    using Tracer = System.Diagnostics.Trace;


    internal class ZeroMqBase : ReplaySocket, IDisposable
    {
        private static ZmqContext zContext = null;
        private static ZmqSocket zSocket = null;
        private CancellationTokenSource tokenSource;


        internal ZeroMqBase(string Address) : base (Address)
        {
            tokenSource = new CancellationTokenSource();
        }

        private ZmqSocket CreateSocket()
        {
            if (zSocket != null)
                return zSocket;

            zContext = ZmqContext.Create();
            zSocket = zContext.CreateSocket(SocketType.REP);
            zSocket.Bind(Endpoint);

#if DEBUG
            System.Diagnostics.Trace.WriteLine("[ZeroMqBase] Context created on thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
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

        protected override void Start()
        {
            Task.Factory.StartNew(() => {

                CreateSocket();

                while (true)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                        base.Dispose();

                    tokenSource.Token.ThrowIfCancellationRequested();

                    base.DoWork();
                }

            }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }


        protected override void Stop()
        {
            if (tokenSource != null)
                tokenSource.Cancel();
        }
    }
}