using System;
using System.IO;
using System.IO.Ports;

namespace csModbusLib
{
    public class MbASCII : MbSerial
    {
        // Modbus ASCII Frame
        // "Startchar (':') Hex-String 2-Chars LRCC - CR,LF

        public MbASCII() :base ()  { }

        public MbASCII(string port, int baudrate) : base(port, baudrate){  }

        protected override bool StartOfFrameDetected()
        {
            if (sp.BytesToRead > 0) {
                if (sp.ReadByte() == ':') {
                    return true;
                }
            }
            return false;
        }

        protected override int NumOfSerialBytes(int count)
        {
            return 2* count;
        }

        protected override void ReceiveBytes (byte[] RxData, int offset, int count)
        {
            byte[] hexchars = new byte[2];
            for (int i = 0; i < count; ++i) {
                base.ReceiveBytes(hexchars,0, 2);
                RxData[offset+i] = (byte) ASCII2Hex(hexchars);
            }
        }

        private int ASCII2Hex (byte[] hexchars)
        {
            return ((ASCII2nibble(hexchars[0]) << 4) | ASCII2nibble(hexchars[1])) & 0xff;
        }

        private int ASCII2nibble (int hexchar)
        {
            if ((hexchar >= '0') && (hexchar <= '9')) {
                return hexchar - 0x30;
            } else {
                hexchar = hexchar & ~0x20; // toupper
                if ((hexchar >= 'A') && (hexchar <= 'F')) {
                    return hexchar - ('A' - 10);
                } else {
                    return 0;
                }
            }
        }

        protected override int EndOffFrameLenthth()
        {
            return 4;   // 2 chars LRC  + CRLF
        }

        protected override bool Check_EndOfFrame()
        {
            ReceiveBytes(1);   // Read LRC

            byte[] crlf = new byte[2];       // Read CR/LF
            base.ReceiveBytes (crlf,0,2);

            // Check LRC
            byte calc_lrv = CalcLRC(MbData.Data, MbRawData.ADU_OFFS, MbData.EndIdx - MbRawData.ADU_OFFS);
            return (calc_lrv == 0);
        }

        private static byte CalcLRC(byte[] buffer, int offset, int length)
        {
            int lrc_result = 0;
            for (int i = 0; i < length; ++i) {
                lrc_result += buffer[offset + i];
                lrc_result &= 0xff;
            }
            lrc_result = (- lrc_result) & 0xff;
            return (byte) lrc_result;
        }

        public override void SendFrame(int Length)
        {
            byte lrc_value = CalcLRC(MbData.Data, MbRawData.ADU_OFFS, Length);

            MbData.Data[MbRawData.ADU_OFFS + Length] = lrc_value;
            Length += 1;
            byte[] hexbuff = new byte[Length * 2 + 3];
            hexbuff[0] = (byte)':';

            for (int i = 0; i < Length; ++i) {
                hexbuff[1 + 2 * i] = ByteToHexChar(MbData.Data[MbRawData.ADU_OFFS + i] >> 4);
                hexbuff[2 + 2 * i] = ByteToHexChar(MbData.Data[MbRawData.ADU_OFFS + i]);
            }
            hexbuff[hexbuff.Length - 2] = 0x0d;
            hexbuff[hexbuff.Length - 1] = 0x0a;

            SendData(hexbuff, 0, hexbuff.Length);
        }

        private static byte ByteToHexChar(int b)
        {
            b &= 0x0f;
            int hexChar = b < 10 ? (b + 48) : (b + 55);
            return (byte)hexChar;
        }
    }
}
