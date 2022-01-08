<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmModsMaster
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.cmStart = New System.Windows.Forms.Button()
        Me.lbCount = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lbErrorCnt = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lbConnectionOptions = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lbLastError = New System.Windows.Forms.Label()
        Me.ViewPanel = New System.Windows.Forms.Panel()
        Me.cmTest = New System.Windows.Forms.Button()
        Me.HoldingRegs1 = New csModbusView.MasterHoldingRegsGridView()
        Me.HoldingRegs2 = New csModbusView.MasterHoldingRegsGridView()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.ViewPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.OptionsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(636, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(93, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SettingsToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'cmStart
        '
        Me.cmStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmStart.Location = New System.Drawing.Point(513, 36)
        Me.cmStart.Name = "cmStart"
        Me.cmStart.Size = New System.Drawing.Size(111, 33)
        Me.cmStart.TabIndex = 1
        Me.cmStart.Text = "Start"
        Me.cmStart.UseVisualStyleBackColor = True
        '
        'lbCount
        '
        Me.lbCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbCount.AutoSize = True
        Me.lbCount.Location = New System.Drawing.Point(510, 320)
        Me.lbCount.Name = "lbCount"
        Me.lbCount.Size = New System.Drawing.Size(49, 13)
        Me.lbCount.TabIndex = 3
        Me.lbCount.Text = "OKcount"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(510, 339)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Errors"
        '
        'lbErrorCnt
        '
        Me.lbErrorCnt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbErrorCnt.AutoSize = True
        Me.lbErrorCnt.Location = New System.Drawing.Point(550, 339)
        Me.lbErrorCnt.Name = "lbErrorCnt"
        Me.lbErrorCnt.Size = New System.Drawing.Size(10, 13)
        Me.lbErrorCnt.TabIndex = 5
        Me.lbErrorCnt.Text = "-"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lbConnectionOptions})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 402)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(636, 22)
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lbConnectionOptions
        '
        Me.lbConnectionOptions.Name = "lbConnectionOptions"
        Me.lbConnectionOptions.Size = New System.Drawing.Size(69, 17)
        Me.lbConnectionOptions.Text = "Connection"
        '
        'lbLastError
        '
        Me.lbLastError.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbLastError.AutoSize = True
        Me.lbLastError.Location = New System.Drawing.Point(510, 362)
        Me.lbLastError.Name = "lbLastError"
        Me.lbLastError.Size = New System.Drawing.Size(10, 13)
        Me.lbLastError.TabIndex = 7
        Me.lbLastError.Text = "-"
        '
        'ViewPanel
        '
        Me.ViewPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ViewPanel.Controls.Add(Me.HoldingRegs2)
        Me.ViewPanel.Controls.Add(Me.HoldingRegs1)
        Me.ViewPanel.Controls.Add(Me.cmTest)
        Me.ViewPanel.Location = New System.Drawing.Point(12, 37)
        Me.ViewPanel.Name = "ViewPanel"
        Me.ViewPanel.Size = New System.Drawing.Size(477, 363)
        Me.ViewPanel.TabIndex = 9
        '
        'cmTest
        '
        Me.cmTest.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmTest.Location = New System.Drawing.Point(13, 320)
        Me.cmTest.Name = "cmTest"
        Me.cmTest.Size = New System.Drawing.Size(178, 29)
        Me.cmTest.TabIndex = 10
        Me.cmTest.Text = "RD Regs 1 / WR Regs 2"
        Me.cmTest.UseVisualStyleBackColor = True
        '
        'HoldingRegs1
        '
        Me.HoldingRegs1.BaseAddr = CType(10US, UShort)
        Me.HoldingRegs1.ItemColumns = 1
        Me.HoldingRegs1.ItemNames = Nothing
        Me.HoldingRegs1.Location = New System.Drawing.Point(3, 3)
        Me.HoldingRegs1.Name = "HoldingRegs1"
        Me.HoldingRegs1.NumItems = CType(8US, UShort)
        Me.HoldingRegs1.Size = New System.Drawing.Size(124, 162)
        Me.HoldingRegs1.TabIndex = 11
        Me.HoldingRegs1.Title = "Holding Regs 1"
        '
        'HoldingRegs2
        '
        Me.HoldingRegs2.BaseAddr = CType(20US, UShort)
        Me.HoldingRegs2.ItemColumns = 1
        Me.HoldingRegs2.ItemNames = Nothing
        Me.HoldingRegs2.Location = New System.Drawing.Point(3, 171)
        Me.HoldingRegs2.Name = "HoldingRegs2"
        Me.HoldingRegs2.NumItems = CType(5US, UShort)
        Me.HoldingRegs2.Size = New System.Drawing.Size(124, 108)
        Me.HoldingRegs2.TabIndex = 12
        Me.HoldingRegs2.Title = "Holding Regs 2"
        '
        'frmModsMaster
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(636, 424)
        Me.Controls.Add(Me.ViewPanel)
        Me.Controls.Add(Me.lbLastError)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.lbErrorCnt)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lbCount)
        Me.Controls.Add(Me.cmStart)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmModsMaster"
        Me.Text = "cs Modbus Master"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ViewPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmStart As System.Windows.Forms.Button
    Friend WithEvents lbCount As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbErrorCnt As System.Windows.Forms.Label
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents lbConnectionOptions As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lbLastError As System.Windows.Forms.Label
    Friend WithEvents ViewPanel As System.Windows.Forms.Panel
    Friend WithEvents cmTest As System.Windows.Forms.Button
    Friend WithEvents HoldingRegs2 As csModbusView.MasterHoldingRegsGridView
    Friend WithEvents HoldingRegs1 As csModbusView.MasterHoldingRegsGridView
End Class
