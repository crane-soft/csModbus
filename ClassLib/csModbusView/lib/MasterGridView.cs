using System.Threading;
using System.Windows.Forms;
using csModbusLib;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class MasterGridView : ModbusView
    {
        protected MbMaster MyMaster;
        protected csModbusLib.ErrorCodes ErrCode;
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

        public csModbusLib.ErrorCodes Update_ModbusData()
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

        protected abstract csModbusLib.ErrorCodes Modbus_ReadData();
        protected abstract void UpdateCellValues();
    }

    public abstract class MasterRegsGridView : MasterGridView
    {
        protected ushort[] MbRegsData;

        public MasterRegsGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns)
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public override void InitGridView(MbMaster Master)
        {
            base.InitGridView(Master);
            MbRegsData = new ushort[NumItems];
        }

        protected override void UpdateCellValues()
        {
            GridView.UpDateModbusCells(MbRegsData);
        }

        protected override void CellValueChanged(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            mut.WaitOne();
            ModBusSendRegs((ushort)e.RowIndex, (ushort)CurrentCell.Value);
            mut.ReleaseMutex();
        }

        protected virtual void ModBusSendRegs(ushort Idx, ushort Value) { }
    }

    public abstract class MasterBoolGridView : MasterGridView
    {
        protected bool[] MbCoilsData;

        public MasterBoolGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public override void InitGridView(MbMaster Master)
        {
            base.InitGridView(Master);
            MbCoilsData = new bool[NumItems];
        }

        protected override void UpdateCellValues()
        {
            GridView.UpDateModbusCells(MbCoilsData);
        }

        protected override void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            // Tricky solution here because CheckBoxCell CellValueChanged does sometimes not fires 
            // I use CellContentClick which works with MousClick as well as wit keyboard
            // The cell is set to ReadOnly and the check status is changed here
            // so I'm sure the grisview cell is equal to the mosbus data
            if (!CurrentCell.ReadOnly) {
                bool NewCellValue = ! (bool) CurrentCell.Value;
                CurrentCell.Value = NewCellValue;
                mut.WaitOne();
                int DataIdx = e.RowIndex * ItemColumns + e.ColumnIndex;
                ModbusSendCoils(DataIdx, NewCellValue);
                mut.ReleaseMutex();
            }
        }

        protected virtual void ModbusSendCoils(int Idx, bool Value)
        {
        }
    }

    public class MasterHoldingRegsGridView : MasterRegsGridView
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

        protected override csModbusLib.ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadHoldingRegisters(BaseAddr, NumItems, MbRegsData, 0);
        }

        protected override void ModBusSendRegs(ushort Idx, ushort Value)
        {
            // TODO Async Call ?
            ErrCode = MyMaster.WriteSingleRegister((ushort)(BaseAddr + Idx), Value);
        }
    }

    public class MasterInputRegsGridView : MasterRegsGridView
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

        protected override csModbusLib.ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadInputRegisters(BaseAddr, NumItems, MbRegsData, 0);
        }
    }

    public class MasterCoilsGridView : MasterBoolGridView
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

        protected override csModbusLib.ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadCoils(BaseAddr, NumItems, MbCoilsData, 0);
        }

        protected override void ModbusSendCoils(int Idx, bool Value)
        {
            // TODO Async Call ?
            ErrCode = MyMaster.WriteSingleCoil((ushort)(BaseAddr + Idx), Value);
        }
    }

    public class MasterDiscretInputsGridView : MasterBoolGridView
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

        protected override csModbusLib.ErrorCodes Modbus_ReadData()
        {
            return MyMaster.ReadDiscreteInputs(BaseAddr, NumItems, MbCoilsData, 0);
        }
    }
}

