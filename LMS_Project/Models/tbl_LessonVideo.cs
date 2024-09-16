namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;

    public class tbl_LessonVideo : DomainEntity
    {
        public int? SectionId { get; set; }
        /// <summary>
        /// 1 - Video
        /// 2 - Bài tập
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        public LessonFileType FileType { get; set; } 
        public string Name { get; set; }
        public int? Index { get; set; }
        public string VideoUrl { get; set; }
        public int? ExamId { get; set; }
        public int Minute { get; set; }
        public string Thumbnail { get; set; }
        public int? AntiDownVideoId { get; set; }
        [NotMapped]
        public string VideoUploadId { get; set; }
        public tbl_LessonVideo() : base() { }
        public tbl_LessonVideo(object model) : base(model) { }
    }
    public class LessonVideoModel
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        /// <summary>
        /// 1 - Video
        /// 2 - Bài tập
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public int? Index { get; set; }
        public string VideoUrl { get; set; }
        public int? ExamId { get; set; }
        public bool isCompleted { get; set; }
        public int Minute { get; set; }
        public string Thumbnail { get; set; }
        public int? AntiDownVideoId { get; set; }
        public string VideoUploadId { get; set; }
        /// <summary>
        /// Có file
        /// </summary>
        public bool HasFile { get; set; }
        public LessonFileType FileType { get; set; }
    }
}