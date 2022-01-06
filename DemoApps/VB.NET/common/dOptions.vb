Imports csModbusLib

Public Class dlgOptions
    Private gDeviceType As csModbusLib.DeviceType

    Public Sub New(DeviceType As csModbusLib.DeviceType)

        InitializeComponent()
        gDeviceType = DeviceType
        If DeviceType = csModbusLib.DeviceType.MASTER Then
            lbHostname.Visible = True
            tbHostname.Visible = True
        Else
            lbHostname.Visible = False
            tbHostname.Visible = False
            gbEthernet.Height -= tbHostname.Height
        End If

    End Sub

    Private Sub dlgOptions_Load(sender As Object, e As EventArgs) Handles Me.Load
        cbMode.Text = My.Settings.Connection
        tbSlaveID.Text = My.Settings.SlaveID
        tbTCPport.Text = My.Settings.TCPport
        tbComPort.Text = My.Settings.ComPort
        cbBaud.Text = My.Settings.Baudrate
        If gDeviceType = csModbusLib.DeviceType.MASTER Then
            tbHostname.Text = My.Settings.Hostname
        End If
    End Sub
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        My.Settings.Connection = cbMode.Text
        My.Settings.SlaveID = Val(tbSlaveID.Text)
        My.Settings.TCPport = Val(tbTCPport.Text)
        My.Settings.ComPort = tbComPort.Text
        My.Settings.Baudrate = Val(cbBaud.Text)
        If gDeviceType = csModbusLib.DeviceType.MASTER Then
            My.Settings.Hostname = tbHostname.Text
        End If
        My.Settings.Save()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub cbMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbMode.SelectedIndexChanged
        Select Case cbMode.Text
            Case "TCP", "UDP"
                gbSerial.Enabled = False
                gbEthernet.Enabled = True
            Case "RTU", "ASCII"
                gbSerial.Enabled = True
                gbEthernet.Enabled = False
        End Select
    End Sub
End Class
