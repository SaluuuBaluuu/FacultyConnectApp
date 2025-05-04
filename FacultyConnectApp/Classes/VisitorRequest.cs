using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyConnectApp.Models
{
    public class VisitorRequest
    {
        [JsonProperty("visitor_name")]
        public string visitor_name { get; set; }

        [JsonProperty("student_number")]
        public string student_number { get; set; }

        [JsonProperty("purpose")]
        public string purpose { get; set; }

        // Add a timestamp property that might be used in some implementations
        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        // Default constructor (required for deserialization)
        public VisitorRequest()
        {
            // Default constructor
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Override ToString for debugging
        public override string ToString()
        {
            return $"Visitor: {visitor_name}, Student#: {student_number}, Purpose: {purpose}";
        }
    }
}

