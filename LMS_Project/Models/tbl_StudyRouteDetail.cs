using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_StudyRouteDetail : DomainEntity
    {
        /// <summary>
        /// Lộ trình học
        /// </summary>
        public int StudyRouteId { get; set; }
        /// <summary>
        /// Khóa học
        /// </summary>
        public int VideoCourseId { get; set; }
        /// <summary>
        /// Thứ tự học
        /// </summary>
        public int Index { get; set; }
        [NotMapped]
        public string Name { get; set; }

        public tbl_StudyRouteDetail() : base() { }
        public tbl_StudyRouteDetail(object model) : base(model) { }
    }
}