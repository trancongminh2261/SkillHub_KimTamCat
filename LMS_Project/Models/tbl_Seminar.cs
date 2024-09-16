namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_Seminar : DomainEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? VideoCourseId { get; set; }
        public int LeaderId { get; set; }
        /// <summary>
        /// 1 - Chưa diễn ra
        /// 2 - Đang diễn ra
        /// 3 - Kết thúc
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Số lượng người tham gia
        /// </summary>
        public int? Member { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 1 - Online 
        /// 2 - Offline
        /// </summary>
        public string TypeName { get; set; }
        public tbl_Seminar() : base() { }
        public tbl_Seminar(object model) : base(model) { }
    }
    public class Get_Seminar : DomainEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? VideoCourseId { get; set; }
        public string VideoCourseName { get; set; }
        public int LeaderId { get; set; }
        public string LeaderName { get; set; }
        /// <summary>
        /// 1 - Chưa diễn ra
        /// 2 - Đang diễn ra
        /// 3 - Kết thúc
        /// </summary>
        public int? Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Số lượng người tham gia
        /// </summary>
        public int? Member { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 1 - Online 
        /// 2 - Offline
        /// </summary>
        public string TypeName { get; set; }
        public string RoomId { get; set; }
        public string RoomPass { get; set; }
        public string SignatureTeacher { get; set; }
        public string SignatureStudent { get; set; }
        public string APIKey { get; set; }
        public int TotalRow { get; set; }
    }
    public class SeminarModel : DomainEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? VideoCourseId { get; set; }
        public string VideoCourseName { get; set; }
        public int LeaderId { get; set; }
        public string LeaderName { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Số lượng người tham gia
        /// </summary>
        public int? Member { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 1 - Online 
        /// 2 - Offline
        /// </summary>
        public string TypeName { get; set; }
        public string RoomId { get; set; }
        public string RoomPass { get; set; }
        public string SignatureTeacher { get; set; }
        public string SignatureStudent { get; set; }
        public string APIKey { get; set; }
        public SeminarModel() : base() { }
        public SeminarModel(object model) : base(model) { }
    }
}