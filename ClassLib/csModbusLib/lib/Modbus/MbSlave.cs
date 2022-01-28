using System;
using System.Diagnostics;
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
            gInterface = null;
            gDataServer = null;
		}

        public MbSlave (MbInterface Interface)
        {
            InitInterface(Interface);
            gDataServer = null;
        }

        public MbSlave (MbInterface Interface, MbSlaveDataServer DataServer)
        {
            InitInterface(Interface);
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
            Debug.Print("Listener started");
            while (running) {
                try {
                    ReceiveMasterRequestMessage();
                    DataServices();
                    SendResponseMessage();

                } catch (ModbusException ex) {
                    if (running) {
                        Debug.Print("ModbusException  {0}", ex.ErrorCode);
                        gInterface.ReConnect();
                    }
                }
            }
            Debug.Print("Listener stopped");
        }

        protected void ReceiveMasterRequestMessage()
        {
            gInterface.ReceiveHeader(DeviceType.SLAVE, Frame.RawData);
            Frame.ReceiveMasterRequest(gInterface);
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
                }

                ListenThread = new Thread(this.HandleRequestMessages);
                ListenThread.Start();
            }
        }

        public void StartListen (MbSlaveDataServer DataServer)
        {
            gDataServer = DataServer;
            StartListen();
        }

        public void StartListen (MbInterface Interface, MbSlaveDataServer DataServer)
        {
            InitInterface(Interface);
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
                while (ListenThread.IsAlive) {
                    Thread.Yield();
                    Thread.Sleep(1);
                }

                ListenThread = null;
            }
        }
    }
}
