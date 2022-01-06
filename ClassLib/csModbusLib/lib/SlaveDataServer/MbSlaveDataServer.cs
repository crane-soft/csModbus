using System;

namespace csModbusLib {

    public class MbSlaveDataServer {

        public MbSlaveDataServer NextDataServer;   
        private int gSlaveID;
        protected MBSFrame Frame;

        public MbSlaveDataServer()
        {
            gSlaveID = 0;
            NextDataServer = null;
        }

        public MbSlaveDataServer(int SlaveID)
        {
            gSlaveID = SlaveID;
        }

        public void Add (MbSlaveDataServer NewServer)
        {
            NextDataServer = NewServer;
        }

        public int SlaveID
        {
            get { return gSlaveID; }
            set { gSlaveID = value; }
        }

        public virtual void DataServices(MBSFrame aFrame)
        {
            Frame = aFrame;
            if (Frame.SlaveId == gSlaveID) {
                switch (Frame.FunctionCode) {
                    case ModbusCodes.READ_COILS:
                        if (ReadCoils()) return;
                        break;
                    case ModbusCodes.READ_DISCRETE_INPUTS:
                        if (ReadDiscreteInputs()) return;
                        break;
                    case ModbusCodes.READ_HOLDING_REGISTERS:
                        if (ReadHoldingRegisters()) return;
                        break;
                    case ModbusCodes.READ_INPUT_REGISTERS:
                        if (ReadInputRegisters()) return;
                        break;
                    case ModbusCodes.WRITE_SINGLE_COIL:
                        if (WriteSingleCoil())
                            return;
                        break;
                    case ModbusCodes.WRITE_SINGLE_REGISTER:
                        if (WriteSingleRegister()) return;
                        break;
                    case ModbusCodes.WRITE_MULTIPLE_COILS:
                        if (WriteMultipleCoills()) return;
                        break;
                    case ModbusCodes.WRITE_MULTIPLE_REGISTERS:
                        if (WriteMultipleRegisters()) return;
                        break;
                    case ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS:
                        if (WriteMultipleRegisters()) {
                            Frame.GetReadDataAddress();
                            if (ReadHoldingRegisters()) {
                                return;
                            }
                        }
                        break;

                }
                Frame.ExceptionCode = ExceptionCodes.ILLEGAL_FUNCTION;
            }
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
