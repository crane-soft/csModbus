using csModbusLib;
using System;
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
        private bool EnableRefresh;
        private bool _AutoSize;

        private enum uintFormat
        {
            isdecimal,
            trailing_h,
            leading_0x
        }

        private uintFormat _BaseAddrFormat;
        private ModbusObjectType _MbType;
        public ModbusView(ModbusObjectType MbType, string Title, bool IsMaster)
        {
            EnableRefresh = false;
            this._MbType = MbType;
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
            this.Resize += ModbusView_SizeChanged;
            this.AutoSizeChanged += ModbusView_AutoSizeChanged;
            AutoSize = true;
            DataType = MbGridView.ModbusDataType.UINT16;
            Endianes = B32Endianess.BigEndian;
            EnableRefresh = true;
            _BaseAddrFormat = uintFormat.isdecimal;
            RefreshView();
        }

        public void setDesignMode(bool desigMode)
        {
            if (desigMode) {
                mbView.Enabled = false;
                lbTitle.Enabled = false;
            } else {
                mbView.Enabled = true;
                lbTitle.Enabled = true;
                RefreshView();
            }
        }

        public void setBrowsableProperties()
        {
            bool isNotCoil = mbView.IsCoil == false;
            setBrowsableProperty("DataType", isNotCoil);
            setBrowsableProperty("Endianes", isNotCoil && (TypeSize > 1));

            bool RdWr = (_MbType == ModbusObjectType.HoldingRegister) || (_MbType == ModbusObjectType.Coils);
            setBrowsableProperty("WrOnly", RdWr);
            setAllPropertyCategory(_MbType.ToString());
        }

        protected void SetDataSize(ushort BaseAddr, ushort NumItems, int ItemColumns)
        {
            EnableRefresh = false;
            this.uBaseAddr = BaseAddr;
            this.NumItems = NumItems;
            this.ItemColumns = ItemColumns;
            EnableRefresh = true;
            RefreshView();
        }

        public MbGridView GridView
        {
            get {
                return mbView;
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Description("The title of this register group")]
        [System.ComponentModel.DefaultValue("")]
        public virtual string Title
        {
            get { return lbTitle.Text; }
            set { lbTitle.Text = value; }
        }
                
        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Description("The modbus base address of this register group")]
        [System.ComponentModel.DefaultValue(0)]
        public string BaseAddr
        {
            get {
                switch (_BaseAddrFormat) {
                    case uintFormat.leading_0x:
                        return "0x" + uBaseAddr.ToString("X4");
                    case uintFormat.trailing_h:
                        return uBaseAddr.ToString("X4") + "h";
                    default:
                        return uBaseAddr.ToString();
                }

            }

            set {
                string sAddr = value;
                if (sAddr.StartsWith("0x")) {
                    _BaseAddrFormat = uintFormat.leading_0x;
                    sAddr = sAddr.Replace("0x", "");
                } else if (value.EndsWith("h")) {
                    _BaseAddrFormat = uintFormat.trailing_h;
                    sAddr = sAddr.Replace("h", "");
                } else {
                    _BaseAddrFormat = uintFormat.isdecimal;

                }
                if (_BaseAddrFormat == uintFormat.isdecimal)
                    uBaseAddr = Convert.ToUInt16(sAddr);
                else
                    uBaseAddr = Convert.ToUInt16(sAddr, 16);
            }
        }

        [Browsable(false)]
        public ushort uBaseAddr
        {
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
        public virtual ushort NumItems
        {
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
        public virtual int ItemColumns
        {
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
        public string[] ItemNames
        {
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
        [System.ComponentModel.DefaultValue(B32Endianess.BigEndian)]
        public B32Endianess Endianes
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
        public override bool AutoSize
        {
            get {
                return _AutoSize;
            }
            set {
                _AutoSize = value;
                RefreshView();
            }
        }

        [System.ComponentModel.Category("csModbus")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Write Only")]
        [System.ComponentModel.DefaultValue(false)]
        public bool WrOnly { get; set; }

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
            if (EnableRefresh) {
                AdjustSize(); ;
            }
        }

        private void RefreshView()
        {
            if (EnableRefresh) {
                EnableRefresh = false;
                mbView.Width = this.Width;
                mbView.InitGridView();
                AdjustSize();
                EnableRefresh = true;
            }
        }

        public void AdjustSize()
        {
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
            if (EnableRefresh) {
                RefreshView();
            }
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

        private void setAllPropertyCategory(string newCategory)
        {
            string[] propertyList = {
                "Title", "BaseAddr", "NumItems", "ItemColumns", "ItemNames", "DataType", "Endianes","WrOnly"
            };

            foreach (string prop in propertyList) {
                setBrowsableCategory(prop, newCategory);
            }
        }
        private void setBrowsableCategory(string strPropertyName, string newCategory)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[strPropertyName];
            CategoryAttribute categoryAttribute = (CategoryAttribute)descriptor.Attributes[typeof(CategoryAttribute)];
            FieldInfo category = categoryAttribute.GetType().GetField("categoryValue", BindingFlags.NonPublic | BindingFlags.Instance);
            category.SetValue(categoryAttribute, newCategory);
        }
    }
}