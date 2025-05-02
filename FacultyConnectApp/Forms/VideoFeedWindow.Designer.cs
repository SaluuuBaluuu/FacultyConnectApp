namespace FacultyConnectApp.Forms

{
    partial class VideoFeedWindow
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
            this.videoBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.btnEndCall = new System.Windows.Forms.Button();
            this.btnSwitchToAudio = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.videoBrowser)).BeginInit();
            this.panelTopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // videoBrowser
            // 
            this.videoBrowser.AllowExternalDrop = true;
            this.videoBrowser.CreationProperties = null;
            this.videoBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
            this.videoBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoBrowser.Location = new System.Drawing.Point(0, 40);
            this.videoBrowser.Name = "videoBrowser";
            this.videoBrowser.Size = new System.Drawing.Size(784, 421);
            this.videoBrowser.TabIndex = 0;
            this.videoBrowser.ZoomFactor = 1D;
            this.videoBrowser.Click += new System.EventHandler(this.videoBrowser_Click);
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panelTopBar.Controls.Add(this.btnEndCall);
            this.panelTopBar.Controls.Add(this.btnSwitchToAudio);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(784, 40);
            this.panelTopBar.TabIndex = 1;
            // 
            // btnEndCall
            // 
            this.btnEndCall.ForeColor = System.Drawing.Color.Black;
            this.btnEndCall.Location = new System.Drawing.Point(670, 8);
            this.btnEndCall.Name = "btnEndCall";
            this.btnEndCall.Size = new System.Drawing.Size(85, 25);
            this.btnEndCall.TabIndex = 2;
            this.btnEndCall.Text = "End Call";
            this.btnEndCall.UseVisualStyleBackColor = true;
            this.btnEndCall.Click += new System.EventHandler(this.btnEndCall_Click);
            // 
            // btnSwitchToAudio
            // 
            this.btnSwitchToAudio.ForeColor = System.Drawing.Color.Black;
            this.btnSwitchToAudio.Location = new System.Drawing.Point(560, 8);
            this.btnSwitchToAudio.Name = "btnSwitchToAudio";
            this.btnSwitchToAudio.Size = new System.Drawing.Size(100, 25);
            this.btnSwitchToAudio.TabIndex = 3;
            this.btnSwitchToAudio.Text = "Switch to Audio";
            this.btnSwitchToAudio.UseVisualStyleBackColor = true;
            this.btnSwitchToAudio.Click += new System.EventHandler(this.btnSwitchToAudio_Click);
            // 
            // VideoFeedWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.videoBrowser);
            this.Controls.Add(this.panelTopBar);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "VideoFeedWindow";
            this.Text = "Faculty Connect – Live Feed";
            this.Load += new System.EventHandler(this.VideoFeedWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.videoBrowser)).EndInit();
            this.panelTopBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSwitchToAudio;
        private System.Windows.Forms.Button btnEndCall;
        private Microsoft.Web.WebView2.WinForms.WebView2 videoBrowser;
        private System.Windows.Forms.Panel panelTopBar;


    }
}
// 