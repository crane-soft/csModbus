using System;
using System.Windows.Forms;
using csModbusLib;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class SlaveGridView : ModbusView
    {
        protected ModbusData ModbusData;

        public delegate void ValueChangedHandler(object sender, ModbusData.ModbusValueEventArgs e);
        public delegate void ValueReadHandler(object sender, ModbusData.ModbusValueEventArgs e);
        public event ValueChangedHandler ValueChangedEvent;
        public event ValueReadHandler ValueReadEvent;

        public SlaveGridView(ModbusObjectType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) : base(MbType, Title, false)
        {
            this.SetDataSize(BaseAddr, NumItems, ItemColumns);
        }

        protected void InitData(ModbusData nMbData)
        {
            ModbusData = nMbData;
            GridView.DisableCellEvents = true;
            UpdateCellValues();
            GridView.DisableCellEvents = false;
            ModbusData.ValueChangedEvent += this.MbData_ValueChangedEvent;
            ModbusData.ValueReadEvent += MbData_ValueReadEvent;
        }

        private delegate void ValueChangedCallback(object sender, ModbusData.ModbusValueEventArgs e);
        private void MbData_ValueReadEvent(object sender, ModbusData.ModbusValueEventArgs e)
        {
            if (this.ValueReadEvent != null) {
                if (GridView.InvokeRequired) {
                    var d = new ValueChangedCallback(MbData_ValueReadEvent);
                    GridView.BeginInvoke(d, new object[] { sender, e });
                } else {
                    this.ValueReadEvent.Invoke(this, e);
                }
            }
        }

        public void MbData_ValueChangedEvent(object sender, ModbusData.ModbusValueEventArgs e)
        {
            if (this.ValueChangedEvent != null) {
                if (GridView.InvokeRequired) {
                    var d = new ValueChangedCallback(MbData_ValueChangedEvent);
                    GridView.BeginInvoke(d, new object[] { sender, e });
                } else {
                    GridView.DisableCellEvents = true;
                    UpDateGridCells(e);
                    GridView.DisableCellEvents = false;
                    this.ValueChangedEvent.Invoke(this, e);
                }
            }
        }

        protected int GridDataIdx(DataGridViewCellEventArgs e)
        {
            return (e.RowIndex * ItemColumns + e.ColumnIndex) * TypeSize;
        }

        public abstract void AddDataToServer(StdDataServer DataSerer);
  

        public ushort[] Data {
            get {
                return ModbusData.Data;
            }
        }

        public void SetValue(int Idx, ushort Value)
        {
            ModbusData.Data[Idx] = Value;
            GridView.UpDateModbusCells(ModbusData.Data, Idx, 1);
        }

        protected void UpdateCellValues()
        {
            GridView.UpDateModbusCells(ModbusData.Data);
        }

        protected void UpDateGridCells(ModbusData.ModbusValueEventArgs e)
        {
            GridView.UpDateModbusCells(ModbusData.Data, e.BaseIdx, e.Size);
        }
    }

    public abstract class SlaveRegsGridViewData : SlaveGridView
    {
        public SlaveRegsGridViewData(ModbusObjectType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns)
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }
        protected override void CellValueChanged(ushort[] data, DataGridViewCellEventArgs e)
        {
            int idx = GridDataIdx(e);
            ModbusData.Data[idx] = data[0];
            if (data.Length > 1) {
                ModbusData.Data[idx + 1] = data[1];
            }
        }

    }

    public abstract class SlaveCoilsGridViewData : SlaveGridView
    {
        public SlaveCoilsGridViewData(ModbusObjectType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns)
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        protected override void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            int DataIdx = GridDataIdx(e);
            bool CheckValue = ModbusData.Data[DataIdx] != 0;
            CheckValue = !CheckValue;
            ModbusData.Data[DataIdx] = (ushort)(CheckValue ? 1 : 0);
            CurrentCell.Value = CheckValue;
        }
    }

    public  class SlaveHoldingRegsGridView : SlaveRegsGridViewData
    {
        public SlaveHoldingRegsGridView() : this(0, 1)
        {
        }

        public SlaveHoldingRegsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Holding Register", BaseAddr, NumItems, 1)
        {
        }

        public SlaveHoldingRegsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.HoldingRegister, Title, BaseAddr, NumItems, ItemColumns)
        {
        }
        public override void AddDataToServer(StdDataServer DataSerer)
        {
            ModbusRegsData RegsData = new ModbusRegsData(uBaseAddr, DataSize);
            InitData(RegsData);
            DataSerer.AddHoldingRegisters(RegsData);
        }
    }

    public  class SlaveInputRegsGridView : SlaveRegsGridViewData
    {
        public SlaveInputRegsGridView() : this(0, 1)
        {
        }

        public SlaveInputRegsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Input Register", BaseAddr, NumItems, 1)
        {
        }

        public SlaveInputRegsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.InputRegister, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public override void AddDataToServer(StdDataServer DataSerer)
        {
            ModbusRegsData RegsData = new ModbusRegsData(uBaseAddr, DataSize);
            InitData(RegsData);
            DataSerer.AddInputRegisters(RegsData);
        }
    }

    public  class SlaveCoilsGridView : SlaveCoilsGridViewData
    {
        public SlaveCoilsGridView() : this(0, 8)
        {
        }

        public SlaveCoilsGridView(ushort BaseAddr, ushort NumItems) 
            : this("Coils", BaseAddr, NumItems, 8)
        {
        }

        public SlaveCoilsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.Coils, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public override void AddDataToServer(StdDataServer DataSerer)
        {
            ModbusCoilsData CoilsData = new ModbusCoilsData(uBaseAddr, DataSize);
            InitData(CoilsData);
            DataSerer.AddCoils(CoilsData);
        }
    }

    public  class SlaveDiscretInputsGridView : SlaveCoilsGridViewData
    {
        public SlaveDiscretInputsGridView() : this(0, 8) { }

        public SlaveDiscretInputsGridView(ushort BaseAddr, ushort NumItems) 
            : this("DiscretInputs", BaseAddr, NumItems, 8) {}

        public SlaveDiscretInputsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.DiscreteInputs, Title, BaseAddr, NumItems, ItemColumns){ }

        public override void AddDataToServer(StdDataServer DataSerer)
        {
            ModbusCoilsData CoilsData = new ModbusCoilsData(uBaseAddr, DataSize);
            InitData(CoilsData);
            DataSerer.AddDiscreteInputs(CoilsData);
        }
    }
}
