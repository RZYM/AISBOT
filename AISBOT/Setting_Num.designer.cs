
namespace AISBOT
{
    partial class Setting_Num
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
            this.nsTheme1 = new NSTheme();
            this.nsGroupBox1 = new NSGroupBox();
            this.nsButton1 = new NSButton();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.nsControlButton1 = new NSControlButton();
            this.nsTheme1.SuspendLayout();
            this.nsGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nsTheme1
            // 
            this.nsTheme1.AccentOffset = 0;
            this.nsTheme1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.nsTheme1.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.nsTheme1.Colors = new Bloom[0];
            this.nsTheme1.Controls.Add(this.nsControlButton1);
            this.nsTheme1.Controls.Add(this.nsGroupBox1);
            this.nsTheme1.Customization = "";
            this.nsTheme1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nsTheme1.Font = new System.Drawing.Font("Verdana", 8F);
            this.nsTheme1.Image = null;
            this.nsTheme1.Location = new System.Drawing.Point(0, 0);
            this.nsTheme1.Movable = true;
            this.nsTheme1.Name = "nsTheme1";
            this.nsTheme1.NoRounding = false;
            this.nsTheme1.Sizable = true;
            this.nsTheme1.Size = new System.Drawing.Size(570, 382);
            this.nsTheme1.SmartBounds = true;
            this.nsTheme1.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            this.nsTheme1.TabIndex = 0;
            this.nsTheme1.Text = "Setting_Num";
            this.nsTheme1.TransparencyKey = System.Drawing.Color.Empty;
            this.nsTheme1.Transparent = false;
            this.nsTheme1.Click += new System.EventHandler(this.nsTheme1_Click);
            // 
            // nsGroupBox1
            // 
            this.nsGroupBox1.Controls.Add(this.nsButton1);
            this.nsGroupBox1.Controls.Add(this.richTextBox1);
            this.nsGroupBox1.DrawSeperator = false;
            this.nsGroupBox1.Location = new System.Drawing.Point(12, 34);
            this.nsGroupBox1.Name = "nsGroupBox1";
            this.nsGroupBox1.Size = new System.Drawing.Size(546, 336);
            this.nsGroupBox1.SubTitle = "";
            this.nsGroupBox1.TabIndex = 0;
            this.nsGroupBox1.Text = "nsGroupBox1";
            this.nsGroupBox1.Title = "เลขคู่ ที่ต้องการ";
            // 
            // nsButton1
            // 
            this.nsButton1.Location = new System.Drawing.Point(452, 296);
            this.nsButton1.Name = "nsButton1";
            this.nsButton1.Size = new System.Drawing.Size(75, 23);
            this.nsButton1.TabIndex = 1;
            this.nsButton1.Text = "บันทึก";
            this.nsButton1.Click += new System.EventHandler(this.nsButton1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(14, 30);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(513, 249);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "00,55,66";
            // 
            // nsControlButton1
            // 
            this.nsControlButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nsControlButton1.ControlButton = NSControlButton.Button.Close;
            this.nsControlButton1.Location = new System.Drawing.Point(542, 5);
            this.nsControlButton1.Margin = new System.Windows.Forms.Padding(0);
            this.nsControlButton1.MaximumSize = new System.Drawing.Size(18, 20);
            this.nsControlButton1.MinimumSize = new System.Drawing.Size(18, 20);
            this.nsControlButton1.Name = "nsControlButton1";
            this.nsControlButton1.Size = new System.Drawing.Size(18, 20);
            this.nsControlButton1.TabIndex = 2;
            this.nsControlButton1.Text = "nsControlButton1";
            // 
            // Setting_Num
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 382);
            this.Controls.Add(this.nsTheme1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "Setting_Num";
            this.Text = "Setting_Num";
            this.Load += new System.EventHandler(this.Setting_Num_Load);
            this.nsTheme1.ResumeLayout(false);
            this.nsGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private NSTheme nsTheme1;
        private NSGroupBox nsGroupBox1;
        private NSButton nsButton1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private NSControlButton nsControlButton1;
    }
}