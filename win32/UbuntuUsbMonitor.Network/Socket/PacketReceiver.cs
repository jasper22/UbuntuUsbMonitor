
namespace UbuntuUsbMonitor.Network.Socket
{
    using System;
    using System.Collections;
    using System.Threading;
    using System.Threading.Tasks;
    using UbuntuUsbMonitor.Network.Monitor;
    using Tracer = System.Diagnostics.Trace;

    internal class PacketReceiver : ZeroMqBase, IDisposable
    {
        private Queue packetReceiver = null;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private ReaderWriterLockSlim readerWriterLock = null;

        internal EventHandler<NewPacketEventArgs> OnNewPacketReceived;

        internal PacketReceiver(string LocalAddress) : base (LocalAddress)
        {
            packetReceiver = new Queue();
            tokenSource = new CancellationTokenSource();
            readerWriterLock = new ReaderWriterLockSlim();

            tokenSource.Token.Register(() => { 
                // Clear on cancel
                packetReceiver.Clear(); 
            });


            var thReceiveQueue = Task.Factory.StartNew(() => {
                while (tokenSource.Token.IsCancellationRequested == false)
                {
                    readerWriterLock.EnterUpgradeableReadLock();
                    if (packetReceiver.Count > 0)
                    {
                        readerWriterLock.EnterWriteLock();

                        byte[] data = (byte[])packetReceiver.Dequeue();

                        readerWriterLock.ExitWriteLock();

                        if (OnNewPacketReceived != null)
                            OnNewPacketReceived(this, new NewPacketEventArgs(data));
                    }

                    readerWriterLock.ExitUpgradeableReadLock();
                }
            });

        }

        public void Dispose()
        {
            base.Dispose();

            if (tokenSource != null)
                tokenSource.Cancel();
        }

        protected override void OnMessageReceived(byte[] Message)
        {
            try
            {
                readerWriterLock.EnterWriteLock();
                packetReceiver.Enqueue(Message);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }

        }

        internal new void Start()
        {
#if DEBUG
            Tracer.WriteLine("[PacketReceiver] Starting the process");
#endif
            // Start the process
            base.Start();
        }

        internal void Stop()
        {
            base.Stop();

            if (tokenSource != null)
                tokenSource.Cancel();
        }
    }
}
