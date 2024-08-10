using System;
using System.Diagnostics;
using System.Threading;

namespace csModbusLib
{
    public class MbSlave : MbBase
    {
        protected MbSlaveDataServer gDataServer = null;
        protected MBSFrame Frame = new MBSFrame();

        #region Constructors
        public MbSlave() { }

        public MbSlave(MbInterface Interface)
        {
            InitInterface(Interface);
        }

        public MbSlave(MbInterface Interface, MbSlaveDataServer DataServer)
        {
            InitInterface(Interface);
            gDataServer = DataServer;
        }
        #endregion

        public bool StartListen()
        {
            if (gInterface != null) {
                if (running) {
                    StopListen();
                }

                if (gInterface.Connect(Frame.RawData)) {
                    StartListener();
                    return true;
                }
            }
            return false;
        }

        public bool StartListen(MbSlaveDataServer DataServer)
        {
            gDataServer = DataServer;
            return StartListen();
        }

        public bool StartListen(MbInterface Interface, MbSlaveDataServer DataServer)
        {
            InitInterface(Interface);
            gDataServer = DataServer;
            return StartListen();
        }

        public void StopListen()
        {
            running = false;
            if (gInterface != null) {
                gInterface.DisConnect();
            }
            StopListener();

        }

        virtual protected void StartListener() { }
        virtual protected void StopListener() { }

        public MbSlaveDataServer DataServer
        {
            get { return gDataServer; }
            set { gDataServer = value; }
        }

        public void HandleRequestMessages()
        {
            running = true;
            Debug.Print("Listener started");
            while (running) {
                try {
                    ReceiveMasterRequestMessage();
                    DataServices();
                    SendResponseMessage();

                }
                catch (ModbusException ex) {
                    if (running) {
                        Debug.Print("ModbusException  {0}", ex.ErrorCode);
                        gInterface.DisConnect();
                        // TODO raise disconnect event 
                        break;
                    }
                }
            }
            Debug.Print("Listener stopped");
        }

        protected void ReceiveMasterRequestMessage()
        {
            gInterface.ReceiveHeader(MbInterface.InfiniteTimeout);
            Frame.ReceiveMasterRequest(gInterface);
        }

        protected void SendResponseMessage()
        {
            int MsgLen = Frame.ToMasterResponseMessageLength();
            gInterface.SendFrame(MsgLen);
        }

        protected void DataServices()
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
        private Thread ListenThread = null;

        public MbSlaveServer() { }
        public MbSlaveServer(MbInterface Interface) : base(Interface) { }
        public MbSlaveServer(MbInterface Interface, MbSlaveDataServer DataServer) : base(Interface, DataServer) { }

        override protected void StartListener()
        {
            ListenThread = new Thread(this.HandleRequestMessages);
            ListenThread.Start();
        }
        override protected void StopListener()
        {
            if (ListenThread != null) {
                while (ListenThread.IsAlive) {
                    Thread.Yield();
                    Thread.Sleep(1);
                }

                ListenThread = null;
            }
        }
    }
}