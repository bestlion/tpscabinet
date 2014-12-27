namespace tpscabinet
{
    partial class frm_notify
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
            this.lbl_Info = new System.Windows.Forms.Label();
            this.pic_Graph = new System.Windows.Forms.PictureBox();
            this.lbl_package1 = new System.Windows.Forms.Label();
            this.lbl_package2 = new System.Windows.Forms.Label();
            this.tbl_Packages = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Graph)).BeginInit();
            this.tbl_Packages.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Info
            // 
            this.lbl_Info.AutoSize = true;
            this.lbl_Info.Location = new System.Drawing.Point(91, 7);
            this.lbl_Info.Name = "lbl_Info";
            this.lbl_Info.Size = new System.Drawing.Size(40, 13);
            this.lbl_Info.TabIndex = 2;
            this.lbl_Info.Text = "InfoPnl";
            // 
            // pic_Graph
            // 
            this.pic_Graph.ErrorImage = null;
            this.pic_Graph.InitialImage = null;
            this.pic_Graph.Location = new System.Drawing.Point(10, 10);
            this.pic_Graph.Name = "pic_Graph";
            this.pic_Graph.Size = new System.Drawing.Size(75, 75);
            this.pic_Graph.TabIndex = 3;
            this.pic_Graph.TabStop = false;
            this.pic_Graph.Visible = false;
            // 
            // lbl_package1
            // 
            this.lbl_package1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_package1.AutoSize = true;
            this.lbl_package1.BackColor = System.Drawing.Color.Transparent;
            this.lbl_package1.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbl_package1.Location = new System.Drawing.Point(0, 0);
            this.lbl_package1.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_package1.Name = "lbl_package1";
            this.lbl_package1.Size = new System.Drawing.Size(55, 13);
            this.lbl_package1.TabIndex = 4;
            this.lbl_package1.Text = "package1";
            this.lbl_package1.MouseLeave += new System.EventHandler(this.lbl_package_MouseLeave);
            this.lbl_package1.MouseHover += new System.EventHandler(this.lbl_package1_MouseHover);
            // 
            // lbl_package2
            // 
            this.lbl_package2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_package2.AutoSize = true;
            this.lbl_package2.BackColor = System.Drawing.Color.Transparent;
            this.lbl_package2.ForeColor = System.Drawing.Color.DarkCyan;
            this.lbl_package2.Location = new System.Drawing.Point(120, 0);
            this.lbl_package2.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_package2.Name = "lbl_package2";
            this.lbl_package2.Size = new System.Drawing.Size(55, 13);
            this.lbl_package2.TabIndex = 5;
            this.lbl_package2.Text = "package2";
            this.lbl_package2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lbl_package2.MouseLeave += new System.EventHandler(this.lbl_package_MouseLeave);
            this.lbl_package2.MouseHover += new System.EventHandler(this.lbl_package2_MouseHover);
            // 
            // tbl_Packages
            // 
            this.tbl_Packages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbl_Packages.BackColor = System.Drawing.Color.Transparent;
            this.tbl_Packages.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbl_Packages.ColumnCount = 2;
            this.tbl_Packages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_Packages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_Packages.Controls.Add(this.lbl_package2, 1, 0);
            this.tbl_Packages.Controls.Add(this.lbl_package1, 0, 0);
            this.tbl_Packages.Location = new System.Drawing.Point(91, 73);
            this.tbl_Packages.Margin = new System.Windows.Forms.Padding(0);
            this.tbl_Packages.Name = "tbl_Packages";
            this.tbl_Packages.RowCount = 1;
            this.tbl_Packages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbl_Packages.Size = new System.Drawing.Size(175, 13);
            this.tbl_Packages.TabIndex = 6;
            // 
            // frm_notify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(273, 95);
            this.ControlBox = false;
            this.Controls.Add(this.lbl_Info);
            this.Controls.Add(this.pic_Graph);
            this.Controls.Add(this.tbl_Packages);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(220, 28);
            this.Name = "frm_notify";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Honeydew;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_notify_FormClosing);
            this.Click += new System.EventHandler(this.ctrl_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Graph)).EndInit();
            this.tbl_Packages.ResumeLayout(false);
            this.tbl_Packages.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Info;
        private System.Windows.Forms.PictureBox pic_Graph;
        private System.Windows.Forms.Label lbl_package1;
        private System.Windows.Forms.Label lbl_package2;
        private System.Windows.Forms.TableLayoutPanel tbl_Packages;
    }
}