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
using System.Drawing;
using Microsoft.Win32; // Add this for Registry access

namespace FacultyConnectApp.Forms
{
    public partial class MainDashboard : Form
    {
        private UserData currentUser;
        private string lecturerName;
        private FirebaseService firebaseService;
        private bool isConnected = false;
        private NotifyIcon notifyIcon;
        private List<VisitorRequest> visitorHistory = new List<VisitorRequest>();
        private const int MAX_HISTORY = 5; // Keep last 5 visitors

        public MainDashboard(UserData userData)
        {
            InitializeComponent();
            currentUser = userData;

 

            // Hide test buttons in production mode
            btnTestRequest.Visible = false;
            btnDirectTest.Visible = false;
            btnTestFirebase.Visible = false;
            btnRestartListener.Visible = false;

            // Register resize event
            this.Resize += MainDashboard_Resize;
        }

        private void MainDashboard_Resize(object sender, EventArgs e)
        {
           
        }

        private void CenterControls()
        {
            int startY = 60; // Start position from top
            int spacing = 30; // Spacing between labels

            lblVisitorName.Top = startY;
            lblStudentNumber.Top = startY + spacing;
            lblPurpose.Top = startY + (spacing * 2);
            lblTimestamp.Top = startY + (spacing * 3);

            // Adjust label widths to match panel width
            lblVisitorName.Width = panelVisitorMessage.Width - 40; // 20px margin on each side
            lblStudentNumber.Width = panelVisitorMessage.Width - 40;
            lblPurpose.Width = panelVisitorMessage.Width - 40;
            lblTimestamp.Width = panelVisitorMessage.Width - 40;

            // Center labels horizontally
            lblVisitorName.Left = (panelVisitorMessage.Width - lblVisitorName.Width) / 2;
            lblStudentNumber.Left = (panelVisitorMessage.Width - lblStudentNumber.Width) / 2;
            lblPurpose.Left = (panelVisitorMessage.Width - lblPurpose.Width) / 2;
            lblTimestamp.Left = (panelVisitorMessage.Width - lblTimestamp.Width) / 2;

            // Show the visitor message title at the top center
            lblVisitorMessage.Left = (panelVisitorMessage.Width - lblVisitorMessage.Width) / 2;
            lblVisitorMessage.Top = 12;
        }

        private async void MainDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                // Set initial status
                UpdateConnectionStatus("Initializing...");
                Debug.WriteLine("MainDashboard is initializing...");

                // Load lecturer identity from config
                LoadLecturerIdentity();

                // Initialize Firebase service
                firebaseService = new FirebaseService(this);
                firebaseService.SetNotifyIcon(notifyIcon);

                if (!string.IsNullOrEmpty(lecturerName))
                {
                    // First test the connection
                    Debug.WriteLine("Testing Firebase connection...");
                    UpdateConnectionStatus($"Testing connection for {lecturerName}...");
                    await firebaseService.SilentTestConnection(lecturerName);

                    // Then start listening for requests
                    Debug.WriteLine("Starting Firebase listener...");
                    UpdateConnectionStatus($"Starting listener for {lecturerName}...");
                    await firebaseService.ListenForRequests(lecturerName);

                    // Start listening for audio call requests
                    Debug.WriteLine("Starting audio call request listener...");
                    await firebaseService.ListenForAudioCallRequests(lecturerName);

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
                    lblWelcome.Text = $"Welcome back,\nProf. Walingo";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainDashboard_Load: {ex.Message}");
                MessageBox.Show($"Error initializing dashboard: {ex.Message}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to initialize");
            }
            await Task.Delay(5000); // Wait 5 seconds for listeners to initialize
            await firebaseService.TestAudioCallRequest(lecturerName);
        }

        private void InitializeSystemTray()
        {
            try
            {
                Debug.WriteLine("Initializing system tray icon");

                notifyIcon = new NotifyIcon();
                notifyIcon.Icon = SystemIcons.Application; // Or use your custom icon
                notifyIcon.Text = "Faculty Connect";
                notifyIcon.Visible = true;

                // Create context menu
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.Add("Open Dashboard", null, (s, e) => {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    this.Focus();
                });
                menu.Items.Add("Exit Application", null, (s, e) => {
                    // This will fully exit the application
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    Application.Exit();
                });
                notifyIcon.ContextMenuStrip = menu;

                // Double-click to restore
                notifyIcon.DoubleClick += (s, e) => {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    this.Focus();
                };

                // Handle form closing event to hide instead of close
                this.FormClosing += new FormClosingEventHandler(MainDashboard_FormClosing);
                this.FormClosed += new FormClosedEventHandler(MainDashboard_FormClosed);

                // Handle minimize button
                this.Resize += (s, e) => {
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        // Keep showing in taskbar for better user experience
                        this.ShowInTaskbar = true;

                        // Show balloon tip
                        notifyIcon.ShowBalloonTip(3000, "Faculty Connect",
                            "Application is running in the background", ToolTipIcon.Info);
                    }
                };

                Debug.WriteLine("System tray icon initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing system tray: {ex.Message}");
            }
        }

        private void MainDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user is closing the form (not application exit)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Prevent the form from closing
                this.Hide(); // Hide the form instead

                // Show notification that app is still running
                notifyIcon.ShowBalloonTip(3000, "Faculty Connect",
                    "Application is still running in the system tray. Right-click to exit.",
                    ToolTipIcon.Info);
            }
        }

        private void MainDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // Clean up resources
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                }

                // Stop Firebase listener
                firebaseService?.StopListening();

                // Force terminate if Visual Studio is in debug mode
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in FormClosed: {ex.Message}");
            }
        }

        private void SetStartupWithWindows(bool enable)
        {
            try
            {
                string appName = "FacultyConnectApp";
                string appPath = Application.ExecutablePath;

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enable)
                    {
                        key.SetValue(appName, appPath);
                        Debug.WriteLine("Application set to start with Windows");
                    }
                    else
                    {
                        if (key.GetValue(appName) != null)
                        {
                            key.DeleteValue(appName);
                            Debug.WriteLine("Application removed from Windows startup");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting startup: {ex.Message}");
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
                    // For testing purposes, set a default name - use the exact name in your Firebase
                    lecturerName = "Tom Walingo";
                    Debug.WriteLine($"Using default lecturer name: {lecturerName}");
                    MessageBox.Show("Config file (config.json) not found. Using default lecturer name for testing.",
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
            try
            {
                Debug.WriteLine($"UpdateVisitorMessageOnDashboard called with visitor: {request.visitor_name}");

                if (this.InvokeRequired)
                {
                    Debug.WriteLine("Invoke required for UI update - using Invoke");
                    this.Invoke(new Action(() => UpdateVisitorMessageOnDashboard(request)));
                    return;
                }

                // Update visitor information with center-aligned text
                lblVisitorName.Text = "Visitor: " + request.visitor_name;
                lblStudentNumber.Text = "Student #: " + request.student_number;
                lblPurpose.Text = "Purpose: " + request.purpose;

                // Format and display timestamp
                try
                {
                    if (!string.IsNullOrEmpty(request.timestamp))
                    {
                        DateTime requestTime;
                        if (DateTime.TryParse(request.timestamp, out requestTime))
                        {
                            lblTimestamp.Text = "Time: " + requestTime.ToString("h:mm tt, MMM d");
                        }
                        else
                        {
                            lblTimestamp.Text = "Time: " + request.timestamp;
                        }
                    }
                    else
                    {
                        lblTimestamp.Text = "Time: " + DateTime.Now.ToString("h:mm tt, MMM d");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error formatting timestamp: {ex.Message}");
                    lblTimestamp.Text = "Time: " + DateTime.Now.ToString("h:mm tt, MMM d");
                }

                // Refresh the UI
                CenterControls(); // Recalculate positions

                lblVisitorName.Refresh();
                lblStudentNumber.Refresh();
                lblPurpose.Refresh();
                lblTimestamp.Refresh();

                Debug.WriteLine("Dashboard labels updated successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateVisitorMessageOnDashboard: {ex.Message}");
                MessageBox.Show($"Error updating visitor message: {ex.Message}",
                    "UI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowVisitorHistory()
        {
            try
            {
                if (visitorHistory.Count == 0)
                {
                    MessageBox.Show("No visitor history available.",
                        "Visitor History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create a history display string
                StringBuilder history = new StringBuilder();
                history.AppendLine("Recent Visitors:");
                history.AppendLine();

                for (int i = 0; i < visitorHistory.Count; i++)
                {
                    var visitor = visitorHistory[i];
                    history.AppendLine($"{i + 1}. {visitor.visitor_name}");
                    history.AppendLine($"   Purpose: {visitor.purpose}");
                    history.AppendLine($"   Student #: {visitor.student_number}");

                    // Format timestamp
                    DateTime requestTime;
                    if (DateTime.TryParse(visitor.timestamp, out requestTime))
                        history.AppendLine($"   Time: {requestTime.ToString("h:mm tt, MMM d, yyyy")}");
                    else
                        history.AppendLine($"   Time: {visitor.timestamp}");

                    history.AppendLine();
                }

                MessageBox.Show(history.ToString(), "Visitor History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing visitor history: {ex.Message}");
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
                    statusLabel.Refresh(); // Force refresh to ensure visibility
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

        // Test button method - correctly connected in Designer.cs now
        private async void btnTestFirebase_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Test Firebase button clicked");
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

        // Restart Firebase listener
        private async void btnRestartListener_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Restart listener button clicked");
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

        // Test request
        private async void btnTestRequest_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Test Request button clicked");

                if (firebaseService != null && !string.IsNullOrEmpty(lecturerName))
                {
                    // Create a test request
                    var testRequest = new VisitorRequest
                    {
                        visitor_name = "Test User " + DateTime.Now.ToString("HH:mm:ss"),
                        student_number = "999999999",
                        purpose = "Testing request handling",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    // Update UI first - this is working as confirmed
                    UpdateVisitorMessageOnDashboard(testRequest);
                    Debug.WriteLine("Dashboard updated directly first");

                    // Update Firebase after UI is updated
                    Debug.WriteLine("Simulating Firebase request after UI update");
                    UpdateConnectionStatus($"Simulating visitor request for {lecturerName}...");
                    await firebaseService.SimulateVisitorRequest(lecturerName);
                    UpdateConnectionStatus($"Connected: Listening for requests for {lecturerName}");
                }
                else
                {
                    MessageBox.Show("Firebase service or lecturer name not initialized.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error simulating visitor request: {ex.Message}");
                MessageBox.Show($"Error simulating visitor request: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus("Error: Failed to simulate request");
            }
        }

        // Direct UI Test
        private void btnDirectTest_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Direct UI Test button clicked");

                // Create a test visitor request
                var testRequest = new VisitorRequest
                {
                    visitor_name = "Test Visitor",
                    student_number = "123456789",
                    purpose = "Direct UI Testing",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Update the UI directly
                UpdateVisitorMessageOnDashboard(testRequest);
                Debug.WriteLine("Direct update to dashboard completed");

                // Show a test popup
                try
                {
                    Debug.WriteLine("Creating test popup form");
                    var popupForm = new VisitorPopupForm(testRequest);
                    Debug.WriteLine("Showing test popup form");
                    var result = popupForm.ShowDialog();
                    Debug.WriteLine($"Popup form result: {result}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error showing popup form: {ex.Message}");
                    MessageBox.Show($"Error showing popup: {ex.Message}",
                        "Popup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in direct UI test: {ex.Message}");
                MessageBox.Show($"Direct UI test failed: {ex.Message}",
                    "Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void panelVisitorMessage_Paint(object sender, PaintEventArgs e) { }

        private async void btnDirectPathTest_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Direct Firebase path test requested");
                if (firebaseService != null)
                {
                    await firebaseService.DirectPathTest();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in direct path test: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Path Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblTimestamp_Click(object sender, EventArgs e)
        {

        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            ShowVisitorHistory();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Only run this code in Debug mode
            #if DEBUG 
            // Force terminate the application immediately
            Process.GetCurrentProcess().Kill();
            #endif
        }

        private void MainDashboard_Load_1(object sender, EventArgs e)
        {

        }

        
    }
}