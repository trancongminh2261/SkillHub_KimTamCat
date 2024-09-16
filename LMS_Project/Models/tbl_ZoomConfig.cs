namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_ZoomConfig : DomainEntity
    {
        public string UserZoom { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
        public bool? Active { get; set; }
        public tbl_ZoomConfig() : base() { }
        public tbl_ZoomConfig(object model) : base(model) { }
    }
}