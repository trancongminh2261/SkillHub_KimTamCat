namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_Certificate : DomainEntity
    {
        public int? UserId { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        public string Background { get; set; }
        public int VideoCourseId { get; set; }
        public string PDFUrl { get; set; }
        /// <summary>
        /// Mặt sau
        /// </summary>
        public string Backside { get; set; }
        /// <summary>
        /// ngày hết hạn
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// Valid - còn hạn
        /// Expired - hết hạn
        /// Indefinitely - vô thời hạn
        /// </summary>
        public string Status { get; set; }
        [NotMapped]
        public string VideoCourseName { get; set; }
        [NotMapped]
        public string VideoCourseThumbnail { get; set; }
        [NotMapped]
        public string UserCode { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string Mobile { get; set; } // số điện thoại
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string Avatar { get; set; }
        public tbl_Certificate() : base() { }
        public tbl_Certificate(object model) : base(model) { }
    }
    public class Get_Certificate : DomainEntity
    {
        public int? UserId { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        public string Background { get; set; }
        public int VideoCourseId { get; set; }
        /// <summary>
        /// Mặt sau
        /// </summary>
        public string Backside { get; set; }
        public string PDFUrl { get; set; }
        public string VideoCourseName { get; set; }
        public string VideoCourseThumbnail { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        /// <summary>
        /// ngày hết hạn
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// nếu hôm nay lớn hơn ngày hết hạn thì trạng thái hết hạn ngược lại thì còn hạn
        /// </summary>
        public string Status { get; set; }
        public int TotalRow { get; set; }
    }
}