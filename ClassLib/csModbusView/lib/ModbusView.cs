using csModbusLib;
using System.Windows.Forms;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class ModbusView : Panel
    {
        public enum ModbusDataType
        {
            Coils,
            DiscreteInputs,
            HoldingRegister,
            InputRegister
        }

        private Label lbTitle;
        private MbGridView mbView;
        private bool InhibitRefresh;
        private bool SizeExplizit;
        
        public ModbusView(ModbusDataType MbType, string Title, bool IsMaster)
        {
            lbTitle = new Label();
            lbTitle.Text = Title;
            lbTitle.BackColor = System.Drawing.SystemColors.ControlLight;
            lbTitle.Dock = DockStyle.Top;
            lbTitle.Location = new System.Drawing.Point(0, 0);
            lbTitle.Height = 17;
            lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Controls.Add(lbTitle);
            mbView = new MbGridView();
            mbView.Left = 0;
            mbView.Top = lbTitle.Height;
            mbView.Width = this.Width;
            mbView.Height = this.Height - lbTitle.Height;
            mbView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mbView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Controls.Add(mbView);
            InitModbusType(MbType, IsMaster);
            mbView.MbCellValueChanged += MbView_CellValueChanged;
            mbView.MbCellContentClick += MbView_CellContentClick;
            mbView.RowHeadersWidthChanged += MbView_RowHeadersWidthChanged;
            this.SizeChanged += ModbusView_SizeChanged;
        }

       public void setDesignMode (bool desigMode)
        {
            if (desigMode) {
                mbView.Enabled = false;
                lbTitle.Enabled = false;
            } else {
                mbView.Enabled = true;
                lbTitle.Enabled = true;

            }
        } 

        protected void SetDataSize(ushort BaseAddr, ushort NumItems, int ItemColumns)
        {
            InhibitRefresh = true;
            this.BaseAddr = BaseAddr;
            this.NumItems = NumItems;
            this.ItemColumns = ItemColumns;
            InhibitRefresh = false;
            RefreshView();
        }
   
        public MbGridView GridView {
            get {
                return mbView;
            }
        }

        [System.ComponentModel.Category("csModbus")]
        public string Title {
            get {
                return lbTitle.Text;
            }

            set {
                lbTitle.Text = value;
            }
        }

        [System.ComponentModel.Category("csModbus")]
        public ushort BaseAddr {
            get {
                return mbView.BaseAddr;
            }

            set {
                mbView.BaseAddr = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        public ushort NumItems {
            get {
                return mbView.NumItems;
            }

            set {
                mbView.NumItems = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        public int ItemColumns {
            get {
                return mbView.ItemColumns;
            }

            set {
                mbView.ItemColumns = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Set the name of each item")]
        public string[] ItemNames {
            get {
                return mbView.ItemNames;
            }

            set {
                mbView.ItemNames = value;
                RefreshView();
            }
        }

        public void InitModbusType(ModbusDataType MbType, bool IsMaster)
        {
            switch (MbType) {
                case ModbusDataType.DiscreteInputs: {
                        mbView.IsCoil = true;
                        mbView.ReadOnly = IsMaster;
                        break;
                    }

                case ModbusDataType.Coils: {
                        mbView.IsCoil = true;
                        mbView.ReadOnly = false;
                        break;
                    }

                case ModbusDataType.InputRegister: {
                        mbView.IsCoil = false;
                        mbView.ReadOnly = IsMaster;
                        break;
                    }

                case ModbusDataType.HoldingRegister: {
                        mbView.IsCoil = false;
                        mbView.ReadOnly = false;
                        break;
                    }
            }
        }

        private bool DisableSizeChangedEvent;
        private void MbView_RowHeadersWidthChanged(object sender, System.EventArgs e)
        {
            if (InhibitRefresh == false) {
                AdjustParentSize(); ;
            }
        }

        private void RefreshView()
        {
            if (InhibitRefresh == false) {
                mbView.InitGridView();
                AdjustParentSize();
            }
        }

        private void AdjustParentSize() {
            if (SizeExplizit == false) {
                DisableSizeChangedEvent = true;
                mbView.AdjustParentSize(this);
                DisableSizeChangedEvent = false;
            }
        }

        private void ModbusView_SizeChanged(object sender, System.EventArgs e)
        {
            if ((DisableSizeChangedEvent == false) && (DesignMode == false))
                SizeExplizit = true;
        }

        private void MbView_CellValueChanged(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            CellValueChanged(CurrentCell, e);
        }

        private void MbView_CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            CellContentClick(CurrentCell, e);
        }

        protected virtual void CellValueChanged(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
        }

        protected virtual void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
        }
    }
}