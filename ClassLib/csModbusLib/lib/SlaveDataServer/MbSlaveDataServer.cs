using System;

namespace csModbusLib {

    public class MbSlaveDataServer {

        public MbSlaveDataServer NextDataServer;   
        private int gSlaveID;
        protected MBSFrame Frame;

        public MbSlaveDataServer() : this (0) {}

        public MbSlaveDataServer(int SlaveID)
        {
            gSlaveID = SlaveID;
            NextDataServer = null;
        }

        public void Add (MbSlaveDataServer NewServer) {
            NextDataServer = NewServer;
        }

        public int SlaveID
        {
            get { return gSlaveID; }
            set { gSlaveID = value; }
        }

        public bool DataServices(MBSFrame aFrame)
        {
            Frame = aFrame;
            if (Frame.SlaveId != gSlaveID) {
                return false;
            }

            switch (Frame.FunctionCode) {
                case ModbusCodes.READ_COILS:
                    if (ReadCoils()) return true; ;
                    break;
                case ModbusCodes.READ_DISCRETE_INPUTS:
                    if (ReadDiscreteInputs()) return true;
                    break;
                case ModbusCodes.READ_HOLDING_REGISTERS:
                    if (ReadHoldingRegisters()) return true;
                    break;
                case ModbusCodes.READ_INPUT_REGISTERS:
                    if (ReadInputRegisters()) return true; ;
                    break;
                case ModbusCodes.WRITE_SINGLE_COIL:
                    if (WriteSingleCoil()) return true;
                    break;
                case ModbusCodes.WRITE_SINGLE_REGISTER:
                    if (WriteSingleRegister()) return true;
                    break;
                case ModbusCodes.WRITE_MULTIPLE_COILS:
                    if (WriteMultipleCoills()) return true;
                    break;
                case ModbusCodes.WRITE_MULTIPLE_REGISTERS:
                    if (WriteMultipleRegisters()) return true;
                    break;
                case ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS:
                    Frame.SaveWritaData();
                    if (ReadHoldingRegisters()) {
                        Frame.GetRwWriteAddress();
                        if (WriteMultipleRegisters()) {
                            return true;
                        }
                    }
                    break;
                default:
                    Frame.ExceptionCode = ExceptionCodes.ILLEGAL_FUNCTION;
                    return true;

            }
            Frame.ExceptionCode = ExceptionCodes.ILLEGAL_DATA_ADDRESS;
            return true;
        }

        protected virtual bool ReadCoils() { return false; }
        protected virtual bool ReadDiscreteInputs() { return false; }
        protected virtual bool ReadHoldingRegisters() { return false; }
        protected virtual bool ReadInputRegisters() { return false; }
        protected virtual bool WriteSingleCoil() { return false; }
        protected virtual bool WriteSingleRegister() { return false; }
        protected virtual bool WriteMultipleCoills() { return false; }
        protected virtual bool WriteMultipleRegisters() { return false; }
    }
}
