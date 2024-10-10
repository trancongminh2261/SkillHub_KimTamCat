using LMS_Project.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.VideoCourseCompletedHistory
{
    public class VideoCourseCompletedHistoryDTO : DomainDTO
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public int VideoCourseId { get; set; }
        public string VideoCourseName { get; set; }
        public DateTime CompletedDate { get; set; }
        public VideoCourseCompletedHistoryDTO() : base() { }
        public VideoCourseCompletedHistoryDTO(object model) : base(model) { }
    }
}