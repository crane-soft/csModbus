using System;

namespace csModbusLib {
    #region CRC16 Class
    /// <summary>
    /// Static class for CRC16 compute
    /// </summary>
    public class CRC16 {
        // TODO CRC into Utils verschieben
        private const UInt16 POLY = 0xA001;
        private UInt16[] crc_tab16;

        private void InitCRC16Tab()
        {
            UInt16 crc, c;

            if (crc_tab16 == null) {
                crc_tab16 = new UInt16[256];
                for (int ii = 0; ii < 256; ii++) {
                    crc = 0;
                    c = (UInt16)ii;
                    for (int jj = 0; jj < 8; jj++) {

                        if (((crc ^ c) & 0x0001) == 0x0001)
                            crc = (UInt16)((crc >> 1) ^ POLY);
                        else
                            crc = (UInt16)(crc >> 1);

                        c = (UInt16)(c >> 1);
                    }

                    crc_tab16[ii] = crc;
                }
            }
        }

        private UInt16 UpdateCRC16(UInt16 crc, byte bt)
        {
            byte tableIndex = (byte)(crc ^ bt);
            crc >>= 8;
            crc ^= crc_tab16[tableIndex];
            return crc;
        }

        public UInt16 CalcCRC16(byte[] buffer, int offset, int length)
        {
            UInt16 crc = 0xFFFF;
            for (int ii = 0; ii < length; ii++)
                crc = UpdateCRC16(crc, buffer[offset + ii]);
            // I have to exange high low byte 
            return (UInt16)((crc >> 8) | ((crc & 0xff) << 8));
        }

        public CRC16()
        {
            InitCRC16Tab();
        }
    }

    #endregion

    public class MbRTU : MbSerial {

        private CRC16 crc16;
        public MbRTU() :base ()
        {
             Init();
        }

        public MbRTU(string port, int baudrate)
            : base(port, baudrate)
        {
            Init();
        }
  
        private void Init()
        {
            crc16 = new CRC16();
        }

        public override bool StartOfFrameDetected()
        {
            return (sp.BytesToRead >= 2);
        }
        public override int EndOffFrameLenthth()
        {
            return 2;
        }
        protected override bool Check_EndOfFrame()
        {
            int crc_idx = MbData.EndIdx;
            ReceiveBytes(2);

            // Check CRC
            ushort msg_crc = MbData.GetUInt16(crc_idx);
            ushort calc_crc = crc16.CalcCRC16(MbData.Data, MbRawData.ADU_OFFS, crc_idx - MbRawData.ADU_OFFS);

            return (msg_crc == calc_crc);
        }

        public override void SendFrame(int Length)
        {
            ushort calc_crc = crc16.CalcCRC16(MbData.Data, MbRawData.ADU_OFFS, Length);
            MbData.PutUInt16(MbRawData.ADU_OFFS+Length, calc_crc);
            Length += 2;
            SendData(MbData.Data, MbRawData.ADU_OFFS, Length);
        }
    }
}
