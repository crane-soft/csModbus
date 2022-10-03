using System;
using System.Windows.Forms;
using System.Drawing;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    [System.ComponentModel.ToolboxItem(false)]
    public class MbGridView : DataGridView
    {
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
            this.SelectionChanged += MbGridView_SelectionChanged;
            this.CellClick += MbGridView_CellClick;
            this.CellValueChanged += MbGridView_CellValueChanged;
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsCoil { get; set; }
        public bool DisableCellEvents { get; set; }
        public ushort BaseAddr { get; set; }
        public ushort NumItems { get; set; }
        public int ItemColumns { get; set; }
        public string[] ItemNames { get; set; }

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
            for (int i = 0; i <= this.RowCount - 1; i++)
            {
                if (ItemNameCount > i)
                    Rows[i].HeaderCell.Value = ItemNames[i];
                else
                    Rows[i].HeaderCell.Value = "R" + (BaseAddr + i * ItemColumns).ToString();

                for (int itemCol = 0; itemCol <= ItemColumns - 1; itemCol++)
                {
                    itemCount -= 1;
                    if (itemCount >= 0)
                        NewItemCell(i, itemCol);
                    else
                        NewEmptyCell(i, itemCol);
                }
            }

            for (int itemCol = 0; itemCol <= ItemColumns - 1; itemCol++)
            {
                if (IsCoil)
                {
                    Columns[itemCol].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    Columns[itemCol].Width = RowTemplate.Height;
                }
                else
                    Columns[itemCol].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            DisableCellEvents = false;
        }

        public void AdjustParentSize(Control parent)
        {
            if ((NumItems == 0) | (this.RowCount == 0))
                return;
            parent.Height = this.Top + this.RowCount * this.RowTemplate.Height + 1;

            int ColWidth;
            if (IsCoil)
                ColWidth = Columns[0].Width;
            else
                ColWidth = 45;
            parent.Width = RowHeadersWidth + ItemColumns * ColWidth + 1;
        }

        private void NewItemCell(int Row, int Col)
        {
            DataGridViewCell itemCell;
            if (IsCoil)
            {
                itemCell = new DataGridViewCheckBoxCell();
                itemCell.Value = false;
            }
            else
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
            for (int iRow = 0; iRow <= Names.Length; iRow++)
            {
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
        public delegate void MbCellValueChangedEventHandler(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e);

        private void MbGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DisableCellEvents)
                return;
            if (e.ColumnIndex >= 0)
            {
                DataGridViewCell CurrentCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (CurrentCell.Tag != null)
                   MbCellContentClick?.Invoke(CurrentCell, e);
            }
        }

        private void MbGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DisableCellEvents)
                return;
            if (e.ColumnIndex >= 0)
            {
                DataGridViewCell CurrentCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (CurrentCell.Tag != null)
                    MbCellValueChanged?.Invoke(CurrentCell, e);
            }
        }

        public void UpDateModbusCells<DataT>(DataT[] ModbusData)
        {
            UpDateModbusCells(ModbusData, 0, ModbusData.Length);
        }

        public void UpDateModbusCells<DataT>(DataT[] ModbusData, int BaseIdx, int Size)
        {
            int iRow = BaseIdx / ItemColumns;
            int iCol = BaseIdx % ItemColumns;
            for (int i = BaseIdx; i <= BaseIdx + Size - 1; i++)
            {
                DataGridViewCell mbCell = this.Rows[iRow].Cells[iCol];
                mbCell.Value = ModbusData[i];
                iCol += 1;
                if ((iCol == this.ColumnCount))
                {
                    iCol = 0;
                    iRow += 1;
                }
            }
        }
    }
}
