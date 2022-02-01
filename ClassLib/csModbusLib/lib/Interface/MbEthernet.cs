using System;
using System.Net;
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
        protected UdpClient  mUdpClient;
        protected IPEndPoint udpRemoteAddr;

        private UInt16 TransactionIdentifier;

        public MbEthernet()
        {
            TransactionIdentifier = 0;
            mUdpClient = null;
        }

        protected void SetPort(int port)
        {
            remote_port = port;
        }

        protected virtual void ReceiveHeaderData(int timeOut, MbRawData MbData) { }

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

        protected void UdpReceiveHeaderData(int timeOut, MbRawData RxData)
        {
            RxData.EndIdx = 0;
            mUdpClient.Client.ReceiveTimeout = timeOut;
            try {
                byte[] rxbuff = mUdpClient.Receive(ref udpRemoteAddr);
                RxData.CopyFrom(rxbuff, 0, rxbuff.Length);

                if (RxData.CheckEthFrameLength() > 0) {
                    // we assume all framedata in one datagram
                    throw new ModbusException(csModbusLib.ErrorCodes.MESSAGE_INCOMPLETE);
                }

            } catch (SocketException ex) {
                if (ex.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut) {
                    throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
                } else {
                    throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_ERROR);
                }
            }
        }

    }
}
