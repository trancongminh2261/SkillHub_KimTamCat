namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Threading.Tasks;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_GradingEssay : DomainEntity
    {
        /// <summary>
        /// Tiêu chuẩn đánh giá
        /// </summary>
        public int StandardId { get; set; }
        /// <summary>
        /// Tiêu chuẩn
        /// </summary>
        [NotMapped]
        public string StandardName { get; set; }
        /// <summary>
        /// Điểm
        /// </summary>
        [NotMapped]
        public double StandardPoint { get; set; }
        /// <summary>
        /// Kết quả làm bài của học viên
        /// </summary>
        public int ExamResultId { get; set; }
        public tbl_GradingEssay() : base() { }
        public tbl_GradingEssay(object model) : base(model) { }
    }
}