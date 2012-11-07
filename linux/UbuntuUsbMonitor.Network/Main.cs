// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using ZeroMQ;
using DevTest.Sockets;
using DevTest.Monitor;

namespace DevTest
{
    class MainClass
    {
		private const int URB_PACKET_SIZE_BYTES = 48;
		private const int MON_IOC_MAGIC	= 0x92;
        //private PackageSender packageSender;

        public static void Main(string[] args)
        {
            System.Console.WriteLine("Trying to open device");

            MemoryStream mStream = new MemoryStream();
            byte [] ddata = new byte[100];
            new Random().NextBytes(ddata);
            mStream.Write(ddata,0, ddata.Length);

            PacketSender packageSender;
            packageSender = new PacketSender("tcp://10.0.0.178:5555");

			Insider insider = new Insider();
            insider.OnNewEvents += (sender, e) => {

                var tt3 = (from singlePacket in insider.Packets
                            select singlePacket).ToList();

//                tt3.ForEach(x => {
//                    Console.WriteLine("UrbID: " + x.URB_ID.ToString());
//                });

                if (tt3.Any())
                {
                    System.Console.WriteLine("There's " + tt3.Count().ToString() + " events");

                    tt3.ForEach((packet) => {
                        packageSender.SendPacket(packet);
                    });
                }
                else
                    System.Console.WriteLine("There was no events");
            };

            while(packageSender.Count > 0)
            {
                System.Console.WriteLine("There's still " + packageSender.Count.ToString() + " packets waiting to be send. Sleeping" );
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
            }

            for(int iCounter =0; iCounter < 100; iCounter++)
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

            packageSender.Dispose();

			System.Console.WriteLine("Program end");
        }
    }
}
