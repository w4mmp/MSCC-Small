namespace OmniaGUI
{
    partial class MonitorForm
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
            this.Monitortxt = new System.Windows.Forms.TextBox();
            this.Heartbeatcheckbox = new System.Windows.Forms.CheckBox();
            this.MonitorPausecheckBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Monitortxt
            // 
            this.Monitortxt.AcceptsReturn = true;
            this.Monitortxt.BackColor = System.Drawing.Color.Black;
            this.Monitortxt.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Monitortxt.ForeColor = System.Drawing.Color.Lime;
            this.Monitortxt.Location = new System.Drawing.Point(24, 65);
            this.Monitortxt.Multiline = true;
            this.Monitortxt.Name = "Monitortxt";
            this.Monitortxt.ReadOnly = true;
            this.Monitortxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Monitortxt.Size = new System.Drawing.Size(472, 336);
            this.Monitortxt.TabIndex = 9;
            this.Monitortxt.TextChanged += new System.EventHandler(this.Monitortxt_TextChanged);
            // 
            // Heartbeatcheckbox
            // 
            this.Heartbeatcheckbox.AutoSize = true;
            this.Heartbeatcheckbox.BackColor = System.Drawing.Color.White;
            this.Heartbeatcheckbox.Location = new System.Drawing.Point(55, 32);
            this.Heartbeatcheckbox.Name = "Heartbeatcheckbox";
            this.Heartbeatcheckbox.Size = new System.Drawing.Size(82, 17);
            this.Heartbeatcheckbox.TabIndex = 10;
            this.Heartbeatcheckbox.Text = "Display ALL";
            this.Heartbeatcheckbox.UseVisualStyleBackColor = false;
            this.Heartbeatcheckbox.CheckedChanged += new System.EventHandler(this.Heartbeatcheckbox_CheckedChanged);
            // 
            // MonitorPausecheckBox1
            // 
            this.MonitorPausecheckBox1.AutoSize = true;
            this.MonitorPausecheckBox1.BackColor = System.Drawing.Color.White;
            this.MonitorPausecheckBox1.Location = new System.Drawing.Point(196, 32);
            this.MonitorPausecheckBox1.Name = "MonitorPausecheckBox1";
            this.MonitorPausecheckBox1.Size = new System.Drawing.Size(94, 17);
            this.MonitorPausecheckBox1.TabIndex = 11;
            this.MonitorPausecheckBox1.Text = "Pause Monitor";
            this.MonitorPausecheckBox1.UseVisualStyleBackColor = false;
            this.MonitorPausecheckBox1.CheckedChanged += new System.EventHandler(this.MonitorPausecheckBox1_CheckedChanged);
            // 
            // MonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(521, 404);
            this.ControlBox = false;
            this.Controls.Add(this.MonitorPausecheckBox1);
            this.Controls.Add(this.Heartbeatcheckbox);
            this.Controls.Add(this.Monitortxt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MonitorForm";
            this.ShowIcon = false;
            this.Text = "Monitor";
            this.Load += new System.EventHandler(this.MonitorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox Monitortxt;
        private System.Windows.Forms.CheckBox Heartbeatcheckbox;
        private System.Windows.Forms.CheckBox MonitorPausecheckBox1;
    }
}