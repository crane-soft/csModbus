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

        protected SerialPort sp;
  
        protected abstract bool Check_EndOfFrame(MbRawData RxData);
        protected abstract bool StartOfFrameFound ();

        public MbSerial()
        {
            sp = new SerialPort();
            Init();
        }

        public MbSerial(string PortName, int BaudRate, int databits, Parity parity, StopBits stopbits, Handshake handshake)
        {
            sp = new SerialPort();
            SetComParms(PortName, BaudRate, databits, parity, stopbits, handshake);
            Init();
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

        private void Init ()
        {
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
                Thread.Sleep(50);    // wait until DataReceivedHandler has finisched
                sp.Close();
            }
        }

        public override bool ReceiveHeader (MbRawData MbData)
        {
            sp.ReadTimeout = 1;
            if (StartOfFrameFound()) {
                sp.ReadTimeout = rx_timeout;
                MbData.IniADUoffs();
                ReceiveBytes(MbData, 2); // Receive Address + Function-Byte
                return true;
            }
            return false;
        }

        public override void ReceiveBytes (MbRawData RxData, int count)
        {
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

            } catch (SystemException) {
                throw new ModbusException(csModbusLib.ErrorCodes.TX_TIMEOUT);
            }
        }
    }
}
