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

        public override bool ReceiveHeader(MbRawData MbData)
        {
            MbData.EndIdx = 0;
            ReceiveBytes(MbData, 8);
            CheckTransactionIdentifier(MbData);
            return true;
        }
    }

    public class MbUDPMaster : MbETHMaster
    {
        private int BytesReaded;
        public MbUDPMaster(string host_name, int port)
            : base(host_name, port)
        {
            mUdpClient = new UdpClient(host_name, port);
        }

        public override Boolean Connect()
        {
            try {
                if (mUdpClient == null) {
                    mUdpClient = new UdpClient(remote_host, remote_port);
                    mUdpClient.Connect(remote_host, remote_port);
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
            BytesReaded = 0;
        }

        public override void ReceiveBytes(MbRawData RxData, int count)
        {
            int BytestoRead = (BytesReaded + count) - RxData.EndIdx;
            if (BytestoRead > 0) {
                System.Net.IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
                mUdpClient.Client.ReceiveTimeout = 200;

                try {
                    byte[] rxbuff = mUdpClient.Receive(ref ipe);
                    RxData.CopyFrom(rxbuff, 0, rxbuff.Length);
                    BytesReaded += count;
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

        public override void ReceiveBytes(MbRawData RxData, int count)
        {
            nwStream.ReadTimeout = 100;
            try {
                nwStream.Read(RxData.Data, RxData.EndIdx, count);
                RxData.EndIdx += count;
            } catch (IOException) {
                throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
            }
        }
    }
}
