using FacultyConnectApp.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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

        private async void button1_Click(object sender, EventArgs e)
        {
            var data = new { access_granted = true };
            var json = JsonConvert.SerializeObject(data);

            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string firebaseUrl = "https://facultyconnectdb-default-rtdb.asia-southeast1.firebasedatabase.app/door_control.json"; // Update this
                var response = await client.PutAsync(firebaseUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Access granted!");
                }
                else
                {
                    MessageBox.Show("Failed to send access signal.");
                }
            }

        }

        private void btnVideoCall_Click(object sender, EventArgs e)
        {
            var videoWindow = new VideoFeedWindow();
            videoWindow.Show();

        }

        private void btnAudioCall_Click(object sender, EventArgs e)
        {
            var audioWindow = new AudioCallWindow();
            audioWindow.Show();
        }
    }
}
