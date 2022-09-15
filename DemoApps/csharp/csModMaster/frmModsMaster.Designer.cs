namespace csModMaster
{
    partial class frmModsMaster
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try {
                if (disposing && components != null) {
                    components.Dispose();
                }
            } finally {
                base.Dispose(disposing);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmStart = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.lbErrorCnt = new System.Windows.Forms.Label();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbConnectionOptions = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbLastError = new System.Windows.Forms.ToolStripStatusLabel();
            this.LbLastModbusException = new System.Windows.Forms.ToolStripStatusLabel();
            this.ViewPanel = new System.Windows.Forms.Panel();
            this.HoldingRegs2 = new csModbusView.MasterHoldingRegsGridView();
            this.HoldingRegs1 = new csModbusView.MasterHoldingRegsGridView();
            this.cmTest = new System.Windows.Forms.Button();
            this.outPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.MenuStrip1.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.ViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.OptionsToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(1059, 24);
            this.MenuStrip1.TabIndex = 0;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.ExitToolStripMenuItem.Text = "Exit";
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingsToolStripMenuItem});
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.OptionsToolStripMenuItem.Text = "Options";
            // 
            // SettingsToolStripMenuItem
            // 
            this.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem";
            this.SettingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.SettingsToolStripMenuItem.Text = "Settings";
            // 
            // cmStart
            // 
            this.cmStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmStart.Location = new System.Drawing.Point(936, 36);
            this.cmStart.Name = "cmStart";
            this.cmStart.Size = new System.Drawing.Size(111, 33);
            this.cmStart.TabIndex = 1;
            this.cmStart.Text = "Start";
            this.cmStart.UseVisualStyleBackColor = true;
            // 
            // lbCount
            // 
            this.lbCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(933, 320);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(49, 13);
            this.lbCount.TabIndex = 3;
            this.lbCount.Text = "OKcount";
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(933, 339);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(34, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "Errors";
            // 
            // lbErrorCnt
            // 
            this.lbErrorCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbErrorCnt.AutoSize = true;
            this.lbErrorCnt.Location = new System.Drawing.Point(973, 339);
            this.lbErrorCnt.Name = "lbErrorCnt";
            this.lbErrorCnt.Size = new System.Drawing.Size(10, 13);
            this.lbErrorCnt.TabIndex = 5;
            this.lbErrorCnt.Text = "-";
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbConnectionOptions,
            this.lbLastError,
            this.LbLastModbusException});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 402);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(1059, 22);
            this.StatusStrip1.TabIndex = 6;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // lbConnectionOptions
            // 
            this.lbConnectionOptions.Name = "lbConnectionOptions";
            this.lbConnectionOptions.Size = new System.Drawing.Size(69, 17);
            this.lbConnectionOptions.Text = "Connection";
            // 
            // lbLastError
            // 
            this.lbLastError.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
            this.lbLastError.Name = "lbLastError";
            this.lbLastError.Size = new System.Drawing.Size(51, 17);
            this.lbLastError.Text = "No Error";
            // 
            // LbLastModbusException
            // 
            this.LbLastModbusException.Margin = new System.Windows.Forms.Padding(10, 3, 0, 2);
            this.LbLastModbusException.Name = "LbLastModbusException";
            this.LbLastModbusException.Size = new System.Drawing.Size(12, 17);
            this.LbLastModbusException.Text = "-";
            // 
            // ViewPanel
            // 
            this.ViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ViewPanel.Controls.Add(this.button1);
            this.ViewPanel.Controls.Add(this.HoldingRegs2);
            this.ViewPanel.Controls.Add(this.HoldingRegs1);
            this.ViewPanel.Location = new System.Drawing.Point(12, 37);
            this.ViewPanel.Name = "ViewPanel";
            this.ViewPanel.Size = new System.Drawing.Size(420, 315);
            this.ViewPanel.TabIndex = 9;
            // 
            // HoldingRegs2
            // 
            this.HoldingRegs2.BaseAddr = ((ushort)(20));
            this.HoldingRegs2.ItemColumns = 1;
            this.HoldingRegs2.ItemNames = null;
            this.HoldingRegs2.Location = new System.Drawing.Point(3, 171);
            this.HoldingRegs2.Name = "HoldingRegs2";
            this.HoldingRegs2.NumItems = ((ushort)(5));
            this.HoldingRegs2.Size = new System.Drawing.Size(124, 108);
            this.HoldingRegs2.TabIndex = 12;
            this.HoldingRegs2.Title = "Holding Regs 2";
            // 
            // HoldingRegs1
            // 
            this.HoldingRegs1.BaseAddr = ((ushort)(10));
            this.HoldingRegs1.ItemColumns = 1;
            this.HoldingRegs1.ItemNames = null;
            this.HoldingRegs1.Location = new System.Drawing.Point(3, 3);
            this.HoldingRegs1.Name = "HoldingRegs1";
            this.HoldingRegs1.NumItems = ((ushort)(8));
            this.HoldingRegs1.Size = new System.Drawing.Size(124, 162);
            this.HoldingRegs1.TabIndex = 11;
            this.HoldingRegs1.Title = "Holding Regs 1";
            // 
            // cmTest
            // 
            this.cmTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmTest.Location = new System.Drawing.Point(15, 361);
            this.cmTest.Name = "cmTest";
            this.cmTest.Size = new System.Drawing.Size(178, 29);
            this.cmTest.TabIndex = 10;
            this.cmTest.Text = "RD Regs 1 / WR Regs 2";
            this.cmTest.UseVisualStyleBackColor = true;
            // 
            // outPanel
            // 
            this.outPanel.Location = new System.Drawing.Point(459, 36);
            this.outPanel.Name = "outPanel";
            this.outPanel.Size = new System.Drawing.Size(437, 316);
            this.outPanel.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(310, 274);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 28);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmModsMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 424);
            this.Controls.Add(this.outPanel);
            this.Controls.Add(this.ViewPanel);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.cmTest);
            this.Controls.Add(this.lbErrorCnt);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.cmStart);
            this.Controls.Add(this.MenuStrip1);
            this.MainMenuStrip = this.MenuStrip1;
            this.Name = "frmModsMaster";
            this.Text = "cs Modbus Master";
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        System.Windows.Forms.MenuStrip MenuStrip1;
        System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        System.Windows.Forms.ToolStripMenuItem SettingsToolStripMenuItem;
        System.Windows.Forms.Button cmStart;
        System.Windows.Forms.Label lbCount;
        System.Windows.Forms.Label Label1;
        System.Windows.Forms.Label lbErrorCnt;
        System.Windows.Forms.StatusStrip StatusStrip1;
        System.Windows.Forms.ToolStripStatusLabel lbConnectionOptions;
        System.Windows.Forms.Panel ViewPanel;
        System.Windows.Forms.Button cmTest;
        csModbusView.MasterHoldingRegsGridView HoldingRegs2;
        csModbusView.MasterHoldingRegsGridView HoldingRegs1;
        System.Windows.Forms.ToolStripStatusLabel lbLastError;
        System.Windows.Forms.ToolStripStatusLabel LbLastModbusException;
        private System.Windows.Forms.Panel outPanel;
        private System.Windows.Forms.Button button1;
    }
}

