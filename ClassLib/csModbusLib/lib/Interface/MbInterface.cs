using System.Diagnostics;

// http://www.simplymodbus.ca/index.html

namespace csModbusLib
{
    public abstract class MbInterface
    {
        public const int InfiniteTimeout = -1;
        public const int ResponseTimeout = 200;

        protected bool IsConnected = false;

        public MbInterface() { }

        public abstract bool Connect ();
        public abstract void DisConnect();
        public abstract void ReceiveHeader(int timeOut, MbRawData MbData);

        public abstract void SendFrame(MbRawData TransmitData, int Length);

        public virtual void ReceiveBytes(MbRawData RxData, int count)  { }
        public virtual void EndOfFrame(MbRawData RxData) { }

        public bool ReConnect()
        {
            DisConnect();
            return Connect();
        }
    }
}
