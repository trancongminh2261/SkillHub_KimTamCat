namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_ExamSection : DomainEntity
    {
        public int? ExamId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Nội dung chú thích đáp án đúng của đoạn văn
        /// </summary>
        public string Explanations { get; set; }
        public int? Index { get; set; }
        public tbl_ExamSection() : base() { }
        public tbl_ExamSection(object model) : base(model) { }
    }
}