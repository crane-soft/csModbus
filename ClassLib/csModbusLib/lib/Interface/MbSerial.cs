using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace csModbusLib
{

    public delegate void DataReceivedHandler();

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
            oneByteTime_us = GetCharTime();
        }


        private int GetCharTime()
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

        protected int GetTimeOut_ms(int ByteCount)
        {
            ByteCount = RawNumOfBytes(ByteCount);
            int timeOut = (ByteCount * oneByteTime_us) / 1000;
            return timeOut + 50;    // 
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
                        throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
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
            ReceiveBytes( 2); // Node-ID + Function-Byte
        }

        public override void ReceiveBytes(int count)
        {
            sp.ReadTimeout = GetTimeOut_ms(count);
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
                throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
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
        public bool BytesAvailable(int bytesNeeded)
        {
            bytesNeeded = RawNumOfBytes(bytesNeeded);
            return (sp.BytesToRead >= bytesNeeded);
        }

        protected virtual int RawNumOfBytes(int count)
        {
            return count;
        }

        // Event driven section
        public event DataReceivedHandler DataReceivedEvent;

        private int DataNeeded;

        public void Wait4FrameStartEvent()
        {
            SetDataReceivedEventHandler (Sp_Wait4FrameStart);
        }
        public void Wait4Date(int DataCount = 0)
        {
            DataNeeded = DataCount;
            SetDataReceivedEventHandler(Sp_DataReceived);
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
            if (BytesAvailable(DataNeeded)) {
                ReceiveBytes(DataNeeded);
                RaiseReceiveEvent();
            }
        }
        private void RaiseReceiveEvent()
        {
            ClearDataReceivedEventHandler();
            if (DataReceivedEvent != null)
                DataReceivedEvent();

        }
    }
}