using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    /// <summary>
    /// kết quả của kỳ thi
    /// </summary>
    public class tbl_ExamPeriodResult : DomainEntity
    {
        /// <summary>
        /// nhân viên
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// kỳ thi
        /// </summary>
        public int ExamPeriodId { get; set; }
        /// <summary>
        /// đề thi
        /// </summary>
        public int? ExamId { get; set; }
        /// <summary>
        /// thời gian bắt đầu làm bài
        /// </summary>
        public DateTime? StartDoingTime { get; set; }
        /// <summary>
        /// thời gian kết thúc làm bài
        /// </summary>
        public DateTime? EndDoingTime { get; set; }
        /// <summary>
        /// điểm đạt được
        /// </summary>
        public double? MyPoint { get; set; }
        /// <summary>
        /// tổng điểm của đề
        /// </summary>
        public double? TotalPoint { get; set; }
        /*/// <summary>
        /// trạng thái chấm bài 
        /// </summary>
        public string Status { get; set; }*/
        /// <summary>
        /// kết quả 
        /// Pass - đậu
        /// Fail - rớt
        /// </summary>
        public string Result { get; set; }
        public tbl_ExamPeriodResult() : base() { }
        public tbl_ExamPeriodResult(object model) : base(model) { }
    }
}