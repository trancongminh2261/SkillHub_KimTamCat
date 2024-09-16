namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_QuestionInVideo : DomainEntity
    {
        public int? VideoCourseId { get; set; }
        public int? LessonVideoId { get; set; }
        public int? UserId { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string Avatar { get; set; }
        public string Content { get; set; }
        public tbl_QuestionInVideo() : base() { }
        public tbl_QuestionInVideo(object model) : base(model) { }
    }
    public class Get_QuestionInVideo : DomainEntity
    {
        public int? VideoCourseId { get; set; }
        public int? LessonVideoId { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public int TotalRow { get; set; }
    }
}