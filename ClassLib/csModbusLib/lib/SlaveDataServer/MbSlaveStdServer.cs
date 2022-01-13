using System;

namespace csModbusLib {
    
    public class StdDataServer : MbSlaveDataServer {

        private ModbusDataT<bool> gDiscreteInputs;
        private ModbusDataT<bool> gCoils;
        private ModbusDataT<ushort> gInputRegisters;
        private ModbusDataT<ushort> gHoldingRegisters;

        public StdDataServer() : this (0) {}

        public StdDataServer(int SlaveID) : base (SlaveID)
        {
            gDiscreteInputs = null;
            gCoils = null;
            gInputRegisters = null;
            gHoldingRegisters = null;
        }

        public void AddCoils(ModbusDataT<bool> BoolData)  {
            AddModbusData<bool> (ref gCoils, BoolData);
        }

        public void AddCoils(int BaseAddr, bool[] Coils) {
            AddModbusData<bool>(ref gCoils, BaseAddr, Coils);
        }

        public void AddDiscreteInputs(ModbusDataT<bool> BoolData){
            AddModbusData<bool>(ref gDiscreteInputs, BoolData);
        }

        public void AddDiscreteInputs(int BaseAddr, bool[] Coils) {
            AddModbusData<bool>(ref gDiscreteInputs,BaseAddr, Coils);
        }

        public void AddInputRegisters(ModbusDataT<ushort> RegsData){
            AddModbusData<ushort>(ref gInputRegisters,RegsData);
        }

        public void AddInputRegisters(int BaseAddr, ushort[] Registers) {
            AddModbusData<ushort>(ref gInputRegisters,BaseAddr, Registers);
        }

        public void AddHoldingRegisters(ModbusDataT<ushort> RegsData) {
            AddModbusData<ushort>(ref gHoldingRegisters,RegsData);
        }

        public void AddHoldingRegisters(int BaseAddr, ushort[] Registers)  {
            AddModbusData<ushort>(ref gHoldingRegisters,BaseAddr, Registers);
        }

        protected override bool ReadCoils() {
            return ScannAll4Reading(gCoils);
        }

        protected override bool WriteSingleCoil() {
            return ScannAll4SingleWrite(gCoils);
        }

        protected override bool WriteMultipleCoills() {
            return ScannAll4Writing(gCoils);
        }

        protected override bool ReadDiscreteInputs() {
            return ScannAll4Reading(gDiscreteInputs);
        }

        protected override bool ReadInputRegisters() {
            return ScannAll4Reading(gInputRegisters);
        }

        protected override bool ReadHoldingRegisters()  {
            return ScannAll4Reading(gHoldingRegisters);
        }

        protected override bool WriteSingleRegister()  {
            return ScannAll4SingleWrite(gHoldingRegisters);
        }

        protected override bool WriteMultipleRegisters() {
            return ScannAll4Writing(gHoldingRegisters);
        }

        private bool ScannAll4Reading(ModbusData Data)
        {
            if (Data == null)
                return false;
            return Data.ScannAll4Reading(Frame);
        }

        private bool ScannAll4SingleWrite(ModbusData Data)
        {
            if (Data == null)
                return false;
            return Data.ScannAll4SingleWrite(Frame);
        }

        private bool ScannAll4Writing(ModbusData Data)
        {
            if (Data == null)
                return false;
            return Data.ScannAll4Writing(Frame);
        }

        private void AddModbusData<DataT>(ref ModbusDataT<DataT> BaseData, ModbusDataT<DataT> AddData)
        {
            if (BaseData == null) {
                BaseData = AddData;
            } else {
                BaseData.AddModbusData(AddData);
            }
        }

        private void AddModbusData<DataT>(ref ModbusDataT<DataT> BaseData, int BaseAddr, DataT[] ModbusData)
        {
            if (BaseData == null) {
                BaseData = new ModbusDataT<DataT>();
            }
            BaseData.AddData(BaseAddr, ModbusData);
        }
    }
}

