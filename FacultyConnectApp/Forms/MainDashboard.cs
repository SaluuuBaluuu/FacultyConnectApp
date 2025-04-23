using FacultyConnectApp.Classes;
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
    public partial class MainDashboard : Form
    {
        private UserData currentUser;
        public MainDashboard(UserData userData)
        {
            InitializeComponent();
            currentUser = userData;
        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
