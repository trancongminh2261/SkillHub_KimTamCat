namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static LMS_Project.Models.lmsEnum;

    public class tbl_ExerciseGroup : DomainEntity
    {
        public int? ExamId { get; set; }
        public int? ExamSectionId { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        public int? ExerciseNumber { get; set; }
	    public ExerciseLevel? Level { get; set; }
        public string LevelName { get; set; }
        public ExerciseType? Type { get; set; }
        public string TypeName { get; set; }
        /// <summary>
        /// Câu gốc
        /// </summary>
        public int? SourceId { get; set; } 
        /// <summary>
        /// Vị trí của nhóm trong đề
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// Video của đề
        /// </summary>
        public string VideoUrl { get; set; }
        public tbl_ExerciseGroup() : base() { }
        public tbl_ExerciseGroup(object model) : base(model) { }
    }
    public class Get_ExerciseGroup
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        /// <summary>
        /// Video của đề
        /// </summary>
        public string VideoUrl { get; set; }
        public int? ExerciseNumber { get; set; }
        public ExerciseLevel? Level { get; set; }
        public string LevelName { get; set; }
        public ExerciseType? Type { get; set; }
        public string TypeName { get; set; }
        public int? ExerciseId { get; set; }
        public string ExerciseContent { get; set; }
        public string InputId { get; set; }
        public string DescribeAnswer { get; set; }
        public int? AnswerId { get; set; }
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
        public int? ExamSectionId { get; set; }
		public string ExamSectionName { get; set; }
		public string Explanations { get; set; }
		public int? ExamSectionIndex { get; set; }
		public int? ExerciseGroupIndex { get; set; }
		public double? Point { get; set; }
		public int? ExerciseIndex { get; set; }
        public bool? Enable { get; set; }
        public bool? ExerciseEnable { get; set; }
        public bool? AnswerEnable { get; set; }
        public int TotalRow { get; set; }
    }
    public class ExamSectionModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Nội dung chú thích đáp án đúng của đoạn văn
        /// </summary>
        public string Explanations { get; set; }
        public int? Index { get; set; }
        public List<ExerciseGroupModel> ExerciseGroups { get; set; }
    }
    public class ExerciseGroupModel
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        public int? ExerciseNumber { get; set; }
        public ExerciseLevel? Level { get; set; }
        public string LevelName { get; set; }
        public ExerciseType? Type { get; set; }
        public string TypeName { get; set; }
        public int? Index { get; set; }
        /// <summary>
        /// Đã tồn tại trong đề
        /// </summary>
        //public bool IsExist { get; set; }
        /// <summary>
        /// Vị trí câu bắt đầu trong đề
        /// </summary>
        public int? SIndexInExam { get
            { return Exercises.Count == 0 ? 0 : Exercises[0].IndexInExam; }}
        /// <summary>
        /// Vị trí câu kết thúc trong đề
        /// </summary>
        public int? EIndexInExam { get
            { return SIndexInExam == 0 ? 0 : SIndexInExam + (Exercises.Count - 1); }
        }
        /// <summary>
        /// Video của đề
        /// </summary>
        public string VideoUrl { get; set; }
        public List<ExerciseModel> Exercises { get; set; }
    }
    public class ExerciseModel
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public string InputId { get; set; }
        public string DescribeAnswer { get; set; }
        /// <summary>
        /// Vị trí câu trong đề
        /// </summary>
        public int? IndexInExam { get; set; }
        public int? Index { get; set; }
        public double? Point { get; set; }
        public int? Correct { get; set; }
        public List<AnswerModel> Answers { get; set;}
    }
    public class AnswerModel
    {
        public int? Id { get; set; }
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
    }
}