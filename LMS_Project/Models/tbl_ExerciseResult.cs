namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_ExerciseResult : DomainEntity
    {
        public int? ExamResultId { get; set; }
        public int? ExamSectionResultId { get; set; }
        public int? ExerciseGroupResultId { get; set; }
        public int? ExerciseId { get; set; }
        public string Content { get; set; }
        public string InputId { get; set; }
        public string DescribeAnswer { get; set; }
        /// <summary>
        /// Vị trí của câu trong nhóm
        /// </summary>
        public int? Index { get; set; }
        public double? Point { get; set; }
        public bool? IsResult { get; set; }
        public tbl_ExerciseResult() : base() { }
        public tbl_ExerciseResult(object model) : base(model) { }
    }
}