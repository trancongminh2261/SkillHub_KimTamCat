namespace LMS_Project.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class lmsDbContext : DbContext
    {
        public lmsDbContext()
            : base("name=DbContext")
        {
        }
        //public virtual DbSet<tbl_ExamPeriodResult> tbl_ExamPeriodResult { get; set; }
        public virtual DbSet<tbl_UserInExamPeriod> tbl_UserInExamPeriod { get; set; }
        public virtual DbSet<tbl_ExamPeriod> tbl_ExamPeriod { get; set; }
        public virtual DbSet<tbl_VideoConfigOption> tbl_VideoConfigOption { get; set; }
        public virtual DbSet<tbl_VideoConfigQuestion> tbl_VideoConfigQuestion { get; set; }
        public virtual DbSet<tbl_VideoConfig> tbl_VideoConfig { get; set; }
        public virtual DbSet<tbl_AntiDownVideo> tbl_AntiDownVideo { get; set; }
        public virtual DbSet<tbl_VideoCourseAllow> tbl_VideoCourseAllow { get; set; }
        public virtual DbSet<tbl_WriteLog> tbl_WriteLog { get; set; }
        public virtual DbSet<tbl_StudyRouteDetail> tbl_StudyRouteDetail { get; set; }
        public virtual DbSet<tbl_StudyRoute> tbl_StudyRoute { get; set; }
        public virtual DbSet<tbl_Department> tbl_Department { get; set; }
        public virtual DbSet<tbl_TimeWatchingVideo> tbl_TimeWatchingVideo { get; set; }
        public virtual DbSet<tbl_GradingEssay> tbl_GradingEssay { get; set; }
        public virtual DbSet<tbl_Standard> tbl_Standard { get; set; }
        public virtual DbSet<tbl_Permission> tbl_Permission { get; set; }
        public virtual DbSet<tbl_Tag> tbl_Tag { get; set; }
        public virtual DbSet<tbl_SeminarRecord> tbl_SeminarRecord { get; set; }
        public virtual DbSet<tbl_AnswerResult> tbl_AnswerResult { get; set; }
        public virtual DbSet<tbl_ExerciseResult> tbl_ExerciseResult { get; set; }
        public virtual DbSet<tbl_ExerciseGroupResult> tbl_ExerciseGroupResult { get; set; }
        public virtual DbSet<tbl_ExamSectionResult> tbl_ExamSectionResult { get; set; }
        public virtual DbSet<tbl_ExamResult> tbl_ExamResult { get; set; }
        public virtual DbSet<tbl_ZoomRoom> tbl_ZoomRoom { get; set; }
        public virtual DbSet<tbl_ZoomConfig> tbl_ZoomConfig { get; set; }
        public virtual DbSet<tbl_Seminar> tbl_Seminar { get; set; }
        public virtual DbSet<tbl_Certificate> tbl_Certificate { get; set; }
        public virtual DbSet<tbl_CertificateConfig> tbl_CertificateConfig { get; set; }
        public virtual DbSet<tbl_SectionCompleted> tbl_SectionCompleted { get; set; }
        public virtual DbSet<tbl_LessonCompleted> tbl_LessonCompleted { get; set; }
        public virtual DbSet<tbl_AnswerInVideo> tbl_AnswerInVideo { get; set; }
        public virtual DbSet<tbl_QuestionInVideo> tbl_QuestionInVideo { get; set; }
        public virtual DbSet<tbl_VideoCourseStudent> tbl_VideoCourseStudent { get; set; }
        public virtual DbSet<tbl_FileInVideo> tbl_FileInVideo { get; set; }
        public virtual DbSet<tbl_LessonVideo> tbl_LessonVideo { get; set; }
        public virtual DbSet<tbl_Section> tbl_Section { get; set; }
        public virtual DbSet<tbl_VideoCourse> tbl_VideoCourse { get; set; }
        public virtual DbSet<tbl_ChangeInfo> tbl_ChangeInfo { get; set; }
        public virtual DbSet<tbl_Config> tbl_Config { get; set; }
        public virtual DbSet<tbl_Area> tbl_Area { get; set; }
        public virtual DbSet<tbl_District> tbl_District { get; set; }
        public virtual DbSet<tbl_Notification> tbl_Notification { get; set; }
        public virtual DbSet<tbl_NotificationInVideo> tbl_NotificationInVideo { get; set; }
        public virtual DbSet<tbl_UserInformation> tbl_UserInformation { get; set; }
        public virtual DbSet<tbl_Ward> tbl_Ward { get; set; }
        public virtual DbSet<tbl_ExerciseGroup> tbl_ExerciseGroup { get; set; }
        public virtual DbSet<tbl_Exercise> tbl_Exercise { get; set; }
        public virtual DbSet<tbl_Answer> tbl_Answer { get; set; }
        public virtual DbSet<tbl_Exam> tbl_Exam { get; set; }
        public virtual DbSet<tbl_ExamSection> tbl_ExamSection { get; set; }
        public virtual DbSet<tbl_Topic> tbl_Topic{ get; set; }
        public virtual DbSet<tbl_Document> tbl_Document{ get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
