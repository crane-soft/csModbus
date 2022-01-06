Imports csModbusLib
Imports csModbusView
Public Class frmModsMaster

    Private ModMaster As MbMaster
    Private modbusConnection As MbInterface

    Private Const BaseAddress As UShort = 10

    Private sysRefreshTimer As System.Timers.Timer

    Private RefreshCount As Integer = 0
    Private ErrorCount As Integer = 0
    Private Running As Boolean = False
    Private RefreshRunning As Boolean
    Private ModbusDataList As New Collection()
    Private CurentAddPos As Point
    Public Sub New()


        InitializeComponent()

        ModMaster = New MbMaster()

        ' Views created without designer

        CurentAddPos = New Point(0, 0)
        AddModbusView(New MasterHoldingRegsGridView(10, 8))
        AddModbusView(New MasterHoldingRegsGridView(20, 5))
        CurentAddPos.Y = 0
        CurentAddPos.X += 130
        AddModbusView(New MasterInputRegsGridView(7, 10))

        ' With created in designer
        ModbusDataList.Add(ucCoils)
        ModbusDataList.Add(ucDiscretInputs)

        For Each mbView As MasterGridView In ModbusDataList
            mbView.InitGridView(Me.ModMaster)
        Next

        sysRefreshTimer = New System.Timers.Timer()
        sysRefreshTimer.Enabled = False
        sysRefreshTimer.AutoReset = False
        sysRefreshTimer.Interval = 2
        AddHandler sysRefreshTimer.Elapsed, AddressOf OnSystemTimedEvent

        cmStart.Enabled = False


    End Sub
    Private Sub AddModbusView(MbView As MasterGridView)
        MbView.Location = CurentAddPos
        ViewPanel.Controls.Add(MbView)
        ModbusDataList.Add(MbView)
        CurentAddPos.Y += MbView.Height
    End Sub

    Private Sub csModsMaster_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitConnection()
    End Sub
    Private Function MasterConnect() As Boolean
        If ModMaster.IsConnected() Then
            Return True
        End If

        Return ModMaster.Connect(modbusConnection, 1)
    End Function
    Private Sub cmStart_Click(sender As Object, e As EventArgs) Handles cmStart.Click
        cmStart.Enabled = False
        If Running = False Then
            If MasterConnect() Then
                sysRefreshTimer.Enabled = True
                cmStart.Text = "Stop"
                cmStart.BackColor = Color.Red
                SettingsToolStripMenuItem.Enabled = False
                Running = True
                cmTest.Enabled = False
            End If

        Else
            ' TODO Master should closed at the end of one polling cycle
            ' therfor better make a close request here which is executet in the timer

            ModMaster.Close()
            sysRefreshTimer.Enabled = False
            cmStart.Text = "Start"
            cmStart.BackColor = Color.Green
            Running = False
            lbCount.Text = 0
            RefreshCount = 0
            SettingsToolStripMenuItem.Enabled = True

            ErrorCount = 0
            lbErrorCnt.Text = 0
            lbLastError.Text = ""
            cmTest.Enabled = True
        End If
        cmStart.Enabled = True
    End Sub

    Private Sub OnSystemTimedEvent(sender As Object, e As Timers.ElapsedEventArgs)
        sysRefreshTimer.Enabled = False
        Dim ErrCode As csModbusLib.ErrorCodes
        ErrCode = RequestData()
        Invoke_DisplayErrorCode(ErrCode)

    End Sub
    Private Function RequestData() As csModbusLib.ErrorCodes
        Dim ErrCode As csModbusLib.ErrorCodes

        For Each mbView As MasterGridView In ModbusDataList
            ErrCode = mbView.Update_ModbusData()
            If ErrCode <> ErrorCodes.NO_ERROR Then
                Return ErrCode
            End If
        Next
        Return ErrorCodes.NO_ERROR
    End Function

    Delegate Sub DisplayErrorCode_Callback(ErrCode As csModbusLib.ErrorCodes)

    Private Sub Invoke_DisplayErrorCode(ErrCode As csModbusLib.ErrorCodes)
        If Me.InvokeRequired Then
            Dim d As New DisplayErrorCode_Callback(AddressOf DisplayErrorCode)
            Me.Invoke(d, New Object() {ErrCode})
        Else
            DisplayErrorCode(ErrCode)
        End If
    End Sub

    Private Sub DisplayErrorCode(ErrCode As csModbusLib.ErrorCodes)
        If ErrCode = ErrorCodes.NO_ERROR Then
            RefreshCount += 1
            lbCount.Text = RefreshCount
            ' TODO
            ' Transaction Ident only exist with Ethernet (UDP /TCP)
            ' lbCount.Text = ModMaster.GetTransactionIdentifier()
        Else
            ErrorCount += 1
            lbErrorCnt.Text = ErrorCount.ToString
            lbLastError.Text = ErrCode.ToString
        End If

        If Running Then
            sysRefreshTimer.Enabled = True
        End If

    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        Dim OptionsDialog As New dlgOptions(DeviceType.MASTER)
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
                modbusConnection = New MbTCPMaster(My.Settings.Hostname, My.Settings.TCPport)
                lbConnectionOptions.Text = String.Format("TCP {0} Port {1}", My.Settings.Hostname, My.Settings.TCPport)
            Case "UDP"
                modbusConnection = New MbUDPMaster(My.Settings.Hostname, My.Settings.TCPport)
                lbConnectionOptions.Text = String.Format("UDP {0}:{1}", My.Settings.Hostname, My.Settings.TCPport)

            Case Else
                MsgBox(My.Settings.Connection + vbCrLf + "not supported")
                cmStart.Enabled = False
                Return
        End Select
        cmStart.Enabled = True
    End Sub

    Private Sub GridViewRegs_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)
        sysRefreshTimer.Enabled = False
    End Sub

    Private Sub cmTest_Click_1(sender As Object, e As EventArgs) Handles cmTest.Click
        TestFunc()

    End Sub

    Private Sub TestFunc()

        If MasterConnect() Then

            Dim rddata(8) As UInt16
            Dim tdata(10) As UInt16
            tdata(0) = 73
            tdata(1) = 28
            tdata(2) = 29
            tdata(3) = 45
            tdata(4) = 46

            Dim ErrCode As csModbusLib.ErrorCodes
            ErrCode = ModMaster.ReadWriteMultipleRegisters(10, 8, rddata, 20, 5, tdata)
            DisplayErrorCode(ErrCode)

        End If

    End Sub

End Class
