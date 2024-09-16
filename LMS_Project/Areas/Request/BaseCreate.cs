
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LMS_Project.LMS;
using static LMS_Project.Models.lmsEnum;
using LMS_Project.Enum;

namespace LMS_Project.Areas.Request
{
    public class BaseCreate
    {
        [JsonIgnore]
        public bool Enable { get; set; } = true;
        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
    public class VideoCourseAllowCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        public int VideoCourseId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn đối tượng được phép tham gia khóa học")]
        public List<int> ListValueId { get; set; }
        /// <summary>
        /// Phân loại là phòng ban hay nhân viên được phép tham gia khóa học này
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại đối tượng")]
        public VideoCourseAllowEnum.Type Type { get; set; }
    }
    
    public class MultiStudyRouteDetailCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lộ trình học")]
        public int StudyRouteId { get; set; }
        //khóa học
        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        public List<int> ListVideoCourseId { get; set; }
    }
    public class StudyRouteDetailCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lộ trình học")]
        public int StudyRouteId { get; set; }
        //khóa học
        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        public int VideoCourseId { get; set; }
    }
    public class StudyRouteCreate : BaseCreate
    {
        //nhân viên
        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        public int DepartmentId { get; set; }
        /// <summary>
        /// Lựa chọn có học bài theo thứ tự hay không
        /// mặc định là có
        /// true - có
        /// false - không
        /// </summary>
        public bool LearnInOrder { get; set; } = true;
    }
    public class DepartmentCreate : BaseCreate
    {
        public string Name { get; set; }
        public int parentDepartmentId { get; set; }
    }
    public class RootDepartmentCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
    }
    public class StandardCreate : BaseCreate
    {
        /// <summary>
        /// Tiêu chuẩn
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        /// <summary>
        /// Điểm
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập điểm")]
        public double? Point { get; set; }
        /// <summary>
        /// 1 - Dùng chung
        /// 2 - Dùng riêng
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// khóa học
        /// </summary>
        public int VideoCourseId { get; set; }
    }
    public class CertificateConfigCreate : BaseCreate
    {
        /// <summary>
        /// Vui lòng nhập tên chứng chỉ
        /// </summary>
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung chứng chỉ")]
        public string Content { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập hình nền chứng chỉ")]
        public string Background { get; set; }
        /// <summary>
        /// Mặt sau
        /// </summary>
        public string Backside { get; set; }
    }
    public class TagCreate : BaseCreate
    {
        public string Name { get; set; }
        /// <summary>
        /// 1 - Chủ đề
        /// 2 - Thời gian học
        /// 3 - Cấp lớp
        /// </summary>
        public int? Type { get; set; }
        public string TypeName { get {
                return Type == 1 ? "Chủ đề" : Type == 2 ? "Thời gian học" : Type == 3 ? "Cấp lớp" : "";
            } 
        }
    }
    public class MarriageCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class AcademicLevelCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class JobCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class MonthlyIncomeCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class JobOfFatherCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class JobOfMotherCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class JobOfSpouseCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class IncomeOfFamilyCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class PurposeCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class SourceCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui long nhập tên")]
        public string Name { get; set; }
    }
    public class SeminarRecordCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn Webinar")]
        public int? SeminarId { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
    }
    public class NotificationInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
        [JsonIgnore]
        public bool IsSend { get { return false; } }
    }
    public class ExerciseGroupInExamCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn phần trong đề thi")]
        public int? ExamSectionId { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cấp độ")]
        public ExerciseLevel? Level { get; set; }
        [JsonIgnore]
        public string LevelName
        {
            get { return Level == ExerciseLevel.Easy ? "Dễ" : Level == ExerciseLevel.Normal ? "Trung bình" : Level == ExerciseLevel.Difficult ? "Khó" : ""; }
        }
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public ExerciseType? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == ExerciseType.MultipleChoice ? "Trắc nghiệp"
                    : Type == ExerciseType.DragDrop ? "Kéo thả"
                    : Type == ExerciseType.FillInTheBlank ? "Điền vào ô trống" : "";
                //: Type == ExerciseType.Essay ? "Tự luận" : "";
            }
        }
        public List<ExerciseInExamCreate> ExerciseInExamCreates { get; set; }
    }
    public class ExerciseInExamCreate : BaseCreate
    {
        public string Content { get; set; }
        /// <summary>
        /// Áp dụng cho câu kéo thả và điền từ
        /// </summary>
        public string InputId { get; set; }
        public List<AnswerInExamCreate> AnswerInExamCreates { get; set; }
        public string DescribeAnswer { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập điểm")]
        public double? Point { get; set; }
    }
    public class AnswerInExamCreate : BaseCreate
    {
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
    }
    public class ExamSectionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn đề")]
        public int? ExamId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Nội dung chú thích đáp án đúng của đoạn văn
        /// </summary>
        public string Explanations { get; set; }
    }
    public class ExerciseGroupCreate : BaseCreate
    {

        [Required(ErrorMessage = "Vui lòng chọn phần trong đề thi")]
        public int? ExamSectionId { get; set; }
        public string Content { get; set; }
        public string Paragraph { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cấp độ")]
        public ExerciseLevel? Level { get; set; }
        [JsonIgnore]
        public string LevelName
        {
            get { return Level == ExerciseLevel.Easy ? "Dễ" : Level == ExerciseLevel.Normal ? "Trung bình" : Level == ExerciseLevel.Difficult ? "Khó" : ""; }
        }
        [Required(ErrorMessage = "Vui lòng chọn loại")]
        public ExerciseType? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return Type == ExerciseType.MultipleChoice ? "Trắc nghiệp"
                    : Type == ExerciseType.DragDrop ? "Kéo thả"
                    : Type == ExerciseType.FillInTheBlank ? "Điền vào ô trống"
                    : Type == ExerciseType.Essay ? "Tự luận" : "";
            }
        }
        /// <summary>
        /// Video của đề
        /// </summary>
        public string VideoUrl { get; set; }
        public List<ExerciseCreate> ExerciseCreates { get; set; }
    }
    public class ExerciseCreate : BaseCreate
    {
        public string Content { get; set; }
        /// <summary>
        /// Áp dụng cho câu kéo thả và điền từ
        /// </summary>
        public string InputId { get; set; }
        public List<AnswerCreate> AnswerCreates { get; set; }
        public string DescribeAnswer { get; set; }
        public double? Point { get; set; }
    }
    public class AnswerCreate : BaseCreate
    {
        public string AnswerContent { get; set; }
        public bool? IsTrue { get; set; }
    }

    public class ExamCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đề")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã đề")]
        public string Code { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public int NumberExercise { get { return 0; } }
        /// <summary>
        /// Thời gian làm bài
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập thời gian làm bài")]
        public int Time { get; set; }
        [JsonIgnore]
        public int? DifficultExercise { get { return 0; } }
        [JsonIgnore]
        public int? NormalExercise { get { return 0; } }
        [JsonIgnore]
        public int? EasyExercise { get { return 0; } }
        /// <summary>
        /// Số điểm đạt
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số điểm đạt")]
        public double PassPoint { get; set; }
        /// <summary>
        /// 1 - Kiến thức
        /// 2 - Kỹ năng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại đề")]
        public int? Type { get; set; }
    }
    public class ZoomConfigCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản zoom")]
        public string UserZoom { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập API Key")]
        public string APIKey { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập API Secret")]
        public string APISecret { get; set; }
        [JsonIgnore]
        public bool Active { get { return false; } }
    }
    public class SeminarCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên buổi Webinar")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime? StartTime { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime? EndTime { get; set; }
        public int VideoCourseId { get; set; } = 0;
        public int LeaderId { get; set; }
        [JsonIgnore]
        public int Status { get { return 1; } }
        [JsonIgnore]
        public string StatusName { get { return "Chưa diễn ra"; } }
        /// <summary>
        /// Số lượng người tham gia, bỏ trống là không giới hạn
        /// </summary>
        public int Member { get; set; } = 0;
        public string Thumbnail { get; set; }
        /// <summary>
        /// 1 - Online 
        /// 2 - Offline
        /// </summary>
        public int Type { get; set; }
        [JsonIgnore]
        public string TypeName {
            get {
                return Type == 1 ? "Online" : Type == 2 ? "Offline" : "";
            }
        }
    }
    public class CertificateCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        public string Content { get; set; }
    }
    public class AnswerInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Không tìm thấy câu hỏi")]
        public int? QuestionInVideoId { get; set; }
        public string Content { get; set; }
    }
    public class QuestionInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        public int? LessonVideoId { get; set; }
        public string Content { get; set; }
    }
    public class FileInVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn bài học")]
        public int? LessonVideoId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
    public class LessonVideoCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn phần")]
        public int? SectionId { get; set; }
        /// <summary>
        /// 1 - Video
        /// 2 - Bài tập
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại bài học")]
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get { return Type == ((int)LessonType.Video) ? "Video" : Type == ((int)LessonType.BaiTap) ? "Bài tập" : ""; }
        }
        public string Name { get; set; }
        /// <summary>
        /// File video
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// Bài tập
        /// </summary>
        public int? ExamId { get; set; }
        public LessonFileType? FileType { get; set; }
        public int Minute { get; set; }
        public string Thumbnail { get; set; }

    }

    public class LessonVideoCreateV2 : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn phần")]
        public int? SectionId { get; set; }
        /// <summary>
        /// 1 - Video
        /// 2 - Bài tập
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại bài học")]
        public int? Type { get; set; }
        [JsonIgnore]
        public string TypeName
        {
            get { return Type == ((int)LessonType.Video) ? "Video" : Type == ((int)LessonType.BaiTap) ? "Bài tập" : ""; }
        }
        public string Name { get; set; }
        /// <summary>
        /// File video
        /// </summary>
        public string VideoUrl { get; set; }
        public string VideoUploadId { get; set; }
        /// <summary>
        /// Bài tập
        /// </summary>
        public int? ExamId { get; set; }
        public LessonFileType? FileType { get; set; }
        public int Minute { get; set; }
        public string Thumbnail { get; set; }
    }

    public class SectionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn khoá học")]
        public int? VideoCourseId { get; set; }
        public string Name { get; set; }
    }
    public class VideoCourseCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên khóa học")]
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Điều kiện đã học khoá này
        /// </summary>
        public int BeforeCourseId { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn mẫu chứng chỉ")]
        public int? CertificateConfigId { get; set; }
        /// <summary>
        /// cho phép tất cả mọi người tham gia hay không
        /// </summary>
        public bool? IsPublic { get; set; } = true;
    }
    public class RegisterModel : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string FullName { get; set; }
        public string NickName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        //[JsonIgnore]
        //public string UserName { get { return CMND; } }
        public string Email { get {
                return checkEmail;
            } set
            { checkEmail = value.Trim(); }}
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        [JsonIgnore]
        public string checkEmail { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [JsonIgnore]
        public int Gender { get { return ((int)GenderEnum.khac); } }
        [JsonIgnore]
        public int? StatusId
        {
            get { return ((int)AccountStatus.active); }
        }
        [JsonIgnore]
        public int RoleId { get { return ((int)RoleEnum.student); } }
        [JsonIgnore]
        public string RoleName { get { return "Học viên"; } }
        [Required(ErrorMessage = "Vui lòng chọn tỉnh hoặc thành phố")]
        public int? AreaId { get; set; }
        public string Password { get; set; }
    }
    public class UserCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string FullName { get; set; }
        public string NickName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        //[JsonIgnore]
        //public string UserName { get { return CMND; } }
        public DateTime? DOB { get; set; } = DateTime.Now;
        /// <summary>
        /// 0 - Nữ 
        /// 1 - Nam
        /// 2 - Khác
        /// </summary>
        public int Gender { get; set; } = ((int)GenderEnum.khac);
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "vui lòng nhập email")]
        public string Email
        {
            get
            {
                return checkEmail;
            }
            set
            { checkEmail = value.Trim(); }
        }
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        [JsonIgnore]
        public string checkEmail { get; set; }
        public string Address { get; set; }
        [JsonIgnore]
        public int? StatusId
        {
            get { return ((int)AccountStatus.active); }
        }
        /// <summary>
        /// 1 - Admin
        /// 2 - Giáo viên
        /// 3 - Học viên
        /// </summary>
        [Required]
        public int RoleId { get; set; }
        [JsonIgnore]
        public string RoleName
        {
            get
            {
                return RoleId == ((int)RoleEnum.admin) ? "Admin"
                  : RoleId == ((int)RoleEnum.teacher) ? "Giáo viên"
                  : RoleId == ((int)RoleEnum.student) ? "Học viên"
                  : RoleId == ((int)RoleEnum.manager) ? "Quản lý"
                  : "";
            }
        }
        public string Avatar { get; set; }
        public int? AreaId { get; set; }
        public int DistrictId { get; set; } = 0;
        public int WardId { get; set; } = 0;
        public string Password
        {
            get
            {
                return Encryptor.Encrypt(SetPassword);
            }
            set
            {
                SetPassword = value;
            }
        }
        [JsonIgnore]
        public string SetPassword { get; set; }
        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string CMND { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class TopicCreate : BaseCreate
    {
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DocumentCreate : BaseCreate
    {
        [Required(ErrorMessage = "Topic is required")]
        public int? TopicId { get; set; }
        public string FileType { get; set; }
        [Required(ErrorMessage = "File is required")]
        public string FileName { get; set; }
        public string AbsolutePath { get; set; }
    }
    public class VideoConfigCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn bài học")]
        public int LessonVideoId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thời điểm cần dừng")]
        public int StopInMinute { get; set; }
        /// <summary>
        /// 1 - nhấn tiếp tục
        /// 2 - trả lời câu hỏi
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại cấu hình")]
        public VideoConfigEnum.Type Type { get; set; }
        public List<VideoConfigQuestionCreate> VideoConfigQuestions { get; set; }
    }
    public class VideoConfigQuestionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi")]
        public string Content { get; set; }
        public List<VideoConfigOptionCreate> VideoConfigOptions { get; set; }
    }
    public class VideoConfigOptionCreate : BaseCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung đáp án")]
        public string Content { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}