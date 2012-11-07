// //
// //
// // (c) Copyright Walletex Microelectronics Ltd. Israel
using System;
using ZeroMQ;

namespace DevTest.Sockets
{
    internal enum OpStatus
    {
        Unknown,
        Incomplete,
        Interupted,
        Success,
        TryAgain
    }

    internal static class SocketStatusConverter
    {
        internal static OpStatus Convert(this ZeroMQ.SendStatus ConvertStatus)
        {
            OpStatus stat = OpStatus.Unknown;

            switch(ConvertStatus)
            {
                case SendStatus.Incomplete:
                    stat = OpStatus.Incomplete;
                    break;
                case SendStatus.Interrupted:
                    stat = OpStatus.Interupted;
                    break;
                case SendStatus.None:
                    stat = OpStatus.Unknown;
                    break;
                case SendStatus.Sent:
                    stat = OpStatus.Success;
                    break;
                case SendStatus.TryAgain:
                    stat = OpStatus.TryAgain;
                    break;
                default:
                    stat = OpStatus.Unknown;
                    break;
            }

            return stat;
        }

        internal static OpStatus Convert (this ZeroMQ.ReceiveStatus ConvertStatus)
        {
            OpStatus stat = OpStatus.Unknown;

            switch(ConvertStatus)
            {
                case ReceiveStatus.Interrupted:
                    stat = OpStatus.Interupted;
                    break;
                case ReceiveStatus.None:
                    stat = OpStatus.Unknown;
                    break;
                case ReceiveStatus.Received:
                    stat = OpStatus.Success;
                    break;
                case ReceiveStatus.TryAgain:
                    stat = OpStatus.TryAgain;
                    break;
                default:
                    stat = OpStatus.Unknown;
                    break;
            }

            return stat;
        }

    }
}


