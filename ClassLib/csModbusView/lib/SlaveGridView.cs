using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using csModbusLib;
using System.Windows.Forms;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract  class SlaveGridView : ModbusView
    {
        private csModbusLib.ModbusData MbData;

        public SlaveGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) : base(MbType, Title, false)
        {
            this.SetDataSize(BaseAddr, NumItems, ItemColumns);
        }

        protected void InitData(csModbusLib.ModbusData nMbData)
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

    public abstract  class SlaveRegsGridView : SlaveGridView
    {
        protected csModbusLib.ModbusRegsData MbRegsData;

        public SlaveRegsGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public void SetValue(int Idx, ushort Value)
        {
            MbRegsData.Data[Idx] = Value;
            GridView.UpDateModbusCells(MbRegsData.Data,Idx,1);
        }

        protected override void UpdateCellValues()
        {
            GridView.UpDateModbusCells(MbRegsData.Data);
        }

        protected override void UpDateGridCells(ModbusData.ValueChangedEventArgs e)
        {
            GridView.UpDateModbusCells(MbRegsData.Data, e.BaseIdx, e.Size);
        }

        protected override void CellValueChanged(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            if (GridView.DisableCellEvents == false) {
                
                MbRegsData.Data[GridDataIdx(e)] = Convert.ToUInt16(CurrentCell.Value);
            }
        }
    }

    public abstract  class SlaveBoolGridView : SlaveGridView
    {
        protected csModbusLib.ModbusBoolData MbBoolData;

        public SlaveBoolGridView(ModbusDataType MbType, string Title, ushort BaseAddr, ushort NumItems, int ItemColumns) 
            : base(MbType, Title, BaseAddr, NumItems, ItemColumns)
        {
        }

        public void SetValue(int Idx, bool Value)
        {
            MbBoolData.Data[Idx] = Value;
            GridView.UpDateModbusCells(MbBoolData.Data, Idx, 1);
        }

        protected override void UpdateCellValues()
        {
            GridView.UpDateModbusCells(MbBoolData.Data);
        }

        protected override void UpDateGridCells(ModbusData.ValueChangedEventArgs e)
        {
            GridView.UpDateModbusCells(MbBoolData.Data, e.BaseIdx, e.Size);
        }

        protected override void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            int DataIdx = GridDataIdx(e);
            bool CheckValue = !MbBoolData.Data[DataIdx];
            MbBoolData.Data[DataIdx] = CheckValue;
            CurrentCell.Value = CheckValue;
        }
    }

    public  class SlaveHoldingRegsGridView : SlaveRegsGridView
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
            MbRegsData = new csModbusLib.ModbusRegsData(BaseAddr, NumItems);
            InitData(MbRegsData);
            DataSerer.AddHoldingRegisters(MbRegsData);
        }
    }

    public  class SlaveInputRegsGridView : SlaveRegsGridView
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
            MbRegsData = new csModbusLib.ModbusRegsData(BaseAddr, NumItems);
            InitData(MbRegsData);
            DataSerer.AddInputRegisters(MbRegsData);
        }
    }

    public  class SlaveCoilsGridView : SlaveBoolGridView
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
            MbBoolData = new csModbusLib.ModbusBoolData(BaseAddr, NumItems);
            InitData(MbBoolData);
            DataSerer.AddCoils(MbBoolData);
        }
    }

    public  class SlaveDiscretInputsGridView : SlaveBoolGridView
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
            MbBoolData = new csModbusLib.ModbusBoolData(BaseAddr, NumItems);
            InitData(MbBoolData);
            DataSerer.AddDiscreteInputs(MbBoolData);
        }
    }
}
