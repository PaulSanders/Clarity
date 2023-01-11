namespace WinformsFileExplorer.Views
{
    partial class MainView
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblReady = new System.Windows.Forms.ToolStripStatusLabel();
            this.propertyBinding1 = new Clarity.Winforms.PropertyBinding(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.driveStructureView1 = new WinformsFileExplorer.Views.DriveStructureView();
            this.driveView1 = new WinformsFileExplorer.Views.DriveView();
            this.fileView1 = new WinformsFileExplorer.Views.FileView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonBinding1 = new Clarity.Winforms.ButtonBinding(this.components);
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.propertyBinding1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buttonBinding1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.propertyBinding1.SetDisplayFormat(this.statusStrip1, null);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblReady});
            this.statusStrip1.Location = new System.Drawing.Point(0, 268);
            this.statusStrip1.Name = "statusStrip1";
            this.propertyBinding1.SetPropertyName(this.statusStrip1, null);
            this.statusStrip1.Size = new System.Drawing.Size(441, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblReady
            // 
            this.lblReady.Name = "lblReady";
            this.lblReady.Size = new System.Drawing.Size(39, 17);
            this.lblReady.Text = "Ready";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.DarkGray;
            this.propertyBinding1.SetDisplayFormat(this.splitContainer1, null);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.driveStructureView1);
            this.splitContainer1.Panel1.Controls.Add(this.driveView1);
            this.propertyBinding1.SetDisplayFormat(this.splitContainer1.Panel1, null);
            this.propertyBinding1.SetPropertyName(this.splitContainer1.Panel1, null);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.fileView1);
            this.propertyBinding1.SetDisplayFormat(this.splitContainer1.Panel2, null);
            this.propertyBinding1.SetPropertyName(this.splitContainer1.Panel2, null);
            this.propertyBinding1.SetPropertyName(this.splitContainer1, null);
            this.splitContainer1.Size = new System.Drawing.Size(441, 244);
            this.splitContainer1.SplitterDistance = 147;
            this.splitContainer1.TabIndex = 2;
            // 
            // driveStructureView1
            // 
            this.driveStructureView1.AutoCreateViewModel = false;
            this.propertyBinding1.SetDisplayFormat(this.driveStructureView1, null);
            this.driveStructureView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.driveStructureView1.Location = new System.Drawing.Point(0, 26);
            this.driveStructureView1.Name = "driveStructureView1";
            this.propertyBinding1.SetPropertyName(this.driveStructureView1, null);
            this.driveStructureView1.Size = new System.Drawing.Size(147, 218);
            this.driveStructureView1.TabIndex = 1;
            this.driveStructureView1.ViewModel = null;
            this.driveStructureView1.ViewModelType = null;
            // 
            // driveView1
            // 
            this.driveView1.AutoCreateViewModel = false;
            this.propertyBinding1.SetDisplayFormat(this.driveView1, null);
            this.driveView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.driveView1.Location = new System.Drawing.Point(0, 0);
            this.driveView1.Name = "driveView1";
            this.propertyBinding1.SetPropertyName(this.driveView1, null);
            this.driveView1.Size = new System.Drawing.Size(147, 26);
            this.driveView1.TabIndex = 0;
            this.driveView1.ViewModel = null;
            this.driveView1.ViewModelType = null;
            // 
            // fileView1
            // 
            this.fileView1.AutoCreateViewModel = false;
            this.propertyBinding1.SetDisplayFormat(this.fileView1, null);
            this.fileView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView1.Location = new System.Drawing.Point(0, 0);
            this.fileView1.Name = "fileView1";
            this.propertyBinding1.SetPropertyName(this.fileView1, null);
            this.fileView1.Size = new System.Drawing.Size(290, 244);
            this.fileView1.TabIndex = 0;
            this.fileView1.ViewModel = null;
            this.fileView1.ViewModelType = null;
            // 
            // menuStrip1
            // 
            this.propertyBinding1.SetDisplayFormat(this.menuStrip1, null);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.propertyBinding1.SetPropertyName(this.menuStrip1, null);
            this.menuStrip1.Size = new System.Drawing.Size(441, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.buttonBinding1.SetCommandName(this.fileToolStripMenuItem, null);
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.buttonBinding1.SetCommandName(this.exitToolStripMenuItem, "CloseCommand");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.propertyBinding1.SetDisplayFormat(this, null);
            this.Name = "MainView";
            this.propertyBinding1.SetPropertyName(this, null);
            this.Size = new System.Drawing.Size(441, 290);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.propertyBinding1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buttonBinding1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Clarity.Winforms.PropertyBinding propertyBinding1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblReady;
        private Clarity.Winforms.ButtonBinding buttonBinding1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private DriveView driveView1;
        private DriveStructureView driveStructureView1;
        private FileView fileView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;


    }
}
