// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTest.Sockets
{
    internal class PacketDispatcher : ZeroMqBase, IDisposable
    {
        private Queue packetsToSend = null;
        private CancellationTokenSource tokenSource = null;
        private ReaderWriterLockSlim readerWriterlock = new ReaderWriterLockSlim();


        internal PacketDispatcher(string ServerAddress) : base (ServerAddress)
        {
            packetsToSend = new Queue();
            tokenSource = new CancellationTokenSource();
            tokenSource.Token.Register(()=>{
                // Clean the queue on cancellation
                packetsToSend.Clear();
            });

            var thDispatchQueue = Task.Factory.StartNew(()=>{
                while(tokenSource.Token.IsCancellationRequested == false)
                {
                    readerWriterlock.EnterUpgradeableReadLock();
                    if (packetsToSend.Count > 0)
                    {
                        readerWriterlock.EnterWriteLock();
#if DEBUG
                        System.Console.WriteLine("[PacketDispatcher] Trying to take packet from queue");
#endif
                        while(packetsToSend.Count > 0)
                        {
                            byte[] singlePacket =  (byte[]) packetsToSend.Dequeue();

                            base.Send(singlePacket);
#if DEBUG
                            System.Console.WriteLine("[PacketDispatcher] Packet send. " + packetsToSend.Count.ToString() + " left");
#endif
                        }

                        readerWriterlock.ExitWriteLock();
                    }
                    readerWriterlock.ExitUpgradeableReadLock();

                    // Wait a little bit
                    tokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                    tokenSource.Token.ThrowIfCancellationRequested();
                }

            }
            , tokenSource.Token
            , TaskCreationOptions.LongRunning
            , TaskScheduler.Default);
        }

        public void Dispose()
        {
            base.Dispose();

            if (tokenSource != null)
                tokenSource.Cancel();

            tokenSource = null;
        }

        internal virtual int Count
        {
            get
            {
                int count = 0;

                try
                {
                    readerWriterlock.EnterReadLock();
                    count = packetsToSend.Count;
                    return count;
                }
                finally
                {
                    readerWriterlock.ExitReadLock();
                }
            }
        }

        internal void Stop()
        {
            if (tokenSource != null)
                tokenSource.Cancel();
        }

        internal void AddMessageRaw(byte [] Data)
        {
            try
            {
                readerWriterlock.EnterWriteLock();

                packetsToSend.Enqueue(Data);
#if DEBUG
                System.Console.WriteLine("[PacketDispatcher] Data was added to queue. Queue size: " + packetsToSend.Count.ToString());
#endif
            }
            finally
            {
                readerWriterlock.ExitWriteLock();
            }
        }
    }
}

