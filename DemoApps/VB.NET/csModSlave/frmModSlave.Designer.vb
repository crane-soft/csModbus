<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmModSlave
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
        Me.mainMenu = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ModbusTCPToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ModbusRTUToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lbConnectionOptions = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lbListenStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lbAutoStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.mainToolBar = New System.Windows.Forms.ToolStrip()
        Me.ToolButtonStart = New System.Windows.Forms.ToolStripButton()
        Me.ToolButtonStop = New System.Windows.Forms.ToolStripButton()
        Me.ToolButtonConnection = New System.Windows.Forms.ToolStripButton()
        Me.ViewPanel = New System.Windows.Forms.Panel()
        Me.cmStartAuto = New System.Windows.Forms.Button()
        Me.ucDiscretInputs = New csModbusView.SlaveDiscretInputsGridView()
        Me.ucInputRegs = New csModbusView.SlaveInputRegsGridView()
        Me.ucHoldingRegs2 = New csModbusView.SlaveHoldingRegsGridView()
        Me.ucHoldingRegs1 = New csModbusView.SlaveHoldingRegsGridView()
        Me.ucCoils = New csModbusView.SlaveCoilsGridView()
        Me.mainMenu.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.mainToolBar.SuspendLayout()
        Me.ViewPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'mainMenu
        '
        Me.mainMenu.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.mainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.OptionsToolStripMenuItem})
        Me.mainMenu.Location = New System.Drawing.Point(0, 0)
        Me.mainMenu.Name = "mainMenu"
        Me.mainMenu.Padding = New System.Windows.Forms.Padding(8, 2, 0, 2)
        Me.mainMenu.Size = New System.Drawing.Size(629, 28)
        Me.mainMenu.TabIndex = 0
        Me.mainMenu.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(44, 24)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(108, 26)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ModbusTCPToolStripMenuItem, Me.ModbusRTUToolStripMenuItem, Me.ToolStripSeparator1, Me.SettingsToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(73, 24)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'ModbusTCPToolStripMenuItem
        '
        Me.ModbusTCPToolStripMenuItem.Name = "ModbusTCPToolStripMenuItem"
        Me.ModbusTCPToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
        Me.ModbusTCPToolStripMenuItem.Text = "Modbus TCP"
        '
        'ModbusRTUToolStripMenuItem
        '
        Me.ModbusRTUToolStripMenuItem.Name = "ModbusRTUToolStripMenuItem"
        Me.ModbusRTUToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
        Me.ModbusRTUToolStripMenuItem.Text = "Modbus RTU"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(178, 6)
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lbConnectionOptions, Me.lbListenStatus, Me.lbAutoStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 515)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 19, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(629, 25)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lbConnectionOptions
        '
        Me.lbConnectionOptions.Name = "lbConnectionOptions"
        Me.lbConnectionOptions.Size = New System.Drawing.Size(84, 20)
        Me.lbConnectionOptions.Text = "Connection"
        '
        'lbListenStatus
        '
        Me.lbListenStatus.AutoSize = False
        Me.lbListenStatus.Name = "lbListenStatus"
        Me.lbListenStatus.Size = New System.Drawing.Size(40, 20)
        Me.lbListenStatus.Text = "_"
        Me.lbListenStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbAutoStatus
        '
        Me.lbAutoStatus.Margin = New System.Windows.Forms.Padding(15, 3, 0, 2)
        Me.lbAutoStatus.Name = "lbAutoStatus"
        Me.lbAutoStatus.Size = New System.Drawing.Size(149, 20)
        Me.lbAutoStatus.Text = "Automation Stopped"
        '
        'mainToolBar
        '
        Me.mainToolBar.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.mainToolBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolButtonStart, Me.ToolButtonStop, Me.ToolButtonConnection})
        Me.mainToolBar.Location = New System.Drawing.Point(0, 28)
        Me.mainToolBar.Name = "mainToolBar"
        Me.mainToolBar.Size = New System.Drawing.Size(629, 39)
        Me.mainToolBar.TabIndex = 2
        Me.mainToolBar.Text = "mainToolStrip"
        '
        'ToolButtonStart
        '
        Me.ToolButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolButtonStart.Image = Global.csModSlave.My.Resources.Resources.start_icon
        Me.ToolButtonStart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolButtonStart.Name = "ToolButtonStart"
        Me.ToolButtonStart.Size = New System.Drawing.Size(36, 36)
        Me.ToolButtonStart.Text = "Start Listen"
        '
        'ToolButtonStop
        '
        Me.ToolButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolButtonStop.Image = Global.csModSlave.My.Resources.Resources.stop_icon
        Me.ToolButtonStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolButtonStop.Name = "ToolButtonStop"
        Me.ToolButtonStop.Size = New System.Drawing.Size(36, 36)
        Me.ToolButtonStop.Text = "Stop Listen"
        '
        'ToolButtonConnection
        '
        Me.ToolButtonConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolButtonConnection.Image = Global.csModSlave.My.Resources.Resources.iconConnection
        Me.ToolButtonConnection.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolButtonConnection.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolButtonConnection.Name = "ToolButtonConnection"
        Me.ToolButtonConnection.Size = New System.Drawing.Size(36, 36)
        Me.ToolButtonConnection.Text = "Connection Oprions"
        '
        'ViewPanel
        '
        Me.ViewPanel.Controls.Add(Me.cmStartAuto)
        Me.ViewPanel.Controls.Add(Me.ucDiscretInputs)
        Me.ViewPanel.Controls.Add(Me.ucInputRegs)
        Me.ViewPanel.Controls.Add(Me.ucHoldingRegs2)
        Me.ViewPanel.Controls.Add(Me.ucHoldingRegs1)
        Me.ViewPanel.Controls.Add(Me.ucCoils)
        Me.ViewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ViewPanel.Location = New System.Drawing.Point(0, 67)
        Me.ViewPanel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ViewPanel.Name = "ViewPanel"
        Me.ViewPanel.Size = New System.Drawing.Size(629, 448)
        Me.ViewPanel.TabIndex = 3
        '
        'cmStartAuto
        '
        Me.cmStartAuto.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmStartAuto.Location = New System.Drawing.Point(16, 398)
        Me.cmStartAuto.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cmStartAuto.Name = "cmStartAuto"
        Me.cmStartAuto.Size = New System.Drawing.Size(247, 28)
        Me.cmStartAuto.TabIndex = 5
        Me.cmStartAuto.Text = "Start Automation"
        Me.cmStartAuto.UseVisualStyleBackColor = True
        '
        'ucDiscretInputs
        '
        Me.ucDiscretInputs.BaseAddr = CType(20US, UShort)
        Me.ucDiscretInputs.ItemColumns = 8
        Me.ucDiscretInputs.ItemNames = Nothing
        Me.ucDiscretInputs.Location = New System.Drawing.Point(172, 255)
        Me.ucDiscretInputs.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ucDiscretInputs.Name = "ucDiscretInputs"
        Me.ucDiscretInputs.NumItems = CType(20US, UShort)
        Me.ucDiscretInputs.Size = New System.Drawing.Size(307, 94)
        Me.ucDiscretInputs.TabIndex = 4
        Me.ucDiscretInputs.Title = "DiscretInputs"
        '
        'ucInputRegs
        '
        Me.ucInputRegs.BaseAddr = CType(7US, UShort)
        Me.ucInputRegs.ItemColumns = 3
        Me.ucInputRegs.ItemNames = New String() {"Velo", "Accel"}
        Me.ucInputRegs.Location = New System.Drawing.Point(172, 5)
        Me.ucInputRegs.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ucInputRegs.Name = "ucInputRegs"
        Me.ucInputRegs.NumItems = CType(10US, UShort)
        Me.ucInputRegs.Size = New System.Drawing.Size(305, 116)
        Me.ucInputRegs.TabIndex = 3
        Me.ucInputRegs.Title = "Input Register"
        '
        'ucHoldingRegs2
        '
        Me.ucHoldingRegs2.BaseAddr = CType(20US, UShort)
        Me.ucHoldingRegs2.ItemColumns = 1
        Me.ucHoldingRegs2.ItemNames = Nothing
        Me.ucHoldingRegs2.Location = New System.Drawing.Point(4, 210)
        Me.ucHoldingRegs2.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ucHoldingRegs2.Name = "ucHoldingRegs2"
        Me.ucHoldingRegs2.NumItems = CType(5US, UShort)
        Me.ucHoldingRegs2.Size = New System.Drawing.Size(175, 138)
        Me.ucHoldingRegs2.TabIndex = 2
        Me.ucHoldingRegs2.Title = "Holding Register"
        '
        'ucHoldingRegs1
        '
        Me.ucHoldingRegs1.BaseAddr = CType(10US, UShort)
        Me.ucHoldingRegs1.ItemColumns = 1
        Me.ucHoldingRegs1.ItemNames = Nothing
        Me.ucHoldingRegs1.Location = New System.Drawing.Point(4, 4)
        Me.ucHoldingRegs1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ucHoldingRegs1.Name = "ucHoldingRegs1"
        Me.ucHoldingRegs1.NumItems = CType(8US, UShort)
        Me.ucHoldingRegs1.Size = New System.Drawing.Size(175, 204)
        Me.ucHoldingRegs1.TabIndex = 1
        Me.ucHoldingRegs1.Title = "Holding Register"
        '
        'ucCoils
        '
        Me.ucCoils.BaseAddr = CType(10US, UShort)
        Me.ucCoils.ItemColumns = 8
        Me.ucCoils.ItemNames = Nothing
        Me.ucCoils.Location = New System.Drawing.Point(172, 137)
        Me.ucCoils.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ucCoils.Name = "ucCoils"
        Me.ucCoils.NumItems = CType(20US, UShort)
        Me.ucCoils.Size = New System.Drawing.Size(307, 94)
        Me.ucCoils.TabIndex = 0
        Me.ucCoils.Title = "Coils"
        '
        'frmModSlave
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(629, 540)
        Me.Controls.Add(Me.ViewPanel)
        Me.Controls.Add(Me.mainToolBar)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.mainMenu)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "frmModSlave"
        Me.Text = "cs-Modbus Slave"
        Me.mainMenu.ResumeLayout(False)
        Me.mainMenu.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.mainToolBar.ResumeLayout(False)
        Me.mainToolBar.PerformLayout()
        Me.ViewPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents mainMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ModbusRTUToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ModbusTCPToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents mainToolBar As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolButtonConnection As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolButtonStart As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolButtonStop As System.Windows.Forms.ToolStripButton
    Friend WithEvents lbConnectionOptions As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ViewPanel As System.Windows.Forms.Panel
    Friend WithEvents ucCoils As csModbusView.SlaveCoilsGridView
    Friend WithEvents ucHoldingRegs1 As csModbusView.SlaveHoldingRegsGridView
    Friend WithEvents ucHoldingRegs2 As csModbusView.SlaveHoldingRegsGridView
    Friend WithEvents ucInputRegs As csModbusView.SlaveInputRegsGridView
    Friend WithEvents ucDiscretInputs As csModbusView.SlaveDiscretInputsGridView
    Friend WithEvents cmStartAuto As Button
    Friend WithEvents lbAutoStatus As ToolStripStatusLabel
    Friend WithEvents lbListenStatus As ToolStripStatusLabel
End Class
