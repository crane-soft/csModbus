Imports csModbusLib
Imports csModbusView
Public Class frmModSlave

    Private modSlave As MbSlaveServer = New MbSlaveServer()
    Private modbusConnection As MbInterface
    Private MyDataServer As StdDataServer
    Private ListenCount As Integer
    Private ListenPoints As Integer

    ' Demo Automation
    Private AutomationBlink As Boolean
    Private AutomationShift As Int16
    Private WithEvents AutomationTimer As New System.Windows.Forms.Timer()
    Private SlaveGridViewList As New Collection()

    Sub New()
        InitializeComponent()

        ToolButtonStop.Enabled = False

        InitModbusData()
        InitConnection()

    End Sub

    Private Sub InitModbusData()

        SlaveGridViewList.Add(ucHoldingRegs1)
        SlaveGridViewList.Add(ucHoldingRegs2)
        SlaveGridViewList.Add(ucInputRegs)
        SlaveGridViewList.Add(ucCoils)
        SlaveGridViewList.Add(ucDiscretInputs)

        MyDataServer = New StdDataServer()

        For Each mbView As SlaveGridView In SlaveGridViewList
            mbView.AddDataToServer(MyDataServer)
            AddHandler mbView.ValueChangedEvent, AddressOf MBGridView_ValueChanged
            AddHandler mbView.ValueReadEvent, AddressOf MBGridView_ValueChanged
        Next

    End Sub
    Private Sub MBGridView_ValueChanged(sender As Object, e As ModbusData.ModbusValueEventArgs)
        lbListenStatus.AutoSize = False
        ListenCount += 1
        If ListenCount >= 50 Then
            ListenCount = 0
            ListenPoints += 1
            If ListenPoints = 6 Then
                ListenPoints = 0
            End If
            Dim PointsStr As String = ""
            For i As Integer = 0 To ListenPoints
                PointsStr += "."
            Next
            lbListenStatus.Text = PointsStr
        End If

    End Sub

    Private Sub ToolButtonStart_Click(sender As Object, e As EventArgs) Handles ToolButtonStart.Click
        modSlave.StartListen(modbusConnection, MyDataServer)
        ToolButtonStart.Enabled = False
        ToolButtonStop.Enabled = True
        lbListenStatus.Text = "Listening..."
        lbListenStatus.AutoSize = True
    End Sub

    Private Sub ToolButtonStop_Click(sender As Object, e As EventArgs) Handles ToolButtonStop.Click
        modSlave.StopListen()
        ToolButtonStart.Enabled = True
        ToolButtonStop.Enabled = False
        lbListenStatus.Text = "_"
        lbListenStatus.AutoSize = False
    End Sub

    Private Sub csModSlave_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If modSlave IsNot Nothing Then
            modSlave.StopListen()
        End If
    End Sub

    Private Sub ToolButtonConnection_Click(sender As Object, e As EventArgs) Handles ToolButtonConnection.Click
        ShowOptionsDialog(Nothing)
    End Sub

    Private Sub ShowOptionsDialog(ConnectionIfc As String)
        Dim OptionsDialog As New dlgOptions(DeviceType.SLAVE, ConnectionIfc)
        If OptionsDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            InitConnection()
        End If
    End Sub
    Private Sub InitConnection()
        Select Case My.Settings.Connection
            Case "RTU"
                modbusConnection = New MbRTU(My.Settings.ComPort, My.Settings.Baudrate)
                lbConnectionOptions.Text = String.Format("RTU {0},{1}", My.Settings.ComPort, My.Settings.Baudrate)
            Case "ASCII"
                modbusConnection = New MbASCII(My.Settings.ComPort, My.Settings.Baudrate)
                lbConnectionOptions.Text = String.Format("ASCII {0},{1}", My.Settings.ComPort, My.Settings.Baudrate)
            Case "TCP"
                modbusConnection = New MbTCPSlave(My.Settings.TCPport)
                lbConnectionOptions.Text = String.Format("TCP:{0}", My.Settings.TCPport)
            Case "UDP"
                'modbusConnection = New MbUDPSlave(My.Settings.TCPport)
                modbusConnection = New MbUDPSyncSlave(My.Settings.TCPport)
                lbConnectionOptions.Text = String.Format("UDP:{0}", My.Settings.TCPport)
            Case Else
                MsgBox(My.Settings.Connection + vbCrLf + "not supported")
                ToolButtonStart.Enabled = False
                Return
        End Select
        MyDataServer.SlaveID = My.Settings.SlaveID
    End Sub

    Private Sub TestTimer_Tick(sender As Object, e As EventArgs) Handles AutomationTimer.Tick

        ucHoldingRegs1.SetValue(0, ucHoldingRegs1.Data(0) + 1)
        If ucHoldingRegs1.Data(0) And 1 Then
            AutomationBlink = Not AutomationBlink
            ucDiscretInputs.SetValue(19, AutomationBlink)
        End If
        If (AutomationShift And &H7F) = 0 Then
            AutomationShift = 1
        Else
            AutomationShift <<= 1
        End If
        For i As Integer = 0 To 7
            ucDiscretInputs.SetValue(8 + i, AutomationShift And 1 << i)
        Next

    End Sub

    Private Sub cmStartAuto_Click(sender As Object, e As EventArgs) Handles cmStartAuto.Click

        If AutomationTimer.Enabled Then
            AutomationTimer.Enabled = False
            cmStartAuto.Text = "Automation Start"
            lbAutoStatus.Text = "Automation Stopped"
        Else
            AutomationTimer.Interval = 500
            AutomationTimer.Enabled = True
            cmStartAuto.Text = "Automation Stop"
            lbAutoStatus.Text = "Automation Running"
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        ShowOptionsDialog(Nothing)
    End Sub

    Private Sub ModbusTCPToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModbusTCPToolStripMenuItem.Click
        ShowOptionsDialog("TCP")
    End Sub

    Private Sub ModbusRTUToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModbusRTUToolStripMenuItem.Click
        ShowOptionsDialog("RTU")
    End Sub
End Class
