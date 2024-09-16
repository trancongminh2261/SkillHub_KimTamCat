using LMS_Project.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO
{
    public class VideoCourseAllowDTO : DomainDTO
    {
        public int VideoCourseId { get; set; }
        public int? ValueId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Phân loại là phòng ban hay nhân viên được phép tham gia khóa học này
        /// </summary>
        public string Type { get; set; }
        public VideoCourseAllowDTO() : base() { }
        public VideoCourseAllowDTO(object model) : base(model) { }
    }
}