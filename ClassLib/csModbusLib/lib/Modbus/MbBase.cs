using System;

namespace csModbusLib
{
    #region ModbusException
    public enum ErrorCodes
    {
        NO_ERROR = 0,
        CONNECTION_ERROR,
        RX_TIMEOUT,
        TX_TIMEOUT,
        MESSAGE_INCOMPLETE,
        WRONG_CRC,
        WRONG_IDENTIFIER,
        ILLEGAL_FUNCTION_CODE,
        CONNECTION_CLOSED,
        MODBUS_EXCEPTION,
    }

    public class ModbusException : Exception
    {
        public ErrorCodes ErrorCode;
        public ModbusException(ErrorCodes errCode)
        {
            ErrorCode = errCode;
        }
    }
    #endregion

    #region Enumerations

    public enum ConnectionType
    {
        NO_CONNECTION = 0,
        SERIAL_RTU = 1,
        SERIAL_ASCII = 2,
        TCP_IP = 3,
        UDP_IP = 4
    }

    public enum DeviceType
    {
        NO_TYPE = 0,
        MASTER = 1,
        SLAVE = 2
    }
    
    /// <summary>
    /// Modbus calling codes
    /// </summary>
    public enum ModbusCodes
    {
        READ_COILS = 0x01,
        READ_DISCRETE_INPUTS = 0x02,
        READ_HOLDING_REGISTERS = 0x03,
        READ_INPUT_REGISTERS = 0x04,
        WRITE_SINGLE_COIL = 0x05,
        WRITE_SINGLE_REGISTER = 0x06,
     // READ_EXCEPTION_STATUS = 0x07,       // serial Line only
     // DIAGNOSTIC = 0x08,                  // serial Line only
     // GET_COM_EVENT_COUNTER = 0x0B,       // serial Line only
     // GET_COM_EVENT_LOG = 0x0C,           // serial Line only
        WRITE_MULTIPLE_COILS = 0x0F,
        WRITE_MULTIPLE_REGISTERS = 0x10,
     // REPORT_SLAVE_ID = 0x11,             // serial Line only
     // READ_FILE_RECORD = 0x14,
     // WRITE_FILE_RECORD = 0x15,
        MASK_WRITE_REGISTER = 0x16,         // TODO
        READ_WRITE_MULTIPLE_REGISTERS = 0x17,     
        //READ_FIFO_QUEUE = 0x18,
        //READ_DEVICE_IDENTIFICATION = 0x2B
    }

    /// <summary>
    /// Exception Codes
    /// </summary>
    public enum ExceptionCodes 
    {
        NO_EXCEPTION = 0,
        ILLEGAL_FUNCTION = 1,       // The function code received in the query is not an allowable action for the slave.
                                    // If a Poll Program Complete command was issued, this code indicates that no program function preceded it.
        ILLEGAL_DATA_ADDRESS = 2,   // The data address received in the query is not an allowable address for the slave.
        ILLEGAL_DATA_VALUE = 3,     // A value contained in the query data field is not an allowable value for the slave.
        SLAVE_DEVICE_FAILURE = 4,   // An unrecoverable error occurred while the slave was attempting to perform the requested action.
        ACKNOWLEDGE = 5,            // The slave has accepted the request and is processing it, but a long duration of time will be required to do so.  
                                    // This response is returned to prevent a timeout error from occurring in the master.  
                                    // The master can next issue a Poll Program Complete message to determine if processing is completed.
        SLAVE_DEVICE_BUSY = 6,      // The slave is engaged in processing a long–duration program command.  The master should retransmit 
                                    // the message later when the slave is free.
        NEGATIVE_ACKNOWLEDGE = 7,   // The slave cannot perform the program function received in the query.  
                                    // This code is returned for an unsuccessful programming request using function code 13 or 14 decimal.  
                                    // The master should request diagnostic or error information from the slave.
        MEMORY_PARITY_ERROR = 8,    // The slave attempted to read extended memory, but detected a parity error in the memory.  
                                    // The master can retry the request, but service may be required on the slave device. 
    }

    #endregion

   #region Modbus Base abstract class
  
    public abstract class MbBase
    {
        public const ushort MAX_FRAME_LEN = 256 + 6;
        
        public MbBase() { }

        protected void InitInterface(MbInterface Interface)
        {   gInterface = Interface;
        }

        protected MbInterface gInterface = null;
        protected bool running = false;
        protected ConnectionType connection_type = ConnectionType.NO_CONNECTION;
    }

    #endregion
}
