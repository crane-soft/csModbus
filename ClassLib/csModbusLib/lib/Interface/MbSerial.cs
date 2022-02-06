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

        private int oneByteTime_us;     

        protected SerialPort sp = new SerialPort();
        protected abstract bool StartOfFrameDetected();
        protected abstract bool Check_EndOfFrame(MbRawData RxData);

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
                    sp.WriteTimeout = 200;
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

        public override void ReceiveHeader(int timeOut, MbRawData RxData)
        {
            RxData.IniADUoffs();
            WaitFrameStart(timeOut);
            ReceiveBytes(RxData, 2); // Node-ID + Function-Byte
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
