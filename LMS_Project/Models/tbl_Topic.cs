namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Topic : DomainEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public tbl_Topic() : base() { }
        public tbl_Topic(object model) : base(model) { }
    }
}
