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
            StopListener();
            if (gInterface != null) {
                gInterface.DisConnect();
            }

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

    public class MbSlaveStateMachine : MbSlave
    {
        public enum enRxStates
        {  
            Idle,
            StartOfFrame,
            ReceiveHeader,
            RcvMessage,
            RcvAdditionalData,
            RcvEndOfFrame
        }
        private enRxStates RxState;

        private MbSerial SerialInterface;
        private System.Timers.Timer TimeoutTimer;

        public MbSlaveStateMachine() { }
        public MbSlaveStateMachine(MbSerial Interface) : base(Interface) { }
        public MbSlaveStateMachine(MbSerial Interface, MbSlaveDataServer DataServer) : base(Interface, DataServer) { }
        override protected void StartListener()
        {
            SerialInterface = (MbSerial)gInterface;
            SerialInterface.DataReceivedEvent += SerialInterface_DataReceivedEvent;
            InitTimeoutTimer();
            WaitFrameStart();
        }

        override protected void StopListener()
        {
            SerialInterface.DataReceivedEvent -= SerialInterface_DataReceivedEvent;
            if (TimeoutTimer != null)
                TimeoutTimer.Stop();
        }

        private void InitTimeoutTimer()
        {
            TimeoutTimer = new System.Timers.Timer();
            TimeoutTimer.AutoReset = false;
            TimeoutTimer.Elapsed += TimeoutTimer_Elapsed;
        }

        private void WaitFrameStart()
        {
            Debug.Print("WaitFrameStart");
            RxState = enRxStates.StartOfFrame;
            SerialInterface.Wait4FrameStartEvent();
        }

        private void WaitFrameEnd()
        {
            RxState = enRxStates.RcvEndOfFrame;
            int timeOut = SerialInterface.Wait4FrameEndEvent();
            StartTimeOut(timeOut);
        }

        private void WaitFrameData(enRxStates NextState, int DataLen)
        {
            RxState = NextState;
            Debug.Print("WaitFrameData NextState = " + NextState);
            int timeOut = SerialInterface.Wait4Bytes(DataLen);
             StartTimeOut(timeOut);
        }

        private void StartTimeOut(int timeOut)
        {
            if (timeOut > 0) {
                TimeoutTimer.Enabled = false;
                TimeoutTimer.Interval = timeOut;
                TimeoutTimer.Start();
            }
        }
        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.Print("MbSlaveStateMachine Rx-Timeout");
            //WaitFrameStart();
        }

        private void SerialInterface_DataReceivedEvent(ModbusException ex)
        {
            int DataLen;
            TimeoutTimer.Stop();
            if (ex != null) {
                WaitFrameStart();
                return;
            }

            switch (RxState) {
                case enRxStates.StartOfFrame:
                    Debug.Print("StartOfFrame");
                    WaitFrameData (enRxStates.ReceiveHeader,2);
                    break;
                case enRxStates.ReceiveHeader:
                    DataLen = Frame.ParseMasterRequest();
                    Debug.Print("Header Received, DateLen: " + DataLen.ToString());
                    WaitFrameData(enRxStates.RcvMessage, DataLen);
                    break;
                case enRxStates.RcvMessage:
                    DataLen = Frame.ParseDataCount();
                    Debug.Print("Data Received, Additional Data: " + DataLen.ToString());
                    if (DataLen != 0) {
                        WaitFrameData(enRxStates.RcvAdditionalData, DataLen);
                    } else {
                        WaitFrameEnd();
                    }
                    break;
                case enRxStates.RcvAdditionalData:
                    Debug.Print("Additional Data Received");
                    WaitFrameEnd();
                    break;
                case enRxStates.RcvEndOfFrame:
                    Debug.Print("EndOffFrane Received");
                    MasterRequestReceived();
                    break;
            }
        }

        private void MasterRequestReceived()
        {
            try {
                SerialInterface.EndOfFrame();
                DataServices();
                SendResponseMessage();
            }
            catch (ModbusException ex) {
                Debug.Print("ModbusException  {0}", ex.ErrorCode);
            }
            WaitFrameStart();
        }
    }
}
