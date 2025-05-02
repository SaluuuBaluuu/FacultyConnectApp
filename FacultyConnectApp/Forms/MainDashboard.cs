using FacultyConnectApp.Classes;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FacultyConnectApp.Services;
using FacultyConnectApp.Models;
using System.Threading.Tasks;

namespace FacultyConnectApp.Forms
{
    public partial class MainDashboard : Form
    {
        private UserData currentUser;
        private string lecturerName;
        private FirebaseService firebaseService;

        public MainDashboard(UserData userData)
        {
            InitializeComponent();
            currentUser = userData;
        }

        private async void MainDashboard_Load(object sender, EventArgs e)
        {
            LoadLecturerIdentity();

            firebaseService = new FirebaseService(this);

            if (!string.IsNullOrEmpty(lecturerName))
            {
                await firebaseService.ListenForRequests(lecturerName);
            }
            else
            {
                MessageBox.Show("Lecturer name not loaded. Firebase listener not started.");
            }
        }

        private void LoadLecturerIdentity()
        {
            try
            {
                var configText = File.ReadAllText("config.json");
                var config = JsonConvert.DeserializeObject<Config>(configText);
                lecturerName = config.lecturer_name;
                Console.WriteLine($"Loaded lecturer: {lecturerName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading lecturer config: " + ex.Message);
            }
        }

        public void UpdateVisitorMessageOnDashboard(VisitorRequest request)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateVisitorMessageOnDashboard(request)));
                return;
            }

            lblVisitorName.Text = "Visitor: " + request.visitor_name;
            lblStudentNumber.Text = "Student #: " + request.student_number;
            lblPurpose.Text = "Purpose: " + request.purpose;
        }

        private void VMainDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            firebaseService?.StopListening();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var data = new { access_granted = true };
            var json = JsonConvert.SerializeObject(data);

            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string firebaseUrl = "https://facultyconnectdb-default-rtdb.asia-southeast1.firebasedatabase.app/door_control.json";

                var response = await client.PutAsync(firebaseUrl, content);
                MessageBox.Show(response.IsSuccessStatusCode ? "Access granted!" : "Failed to send access signal.");
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

        public class Config
        {
            public string lecturer_name { get; set; }
        }

        private void panelSidebar_Paint(object sender, PaintEventArgs e) { }
        private void lstLecturers_SelectedIndexChanged(object sender, EventArgs e) { }
        private void panelSidebar_Paint_1(object sender, PaintEventArgs e) { }
        private void lblWelcome_Click(object sender, EventArgs e) { }
        private void lblFacultyConnect_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void btnEndCall_Click(object sender, EventArgs e) { }
    }
}

