Imports System.ComponentModel
Imports csModbusLib
Imports csModbusView
Public Class frmModsMaster

    Private ModMaster As MbMaster
    Private modbusConnection As MbInterface

    Private sysRefreshTimer As System.Timers.Timer
    Private RefreshCount As Integer = 0
    Private ErrorCount As Integer = 0
    Private Running As Boolean = False
    Private ModbusDataList As New Collection()
    Private CurentAddPos As Point

    Public Sub New()

        InitializeComponent()

        ModMaster = New MbMaster()

        ' Views created in designer
        ModbusDataList.Add(HoldingRegs1)
        ModbusDataList.Add(HoldingRegs2)

        ' Views created here
        CurentAddPos = New Point(HoldingRegs1.Width + 20, 0)

        AddModbusView(New MasterCoilsGridView(10, 20))
        AddModbusView(New MasterDiscretInputsGridView(20, 20))


        For Each mbView As MasterGridView In ModbusDataList
            mbView.InitGridView(Me.ModMaster)
        Next

        sysRefreshTimer = New System.Timers.Timer()
        sysRefreshTimer.Enabled = False
        sysRefreshTimer.AutoReset = False
        sysRefreshTimer.Interval = 20
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
        lbLastError.Text = "Connecting.."
        Me.Update()
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
                lbLastError.Text = "Connected"
            Else
                lbLastError.Text = "Connection Error"
            End If

        Else
            ' TODO Master should closed at the end of one polling cycle
            ' therfor better make a close request here which is executet in the timer

            MasterClose()
            cmStart.Text = "Start"
            cmStart.BackColor = Color.Green
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
    Private Sub MasterClose()
        sysRefreshTimer.Enabled = False
        ModMaster.Close()
        Running = False
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
            If ErrCode = ErrorCodes.MODBUS_EXCEPTION Then
                Dim ModbusException As ExceptionCodes = ModMaster.GetModusException()
                LbLastModbusException.Text = ModbusException.ToString()
            Else
                LbLastModbusException.Text = ""
            End If
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

            Dim rdData As MasterHoldingRegsGridView = HoldingRegs1
            Dim wrData As MasterHoldingRegsGridView = HoldingRegs2
            Dim ErrCode As csModbusLib.ErrorCodes
            ErrCode = ModMaster.ReadWriteMultipleRegisters(rdData.BaseAddr, rdData.NumItems, rdData.Data,
                                                           wrData.BaseAddr, wrData.NumItems, wrData.Data)
            DisplayErrorCode(ErrCode)
            If ErrCode = csModbusLib.ErrorCodes.NO_ERROR Then
                rdData.UpdateCellValues()
            End If
        End If

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub frmModsMaster_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If ModMaster.IsConnected() Then
            MasterClose()
        End If
    End Sub
End Class
