using System.Diagnostics;

// http://www.simplymodbus.ca/index.html

namespace csModbusLib
{
    public abstract class MbInterface
    {
        public const int InfiniteTimeout = -1;
        public const int ResponseTimeout = 200;

        protected bool IsConnected = false;
        protected MbRawData MbData;
        public MbInterface() { }

        

        public virtual bool Connect (MbRawData Data)
        {
            this.MbData = Data;
            IsConnected = false;
            return IsConnected;
        }
        public abstract void DisConnect();
        public abstract void ReceiveHeader(int timeOut);

        public abstract void SendFrame(int Length);

        public virtual void ReceiveBytes(int count)  { }
        public virtual void EndOfFrame() { }
   
    }
}
