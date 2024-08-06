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

        public override void ReceiveHeader(int timeOut)
        {
            MbData.EndIdx = 0;
            ReceiveHeaderData(timeOut, MbData);
            CheckTransactionIdentifier(MbData);
        }
    }

    public class MbUDPMaster : MbETHMaster
    {
        public MbUDPMaster(string host_name, int port)  : base(host_name, port)  { }

        public override bool Connect(MbRawData Data)
        {
            base.Connect(Data);
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

        public override void SendFrame(int Length)
        {
            FillMBAPHeader(MbData, Length);
            mUdpClient.Send(MbData.Data, Length + MBAP_Header_Size);
        }

        protected override void ReceiveHeaderData(int timeOut, MbRawData RxData)
        {
            UdpReceiveHeaderData(timeOut, RxData);
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

        public override Boolean Connect(MbRawData Data)
        {
            base.Connect(Data);
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

        public override void SendFrame( int Length)
        {
            FillMBAPHeader(MbData, Length);
            nwStream.Write(MbData.Data, 0, Length + MBAP_Header_Size);
        }

        protected override void ReceiveHeaderData(int timeOut, MbRawData RxData)
        {
            ReadData(ResponseTimeout, RxData, 8);
            int bytes2read = RxData.CheckEthFrameLength();
            if (bytes2read > 0) {
                ReadData(50, RxData, bytes2read);
            }
        }

        protected void ReadData(int timeOut, MbRawData RxData, int length)
        {
            try {
                nwStream.ReadTimeout = timeOut;
                int readed = nwStream.Read(RxData.Data, RxData.EndIdx, length);
                RxData.EndIdx += readed;
            } catch (IOException ) {
                throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
            }
        }
    }
}
