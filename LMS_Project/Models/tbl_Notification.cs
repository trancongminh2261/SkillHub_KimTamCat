
namespace LMS_Project.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class tbl_Notification : DomainEntity
    {
        public int? UserId { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }
        /// <summary>
        /// Đã xem
        /// </summary>
        public bool? IsSeen { get; set; }
        public tbl_Notification() : base() { }
        public tbl_Notification(object model) : base(model) { }
    }
    public class Get_Notification : DomainEntity
    {
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Đã xem
        /// </summary>
        public bool? IsSeen { get; set; }
        public int TotalRow { get; set; }
    }
}
