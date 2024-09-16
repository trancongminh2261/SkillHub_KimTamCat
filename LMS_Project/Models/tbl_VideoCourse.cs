namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_VideoCourse : DomainEntity
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }       
        public string Tags { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public int BeforeCourseId { get; set; }
        /// <summary>
        /// Chứng chỉ mẫu
        /// </summary>
        public int? CertificateConfigId { get; set; }
        /// <summary>
        /// cho phép tất cả mọi người tham gia hay không
        /// </summary>
        public bool? IsPublic { get; set; }
        [NotMapped]
        public string CertificateConfigName { get; set; }
        [NotMapped]
        public string BeforeCourseName { get; set; }
        public double TotalRate { get; set; }
        public double TotalStudent { get; set; }
        [NotMapped]
        public bool Disable { get; set; }
        [NotMapped]
        public int TotalLesson { get; set; }
        [NotMapped]
        public int TotalSection { get; set; }
        [NotMapped]
        public int TotalMinute { get; set; }
        [NotMapped]
        public List<TagModel> TagModels { get; set; }
        [NotMapped]
        public int Status { get; set; }
        public tbl_VideoCourse() : base() { }
        public tbl_VideoCourse(object model) : base(model) { }
    }
    public class TagModel
    { 
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Get_VideoCourse : DomainEntity
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public int BeforeCourseId { get; set; }
        public string BeforeCourseName { get; set; }
        /// <summary>
        /// Chứng chỉ mẫu
        /// </summary>
        public int? CertificateConfigId { get; set; }
        public string CertificateConfigName { get; set; }
        /// <summary>
        /// cho phép tất cả mọi người tham gia hay không
        /// </summary>
        public bool? IsPublic { get; set; }
        public int Status { get; set; }
        public double TotalRate { get; set; }
        public double TotalStudent { get; set; }
        public int TotalLesson { get; set; }
        public int TotalSection { get; set; }
        public int TotalMinute { get; set; }
        public double CompletedPercent { get; set; }
        public int TotalRow { get; set; }
    }
    public class VideoCourseByStudent : DomainEntity
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        //public int BeforeCourseId { get; set; }
        //public string BeforeCourseName { get; set; }
        /// <summary>
        /// Chứng chỉ mẫu
        /// </summary>
        public int? CertificateConfigId { get; set; }
        public string CertificateConfigName { get; set; }
        /// <summary>
        /// cho phép tất cả mọi người tham gia hay không
        /// </summary>
        public bool? IsPublic { get; set; }
        public double TotalRate { get; set; }
        public int TotalLesson { get; set; }
        public int TotalSection { get; set; }
        public int TotalMinute { get; set; }
        public double TotalStudent { get; set; }
        /// <summary>
        /// 1 - Chưa học
        /// 2 - Đang học 
        /// 3 - Hoàn thành
        /// </summary>
        public int Status { get; set; }
        public string StatusName {
            get {
                return Status == 1 ? "Chưa học"
                   : Status == 2 ? "Đang học"
                   : Status == 3 ? "Hoàn thành" : "";
            }
        }
        public bool Disable { get; set; }
        public double CompletedPercent { get; set; }
        public List<TagModel> TagModels { get; set; }
    }
}