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
using System.Diagnostics;
using System.Collections.Generic;

namespace FacultyConnectApp.Forms
{
    public partial class MainDashboard : Form
    {
        private UserData currentUser;
        private string lecturerName;
        private FirebaseService firebaseService;
        private bool isConnected = false;

        public MainDashboard(UserData userData)
        {
            InitializeComponent();
            currentUser = userData;
        }

        private async void MainDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                // Set initial status
                UpdateConnectionStatus("Initializing...");

                // Load lecturer identity from config
                LoadLecturerIdentity();

                // Initialize Firebase service
                firebaseService = new FirebaseService(this);

                if (!string.IsNullOrEmpty(lecturerName))
                {
                    // First test the connection
                    Debug.WriteLine("Testing Firebase connection...");
                    UpdateConnectionStatus($"Testing connection for {lecturerName}...");
                    await firebaseService.TestFirebaseConnection(lecturerName);

                    // Then start listening for requests
                    Debug.WriteLine("Starting Firebase listener...");
                    UpdateConnectionStatus($"Starting listener for {lecturerName}...");
                    await firebaseService.ListenForRequests(lecturerName);

                    // Update status to show we're listening
                    isConnected = true;
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                    Debug.WriteLine($"Firebase listener started for {lecturerName}");
                }
                else
                {
                    MessageBox.Show("Lecturer name not loaded. Firebase listener not started.",
                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UpdateConnectionStatus("Not connected - lecturer name missing");
                    Debug.WriteLine("Failed to start Firebase listener: Lecturer name is empty");
                }

                // Update welcome label with lecturer name if available
                if (!string.IsNullOrEmpty(lecturerName))
                {
                    lblWelcome.Text = $"Welcome, {lecturerName}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainDashboard_Load: {ex.Message}");
                MessageBox.Show($"Error initializing dashboard: {ex.Message}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to initialize");
            }
        }

        private void LoadLecturerIdentity()
        {
            try
            {
                if (File.Exists("config.json"))
                {
                    var configText = File.ReadAllText("config.json");
                    var config = JsonConvert.DeserializeObject<Config>(configText);
                    lecturerName = config.lecturer_name;
                    Debug.WriteLine($"Loaded lecturer: {lecturerName}");
                }
                else
                {
                    Debug.WriteLine("Config file not found");
                    MessageBox.Show("Config file (config.json) not found. Please create it with a lecturer_name field.",
                        "Configuration Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading lecturer config: {ex.Message}");
                MessageBox.Show("Error reading lecturer config: " + ex.Message,
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateVisitorMessageOnDashboard(VisitorRequest request)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateVisitorMessageOnDashboard(request)));
                return;
            }

            try
            {
                Debug.WriteLine($"Updating dashboard with visitor: {request.visitor_name}");
                lblVisitorName.Text = "Visitor: " + request.visitor_name;
                lblStudentNumber.Text = "Student #: " + request.student_number;
                lblPurpose.Text = "Purpose: " + request.purpose;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating visitor message: {ex.Message}");
            }
        }

        private void MainDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Stopping Firebase listener on form close");
                firebaseService?.StopListening();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping Firebase listener: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateConnectionStatus("Sending door access signal...");

                // Create proper JSON structure
                var data = new Dictionary<string, bool>
                {
                    ["access_granted"] = true
                };

                var json = JsonConvert.SerializeObject(data);

                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    string firebaseUrl = "https://facultyconnectdb-default-rtdb.asia-southeast1.firebasedatabase.app/door_control.json";

                    var response = await client.PutAsync(firebaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Access granted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Debug.WriteLine("Door access signal sent successfully");
                    }
                    else
                    {
                        MessageBox.Show($"Failed to send access signal. Status: {response.StatusCode}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Debug.WriteLine($"Failed to send door access signal: {response.StatusCode}");
                    }

                    // Restore previous connection status
                    UpdateConnectionStatus(isConnected ?
                        $"Connected: Listening for requests for {lecturerName}" :
                        "Not connected");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error granting access: {ex.Message}");
                MessageBox.Show($"Error granting access: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus(isConnected ?
                    $"Connected: Listening for requests for {lecturerName}" :
                    "Not connected");
            }
        }

        public void UpdateConnectionStatus(string status)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdateConnectionStatus(status)));
                    return;
                }

                // Update the status label
                if (statusLabel != null)
                {
                    statusLabel.Text = status;
                    Debug.WriteLine($"Connection status updated: {status}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating connection status: {ex.Message}");
            }
        }

        private void btnVideoCall_Click(object sender, EventArgs e)
        {
            try
            {
                var videoWindow = new VideoFeedWindow();
                videoWindow.Show();
                Debug.WriteLine("Video call window opened");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening video call: {ex.Message}");
                MessageBox.Show($"Error opening video call: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAudioCall_Click(object sender, EventArgs e)
        {
            try
            {
                var audioWindow = new AudioCallWindow();
                audioWindow.Show();
                Debug.WriteLine("Audio call window opened");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening audio call: {ex.Message}");
                MessageBox.Show($"Error opening audio call: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New method to test Firebase connection manually
        private async void btnTestFirebase_Click(object sender, EventArgs e)
        {
            try
            {
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    UpdateConnectionStatus($"Testing connection for {lecturerName}...");
                    await firebaseService.TestFirebaseConnection(lecturerName);
                    UpdateConnectionStatus(isConnected ?
                        $"Connected: Listening for requests for {lecturerName}" :
                        "Connection test completed");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error testing Firebase connection: {ex.Message}");
                MessageBox.Show($"Error testing Firebase connection: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to test connection");
            }
        }

        // New method to restart Firebase listener
        private async void btnRestartListener_Click(object sender, EventArgs e)
        {
            try
            {
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    UpdateConnectionStatus($"Restarting listener for {lecturerName}...");

                    // Stop current listener
                    firebaseService.StopListening();

                    // Wait a moment
                    await Task.Delay(1000);

                    // Start new listener
                    await firebaseService.ListenForRequests(lecturerName);

                    isConnected = true;
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                    Debug.WriteLine($"Firebase listener restarted for {lecturerName}");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restarting Firebase listener: {ex.Message}");
                MessageBox.Show($"Error restarting Firebase listener: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to restart listener");
                isConnected = false;
            }
        }

        public class Config
        {
            public string lecturer_name { get; set; }
        }

        // Keep your existing UI event handlers
        private void panelSidebar_Paint(object sender, PaintEventArgs e) { }
        private void lstLecturers_SelectedIndexChanged(object sender, EventArgs e) { }
        private void panelSidebar_Paint_1(object sender, PaintEventArgs e) { }
        private void lblWelcome_Click(object sender, EventArgs e) { }
        private void lblFacultyConnect_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void btnEndCall_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        private async void btnTestRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    UpdateConnectionStatus($"Simulating visitor request for {lecturerName}...");
                    await firebaseService.SimulateVisitorRequest(lecturerName);
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                    Debug.WriteLine("Test visitor request simulated");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error simulating visitor request: {ex.Message}");
                MessageBox.Show($"Error simulating visitor request: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to simulate request");
            }
        }

        private void panelVisitorMessage_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}