namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_Exam : DomainEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Số câu hỏi
        /// </summary>
        public int? NumberExercise { get; set; }
        /// <summary>
        /// Thời gian làm bài
        /// </summary>
        public int? Time { get; set; }
        /// <summary>
        /// Số câu khó
        /// </summary>
        public int? DifficultExercise { get; set; }
        /// <summary>
        /// Số câu trung bình
        /// </summary>
        public int? NormalExercise { get; set; }
        /// <summary>
        /// Số câu dễ
        /// </summary>
        public int? EasyExercise { get; set; }
        /// <summary>
        /// Số điểm đạt
        /// </summary>
        public double PassPoint { get; set; }
        /// <summary>
        /// 1 - Kiến thức
        /// 2 - Kỹ năng
        /// </summary>
        public int? Type { get; set; }
        [NotMapped]
        public string TypeName { get
            {
                return Type == 1 ? "Kiến thức" : Type == 2 ? "Kỹ năng" : "";
            }
        }
        public tbl_Exam() : base() { }
        public tbl_Exam(object model) : base(model) { }
    }
    public class Get_Exam : DomainEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Số câu hỏi
        /// </summary>
        public int? NumberExercise { get; set; }
        /// <summary>
        /// Thời gian làm bài
        /// </summary>
        public int? Time { get; set; }
        /// <summary>
        /// Số câu khó
        /// </summary>
        public int? DifficultExercise { get; set; }
        /// <summary>
        /// Số câu trung bình
        /// </summary>
        public int? NormalExercise { get; set; }
        /// <summary>
        /// Số câu dễ
        /// </summary>
        public int? EasyExercise { get; set; }
        /// <summary>
        /// Số điểm đạt
        /// </summary>
        public double PassPoint { get; set; }
        public double? TotalPoint { get; set; }
        /// <summary>
        /// 1 - Kiến thức
        /// 2 - Kỹ năng
        /// </summary>
        public int? Type { get; set; }
        public int TotalRow { get; set; }
    }
}