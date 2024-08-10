using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace csModbusLib {
    public abstract class MbETHSlave : MbEthernet {
     
        public MbETHSlave (int port) {
            SetPort(port);
        }

        public override void SendFrame(int Length)
        {
            MbData.PutUInt16(4, (UInt16)Length);
            try {
                SendFrameData(Length + MBAP_Header_Size);
                FreeMessage();
            } catch (System.Exception ex) {
                Debug.Print(ex.Message);
                throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_ERROR);
            }
        }
        protected abstract void SendFrameData(int Length);
        protected virtual void FreeMessage() { }
    }

    public class MbUDPSlave : MbETHSlave
    {
        public MbUDPSlave(int port) : base(port) { }

        public override bool Connect(MbRawData Data)
        {
            base.Connect(Data);
            try {
                mUdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, remote_port));
                IsConnected = true;
                Debug.Print("Connected");

            } catch (System.Exception ex) {
                Debug.Print(ex.Message);
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect()
        {
            if (IsConnected) {
                IsConnected = false;
                if (mUdpClient != null) {
                    mUdpClient.Client.Shutdown(SocketShutdown.Both);
                    mUdpClient.Close();
                    Debug.WriteLine("Connection Closed");
                }
            }
        }

        public override void ReceiveHeader(int timeOut)
        {
            UdpReceiveHeaderData(timeOut);
        }

        protected override void SendFrameData(int Length)
        {
            mUdpClient.Send(MbData.Data, Length, udpRemoteAddr);
        }
    }

    public class MbTCPSlave : MbETHSlave
    {
        private TcpContext ConnectionContext;
        private TcpListener tcpl;
        private SemaphoreSlim smRxProcess;
        private SemaphoreSlim smRxDataAvail;

        public MbTCPSlave(int port) : base(port)
        {
        }

        public override bool Connect(MbRawData Data)
        {
            base.Connect(Data);
            try {
                smRxProcess = new SemaphoreSlim(1, 1);
                smRxDataAvail = new SemaphoreSlim(0, 1);

                tcpl = new TcpListener(new IPEndPoint(IPAddress.Any, remote_port));
                tcpl.Start();
                tcpl.BeginAcceptTcpClient(OnClientAccepted, tcpl);
                IsConnected = true;

            } catch (System.Exception) {
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect()
        {
            if (IsConnected) {
                IsConnected = false;
                tcpl.Stop();

                if (smRxProcess.CurrentCount == 0)
                    smRxProcess.Release();
                if (smRxDataAvail.CurrentCount == 0)
                    smRxDataAvail.Release();
            }
        }

        public override void ReceiveHeader(int timeOut)
        {
            smRxDataAvail.Wait();
            if (IsConnected == false)
                throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_CLOSED);
            ConnectionContext.CopyRxData(MbData);
        }

        protected override void SendFrameData(int Length)
        {
            ConnectionContext.SendFrame(MbData.Data, Length);
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            TcpListener listener = ar.AsyncState as TcpListener;
            if ((listener == null) || (IsConnected == false)) {
                // the listener was closed
                return;
            }

            try {
                TcpClient Client = tcpl.EndAcceptTcpClient(ar);
                TcpContext context = new TcpContext(Client);
                Debug.WriteLine(String.Format("Client accepted {0}",Client.Client.RemoteEndPoint.ToString()));

                context.BeginNewFrame();
                context.BeginReadFrameData(OnReadFrame);
            } catch (System.Exception ex) {
                Debug.WriteLine(ex.Message);
            }
            tcpl.BeginAcceptTcpClient(OnClientAccepted, tcpl);
        }

        private void OnReadFrame(IAsyncResult ar)
        {
            TcpContext context = ar.AsyncState as TcpContext;
            if ((context == null) || (IsConnected == false)) {
                // maybe the listener was closed
                return;
            }

            if (context.EndReceive(ar)) {
                RequestReceived(context);
                context.BeginNewFrame();

            }

            context.BeginReadFrameData(OnReadFrame);
        }

        private void RequestReceived(TcpContext newContext)
        {
            smRxProcess.Wait();
            ConnectionContext = newContext;
            smRxDataAvail.Release();
        }

        protected override void FreeMessage()
        {
            smRxProcess.Release();
        }

        private class TcpContext
        {
            private MbRawData FrameBuffer;
            private bool closed;
            private bool NewFrame = false;
            private TcpClient Client;
            private NetworkStream Stream;
            private int Bytes2Read;
            private int ReadIndex;

            public TcpContext(TcpClient aClient)
            {
                FrameBuffer = new MbRawData(MbBase.MAX_FRAME_LEN);
                Client = aClient;
                Stream = Client.GetStream();
                closed = false;
            }

            public void SendFrame(byte[] data, int Length)
            {
                Stream.Write(data, 0, Length);
            }

            public void BeginNewFrame()
            {
                Bytes2Read = MBAP_Header_Size;
                ReadIndex = 0;
                NewFrame = true;
            }

            public void BeginReadFrameData(AsyncCallback callback)
            {
                if (closed == false)
                    Stream.BeginRead(FrameBuffer.Data, ReadIndex, Bytes2Read, callback, this);
            }

            public bool EndReceive(IAsyncResult ar)
            {
                int readed = Stream.EndRead(ar);
                if ((readed == 0) || (Client.Connected == false)) {
                    // Connection closed
                    Debug.WriteLine(String.Format("Client disconnected {0}", Client.Client.RemoteEndPoint.ToString()));

                    Client.Close();
                    Stream = null;
                    closed = true;
                    return false;
                }

                ReadIndex += readed;
                Bytes2Read -= readed;
                if (Bytes2Read > 0) { 
                    return false;
                }

                FrameBuffer.EndIdx = ReadIndex;
                if (NewFrame) {
                    Bytes2Read = FrameBuffer.CheckEthFrameLength();
                    NewFrame = false;
                    return false;
                }            

                return true;
            }

            public void CopyRxData(MbRawData RxData)
            {
                RxData.CopyFrom(FrameBuffer);
            }

        }
    }
}
