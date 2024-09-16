using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.ServerDownload
{
    public class ResponseUploadVideoDTO
    {
        /// <summary>
        /// id sau khi upload video
        /// </summary>
        public string _id { get; set; }       
        public string video_protection_id { get; set; }
        public string title { get; set; }
        public string filename { get; set; }
        public string extension { get; set; }
        public Render render { get; set; }
        public double? views { get; set; }
        public string last_seen_date { get; set; }
        /// <summary>
        /// có chống download hay không
        /// </summary>
        public bool? Protected { get; set; }
        public double? size { get; set; }
        public double? duration { get; set; }
        public double? bitrate { get; set; }
        public string dimensions { get; set; }
        public string thumbnail { get; set; }
        public string type { get; set; }
        public string properties { get; set; }
        public double? created_at { get; set; }
        public string created_by { get; set; }
        public string product_name { get; set; }
        public string link { get; set; }     
    }

    public class Render
    {
        public string status { get; set; }
        public double? s480p { get; set; }
        public double? s720p { get; set; }
        public double? s1080p { get; set; }
        public double? s1920p { get; set; }
        public double? s2160p { get; set; }
        public double? sOriginal { get; set; }
    }
    public class Properties
    {
        public List<string> links { get; set; }
    }
}