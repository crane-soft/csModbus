using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace csModbusLib {

    public abstract class MbSerial : MbInterface {
        public enum ModbusSerialType {
            RTU = 0,
            ASCII = 1
        }
        const int DEFAULT_RESPONSE_TIMEOUT = 200;
        protected Int32 oneByteTime_us;     // 

        protected SerialPort sp;
        protected abstract bool Check_EndOfFrame(MbRawData RxData);
        protected abstract bool StartOfFrameDetected();

        public MbSerial()
        {
            Init();
        }

        public MbSerial(string PortName, int BaudRate)
        {
            Init();
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

        }

        private void Init()
        {
            sp = new SerialPort();
            oneByteTime_us = GetCharTime();

        }

        private Int32 GetCharTime()
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

            return Convert.ToInt32(Math.Ceiling(1E6 / ((double)sp.BaudRate / (double)nbits)));
        }

        protected virtual int GetTimeOut_ms (int NumBytes)
        {
            int timeOut = (NumBytes * oneByteTime_us) / 1000;
            return timeOut + 50;    // 

        }

        public override bool Connect()
        {
            IsConnected = false;

            try {
                sp.Open();
                if (sp.IsOpen) {
                    IsConnected = true;
                    sp.WriteTimeout = 600;
                }
            } catch (System.Exception ex) {
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
                if (timeout != SerialPort.InfiniteTimeout) {
                    timeout -= 10;
                    if (timeout <= 0)
                        throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
                }
                if (IsConnected == false) {
                    throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_CLOSED);
                }
            }
        }

        public override bool ReceiveHeader(DeviceType dtype, MbRawData MbData)
        {
            MbData.IniADUoffs();

            if (dtype == DeviceType.MASTER) {
                WaitFrameStart(DEFAULT_RESPONSE_TIMEOUT);
            } else {
                WaitFrameStart( SerialPort.InfiniteTimeout);
            }

            ReceiveBytes(MbData, 2); // Node-ID + Function-Byte
            return true;
        }

        public override void ReceiveBytes (MbRawData RxData, int count)
        {
            sp.ReadTimeout= GetTimeOut_ms(count);
            ReceiveBytes(RxData.Data, RxData.EndIdx, count);
            RxData.EndIdx += count;
        }

        protected virtual void ReceiveBytes (byte[] RxData, int offset, int count)
        {
            try {
                int bytesRead;
                while (count > 0) {
                    bytesRead = sp.Read(RxData, offset, count);
                    count -= bytesRead;
                    offset += bytesRead;
                }
            } catch (SystemException) {
                throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
            }
        }

        public override void EndOfFrame(MbRawData RxData)
        {
            if (Check_EndOfFrame(RxData) == false) {
               // If the server receives the request, but detects a communication error (parity, LRC, CRC,  ...),
                // no response is returned. The client program will eventually process a timeout condition for the request.
                throw new ModbusException(csModbusLib.ErrorCodes.WRONG_CRC);
            }
         }

        protected void SendData (byte[] Data, int offs, int count)
        {
            try {
                // DiscardBuffer to resync start of frame
                sp.DiscardOutBuffer();
                sp.DiscardInBuffer();

                sp.Write(Data, offs, count);

            } catch (SystemException ex) {
                Debug.Print(ex.Message);
                throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
            }
        }
    }
}
