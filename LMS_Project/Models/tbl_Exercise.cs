namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Exercise : DomainEntity
    {
        public int? ExamId { get; set; }
        public int? ExamSectionId { get; set; }
        public int? ExerciseGroupId { get; set; }
        public string Content { get; set; }
        public string InputId { get; set; }
        public string DescribeAnswer { get; set; }
        /// <summary>
        /// Vị trí của câu trong nhóm
        /// </summary>
        public int? Index { get; set; }
        public double? Point { get; set; }
        public tbl_Exercise() : base() { }
        public tbl_Exercise(object model) : base(model) { }
    }
}
