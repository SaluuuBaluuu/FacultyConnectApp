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

namespace FacultyConnectApp.Services
{
    public class FirebaseService
    {
        private MainDashboard _dashboard;
        private FirebaseClient firebaseClient;
        private IDisposable requestSubscription;

        // Base URL
        private readonly string dbUrl = "https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/";

        public FirebaseService(MainDashboard dashboard)
        {
            _dashboard = dashboard;
            firebaseClient = new FirebaseClient(dbUrl);
        }

        public async Task ListenForRequests(string lecturerName)
        {
            Console.WriteLine($"[Firebase] Starting Listener for: {lecturerName}");

            try
            {
                // First, ensure the lecturer path exists with proper initial values
                await InitializeAvailabilityAsync(lecturerName);

                Debug.WriteLine($"Setting up subscription for: lecturers/{lecturerName}/request");

                // Subscribe to changes on the request node
                requestSubscription = firebaseClient
                .Child("lecturers")
                .Child(lecturerName)
                .Child("request")
                .AsObservable<VisitorRequest>()
                .Where(e => e.Object != null)  // Filter nulls
                .Subscribe(async data =>
                {
                    Debug.WriteLine("🔥 Firebase triggered!");
                    Debug.WriteLine("Raw event key: " + data.Key);
                    Debug.WriteLine("Raw event object: " + JsonConvert.SerializeObject(data.Object));

                    if (data.Object != null)
                    {
                        Debug.WriteLine("✅ Visitor data received: " + data.Object.visitor_name);

                        _dashboard.Invoke((MethodInvoker)(async () =>
                        {
                            VisitorPopupForm popup = new VisitorPopupForm(data.Object);
                            var result = popup.ShowDialog();

                            _dashboard.UpdateVisitorMessageOnDashboard(data.Object);
                            await ProcessPopupResultAsync(result, lecturerName);
                        }));
                    }
                    else
                    {
                        Debug.WriteLine("⚠️ Firebase change received, but data was null.");
                    }
                });
            }
            catch (FirebaseException ex)
            {
                MessageBox.Show("Firebase subscription failed: " + ex.Message);
                Console.WriteLine("❌ Firebase error: " + ex.Message);
            }
        }

        private async Task InitializeAvailabilityAsync(string lecturerName)
        {
            // Properly serialize the value as JSON before sending
            await firebaseClient
                .Child("lecturers")
                .Child(lecturerName)
                .Child("is_available")
                .PutAsync(JsonConvert.SerializeObject("pending"));
        }

        private async Task ProcessPopupResultAsync(DialogResult result, string lecturerName)
        {
            bool isAvailable = result == DialogResult.Yes;

            // Properly serialize the boolean value as JSON
            await firebaseClient
                .Child("lecturers")
                .Child(lecturerName)
                .Child("is_available")
                .PutAsync(JsonConvert.SerializeObject(isAvailable));

            // Clear the request node
            await firebaseClient
                .Child("lecturers")
                .Child(lecturerName)
                .Child("request")
                .DeleteAsync();
        }

        public void StopListening()
        {
            requestSubscription?.Dispose();
        }
    }
}