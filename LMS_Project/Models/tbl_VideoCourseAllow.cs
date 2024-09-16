using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_VideoCourseAllow : DomainEntity
    {
        public int VideoCourseId { get; set; }
        public int? ValueId { get; set; }
        /// <summary>
        /// Phân loại là phòng ban hay nhân viên được phép tham gia khóa học này
        /// </summary>
        public string Type { get; set; }       
        public tbl_VideoCourseAllow() : base() { }
        public tbl_VideoCourseAllow(object model) : base(model) { }
    }
}