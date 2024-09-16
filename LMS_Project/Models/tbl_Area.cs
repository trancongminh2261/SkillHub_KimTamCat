namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Area : DomainEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string Abbreviation { get; set; }
        public tbl_Area() : base() { }
        public tbl_Area(object model) : base(model) { }
    }
}
