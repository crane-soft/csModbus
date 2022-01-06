using System;
using System.Threading;

namespace csModbusLib
{

    public class MbSlave : MbBase
    {

        protected MbSlaveDataServer gDataServer;
        private MBSFrame Frame = new MBSFrame();

        #region Constructors
        public MbSlave ()
        {
        }

        public MbSlave (MbInterface Interface)
        {
            gInterface = Interface;
            gDataServer = null;
        }

        public MbSlave (MbInterface Interface, MbSlaveDataServer DataServer)
        {
            gInterface = Interface;
            gDataServer = DataServer;
        }
        #endregion

        public MbSlaveDataServer DataServer
        {
            get { return gDataServer; }
            set { gDataServer = value; }
        }


        public void HandleRequestMessages ()
        {
            running = true;
            gInterface.Connect();
            while (running) {
                try {
                    if (ReceiveMasterRequestMessage()) {
                        DataServices();
                        SendResponseMessage();
                        Thread.Sleep(1);
                    }
                } catch (ModbusException ex) {
                    Console.WriteLine("ModbusException  {0}", ex.ErrorCode);
                    gInterface.ReConnect();
                }
                //Console.WriteLine("");
            }
        }

        protected bool ReceiveMasterRequestMessage ()
        {
            while (running) {
                if (gInterface.ReceiveHeader(Frame.RawData)) {
                    Frame.ReceiveMasterRequest(gInterface);
                    return true;
                }
                Thread.Sleep(1);
            }
            return false;
        }

        private void SendResponseMessage ()
        {
            int MsgLen = Frame.ToMasterResponseMessageLength();
            gInterface.SendFrame(Frame.RawData, MsgLen);
        }

        private void DataServices ()
        {
            MbSlaveDataServer DataServer = gDataServer;
            while (DataServer != null) {
                DataServer.DataServices(Frame);
                DataServer = DataServer.NextDataServer;
            }
        }
    }

    public class MbSlaveServer : MbSlave
    {
        private Thread ListenThread;
        public MbSlaveServer () {}
        public MbSlaveServer (MbInterface Interface) : base(Interface) { }
        public MbSlaveServer (MbInterface Interface, MbSlaveDataServer DataServer) : base(Interface, DataServer) { }

        public void StartListen ()
        {
            if (gInterface != null) {
                if (running) {
                    StopListen();
                    Thread.Sleep(100);
                }

                if (ListenThread == null) {
                    ListenThread = new Thread(this.HandleRequestMessages);
                    ListenThread.Start();
                }
            }
        }

        public void StartListen (MbSlaveDataServer DataServer)
        {
            gDataServer = DataServer;
            StartListen();
        }

        public void StartListen (MbInterface Interface, MbSlaveDataServer DataServer)
        {
            gInterface = Interface;
            gDataServer = DataServer;
            StartListen();
        }

        public void StopListen ()
        {
            running = false;
            if (gInterface != null) {
                gInterface.DisConnect();
            }
            if (ListenThread != null) {
                ListenThread.Join();
                ListenThread = null;
            }
        }
    }
}
