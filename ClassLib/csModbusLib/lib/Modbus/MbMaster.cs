using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace csModbusLib {
    public class MbMaster : MbBase {

        private MBMFrame Frame = new MBMFrame();
        private byte Current_SlaveID;
        private Stopwatch timeoutTmer = new Stopwatch();
        private ErrorCodes LastError;
        #region Constructors
        public MbMaster()
        {
        }
      
        public MbMaster(MbInterface Interface)
        {
            gInterface = Interface;
        }

        #endregion

        #region Properties
        public bool IsConnected
        {
            get
            {
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

        public ushort GetTransactionIdentifier ()
        {
            if (Frame != null)
                return Frame.GetTransactionIdentifier();
            return 0;
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

        public Boolean Connect (MbInterface Interface, byte newSlaveID)
        {
            gInterface = Interface;
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

        // TODO: different classes for sync and assync
        #region Public Synchron Functions
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

        public ErrorCodes WriteSingleRegister (ushort Address, ushort Data)
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

        public ErrorCodes WriteMultipleRegisters (ushort Address, ushort Length, ushort[] SrcData, int SrcOffs = 0)
        {  // TODO not yet tested
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_REGISTERS, Address, Length, SrcData, SrcOffs))
                ReceiveSlaveResponse();

            return LastError;
        }

        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, ushort RdLength, ushort[] DestData,  
                                                     ushort WrAddress, ushort WrLength, ushort[] SrcData,  int DestOffs = 0 , int SrcOffs = 0)
        {  // TODO not yet tested
            if (SendMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, WrLength, SrcData, SrcOffs))
                ReadSlaveRegisterValues(DestData, DestOffs);
            return LastError;
        }

        #endregion
       
        #region Public Async Functions
        // https://www.dotnetperls.com/async
        // http://gigi.nullneuron.net/gigilabs/working-with-asynchronous-methods-in-c/
        // https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/concepts/async/

        public async Task<ErrorCodes> ReadCoilsAsync(ushort Address, ushort Length, bool[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_COILS, Address, Length)) {
                await Task.Run(() => ReadSlaveBitValues(DestData, DestOffs));
            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadDiscreteInputsAsync(ushort Address, ushort Length, bool[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_DISCRETE_INPUTS, Address, Length)) {
                await Task.Run(() => ReadSlaveBitValues(DestData, DestOffs));

            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadHoldingRegistersAsync(ushort Address, ushort Length, ushort[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_HOLDING_REGISTERS, Address, Length)) {
                await Task.Run(() => ReadSlaveRegisterValues(DestData, DestOffs));
            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadInputRegistersAsync(ushort Address, ushort Length, ushort[] DestData, int DestOffs = 0)
        {
            if (SendSingleRequest(ModbusCodes.READ_INPUT_REGISTERS, Address, Length)) {
                await Task.Run(() => ReadSlaveRegisterValues(DestData, DestOffs));
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteSingleCoilAsync(ushort Address, bool BitData)
        {
            UInt16 Data = (UInt16)(BitData ? 1 : 0);
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_COIL, Address, Data)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteSingleRegisterAsync(ushort Address, ushort Data)
        {
            if (SendSingleRequest(ModbusCodes.WRITE_SINGLE_REGISTER, Address, Data)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteMultipleCoilsAsync(ushort Address, ushort Length, bool[] SrcData, int SrcOffs = 0)
        {
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_COILS, Address, Length, SrcData, SrcOffs)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> WriteMultipleRegistersAsync(ushort Address, ushort Length, ushort[] SrcData, int SrcOffs = 0)
        {
            if (SendMultipleWriteRequest(ModbusCodes.WRITE_MULTIPLE_REGISTERS, Address, Length, SrcData, SrcOffs)) {
                await Task.Run(() => ReceiveSlaveResponse());
            }
            return LastError;
        }

        public async Task<ErrorCodes> ReadWriteMultipleRegistersAsync(ushort RdAddress, ushort RdLength, ushort[] DestData,
                                                     ushort WrAddress, ushort WrLength, ushort[] SrcData, int DestOffs = 0, int SrcOffs = 0)
        {
            if (SendMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, WrLength, SrcData, SrcOffs))
                await Task.Run(() => ReadSlaveRegisterValues(DestData, DestOffs));
            return LastError;
        }

        #endregion

        #region Private Functions

        private bool SendSingleRequest(ModbusCodes Fcode, ushort Address, ushort DataOrLen)
        {
            int MsgLen = Frame.BuildRequest(Fcode, Address, DataOrLen);
            return SendRequestMessage(MsgLen);
        }

        private bool SendMultipleWriteRequest(ModbusCodes Fcode, ushort Address, ushort Length, object SrcData, int SrcOffs)
        {
            int MsgLen = Frame.BuildMultipleWriteRequest(Fcode, Address, Length, SrcData, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        private bool SendMultipleReadWriteRequest(ushort RdAddress, ushort RdLength, ushort WrAddress, ushort WrLength, ushort[] SrcData, int SrcOffs)
        {
            int MsgLen = Frame.BuildMultipleReadWriteRequest(RdAddress, RdLength, WrAddress, WrLength, SrcData, SrcOffs);
            return SendRequestMessage(MsgLen);
        }

        private bool SendRequestMessage(int MsgLen)
        {   // TODO check if connected
            LastError = ErrorCodes.NO_ERROR;
            try {
                gInterface.SendFrame(Frame.RawData, MsgLen);
            }
            catch (ModbusException ex) {
                LastError = ex.ErrorCode;
                if (running) {
                    gInterface.ReConnect();
                }

                return false;
            } catch (Exception ) {
                // TODO eval error
                return false;

            }
            return true;
        }

        private bool ReadSlaveBitValues(bool[] DestArray, int DestOffs)
        {
            if (ReceiveSlaveResponse()) {
                Frame.ReadSlaveBitValues(DestArray, DestOffs);
                return true;
            }
            return false;
        }

        private bool ReadSlaveRegisterValues(UInt16[] DestArray, int DestOffs)
        {
            if (ReceiveSlaveResponse()) {
                Frame.ReadSlaveRegisterValues(DestArray, DestOffs);
                return true;
            }
            return false;
        }

        private bool ReceiveSlaveResponse()
        {
            try {
                ReceiveSlaveResponseWithTimeout();
            }
            catch (ModbusException ex) {
                if (ex.ErrorCode != ErrorCodes.CONNECTION_CLOSED)
                    gInterface.ReConnect();
                LastError = ex.ErrorCode;
                return false;
            }
            return true;
        }
  
        private void ReceiveSlaveResponseWithTimeout()
        {  
            timeoutTmer.Restart();

            while (running) {
                if (timeoutTmer.ElapsedMilliseconds > 500) {
                    throw new ModbusException(csModbusLib.ErrorCodes.RX_TIMEOUT);
                }
                if (gInterface.ReceiveHeader(Frame.RawData)) {
                    Frame.ReceiveSlaveResponse(gInterface);
                    return;
                 }
                Thread.Sleep(1);
            }
            throw new ModbusException(csModbusLib.ErrorCodes.CONNECTION_CLOSED);
        }
        #endregion
    }
}


