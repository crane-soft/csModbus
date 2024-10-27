using System;

namespace csModbusLib {
    
    public class StdDataServer : MbSlaveDataServer {

        private ModbusData gDiscreteInputs;
        private ModbusData gCoils;
        private ModbusData gInputRegisters;
        private ModbusData gHoldingRegisters;

        public StdDataServer() : this (0) {}

        public StdDataServer(int SlaveID) : base (SlaveID)
        {
            gDiscreteInputs = null;
            gCoils = null;
            gInputRegisters = null;
            gHoldingRegisters = null;
        }

        #region Add Coils
        public void AddCoils(ModbusCoilsData CoilsData)  {
            gCoils = AddModbusData(gCoils, CoilsData);
        }

        public void AddCoils(int BaseAddr, ushort[] Coils) {
            gCoils = AddModbusData(gCoils, new ModbusCoilsData(BaseAddr, Coils));
        }

        public void AddDiscreteInputs(ModbusCoilsData CoilsData){
            gDiscreteInputs = AddModbusData(gDiscreteInputs, CoilsData);
        }

        public void AddDiscreteInputs(int BaseAddr, ushort[] Coils) {
            gDiscreteInputs = AddModbusData(gDiscreteInputs, new ModbusCoilsData(BaseAddr, Coils));
        }
        #endregion

        #region Add InputRegister <RegsData, ushort, short, UInt32 Int32>
        public void AddInputRegisters(ModbusRegsData RegsData){
            gInputRegisters = AddModbusData(gInputRegisters,RegsData);
        }
        public void AddInputRegisters(int BaseAddr, UInt16[] Registers) {
            gInputRegisters = AddModbusData(gInputRegisters, new ModbusRegsData (BaseAddr, Registers));
        }
        public void AddInputRegisters(int BaseAddr, Int16[] Registers)
        {
            gInputRegisters = AddModbusData(gInputRegisters, new ModbusRegsData(BaseAddr, (UInt16[])(object)Registers));
        }
        public void AddInputRegisters(int BaseAddr, UInt32[] Registers)  {
            gInputRegisters = AddModbusData(gInputRegisters, new Modbusb32Data<UInt32>(BaseAddr, Registers));
        }
        public void AddInputRegisters(int BaseAddr, Int32[] Registers)
        {
            gInputRegisters = AddModbusData(gInputRegisters, new Modbusb32Data<Int32>(BaseAddr, Registers));
        }
        public void AddInputRegisters(int BaseAddr, float[] Registers)
        {
            gInputRegisters = AddModbusData(gInputRegisters, new Modbusb32Data<float>(BaseAddr, Registers));
        }
        #endregion

        #region Add Holding Register  <RegsData, ushort, short, UInt32 Int32>
        public void AddHoldingRegisters(ModbusRegsData RegsData) {
            gHoldingRegisters = AddModbusData(gHoldingRegisters,RegsData);
        }
        public void AddHoldingRegisters(int BaseAddr, UInt16[] Registers)  {
            gHoldingRegisters = AddModbusData(gHoldingRegisters, new ModbusRegsData(BaseAddr, Registers));
        }
        public void AddHoldingRegisters(int BaseAddr, Int16[] Registers)
        {
            gHoldingRegisters = AddModbusData(gHoldingRegisters, new ModbusRegsData(BaseAddr, (UInt16[])(object)Registers));
        }
        public void AddHoldingRegisters(int BaseAddr, UInt32[] Registers) {
            gHoldingRegisters = AddModbusData(gHoldingRegisters, new Modbusb32Data<UInt32>(BaseAddr, Registers));
        }
        public void AddHoldingRegisters(int BaseAddr, Int32[] Registers)
        {
            gHoldingRegisters = AddModbusData(gHoldingRegisters, new Modbusb32Data<Int32>(BaseAddr, Registers));
        }
        public void AddHoldingRegisters(int BaseAddr, float[] Registers)
        {
            gHoldingRegisters = AddModbusData(gHoldingRegisters, new Modbusb32Data<float>(BaseAddr, Registers));
        }
        #endregion

        #region Dataservice
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


        private ModbusData AddModbusData( ModbusData  BaseData, ModbusData AddData)
        {
            if (BaseData == null) {
                BaseData = AddData;
            } else {
                BaseData.AddModbusData(AddData);
            }
            return BaseData;
        }
        #endregion
    }
}
