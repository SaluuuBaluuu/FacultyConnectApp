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
            StopSendingAudio();
            this.Close();


        }

        private void AudioCallWindow_Load(object sender, EventArgs e)
        {
            
           
            StartSendingAudio();


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

    

    



    }

}
