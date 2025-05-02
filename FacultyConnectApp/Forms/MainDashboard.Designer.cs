namespace FacultyConnectApp.Forms
{
    partial class MainDashboard
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
            this.btnGrantAccess = new System.Windows.Forms.Button();
            this.btnAudioCall = new System.Windows.Forms.Button();
            this.btnEndCall = new System.Windows.Forms.Button();
            this.btnVideoCall = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGrantAccess
            // 
            this.btnGrantAccess.Location = new System.Drawing.Point(347, 56);
            this.btnGrantAccess.Name = "btnGrantAccess";
            this.btnGrantAccess.Size = new System.Drawing.Size(104, 25);
            this.btnGrantAccess.TabIndex = 0;
            this.btnGrantAccess.Text = "Grant Access\r\n";
            this.btnGrantAccess.UseVisualStyleBackColor = true;
            this.btnGrantAccess.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAudioCall
            // 
            this.btnAudioCall.Location = new System.Drawing.Point(537, 56);
            this.btnAudioCall.Name = "btnAudioCall";
            this.btnAudioCall.Size = new System.Drawing.Size(104, 25);
            this.btnAudioCall.TabIndex = 2;
            this.btnAudioCall.Text = "Audio Call";
            this.btnAudioCall.UseVisualStyleBackColor = true;
            this.btnAudioCall.Click += new System.EventHandler(this.btnAudioCall_Click);
            // 
            // btnEndCall
            // 
            this.btnEndCall.Location = new System.Drawing.Point(537, 148);
            this.btnEndCall.Name = "btnEndCall";
            this.btnEndCall.Size = new System.Drawing.Size(104, 25);
            this.btnEndCall.TabIndex = 3;
            this.btnEndCall.Text = "End Call";
            this.btnEndCall.UseVisualStyleBackColor = true;
            this.btnEndCall.Visible = false;
            // 
            // btnVideoCall
            // 
            this.btnVideoCall.Location = new System.Drawing.Point(537, 100);
            this.btnVideoCall.Name = "btnVideoCall";
            this.btnVideoCall.Size = new System.Drawing.Size(104, 25);
            this.btnVideoCall.TabIndex = 4;
            this.btnVideoCall.Text = "Video Call";
            this.btnVideoCall.UseVisualStyleBackColor = true;
            this.btnVideoCall.Click += new System.EventHandler(this.btnVideoCall_Click);
            // 
            // MainDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnVideoCall);
            this.Controls.Add(this.btnEndCall);
            this.Controls.Add(this.btnAudioCall);
            this.Controls.Add(this.btnGrantAccess);
            this.Name = "MainDashboard";
            this.Text = "MainDashboard";
            this.Load += new System.EventHandler(this.MainDashboard_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGrantAccess;
        private System.Windows.Forms.Button btnAudioCall;
        private System.Windows.Forms.Button btnEndCall;
        private System.Windows.Forms.Button btnVideoCall;
    }
}