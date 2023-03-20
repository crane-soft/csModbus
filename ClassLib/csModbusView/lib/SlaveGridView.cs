using System;
using System.Windows.Forms;
using csModbusLib;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract  class SlaveGridView : ModbusView
    {
        private ModbusData MbData;

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
            MbData = nMbData;
            GridView.DisableCellEvents = true;
            UpdateCellValues();
            GridView.DisableCellEvents = false;
            MbData.ValueChangedEvent += this.MbData_ValueChangedEvent;
            MbData.ValueReadEvent += MbData_ValueReadEvent;
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
            return e.RowIndex * ItemColumns + e.ColumnIndex;
        }

        public abstract void AddDataToServer(StdDataServer DataSerer);
        protected abstract void UpDateGridCells(ModbusData.ModbusValueEventArgs e);
        protected abstract void UpdateCellValues();
    }

    public abstract  class SlaveGridViewDataT<DataT> : SlaveGridView
    {
        protected ModbusDataT<DataT> ModbusData;

        public SlaveGridViewDataT(ModbusObjectType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public DataT[] Data {
            get {
                return ModbusData.Data;
            }
        }
        public void SetValue(int Idx, DataT Value)
        {
            ModbusData.Data[Idx] = Value;
            GridView.UpDateModbusCells(ModbusData.Data,Idx,1);
        }

        protected override void UpdateCellValues()
        {
            GridView.UpDateModbusCells(ModbusData.Data);
        }

        protected override void UpDateGridCells(ModbusData.ModbusValueEventArgs e)
        {
            GridView.UpDateModbusCells(ModbusData.Data, e.BaseIdx, e.Size);
        }

        protected override void CellValueChanged(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            if (typeof(DataT) == typeof(ushort)) {
                ModbusData.Data[GridDataIdx(e)] =  (DataT) (object) Convert.ToUInt16(CurrentCell.Value);
            }
        }

        protected override void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            if (typeof(DataT) == typeof(bool)) {
                int DataIdx = GridDataIdx(e);
                bool CheckValue =  (bool)(object) ModbusData.Data[DataIdx];
                CheckValue = !CheckValue;
                ModbusData.Data[DataIdx] = (DataT) (object)CheckValue;
                CurrentCell.Value = CheckValue;
            }
        }

    }

    public  class SlaveHoldingRegsGridView : SlaveGridViewDataT<ushort>
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
            ModbusData = new ModbusDataT<ushort>(BaseAddr, NumItems);
            InitData(ModbusData);
            DataSerer.AddHoldingRegisters(ModbusData);
        }
    }

    public  class SlaveInputRegsGridView : SlaveGridViewDataT<ushort>
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
            ModbusData = new ModbusDataT<ushort>(BaseAddr, NumItems);
            InitData(ModbusData);
            DataSerer.AddInputRegisters(ModbusData);
        }
    }

    public  class SlaveCoilsGridView : SlaveGridViewDataT<bool>
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
            ModbusData = new ModbusDataT<bool>(BaseAddr, NumItems);
            InitData(ModbusData);
            DataSerer.AddCoils(ModbusData);
        }
    }

    public  class SlaveDiscretInputsGridView : SlaveGridViewDataT<bool>
    {
        public SlaveDiscretInputsGridView() : this(0, 8) { }

        public SlaveDiscretInputsGridView(ushort BaseAddr, ushort NumItems) 
            : this("DiscretInputs", BaseAddr, NumItems, 8) {}

        public SlaveDiscretInputsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusObjectType.DiscreteInputs, Title, BaseAddr, NumItems, ItemColumns){ }

        public override void AddDataToServer(StdDataServer DataSerer)
        {
            ModbusData = new ModbusDataT<bool>(BaseAddr, NumItems);
            InitData(ModbusData);
            DataSerer.AddDiscreteInputs(ModbusData);
        }
    }
}
