using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

// Wir machen's asychron auch mit dem Ziel mehrere Clients gleichzeitig zu bedienen
// https://stackoverflow.com/questions/6023264/high-performance-tcp-server-in-c-sharp
//
namespace csModbusLib
{
    public class MbTCPMaster : MbETHMaster
    {
        private TcpClient tcpc;   // TCP-Master (Client)

        public MbTCPMaster(string host_name , int port)
            : base (host_name,port)
        {
            tcpc = new TcpClient ();
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
            }
            catch (System.Exception) {
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

        protected override Boolean RecvDataAvailable()
        {
            return nwStream.DataAvailable;
        }

        public override void ReceiveBytes(MbRawData RxData, int count)
        {
            nwStream.ReadTimeout = 100;
            try {
                nwStream.Read(RxData.Data, RxData.EndIdx, count);
                RxData.EndIdx += count;
            }
            catch (IOException) {
                throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
            }
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

                // OnClientRead wurde in einem separatem Thread gestartet, 
                // daher darf ich hier in Ruhe zu Ende lesen
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

        private TcpListener tcpl; // TCP-Slave (Server)

        public MbTCPSlave (int port) : base(port) { }

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
            }
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
    }
}
