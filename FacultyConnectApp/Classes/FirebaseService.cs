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

namespace FacultyConnectApp.Services
{
    public class FirebaseService
    {
        private MainDashboard _dashboard;
        private FirebaseClient firebaseClient;
        private IDisposable requestSubscription;
        private bool isListening = false;

        // Base URL
        private readonly string dbUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/";

        public FirebaseService(MainDashboard dashboard)
        {
            _dashboard = dashboard;
            firebaseClient = new FirebaseClient(dbUrl);
            Debug.WriteLine("🔥 Firebase service initialized");
        }

        public async Task ListenForRequests(string lecturerName)
        {
            if (string.IsNullOrEmpty(lecturerName))
            {
                Debug.WriteLine("❌ Error: Lecturer name cannot be empty");
                MessageBox.Show("Lecturer name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Debug.WriteLine($"[Firebase] Starting Listener for: {lecturerName}");

            try
            {
                // First, ensure the lecturer path exists with proper initial values
                await InitializeAvailabilityAsync(lecturerName);

                // Clean up any existing subscription
                StopListening();

                Debug.WriteLine($"Setting up subscription for: lecturers/{lecturerName}/request");

                // Test if we can read from the path first
                try
                {
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

                // Subscribe to changes on the request node - important change: use ChildAdded event instead
                // Replace this section in the ListenForRequests method:

                // Subscribe to changes on the request node
                requestSubscription = firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .AsObservable<Dictionary<string, object>>()
                    .Where(data => data.Object != null) // Filter out null objects
                    .Subscribe(
                        data =>
                        {
                            Debug.WriteLine("🔥 Firebase event received!");
                            Debug.WriteLine($"Event key: {data.Key}, Has object: {data.Object != null}");

                            try
                            {
                                if (data.Object != null)
                                {
                                    // We need to rebuild a visitor request from potentially partial data
                                    VisitorRequest existingRequest = TryBuildVisitorRequest(data.Object);

                                    if (existingRequest != null)
                                    {
                                        Debug.WriteLine("✅ Visitor data processed: " + JsonConvert.SerializeObject(existingRequest));

                                        // Only show popup if we have at least visitor name and purpose
                                        if (!string.IsNullOrEmpty(existingRequest.visitor_name) &&
                                            !string.IsNullOrEmpty(existingRequest.purpose))
                                        {
                                            // Update dashboard first
                                            _dashboard.BeginInvoke(new Action(() =>
                                            {
                                                _dashboard.UpdateVisitorMessageOnDashboard(existingRequest);
                                            }));

                                            // Then show the popup on the UI thread
                                            _dashboard.BeginInvoke(new Action(async () =>
                                            {
                                                try
                                                {
                                                    Debug.WriteLine("⚠️ Showing visitor popup form");

                                                    // Create new popup form
                                                    VisitorPopupForm popup = new VisitorPopupForm(existingRequest);

                                                    // Show the form as dialog - this is critical to make it wait for user input
                                                    DialogResult result = popup.ShowDialog();

                                                    Debug.WriteLine($"Popup result: {result}");

                                                    // Process the dialog result
                                                    await ProcessPopupResultAsync(result, lecturerName);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine($"❌ Error showing visitor popup: {ex.Message}");
                                                    MessageBox.Show($"Error showing visitor popup: {ex.Message}",
                                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            Debug.WriteLine("⚠️ Received partial visitor data, waiting for complete data");
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("⚠️ Could not build visitor request from data");
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("⚠️ Firebase change received, but data was null.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"❌ Error processing Firebase event: {ex.Message}");
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
                _dashboard.BeginInvoke(new Action(() =>
                {
                    _dashboard.UpdateConnectionStatus("Connected to Firebase");
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Firebase error: {ex.Message}");
                MessageBox.Show($"Firebase subscription failed: {ex.Message}", "Firebase Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New helper method to build a VisitorRequest from various data formats
        // Replace/Update the existing TryBuildVisitorRequest method to fix any potential data parsing issues:

        private VisitorRequest TryBuildVisitorRequest(object data)
        {
            try
            {
                Debug.WriteLine($"Attempting to build visitor request from data type: {data?.GetType().Name}");

                // Case 1: If data is already a Dictionary<string, object>, try to extract properties
                if (data is Dictionary<string, object> dict)
                {
                    Debug.WriteLine($"Processing dictionary data with keys: {string.Join(", ", dict.Keys)}");

                    var request = new VisitorRequest();

                    if (dict.ContainsKey("visitor_name") && dict["visitor_name"] != null)
                        request.visitor_name = dict["visitor_name"].ToString();

                    if (dict.ContainsKey("student_number") && dict["student_number"] != null)
                        request.student_number = dict["student_number"].ToString();

                    if (dict.ContainsKey("purpose") && dict["purpose"] != null)
                        request.purpose = dict["purpose"].ToString();

                    if (dict.ContainsKey("timestamp") && dict["timestamp"] != null)
                        request.timestamp = dict["timestamp"].ToString();

                    // Added debug logging to see what we got
                    Debug.WriteLine($"Built request: {request}");
                    return request;
                }

                // Case 2: Try to deserialize the whole object to VisitorRequest
                var jsonData = JsonConvert.SerializeObject(data);
                Debug.WriteLine($"Raw JSON data: {jsonData}");

                try
                {
                    var request = JsonConvert.DeserializeObject<VisitorRequest>(jsonData);
                    Debug.WriteLine($"Deserialized request: {request}");
                    return request;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not deserialize as VisitorRequest: {ex.Message}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error building visitor request: {ex.Message}");
                return null;
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
                string availabilityStatus = result == DialogResult.Yes ? "true" : "false";
                Debug.WriteLine($"Processing popup result: {availabilityStatus}");

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
                _dashboard?.BeginInvoke(new Action(() =>
                {
                    _dashboard.UpdateConnectionStatus("Disconnected from Firebase");
                }));
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

        // Modified method to simulate a visitor request for testing with complete visitor object
        public async Task SimulateVisitorRequest(string lecturerName)
        {
            try
            {
                Debug.WriteLine("Simulating visitor request...");

                var visitorRequest = new VisitorRequest
                {
                    visitor_name = "Test User",
                    student_number = "999999999",
                    purpose = "Testing request handling",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Convert to dictionary first to ensure proper structure
                var requestData = new Dictionary<string, object>
                {
                    ["visitor_name"] = visitorRequest.visitor_name,
                    ["student_number"] = visitorRequest.student_number,
                    ["purpose"] = visitorRequest.purpose,
                    ["timestamp"] = visitorRequest.timestamp
                };

                await firebaseClient
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("request")
                    .PutAsync(requestData);

                Debug.WriteLine("✅ Successfully sent test visitor request");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Simulation failed: {ex.Message}");
                MessageBox.Show($"Visitor request simulation failed: {ex.Message}", "Simulation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}