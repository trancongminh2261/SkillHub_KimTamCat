using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LMS_Project.Models
{
    /// <summary>
    /// kỳ thi
    /// </summary>
    public class tbl_ExamPeriod : DomainEntity
    {
        /// <summary>
        /// mã kỳ thi
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// tên kỳ thi
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// thời gian bắt đầu
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// thời gian kết thúc
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// chương trình học
        /// </summary>
        public int? VideoCourseId { get; set; }
        /// <summary>
        /// đề kiểm tra
        /// </summary>
        public int ExamId { get; set; }
        /// <summary>
        /// thời gian gia hạn (bao nhiêu tháng)
        /// </summary>
        public int? ExtensionPeriod { get; set; }
        /// <summary>
        /// điểm sàn ( mức điểm cần đạt được để vượt qua kỳ thi )
        /// </summary>
        //public double PassingScore { get; set; }
        /// <summary>
        /// mô tả 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// UpComing - sắp diễn ra
        /// Opening - đang diễn ra
        /// Closed - đã kết thúc
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// số lượng tối đa
        /// </summary>
        public int? MaxQuantity { get; set; }
        public tbl_ExamPeriod() : base() { }
        public tbl_ExamPeriod(object model) : base(model) { }
    }
}