using System;
using System.Windows.Forms;
using System.Drawing;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    [System.ComponentModel.ToolboxItem(false)]
    public class MbGridView : DataGridView
    {
        public enum ModbusDataType
        {
            INT16,
            UINT16,
            INT32,
            UINT32,
            IEEE_754,
            PRO_STUD
        }

        public enum Endianess
        {
            LittleEndian,
            BigEndian,
        }

        private ModbusDataType _DataType;
        private ushort _NumItems;
        private int _TypeSize;
        private int _DataSize;
        
        public MbGridView()
        {
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;

            RowTemplate.Height = 18;
            RowHeadersVisible = true;
            ColumnHeadersVisible = false;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;

            RowCount = 0;
            ColumnCount = 0;

            MultiSelect = false;
            this.DataType = ModbusDataType.UINT16;
            this.ScrollBars = ScrollBars.None;
            this.SelectionChanged += MbGridView_SelectionChanged;
            this.CellClick += MbGridView_CellClick;
            this.CellValueChanged += MbGridView_CellValueChanged;
        }

        public bool IsCoil { get; set; }
        public bool DisableCellEvents { get; set; }
        public ushort BaseAddr { get; set; }

        public int ItemColumns { get; set; }
        public string[] ItemNames { get; set; }
        public ushort NumItems
        {
            get {
                return _NumItems;
            }
            set {
                _NumItems = value;
                _DataSize = _NumItems * _TypeSize;
            }
        }
        public ModbusDataType DataType
        {
            get {
                return _DataType;
            }
            set {
                _DataType = value;
                switch (_DataType) {
                    case ModbusDataType.UINT16:
                    case ModbusDataType.INT16:
                        _TypeSize = 1;
                        break;
                    default:
                        _TypeSize = 2;
                        break;
                }
                _DataSize = _NumItems * _TypeSize;
            }
        }
        public int DataSize
        {
            get {
                return _DataSize;
            }
        }
        public int TypeSize
        {
            get {
                return _TypeSize;
            }
        }
        public Endianess Int32Endianes { get; set; }
        public void InitGridView()
        {
            if (NumItems == 0)
                return;
            if (ItemColumns == 0)
                ItemColumns = 1;
            ColumnCount = ItemColumns;

            int NumRows = NumItems / ItemColumns;
            int LeftItems = NumItems % ItemColumns;
            if (LeftItems > 0)
                NumRows += 1;
            this.RowCount = NumRows;
            int ItemNameCount = 0;
            if (ItemNames != null)
                ItemNameCount = ItemNames.Length;

            DisableCellEvents = true;
            int itemCount = NumItems;
            for (int i = 0; i <= this.RowCount - 1; i++) {
                if (ItemNameCount > i)
                    Rows[i].HeaderCell.Value = ItemNames[i];
                else
                    Rows[i].HeaderCell.Value = "R" + (BaseAddr + i * TypeSize * ItemColumns).ToString();

                for (int itemCol = 0; itemCol <= ItemColumns - 1; itemCol++) {
                    itemCount -= 1;
                    if (itemCount >= 0)
                        NewItemCell(i, itemCol);
                    else
                        NewEmptyCell(i, itemCol);
                }
            }

            for (int itemCol = 0; itemCol <= ItemColumns - 1; itemCol++) {
                if (IsCoil) {
                    Columns[itemCol].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    Columns[itemCol].Width = RowTemplate.Height;
                } else
                    Columns[itemCol].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            DisableCellEvents = false;
        }

        public void AdjustParentSize(Control parent)
        {
            if ((NumItems == 0) | (this.RowCount == 0))
                return;
            parent.Height = this.Top + this.RowCount * this.RowTemplate.Height + 3;

            int ColWidth;
            if (IsCoil)
                ColWidth = Columns[0].Width;
            else
                ColWidth = 45;
            parent.Width = RowHeadersWidth + ItemColumns * ColWidth + 2;
        }

        private void NewItemCell(int Row, int Col)
        {
            DataGridViewCell itemCell;
            if (IsCoil) {
                itemCell = new DataGridViewCheckBoxCell();
                itemCell.Value = false;
            } else
                itemCell = new DataGridViewTextBoxCell();
            itemCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemCell.Tag = this;
            Rows[Row].Cells[Col] = itemCell;
            // All Coils Read-Only becaus chec changed is done in CellClick Event
            itemCell.ReadOnly = IsCoil;
        }

        private void NewEmptyCell(int Row, int Col)
        {
            DataGridViewCell itemCell;
            itemCell = new DataGridViewTextBoxCell();
            itemCell.Value = "";
            itemCell.Style.BackColor = this.BackgroundColor;
            Rows[Row].Cells[Col] = itemCell;
            itemCell.ReadOnly = true;
        }

        public void SetItemNames(string[] Names)
        {
            for (int iRow = 0; iRow <= Names.Length; iRow++) {
                if (iRow >= RowCount)
                    break;
                Rows[iRow].HeaderCell.Value = Names[iRow];
            }
        }

        private void MbGridView_SelectionChanged(object sender, EventArgs e)
        {

            // For Each Item As DataGridViewCell In Me.SelectedCells
            // If Item.Tag Is Nothing Then   ' If only empty cells should be selected off ?
            this.ClearSelection();
        }

        public event MbCellContentClickEventHandler MbCellContentClick;
        public delegate void MbCellContentClickEventHandler(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e);
        public event MbCellValueChangedEventHandler MbCellValueChanged;
        public delegate void MbCellValueChangedEventHandler(ushort[] data, DataGridViewCellEventArgs e);

        private void MbGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DisableCellEvents)
                return;
            if (e.ColumnIndex >= 0) {
                DataGridViewCell CurrentCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (CurrentCell.Tag != null)
                    MbCellContentClick?.Invoke(CurrentCell, e);
            }
        }

        private void MbGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DisableCellEvents)
                return;
            if (e.ColumnIndex >= 0) {
                DataGridViewCell CurrentCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (CurrentCell.Tag != null) {
                    try {
                        ushort[] modData;
                        if (_TypeSize == 1) {
                            modData = Get16bitModbusData(CurrentCell.Value);
                        } else {
                            modData = Get32bitModbusData(CurrentCell.Value);
                            SwapEndinness(modData);
                        }
                        MbCellValueChanged?.Invoke(modData, e);

                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public void UpDateModbusCells<DataT>(DataT[] ModbusData)
        {
            UpDateModbusCells(ModbusData, 0, ModbusData.Length);
        }

        public void UpDateModbusCells<DataT>(DataT[] ModbusData, int BaseIdx, int Size)
        {
            int CellIdx = BaseIdx / _TypeSize;
            int iRow = CellIdx / ItemColumns;
            int iCol = CellIdx % ItemColumns;

            bool isLongValue = (typeof(DataT) == typeof(ushort)) && (_TypeSize == 2);

            for (int dIdx = BaseIdx; dIdx <= BaseIdx + Size - 1; dIdx += _TypeSize) {
                DataGridViewCell mbCell = this.Rows[iRow].Cells[iCol];
                if (IsCoil) {
                    mbCell.Value = ModbusData[dIdx];
                } else {
                    if (isLongValue) {
                        ushort[] mData = new ushort[2];
                        Array.Copy(ModbusData, dIdx, mData, 0, 2);
                        SwapEndinness(mData);

                        mbCell.Value = Get32bitCellValue(mData);
                    } else {
                        mbCell.Value = Get16bitCellValue((ushort)(object)ModbusData[dIdx]);
                    }
                }
                iCol += 1;
                if ((iCol == this.ColumnCount)) {
                    iCol = 0;
                    iRow += 1;
                }
            }
        }

        private object Get16bitCellValue(ushort ModbusData)
        {
            UInt16 cellValue = Convert.ToUInt16(ModbusData);
            if (_DataType == ModbusDataType.INT16) {
                return unchecked((Int16)cellValue);
            } else {
                return cellValue;
            }
        }

        private object Get32bitCellValue(ushort[] ModbusData)
        {
            UInt32 cellValue;
            cellValue = Convert.ToUInt32(ModbusData[1]) << 16;
            cellValue |= Convert.ToUInt32(ModbusData[0]);
            switch (_DataType) {
                case ModbusDataType.INT32:
                    return unchecked((Int32)cellValue);

                case ModbusDataType.IEEE_754:
                    Single[] fValue = new Single[1];
                    Buffer.BlockCopy(ModbusData, 0, fValue, 0, 4);
                    return fValue[0];

                case ModbusDataType.PRO_STUD:
                    int IntValue = ModbusData[1];
                    int sign = 1;
                    if ((IntValue & 0x8000) != 0) {
                        IntValue = IntValue & 0x7fff;
                        sign = -1;
                    }
                    int FractValue = ModbusData[0];
                    double value = (double)IntValue;
                    value += (double)FractValue / 100;
                    if (sign < 0) {
                        value = -value;
                    }
                    return value;

                default:
                    return unchecked((UInt32)cellValue);
            }
        }


        private ushort[] Get16bitModbusData(object cValue)
        {
            ushort[] modData = new ushort[1];
            if (_DataType == ModbusDataType.INT16) {
                Int16 iValue = Convert.ToInt16(cValue);
                modData[0] = unchecked((ushort)iValue);
            } else {
                modData[0] = Convert.ToUInt16(cValue);
            }
            return modData;
        }

        private ushort[] Get32bitModbusData(object cValue)
        {
            ushort[] modData = new ushort[2];
            UInt32 modValue = 0;
            Int32 iValue;
            switch (_DataType) {
                case ModbusDataType.PRO_STUD:
                    double dValue = Convert.ToDouble(cValue);
                    ushort sign = 0;
                    if (dValue < 0) {
                        dValue = -dValue;
                        sign = 0x8000;
                    }
                    iValue = (ushort)Math.Truncate(dValue);
                    if (iValue > Int16.MaxValue) {
                        iValue = Int16.MaxValue;
                    } else if (iValue < Int16.MinValue) {
                        iValue = Int16.MinValue;
                    }
                    modData[1] = (ushort)(iValue | sign);
                    modData[0] = Convert.ToUInt16((dValue - iValue) * 100);
                    return modData;

                case ModbusDataType.IEEE_754:
                    Single[] fValue = new Single[1];
                    fValue[0] = Convert.ToSingle(cValue);
                    Buffer.BlockCopy(fValue, 0, modData, 0, 4);
                    return modData;

                case ModbusDataType.INT32:
                    iValue = Convert.ToInt32(cValue);
                    modValue = unchecked((UInt32)iValue);
                    break;

                default:
                    modValue = Convert.ToUInt32(CurrentCell.Value);
                    break;
            }

            modData[1] = (ushort)(modValue >> 16);
            modData[0] = (ushort)(modValue & 0xffff);
            return modData;
        }

        void SwapEndinness(ushort[] data)
        {
            if (Int32Endianes == Endianess.BigEndian) {
                ushort tmp = data[0];
                data[0] = data[1];
                data[1] = tmp;
            }
        }
    }
}
