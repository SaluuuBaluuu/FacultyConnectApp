namespace FacultyConnectApp.Forms
{
    partial class VisitorPopupForm
    {
        private System.ComponentModel.IContainer components = null;

        // Declare Controls
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVisitorName;
        private System.Windows.Forms.Label lblStudentNumber;
        private System.Windows.Forms.Label lblPurpose;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;

        /// <summary>
        /// Clean up resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVisitorName = new System.Windows.Forms.Label();
            this.lblStudentNumber = new System.Windows.Forms.Label();
            this.lblPurpose = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(9, 7);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(270, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Visitor Request";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVisitorName
            // 
            this.lblVisitorName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblVisitorName.Location = new System.Drawing.Point(9, 49);
            this.lblVisitorName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVisitorName.Name = "lblVisitorName";
            this.lblVisitorName.Size = new System.Drawing.Size(270, 20);
            this.lblVisitorName.TabIndex = 1;
            this.lblVisitorName.Text = "Visitor: ";
            // 
            // lblStudentNumber
            // 
            this.lblStudentNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblStudentNumber.Location = new System.Drawing.Point(9, 77);
            this.lblStudentNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStudentNumber.Name = "lblStudentNumber";
            this.lblStudentNumber.Size = new System.Drawing.Size(270, 20);
            this.lblStudentNumber.TabIndex = 2;
            this.lblStudentNumber.Text = "Student #: ";
            this.lblStudentNumber.Click += new System.EventHandler(this.lblStudentNumber_Click);
            // 
            // lblPurpose
            // 
            this.lblPurpose.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPurpose.Location = new System.Drawing.Point(9, 106);
            this.lblPurpose.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPurpose.Name = "lblPurpose";
            this.lblPurpose.Size = new System.Drawing.Size(270, 20);
            this.lblPurpose.TabIndex = 3;
            this.lblPurpose.Text = "Purpose: ";
            // 
            // btnYes
            // 
            this.btnYes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnYes.Location = new System.Drawing.Point(45, 146);
            this.btnYes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 28);
            this.btnYes.TabIndex = 4;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNo.Location = new System.Drawing.Point(165, 146);
            this.btnNo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 28);
            this.btnNo.TabIndex = 5;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // VisitorPopupForm
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(288, 196);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lblPurpose);
            this.Controls.Add(this.lblStudentNumber);
            this.Controls.Add(this.lblVisitorName);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VisitorPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visitor Check-In";
            this.ResumeLayout(false);

        }

        #endregion
    }
}

