using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csModbusLib;

namespace csModMaster
{
    public partial class dlgOptions : Form
    {
        private csModbusLib.DeviceType gDeviceType;
        private string gConnectionIfc;
        public dlgOptions(csModbusLib.DeviceType DeviceType)
            : this(DeviceType, null)
        {
        }

        public dlgOptions(csModbusLib.DeviceType DeviceType, string ConnectionIfc)
        {
            InitializeComponent();
            Load += dlgOptions_Load;
            OK_Button.Click += OK_Button_Click;
            Cancel_Button.Click += Cancel_Button_Click;
            cbMode.SelectedIndexChanged += cbMode_SelectedIndexChanged;


            gDeviceType = DeviceType;
            gConnectionIfc = ConnectionIfc;
            if (DeviceType == csModbusLib.DeviceType.MASTER) {
                lbHostname.Visible = true;
                tbHostname.Visible = true;
            } else {
                lbHostname.Visible = false;
                tbHostname.Visible = false;
                gbEthernet.Height -= tbHostname.Height;
            }
           
        }

        private void dlgOptions_Load(object sender, EventArgs e)
        {
            if (gConnectionIfc != null)
                cbMode.Text = gConnectionIfc;
            else
                cbMode.Text = Properties.Settings.Default.Connection;

            tbSlaveID.Text = Properties.Settings.Default.SlaveID.ToString();
            tbTCPport.Text = Properties.Settings.Default.TCPport.ToString();
            tbComPort.Text = Properties.Settings.Default.ComPort;
            cbBaud.Text = Properties.Settings.Default.Baudrate.ToString();
            if (gDeviceType == csModbusLib.DeviceType.MASTER)
                tbHostname.Text = Properties.Settings.Default.Hostname;
        }

        private void OK_Button_Click(System.Object sender, System.EventArgs e)
        {
            Properties.Settings.Default.Connection = cbMode.Text;
            Properties.Settings.Default.SlaveID = Convert.ToInt32(tbSlaveID.Text);
            Properties.Settings.Default.TCPport = Convert.ToInt32(tbTCPport.Text);
            Properties.Settings.Default.ComPort = tbComPort.Text;
            Properties.Settings.Default.Baudrate = Convert.ToInt32(cbBaud.Text);
            if (gDeviceType == csModbusLib.DeviceType.MASTER)
                Properties.Settings.Default.Hostname = tbHostname.Text;
            Properties.Settings.Default.Save();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbMode.Text) {
                case "TCP":
                case "UDP": {
                        gbSerial.Enabled = false;
                        gbEthernet.Enabled = true;
                        break;
                    }

                case "RTU":
                case "ASCII": {
                        gbSerial.Enabled = true;
                        gbEthernet.Enabled = false;
                        break;
                    }
            }
        }
    }
}
