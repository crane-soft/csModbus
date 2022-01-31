using System;
using System.Net.Sockets;
using System.Threading;

namespace csModbusLib {
    public abstract class MbEthernet : MbInterface {
        public enum ModbusEthernetType {
            TCP = 0,
            UDP = 1
        }

        protected const int MBAP_Header_Size = 6;
        protected int remote_port;
        protected UdpClient mUdpClient;
        private UInt16 TransactionIdentifier;

        public MbEthernet()
        {
            TransactionIdentifier = 0;
        }

        protected void SetPort(int port)
        {
            remote_port = port;
        }

        protected void FillMBAPHeader(MbRawData TransmitData, int Length)
        {
            ++TransactionIdentifier;
            TransmitData.PutUInt16(0, TransactionIdentifier);
            TransmitData.PutUInt16(2, 0);
            TransmitData.PutUInt16(4, (UInt16)Length);
        }

        protected void CheckTransactionIdentifier(MbRawData ReceivMessage)
        {
            ushort RxIdentifier = ReceivMessage.GetUInt16(0);
            if (RxIdentifier != TransactionIdentifier) {
                throw new ModbusException(csModbusLib.ErrorCodes.WRONG_IDENTIFIER);
            }
        }
    }
}
