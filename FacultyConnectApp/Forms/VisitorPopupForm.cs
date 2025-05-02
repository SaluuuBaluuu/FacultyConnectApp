using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FacultyConnectApp.Models;
using FacultyConnectApp.Services;
using Newtonsoft.Json;
using static Google.Rpc.Context.AttributeContext.Types;

namespace FacultyConnectApp.Forms
{
    public partial class VisitorPopupForm : Form
    {
        private VisitorRequest visitorRequest;
        public VisitorPopupForm(VisitorRequest request)
        {
            InitializeComponent();
            this.TopMost = true;
            this.visitorRequest = request;
            lblVisitorName.Text = $"Visitor: {request.visitor_name}";
            lblStudentNumber.Text = $"Student #: {request.student_number}";
            lblPurpose.Text = $"Purpose: {request.purpose}";
        }

        private void VisitorPopupForm_Load(object sender, EventArgs e)
        {


        }
        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void VisitorPopupForm_Load_1(object sender, EventArgs e)
        {

        }

        private void lblStudentNumber_Click(object sender, EventArgs e)
        {

        }
    }
}
