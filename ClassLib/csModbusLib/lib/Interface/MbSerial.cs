using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace csModbusLib
{

    public delegate void DataReceivedHandler(ModbusException ex);

    public abstract class MbSerial : MbInterface
    {
        public enum ModbusSerialType
        {
            RTU = 0,
            ASCII = 1
        }


        protected SerialPort sp = new SerialPort();
        protected abstract bool StartOfFrameDetected();
        protected abstract bool Check_EndOfFrame();
        protected abstract int EndOffFrameLenthth();

        private int oneByteTime_us;
        public MbSerial()
        {
        }

        public MbSerial(string PortName, int BaudRate)
        {
            SetComParms(PortName, BaudRate);
        }

        public void SetComParms(string PortName, int BaudRate)
        {
            SetComParms(PortName, BaudRate, 8, Parity.None, StopBits.One, Handshake.None);
        }

        public void SetComParms(string PortName, int BaudRate, int DataBits, Parity Parity, StopBits StopBits, Handshake Handshake)
        {
            sp.PortName = PortName;
            sp.BaudRate = BaudRate;
            sp.DataBits = DataBits;
            sp.Parity = Parity;
            sp.StopBits = StopBits;
            sp.Handshake = Handshake;
            sp.RtsEnable = false;
            oneByteTime_us = SerialByteTime();
        }

        private int SerialByteTime()
        {
            int nbits = 1 + sp.DataBits;
            nbits += sp.Parity == Parity.None ? 0 : 1;
            switch (sp.StopBits) {
                case StopBits.One:
                    nbits += 1;
                    break;

                case StopBits.OnePointFive: // Ceiling
                case StopBits.Two:
                    nbits += 2;
                    break;
            }

            return (1000000 * nbits) / sp.BaudRate;
        }

        protected int GetTimeOut_ms(int serialBytesCnt)
        {
            if (serialBytesCnt == 0)
                return 0;
            int timeOut = (serialBytesCnt * oneByteTime_us) / 1000;
            return timeOut + 50;    // we need more timeout in a Windoww environement
        }

        public override bool Connect(MbRawData Data)
        {
            base.Connect(Data);
            try {
                sp.Open();
                if (sp.IsOpen) {
                    IsConnected = true;
                    sp.WriteTimeout = 200;
                }
            }
            catch (System.Exception ex) {
                Debug.Print(ex.Message);
                IsConnected = false;
            }
            return IsConnected;
        }

        public override void DisConnect()
        {
            if (IsConnected) {
                IsConnected = false;
                Thread.Sleep(10);
                sp.Close();
            }
        }

        private void WaitFrameStart(int timeout)
        {
            while (StartOfFrameDetected() == false) {
                Thread.Sleep(10);
                if (timeout != MbInterface.InfiniteTimeout) {
                    timeout -= 10;
                    if (timeout <= 0)
                        throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
                }
                if (IsConnected == false) {
                    throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_CLOSED);
                }
            }
        }


        public override void ReceiveHeader(int timeOut)
        {
            MbData.IniADUoffs();
            WaitFrameStart(timeOut);
            ReceiveBytes(2); // Node-ID + Function-Byte
        }

        public override void ReceiveBytes(int count)
        {
            sp.ReadTimeout = GetTimeOut_ms(NumOfSerialBytes(count));
            ReceiveBytes(MbData.Data, MbData.EndIdx, count);
            MbData.EndIdx += count;
        }

        protected virtual void ReceiveBytes(byte[] RxData, int offset, int count)
        {
            try {
                int bytesRead;
                while (count > 0) {
                    bytesRead = sp.Read(RxData, offset, count);
                    count -= bytesRead;
                    offset += bytesRead;
                }
            }
            catch (SystemException) {
                throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
            }
        }

        public override void EndOfFrame()
        {
            if (Check_EndOfFrame() == false) {
                // If the server receives the request, but detects a communication error (parity, LRC, CRC,  ...),
                // no response is returned. The client program will eventually process a timeout condition for the request.
                throw new ModbusException(csModbusLib.ErrorCodes.WRONG_CRC);
            }
        }

        protected void SendData(byte[] Data, int offs, int count)
        {
            try {
                // DiscardBuffer to resync start of frame
                sp.DiscardOutBuffer();
                sp.DiscardInBuffer();

                sp.Write(Data, offs, count);

            }
            catch (SystemException ex) {
                Debug.Print(ex.Message);
                throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
            }
        }
        private bool BytesAvailable(int bytesNeeded)
        {
            bytesNeeded = NumOfSerialBytes(bytesNeeded);
            return (sp.BytesToRead >= bytesNeeded);
        }

        protected virtual int NumOfSerialBytes(int count)
        {
            return count;   // Default (RTU) , ASCI will have double of count
        }
    //}
    #region Event driven section
    //public abstract class MbStateSerial : MbSerial { 
        public event DataReceivedHandler DataReceivedEvent;
        private int DataBytesNeeded;
        private int serialBytesNeeded;

        public void Wait4FrameStartEvent()
        {
            SetDataReceivedEventHandler (Sp_Wait4FrameStart);
        }

        public int Wait4Bytes(int ByteCount)
        {
            DataBytesNeeded = ByteCount;
            serialBytesNeeded = NumOfSerialBytes(ByteCount);
            return InitReceiveByteEvent();
        }

        public int Wait4FrameEndEvent()
        {
            DataBytesNeeded = 0;
            serialBytesNeeded = EndOffFrameLenthth();
            return InitReceiveByteEvent();
        }

        private int InitReceiveByteEvent()
        {
            sp.ReceivedBytesThreshold = serialBytesNeeded;
            SetDataReceivedEventHandler(Sp_DataReceived);
            return GetTimeOut_ms(serialBytesNeeded);
        }

        private void SetDataReceivedEventHandler(SerialDataReceivedEventHandler handler)
        {
            ClearDataReceivedEventHandler();
            sp.DataReceived += handler;
            handler(null, null);
        }

        private void ClearDataReceivedEventHandler()
        {
            sp.DataReceived -= Sp_Wait4FrameStart;
            sp.DataReceived -= Sp_DataReceived;
        }

        private void Sp_Wait4FrameStart(object sender, SerialDataReceivedEventArgs e)
        {
            if (StartOfFrameDetected()) {
                MbData.IniADUoffs();
                RaiseReceiveEvent();
            }
        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sp.BytesToRead >= serialBytesNeeded) {
                if (DataBytesNeeded > 0) {
                    try {
                        ReceiveBytes(DataBytesNeeded);
                    } catch (ModbusException ex) {
                        RaiseReceiveEvent(ex);
                    }
                }
                RaiseReceiveEvent();
            }
        }

        private void RaiseReceiveEvent(ModbusException ex = null)
        {
            ClearDataReceivedEventHandler();
            serialBytesNeeded = 0;
            DataBytesNeeded = 0;

            if (DataReceivedEvent != null)
                DataReceivedEvent(ex);
        }
    }
    #endregion
}
