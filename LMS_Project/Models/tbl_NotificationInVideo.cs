namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_NotificationInVideo : DomainEntity
    {
        public int? VideoCourseId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool? IsSend { get; set; }
        public tbl_NotificationInVideo() : base() { }
        public tbl_NotificationInVideo(object model) : base(model) { }
    }
    public class Get_NotificationInVideo : DomainEntity
    {
        public int? VideoCourseId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool? IsSend { get; set; }
        public int TotalRow { get; set; }
    }
}
