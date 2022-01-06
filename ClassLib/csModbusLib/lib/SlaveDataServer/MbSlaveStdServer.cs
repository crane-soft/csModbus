using System;

namespace csModbusLib {
    
    public class StdDataServer : MbSlaveDataServer {

        private const int MAX_ELEMENTS = 32000;
        private ModbusDataT<bool> gDiscreteInputs;
        private ModbusDataT<bool> gCoils;
        private ModbusDataT<ushort> gInputRegisters;
        private ModbusDataT<ushort> gHoldingRegisters;

        public StdDataServer()
        {
            gDiscreteInputs =  null;
            gCoils = null;
            gInputRegisters = null;
            gHoldingRegisters = null;
        }

        private void AddBools (ref ModbusDataT<bool> BaseData, ModbusDataT<bool> AddData)
        {
            if (BaseData == null) {
                BaseData = AddData;
            } else {
                BaseData.AddModbusData(AddData);
            }
        }

        private void AddBools(ref ModbusDataT<bool> BaseData, int BaseAddr, bool[] Coils)
        {
            if (BaseData == null) {
                BaseData = new ModbusDataT<bool>();
            }
            BaseData.AddData(BaseAddr, Coils);
        }

        public void AddRegisters(ref ModbusDataT<ushort> BaseData, ModbusDataT<ushort> AddData)
        {
            if (BaseData == null) {
                BaseData = AddData;
            } else {
                BaseData.AddModbusData(AddData);
            }
        }

        public void AddRegisters(ref ModbusDataT<ushort> BaseData, int BaseAddr, ushort[] Registers)
        {
            if (BaseData == null) {
                BaseData = new ModbusDataT<ushort>();
            }
            BaseData.AddData(BaseAddr, Registers);
        }


        public void AddCoils(ModbusDataT<bool> BoolData)   
        {   AddBools(ref gCoils, BoolData);
        }

        public void AddCoils(int BaseAddr, bool[] Coils)
        {   AddBools(ref gCoils, BaseAddr, Coils);
        }

        public void AddDiscreteInputs(ModbusDataT<bool> BoolData)
        {   AddBools(ref gDiscreteInputs, BoolData);
        }

        public void AddDiscreteInputs(int BaseAddr, bool[] Coils)
        {   AddBools(ref gDiscreteInputs,BaseAddr, Coils);
        }

        public void AddInputRegisters(ModbusDataT<ushort> RegsData)
        {   AddRegisters(ref gInputRegisters,RegsData);
        }

        public void AddInputRegisters(int BaseAddr, ushort[] Registers)
        {   AddRegisters(ref gInputRegisters,BaseAddr, Registers);
        }

        public void AddHoldingRegisters(ModbusDataT<ushort> RegsData)
        {   AddRegisters(ref gHoldingRegisters,RegsData);
        }

        public void AddHoldingRegisters(int BaseAddr, ushort[] Registers)
        {   AddRegisters(ref gHoldingRegisters,BaseAddr, Registers);
        }

        protected override bool ReadCoils()
        {
            if (gCoils == null)
                return false;
            gCoils.ScannAll4Reading(Frame);
            return true;
        }

        protected override bool WriteSingleCoil()
        {
            if (gCoils == null)
                return false;
            gCoils.ScannAll4SingleWrite(Frame);
            return true;
        }

        protected override bool WriteMultipleCoills()
        {
            if (gCoils == null)
                return false;
            gCoils.ScannAll4Writing(Frame);
            return true;
        }

        protected override bool ReadDiscreteInputs()
        {
            if (gDiscreteInputs == null)
                return false;
            gDiscreteInputs.ScannAll4Reading(Frame);
            return true;
        }

        protected override bool ReadInputRegisters()
        {
            if (gInputRegisters == null)
                return false;
            gInputRegisters.ScannAll4Reading(Frame);
            return true;
        }

        protected override bool ReadHoldingRegisters()
        {
            if (gHoldingRegisters == null)
                return false;
            gHoldingRegisters.ScannAll4Reading(Frame);
            return true;
        }

        protected override bool WriteSingleRegister()
        {
            if (gHoldingRegisters == null)
                return false;
            gHoldingRegisters.ScannAll4SingleWrite(Frame);
            return true;
        }

        protected override bool WriteMultipleRegisters()
        {
            if (gHoldingRegisters == null)
                return false;
            gHoldingRegisters.ScannAll4Writing(Frame);
            return true;
        }
    }
}

