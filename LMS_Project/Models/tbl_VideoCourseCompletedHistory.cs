using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_VideoCourseCompletedHistory : DomainEntity
    {
        public int UserId { get; set; }
        public int VideoCourseId { get; set; }
        public DateTime CompletedDate { get; set; }
        public tbl_VideoCourseCompletedHistory() : base() { }
        public tbl_VideoCourseCompletedHistory(object model) : base(model) { }
    }
}