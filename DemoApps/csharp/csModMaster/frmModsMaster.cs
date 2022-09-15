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
using csModbusView;

namespace csModMaster
{
    public partial class frmModsMaster : Form
    {
        private MbMaster ModMaster;
        private MbInterface modbusConnection;

        private System.Timers.Timer sysRefreshTimer;
        private int RefreshCount = 0;
        private int ErrorCount = 0;
        private bool Running = false;
        private List<ModbusView> ModbusViewList = new List<ModbusView>();
        private Point CurentAddPos;
        private csModbusLib.ConnectionType InterfaceType = ConnectionType.NO_CONNECTION;

        public frmModsMaster()
        {
            InitializeComponent();
            this.Load += csModsMaster_Load;
            this.FormClosing += frmModsMaster_Closing;

            this.SettingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;
            this.ExitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;

            this.cmStart.Click += cmStart_Click;
            this.cmTest.Click += cmTest_Click_1;

            ModMaster = new MbMaster();

            // Views created in designer
            ModbusViewList.Add(HoldingRegs1);
            ModbusViewList.Add(HoldingRegs2);

            // Views created here
            CurentAddPos = new Point(HoldingRegs1.Width + 20, 0);

            AddModbusView(new MasterCoilsGridView(10, 20));
            AddModbusView(new MasterDiscretInputsGridView(20, 20));


            foreach (MasterGridView mbView in ModbusViewList)
                mbView.InitGridView(this.ModMaster);

            sysRefreshTimer = new System.Timers.Timer();
            sysRefreshTimer.Enabled = false;
            sysRefreshTimer.AutoReset = false;
            sysRefreshTimer.Interval = 20;
            sysRefreshTimer.Elapsed += OnSystemTimedEvent;

            cmStart.Enabled = false;
            SerializeAllMbViews();
        }

        private void AddModbusView(MasterGridView MbView)
        {
            MbView.Location = CurentAddPos;
            ViewPanel.Controls.Add(MbView);
            ModbusViewList.Add(MbView);
            CurentAddPos.Y += MbView.Height;
        }

        private void SerializeAllMbViews()
        {
            MbViewSerialize mbser = new MbViewSerialize();
            mbser.Serialize(ViewPanel.Controls);
           // mbser.Deserialize(outPanel.Controls);
        }

        private void csModsMaster_Load(object sender, EventArgs e)
        {
            InitConnection();
        }
        private bool MasterConnect()
        {
            if (ModMaster.IsConnected)
                return true;
            lbLastError.Text = "Connecting..";
            this.Update();
            return ModMaster.Connect(modbusConnection, 1);
        }
        private void cmStart_Click(object sender, EventArgs e)
        {
            cmStart.Enabled = false;
            if (Running == false)
            {
                if (MasterConnect())
                {
                    sysRefreshTimer.Enabled = true;
                    cmStart.Text = "Stop";
                    cmStart.BackColor = Color.Red;
                    SettingsToolStripMenuItem.Enabled = false;
                    Running = true;
                    cmTest.Enabled = false;
                    lbLastError.Text = "Connected";
                }
                else
                    lbLastError.Text = "Connection Error";
            }
            else
                // TODO Master should closed at the end of one polling cycle
                // therfor better make a close request here which is executet in the timer

                MasterClose();
            cmStart.Enabled = true;
        }

        private void MasterClose()
        {
            sysRefreshTimer.Enabled = false;
            ModMaster.Close();
            Running = false;
            cmStart.Text = "Start";
            cmStart.BackColor = Color.Green;
            lbCount.Text = "0";
            RefreshCount = 0;
            SettingsToolStripMenuItem.Enabled = true;

            ErrorCount = 0;
            lbErrorCnt.Text = "0";
            lbLastError.Text = "";
            cmTest.Enabled = true;
        }

        private void OnSystemTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            sysRefreshTimer.Enabled = false;
            csModbusLib.ErrorCodes ErrCode;
            ErrCode = RequestData();
            Invoke_DisplayErrorCode(ErrCode);
        }
        private csModbusLib.ErrorCodes RequestData()
        {
            csModbusLib.ErrorCodes ErrCode;

            foreach (MasterGridView mbView in ModbusViewList)
            {
                ErrCode = mbView.Update_ModbusData();
                if (ErrCode != ErrorCodes.NO_ERROR)
                    return ErrCode;
            }
            return ErrorCodes.NO_ERROR;
        }

        delegate void DisplayErrorCode_Callback(csModbusLib.ErrorCodes ErrCode);

        private void Invoke_DisplayErrorCode(csModbusLib.ErrorCodes ErrCode)
        {
            if (this.InvokeRequired)
            {
                DisplayErrorCode_Callback d = new DisplayErrorCode_Callback(DisplayErrorCode);
                this.Invoke(d, new object[] { ErrCode });
            }
            else
                DisplayErrorCode(ErrCode);
        }

        private void DisplayErrorCode(csModbusLib.ErrorCodes ErrCode)
        {
            if (ErrCode == ErrorCodes.NO_ERROR)
            {
                RefreshCount += 1;
                lbCount.Text = RefreshCount.ToString();
            }
            else
            {
                ErrorCount += 1;
                lbErrorCnt.Text = ErrorCount.ToString();
                lbLastError.Text = ErrCode.ToString();
                if (ErrCode == ErrorCodes.MODBUS_EXCEPTION)
                {
                    ExceptionCodes ModbusException = ModMaster.GetModusException();
                    LbLastModbusException.Text = ModbusException.ToString();
                }
                else
                    LbLastModbusException.Text = "";

                if (InterfaceType == ConnectionType.TCP_IP)
                {
                    if ((ErrCode == csModbusLib.ErrorCodes.CONNECTION_ERROR) | (ErrCode == csModbusLib.ErrorCodes.CONNECTION_CLOSED))
                        MasterClose();
                }
            }

            if (Running)
                sysRefreshTimer.Enabled = true;
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOptions OptionsDialog = new dlgOptions(DeviceType.MASTER);
            if (OptionsDialog.ShowDialog() == DialogResult.OK)
                InitConnection();
        }
        private void InitConnection()
        {
            switch (Properties.Settings.Default.Connection)
            {
                case "RTU":
                    {
                        InterfaceType = csModbusLib.ConnectionType.SERIAL_RTU;
                        modbusConnection = new MbRTU(Properties.Settings.Default.ComPort, Properties.Settings.Default.Baudrate);
                        lbConnectionOptions.Text = string.Format("RTU {0},{1}", Properties.Settings.Default.ComPort, Properties.Settings.Default.Baudrate);
                        break;
                    }

                case "ASCII":
                    {
                        InterfaceType = csModbusLib.ConnectionType.SERIAL_ASCII;
                        modbusConnection = new MbASCII(Properties.Settings.Default.ComPort, Properties.Settings.Default.Baudrate);
                        lbConnectionOptions.Text = string.Format("ASCII {0},{1}", Properties.Settings.Default.ComPort, Properties.Settings.Default.Baudrate);
                        break;
                    }

                case "TCP":
                    {
                        InterfaceType = csModbusLib.ConnectionType.TCP_IP;
                        modbusConnection = new MbTCPMaster(Properties.Settings.Default.Hostname, Properties.Settings.Default.TCPport);
                        lbConnectionOptions.Text = string.Format("TCP {0} Port {1}", Properties.Settings.Default.Hostname, Properties.Settings.Default.TCPport);
                        break;
                    }

                case "UDP":
                    {
                        InterfaceType = csModbusLib.ConnectionType.UDP_IP;
                        modbusConnection = new MbUDPMaster(Properties.Settings.Default.Hostname, Properties.Settings.Default.TCPport);
                        lbConnectionOptions.Text = string.Format("UDP {0}:{1}", Properties.Settings.Default.Hostname, Properties.Settings.Default.TCPport);
                        break;
                    }

                default:
                    {
                        MessageBox.Show (Properties.Settings.Default.Connection + "\r\n" + "not supported");
                        cmStart.Enabled = false;
                        return;
                    }
            }
            cmStart.Enabled = true;
        }

        private void GridViewRegs_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            sysRefreshTimer.Enabled = false;
        }

        private void cmTest_Click_1(object sender, EventArgs e)
        {
            TestFunc();
        }

        private void TestFunc()
        {
            if (MasterConnect())
            {
                MasterHoldingRegsGridView rdData = HoldingRegs1;
                MasterHoldingRegsGridView wrData = HoldingRegs2;
                csModbusLib.ErrorCodes ErrCode;
                ErrCode = ModMaster.ReadWriteMultipleRegisters(rdData.BaseAddr, rdData.NumItems, rdData.Data, wrData.BaseAddr, wrData.NumItems, wrData.Data);
                DisplayErrorCode(ErrCode);
                if (ErrCode == csModbusLib.ErrorCodes.NO_ERROR)
                    rdData.UpdateCellValues();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmModsMaster_Closing(object sender, CancelEventArgs e)
        {
            if (ModMaster.IsConnected)
                MasterClose();
        }
  
    }
}
