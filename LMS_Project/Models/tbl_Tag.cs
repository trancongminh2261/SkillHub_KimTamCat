namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_Tag : DomainEntity
    {
        public string Name { get; set; }
        /// <summary>
        /// 1 - Chủ đề
        /// 2 - Thời gian học
        /// 3 - Cấp lớp
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        public tbl_Tag() : base() { }
        public tbl_Tag(object model) : base(model) { }
    }
}