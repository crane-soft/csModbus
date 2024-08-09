using System;
using System.Collections;
using System.Diagnostics;

/* Modbus Format
   Request Format                   0           1           2           4           6
   READ_COILS = 0x01,               SlaveID(1)  FCode(1)    Address(2)  Count(2)
   READ_DISCRETE_INPUTS = 0x02,     SlaveID(1)  FCode(1)    Address(2)  Count(2)
   READ_HOLDING_REGISTERS = 0x03,   SlaveID(1)  FCode(1)    Address(2)  Count(2)
   READ_INPUT_REGISTERS = 0x04,     SlaveID(1)  FCode(1)    Address(2)  Count(2)
  
   WRITE_SINGLE_COIL = 0x05,        SlaveID(1)  FCode(1)    Address(2)  Value(2)
   WRITE_SINGLE_REGISTER = 0x06,    SlaveID(1)  FCode(1)    Address(2)  Value(2)
   READ_EXCEPTION_STATUS = 0x07,    -----------------

   WRITE_MULTIPLE_COILS = 0x0F,     SlaveID(1)  FCode(1)    Address(2)  Count(2)    NumBytes(1)  Data
   WRITE_MULTIPLE_REGISTERS = 0x10, SlaveID(1)  FCode(1)    Address(2)  Count(2)    NumBytes(1)  Data
   REPORT_SLAVE_ID = 0x11,
   READ_WRITE_MULTIPLE_REGS = 0x17, SlaveID(1)  FCode(1) 	RdAddr(2)	RdCnt(2)	WrAddr(2)	WrCnd(2)	NumBytes(1)  Data
 

   Respones Format
   READ_COILS = 0x01,               SlaveID(1)  FCode(1)    NumBytes(1)  Data
   READ_DISCRETE_INPUTS = 0x02,     SlaveID(1)  FCode(1)    NumBytes(1)  Data
   READ_HOLDING_REGISTERS = 0x03,   SlaveID(1)  FCode(1)    NumBytes(1)  Data
   READ_INPUT_REGISTERS = 0x04,     SlaveID(1)  FCode(1)    NumBytes(1)  Data
  
   WRITE_SINGLE_COIL = 0x05,        SlaveID(1)  FCode(1)    Address(2)  Value(2)
   WRITE_SINGLE_REGISTER = 0x06,    SlaveID(1)  FCode(1)    Address(2)  Value(2)
   READ_EXCEPTION_STATUS = 0x07,    -----------------

   WRITE_MULTIPLE_COILS = 0x0F,     SlaveID(1)  FCode(1)    Address(2)  Count(2)   
   WRITE_MULTIPLE_REGISTERS = 0x10, SlaveID(1)  FCode(1)    Address(2)  Count(2)   
   READ_WRITE_MULTIPLE_REGS = 0x17	SlaveID(1)  FCode(1)    NumBytes(1)  Data
   Exception Response               SlaveID(1)  FCode(1 	ExexptionCode(1)
 
 */

namespace csModbusLib
{
    public class MbRawData
    {
        public const int ADU_OFFS = 6;

        public int EndIdx = 0;
        public Byte[] Data = null; 

        public MbRawData() {
            Init(MbBase.MAX_FRAME_LEN);
        }

        public MbRawData(int Size) {
            Init(Size);
        }

        private void Init(int Size) {
            Data = new Byte[Size];
            EndIdx = 0;
        }

        public void Clear() {
            EndIdx = ADU_OFFS;
        }

        public void CopyFrom (MbRawData source)
        {
            Array.Copy(source.Data, Data, source.EndIdx);
            EndIdx = source.EndIdx;
        }

        public void CopyFrom (byte[] source ,int srcIdx, int length)
        {
            int maxLength = Data.Length - EndIdx;
            if (length > maxLength) {
                length = maxLength;
            }
            Array.Copy(source, srcIdx, Data, EndIdx, length);
            EndIdx += length;
        }

        public ushort GetUInt16(int ByteOffs)
        {
            return (ushort)((Data[ByteOffs] << 8) | (Data[ByteOffs + 1] & 0x00FF));
        }

        public void PutUInt16(int ByteOffs, UInt16 Value)
        {
            Data[ByteOffs] = (Byte)(Value >> 8);
            Data[ByteOffs+1] = (Byte)(Value & 0x00FF);
        }

        public void CopyUInt16 (UInt16[] DestArray, int SrcOffs, int DestOffs, int Length)
        {
            for (int i = 0; i < Length; ++i)
                DestArray[DestOffs + i] = GetUInt16(SrcOffs + i * 2);
        }

        public void FillUInt16(UInt16[] SrcArray, int SrcOffs, int DestOffs, int Length)
        {
            for (int i = 0; i < Length; ++i)
                PutUInt16(DestOffs+ i * 2, SrcArray[i]);
        }

        public int CheckEthFrameLength()
        {
            int frameLength = GetUInt16(ADU_OFFS-2);
            int bytesleft = (frameLength + ADU_OFFS) - EndIdx;
            return bytesleft;
        }
    }

    public class MbFrame {
        protected const int REQST_UINIT_ID_IDX = MbRawData.ADU_OFFS + 0;
        protected const int REQST_FCODE_IDX = MbRawData.ADU_OFFS + 1;
        protected const int RESPNS_ERR_IDX = MbRawData.ADU_OFFS + 2;

        protected const int REQST_ADDR_IDX = MbRawData.ADU_OFFS + 2;
        protected const int REQST_SINGLE_DATA_IDX = MbRawData.ADU_OFFS + 4;
        protected const int REQST_DATA_CNT_IDX = MbRawData.ADU_OFFS + 4;   // Request for Single Write Functions
        protected const int REQST_DATA_LEN_IDX = MbRawData.ADU_OFFS + 6;
        protected const int REQST_WRADDR_IDX = MbRawData.ADU_OFFS + 6;

        protected const int REQST_DATA_IDX = MbRawData.ADU_OFFS + 7;      // 
        protected const int RESPNS_DATA_IDX = MbRawData.ADU_OFFS + 3;           // Response for Read Functions
        protected const int RESPNS_LEN_IDX = MbRawData.ADU_OFFS + 2;

        public int SlaveId = 0;
        public ModbusCodes FunctionCode;
        public UInt16 DataAddress;
        public UInt16 DataCount;

        public ExceptionCodes ExceptionCode { get; set; }
        public MbRawData RawData;
        
        public MbFrame()
        {
            RawData = new MbRawData ();
            ExceptionCode = ExceptionCodes.NO_EXCEPTION;
        }

        public void GetBitData(UInt16[] DestBits, int DestIdx, int FrameIdx)
        {
            int bitCnt = 0;
            byte dataByte = 0;

            for (int i = 0; i < DataCount; ++i) {
                if (bitCnt == 0) {
                    dataByte = RawData.Data[FrameIdx++];
                }
                int b = dataByte & 1;
                DestBits[DestIdx++] = (ushort)b;
                dataByte >>= 1;
                bitCnt = (bitCnt + 1) & 0x07;
            }
        }

        public int PutBitData(UInt16[] SrcBits, int SrcIdx, int FrameIdx)
        {
            int bitCnt = 8;
            byte dataByte = 0;
            int NumBytes = 0;

            for (int i = 0; i < DataCount; ++i) {
                dataByte >>= 1;
                if (SrcBits[SrcIdx++] != 0) {
                    dataByte |= 0x80;
                }

                if (--bitCnt == 0) {
                    RawData.Data[FrameIdx+ NumBytes] = dataByte;
                    bitCnt = 8;
                    dataByte = 0;
                    ++NumBytes;
                }
            }
            if (bitCnt != 0) {
                dataByte >>= bitCnt;
                RawData.Data[FrameIdx + NumBytes] = dataByte;
                ++NumBytes;
            } else {
                bitCnt = 1;
            }
            RawData.Data[FrameIdx - 1] = (byte)NumBytes;
            return NumBytes;
        }

        protected int ResponseMessageLength()
        {
            if (FunctionCode <= ModbusCodes.READ_INPUT_REGISTERS) {
                return 3;
            }

            switch (FunctionCode) {
                case ModbusCodes.WRITE_SINGLE_COIL:
                case ModbusCodes.WRITE_SINGLE_REGISTER:
                case ModbusCodes.WRITE_MULTIPLE_COILS:
                case ModbusCodes.WRITE_MULTIPLE_REGISTERS:
                    return 6;
            }
            throw new ModbusException(ErrorCodes.ILLEGAL_FUNCTION_CODE);
        }
    }

    /* -------------------------------------------------------------------------------
     * Modbus-Slave Frame
     ---------------------------------------------------------------------------------*/
    public class MBSFrame : MbFrame 
    {
        private bool WrMultipleData;
        private bool WrSingleData;
        private bool ValidAddressFound;
        private MbRawData WriteData;
        public MBSFrame()
        {
            ValidAddressFound = false;
        }

        private int FromMasterRequestMessageLen()
        {
            WrSingleData = false;
            WrMultipleData = false;

            if (FunctionCode <= ModbusCodes.WRITE_SINGLE_REGISTER) {
                if (FunctionCode >= ModbusCodes.WRITE_SINGLE_COIL) {
                    WrSingleData = true;
                }
                return 4;
            }

            switch (FunctionCode) {
                case ModbusCodes.WRITE_MULTIPLE_COILS:
                case ModbusCodes.WRITE_MULTIPLE_REGISTERS:
                    WrMultipleData = true;
                    return 5;
                case ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS:
                    WrMultipleData = true;
                    return 9;
            }

            throw new ModbusException(ErrorCodes.ILLEGAL_FUNCTION_CODE);
        }

        public int ParseMasterRequest()
        {
            ExceptionCode = ExceptionCodes.NO_EXCEPTION;
            SlaveId = RawData.Data[REQST_UINIT_ID_IDX];
            FunctionCode = (ModbusCodes)RawData.Data[REQST_FCODE_IDX];

            //Debug.WriteLine(String.Format("Rx: Cmd{0}", FunctionCode));
            int MsgLen = FromMasterRequestMessageLen();
            return MsgLen;
        }

        public int ParseDataCount()
        {
            DataAddress = RawData.GetUInt16(REQST_ADDR_IDX);

            int AdditionalData = 0;

            if (WrSingleData == true) {
                DataCount = 1;
            } else {
                DataCount = RawData.GetUInt16(REQST_DATA_CNT_IDX);
                if (WrMultipleData) {
                    if (FunctionCode == ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS) {
                        AdditionalData = RawData.Data[REQST_DATA_LEN_IDX + 4];

                        // Create extra RawData for Write request
                        WriteData = new MbRawData(REQST_DATA_IDX + AdditionalData);
                        // Copy Head NodeID and Function Code
                        WriteData.CopyFrom(RawData.Data, 0, REQST_ADDR_IDX);
                        // Copy  the write data
                        WriteData.CopyFrom(RawData.Data, REQST_WRADDR_IDX, AdditionalData + 5);

                    } else {
                        AdditionalData = RawData.Data[REQST_DATA_LEN_IDX];
                    }

                }
            }
            return AdditionalData;
        }
         public void ReceiveMasterRequest(MbInterface Interface)
        {
            int MsgLen = ParseMasterRequest();
            Interface.ReceiveBytes(MsgLen);

            int AdditionalData = ParseDataCount();
            if (AdditionalData != 0) {
                Interface.ReceiveBytes(AdditionalData);
            }

            Interface.EndOfFrame();
        }

        public void GetRwWriteAddress()
        {
            // Write Address for READ_WRITE_MULTIPLE_REGISTERS
            DataAddress = WriteData.GetUInt16(REQST_ADDR_IDX);
            DataCount = WriteData.GetUInt16(REQST_DATA_CNT_IDX );
        }

        private void ExceptionResponse(ExceptionCodes ErrorCode)
        {
            RawData.Data[REQST_FCODE_IDX] |= 0x80;
            RawData.Data[RESPNS_ERR_IDX] = (Byte)ErrorCode;
        }

        public int ToMasterResponseMessageLength()
        {
            if (ExceptionCode == ExceptionCodes.NO_EXCEPTION) {
                if (ValidAddressFound == false) {
                    ExceptionCode = ExceptionCodes.ILLEGAL_DATA_ADDRESS;
                }
            }

            if (ExceptionCode != ExceptionCodes.NO_EXCEPTION) {
                ExceptionResponse(ExceptionCode);
                return 3;
            }

            if ((FunctionCode <= ModbusCodes.READ_INPUT_REGISTERS) || (FunctionCode == ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS)) {
                return 3 + RawData.Data[RESPNS_LEN_IDX];
            } else {
                return ResponseMessageLength();
            }
        }

        public bool MatchAddress(int BaseAddr, int Size)
        {
            int EndAddr = BaseAddr + Size;
            // TODO wenn Size > als Array Fehler erzeugen
            if ((DataAddress >= BaseAddr) && (DataAddress < EndAddr)) {
                if ((DataAddress + DataCount - 1) < EndAddr) {
                    ValidAddressFound = true;
                    return true;
                }
            }
            return false;
        }

        public ushort GetRequestSingleUInt16()
        {
            return RawData.GetUInt16(REQST_SINGLE_DATA_IDX);
        }

        public ushort GetRequestSingleBit()
        {
            return RawData.Data[REQST_SINGLE_DATA_IDX];
        }

        public void PutResponseBitValues(int BaseAddr, ushort[] SrcBits)
        {
            PutBitData(SrcBits, DataAddress - BaseAddr, RESPNS_DATA_IDX);
        }

        public void PutResponseValues(int BaseAddr, UInt16[] RegisterArray)
        {
            for (int i = 0; i < DataCount; ++i) {
                ushort Value = RegisterArray[DataAddress - BaseAddr + i];
                RawData.PutUInt16(RESPNS_DATA_IDX + i * 2, Value);

            }
            RawData.Data[RESPNS_LEN_IDX] = (byte)(DataCount * 2);
        }

        public void GetRequestBitValues(int BaseAddr, UInt16[] DestBits)
        {
            GetBitData(DestBits, DataAddress - BaseAddr, REQST_DATA_IDX);
        }

        public void GetRequestValues(int BaseAddr, UInt16[] DestArray)
        {
            MbRawData SrcData;
            if (FunctionCode == ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS) {
                SrcData = WriteData;
            } else {
                SrcData = RawData;
            }
            SrcData.CopyUInt16(DestArray, REQST_DATA_IDX, DataAddress - BaseAddr, DataCount);
        }
    }

    /* ------------------------------------------------------------
     * Modbus Frame Master Functions  
     -------------------------------------------------------------- */
    public class MBMFrame : MbFrame 
    {
        private const int SLAVE_DATA_IDX = MbRawData.ADU_OFFS + 3;
        private Byte Current_SlaveID;

        public Byte Slave_ID
        {
            get { return Current_SlaveID;   }
            set {  Current_SlaveID = value; }
        }

        public int BuildRequest(ModbusCodes FuncCode, ushort Address, ushort DataOrLen)
        {
            DataAddress = Address;
            DataCount = DataOrLen;   // Data kan sein DataCount oder Single Data
            RawData.Data[REQST_UINIT_ID_IDX] = Current_SlaveID;
            RawData.Data[REQST_FCODE_IDX] = (Byte)FuncCode;
            RawData.PutUInt16(REQST_ADDR_IDX, Address);
            RawData.PutUInt16(REQST_SINGLE_DATA_IDX, DataOrLen);
            return 6;
        }

        public int BuildMultipleWriteRequest(ModbusCodes FuncCode, ushort Address, ushort Length, ushort[] SrcData, int SrcOffs) 
        {
            BuildRequest(FuncCode, Address, Length);
            int NumDataBytes;

            if (FuncCode == ModbusCodes.WRITE_MULTIPLE_REGISTERS) {
                NumDataBytes = Length * 2; // TODO überflow
                RawData.Data[REQST_DATA_LEN_IDX] = (Byte)NumDataBytes;
                RawData.FillUInt16(SrcData, SrcOffs, REQST_DATA_IDX, Length);
            } else if (FuncCode == ModbusCodes.WRITE_MULTIPLE_COILS) {
                NumDataBytes = PutBitData(SrcData, SrcOffs, REQST_DATA_IDX);
            }else {
                return 0;
            }
            return 7 + NumDataBytes;
        }

        public int BuildMultipleReadWriteRequest(ushort RdAddress, ushort RdLength, ushort WrAddress, ushort WrLength, ushort[] SrcData, int SrcOffs)
        {
            BuildRequest(ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS, RdAddress, RdLength);
            RawData.PutUInt16(REQST_ADDR_IDX+4, WrAddress);
            RawData.PutUInt16(REQST_DATA_CNT_IDX+4, WrLength);

            int NumDataBytes = WrLength * 2; // TODO overflow
            RawData.Data[REQST_DATA_LEN_IDX+4] = (Byte)NumDataBytes;
            RawData.FillUInt16(SrcData, SrcOffs, REQST_DATA_IDX+4, WrLength);
            return 11 + NumDataBytes;
        }

        public void ReceiveSlaveResponse(MbInterface Interface)
        {
            SlaveId = RawData.Data[REQST_UINIT_ID_IDX];
            byte RetCode = RawData.Data[REQST_FCODE_IDX];
            if ((RetCode & 0x80) != 0) {
                // Slave reports exception error
                Interface.ReceiveBytes(1);
                ExceptionCode = (ExceptionCodes)RawData.Data[RESPNS_ERR_IDX];
                throw new ModbusException(ErrorCodes.MODBUS_EXCEPTION);
            }

            FunctionCode = (ModbusCodes)RetCode;

            int Bytes2Read;
            if ((FunctionCode <= ModbusCodes.READ_INPUT_REGISTERS) || (FunctionCode == ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS)) {
                Interface.ReceiveBytes(1);
                Bytes2Read = RawData.Data[RESPNS_LEN_IDX];
            } else {
                Bytes2Read = ResponseMessageLength() - 2;
            }
            if (Bytes2Read > 0)
                Interface.ReceiveBytes(Bytes2Read);
            Interface.EndOfFrame();
        }

        public void ReadSlaveRegisterValues(UInt16[] DestArray, int DestOffs)
        {
            RawData.CopyUInt16(DestArray, SLAVE_DATA_IDX, DestOffs, DataCount);
        }

        public void ReadSlaveBitValues(ushort[] DestBits, int DestOffs)
        {
            GetBitData(DestBits, DestOffs, SLAVE_DATA_IDX);
        }

        public ushort GetTransactionIdentifier()
        {
            return RawData.GetUInt16(0);
        }
    }
}
