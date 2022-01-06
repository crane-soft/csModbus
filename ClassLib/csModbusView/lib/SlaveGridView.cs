using System;
using System.Windows.Forms;
using csModbusLib;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract  class SlaveGridView : ModbusView
    {
        private ModbusData MbData;

        public SlaveGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) : base(MbType, Title, false)
        {
            this.SetDataSize(BaseAddr, NumItems, ItemColumns);
        }

        protected void InitData(ModbusData nMbData)
        {
            MbData = nMbData;
            GridView.DisableCellEvents = true;
            UpdateCellValues();
            GridView.DisableCellEvents = false;
            MbData.ValueChangedEvent += this.RegsValueChanged;
        }

        public delegate void ValueChangedCallback(object sender, ModbusData.ValueChangedEventArgs e);

        public void RegsValueChanged(object sender, ModbusData.ValueChangedEventArgs e)
        {
            if (GridView.InvokeRequired) {
                var d = new ValueChangedCallback(RegsValueChanged);
                GridView.Invoke(d, new object[] { sender, e });
            } else {
                GridView.DisableCellEvents = true;
                UpDateGridCells(e);
                GridView.DisableCellEvents = false;
            }
        }

        protected int GridDataIdx(DataGridViewCellEventArgs e)
        {
            return e.RowIndex * ItemColumns + e.ColumnIndex;
        }

        public abstract void AddDataToServer(StdDataServer DataSerer);
        protected abstract void UpDateGridCells(ModbusData.ValueChangedEventArgs e);
        protected abstract void UpdateCellValues();
    }

    public abstract  class SlaveGridViewDataT<DataT> : SlaveGridView
    {
        protected ModbusDataT<DataT> ModbusData;
        public DataT[] Data {
            get {
                return ModbusData.Data;
            }
        }

        public SlaveGridViewDataT(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
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

        protected override void UpDateGridCells(ModbusData.ValueChangedEventArgs e)
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
            : base(ModbusDataType.HoldingRegister, Title, BaseAddr, NumItems, ItemColumns)
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
            : base(ModbusDataType.InputRegister, Title, BaseAddr, NumItems, ItemColumns)
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
            : base(ModbusDataType.Coils, Title, BaseAddr, NumItems, ItemColumns)
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
        public SlaveDiscretInputsGridView() : this(0, 8)
        {
        }

        public SlaveDiscretInputsGridView(ushort BaseAddr, ushort NumItems) 
            : this("DiscretInputs", BaseAddr, NumItems, 8)
        {
        }

        public SlaveDiscretInputsGridView(string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(ModbusDataType.DiscreteInputs, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public override void AddDataToServer(StdDataServer DataSerer)
        {
            ModbusData = new ModbusDataT<bool>(BaseAddr, NumItems);
            InitData(ModbusData);
            DataSerer.AddDiscreteInputs(ModbusData);
        }
    }
}
