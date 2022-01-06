using System.Diagnostics;

// http://www.simplymodbus.ca/index.html

namespace csModbusLib
{
    public abstract class MbInterface
    {
        const int DEFAULT_RX_TIMEOUT = 600;
        protected int rx_timeout;
        protected Stopwatch timeoutTmer;
        protected bool IsConnected;

        public MbInterface()
        {
            rx_timeout = DEFAULT_RX_TIMEOUT;
            timeoutTmer = new Stopwatch();
        }


        public abstract bool Connect ();
        public abstract void DisConnect();
        public abstract bool ReceiveHeader(MbRawData MbData);
        public abstract void SendFrame(MbRawData TransmitData, int Length);

        public virtual void ReceiveBytes(MbRawData RxData, int count) 
        {   // TODO bei UDP -Slave nix zu tun?? vielleicht wenigstens die Anzahl Daten prüfen
        }
        public virtual void EndOfFrame(MbRawData RxData) { }

        public bool ReConnect()
        {
            DisConnect();
            return Connect();
            // TODO error checking
        }

    }
}
