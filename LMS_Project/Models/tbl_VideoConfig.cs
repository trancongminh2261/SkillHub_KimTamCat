using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_VideoConfig : DomainEntity
    {
        public int LessonVideoId { get; set; }
        public int StopInMinute { get; set; }
        /// <summary>
        /// Loại xác nhận
        /// nhấn tiếp tục
        /// trả lời câu hỏi
        /// </summary>
        public string Type { get; set; }
        public tbl_VideoConfig() : base() { }
        public tbl_VideoConfig(object model) : base(model) { }
    }
}