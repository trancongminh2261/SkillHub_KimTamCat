namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_LessonCompleted : DomainEntity
    {
        public int? UserId { get; set; }
        public int? SectionId { get; set; }
        public int? LessonVideoId { get; set; }
        public int? ExamResultId { get; set; }
        public double? TotalPoint { get; set; }
        public tbl_LessonCompleted() : base() { }
        public tbl_LessonCompleted(object model) : base(model) { }
    }
}