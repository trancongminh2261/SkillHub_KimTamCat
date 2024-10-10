using LMS_Project.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.ExamPeriod
{
    public class ExamPeriodDTO : DomainDTO
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
        /// id chương trình học
        /// </summary>
        public int? VideoCourseId { get; set; }
        /// <summary>
        /// tên chương trình học
        /// </summary>
        public string VideoCourseName { get; set; }
        /// <summary>
        /// id đề kiểm tra
        /// </summary>
        public int ExamId { get; set; }
        /// <summary>
        /// mã đề kiểm tra
        /// </summary>
        public string ExamCode { get; set; }
        /// <summary>
        /// tên đề kiểm tra
        /// </summary>
        public string ExamName { get; set; }
        /// <summary>
        /// điểm sàn ( mức điểm cần đạt được để vượt qua kỳ thi )
        /// </summary>
        public double PassPoint { get; set; }
        /// <summary>
        /// thời gian gia hạn (bao nhiêu tháng)
        /// </summary>
        public int? ExtensionPeriod { get; set; }   
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
        /// <summary>
        /// trạng thái đã làm hay chưa
        /// </summary>
        public bool? IsAbleToSubmit { get; set; }
        /// <summary>
        /// trạng thái đã làm hay chưa
        /// </summary>
        public int? TotalSubmit { get; set; }
        public ExamPeriodDTO() : base() { }
        public ExamPeriodDTO(object model) : base(model) { }
    }
}