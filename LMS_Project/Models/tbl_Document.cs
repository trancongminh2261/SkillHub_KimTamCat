namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Document: DomainEntity
    {
        public int? TopicId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string AbsolutePath { get; set; }
        [NotMapped]
        public string TopicName { get; set; }

        public tbl_Document() : base() { }
        public tbl_Document(object model) : base(model) { }
    }
}
