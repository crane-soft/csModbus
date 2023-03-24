using csModbusLib;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace csModbusView
{
    [System.ComponentModel.DesignerCategory("")]
    public abstract class ModbusView : Panel
    {
        public enum ModbusObjectType
        {
            Coils,
            DiscreteInputs,
            HoldingRegister,
            InputRegister
        }

        private MbGridView mbView;
        private Label lbTitle;
        private bool InhibitRefresh;
        private bool _AutoSize;

        public ModbusView(ModbusObjectType MbType, string Title, bool IsMaster)
        {
            lbTitle = new Label();
            lbTitle.BackColor = System.Drawing.SystemColors.ControlLight;
            lbTitle.Dock = DockStyle.Top;
            lbTitle.Location = new System.Drawing.Point(0, 0);
            lbTitle.Height = 17;
            lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Controls.Add(lbTitle);
            this.Title = Title;

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
            BorderStyle = BorderStyle.FixedSingle;
            this.NumItems = 1;
            this.ItemColumns = 1;
            this.SizeChanged += ModbusView_SizeChanged;
            this.AutoSizeChanged += ModbusView_AutoSizeChanged;
            AutoSize = true;
            DataType = MbGridView.ModbusDataType.UINT16;
            Endianes = MbGridView.Endianess.BigEndian;
        }

        public void setDesignMode(bool desigMode)
        {
            if (desigMode) {
                mbView.Enabled = false;
                lbTitle.Enabled = false;
            } else {
                mbView.Enabled = true;
                lbTitle.Enabled = true;
            }
        }

        public void setBrowsableProperties()
        {
            bool isBrowsable = mbView.IsCoil == false;
            setBrowsableProperty("DataType", isBrowsable);
            setBrowsableProperty("Endianes", isBrowsable && (TypeSize == 2));
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
        [System.ComponentModel.Description("The title of this register group")]
        [System.ComponentModel.DefaultValue("")]
        public virtual string Title {
            get { return lbTitle.Text; }
            set {lbTitle.Text = value;  }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Description("The modbus base address of this register group")]
        [System.ComponentModel.DefaultValue(0)]
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
        [System.ComponentModel.Description("Number of consecutive registers / coils for this group")]
        [System.ComponentModel.DefaultValue(1)]
        public virtual ushort NumItems {
            get {
                return mbView.NumItems;
            }

            set {
                mbView.NumItems = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Description("Number of register-/ coil- columns for this group")]
        [System.ComponentModel.DefaultValue(1)]
        public virtual int ItemColumns {
            get {
                return mbView.ItemColumns;
            }

            set {
                mbView.ItemColumns = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Description("Edit register row names for this group")]
        [System.ComponentModel.DefaultValue(null)]
        public string[] ItemNames {
            get {
                return mbView.ItemNames;
            }

            set {
                mbView.ItemNames = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Type of date")]
        [System.ComponentModel.DefaultValue(MbGridView.ModbusDataType.UINT16)]
        public MbGridView.ModbusDataType DataType
        {
            get {
                return mbView.DataType;
            }
            set {
                mbView.DataType = value;
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Enianess for 32bit data types")]
        [System.ComponentModel.DefaultValue(MbGridView.Endianess.BigEndian)]
        public MbGridView.Endianess Endianes
        {
            get {
                return mbView.Int32Endianes;
            }
            set {
                mbView.Int32Endianes = value;
            }
        }


            [System.ComponentModel.Category("Layout")]
        [System.ComponentModel.DefaultValue(true)]
        public override bool AutoSize {
            get {
                return _AutoSize;
            } 
            set {
                _AutoSize = value;
                RefreshView();
            }
        }

        public int DataSize
        {
            get {
                return mbView.DataSize;
            }
        }
        public int TypeSize
        {
            get {
                return mbView.TypeSize;
            }
        }
        public void InitModbusType(ModbusObjectType MbType, bool IsMaster)
        {
            switch (MbType) {
                case ModbusObjectType.DiscreteInputs: {
                    mbView.IsCoil = true;
                    mbView.ReadOnly = IsMaster;
                    break;
                }

                case ModbusObjectType.Coils: {
                    mbView.IsCoil = true;
                    mbView.ReadOnly = false;
                    break;
                }

                case ModbusObjectType.InputRegister: {
                    mbView.IsCoil = false;
                    mbView.ReadOnly = IsMaster;
                    break;
                }

                case ModbusObjectType.HoldingRegister: {
                    mbView.IsCoil = false;
                    mbView.ReadOnly = false;
                    break;
                }
            }
        }

        private void MbView_RowHeadersWidthChanged(object sender, System.EventArgs e)
        {
            if (InhibitRefresh == false) {
                AdjustSize(); ;
            }
        }

        private void RefreshView()
        {
            if (InhibitRefresh == false) {
                mbView.InitGridView();
                AdjustSize();
            }
        }

        public void AdjustSize() {
            if (AutoSize) {
                mbView.AdjustParentSize(this);
            }
        }

        private void ModbusView_AutoSizeChanged(object sender, System.EventArgs e)
        {
            RefreshView();
        }

        private void ModbusView_SizeChanged(object sender, System.EventArgs e)
        {
            RefreshView();
        }

        private void MbView_CellValueChanged(ushort[] data, DataGridViewCellEventArgs e)
        {
            CellValueChanged(data, e);
        }

        private void MbView_CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
            CellContentClick(CurrentCell, e);
        }

        protected virtual void CellValueChanged(ushort[] data, DataGridViewCellEventArgs e)
        {
        }

        protected virtual void CellContentClick(DataGridViewCell CurrentCell, DataGridViewCellEventArgs e)
        {
        }

        /// <summary>
        /// Set the Browsable property.
        /// NOTE: Be sure to decorate the property with [Browsable(true)]
        /// </summary>
        /// <param name="PropertyName">Name of the variable</param>
        /// <param name="bIsBrowsable">Browsable Value</param>
        private void setBrowsableProperty(string strPropertyName, bool bIsBrowsable)
        {
            // Get the Descriptor's Properties
            PropertyDescriptor theDescriptor = TypeDescriptor.GetProperties(this.GetType())[strPropertyName];

            // Get the Descriptor's "Browsable" Attribute
            BrowsableAttribute theDescriptorBrowsableAttribute = (BrowsableAttribute)theDescriptor.Attributes[typeof(BrowsableAttribute)];
            FieldInfo isBrowsable = theDescriptorBrowsableAttribute.GetType().GetField("Browsable", BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);

            // Set the Descriptor's "Browsable" Attribute
            isBrowsable.SetValue(theDescriptorBrowsableAttribute, bIsBrowsable);
        }
    }
}