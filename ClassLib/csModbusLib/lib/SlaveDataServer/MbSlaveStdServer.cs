using System;

namespace csModbusLib {
    
    public class StdDataServer : MbSlaveDataServer {

        private ModbusCoilsData gDiscreteInputs;
        private ModbusCoilsData gCoils;
        private ModbusRegsData gInputRegisters;
        private ModbusRegsData gHoldingRegisters;

        public StdDataServer() : this (0) {}

        public StdDataServer(int SlaveID) : base (SlaveID)
        {
            gDiscreteInputs = null;
            gCoils = null;
            gInputRegisters = null;
            gHoldingRegisters = null;
        }

        public void AddCoils(ModbusCoilsData CoilsData)  {
            gCoils = (ModbusCoilsData)AddModbusData(gCoils, CoilsData);
        }

        public void AddCoils(int BaseAddr, ushort[] Coils) {
            gCoils = (ModbusCoilsData)AddModbusData<ModbusCoilsData>(gCoils, BaseAddr, Coils);
        }

        public void AddDiscreteInputs(ModbusCoilsData CoilsData){
            gDiscreteInputs = (ModbusCoilsData)AddModbusData(gDiscreteInputs, CoilsData);
        }

        public void AddDiscreteInputs(int BaseAddr, ushort[] Coils) {
            gDiscreteInputs = (ModbusCoilsData)AddModbusData<ModbusCoilsData>(gDiscreteInputs,BaseAddr, Coils);
        }

        public void AddInputRegisters(ModbusRegsData RegsData){
            gInputRegisters = (ModbusRegsData)AddModbusData(gInputRegisters,RegsData);
        }

        public void AddInputRegisters(int BaseAddr, ushort[] Registers) {
            gInputRegisters = (ModbusRegsData)AddModbusData<ModbusRegsData>(gInputRegisters,BaseAddr, Registers);
        }

        public void AddHoldingRegisters(ModbusRegsData RegsData) {
            gHoldingRegisters = (ModbusRegsData)AddModbusData(gHoldingRegisters,RegsData);
        }

        public void AddHoldingRegisters(int BaseAddr, ushort[] Registers)  {
            gHoldingRegisters = (ModbusRegsData)AddModbusData<ModbusRegsData>(gHoldingRegisters,BaseAddr, Registers);
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


        private ModbusData AddModbusData(ModbusData BaseData, ModbusData AddData)
        {
            if (BaseData == null) {
                BaseData = AddData;
            } else {
                BaseData.AddModbusData(AddData);
            }
            return BaseData;
        }

        private ModbusData AddModbusData<ModbusDataT>(ModbusData BaseData, int BaseAddr, ushort[] Data)
        {
            if (BaseData == null) {
                if (typeof(ModbusDataT) == typeof(ModbusCoilsData)) {
                    BaseData = new ModbusCoilsData();
                } else {
                    BaseData = new ModbusRegsData();
                }
            }
            BaseData.AddData(BaseAddr, Data);
            return BaseData;
        }
    }
}

