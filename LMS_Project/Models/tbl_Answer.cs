namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;

    public class tbl_Answer : DomainEntity
    {
        public int? ExerciseId { get; set; }
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
        public tbl_Answer() : base() { }
        public tbl_Answer(object model) : base(model) { }
    }
}