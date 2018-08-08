namespace DReAMLegacy
{
    partial class Contact
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
            this.twitter_lbl = new System.Windows.Forms.Label();
            this.email_lbl = new System.Windows.Forms.Label();
            this.documentation_lbl = new System.Windows.Forms.Label();
            this.website_lbl = new System.Windows.Forms.Label();
            this.website_link = new System.Windows.Forms.LinkLabel();
            this.documentation_link = new System.Windows.Forms.LinkLabel();
            this.twitter_link = new System.Windows.Forms.LinkLabel();
            this.email_link = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // close_btn
            // 
            this.close_btn.AutoSize = true;
            this.close_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.close_btn.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close_btn.ForeColor = System.Drawing.Color.White;
            this.close_btn.Location = new System.Drawing.Point(652, -2);
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
            this.minimize_btn.Location = new System.Drawing.Point(624, -2);
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
            this.window_lbl.Size = new System.Drawing.Size(143, 29);
            this.window_lbl.TabIndex = 7;
            this.window_lbl.Text = "Contact";
            // 
            // twitter_lbl
            // 
            this.twitter_lbl.AutoSize = true;
            this.twitter_lbl.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.twitter_lbl.Location = new System.Drawing.Point(12, 223);
            this.twitter_lbl.Name = "twitter_lbl";
            this.twitter_lbl.Size = new System.Drawing.Size(230, 33);
            this.twitter_lbl.TabIndex = 8;
            this.twitter_lbl.Text = "Follow Twitter: ";
            // 
            // email_lbl
            // 
            this.email_lbl.AutoSize = true;
            this.email_lbl.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.email_lbl.Location = new System.Drawing.Point(10, 288);
            this.email_lbl.Name = "email_lbl";
            this.email_lbl.Size = new System.Drawing.Size(154, 33);
            this.email_lbl.TabIndex = 9;
            this.email_lbl.Text = "E-mail Us:";
            // 
            // documentation_lbl
            // 
            this.documentation_lbl.AutoSize = true;
            this.documentation_lbl.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.documentation_lbl.Location = new System.Drawing.Point(10, 159);
            this.documentation_lbl.Name = "documentation_lbl";
            this.documentation_lbl.Size = new System.Drawing.Size(240, 33);
            this.documentation_lbl.TabIndex = 10;
            this.documentation_lbl.Text = "Documentation: ";
            // 
            // website_lbl
            // 
            this.website_lbl.AutoSize = true;
            this.website_lbl.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.website_lbl.Location = new System.Drawing.Point(10, 98);
            this.website_lbl.Name = "website_lbl";
            this.website_lbl.Size = new System.Drawing.Size(246, 33);
            this.website_lbl.TabIndex = 11;
            this.website_lbl.Text = "Official Website: ";
            // 
            // website_link
            // 
            this.website_link.ActiveLinkColor = System.Drawing.Color.White;
            this.website_link.AutoSize = true;
            this.website_link.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.website_link.LinkColor = System.Drawing.Color.Silver;
            this.website_link.Location = new System.Drawing.Point(262, 106);
            this.website_link.Name = "website_link";
            this.website_link.Size = new System.Drawing.Size(228, 26);
            this.website_link.TabIndex = 12;
            this.website_link.TabStop = true;
            this.website_link.Text = "https://juliar.org";
            this.website_link.VisitedLinkColor = System.Drawing.Color.Silver;
            this.website_link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.website_link_LinkClicked);
            // 
            // documentation_link
            // 
            this.documentation_link.ActiveLinkColor = System.Drawing.Color.White;
            this.documentation_link.AutoSize = true;
            this.documentation_link.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.documentation_link.LinkColor = System.Drawing.Color.Silver;
            this.documentation_link.Location = new System.Drawing.Point(262, 167);
            this.documentation_link.Name = "documentation_link";
            this.documentation_link.Size = new System.Drawing.Size(396, 26);
            this.documentation_link.TabIndex = 13;
            this.documentation_link.TabStop = true;
            this.documentation_link.Text = "https://juliar.org/documentation";
            this.documentation_link.VisitedLinkColor = System.Drawing.Color.Silver;
            this.documentation_link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.documentation_link_LinkClicked);
            // 
            // twitter_link
            // 
            this.twitter_link.ActiveLinkColor = System.Drawing.Color.White;
            this.twitter_link.AutoSize = true;
            this.twitter_link.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.twitter_link.LinkColor = System.Drawing.Color.Silver;
            this.twitter_link.Location = new System.Drawing.Point(262, 231);
            this.twitter_link.Name = "twitter_link";
            this.twitter_link.Size = new System.Drawing.Size(372, 26);
            this.twitter_link.TabIndex = 14;
            this.twitter_link.TabStop = true;
            this.twitter_link.Text = "https://twitter.com/juliarLang";
            this.twitter_link.VisitedLinkColor = System.Drawing.Color.Silver;
            this.twitter_link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.twitter_link_LinkClicked);
            // 
            // email_link
            // 
            this.email_link.ActiveLinkColor = System.Drawing.Color.White;
            this.email_link.AutoSize = true;
            this.email_link.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.email_link.LinkColor = System.Drawing.Color.Silver;
            this.email_link.Location = new System.Drawing.Point(262, 296);
            this.email_link.Name = "email_link";
            this.email_link.Size = new System.Drawing.Size(204, 26);
            this.email_link.TabIndex = 15;
            this.email_link.TabStop = true;
            this.email_link.Text = "admin@juliar.org";
            this.email_link.VisitedLinkColor = System.Drawing.Color.Silver;
            this.email_link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.email_link_LinkClicked);
            // 
            // Contact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(677, 369);
            this.Controls.Add(this.email_link);
            this.Controls.Add(this.twitter_link);
            this.Controls.Add(this.documentation_link);
            this.Controls.Add(this.website_link);
            this.Controls.Add(this.website_lbl);
            this.Controls.Add(this.documentation_lbl);
            this.Controls.Add(this.email_lbl);
            this.Controls.Add(this.twitter_lbl);
            this.Controls.Add(this.window_lbl);
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.minimize_btn);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Contact";
            this.Opacity = 0.9D;
            this.Text = "Communicate";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Communicate_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Communicate_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Communicate_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label close_btn;
        private System.Windows.Forms.Label minimize_btn;
        private System.Windows.Forms.Label window_lbl;
        private System.Windows.Forms.Label twitter_lbl;
        private System.Windows.Forms.Label email_lbl;
        private System.Windows.Forms.Label documentation_lbl;
        private System.Windows.Forms.Label website_lbl;
        private System.Windows.Forms.LinkLabel website_link;
        private System.Windows.Forms.LinkLabel documentation_link;
        private System.Windows.Forms.LinkLabel twitter_link;
        private System.Windows.Forms.LinkLabel email_link;
    }
}