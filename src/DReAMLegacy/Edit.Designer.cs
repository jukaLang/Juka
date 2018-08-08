namespace DReAMLegacy
{
    partial class Edit
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.close_btn = new System.Windows.Forms.Label();
            this.minimize_btn = new System.Windows.Forms.Label();
            this.window_lbl = new System.Windows.Forms.Label();
            this.textPanel = new System.Windows.Forms.Panel();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outdentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findAndReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordWrapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showIndentGuidesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWhitespaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.run_btn = new System.Windows.Forms.Button();
            this.output_txt = new System.Windows.Forms.RichTextBox();
            this.output_lbl = new System.Windows.Forms.Label();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // close_btn
            // 
            this.close_btn.AutoSize = true;
            this.close_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.close_btn.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close_btn.ForeColor = System.Drawing.Color.White;
            this.close_btn.Location = new System.Drawing.Point(1005, -2);
            this.close_btn.Name = "close_btn";
            this.close_btn.Size = new System.Drawing.Size(22, 24);
            this.close_btn.TabIndex = 3;
            this.close_btn.Text = "x";
            this.close_btn.Click += new System.EventHandler(this.close_btn_Click);
            // 
            // minimize_btn
            // 
            this.minimize_btn.AutoSize = true;
            this.minimize_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.minimize_btn.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimize_btn.ForeColor = System.Drawing.Color.White;
            this.minimize_btn.Location = new System.Drawing.Point(977, -2);
            this.minimize_btn.Name = "minimize_btn";
            this.minimize_btn.Size = new System.Drawing.Size(22, 24);
            this.minimize_btn.TabIndex = 2;
            this.minimize_btn.Text = "-";
            this.minimize_btn.Click += new System.EventHandler(this.minimize_btn_Click);
            // 
            // window_lbl
            // 
            this.window_lbl.Location = new System.Drawing.Point(12, 9);
            this.window_lbl.Name = "window_lbl";
            this.window_lbl.Size = new System.Drawing.Size(108, 29);
            this.window_lbl.TabIndex = 7;
            this.window_lbl.Text = "Edit";
            // 
            // textPanel
            // 
            this.textPanel.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.textPanel.AllowDrop = true;
            this.textPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.textPanel.ForeColor = System.Drawing.Color.Transparent;
            this.textPanel.Location = new System.Drawing.Point(1, 77);
            this.textPanel.Name = "textPanel";
            this.textPanel.Size = new System.Drawing.Size(813, 428);
            this.textPanel.TabIndex = 8;
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.menu.Dock = System.Windows.Forms.DockStyle.None;
            this.menu.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.editorMenu});
            this.menu.Location = new System.Drawing.Point(16, 38);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(281, 29);
            this.menu.TabIndex = 9;
            this.menu.Text = "Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 25);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.saveAsToolStripMenuItem.Text = "Save As";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem,
            this.indentToolStripMenuItem,
            this.outdentToolStripMenuItem});
            this.editToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(48, 25);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.selectAllToolStripMenuItem.Text = "Select All";
            // 
            // indentToolStripMenuItem
            // 
            this.indentToolStripMenuItem.Name = "indentToolStripMenuItem";
            this.indentToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.indentToolStripMenuItem.Text = "Indent";
            // 
            // outdentToolStripMenuItem
            // 
            this.outdentToolStripMenuItem.Name = "outdentToolStripMenuItem";
            this.outdentToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.outdentToolStripMenuItem.Text = "Outdent";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findAndReplaceToolStripMenuItem,
            this.toolStripSeparator4,
            this.goToLineToolStripMenuItem});
            this.searchToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(69, 25);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.findToolStripMenuItem.Text = "Find";
            // 
            // findAndReplaceToolStripMenuItem
            // 
            this.findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            this.findAndReplaceToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.findAndReplaceToolStripMenuItem.Text = "Find and Replace";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(195, 6);
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            this.goToLineToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.goToLineToolStripMenuItem.Text = "Go to Line...";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordWrapToolStripMenuItem,
            this.showIndentGuidesToolStripMenuItem,
            this.showWhitespaceToolStripMenuItem,
            this.toolStripSeparator2,
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.toolStripSeparator3,
            this.collapseAllToolStripMenuItem,
            this.expandAllToolStripMenuItem});
            this.viewToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // wordWrapToolStripMenuItem
            // 
            this.wordWrapToolStripMenuItem.Name = "wordWrapToolStripMenuItem";
            this.wordWrapToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.wordWrapToolStripMenuItem.Text = "Word Wrap";
            // 
            // showIndentGuidesToolStripMenuItem
            // 
            this.showIndentGuidesToolStripMenuItem.Name = "showIndentGuidesToolStripMenuItem";
            this.showIndentGuidesToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.showIndentGuidesToolStripMenuItem.Text = "Show Indent Guides";
            // 
            // showWhitespaceToolStripMenuItem
            // 
            this.showWhitespaceToolStripMenuItem.Name = "showWhitespaceToolStripMenuItem";
            this.showWhitespaceToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.showWhitespaceToolStripMenuItem.Text = "Show Whitespace";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(216, 6);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            // 
            // editorMenu
            // 
            this.editorMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.editorMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editorMenu.ForeColor = System.Drawing.Color.White;
            this.editorMenu.Name = "editorMenu";
            this.editorMenu.Size = new System.Drawing.Size(54, 25);
            this.editorMenu.Text = "Help";
            // 
            // run_btn
            // 
            this.run_btn.BackColor = System.Drawing.Color.Transparent;
            this.run_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.run_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.run_btn.FlatAppearance.BorderSize = 0;
            this.run_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.run_btn.Location = new System.Drawing.Point(336, 38);
            this.run_btn.Name = "run_btn";
            this.run_btn.Size = new System.Drawing.Size(270, 29);
            this.run_btn.TabIndex = 10;
            this.run_btn.Text = "Run";
            this.run_btn.UseVisualStyleBackColor = false;
            this.run_btn.Click += new System.EventHandler(this.run_btn_Click);
            // 
            // output_txt
            // 
            this.output_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.output_txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.output_txt.ForeColor = System.Drawing.SystemColors.Info;
            this.output_txt.Location = new System.Drawing.Point(821, 77);
            this.output_txt.Name = "output_txt";
            this.output_txt.Size = new System.Drawing.Size(206, 428);
            this.output_txt.TabIndex = 11;
            this.output_txt.Text = "";
            // 
            // output_lbl
            // 
            this.output_lbl.AutoSize = true;
            this.output_lbl.Location = new System.Drawing.Point(817, 43);
            this.output_lbl.Name = "output_lbl";
            this.output_lbl.Size = new System.Drawing.Size(94, 24);
            this.output_lbl.TabIndex = 12;
            this.output_lbl.Text = "Output:";
            // 
            // Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(1028, 504);
            this.Controls.Add(this.output_lbl);
            this.Controls.Add(this.output_txt);
            this.Controls.Add(this.run_btn);
            this.Controls.Add(this.textPanel);
            this.Controls.Add(this.window_lbl);
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.minimize_btn);
            this.Controls.Add(this.menu);
            this.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menu;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Edit";
            this.Opacity = 0.9D;
            this.Text = "Edit";
            this.Load += new System.EventHandler(this.Edit_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Edit_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Edit_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Edit_MouseUp);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label close_btn;
        private System.Windows.Forms.Label minimize_btn;
        private System.Windows.Forms.Label window_lbl;
        private System.Windows.Forms.Panel textPanel;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editorMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outdentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAndReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wordWrapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showIndentGuidesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWhitespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.Button run_btn;
        private System.Windows.Forms.RichTextBox output_txt;
        private System.Windows.Forms.Label output_lbl;
    }
}