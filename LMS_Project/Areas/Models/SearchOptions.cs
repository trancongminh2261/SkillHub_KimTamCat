using LMS_Project.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.Models
{

    public class StandardSearch : SearchOptions
    {
        /// <summary>
        /// Lọc theo khóa video
        /// </summary>
        public int VideoCourseId { get; set; }
    }
    public class StudyRouteSearch : SearchOptions
    {
        public int? DepartmentId { get; set; }
    }
    public class OverviewSearch : SearchOptions
    {
        public string Search { get; set; }
    }
    public class NotificationInVideoSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
    }
    public class CertificateSearch : SearchOptions
    {
        public int UserId { get; set; } = 0;
        public int VideoCourseId { get; set; } = 0;
    }
    public class ExamResultSearch : SearchOptions
    {
        public int? ExamPeriodId { get; set; }
        public int? ExamId { get; set; }
        public int? DepartmentId { get; set; }
        public int StudentId { get; set; } = 0;
        public int VideoCourseId { get; set; } = 0;
        /// <summary>
        /// 1 - Chờ chấm bài
        /// 2 - Đã chấm bài
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Lọc theo giáo viên
        /// </summary>
        public int TeacherId { get; set; }
    }
    public class ExerciseGroupSearch : SearchOptions
    {
        public int Id { get; set; } = 0;
        /// <summary>
        /// Kiểm tra tồn tại trong đề, truyền vào ExamId
        /// </summary>
        public int NotExistInExam { get; set; }
        public ExerciseLevel? Level { get; set; }
        public ExerciseType? Type { get; set; }
    }
    public class ExamSearch : SearchOptions
    { 
        public string Search { get; set; }
    }
    public class QuestionInVideoSearch : SearchOptions
    {
        public int VideoCourseId { get; set; }
        public int LessonVideoId { get; set; }
        public int UserId { get; set; }
    }
    public class SeminarSearch : SearchOptions
    {
        public string Name { get; set; }
        public SeminarStatus? Status { get; set; }
    }
    public class VideoCourseStudentSearch : SearchOptions
    {
        /// <summary>
        /// mẫu : code,be
        /// </summary>
        public string Stags { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class StudentInVideoCourseSearch : SearchOptions
    {
        [Required]
        public int VideoCourseId { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// yyyy/MM/dd
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// yyyy/MM/dd
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
    public class VideoCourseSearch : SearchOptions
    {
        /// <summary>
        /// mẫu : 1,2
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// 1 - Chưa học 
        /// 2 - Đang học 
        /// 3 - Hoàn thành
        /// </summary>
        public int? Status { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
        /// <summary>
        /// Truyền 1,2,3
        /// </summary>
        public string DepartmentIds { get; set; }
    }

    public class VideoCourseSearchV2 : SearchOptions
    {
        /// <summary>
        /// mẫu : 1,2
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// 1 - Chưa học 
        /// 2 - Đang học 
        /// 3 - Hoàn thành
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
        /// <summary>
        /// phòng ban
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// cho phép tất cả đối tượng tham gia hoặc không
        /// </summary>
        public bool? IsPublic { get; set; }
    }

    public class ChangeInfo : SearchOptions
    {
        public string UserCode { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Statuss { get; set; }
    }
    public class UserSearch : SearchOptions
    {
        /// <summary>
        /// 0 - Ngày tạo
        /// 1 - Tên 
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; } = false;
        public string FullName { get; set; }
        public string UserCode { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string RoleIds { get; set; }
        /// <summary>
        /// mẫu 1,2
        /// </summary>
        public string Genders { get; set; }
        /// <summary>
        /// phòng ban
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// trạng thái
        /// </summary>
        public int? StatusId { get; set; }
    }
    public class WardSearch : SearchOptions
    { 
        public int? DistrictId { get; set; }
        public string Name { get; set; }
    }
    public class SetPackageResultDetailSearch : SearchOptions
    { 
        public int? SetPackageResultId { get; set; }
        public int? ExerciseType { get; set; }
        public int? Sort { get; set; }
        public bool SortType { get; set; } //true: tăng dần, false: giảm dần
    }
    public class StudentExistResult : SearchOptions
    { 
        public string FullName { get; set; }
        public int? Sort { get; set; }
        public bool SortType { get; set; } //true: tăng dần, false: giảm dần
    }
    public class DistrictSearch : SearchOptions
    { 
        public int? AreaId { get; set; }
        public string Name { get; set; }
    }
    public class AreaSearch : SearchOptions
    { 
        public string Name { get; set; }
    }
    public class TopicSearch : SearchOptions
    {
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }

    public class DocumentSearch : SearchOptions
    {
        public int? TopicId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 0 - Id
        /// 1 - Name
        /// </summary>
        public int Sort { get; set; } = 0;
        /// <summary>
        /// true: tăng dần, false: giảm dần
        /// </summary>
        public bool SortType { get; set; }
    }
    public class VideoCourseAllowSearch : SearchOptions
    {
        [Required(ErrorMessage ="Vui lòng chọn loại đối tượng")]
        public VideoCourseAllowEnum.Type Type { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khóa video")]
        public int VideoCourseId { get; set; }
    }
    public class VideoCourseAllowAvaibleSearch
    {
        [Required(ErrorMessage = "Vui lòng chọn loại đối tượng")]
        public VideoCourseAllowEnum.Type Type { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khóa video")]
        public int VideoCourseId { get; set; }

    }
    public class UserInExamPeriodSearch : SearchOptions
    {
        [Required(ErrorMessage = "Vui lòng chọn kỳ thi")]
        public int ExamPeriodId { get; set; }
    }
    public class UserInExamPeriodAvailableSearch
    {
        [Required(ErrorMessage = "Vui lòng chọn kỳ thi")]
        public int ExamPeriodId { get; set; }
        public int? DepartmentId { get; set; }
    }
    public class StatisticalSearch : SearchOptions
    {
        public int? Month { get; set; } = DateTime.Now.Month;
        public int? Year { get; set; } = DateTime.Now.Year;
    }
    public class SearchOptions
    {
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;
        public string Search { get; set; }
    }
}
