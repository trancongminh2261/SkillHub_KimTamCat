namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;
    public class tbl_AnswerResult : DomainEntity
    {
        public int? ExerciseResultId { get; set; }
        public int? AnswerId { get; set; }
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
        public int? MyAnswerId { get; set; }
        public string MyAnswerContent { get; set; }
        public bool? MyResult { get; set; }
        public string VideoUrl { get; set; }
        public tbl_AnswerResult() : base() { }
        public tbl_AnswerResult(object model) : base(model) { }
    }
}