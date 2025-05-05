using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyConnectApp.Models
{
    // In your VisitorRequest.cs file, ensure the timestamp is initialized correctly
    public class VisitorRequest
    {
        [JsonProperty("visitor_name")]
        public string visitor_name { get; set; }

        [JsonProperty("student_number")]
        public string student_number { get; set; }

        [JsonProperty("purpose")]
        public string purpose { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        // Default constructor
        public VisitorRequest()
        {
            // Set the timestamp to current time by default
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Override ToString for debugging
        public override string ToString()
        {
            return $"Visitor: {visitor_name}, Student#: {student_number}, Purpose: {purpose}, Time: {timestamp}";
        }
    }
}

