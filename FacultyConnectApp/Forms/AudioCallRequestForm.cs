using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacultyConnectApp.Forms
{
    public partial class AudioCallRequestForm : Form
    {
        private string callerName;
        public AudioCallRequestForm()
        {
            InitializeComponent();
            
        }

        public AudioCallRequestForm(string callerName)
        {
            InitializeComponent();
            this.callerName = callerName;

            // Set caller name in label (make sure you have this label in your form)
            if (lblCallerName != null)
                lblCallerName.Text = $"Incoming call from: {callerName}";
        }

        private void AudioCallRequestForm_Load(object sender, EventArgs e)
        {

        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();

        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();

        }

        private void lblCallerName_Click(object sender, EventArgs e)
        {

        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
