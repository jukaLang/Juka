namespace JukaPackageManager
{
    partial class Juka_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Juka_Main));
            this.label2 = new System.Windows.Forms.Label();
            this.blockchain_btn = new System.Windows.Forms.Button();
            this.packages_list = new System.Windows.Forms.ListBox();
            this.juka_link_1 = new System.Windows.Forms.LinkLabel();
            this.install_btn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.uninstall_btn = new System.Windows.Forms.Button();
            this.check_juka_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(653, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(331, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "Help out by hosting packages on blockchain:";
            // 
            // blockchain_btn
            // 
            this.blockchain_btn.FlatAppearance.BorderSize = 2;
            this.blockchain_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.blockchain_btn.Location = new System.Drawing.Point(652, 33);
            this.blockchain_btn.Name = "blockchain_btn";
            this.blockchain_btn.Size = new System.Drawing.Size(332, 52);
            this.blockchain_btn.TabIndex = 3;
            this.blockchain_btn.Text = "Host Package Blockchain";
            this.blockchain_btn.UseVisualStyleBackColor = true;
            this.blockchain_btn.Click += new System.EventHandler(this.blockchain_btn_Click);
            // 
            // packages_list
            // 
            this.packages_list.FormattingEnabled = true;
            this.packages_list.ItemHeight = 22;
            this.packages_list.Location = new System.Drawing.Point(11, 146);
            this.packages_list.Name = "packages_list";
            this.packages_list.Size = new System.Drawing.Size(288, 378);
            this.packages_list.TabIndex = 4;
            // 
            // juka_link_1
            // 
            this.juka_link_1.AutoSize = true;
            this.juka_link_1.BackColor = System.Drawing.Color.Transparent;
            this.juka_link_1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.juka_link_1.LinkColor = System.Drawing.Color.White;
            this.juka_link_1.Location = new System.Drawing.Point(775, 526);
            this.juka_link_1.Name = "juka_link_1";
            this.juka_link_1.Size = new System.Drawing.Size(209, 22);
            this.juka_link_1.TabIndex = 5;
            this.juka_link_1.TabStop = true;
            this.juka_link_1.Text = "Visit https://jukalang.com";
            // 
            // install_btn
            // 
            this.install_btn.FlatAppearance.BorderSize = 2;
            this.install_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.install_btn.Location = new System.Drawing.Point(304, 276);
            this.install_btn.Name = "install_btn";
            this.install_btn.Size = new System.Drawing.Size(60, 43);
            this.install_btn.TabIndex = 6;
            this.install_btn.Text = ">>";
            this.install_btn.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 22;
            this.listBox1.Location = new System.Drawing.Point(370, 146);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(288, 378);
            this.listBox1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(11, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 35);
            this.label1.TabIndex = 8;
            this.label1.Text = "Available:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(370, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 38);
            this.label3.TabIndex = 9;
            this.label3.Text = "Installed:";
            // 
            // uninstall_btn
            // 
            this.uninstall_btn.FlatAppearance.BorderSize = 2;
            this.uninstall_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uninstall_btn.Location = new System.Drawing.Point(304, 326);
            this.uninstall_btn.Name = "uninstall_btn";
            this.uninstall_btn.Size = new System.Drawing.Size(60, 43);
            this.uninstall_btn.TabIndex = 10;
            this.uninstall_btn.Text = "<<";
            this.uninstall_btn.UseVisualStyleBackColor = true;
            // 
            // check_juka_btn
            // 
            this.check_juka_btn.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.check_juka_btn.FlatAppearance.BorderSize = 2;
            this.check_juka_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.check_juka_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.check_juka_btn.Location = new System.Drawing.Point(11, 34);
            this.check_juka_btn.Name = "check_juka_btn";
            this.check_juka_btn.Size = new System.Drawing.Size(181, 51);
            this.check_juka_btn.TabIndex = 11;
            this.check_juka_btn.Text = "Check for Updates";
            this.check_juka_btn.UseVisualStyleBackColor = true;
            // 
            // Juka_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.ClientSize = new System.Drawing.Size(994, 557);
            this.Controls.Add(this.check_juka_btn);
            this.Controls.Add(this.uninstall_btn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.install_btn);
            this.Controls.Add(this.juka_link_1);
            this.Controls.Add(this.packages_list);
            this.Controls.Add(this.blockchain_btn);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.AliceBlue;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Juka_Main";
            this.Text = "Juka Package Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label2;
        private Button blockchain_btn;
        private ListBox packages_list;
        private LinkLabel juka_link_1;
        private Button install_btn;
        private ListBox listBox1;
        private Label label1;
        private Label label3;
        private Button uninstall_btn;
        private Button check_juka_btn;
    }
}