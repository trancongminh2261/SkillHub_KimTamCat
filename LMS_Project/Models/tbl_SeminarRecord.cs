namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;

    public class tbl_SeminarRecord : DomainEntity
    {
        public int? SeminarId { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
        public tbl_SeminarRecord() : base() { }
        public tbl_SeminarRecord(object model) : base(model) { }
    }
}