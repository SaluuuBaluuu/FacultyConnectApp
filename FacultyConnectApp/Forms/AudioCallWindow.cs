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
    public partial class AudioCallWindow : Form
    {
        public AudioCallWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();

        }

        private void AudioCallWindow_Load(object sender, EventArgs e)
        {
            StartRealAudioStream();
        }

        private void StartRealAudioStream()
        {
            // 🚧 Will be replaced with actual streaming code later
            label1.Text = "Connected. Awaiting audio...";
        }
    }
}
