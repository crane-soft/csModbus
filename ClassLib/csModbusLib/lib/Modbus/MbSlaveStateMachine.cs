using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace csModbusLib
{
    public class MbSlaveStateMachine : MbSlave
    {
        private enum enRxStates
        {
            Idle,
            StartOfFrame,
            ReceiveHeader,
            RcvMessage,
            RcvAdditionalData,
            RcvEndOfFrame
        }
        private enRxStates RxState = enRxStates.Idle;
        private MbSerial SerialInterface;
        private SerialPort sp;
        private System.Timers.Timer TimeoutTimer;
        private int DataBytesNeeded;
        private int serialBytesNeeded;

        public MbSlaveStateMachine() { }
        public MbSlaveStateMachine(MbSerial Interface) : base(Interface) { }
        public MbSlaveStateMachine(MbSerial Interface, MbSlaveDataServer DataServer) : base(Interface, DataServer) { }
        override protected void StartListener()
        {
            SerialInterface = (MbSerial)gInterface;
            sp = SerialInterface.getSerialPort();

            InitTimeoutTimer();
            RxState = enRxStates.Idle;
            sp.DataReceived += SerialInterface_DataReceivedEvent;
            WaitForFrameStart();
        }

        override protected void StopListener()
        {
            sp.DataReceived -= SerialInterface_DataReceivedEvent;
            if (TimeoutTimer != null)
                TimeoutTimer.Stop();
        }

        private void WaitForFrameStart()
        {
            DataBytesNeeded = 0;
            RxState = enRxStates.StartOfFrame;
            SetTimeOut(0);
            SerialInterface_DataReceivedEvent(null, null);  // for sure if DataReceived has not yet fired but data allready avaiable
                                                            // the event funcktuin is called firs
        }

        private void WaitFrameEnd()
        {
            DataBytesNeeded = 0;
            serialBytesNeeded = SerialInterface.EndOffFrameLenthth();
            InitReceiveDataEvent(enRxStates.RcvEndOfFrame);
        }

        private void WaitFrameData(enRxStates NextState, int DataLen)
        {
            DataBytesNeeded = DataLen;
            serialBytesNeeded = SerialInterface.NumOfSerialBytes(DataLen);
            InitReceiveDataEvent(NextState);
        }

        private void InitReceiveDataEvent(enRxStates NextState)
        {
            SetTimeOut(SerialInterface.GetTimeOut_ms(serialBytesNeeded));
            RxState = NextState;

            sp.ReceivedBytesThreshold = serialBytesNeeded;  // all should be initialized now, because if alldata allready available,
                                                            // changing the tiemout property will raise the DataReceived event now
            if (RxState == NextState) {
                SerialInterface_DataReceivedEvent(null, null);  // for sure if DataReceived has not yet fired but some allready avaiable
                                                                // the event funcion ist called once
            }
                                
        }

        private void InitTimeoutTimer()
        {
            TimeoutTimer = new System.Timers.Timer();
            TimeoutTimer.AutoReset = false;
            TimeoutTimer.Elapsed += TimeoutTimer_Elapsed;
        }

        private void SetTimeOut(int timeOut)
        {
            TimeoutTimer.Stop();
            if (timeOut > 0) {
                TimeoutTimer.Enabled = false;
                TimeoutTimer.Interval = timeOut;
                TimeoutTimer.Start();
            }
        }
        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.Print("MbSlaveStateMachine Receive Timout");
            // Delay and frame Start?
            //WaitFrameStart();
        }

        private void SerialInterface_DataReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            int DataLen;

            if (RxState == enRxStates.StartOfFrame) {
                if (SerialInterface.StartOfFrameDetected()) {
                    Frame.RawData.Clear();
                    WaitFrameData(enRxStates.ReceiveHeader, 2);
                }
            } else {

                if (DataBytesNeeded > 0) {
                    if (sp.BytesToRead < serialBytesNeeded) {
                        return;
                    }

                    if (DataBytesNeeded > 0) {
                        try {
                            SerialInterface.ReceiveBytes(DataBytesNeeded);
                        }
                        catch (ModbusException ex) {
                            WaitForFrameStart();
                        }
                    }

                }


                switch (RxState) {
                    case enRxStates.ReceiveHeader:
                        DataLen = Frame.ParseMasterRequest();
                        WaitFrameData(enRxStates.RcvMessage, DataLen);
                        break;
                    case enRxStates.RcvMessage:
                        DataLen = Frame.ParseDataCount();
                        if (DataLen != 0) {
                            WaitFrameData(enRxStates.RcvAdditionalData, DataLen);
                        } else {
                            WaitFrameEnd();
                        }
                        break;
                    case enRxStates.RcvAdditionalData:
                        WaitFrameEnd();
                        break;
                    case enRxStates.RcvEndOfFrame:
                        MasterRequestReceived();
                        break;
                }
            }
        }

        private void MasterRequestReceived()
        {
            TimeoutTimer.Stop();
            try {
                gInterface.EndOfFrame();
                DataServices();
                SendResponseMessage();
            }
            catch (ModbusException ex) {
                Debug.Print("ModbusException  {0}", ex.ErrorCode);
            }
            WaitForFrameStart();
        }
    }
}

