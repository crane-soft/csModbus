using System.Threading;
using System.Windows.Forms;
using System;
using csModbusLib;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class MasterGridView :  ModbusView
    {
        protected MbMaster MyMaster;
        protected ErrorCodes ErrCode;
        protected static Mutex mut = new Mutex();
        protected ushort[] ModbusData;

        public MasterGridView(ModbusObjectType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, true)
        {
            this.SetDataSize(BaseAddr, NumItems, ItemColumns);
        }

        public virtual void InitGridView(MbMaster Master)
        {
            MyMaster = Master;
            ModbusData = new ushort[this.DataSize];
        }

        public ErrorCodes Update_ModbusData()
        {
            mut.WaitOne();
            ErrCode = Modbus_ReadData();
            mut.ReleaseMutex();
            if (ErrCode == ErrorCodes.NO_ERROR) {
                GridView.DisableCellEvents = true;
                Invoke_UpdateCells();
                GridView.DisableCellEvents = false;
            }

            return ErrCode;
        }
        protected abstract ErrorCodes Modbus_ReadData();

        private delegate void Update_Cells_Callback();
        private void Invoke_UpdateCells()
        {
            if (GridView.InvokeRequired) {
                var d = new Update_Cells_Callback(UpdateCellValues);
                GridView.Invoke(d);
            } else {
                UpdateCellValues();
            }
        }

        private void UpdateCellValues()
        {
            GridView.UpDateModbusCells(ModbusData);
        }

        protected ushort MbDataAddress(DataGridViewCellEventArgs e)
        {
            int DataIdx = e.RowIndex * ItemColumns + e.ColumnIndex;
            return (ushort)(uBaseAddr + DataIdx * TypeSize);
        }


        public ushort[] Data {
            get {
                return ModbusData;
            }
        }
        public void SetValue(int Idx, ushort Value)
        {
            Data[Idx] = Value;
            GridView.UpDateModbusCells(Data, Idx, 1);
        }

    }

    public class MasterHoldingRegsGridView : MasterGridView
    {
        public MasterHoldingRegsGridView() : this(0, 1) {}

        public MasterHoldingRegsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Holding Register", BaseAddr, NumItems, 1) {}

        public MasterHoldingRegsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.HoldingRegister, Title, BaseAddr, NumItems, ItemColumns) {}

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadHoldingRegisters(uBaseAddr, ModbusData);
        }

        protected override void CellValueChanged(ushort[] modData, DataGridViewCellEventArgs e)
        {
            if (MyMaster.IsConnected) {
                ushort Address = MbDataAddress(e);
                ModBusSendRegs(Address, modData);
            }
        }

        private void ModBusSendRegs(ushort Address, ushort[] Data)
        {
            mut.WaitOne();
            // TODO Async Call ?
            if (Data.Length == 1) {
                ErrCode = MyMaster.WriteSingleRegister(Address, Data[0]);
            } else {
                ErrCode = MyMaster.WriteMultipleRegisters(Address, Data);
            }
            mut.ReleaseMutex();
        }
    }

    public class MasterInputRegsGridView : MasterGridView
    {
        public MasterInputRegsGridView() : this(0, 1) {}

        public MasterInputRegsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Input Register", BaseAddr, NumItems, 1) {}

        public MasterInputRegsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.InputRegister, Title, BaseAddr, NumItems, ItemColumns) {}

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadInputRegisters(uBaseAddr,  ModbusData);
        }
    }

    public class MasterCoilsGridView : MasterGridView
    {
        public MasterCoilsGridView() : this(0, 1) {}

        public MasterCoilsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Coils", BaseAddr, NumItems, 1) {}

        public MasterCoilsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.Coils, Title, BaseAddr, NumItems, ItemColumns) {}

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadCoils(uBaseAddr, ModbusData);
        }

        protected override void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            bool CoilStatus = !(bool)CurrentCell.Value;
            CurrentCell.Value = CoilStatus;
            ushort ModbusData = (ushort)(CoilStatus ? 0xff : 0);
            if (MyMaster.IsConnected)
                ModbusSendCoil(MbDataAddress(e), ModbusData);
        }

        private  void ModbusSendCoil(ushort Address, ushort Value)
        {
            // TODO Async Call ?
            mut.WaitOne();
            ErrCode = MyMaster.WriteSingleCoil(Address, Value);
            mut.ReleaseMutex();
        }
    }
    
    public class MasterDiscretInputsGridView : MasterGridView
    {
        public MasterDiscretInputsGridView() : this(0, 1) {}

        public MasterDiscretInputsGridView(ushort BaseAddr, ushort NumItems) 
            : this("DiscretInputs", BaseAddr, NumItems, 1) {}

        public MasterDiscretInputsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.DiscreteInputs, Title, BaseAddr, NumItems, ItemColumns) {}

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadDiscreteInputs(uBaseAddr, ModbusData);
        }
    }
}

