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

    public abstract class MbETHMaster : MbEthernet {
        protected string remote_host;
        protected NetworkStream nwStream;

        protected abstract Boolean RecvDataAvailable();

        public MbETHMaster(string host_name, int port)
        {
            remote_host = host_name;
            SetPort(port);
        }

        public override bool ReceiveHeader(MbRawData MbData)
        {
            if (RecvDataAvailable()) {
                MbData.EndIdx = 0;
                ReceiveBytes(MbData, 8);
                CheckTransactionIdentifier(MbData);
                return true;
            }
            return false;
        }

    }

    public abstract class MbETHSlave : MbEthernet {

        // It is allowed, that more tan on master connetcs to on slave
        protected abstract class EthContext {
            public MbRawData Frame_Buffer;
            public abstract void SendFrame(byte[] data, int Length);
        }
        private EthContext CurrentRxContext;  // reference to the just received frame
        private SemaphoreSlim smReqest;
        private bool RequestAvail;
     
        public MbETHSlave (int port)
        {
            SetPort(port);
            smReqest = new SemaphoreSlim(1,1);
            RequestAvail = false;
        }

        public override bool ReceiveHeader(MbRawData MbData)
        {
            if (RequestAvail) {
                MbData.CopyFrom(CurrentRxContext.Frame_Buffer);
                return true;
            }
            return false;
            
        }
  
        protected void RequestReceived (EthContext newContext)
        {
            smReqest.Wait();
            CurrentRxContext = newContext;
            RequestAvail = true;
        }

        public override void SendFrame(MbRawData TransmitData, int Length)
        {
            TransmitData.PutUInt16(4, (UInt16)Length);
            CurrentRxContext.SendFrame(TransmitData.Data, Length + MBAP_Header_Size);
            FreeMessage();
        }

        private void FreeMessage ()
        {
            RequestAvail = false;
            smReqest.Release();
        }
    }
}
