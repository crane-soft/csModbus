using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace csModbusLib
{
    public class MbMasterBase : MbBase
    {
        protected MBMFrame Frame = new MBMFrame();
        private byte Current_SlaveID = 0;
        protected ErrorCodes LastError = ErrorCodes.NO_ERROR;

        #region Constructors

        public MbMasterBase() { }

        public MbMasterBase(MbInterface Interface)
        {
            InitInterface(Interface);
        }

        #endregion

        #region Properties
        public void setLongEndianess(B32Endianess Endianess)
        {
            Frame.setLongEndianess(Endianess);
        }

        public bool IsConnected
        {
            get {
                return running;
            }
        }

        public byte Slave_ID
        {
            get {
                return Current_SlaveID;
            }
            set {
                Current_SlaveID = value;
                Frame.Slave_ID = value;
            }
        }

        public ErrorCodes ErrorCode
        {
            get {
                return LastError;
            }
        }

        public ushort GetTransactionIdentifier()
        {
            return Frame.GetTransactionIdentifier();
        }

        #endregion

        #region Connection
        public Boolean Connect()
        {
            if (gInterface != null) {
                if (running) {
                    Close();
                }
                if (gInterface.Connect(Frame.RawData)) {
                    running = true;
                    return true;
                }
            }
            return false;
        }

        public Boolean Connect(byte newSlaveID)
        {
            Slave_ID = newSlaveID;
            return Connect();
        }

        public Boolean Connect(MbInterface Interface, byte newSlaveID)
        {
            InitInterface(Interface);
            Slave_ID = newSlaveID;
            return Connect();
        }

        public void Close()
        {
            running = false;
            if (gInterface != null) {
                gInterface.DisConnect();
            }
        }
        #endregion

        #region Functions
        public ExceptionCodes GetModusException()
        {
            if (Frame != null) {
                return Frame.ExceptionCode;
            }
            return ExceptionCodes.NO_EXCEPTION;
        }

        protected int DataTypeLength<DataT>(int Length)
        {
            if (B32Converter.Is16BitType<DataT>())
                return Length;
            if (B32Converter.Is32BitType<DataT>())
                return Length * 2;
            LastError = ErrorCodes.ILLEGAL_DATA_TYPE;
            return 0;
        }

        protected ErrorCodes ExeSingleCoilsRequest(ModbusCodes Fcode, ushort Address, ushort[] DestData, int Length = 0, int DestOffs= 0)
        {
            if (Length == 0)
                Length = DestData.Length;
            if (SendSingleRequest(Fcode, Address, Length))
                ReadSlaveBitValues(DestData, DestOffs);
            return LastError;
        }

        protected ErrorCodes ExeSingleRegsRequest<DataT>(ModbusCodes Fcode, ushort Address, DataT[] DestData, int Length = 0, int DestOffs = 0)
        {
            if (Length == 0)
                Length = DestData.Length;
            Length = DataTypeLength<DataT>(Length);
            if (Length > 0) {
                if (SendSingleRequest(Fcode, Address, Length))
                    ReadSlaveRegisterValues(DestData, DestOffs);
            }
            return LastError;
        }

        protected bool SendSingleRequest(ModbusCodes Fcode, ushort Address, int DataOrLen)
        {
            int MsgLen = Frame.BuildRequest(Fcode, Address, DataOrLen);
            return SendRequestMessage(MsgLen);
        }

        protected bool WriteMultipleCoilsRequest(ushort Address, ushort[] SrcData, int Length = 0, int SrcOffs = 0)
        {
            int MsgLen = Frame.BuildMultipleWriteCoilsRequest(Address,  SrcData, Length, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        protected bool WriteMultipleRegsRequest<DataT>(ushort Address,  DataT[] SrcData, int Length, int SrcOffs)
        {
            int MsgLen = Frame.BuildMultipleWriteRegsRequest(Address, SrcData, Length, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        protected bool SendMultipleReadWriteRequest<DataT>( ushort RdAddress, int RdLength, 
                                                            ushort WrAddress, DataT[] SrcData, int WrLength, int SrcOffs)
        {
            int MsgLen = Frame.BuildMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, SrcData, WrLength, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        private bool SendRequestMessage(int MsgLen)
        {   // TODO check if connected
            if (MsgLen <= 0) {
                LastError = ErrorCodes.ILLEGAL_DATA_TYPE;
                return false;
            }

            LastError = ErrorCodes.NO_ERROR;
            try {
                gInterface.SendFrame(MsgLen);
            }
            catch (ModbusException ex) {
                LastError = ex.ErrorCode;
                //if (running) {
                //    gInterface.ReConnect();
                //}
                return false;
            }
            catch (Exception ex) {
                LastError = csModbusLib.ErrorCodes.CONNECTION_ERROR;
                Debug.Print(ex.Message);
                return false;
            }
            return true;
        }

        protected bool ReadSlaveBitValues(ushort[] DestArray, int DestOffs)
        {
            if (ReceiveSlaveResponse()) {
                Frame.ReadSlaveBitValues(DestArray, DestOffs);
                return true;
            }
            return false;
        }

        protected bool ReadSlaveRegisterValues<DataT>(DataT[] DestArray, int DestOffs)
        {
            if (ReceiveSlaveResponse()) {
                Frame.ReadSlaveRegisterValues(DestArray, DestOffs);
                return true;
            }
            return false;
        }

        protected bool ReceiveSlaveResponse()
        {
            try {
                gInterface.ReceiveHeader(MbInterface.ResponseTimeout);
                Frame.ReceiveSlaveResponse(gInterface);
            }
            catch (ModbusException ex) {
                //if ((ex.ErrorCode != ErrorCodes.CONNECTION_CLOSED) && (ex.ErrorCode != ErrorCodes.MODBUS_EXCEPTION))
                //    gInterface.ReConnect();
                LastError = ex.ErrorCode;
                return false;
            }
            return true;
        }

        #endregion
    }

    public class MbMaster : MbMasterBase
    {
        public MbMaster() { }
        public MbMaster(MbInterface Interface) : base(Interface) { }

        #region Coil Functions

        public ErrorCodes ReadCoils(ushort Address, ushort[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ExeSingleCoilsRequest(ModbusCodes.READ_COILS, Address, DestData, Length, DestOffs);
        }
        public ErrorCodes ReadDiscreteInputs(ushort Address, ushort[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ExeSingleCoilsRequest(ModbusCodes.READ_DISCRETE_INPUTS, Address, DestData, Length, DestOffs);
        }
        public ErrorCodes WriteSingleCoil(ushort Address, ushort BitData)
        {
            BitData = (ushort)((BitData != 0) ? 0xff00 : 0);
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_COIL, Address, BitData))
                ReceiveSlaveResponse();
            return LastError;
        }
        public ErrorCodes WriteMultipleCoils(ushort Address, ushort[] SrcData, int Length = 0, int SrcOffs = 0)
        {
            if (WriteMultipleCoilsRequest(Address,  SrcData, Length, SrcOffs))
                ReceiveSlaveResponse();
            return LastError;
        }
        #endregion

        #region protected Generic Register Functions
        protected ErrorCodes ReadHoldingRegisters<DataT>(ushort Address, DataT[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ExeSingleRegsRequest(ModbusCodes.READ_HOLDING_REGISTERS, Address, DestData, Length, DestOffs);
        }
        protected ErrorCodes ReadInputRegisters<DataT>(ushort Address, DataT[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ExeSingleRegsRequest(ModbusCodes.READ_INPUT_REGISTERS, Address, DestData, Length, DestOffs);
        }
        protected ErrorCodes WriteSingleRegister<DataT>(ushort Address, DataT Data)
        {
            if (B32Converter.Is16BitType<DataT>()) {
                if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_REGISTER, Address, (dynamic)Data))
                    ReceiveSlaveResponse();
            } else {
                DataT[] srcData = new DataT[] { Data };
                WriteMultipleRegisters(Address, srcData);
            }
            return LastError;
        }
        protected ErrorCodes WriteMultipleRegisters<DataT>(ushort Address, DataT[] SrcData, int Length= 0, int SrcOffs = 0)
        {
            if (WriteMultipleRegsRequest(Address,  SrcData, Length, SrcOffs))
                ReceiveSlaveResponse();

            return LastError;
        }
        protected ErrorCodes ReadWriteMultipleRegisters<DataT>(ushort RdAddress, DataT[] DestData, int RdLength,
                                                               ushort WrAddress, DataT[] SrcData,  int WrLength, int DestOffs = 0, int SrcOffs = 0)
        {
            if (SendMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, SrcData, WrLength, SrcOffs))
                ReadSlaveRegisterValues(DestData, DestOffs);
            return LastError;
        }
        #endregion

        #region <ushort> Register Functions 
        public ErrorCodes ReadHoldingRegisters(ushort Address, ushort[] DestData, int Length=0, int DestOffs = 0)
        {
            return ReadHoldingRegisters<ushort>(Address, DestData, Length, DestOffs);
        }
        public ErrorCodes ReadInputRegisters(ushort Address,  ushort[] DestData, int Length=0, int DestOffs = 0)
        {
            return ReadInputRegisters<ushort>(Address,  DestData, Length, DestOffs);

        }
        public ErrorCodes WriteSingleRegister(ushort Address, ushort Data)
        {
            return WriteSingleRegister<ushort>(Address, Data);
        }
        public ErrorCodes WriteMultipleRegisters(ushort Address, ushort[] SrcData, int Length=0, int SrcOffs = 0)
        {
            return WriteMultipleRegisters<ushort>(Address, SrcData,Length, SrcOffs);
        }
        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, ushort[] DestData, int RdLength,
                                                     ushort WrAddress, ushort[] SrcData, int WrLength, int DestOffs = 0, int SrcOffs = 0)
        {
            return ReadWriteMultipleRegisters<ushort>(RdAddress, DestData, RdLength, WrAddress, SrcData, WrLength, DestOffs, SrcOffs);
        }
        #endregion
    }
}
