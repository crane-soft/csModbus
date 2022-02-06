using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace csModbusLib
{
    public class MbMasterBase : MbBase
    {
        private MBMFrame Frame = new MBMFrame();
        private byte Current_SlaveID = 0;
        protected ErrorCodes LastError = ErrorCodes.NO_ERROR;
        #region Constructors

        public MbMasterBase()  { }

        public MbMasterBase(MbInterface Interface)  {
            InitInterface(Interface);
        }

        #endregion

        #region Properties
        public bool IsConnected {
            get {
                return running;
            }
        }

        public byte Slave_ID {
            get {
                return Current_SlaveID;
            }
            set {
                Current_SlaveID = value;
                Frame.Slave_ID = value;
            }
        }

        public ErrorCodes ErrorCode {
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
                if (gInterface.Connect()) {
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

        protected bool SendSingleRequest(ModbusCodes Fcode, ushort Address, ushort DataOrLen)
        {
            int MsgLen = Frame.BuildRequest(Fcode, Address, DataOrLen);
            return SendRequestMessage(MsgLen);
        }

        protected bool SendMultipleWriteRequest(ModbusCodes Fcode, ushort Address, ushort Length, object SrcData, int SrcOffs)
        {
            int MsgLen = Frame.BuildMultipleWriteRequest(Fcode, Address, Length, SrcData, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        protected bool SendMultipleReadWriteRequest(ushort RdAddress, ushort RdLength, ushort WrAddress, ushort WrLength, ushort[] SrcData, int SrcOffs)
        {
            int MsgLen = Frame.BuildMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, WrLength, SrcData, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        private bool SendRequestMessage(int MsgLen)
        {   // TODO check if connected
            LastError = ErrorCodes.NO_ERROR;
            try {
                gInterface.SendFrame(Frame.RawData, MsgLen);
            } catch (ModbusException ex) {
                LastError = ex.ErrorCode;
                //if (running) {
                //    gInterface.ReConnect();
                //}
                return false;
            } catch (Exception ex) {
                LastError = csModbusLib.ErrorCodes.CONNECTION_ERROR;
                Debug.Print(ex.Message);
                return false;
            }
            return true;
        }

        protected bool ReadSlaveBitValues(bool[] DestArray, int DestOffs)
        {
            if (ReceiveSlaveResponse()) {
                Frame.ReadSlaveBitValues(DestArray, DestOffs);
                return true;
            }
            return false;
        }

        protected bool ReadSlaveRegisterValues(UInt16[] DestArray, int DestOffs)
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
                gInterface.ReceiveHeader(MbInterface.ResponseTimeout, Frame.RawData);
                Frame.ReceiveSlaveResponse(gInterface);
            } catch (ModbusException ex) {
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

        public ErrorCodes ReadCoils(ushort Address, ushort Length, bool[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_COILS, Address, Length))
                ReadSlaveBitValues(DestData, DestOffs);
            return LastError;
        }

        public ErrorCodes ReadDiscreteInputs(ushort Address, ushort Length, bool[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_DISCRETE_INPUTS, Address, Length))
                ReadSlaveBitValues(DestData, DestOffs);
            return LastError;
        }

        public ErrorCodes ReadHoldingRegisters(ushort Address, ushort Length, ushort[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_HOLDING_REGISTERS, Address, Length))
                ReadSlaveRegisterValues(DestData, DestOffs);
            return LastError;
        }

        public ErrorCodes ReadInputRegisters(ushort Address, ushort Length, ushort[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_INPUT_REGISTERS, Address, Length))
                ReadSlaveRegisterValues(DestData, DestOffs);
            return LastError;
        }

        public ErrorCodes WriteSingleCoil(ushort Address, bool BitData)
        {
            UInt16 Data = (UInt16)(BitData ? 0xff00 : 0);
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_COIL, Address, Data))
                ReceiveSlaveResponse();
            return LastError;
        }

        public ErrorCodes WriteSingleRegister(ushort Address, ushort Data)
        {
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_REGISTER, Address, Data))
                ReceiveSlaveResponse();
            return LastError;
        }

        public ErrorCodes WriteMultipleCoils(ushort Address, ushort Length, bool[] SrcData, int SrcOffs = 0)
        {   // TODO not yet tested
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_COILS, Address, Length, SrcData, SrcOffs))
                ReceiveSlaveResponse();
            return LastError;
        }

        public ErrorCodes WriteMultipleRegisters(ushort Address, ushort Length, ushort[] SrcData, int SrcOffs = 0)
        {  // TODO not yet tested
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_REGISTERS, Address, Length, SrcData, SrcOffs))
                ReceiveSlaveResponse();

            return LastError;
        }

        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, ushort RdLength, ushort[] DestData,
                                                     ushort WrAddress, ushort WrLength, ushort[] SrcData, int DestOffs = 0, int SrcOffs = 0)
        {  
            if (SendMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, WrLength, SrcData, SrcOffs))
                ReadSlaveRegisterValues(DestData, DestOffs);
            return LastError;
        }
    }

    public class MbMasterAsync : MbMasterBase
    {
        // https://www.dotnetperls.com/async
        // http://gigi.nullneuron.net/gigilabs/working-with-asynchronous-methods-in-c/
        // https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/concepts/async/

        public MbMasterAsync() {}
        public MbMasterAsync(MbInterface Interface) : base(Interface) {}

        public async Task<ErrorCodes> ReadCoils(ushort Address, ushort Length, bool[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_COILS, Address, Length)) {
                await Task.Run(() => ReadSlaveBitValues(DestData, DestOffs));
            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadDiscreteInputs(ushort Address, ushort Length, bool[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_DISCRETE_INPUTS, Address, Length)) {
                await Task.Run(() => ReadSlaveBitValues(DestData, DestOffs));

            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadHoldingRegisters(ushort Address, ushort Length, ushort[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_HOLDING_REGISTERS, Address, Length)) {
                await Task.Run(() => ReadSlaveRegisterValues(DestData, DestOffs));
            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadInputRegisters(ushort Address, ushort Length, ushort[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_INPUT_REGISTERS, Address, Length)) {
                await Task.Run(() => ReadSlaveRegisterValues(DestData, DestOffs));
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteSingleCoil(ushort Address, bool BitData)
        {
            UInt16 Data = (UInt16)(BitData ? 1 : 0);
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_COIL, Address, Data)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteSingleRegister(ushort Address, ushort Data)
        {
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_REGISTER, Address, Data)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteMultipleCoils(ushort Address, ushort Length, bool[] SrcData, int SrcOffs = 0)
        {
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_COILS, Address, Length, SrcData, SrcOffs)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteMultipleRegisters(ushort Address, ushort Length, ushort[] SrcData, int SrcOffs = 0)
        {
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_REGISTERS, Address, Length, SrcData, SrcOffs)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadWriteMultipleRegisters(ushort RdAddress, ushort RdLength, ushort[] DestData,
                                                     ushort WrAddress, ushort WrLength, ushort[] SrcData, int DestOffs = 0, int SrcOffs = 0)
        {
            if (SendMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, WrLength, SrcData, SrcOffs))
                await Task.Run(() => ReadSlaveRegisterValues(DestData, DestOffs));
            return LastError;
        }
    }
}


