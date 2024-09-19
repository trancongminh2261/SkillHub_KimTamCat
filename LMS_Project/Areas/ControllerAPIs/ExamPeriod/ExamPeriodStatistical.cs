using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Areas.ControllerAPIs.ExamPeriod
{
    public class ExamPeriodStatistical
    {
        /// <summary>
        /// mã kỳ thi
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// tên kì thi
        /// </summary>
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// số lượng học viên được thêm vào kì thi
        /// </summary>
        public int TotalJoined { get; set; }
        /// <summary>
        /// số lượng học viên nộp bài
        /// </summary>
        public int TotalSubmit { get; set; }
        /// <summary>
        /// số lượng học viên không nộp bài
        /// </summary>
        public int TotalNotSubmit { get; set; }
        /// <summary>
        /// số lượng học viên đậu
        /// </summary>
        public int TotalPass { get; set; }
        /// <summary>
        /// số lượng học viên rớt
        /// </summary>
        public int TotalFail { get; set; }
        /// <summary>
        /// tỷ lệ học viên đậu
        /// </summary>
        public double RatePass { get; set; }
        /// <summary>
        /// tỷ lệ học viên rớt
        /// </summary>
        public double RateFail { get; set; }
    }
}