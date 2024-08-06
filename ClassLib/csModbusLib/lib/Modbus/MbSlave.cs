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

        public bool StartListen()
        {
            if (gInterface != null) {
                if (running) {
                    StopListen();
                }

                if (gInterface.Connect(Frame.RawData)) {
                    ListenThread = new Thread(this.HandleRequestMessages);
                    ListenThread.Start();
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

            if (ListenThread != null) {
                while (ListenThread.IsAlive) {
                    Thread.Yield();
                    Thread.Sleep(1);
                }

                ListenThread = null;
            }
        }
    }

    public class MbSlaveStateMachine : MbSlave
    {
        public enum enRxStates
        {   // TODO Status -> MBSlave.cs
            Idle,
            StartOfFrame,
            ReceiveHeader,
            RcvMessage,
            RcvAdditionalData
        }
        private enRxStates RxState;

        private MbSerial SerialInterface;
        private int DataLen;
          public MbSlaveStateMachine() { }
        public MbSlaveStateMachine(MbSerial Interface) : base(Interface) { }
        public MbSlaveStateMachine(MbSerial Interface, MbSlaveDataServer DataServer) : base(Interface, DataServer) { }
        public bool StartListen()
        {
            if (SerialInterface != null) {
                SerialInterface = (MbSerial)gInterface;
                SerialInterface.DataReceivedEvent += SerialInterface_DataReceivedEvent;
                if (running) {
                    StopListen();
                }

                if (SerialInterface.Connect(Frame.RawData)) {
                    WaitFrameStart();
                }
            }
            return false;
        }

        private void WaitFrameStart()
        {
            Frame.RawData.IniADUoffs();
            SerialInterface.Wait4Event(enRxStates.StartOfFrame);
        }
        private void SerialInterface_DataReceivedEvent(MbSerial.enRxStates RxState)
        {
            // TODO timeout 
            switch (RxState) {
                case enRxStates.StartOfFrame:
                    SerialInterface.Wait4Event(enRxStates.ReceiveHeader,2);
                    break;
                case enRxStates.ReceiveHeader:
                    DataLen = Frame.ParseMasterRequest();
                    SerialInterface.Wait4Event(enRxStates.RcvMessage, DataLen);
                    break;
                case enRxStates.RcvMessage:
                    DataLen = Frame.ParseDataCount();
                    if (DataLen != 0) {
                        SerialInterface.Wait4Event(enRxStates.RcvAdditionalData, DataLen);
                    } else {
                        MasterRequestReceived();
                    }
                    break;
                case enRxStates.RcvAdditionalData:
                    MasterRequestReceived();
                    break;
            }
        }

        private void MasterRequestReceived()
        {
            SerialInterface.EndOfFrame();
            DataServices();
            SendResponseMessage();

        }
        public void StopListen()
        {
            running = false;
            if (gInterface != null) {
                gInterface.DisConnect();
            }
        }
    }
}
