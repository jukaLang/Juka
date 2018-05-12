namespace DReAM
{
    partial class Chat
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
            this.chat_input = new System.Windows.Forms.TextBox();
            this.chat_output = new System.Windows.Forms.RichTextBox();
            this.send_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // close_btn
            // 
            this.close_btn.AutoSize = true;
            this.close_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.close_btn.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close_btn.ForeColor = System.Drawing.Color.White;
            this.close_btn.Location = new System.Drawing.Point(648, -2);
            this.close_btn.Name = "close_btn";
            this.close_btn.Size = new System.Drawing.Size(22, 24);
            this.close_btn.TabIndex = 5;
            this.close_btn.Text = "x";
            this.close_btn.Click += new System.EventHandler(this.close_btn_Click);
            // 
            // minimize_btn
            // 
            this.minimize_btn.AutoSize = true;
            this.minimize_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.minimize_btn.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimize_btn.ForeColor = System.Drawing.Color.White;
            this.minimize_btn.Location = new System.Drawing.Point(620, -2);
            this.minimize_btn.Name = "minimize_btn";
            this.minimize_btn.Size = new System.Drawing.Size(22, 24);
            this.minimize_btn.TabIndex = 4;
            this.minimize_btn.Text = "-";
            this.minimize_btn.Click += new System.EventHandler(this.minimize_btn_Click);
            // 
            // window_lbl
            // 
            this.window_lbl.Location = new System.Drawing.Point(12, 9);
            this.window_lbl.Name = "window_lbl";
            this.window_lbl.Size = new System.Drawing.Size(108, 29);
            this.window_lbl.TabIndex = 6;
            this.window_lbl.Text = "Chat";
            // 
            // chat_input
            // 
            this.chat_input.AllowDrop = true;
            this.chat_input.Location = new System.Drawing.Point(12, 432);
            this.chat_input.Name = "chat_input";
            this.chat_input.Size = new System.Drawing.Size(518, 32);
            this.chat_input.TabIndex = 7;
            // 
            // chat_output
            // 
            this.chat_output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.chat_output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chat_output.EnableAutoDragDrop = true;
            this.chat_output.Location = new System.Drawing.Point(16, 53);
            this.chat_output.Name = "chat_output";
            this.chat_output.ReadOnly = true;
            this.chat_output.Size = new System.Drawing.Size(641, 359);
            this.chat_output.TabIndex = 8;
            this.chat_output.Text = "";
            // 
            // send_btn
            // 
            this.send_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.send_btn.Location = new System.Drawing.Point(553, 432);
            this.send_btn.Name = "send_btn";
            this.send_btn.Size = new System.Drawing.Size(104, 32);
            this.send_btn.TabIndex = 9;
            this.send_btn.Text = "Send";
            this.send_btn.UseVisualStyleBackColor = false;
            this.send_btn.Click += new System.EventHandler(this.send_btn_Click);
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(669, 497);
            this.Controls.Add(this.send_btn);
            this.Controls.Add(this.chat_output);
            this.Controls.Add(this.chat_input);
            this.Controls.Add(this.window_lbl);
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.minimize_btn);
            this.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Chat";
            this.Opacity = 0.9D;
            this.Text = "Settings";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Settings_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Settings_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Settings_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label close_btn;
        private System.Windows.Forms.Label minimize_btn;
        private System.Windows.Forms.Label window_lbl;
        private System.Windows.Forms.TextBox chat_input;
        private System.Windows.Forms.RichTextBox chat_output;
        private System.Windows.Forms.Button send_btn;
    }
}