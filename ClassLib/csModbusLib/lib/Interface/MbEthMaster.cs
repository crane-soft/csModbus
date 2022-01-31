using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace csModbusLib
{
    public abstract class MbETHMaster : MbEthernet
    {
        protected string remote_host;

        public MbETHMaster(string host_name, int port)
        {
            remote_host = host_name;
            SetPort(port);
        }

        public override void ReceiveHeader(int timeOut, MbRawData MbData)
        {
            MbData.EndIdx = 0;
            Readbytes(200,MbData, 8);
            int frameLength = MbData.GetUInt16(4);
            int bytes2read = (frameLength + 6) - MbData.EndIdx;
            if (bytes2read > 0) {
                Readbytes(50, MbData, bytes2read);
            }
            CheckTransactionIdentifier(MbData);
        }

        protected abstract void Readbytes(int timeOut, MbRawData MbData, int length);
    }

    public class MbUDPMaster : MbETHMaster
    {
        public MbUDPMaster(string host_name, int port)
            : base(host_name, port)
        {
            mUdpClient = null;
        }

        public override bool Connect()
        {
            try {
                if (mUdpClient == null) {
                    mUdpClient = new UdpClient(remote_host, remote_port);
                }
                IsConnected = true;

            } catch (System.Exception) {
                IsConnected = false;
            }

            return IsConnected;
        }

        public override void DisConnect()
        {
            if (mUdpClient != null)
                mUdpClient.Close();
            mUdpClient = null;
            IsConnected = false;
        }

        public override void SendFrame(MbRawData TransmitData, int Length)
        {
            FillMBAPHeader(TransmitData, Length);
            mUdpClient.Send(TransmitData.Data, Length + MBAP_Header_Size);
        }

        protected override void Readbytes(int timeOut, MbRawData RxData, int length)
        {
            System.Net.IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
            mUdpClient.Client.ReceiveTimeout = timeOut;
            try {
                byte[] rxbuff = mUdpClient.Receive(ref ipe);
                RxData.CopyFrom(rxbuff, 0, rxbuff.Length);
            } catch (SocketException ex) {
                if (ex.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut) {
                    throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
                } else {
                    throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_ERROR);
                }
            }
        }
    }

    public class MbTCPMaster : MbETHMaster
    {
        private TcpClient tcpc;   
        private NetworkStream nwStream;

        public MbTCPMaster(string host_name, int port)
            : base(host_name, port)
        {
            tcpc = new TcpClient();
        }

        public override Boolean Connect()
        {
            if (tcpc == null)
                tcpc = new TcpClient();
            try {
                tcpc.Connect(remote_host, remote_port);
                if (tcpc.Connected) {
                    nwStream = tcpc.GetStream();
                    IsConnected = true;
                }
            } catch (System.Exception) {
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect()
        {
            if (nwStream != null)
                nwStream.Close();
            if (tcpc != null)
                tcpc.Close();
            tcpc = null;
            IsConnected = false;
        }

        public override void SendFrame(MbRawData TransmitData, int Length)
        {
            FillMBAPHeader(TransmitData, Length);
            nwStream.Write(TransmitData.Data, 0, Length + MBAP_Header_Size);
        }

        protected override void Readbytes(int timeOut, MbRawData RxData, int length)
        {
            nwStream.ReadTimeout = timeOut;
            try {
                int readed = nwStream.Read(RxData.Data, RxData.EndIdx, length);
                RxData.EndIdx += readed;
            } catch (IOException) {
                throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
            }
        }
    }
}
