using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO
{
    public class VideoCourseStatusDTO 
    {
        public int TotalRow { get; set; } = 0;
        public int Waiting { get; set; } = 0;
        public int Studying { get; set; } = 0;
        public int Finished { get; set; } = 0;
    }
}