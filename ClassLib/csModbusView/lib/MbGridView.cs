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
            HEX_16,
            INT32,
            UINT32,
            HEX_32,
            IEEE_754,
            PRO_STUD
        }

        public enum Endianess
        {
            LittleEndian,
            BigEndian,
        }

        private ModbusDataType _DataType;
        private int _TypeSize;
        private int _DataSize;
        private ushort _NumItems;

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
                if (IsCoil) {
                    _TypeSize = 1;
                } else {
                    switch (_DataType) {
                        case ModbusDataType.UINT16:
                        case ModbusDataType.HEX_16:
                        case ModbusDataType.INT16:
                            _TypeSize = 1;
                            break;
                        default:
                            _TypeSize = 2;
                            break;
                    }
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

        private void NewItemCell(int Row, int Col)
        {
            DataGridViewCell itemCell;
            if (IsCoil) {
                itemCell = new ModbusCoilGridViewCell(this);
            } else {
                switch (_DataType) {
                    default:
                        itemCell = new UINT16_GridViewCell(this);
                        break;
                    case ModbusDataType.INT16:
                        itemCell = new INT16_GridViewCell(this);
                        break;
                    case ModbusDataType.HEX_16:
                        itemCell = new HEX16_GridViewCell(this);
                        break;
                    case ModbusDataType.UINT32:
                    case ModbusDataType.HEX_32:
                        itemCell = new UINT32_GridViewCell(this, Int32Endianes);
                        break;
                    case ModbusDataType.INT32:
                        itemCell = new INT32_GridViewCell(this, Int32Endianes);
                        break;
                    case ModbusDataType.IEEE_754:
                        itemCell = new IEEE754_GridViewCell(this, Int32Endianes);
                        break;
                    case ModbusDataType.PRO_STUD:
                        itemCell = new PROSTUD_GridViewCell(this);
                        break;
                }
            }

            Rows[Row].Cells[Col] = itemCell;

            // All Coils Read-Only because chec changed is done in CellClick Event
            // ReadOnly can only be set after assigned to a row
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
                DataGridViewCell clieckedCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (clieckedCell.Tag != null)
                    MbCellContentClick?.Invoke(clieckedCell, e);
            }
        }

        private void MbGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DisableCellEvents || IsCoil)
                return;

            if (e.ColumnIndex >= 0) {
                ModbusRegGridViewCell changedCell = (ModbusRegGridViewCell)this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (changedCell.Tag != null) {

                    try {
                        ushort[] modData = changedCell.GetValue();
                        MbCellValueChanged?.Invoke(modData, e);
                    }

                    catch (Exception ex) {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public void UpDateModbusCells(ushort[] ModbusData)
        {
            UpDateModbusCells(ModbusData, 0, ModbusData.Length);
        }

        public void UpDateModbusCells(ushort[] ModbusData, int BaseIdx, int Size)
        {
            int CellIdx = BaseIdx / _TypeSize;
            int iRow = CellIdx / ItemColumns;
            int iCol = CellIdx % ItemColumns;

            // bool isLongValue = (typeof(DataT) == typeof(ushort)) && (_TypeSize == 2);

            for (int dIdx = BaseIdx; dIdx <= BaseIdx + Size - 1; dIdx += _TypeSize) {
                DataGridViewCell mbCell = this.Rows[iRow].Cells[iCol];
                if (IsCoil) {
                    mbCell.Value = ModbusData[dIdx] != 0;
                } else {
                    ((ModbusRegGridViewCell)mbCell).SetValue((ushort[])(object)ModbusData, dIdx);
                }
                iCol += 1;
                if ((iCol == this.ColumnCount)) {
                    iCol = 0;
                    iRow += 1;
                }
            }
        }
    }
}

