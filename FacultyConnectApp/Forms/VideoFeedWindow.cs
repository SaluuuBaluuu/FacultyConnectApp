using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;




namespace FacultyConnectApp.Forms
{
    public partial class VideoFeedWindow : Form
    {
        public VideoFeedWindow()
        {
            InitializeComponent();
        }

        private async void VideoFeedWindow_Load(object sender, EventArgs e)
        {
            string streamUrl = "http://pr_nh_webcam.axiscam.net:8000/mjpg/video.mjpg?resolution=704x480";

            await videoBrowser.EnsureCoreWebView2Async(null);
            videoBrowser.CoreWebView2.Navigate("https://www.google.com");

        }

        private void btnEndCall_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSwitchToAudio_Click(object sender, EventArgs e)
        {

        }

        private void videoBrowser_Click(object sender, EventArgs e)
        {

        }
    }
}
