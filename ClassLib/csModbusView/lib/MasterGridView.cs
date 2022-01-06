using System.Threading;
using System.Windows.Forms;
using System;
using csModbusLib;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class MasterGridView : ModbusView
    {
        protected MbMaster MyMaster;
        protected ErrorCodes ErrCode;
        protected static Mutex mut = new Mutex();

        public MasterGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, true)
        {
            this.SetDataSize(BaseAddr, NumItems, ItemColumns);
        }

        public virtual void InitGridView(MbMaster Master)
        {
            MyMaster = Master;
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

        public delegate void Update_Cells_Callback();
        protected void Invoke_UpdateCells()
        {
            if (GridView.InvokeRequired) {
                var d = new Update_Cells_Callback(UpdateCellValues);
                GridView.Invoke(d);
            } else {
                UpdateCellValues();
            }
        }

        protected ushort MbDataAddress(DataGridViewCellEventArgs e)
        {
            int DataIdx = e.RowIndex * ItemColumns + e.ColumnIndex;
            return (ushort)(BaseAddr + DataIdx);
        }

        protected abstract ErrorCodes Modbus_ReadData();
        protected abstract void UpdateCellValues();

    }

    public abstract class MasterGridViewDataT<DataT> : MasterGridView
    {
        protected DataT[] MbRegsData;

        public MasterGridViewDataT(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns)
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public override void InitGridView(MbMaster Master)
        {
            base.InitGridView(Master);
            MbRegsData = new DataT[NumItems];
        }

        protected override void UpdateCellValues()
        {
            GridView.UpDateModbusCells(MbRegsData);
        }
    }

    public class MasterHoldingRegsGridView : MasterGridViewDataT<ushort>
    {
        public MasterHoldingRegsGridView() 
            : this(0, 1)
        {
        }

        public MasterHoldingRegsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Holding Register", BaseAddr, NumItems, 1)
        {
        }

        public MasterHoldingRegsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusDataType.HoldingRegister, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadHoldingRegisters(BaseAddr, NumItems, MbRegsData, 0);
        }

        protected override void CellValueChanged(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            ModBusSendRegs(MbDataAddress(e), Convert.ToUInt16(CurrentCell.Value));
        }

        private void ModBusSendRegs(ushort Address, ushort Value)
        {
            mut.WaitOne();
            // TODO Async Call ?
            ErrCode = MyMaster.WriteSingleRegister(Address, Value);
            mut.ReleaseMutex();
        }
    }

    public class MasterInputRegsGridView : MasterGridViewDataT<ushort>
    {
        public MasterInputRegsGridView() : this(0, 1)
        {
        }

        public MasterInputRegsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Input Register", BaseAddr, NumItems, 1)
        {
        }

        public MasterInputRegsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusDataType.InputRegister, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadInputRegisters(BaseAddr, NumItems, MbRegsData, 0);
        }
    }

    public class MasterCoilsGridView : MasterGridViewDataT<bool>
    {
        public MasterCoilsGridView() 
            : this(0, 8)
        {
        }

        public MasterCoilsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Coils", BaseAddr, NumItems, 8)
        {
        }

        public MasterCoilsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusDataType.Coils, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadCoils(BaseAddr, NumItems, MbRegsData, 0);
        }

        protected override void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            bool NewCellValue = !(bool)CurrentCell.Value;
            CurrentCell.Value = NewCellValue;
            ModbusSendCoils(MbDataAddress(e), NewCellValue);
        }

        private  void ModbusSendCoils(ushort Address, bool Value)
        {
            // TODO Async Call ?
            mut.WaitOne();
            ErrCode = MyMaster.WriteSingleCoil(Address, Value);
            mut.ReleaseMutex();
        }
    }

    public class MasterDiscretInputsGridView : MasterGridViewDataT<bool>
    {
        public MasterDiscretInputsGridView() : this(0, 8)
        {
        }

        public MasterDiscretInputsGridView(ushort BaseAddr, ushort NumItems) 
            : this("DiscretInputs", BaseAddr, NumItems, 8)
        {
        }

        public MasterDiscretInputsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusDataType.DiscreteInputs, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        protected override ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadDiscreteInputs(BaseAddr, NumItems, MbRegsData, 0);
        }
    }
}

