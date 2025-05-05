using Firebase.Database;
using Firebase.Database.Query;
using FacultyConnectApp.Models;
using FacultyConnectApp.Forms;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;

namespace FacultyConnectApp.Services
{
    public class FirebaseService
    {
        private string responseStatus;
        private MainDashboard _dashboard;
        private FirebaseClient firebaseClient;
        private IDisposable requestSubscription;
        private IDisposable audioCallSubscription;
        private bool isListening = false;
        private NotifyIcon _notifyIcon;

        // Base URL - ensure this matches your Firebase database
        private readonly string dbUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/";

        public FirebaseService(MainDashboard dashboard)
        {
            _dashboard = dashboard;
            firebaseClient = new FirebaseClient(dbUrl);
            Debug.WriteLine("🔥 Firebase service initialized");
        }

        public void SetNotifyIcon(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
            Debug.WriteLine("System tray notification icon set");
        }

        public async Task ListenForRequests(string lecturerName)
        {
            if (string.IsNullOrEmpty(lecturerName))
            {
                Debug.WriteLine("❌ Error: Lecturer name cannot be empty");
                MessageBox.Show("Lecturer name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Debug.WriteLine($"[Firebase] Starting Listener for: '{lecturerName}'");
            Debug.WriteLine($"[Firebase] Monitoring path: lecturers/{lecturerName}/request");

            try
            {
                // First, ensure the lecturer path exists with proper initial values
                await InitializeAvailabilityAsync(lecturerName);

                // Clean up any existing subscription
                StopListening();

                // Test if we can read from the path first
                try
                {
                    Debug.WriteLine($"Attempting to read from path: lecturers/{lecturerName}");
                    var initialData = await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .OnceAsync<object>();

                    Debug.WriteLine($"Successfully read from lecturer path. Data exists: {initialData != null}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Failed to read from lecturer path: {ex.Message}");
                    MessageBox.Show($"Failed to access Firebase path: {ex.Message}", "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check for any existing requests when starting
                await CheckExistingRequests(lecturerName);

                // Subscribe to changes directly on the entire lecturer node to catch all updates
                requestSubscription = firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .AsObservable<object>()
                    .Subscribe(
                        data =>
                        {
                            Debug.WriteLine("🔥 Firebase lecturer node event received!");

                            // If it's a change to the request node
                            if (data.Key == "request")
                            {
                                Debug.WriteLine($"Request node updated: {JsonConvert.SerializeObject(data.Object)}");

                                // Process the request data
                                if (data.Object != null)
                                {
                                    // Fetch the full request data to handle both full updates and incremental updates
                                    GetAndProcessFullRequest(lecturerName);
                                }
                            }
                        },
                        ex =>
                        {
                            Debug.WriteLine($"❌ Firebase subscription error: {ex.Message}");
                            MessageBox.Show($"Firebase subscription error: {ex.Message}",
                                "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // Try to restart the subscription after a delay
                            Task.Delay(5000).ContinueWith(_ =>
                            {
                                Debug.WriteLine("Attempting to restart Firebase subscription...");
                                ListenForRequests(lecturerName).ConfigureAwait(false);
                            });
                        },
                        () =>
                        {
                            Debug.WriteLine("📊 Firebase subscription completed");
                            isListening = false;
                        });

                isListening = true;
                Debug.WriteLine("✅ Firebase subscription successfully created");

                // Let the user know that the connection is active
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        _dashboard.UpdateConnectionStatus("Connected to Firebase");
                    }));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase error: {ex.Message}");
                MessageBox.Show($"Firebase subscription failed: {ex.Message}", "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CheckExistingRequests(string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Checking for existing requests for {lecturerName}...");
                var requestData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (requestData != null && requestData.Count > 0)
                {
                    Debug.WriteLine($"Found existing request: {JsonConvert.SerializeObject(requestData)}");

                    // Process the existing request
                    var request = BuildVisitorRequestFromDictionary(requestData);
                    if (request != null && !string.IsNullOrEmpty(request.visitor_name) && !string.IsNullOrEmpty(request.purpose))
                    {
                        ProcessVisitorRequest(request, lecturerName);
                    }
                }
                else
                {
                    Debug.WriteLine("No existing requests found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking existing requests: {ex.Message}");
            }
        }

        private async void GetAndProcessFullRequest(string lecturerName)
        {
            try
            {
                var requestData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (requestData != null && requestData.Count > 0)
                {
                    Debug.WriteLine($"Full request data: {JsonConvert.SerializeObject(requestData)}");

                    var request = BuildVisitorRequestFromDictionary(requestData);
                    if (request != null && !string.IsNullOrEmpty(request.visitor_name) && !string.IsNullOrEmpty(request.purpose))
                    {
                        ProcessVisitorRequest(request, lecturerName);
                    }
                    else
                    {
                        Debug.WriteLine("Request data is incomplete");
                    }
                }
                else
                {
                    Debug.WriteLine("No request data found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting full request: {ex.Message}");
            }
        }

        private VisitorRequest BuildVisitorRequestFromDictionary(Dictionary<string, object> dict)
        {
            try
            {
                var request = new VisitorRequest();

                if (dict.ContainsKey("visitor_name") && dict["visitor_name"] != null)
                    request.visitor_name = dict["visitor_name"].ToString();

                if (dict.ContainsKey("student_number") && dict["student_number"] != null)
                    request.student_number = dict["student_number"].ToString();

                if (dict.ContainsKey("purpose") && dict["purpose"] != null)
                    request.purpose = dict["purpose"].ToString();

                if (dict.ContainsKey("timestamp") && dict["timestamp"] != null)
                {
                    // Use the timestamp from Firebase, but if it's the example timestamp,
                    // replace it with the current time
                    var timestampStr = dict["timestamp"].ToString();

                    // Check if it's the fixed example timestamp
                    if (timestampStr == "2025-05-04 12:34:56")
                    {
                        // Replace with current time
                        request.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Debug.WriteLine("Replaced example timestamp with current time");
                    }
                    else
                    {
                        request.timestamp = timestampStr;
                    }
                }
                else
                {
                    // If no timestamp provided, use current time
                    request.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                Debug.WriteLine($"Built request from dictionary: {request}");
                return request;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building request from dictionary: {ex.Message}");
                return null;
            }
        }

        private void ProcessVisitorRequest(VisitorRequest request, string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Processing visitor request: {request}");

                // Show system tray notification
                ShowNotification(request);

                // Make application visible if it's hidden
                if (_dashboard != null && !_dashboard.IsDisposed)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        if (!_dashboard.Visible)
                        {
                            _dashboard.Show();
                            _dashboard.WindowState = FormWindowState.Normal;
                            _dashboard.BringToFront();
                            _dashboard.Focus();
                        }
                    }));
                }

                // Update UI with visitor info
                if (_dashboard != null && !_dashboard.IsDisposed)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        try
                        {
                            // Update dashboard UI
                            Debug.WriteLine("Updating dashboard with visitor info");
                            _dashboard.UpdateVisitorMessageOnDashboard(request);

                            // Show popup
                            Debug.WriteLine("Creating and showing visitor popup");
                            VisitorPopupForm popup = new VisitorPopupForm(request);
                            DialogResult result = popup.ShowDialog();

                            Debug.WriteLine($"Popup result: {result}");

                            // Process result
                            ProcessPopupResultAsync(result, lecturerName).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error updating UI or showing popup: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    // If the dashboard isn't available, create a standalone popup
                    CreateStandalonePopup(request, lecturerName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing visitor request: {ex.Message}");
            }
        }

        private void CreateStandalonePopup(VisitorRequest request, string lecturerName)
        {
            try
            {
                // Create a temporary form to host the popup
                Form tempForm = new Form();
                tempForm.StartPosition = FormStartPosition.CenterScreen;
                tempForm.ShowInTaskbar = false;
                tempForm.FormBorderStyle = FormBorderStyle.None;
                tempForm.Opacity = 0;
                tempForm.Size = new System.Drawing.Size(1, 1);

                tempForm.Shown += (s, e) =>
                {
                    try
                    {
                        VisitorPopupForm popup = new VisitorPopupForm(request);
                        DialogResult result = popup.ShowDialog(tempForm);

                        Debug.WriteLine($"Standalone popup result: {result}");
                        ProcessPopupResultAsync(result, lecturerName).ConfigureAwait(false);

                        tempForm.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error showing standalone popup: {ex.Message}");
                        tempForm.Close();
                    }
                };

                tempForm.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating standalone popup: {ex.Message}");
            }
        }

        private void ShowNotification(VisitorRequest request)
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.BalloonTipTitle = "Faculty Connect - Visitor Request";
                    _notifyIcon.BalloonTipText = $"New visitor: {request.visitor_name}\nPurpose: {request.purpose}";
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(5000);
                    Debug.WriteLine("System tray notification shown");
                }
                else
                {
                    Debug.WriteLine("NotifyIcon is null, cannot show notification");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error showing notification: {ex.Message}");
            }
        }

        private async Task InitializeAvailabilityAsync(string lecturerName)
        {
            try
            {
                Debug.WriteLine($"Initializing availability status for {lecturerName}");

                // Check if the lecturer node exists
                var lecturerData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .OnceSingleAsync<object>();

                if (lecturerData == null)
                {
                    // Create the entire lecturer structure if it doesn't exist
                    var initialData = new Dictionary<string, object>
                    {
                        ["is_available"] = "pending",
                        ["last_updated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .PutAsync(initialData);

                    Debug.WriteLine("Created new lecturer node with pending status");
                }
                else
                {
                    // Update only the availability status - Use a string value wrapped in quotes
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("is_available")
                        .PutAsync<string>("pending");

                    Debug.WriteLine("Updated existing lecturer availability to pending");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error initializing availability: {ex.Message}");
                throw; // Rethrow to let the caller handle it
            }
        }

        private async Task ProcessPopupResultAsync(DialogResult result, string lecturerName)
        {
            try
            {
                // Convert DialogResult to appropriate availability status
                string availabilityStatus = result == DialogResult.Yes ? "true" : "false";
                Debug.WriteLine($"Processing popup result: {result} -> Setting is_available to {availabilityStatus}");

                // Update availability status in Firebase - Use a string value for compatibility
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("is_available")
                    .PutAsync<string>(availabilityStatus);

                Debug.WriteLine("✅ Updated availability status");

                // Update last response time
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("last_response")
                    .PutAsync<string>(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                // Clear the request node
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .DeleteAsync();

                Debug.WriteLine("✅ Cleared request node");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error processing popup result: {ex.Message}");
                throw; // Rethrow to let the caller handle it
            }
        }

        public void StopListening()
        {
            if (requestSubscription != null)
            {
                Debug.WriteLine("Stopping Firebase listener");
                requestSubscription.Dispose();
                requestSubscription = null;
                isListening = false;

                // Update UI to show disconnected status
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        _dashboard.UpdateConnectionStatus("Disconnected from Firebase");
                    }));
                }
            }
        }

        public bool IsListening()
        {
            return isListening;
        }

        public async Task TestFirebaseConnection(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Testing Firebase connection...");

                // Test writing data with proper formatting
                var testTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Create a dictionary for the test data
                var testData = new Dictionary<string, string>
                {
                    ["timestamp"] = testTimestamp
                };

                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .PutAsync(testData);

                Debug.WriteLine("✅ Successfully wrote test data to Firebase");

                // Test reading data
                var readData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .OnceSingleAsync<Dictionary<string, string>>();

                if (readData != null && readData.ContainsKey("timestamp"))
                {
                    Debug.WriteLine($"✅ Successfully read data from Firebase: {readData["timestamp"]}");
                    MessageBox.Show("Firebase connection test successful!", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Debug.WriteLine("⚠️ Read test data format is unexpected");
                    MessageBox.Show("Firebase connection test completed, but data format was unexpected.",
                        "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase test failed: {ex.Message}");
                MessageBox.Show($"Firebase test failed: {ex.Message}", "Connection Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task SimulateVisitorRequest(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Simulating visitor request...");

                // Create a complete visitor request with all fields and current timestamp
                var visitorRequest = new VisitorRequest
                {
                    visitor_name = "Test User " + DateTime.Now.ToString("HH:mm:ss"),
                    student_number = "999999999",
                    purpose = "Testing request handling",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // Current time
                };

                Debug.WriteLine($"Visitor request data: {JsonConvert.SerializeObject(visitorRequest)}");

                // First, directly update the UI for immediate feedback
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        try
                        {
                            Debug.WriteLine("Directly updating dashboard UI first");
                            _dashboard.UpdateVisitorMessageOnDashboard(visitorRequest);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error updating dashboard directly: {ex.Message}");
                        }
                    }));
                }

                // Then send the data to Firebase
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .PutAsync(visitorRequest);

                Debug.WriteLine("✅ Successfully sent test visitor request to Firebase");

                // For testing purposes, show notification
                ShowNotification(visitorRequest);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Simulation failed: {ex.Message}");
                MessageBox.Show($"Visitor request simulation failed: {ex.Message}",
                    "Simulation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task DirectPathTest()
        {
            try
            {
                Debug.WriteLine("Performing direct path test...");

                // Test Tom Walingo path
                var testPath = await firebaseClient
                    .Child("lecturers")
                    .Child("Tom Walingo")
                    .OnceAsync<object>();

                Debug.WriteLine($"Direct access to Tom Walingo node: {(testPath != null ? "Success" : "Failed")}");

                // Test request node under Tom Walingo
                var requestNode = await firebaseClient
                    .Child("lecturers")
                    .Child("Tom Walingo")
                    .Child("request")
                    .OnceAsync<object>();

                Debug.WriteLine($"Direct access to request node: {(requestNode != null ? "Success" : "Failed")}");

                // Manually fetch the request data
                var manualRequest = await firebaseClient
                    .Child("lecturers")
                    .Child("Tom Walingo")
                    .Child("request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (manualRequest != null)
                {
                    Debug.WriteLine("Manual request data:");
                    Debug.WriteLine(JsonConvert.SerializeObject(manualRequest, Formatting.Indented));

                    // Try to process it
                    VisitorRequest testRequest = BuildVisitorRequestFromDictionary(manualRequest);
                    if (testRequest != null)
                    {
                        Debug.WriteLine($"Successfully built visitor request: {testRequest}");

                        // Update dashboard and show popup manually
                        if (_dashboard != null)
                        {
                            _dashboard.Invoke(new Action(() =>
                            {
                                _dashboard.UpdateVisitorMessageOnDashboard(testRequest);

                                // Also show a popup
                                VisitorPopupForm popup = new VisitorPopupForm(testRequest);
                                popup.ShowDialog();
                            }));
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No data found in request node");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in direct path test: {ex.Message}");
            }
        }

        // Add this method to FirebaseService.cs
        public async Task SilentTestConnection(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Testing Firebase connection silently...");

                // Test writing data with proper formatting
                var testTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Create a dictionary for the test data
                var testData = new Dictionary<string, string>
                {
                    ["timestamp"] = testTimestamp
                };

                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .PutAsync(testData);

                Debug.WriteLine("✅ Successfully tested Firebase connection");

                // Test reading data
                var readData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("test_connection")
                    .OnceSingleAsync<Dictionary<string, string>>();

                if (readData != null && readData.ContainsKey("timestamp"))
                {
                    Debug.WriteLine($"✅ Successfully read data from Firebase: {readData["timestamp"]}");
                    // No message box here!
                }
                else
                {
                    Debug.WriteLine("⚠️ Read test data format is unexpected");
                    // No message box here either
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase test failed: {ex.Message}");
                // Update status without showing message box
                if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        _dashboard.UpdateConnectionStatus("Connection error - check network");
                    }));
                }
            }
        }

        // Add these methods to your FirebaseService class

        // Add these methods to your FirebaseService class

        // In FirebaseService.cs
        public async Task ListenForAudioCallRequests(string lecturerName)
        {
            try
            {
                Debug.WriteLine("=============================");
                Debug.WriteLine("Starting to listen for audio call requests...");
                Debug.WriteLine($"Path: lecturers/{lecturerName}/audio_call_request");
                Debug.WriteLine("=============================");

                // Create a separate subscription specifically for audio calls
                audioCallSubscription = firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .AsObservable<object>()  // Subscribe to the entire lecturer node
                    .Where(data => data.Key == "audio_call_request")  // Filter for audio_call_request events
                    .Subscribe(
                        data =>
                        {
                            Debug.WriteLine("✅ AUDIO CALL EVENT RECEIVED: " + JsonConvert.SerializeObject(data.Object));
                            if (data.Object != null)
                            {
                                ProcessAudioCallRequest(data.Object, lecturerName);
                            }
                        },
                        ex =>
                        {
                            Debug.WriteLine($"❌ AUDIO CALL SUBSCRIPTION ERROR: {ex.Message}");
                        });

                // Also check for existing audio call requests
                var existingData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                Debug.WriteLine($"Initial audio_call_request data: {JsonConvert.SerializeObject(existingData)}");

                // Process existing request if it has pending status
                if (existingData != null &&
                    existingData.ContainsKey("status") &&
                    existingData["status"].ToString() == "pending")
                {
                    Debug.WriteLine("Found existing pending call request - processing immediately");
                    ProcessAudioCallRequest(existingData, lecturerName);
                }

                Debug.WriteLine("Audio call request listener successfully established");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ FAILED TO SET UP AUDIO CALL LISTENER: {ex.Message}");
            }
        }

        private async Task CheckExistingAudioCallRequest(string lecturerName)
        {
            try
            {
                var existingData = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                Debug.WriteLine($"Initial audio_call_request data: {JsonConvert.SerializeObject(existingData)}");

                // If there's existing data with pending status, process it immediately
                if (existingData != null &&
                    existingData.ContainsKey("status") &&
                    existingData["status"].ToString() == "pending")
                {
                    Debug.WriteLine("Found existing pending call request - processing it immediately");
                    ProcessAudioCallRequest(existingData, lecturerName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking existing audio call request: {ex.Message}");
            }
        }

        private async Task CheckForNewAudioCallRequest(string lecturerName)
        {
            try
            {
                var data = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                if (data != null &&
                    data.ContainsKey("status") &&
                    data["status"].ToString() == "pending")
                {
                    Debug.WriteLine("Found pending call request during periodic check");
                    ProcessAudioCallRequest(data, lecturerName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in periodic audio call check: {ex.Message}");
            }
        }
        private void ProcessAudioCallRequest(object requestData, string lecturerName)
        {
            try
            {
                Debug.WriteLine("Processing audio call request data: " + JsonConvert.SerializeObject(requestData));

                var callData = requestData as Dictionary<string, object>;
                if (callData == null)
                {
                    Debug.WriteLine("Call data is null or not a Dictionary");
                    return;
                }

                Debug.WriteLine("Call data keys: " + string.Join(", ", callData.Keys));

                // Check for status in a case-insensitive way
                string status = "";
                foreach (var key in callData.Keys)
                {
                    if (string.Equals(key, "status", StringComparison.OrdinalIgnoreCase))
                    {
                        status = callData[key]?.ToString() ?? "";
                        break;
                    }
                }

                Debug.WriteLine("Call status: " + status);

                // Check if status is pending in a case-insensitive way
                if (string.Equals(status, "pending", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Call status is pending, showing popup");

                    // Get caller name
                    string callerName = "Unknown Caller";
                    foreach (var key in callData.Keys)
                    {
                        if (string.Equals(key, "caller_name", StringComparison.OrdinalIgnoreCase))
                        {
                            callerName = callData[key]?.ToString() ?? "Unknown Caller";
                            break;
                        }
                    }

                    // Show notification and popup on UI thread
                    if (_dashboard != null && !_dashboard.IsDisposed && _dashboard.IsHandleCreated)
                    {
                        // Use Invoke instead of BeginInvoke to ensure synchronous execution
                        _dashboard.Invoke(new Action(() =>
                        {
                            try
                            {
                                // Make sure form is visible
                                if (!_dashboard.Visible)
                                {
                                    _dashboard.Show();
                                    _dashboard.WindowState = FormWindowState.Normal;
                                }

                                // Show notification in system tray
                                if (_notifyIcon != null)
                                {
                                    _notifyIcon.ShowBalloonTip(5000, "Incoming Audio Call",
                                        $"Call from: {callerName}", ToolTipIcon.Info);
                                }

                                Debug.WriteLine("Creating AudioCallRequestForm for caller: " + callerName);
                                AudioCallRequestForm requestForm = new AudioCallRequestForm(callerName);

                                Debug.WriteLine("Showing popup dialog");
                                DialogResult result = requestForm.ShowDialog(_dashboard);

                                Debug.WriteLine("Dialog result: " + result);
                                ProcessCallRequestResponse(result, lecturerName).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error showing call request dialog: {ex.Message}");
                                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }));
                    }
                    else
                    {
                        Debug.WriteLine("Dashboard invalid, can't show popup");
                    }
                }
                else
                {
                    Debug.WriteLine("Call status is not pending, ignoring");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing audio call request: {ex.Message}");
            }
        }



        private async Task ProcessCallRequestResponse(DialogResult result, string lecturerName)
        {
            try
            {
                string responseStatus = result == DialogResult.Yes ? "accepted" : "rejected";
                Debug.WriteLine($"Call {responseStatus}");

                // If accepted, open audio call window first
                if (result == DialogResult.Yes)
                {
                    _dashboard.Invoke(new Action(() =>
                    {
                        try
                        {
                            AudioCallWindow audioWindow = new AudioCallWindow();
                            audioWindow.Show();
                            Debug.WriteLine("Audio call window opened successfully");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error opening audio call window: {ex.Message}");
                        }
                    }));
                }

                // Then completely remove the audio call request node
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .DeleteAsync();

                Debug.WriteLine("✅ Removed audio call request data from Firebase");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing call response: {ex.Message}");

                // Try to update the status as a fallback if deletion fails
                try
                {
                    // Update Firebase with response status
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("audio_call_request")
                        .Child("status")
                        .PutAsync(responseStatus);

                    Debug.WriteLine("✅ Updated call status as fallback");
                }
                catch (Exception innerEx)
                {
                    Debug.WriteLine($"Error in fallback status update: {innerEx.Message}");
                }
            }
        }

        public async Task DiagnoseAudioCallRequest(string lecturerName)
        {
            try
            {
                // Print the exact path we're checking
                string path = $"lecturers/{lecturerName}/audio_call_request";
                Console.WriteLine($"Checking path: {path}");

                // Try to directly read data
                var data = await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .OnceSingleAsync<Dictionary<string, object>>();

                // Log what we found
                if (data != null)
                {
                    Console.WriteLine("Found audio call request data:");
                    Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));

                    // Try processing it directly
                    ProcessAudioCallRequest(data, lecturerName);
                }
                else
                {
                    Console.WriteLine("No audio call request data found");

                    // Create test data
                    var testRequest = new Dictionary<string, object>
                    {
                        ["caller_name"] = "Test Caller",
                        ["status"] = "pending",
                        ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    // Try writing test data
                    await firebaseClient
                        .Child("lecturers")
                        .Child(lecturerName)
                        .Child("audio_call_request")
                        .PutAsync(testRequest);

                    Console.WriteLine("Created test audio call request data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Diagnosis error: {ex.Message}");
            }
        }

        // In FirebaseService.cs
        public async Task TestAudioCallRequest(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Testing audio call request directly...");

                // Create test data
                var testRequest = new Dictionary<string, object>
                {
                    ["caller_name"] = "Direct Test Caller",
                    ["status"] = "pending",
                    ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Send to Firebase
                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .PutAsync(testRequest);

                Debug.WriteLine("Test audio call request sent!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error testing audio call: {ex.Message}");
            }
        }
    }
}