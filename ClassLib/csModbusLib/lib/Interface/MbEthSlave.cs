using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace csModbusLib {
    public abstract class MbETHSlave : MbEthernet {

        // MbETHSlave should be multi master capable
        protected abstract class EthContext {
            public MbRawData Frame_Buffer;
            public abstract void SendFrame(byte[] data, int Length);
        }
        protected EthContext CurrentRxContext;  // reference to the just received frame
     
        public MbETHSlave (int port) {
            SetPort(port);
        }

        public override void SendFrame(MbRawData TransmitData, int Length)
        {
            TransmitData.PutUInt16(4, (UInt16)Length);
            try {
                CurrentRxContext.SendFrame(TransmitData.Data, Length + MBAP_Header_Size);
                FreeMessage();
            } catch (System.Exception ex) {
                Debug.Print(ex.Message);
                throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_ERROR);
            }
        }

        protected virtual void FreeMessage() { }
    }

    public class MbUDPSlave : MbETHSlave
    {
        private class UdpContext : EthContext
        {
            public UdpContext(UdpClient client)
            {
                udpc = client;
                Frame_Buffer = new MbRawData();
            }
            public IPEndPoint EndPoint;
            public UdpClient udpc;

            public override void SendFrame(byte[] data, int Length)
            {
                udpc.Send(data, Length, EndPoint);
            }

            public void Receive()
            {
                try {
                    Frame_Buffer.Data = udpc.Receive(ref EndPoint);
                    Frame_Buffer.EndIdx = Frame_Buffer.Data.Length;
                } catch (System.Exception ex) {
                    Debug.Print(ex.Message);
                    throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_ERROR);
                }
            }
        }

        private UdpContext mUdpContext;

        public MbUDPSlave(int port)
            : base(port)
        {
        }

        public override bool Connect()
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

        public override bool ReceiveHeader(DeviceType dtype, MbRawData MbData)
        {
            mUdpContext.Receive();
            CurrentRxContext = mUdpContext;
            MbData.CopyFrom(mUdpContext.Frame_Buffer);
            return true;
        }
    }

    public class MbTCPSlave : MbETHSlave
    {
        private class TcpContext : EthContext
        {
            public TcpContext (TcpClient aClient)
            {
                Frame_Buffer = new MbRawData(MbBase.MAX_FRAME_LEN);
                Client = aClient;
                Stream = Client.GetStream();
            }
            public TcpClient Client;
            public NetworkStream Stream;

            public override void SendFrame (byte[] data, int Length)
            {
                Stream.Write(data, 0, Length);
            }

            public bool EndReceive (IAsyncResult ar)
            {
                if (Client.Connected == false) {
                    return false;
                }
                int read = Stream.EndRead(ar);
                if (read == 0) {
                    return false;
                }

                int Frame_Length = Frame_Buffer.GetUInt16(4);
                int bytes2read = Frame_Length;
                if (bytes2read > Frame_Buffer.Data.Length)
                    bytes2read = Frame_Buffer.Data.Length;

                // OnClientRead was started in a separate thread, so I can finish reading here
                while (bytes2read > 0) {
                    int readed = Stream.Read(Frame_Buffer.Data, MBAP_Header_Size, bytes2read);
                    if (readed == 0) {
                        return false;
                    }

                    bytes2read -= readed;
                }

                Frame_Buffer.EndIdx = MBAP_Header_Size + Frame_Length;
                return true;
            }
        }

        private TcpListener tcpl; 
        private SemaphoreSlim smRxProcess;
        private SemaphoreSlim smRxDataAvail;

        public MbTCPSlave (int port) : base(port) {
            smRxProcess = new SemaphoreSlim(1, 1);
            smRxDataAvail = new SemaphoreSlim(0, 1);
        }

        public override bool Connect ()
        {
            try {
                tcpl = new TcpListener(new IPEndPoint(IPAddress.Any, remote_port));
                tcpl.Start();
                tcpl.BeginAcceptTcpClient(OnClientAccepted, tcpl);
                IsConnected = true;

            } catch (System.Exception) {
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect ()
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

        public override bool ReceiveHeader(DeviceType dtype, MbRawData MbData)
        {
            smRxDataAvail.Wait();
            if (IsConnected == false)
                throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_CLOSED);
            MbData.CopyFrom(CurrentRxContext.Frame_Buffer);
            return true;
        }

        private void OnClientAccepted (IAsyncResult ar)
        {
            if (IsConnected == false)
                return;

            TcpListener listener = ar.AsyncState as TcpListener;
            if (listener == null)
                return;

            try {
                TcpContext context = new TcpContext(tcpl.EndAcceptTcpClient(ar));
                context.Stream.BeginRead(context.Frame_Buffer.Data, 0, MBAP_Header_Size, OnClientRead, context);
            } catch (System.Exception) {
                return;

            }
            tcpl.BeginAcceptTcpClient(OnClientAccepted, tcpl);
        }

        private void OnClientRead (IAsyncResult ar)
        {
            if (IsConnected == false)
                return;

            TcpContext context = ar.AsyncState as TcpContext;
            if (context == null)
                return;

            try {
                if (context.EndReceive(ar)) {
                    RequestReceived(context);
                } else {
                    goto disconnected;

                }
            } catch (System.Exception) {
                goto disconnected;
            }

            context.Stream.BeginRead(context.Frame_Buffer.Data, 0, MBAP_Header_Size, OnClientRead, context);
            return;

        disconnected:
            context.Client.Close();
            context.Stream.Dispose();
            context = null;
        }
        protected void RequestReceived(EthContext newContext)
        {
            smRxProcess.Wait();
            CurrentRxContext = newContext;
            smRxDataAvail.Release();
        }

        protected override void FreeMessage()
        {
            smRxProcess.Release();
        }
    }
}
