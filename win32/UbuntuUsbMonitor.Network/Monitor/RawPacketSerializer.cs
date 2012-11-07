using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using UbuntuUsbMonitor.Shared;

namespace UbuntuUsbMonitor.Network.Monitor
{
    internal static class RawPacketSerializer
    {
        internal static UsbMonPacket Serialize(this byte [] Data)
        {
            UsbMonPacket packet = null;

            using (MemoryStream mStream = new MemoryStream())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UsbMonPacket));

                mStream.Write(Data, 0, Data.Length);
                mStream.Seek(0, SeekOrigin.Begin);

                packet = xmlSerializer.Deserialize(mStream) as UsbMonPacket;
                if (packet == null)
                    throw (new ApplicationException("Provided packet could not be de-serialized"));

                return packet;
            }

            return packet;
        }
    }
}
