<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgOptions
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.gbSerial = New System.Windows.Forms.GroupBox()
        Me.tbComPort = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cbBaud = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbSlaveID = New System.Windows.Forms.TextBox()
        Me.cbMode = New System.Windows.Forms.ComboBox()
        Me.gbEthernet = New System.Windows.Forms.GroupBox()
        Me.tbHostname = New System.Windows.Forms.TextBox()
        Me.lbHostname = New System.Windows.Forms.Label()
        Me.tbTCPport = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.gbSerial.SuspendLayout()
        Me.gbEthernet.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(243, 153)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(37, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Mode"
        '
        'gbSerial
        '
        Me.gbSerial.Controls.Add(Me.tbComPort)
        Me.gbSerial.Controls.Add(Me.Label4)
        Me.gbSerial.Controls.Add(Me.cbBaud)
        Me.gbSerial.Controls.Add(Me.Label3)
        Me.gbSerial.Location = New System.Drawing.Point(204, 12)
        Me.gbSerial.Name = "gbSerial"
        Me.gbSerial.Size = New System.Drawing.Size(176, 106)
        Me.gbSerial.TabIndex = 2
        Me.gbSerial.TabStop = False
        Me.gbSerial.Text = "RTU / ASCII"
        '
        'tbComPort
        '
        Me.tbComPort.Location = New System.Drawing.Point(70, 19)
        Me.tbComPort.Name = "tbComPort"
        Me.tbComPort.Size = New System.Drawing.Size(72, 20)
        Me.tbComPort.TabIndex = 9
        Me.tbComPort.Text = "1"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(17, 22)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(47, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "ComPort"
        '
        'cbBaud
        '
        Me.cbBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbBaud.FormattingEnabled = True
        Me.cbBaud.Items.AddRange(New Object() {"300", "1200", "2400", "9600", "19200", "38400", "115200"})
        Me.cbBaud.Location = New System.Drawing.Point(70, 45)
        Me.cbBaud.Name = "cbBaud"
        Me.cbBaud.Size = New System.Drawing.Size(74, 21)
        Me.cbBaud.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(30, 49)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(32, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Baud"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(26, 49)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "SlaveID"
        '
        'tbSlaveID
        '
        Me.tbSlaveID.Location = New System.Drawing.Point(77, 46)
        Me.tbSlaveID.Name = "tbSlaveID"
        Me.tbSlaveID.Size = New System.Drawing.Size(72, 20)
        Me.tbSlaveID.TabIndex = 3
        Me.tbSlaveID.Text = "1"
        '
        'cbMode
        '
        Me.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMode.FormattingEnabled = True
        Me.cbMode.Items.AddRange(New Object() {"TCP", "UDP", "RTU", "ASCII"})
        Me.cbMode.Location = New System.Drawing.Point(77, 19)
        Me.cbMode.Name = "cbMode"
        Me.cbMode.Size = New System.Drawing.Size(72, 21)
        Me.cbMode.TabIndex = 5
        '
        'gbEthernet
        '
        Me.gbEthernet.Controls.Add(Me.tbHostname)
        Me.gbEthernet.Controls.Add(Me.lbHostname)
        Me.gbEthernet.Controls.Add(Me.tbTCPport)
        Me.gbEthernet.Controls.Add(Me.Label5)
        Me.gbEthernet.Location = New System.Drawing.Point(13, 100)
        Me.gbEthernet.Name = "gbEthernet"
        Me.gbEthernet.Size = New System.Drawing.Size(176, 81)
        Me.gbEthernet.TabIndex = 6
        Me.gbEthernet.TabStop = False
        Me.gbEthernet.Text = "TCP"
        '
        'tbHostname
        '
        Me.tbHostname.Location = New System.Drawing.Point(71, 42)
        Me.tbHostname.Name = "tbHostname"
        Me.tbHostname.Size = New System.Drawing.Size(89, 20)
        Me.tbHostname.TabIndex = 7
        Me.tbHostname.Text = "127.0.0.1"
        '
        'lbHostname
        '
        Me.lbHostname.AutoSize = True
        Me.lbHostname.Location = New System.Drawing.Point(10, 45)
        Me.lbHostname.Name = "lbHostname"
        Me.lbHostname.Size = New System.Drawing.Size(55, 13)
        Me.lbHostname.TabIndex = 6
        Me.lbHostname.Text = "Hostname"
        '
        'tbTCPport
        '
        Me.tbTCPport.Location = New System.Drawing.Point(71, 20)
        Me.tbTCPport.Name = "tbTCPport"
        Me.tbTCPport.Size = New System.Drawing.Size(89, 20)
        Me.tbTCPport.TabIndex = 5
        Me.tbTCPport.Text = "502"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(39, 23)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(26, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Port"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.tbSlaveID)
        Me.GroupBox3.Controls.Add(Me.Label2)
        Me.GroupBox3.Controls.Add(Me.cbMode)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Location = New System.Drawing.Point(13, 12)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(175, 82)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "General"
        '
        'dlgOptions
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(401, 194)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.gbEthernet)
        Me.Controls.Add(Me.gbSerial)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgOptions"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "dlgOptions"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.gbSerial.ResumeLayout(False)
        Me.gbSerial.PerformLayout()
        Me.gbEthernet.ResumeLayout(False)
        Me.gbEthernet.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents gbSerial As System.Windows.Forms.GroupBox
    Friend WithEvents tbSlaveID As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cbMode As System.Windows.Forms.ComboBox
    Friend WithEvents tbComPort As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cbBaud As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents gbEthernet As System.Windows.Forms.GroupBox
    Friend WithEvents tbTCPport As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents tbHostname As System.Windows.Forms.TextBox
    Friend WithEvents lbHostname As System.Windows.Forms.Label

End Class
