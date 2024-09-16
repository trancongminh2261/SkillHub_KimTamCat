using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Areas.Models
{
    public class ZoomModel
    {
        public bool? Status { get; set; }
        public string RoomId { get; set; }
        public string RoomPass { get; set; } 
        public string SignatureTeacher { get; set; }
        public string SignatureStudent { get; set; }
        public string ApiKey { get; set; }  
        public string UserName { get; set; }  

    }
    public class RecordingFiles
    {
        public string Id { get; set; }
        public string meeting_Id { get; set; }
        public string download_url { get; set; }
        public string file_type { get; set; }
        public string play_url { get; set; }
        public string recording_start { get; set; }
        public string recording_end { get; set; }
        public string file_size { get; set; }
        public string file_extension { get; set; }
        public string recording_type { get; set; }
    }
    //{
    //    public static string Id { get; set; }
    //    public static string meeting_Id { get; set; }
    //    public static string recording_start { get; set; }
    //    public static string recording_end { get; set; }
    //    public static string file_type { get; set; }
    //    public static string file_extension { get; set; }
    //    public static string file_size { get; set; }
    //    public static string play_url { get; set; }
    //    public static string download_url { get; set; }
    //    public static string status { get; set; }
    //    public static string recording_type { get; set; }
    //}
}