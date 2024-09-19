using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LMS_Project.Enum;
using Newtonsoft.Json;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.Request
{
    public class BaseUpdate
    {
        [Required(ErrorMessage = "Id is required")]
        public int? Id { get; set; }
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
    public class ExamPeriodUpdate : BaseUpdate
    {
        /// <summary>
        /// mã kỳ thi
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// tên kỳ thi
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// thời gian bắt đầu
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// thời gian kết thúc
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// chương trình học
        /// </summary>
        public int? VideoCourseId { get; set; }
        /// <summary>
        /// đề kiểm tra
        /// </summary>
        public int? ExamId { get; set; }
        /// <summary>
        /// thời gian gia hạn (bao nhiêu tháng)
        /// </summary>
        public int? ExtensionPeriod { get; set; }
        /// <summary>
        /// điểm sàn ( mức điểm cần đạt được để vượt qua kỳ thi )
        /// </summary>
        //public double? PassingScore { get; set; }
        /// <summary>
        /// mô tả 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// UpComing - sắp diễn ra
        /// Opening - đang diễn ra
        /// Closed - đã kết thúc
        /// </summary>
        //public ExamPeriodEnum.Status? Status { get; set; }
        /// <summary>
        /// số lượng người tham gia tối đa
        /// </summary>
        public int? MaxQuantity { get; set; }
    }
    public class IndexUpdate : BaseUpdate
    {
        public int? Index { get; set; }
    }
    public class StudyCourseUpdate : BaseUpdate
    {
        /// <summary>
        /// Lựa chọn có học bài theo thứ tự hay không
        /// true - có
        /// false - không
        /// </summary>
        public bool? LearnInOrder { get; set; }
    }
    public class DepartmentUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public bool? IsRoot { get; set; }
    }
    public class StandardUpdate : BaseUpdate
    {
        /// <summary>
        /// Tiêu chuẩn
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Điểm
        /// </summary>
        public double? Point { get; set; }
        /// <summary>
        /// 1 - Dùng chung
        /// 2 - Dùng riêng
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// khóa học
        /// </summary>
        public int? VideoCourseId { get; set; }
    }
    public class CertificateConfigUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        public string Background { get; set; }
        /// <summary>
        /// Mặt sau
        /// </summary>
        public string Backside { get; set; }
    }
    public class SeminarRecordUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string VideoUrl { get; set; }
    }
    public class ExamSectionUpdate : BaseUpdate
    {
        public string Name { get; set; }
        /// <summary>
        /// Nội dung chú thích đáp án đúng của đoạn văn
        /// </summary>
        public string Explanations { get; set; }
    }
    public class CertificateUpdate : BaseUpdate
    {
        public string Content { get; set; }
    }
    public class ExerciseGroupUpdate : BaseUpdate
    {
        public string Content { get; set; }
        public string Paragraph { get; set; }
        public ExerciseLevel? Level { get; set; }
        [JsonIgnore]
        public string LevelName
        {
            get { return Level == ExerciseLevel.Easy ? "Dễ" : Level == ExerciseLevel.Normal ? "Trung bình" : Level == ExerciseLevel.Difficult ? "Khó" : ""; }
        }
        /// <summary>
        /// Video của đề
        /// </summary>
        public string VideoUrl { get; set; }
        public List<ExerciseUpdate> ExerciseUpdates { get; set; }
    }
    public class ExerciseUpdate: BaseUpdate
    {
        public string Content { get; set; }
        /// <summary>
        /// Áp dụng cho câu kéo thả và điền từ
        /// </summary>
        public string InputId { get; set; }
        public List<AnswerUpdate> AnswerUpdates { get; set; }
        public string DescribeAnswer { get; set; }
        public int? Index { get; set; }
        public bool? Enable { get; set; }
        public double? Point { get; set; }
    }
    public class AnswerUpdate : BaseUpdate
    {
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
        public bool? Enable { get; set; }
    }
    public class ExamUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Thời gian làm bài
        /// </summary>
        public int? Time { get; set; }
        /// <summary>
        /// Số điểm đạt
        /// </summary>
        public double? PassPoint { get; set; }
    }
    public class ZoomConfigUpdate : BaseUpdate
    {
        public string UserZoom { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
    }
    public class SeminarUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? VideoCourseId { get; set; } 
        public int? LeaderId { get; set; }
        public int? Member { get; set; }
        public string Thumbnail { get; set; }
        /// <summary>
        /// 1 - Online 
        /// 2 - Offline
        /// </summary>
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == 1 ? "Online" : Type == 2 ? "Offline" : "";
            }
        }
    }
    public class LessonVideoUpdate : BaseUpdate
    {
        public string Name { get; set; }
        /// <summary>
        /// File video
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// Bài tập
        /// </summary>
        public int? ExamId { get; set; }
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get { return Type == ((int)LessonType.Video) ? "Video" : Type == ((int)LessonType.BaiTap) ? "Bài tập" : ""; }
        }
        public LessonFileType? FileType { get; set; }
        public int? Minute { get; set; }
        public string Thumbnail { get; set; }
    }
    public class LessonVideoUpdateV2 : BaseUpdate
    {
        public string Name { get; set; }
        /// <summary>
        /// File video
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// Bài tập
        /// </summary>
        public int? ExamId { get; set; }
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get { return Type == ((int)LessonType.Video) ? "Video" : Type == ((int)LessonType.BaiTap) ? "Bài tập" : ""; }
        }
        public LessonFileType? FileType { get; set; }
        public int? Minute { get; set; }
        public string Thumbnail { get; set; }
        public string VideoUploadId { get; set; }
    }
    public class SectionUpdate : BaseUpdate
    {
        public string Name { get; set; }
    }
    
    public class VideoCourseUpdate : BaseUpdate
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public int? BeforeCourseId { get; set; }
        public int? CertificateConfigId { get; set; }
        /// <summary>
        /// cho phép tất cả mọi người tham gia hay không
        /// </summary>
        public bool? IsPublic { get; set; } = true;
        /// <summary>
        /// thời hạn chứng chỉ
        /// </summary>
        public int? ExtensionPeriod { get; set; }
    }
    public class ResetPasswordModel
    {
        public string Key { get; set; }
        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [StringLength(128, ErrorMessage = "Mật khẩu phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc nhập")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu mới")]
        [StringLength(128, ErrorMessage = "Mật khẩu xác nhận phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Compare("NewPassword",ErrorMessage = "Mật khẩu xác nhận không đúng")]
        public string ConfirmNewPassword { get; set; }
    }
    public class ChangePasswordModel
    {
        /// <summary>
        /// Mật khẩu cũ
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [StringLength(128, ErrorMessage = "Mật khẩu phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc nhập")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu mới")]
        [StringLength(128, ErrorMessage = "Mật khẩu xác nhận phải có ít nhất 6 kí tự và tối đa 128 ký tự", MinimumLength = 6)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không đúng")]
        public string ConfirmNewPassword { get; set; }
    }
    public class UserUpdate
    {
        [Key]
        public int UserInformationId { get; set; }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public DateTime? DOB { get; set; } 
        public int? Gender { get; set; }
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; } 
        public string Address { get; set; }
        /// <summary>
        /// 0 - Hoạt động
        /// 1 - Khoá
        /// </summary>
        public int? StatusId { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } 
        public int? DistrictId { get; set; }
        public int? WardId { get; set; } 
        public string Password { get; set; }
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string CMND { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class TopicUpdate : BaseUpdate
    {
        public string Code{ get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DocumentUpdate : BaseUpdate
    {
        public int? TopicId { get; set; }
        public string FileName { get; set; }
        public string AbsolutePath { get; set; }
        public string FileType { get; set; }
    }
    public class VideoConfigUpdate : BaseUpdate
    {
        public int? StopInMinute { get; set; }
        public VideoConfigEnum.Type? Type { get; set; }
        public List<VideoConfigQuestionUpdate> VideoConfigQuestions { get; set; }
    }
    public class VideoConfigQuestionUpdate : BaseUpdate
    {
        public string Content { get; set; }
        public int? Index { get; set; }
        public bool? Enable { get; set; }
        public List<VideoConfigOptionUpdate> VideoConfigOptions { get; set; }
    }
    public class VideoConfigOptionUpdate : BaseUpdate
    {
        public string Content { get; set; }
        public bool? IsCorrect { get; set; }
        public bool? Enable { get; set; }
    }
}