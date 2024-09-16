namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_AnswerInVideo : DomainEntity
    {
        public int? QuestionInVideoId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; }
        public tbl_AnswerInVideo() : base() { }
        public tbl_AnswerInVideo(object model) : base(model) { }
    }
    public class AnswerInVideoModel : DomainEntity
    {
        public int? QuestionInVideoId { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
    }
}