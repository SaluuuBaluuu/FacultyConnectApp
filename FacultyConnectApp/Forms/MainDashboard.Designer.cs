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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDashboard));
            this.btnGrantAccess = new System.Windows.Forms.Button();
            this.btnAudioCall = new System.Windows.Forms.Button();
            this.btnVideoCall = new System.Windows.Forms.Button();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lstLecturers = new System.Windows.Forms.ListBox();
            this.lblFacultyConnect = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelVisitorMessage = new System.Windows.Forms.Panel();
            this.lblVisitorMessage = new System.Windows.Forms.Label();
            this.lblVisitorName = new System.Windows.Forms.Label();
            this.lblStudentNumber = new System.Windows.Forms.Label();
            this.lblPurpose = new System.Windows.Forms.Label();
            this.panelSidebar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelVisitorMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGrantAccess
            // 
            this.btnGrantAccess.Location = new System.Drawing.Point(240, 396);
            this.btnGrantAccess.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnGrantAccess.Name = "btnGrantAccess";
            this.btnGrantAccess.Size = new System.Drawing.Size(482, 51);
            this.btnGrantAccess.TabIndex = 0;
            this.btnGrantAccess.Text = "Grant Access\r\n";
            this.btnGrantAccess.UseVisualStyleBackColor = true;
            this.btnGrantAccess.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAudioCall
            // 
            this.btnAudioCall.Location = new System.Drawing.Point(18, 191);
            this.btnAudioCall.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnAudioCall.Name = "btnAudioCall";
            this.btnAudioCall.Size = new System.Drawing.Size(201, 55);
            this.btnAudioCall.TabIndex = 2;
            this.btnAudioCall.Text = "Audio Call";
            this.btnAudioCall.UseVisualStyleBackColor = true;
            this.btnAudioCall.Click += new System.EventHandler(this.btnAudioCall_Click);
            // 
            // btnVideoCall
            // 
            this.btnVideoCall.Location = new System.Drawing.Point(252, 191);
            this.btnVideoCall.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnVideoCall.Name = "btnVideoCall";
            this.btnVideoCall.Size = new System.Drawing.Size(201, 55);
            this.btnVideoCall.TabIndex = 4;
            this.btnVideoCall.Text = "View Visitor";
            this.btnVideoCall.UseVisualStyleBackColor = true;
            this.btnVideoCall.Click += new System.EventHandler(this.btnVideoCall_Click);
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelSidebar.Controls.Add(this.lblWelcome);
            this.panelSidebar.Controls.Add(this.lstLecturers);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(223, 468);
            this.panelSidebar.TabIndex = 0;
            this.panelSidebar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSidebar_Paint_1);
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.Location = new System.Drawing.Point(16, 96);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(154, 48);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Welcome back,\n Prof. Walingo";
            this.lblWelcome.Click += new System.EventHandler(this.lblWelcome_Click);
            // 
            // lstLecturers
            // 
            this.lstLecturers.BackColor = System.Drawing.Color.CornflowerBlue;
            this.lstLecturers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLecturers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstLecturers.ForeColor = System.Drawing.Color.White;
            this.lstLecturers.FormattingEnabled = true;
            this.lstLecturers.ItemHeight = 16;
            this.lstLecturers.Items.AddRange(new object[] {
            "Ernest Bhero",
            "",
            "",
            "Ray Khuboni",
            "",
            "",
            "Bashan Naidoo",
            "",
            "",
            "Tahmid Quazi",
            "",
            "",
            "Jules-Raymond Tapamo",
            "",
            "",
            "Tom Walingo"});
            this.lstLecturers.Location = new System.Drawing.Point(20, 175);
            this.lstLecturers.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.lstLecturers.Name = "lstLecturers";
            this.lstLecturers.Size = new System.Drawing.Size(157, 272);
            this.lstLecturers.TabIndex = 1;
            this.lstLecturers.SelectedIndexChanged += new System.EventHandler(this.lstLecturers_SelectedIndexChanged);
            // 
            // lblFacultyConnect
            // 
            this.lblFacultyConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFacultyConnect.AutoSize = true;
            this.lblFacultyConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacultyConnect.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.lblFacultyConnect.Location = new System.Drawing.Point(317, 30);
            this.lblFacultyConnect.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFacultyConnect.Name = "lblFacultyConnect";
            this.lblFacultyConnect.Size = new System.Drawing.Size(209, 24);
            this.lblFacultyConnect.TabIndex = 0;
            this.lblFacultyConnect.Text = "FACULTY CONNECT";
            this.lblFacultyConnect.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblFacultyConnect.Click += new System.EventHandler(this.lblFacultyConnect_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblFacultyConnect);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(752, 78);
            this.panel1.TabIndex = 5;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = global::FacultyConnectApp.Properties.Resources.uzkn;
            this.pictureBox2.Location = new System.Drawing.Point(572, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(180, 78);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(240, 10);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(71, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // panelVisitorMessage
            // 
            this.panelVisitorMessage.BackColor = System.Drawing.Color.White;
            this.panelVisitorMessage.Controls.Add(this.lblPurpose);
            this.panelVisitorMessage.Controls.Add(this.lblStudentNumber);
            this.panelVisitorMessage.Controls.Add(this.lblVisitorName);
            this.panelVisitorMessage.Controls.Add(this.lblVisitorMessage);
            this.panelVisitorMessage.Controls.Add(this.btnAudioCall);
            this.panelVisitorMessage.Controls.Add(this.btnVideoCall);
            this.panelVisitorMessage.Location = new System.Drawing.Point(240, 96);
            this.panelVisitorMessage.Name = "panelVisitorMessage";
            this.panelVisitorMessage.Size = new System.Drawing.Size(482, 273);
            this.panelVisitorMessage.TabIndex = 6;
            // 
            // lblVisitorMessage
            // 
            this.lblVisitorMessage.AutoSize = true;
            this.lblVisitorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVisitorMessage.Location = new System.Drawing.Point(14, 12);
            this.lblVisitorMessage.Name = "lblVisitorMessage";
            this.lblVisitorMessage.Size = new System.Drawing.Size(143, 24);
            this.lblVisitorMessage.TabIndex = 0;
            this.lblVisitorMessage.Text = "Visitor Message";
            // 
            // lblVisitorName
            // 
            this.lblVisitorName.AutoSize = true;
            this.lblVisitorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVisitorName.Location = new System.Drawing.Point(97, 58);
            this.lblVisitorName.Name = "lblVisitorName";
            this.lblVisitorName.Size = new System.Drawing.Size(87, 16);
            this.lblVisitorName.TabIndex = 5;
            this.lblVisitorName.Text = "Visitor Name:";
            // 
            // lblStudentNumber
            // 
            this.lblStudentNumber.AutoSize = true;
            this.lblStudentNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStudentNumber.Location = new System.Drawing.Point(97, 98);
            this.lblStudentNumber.Name = "lblStudentNumber";
            this.lblStudentNumber.Size = new System.Drawing.Size(106, 16);
            this.lblStudentNumber.TabIndex = 6;
            this.lblStudentNumber.Text = "Student Number:";
            // 
            // lblPurpose
            // 
            this.lblPurpose.AutoSize = true;
            this.lblPurpose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPurpose.Location = new System.Drawing.Point(97, 135);
            this.lblPurpose.Name = "lblPurpose";
            this.lblPurpose.Size = new System.Drawing.Size(100, 16);
            this.lblPurpose.TabIndex = 7;
            this.lblPurpose.Text = "Reason of Visit:";
            // 
            // MainDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(750, 468);
            this.Controls.Add(this.panelVisitorMessage);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.btnGrantAccess);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "MainDashboard";
            this.Text = "Faculty Connect – Dashboard";
            this.Load += new System.EventHandler(this.MainDashboard_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelVisitorMessage.ResumeLayout(false);
            this.panelVisitorMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGrantAccess;
        private System.Windows.Forms.Button btnAudioCall;
        private System.Windows.Forms.Button btnVideoCall;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Label lblFacultyConnect;
        private System.Windows.Forms.ListBox lstLecturers;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panelVisitorMessage;
        private System.Windows.Forms.Label lblVisitorMessage;
        private System.Windows.Forms.Label lblVisitorName;
        private System.Windows.Forms.Label lblPurpose;
        private System.Windows.Forms.Label lblStudentNumber;
    }
}