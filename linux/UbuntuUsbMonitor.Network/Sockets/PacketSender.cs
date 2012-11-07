// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DevTest.Monitor;

namespace DevTest.Sockets
{
    internal class PacketSender : PacketDispatcher
    {
        internal PacketSender(string ServerAddress) : base (ServerAddress)
        {

        }

        internal void SendPacket(UsbMonPacket PacketToSend)
        {
            byte [] data;
            XmlSerializer serializer = new XmlSerializer(typeof(UsbMonPacket));

            using(MemoryStream mStream = new MemoryStream())
            {
                serializer.Serialize(mStream, PacketToSend);
                mStream.Seek(0, SeekOrigin.Begin);
                data = new byte[mStream.Length];
                mStream.Read(data,0, data.Length);
                base.AddMessageRaw(data);
            }
        }
    }
}

