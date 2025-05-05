using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;
using Firebase.Database;
using System.Diagnostics;
using Firebase.Database.Query;








namespace FacultyConnectApp.Forms
{
    public partial class AudioCallWindow : Form
    {
        private string lecturerName = "Tom Walingo";
        public AudioCallWindow()
        {
            InitializeComponent();
            pictureBox1.SendToBack();
            pictureBox1.Dock = DockStyle.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StopSendingAudio();
            this.Close();


        }

        private async void AudioCallWindow_Load(object sender, EventArgs e)
        {
            if (await CheckCallRequestAccepted())
            {
                StartSendingAudio();
                pictureBox1.Controls.Add(label1);
                pictureBox1.Controls.Add(button1);
                label1.BackColor = Color.Transparent;
            }
            else
            {
                // Call was rejected or there was an error
                MessageBox.Show("Call was rejected or is no longer active", "Call Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }

            StartSendingAudio();
            pictureBox1.Controls.Add(label1);
            pictureBox1.Controls.Add(button1);
            label1.BackColor = Color.Transparent;

        }

        // Add this method to check if call was accepted
        private async Task<bool> CheckCallRequestAccepted()
        {
            try
            {
                var client = new FirebaseClient("https://facultyconnectav-default-rtdb.asia-southeast1.firebasedatabase.app/");

                // Get the call request status
                var status = await client
                    .Child("lecturers")
                    .Child(lecturerName)
                    .Child("audio_call_request")
                    .Child("status")
                    .OnceSingleAsync<string>();

                return status == "accepted";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking call status: {ex.Message}");
                return false;
            }
        }


        private void StartSendingAudio()
        {
            try
            {
                udpSender = new UdpClient();
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0;  // First recording device (default microphone)
                waveIn.WaveFormat = new WaveFormat(8000, 16, 1); // 8000Hz, 16-bit, Mono

                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting microphone: " + ex.Message);
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                udpSender.Send(e.Buffer, e.BytesRecorded, piIPAddress, destinationPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending audio: " + ex.Message);
            }
        }


        private void StopSendingAudio()
        {
            try
            {
                waveIn?.StopRecording();
                waveIn?.Dispose();
                udpSender?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping audio: " + ex.Message);
            }
        }



        private UdpClient udpSender;
        private WaveInEvent waveIn;
        private string piIPAddress = "172.20.10.3";  // ⬅️ Replace with your Pi's actual IP!
        private int destinationPort = 9001;            // Same as what Pi is listening on

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

}
