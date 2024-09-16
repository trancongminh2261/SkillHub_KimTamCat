using LMS_Project.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO
{
    public class VideoConfigDTO : DomainDTO
    {
        public int LessonVideoId { get; set; }
        public int StopInMinute { get; set; }
        /// <summary>
        /// Loại xác nhận
        /// nhấn tiếp tục
        /// trả lời câu hỏi
        /// </summary>
        public string Type { get; set; }
        public int TotalQuestion { get; set; }
        public VideoConfigDTO() : base() { }
        public VideoConfigDTO(object model) : base(model) { }
    }
}