using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace csModbusLib
{
    public class MbUDPMaster : MbETHMaster
    {
        private int BytesReaded;
        public MbUDPMaster (string host_name, int port)
            : base(host_name, port)
        {
            mUdpClient = new UdpClient(host_name, port);
        }

        public override Boolean Connect ()
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

        public override void DisConnect ()
        {
            if (mUdpClient != null)
                mUdpClient.Close();
            mUdpClient = null;
            IsConnected = false;
        }

        public override void SendFrame (MbRawData TransmitData, int Length)
        {
            FillMBAPHeader(TransmitData, Length);
            mUdpClient.Send(TransmitData.Data, Length + MBAP_Header_Size);
            BytesReaded = 0;
        }

        protected override Boolean RecvDataAvailable ()
        {
            return mUdpClient.Available != 0;
        }

        public override void ReceiveBytes (MbRawData RxData, int count)
        {
            int BytestoRead = (BytesReaded + count) - RxData.EndIdx;
            if (BytestoRead > 0) {

                // TODO whats about timeout here
                // https://stackoverflow.com/questions/2281441/can-i-set-the-timeout-for-udpclient-in-c
                mUdpClient.Client.ReceiveTimeout = 200;
                timeoutTmer.Restart();
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);

                // TODO make it async?
                while (IsConnected) {
                    if (mUdpClient.Available >= BytestoRead) {
                        byte[] rxbuff = mUdpClient.Receive(ref ipe);
                        RxData.CopyFrom(rxbuff, 0, rxbuff.Length);
                        break;
                    }

                    if (timeoutTmer.ElapsedMilliseconds > 200)
                        throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
                }
            }
            BytesReaded += count;

        }
    }

    public class MbUDPSlave : MbETHSlave
    {
        private class UdpContext : EthContext
        {
            public UdpContext (UdpClient client)
            {
                udpc = client;
                Frame_Buffer = new MbRawData();
            }
            public IPEndPoint EndPoint;
            public UdpClient udpc;
            public override void SendFrame (byte[] data, int Length)
            {
                udpc.Send(data, Length, EndPoint);
            }

            public bool EndReceive (IAsyncResult ar)
            {
                Frame_Buffer.Data = udpc.EndReceive(ar, ref EndPoint);
                if (Frame_Buffer.Data != null) {
                    // TODO check length
                    Frame_Buffer.EndIdx = Frame_Buffer.Data.Length;
                    return true;
                }
                return false;
            }
        }

        private UdpContext[] mUdpContext = new UdpContext[2];
        private int CtxIndex;

        public MbUDPSlave (int port)
            : base(port)
        {
        }

        public override bool Connect ()
        {
            try {
                mUdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, remote_port));
                mUdpContext[0] = new UdpContext(mUdpClient);
                mUdpContext[1] = new UdpContext(mUdpClient);
                CtxIndex = 0;

                mUdpClient.BeginReceive(OnClientRead, null);
                IsConnected = true;
                Debug.Print("Connected");

            } catch (System.Exception ex) {
                Debug.Print(ex.Message);
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect ()
        {
            if (IsConnected) {
                IsConnected = false;
                if (mUdpClient != null) {
                    mUdpClient.Close();
                    Debug.WriteLine("Connection Closed");
                }
            }
        }

        private void OnClientRead (IAsyncResult ar)
        {
            if (IsConnected == false)
                return;

            UdpContext Context = mUdpContext[CtxIndex];
            try {
                if (Context.EndReceive(ar)) {
                    RequestReceived(Context);
                }
            } catch {
                // here if connection closed ??
                // TODO catch errors
            }

           /* if (mUdpClient.Client.Connected == false) {
                DisConnect();
                return;
            }*/
            CtxIndex = (CtxIndex + 1) & 1;
            mUdpClient.BeginReceive(OnClientRead, null);
        }
    }

    public class MbUDPSyncSlave : MbETHSlave
    {
        private class UdpContext : EthContext
        {
            public UdpContext (UdpClient client)
            {
                udpc = client;
                Frame_Buffer = new MbRawData();
            }
            public IPEndPoint EndPoint;
            public UdpClient udpc;

            public override void SendFrame (byte[] data, int Length)
            {
                udpc.Send(data, Length, EndPoint);
            }

            public bool Receive ()
            {
                if (udpc.Available > 0) {

                    Frame_Buffer.Data = udpc.Receive(ref EndPoint);
                    if (Frame_Buffer.Data != null) {
                        // TODO check length
                        Frame_Buffer.EndIdx = Frame_Buffer.Data.Length;
                        return true;
                    }
                }
                return false;
            }
        }

        private UdpContext mUdpContext;

        public MbUDPSyncSlave (int port)
            : base(port)
        {
        }

        public override bool Connect ()
        {
            try {
                mUdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, remote_port));
                mUdpContext = new UdpContext(mUdpClient);
                IsConnected = true;
                Debug.Print("Connected");

            } catch (System.Exception ex) {
                Debug.Print(ex.Message);
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect ()
        {
            if (IsConnected) {
                IsConnected = false;
                if (mUdpClient != null) {
                    mUdpClient.Close();
                    Debug.WriteLine("Connection Closed");
                }
            }
        }
 

        public override bool ReceiveHeader (MbRawData MbData)
        {
            if (mUdpContext.Receive()) {
                RequestReceived(mUdpContext);
                base.ReceiveHeader(MbData);
                return true;
            }
            return false;
        }
    }
}
