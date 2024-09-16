using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;
using Newtonsoft.Json;

namespace LMS_Project.Services
{
    public class ExamResultService
    {
        public class ExamSubmit
        {
            [Required(ErrorMessage = "Vui lòng nhập đề thi")]
            public int? ExamId { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn bài học")]
            public int? LessonVideoId { get; set; }
            public List<ExerciseSubmit> Items { get; set; }

        }
        public class ExerciseSubmit
        {
            [Required(ErrorMessage = "Vui lòng chọn đầy đủ thông tin")]
            public int? ExerciseId { get; set; }
            /// <summary>
            /// đáp án được chọn
            /// </summary>
            public List<AnswerSubmit> Answers { get; set; }
        }
        public class AnswerSubmit
        {
            /// <summary>
            /// Đối với dạng 
            /// </summary>
            public int? AnswerId { get; set; }
            public string AnswerContent { get; set; }
            public string VideoUrl { get; set; }
        }
        /// <summary>
        /// Xữ lý nộp bài
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<tbl_ExamResult> Submit(ExamSubmit model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var exam = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == model.ExamId);
                        if (exam == null)
                            throw new Exception("Không tìm thấy đề");
                        var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == model.LessonVideoId);
                        int sectionId = lessonVideo?.SectionId ?? 0;
                        var section = await db.tbl_Section.SingleOrDefaultAsync(x => x.Id == sectionId);
                        int videoCourseId = section?.VideoCourseId ?? 0;
                        int status = exam.Type == 1 ? 2 : 1;
                        var examResult = new tbl_ExamResult
                        {
                            CreatedBy = user.FullName,
                            CreatedOn = DateTime.Now,
                            Type = exam.Type.Value,
                            VideoCourseId = videoCourseId,
                            Enable = true,
                            ExamId = exam.Id,
                            TeacherId = 0,
                            Status = status,
                            StatusName = status == 1 ? "Chờ chấm bài" : status == 2 ? "Đã chấm bài" : "",
                            IsPass = false,
                            ModifiedBy = user.FullName,
                            LessonVideoId = model.LessonVideoId ?? 0,
                            ModifiedOn = DateTime.Now,
                            PassPoint = exam.PassPoint,
                            StudentId = user.UserInformationId,
                            TotalPoint = 0
                        };
                        double totalPoint = 0;
                        double mypoint = 0;
                        db.tbl_ExamResult.Add(examResult);
                        await db.SaveChangesAsync();
                        var examSections = await db.tbl_ExamSection.Where(x => x.ExamId == exam.Id && x.Enable == true).ToListAsync();
                        if (examSections.Any())
                        {
                            foreach (var examSection in examSections)
                            {
                                var examSectionResult = new tbl_ExamSectionResult
                                {
                                    ExamResultId = examResult.Id,
                                    Explanations = examSection.Explanations,
                                    Index = examSection.Index,
                                    Name = examSection.Name,
                                    CreatedBy = user.FullName,
                                    CreatedOn = DateTime.Now,
                                    Enable = true,
                                    ModifiedBy = user.FullName,
                                    ModifiedOn = DateTime.Now,
                                };
                                db.tbl_ExamSectionResult.Add(examSectionResult);
                                await db.SaveChangesAsync();
                                var exerciseGroups = await db.tbl_ExerciseGroup.Where(x => x.ExamSectionId == examSection.Id && x.Enable == true).ToListAsync();
                                if (exerciseGroups.Any())
                                {
                                    foreach (var exerciseGroup in exerciseGroups)
                                    {
                                        var exerciseGroupResult = new tbl_ExerciseGroupResult
                                        {
                                            Content = exerciseGroup.Content,
                                            ExamResultId = examResult.Id,
                                            ExamSectionResultId = examSectionResult.Id,
                                            ExerciseNumber = exerciseGroup.ExerciseNumber,
                                            Index = exerciseGroup.Index,
                                            Level = exerciseGroup.Level,
                                            LevelName = exerciseGroup.LevelName,
                                            Paragraph = exerciseGroup.Paragraph,
                                            Type = exerciseGroup.Type,
                                            TypeName = exerciseGroup.TypeName,
                                            CreatedBy = user.FullName,
                                            CreatedOn = DateTime.Now,
                                            VideoUrl = exerciseGroup.VideoUrl,
                                            Enable = true,
                                            ModifiedBy = user.FullName,
                                            ModifiedOn = DateTime.Now,
                                        };
                                        db.tbl_ExerciseGroupResult.Add(exerciseGroupResult);
                                        await db.SaveChangesAsync();
                                        var exercises = await db.tbl_Exercise.Where(x => x.ExerciseGroupId == exerciseGroup.Id && x.Enable == true).ToListAsync();
                                        if (exercises.Any())
                                        {
                                            foreach (var exercise in exercises)
                                            {
                                                var exerciseResult = new tbl_ExerciseResult
                                                {
                                                    Content = exercise.Content,
                                                    DescribeAnswer = exercise.DescribeAnswer,
                                                    ExamResultId = examResult.Id,
                                                    ExamSectionResultId = examSectionResult.Id,
                                                    ExerciseGroupResultId = exerciseGroupResult.Id,
                                                    ExerciseId = exercise.Id,
                                                    Index = exercise.Index,
                                                    InputId = exercise.InputId,
                                                    IsResult = false,
                                                    Point = 0,
                                                    CreatedBy = user.FullName,
                                                    CreatedOn = DateTime.Now,
                                                    Enable = true,
                                                    ModifiedBy = user.FullName,
                                                    ModifiedOn = DateTime.Now,
                                                };
                                                db.tbl_ExerciseResult.Add(exerciseResult);
                                                await db.SaveChangesAsync();
                                                bool isResult = false;
                                                var exerciseSubmit = model.Items.Where(x => x.ExerciseId == exercise.Id).FirstOrDefault();
                                                var answers = await db.tbl_Answer.Where(x => x.ExerciseId == exercise.Id && x.Enable == true).ToListAsync();
                                                switch (exerciseGroup.Type)
                                                {
                                                    ///Chấm bài trắc nghiệm
                                                    case ExerciseType.MultipleChoice:
                                                        if (answers.Any())
                                                        {
                                                            foreach (var answer in answers)
                                                            {
                                                                if (exerciseSubmit == null)
                                                                {
                                                                    var answerResult = new tbl_AnswerResult
                                                                    {
                                                                        AnswerContent = answer.AnswerContent,
                                                                        AnswerId = answer.Id,
                                                                        ExerciseResultId = exerciseResult.Id,
                                                                        MyResult = false,
                                                                        IsTrue = answer.IsTrue,
                                                                        MyAnswerContent = "",
                                                                        MyAnswerId = 0,
                                                                        CreatedBy = user.FullName,
                                                                        CreatedOn = DateTime.Now,
                                                                        Enable = true,
                                                                        ModifiedBy = user.FullName,
                                                                        ModifiedOn = DateTime.Now,
                                                                    };
                                                                    db.tbl_AnswerResult.Add(answerResult);
                                                                }
                                                                else
                                                                {
                                                                    var myAnswer = exerciseSubmit.Answers.Where(x => x.AnswerId == answer.Id).FirstOrDefault();
                                                                    var answerResult = new tbl_AnswerResult
                                                                    {
                                                                        AnswerContent = answer.AnswerContent,
                                                                        AnswerId = answer.Id,
                                                                        ExerciseResultId = exerciseResult.Id,
                                                                        MyResult = myAnswer == null ? false : true,
                                                                        IsTrue = answer.IsTrue,
                                                                        MyAnswerContent = myAnswer?.AnswerContent ?? "",
                                                                        MyAnswerId = myAnswer?.AnswerId ?? 0,
                                                                        CreatedBy = user.FullName,
                                                                        CreatedOn = DateTime.Now,
                                                                        Enable = true,
                                                                        ModifiedBy = user.FullName,
                                                                        ModifiedOn = DateTime.Now,
                                                                    };
                                                                    db.tbl_AnswerResult.Add(answerResult);
                                                                }
                                                                await db.SaveChangesAsync();
                                                            }
                                                            var checkResult = await db.tbl_AnswerResult.AnyAsync(x => x.ExerciseResultId == exerciseResult.Id && x.Enable == true
                                                            && x.IsTrue != x.MyResult);
                                                            if (!checkResult)
                                                                isResult = true;
                                                        }
                                                        break;
                                                    ///Chấm bài kéo thả
                                                    case ExerciseType.DragDrop:
                                                        if (answers.Any())
                                                        {
                                                            var correctAnswers = answers.Where(x => x.IsTrue == true).ToList();
                                                            if (exerciseSubmit == null)
                                                            {
                                                                var answerResult = new tbl_AnswerResult
                                                                {
                                                                    AnswerContent = correctAnswers.FirstOrDefault().AnswerContent,
                                                                    AnswerId = correctAnswers.FirstOrDefault().Id,
                                                                    ExerciseResultId = exerciseResult.Id,
                                                                    MyResult = false,
                                                                    IsTrue = true,
                                                                    MyAnswerContent = "",
                                                                    MyAnswerId = 0,
                                                                    CreatedBy = user.FullName,
                                                                    CreatedOn = DateTime.Now,
                                                                    Enable = true,
                                                                    ModifiedBy = user.FullName,
                                                                    ModifiedOn = DateTime.Now,
                                                                };
                                                                db.tbl_AnswerResult.Add(answerResult);
                                                            }
                                                            else
                                                            {
                                                                var myAnswer = exerciseSubmit.Answers.FirstOrDefault();
                                                                var checkResult = correctAnswers.Where(x => x.Id == myAnswer.AnswerId).FirstOrDefault();
                                                                if (checkResult != null)
                                                                {
                                                                    var answerResult = new tbl_AnswerResult
                                                                    {
                                                                        AnswerContent = checkResult.AnswerContent,
                                                                        AnswerId = checkResult.Id,
                                                                        ExerciseResultId = exerciseResult.Id,
                                                                        MyResult = true,
                                                                        IsTrue = checkResult.IsTrue,
                                                                        MyAnswerContent = myAnswer?.AnswerContent ?? "",
                                                                        MyAnswerId = myAnswer?.AnswerId ?? 0,
                                                                        CreatedBy = user.FullName,
                                                                        CreatedOn = DateTime.Now,
                                                                        Enable = true,
                                                                        ModifiedBy = user.FullName,
                                                                        ModifiedOn = DateTime.Now,
                                                                    };
                                                                    db.tbl_AnswerResult.Add(answerResult);
                                                                    isResult = true;
                                                                }
                                                                else
                                                                {
                                                                    var answerResult = new tbl_AnswerResult
                                                                    {
                                                                        AnswerContent = correctAnswers.FirstOrDefault().AnswerContent,
                                                                        AnswerId = correctAnswers.FirstOrDefault().Id,
                                                                        ExerciseResultId = exerciseResult.Id,
                                                                        MyResult = true,
                                                                        IsTrue = correctAnswers.FirstOrDefault().IsTrue,
                                                                        MyAnswerContent = myAnswer?.AnswerContent ?? "",
                                                                        MyAnswerId = myAnswer?.AnswerId ?? 0,
                                                                        CreatedBy = user.FullName,
                                                                        CreatedOn = DateTime.Now,
                                                                        Enable = true,
                                                                        ModifiedBy = user.FullName,
                                                                        ModifiedOn = DateTime.Now,
                                                                    };
                                                                    db.tbl_AnswerResult.Add(answerResult);
                                                                }
                                                            }
                                                            await db.SaveChangesAsync();
                                                        }
                                                        break;
                                                    ///Chấm bài điền vào ô trống
                                                    case ExerciseType.FillInTheBlank:
                                                        if (answers.Any())
                                                        {

                                                            var correctAnswers = answers.Where(x => x.IsTrue == true).ToList();
                                                            if (exerciseSubmit == null)
                                                            {
                                                                var answerResult = new tbl_AnswerResult
                                                                {
                                                                    AnswerContent = correctAnswers.FirstOrDefault().AnswerContent,
                                                                    AnswerId = correctAnswers.FirstOrDefault().Id,
                                                                    ExerciseResultId = exerciseResult.Id,
                                                                    MyResult = false,
                                                                    IsTrue = true,
                                                                    MyAnswerContent = "",
                                                                    MyAnswerId = 0,
                                                                    CreatedBy = user.FullName,
                                                                    CreatedOn = DateTime.Now,
                                                                    Enable = true,
                                                                    ModifiedBy = user.FullName,
                                                                    ModifiedOn = DateTime.Now,
                                                                };
                                                                db.tbl_AnswerResult.Add(answerResult);
                                                            }
                                                            else
                                                            {
                                                                var myAnswer = exerciseSubmit.Answers.FirstOrDefault();
                                                                var checkResult = correctAnswers
                                                                    .Where(x => x.AnswerContent.ToUpper() == myAnswer.AnswerContent.ToUpper()).FirstOrDefault();
                                                                if (checkResult != null)
                                                                {
                                                                    var answerResult = new tbl_AnswerResult
                                                                    {
                                                                        AnswerContent = checkResult.AnswerContent,
                                                                        AnswerId = checkResult.Id,
                                                                        ExerciseResultId = exerciseResult.Id,
                                                                        MyResult = true,
                                                                        IsTrue = checkResult.IsTrue,
                                                                        MyAnswerContent = myAnswer?.AnswerContent ?? "",
                                                                        MyAnswerId = myAnswer?.AnswerId ?? 0,
                                                                        CreatedBy = user.FullName,
                                                                        CreatedOn = DateTime.Now,
                                                                        Enable = true,
                                                                        ModifiedBy = user.FullName,
                                                                        ModifiedOn = DateTime.Now,
                                                                    };
                                                                    db.tbl_AnswerResult.Add(answerResult);
                                                                    isResult = true;
                                                                }
                                                                else
                                                                {
                                                                    var answerResult = new tbl_AnswerResult
                                                                    {
                                                                        AnswerContent = correctAnswers.FirstOrDefault().AnswerContent,
                                                                        AnswerId = correctAnswers.FirstOrDefault().Id,
                                                                        ExerciseResultId = exerciseResult.Id,
                                                                        MyResult = true,
                                                                        IsTrue = correctAnswers.FirstOrDefault().IsTrue,
                                                                        MyAnswerContent = myAnswer?.AnswerContent ?? "",
                                                                        MyAnswerId = myAnswer?.AnswerId ?? 0,
                                                                        CreatedBy = user.FullName,
                                                                        CreatedOn = DateTime.Now,
                                                                        Enable = true,
                                                                        ModifiedBy = user.FullName,
                                                                        ModifiedOn = DateTime.Now,
                                                                    };
                                                                    db.tbl_AnswerResult.Add(answerResult);
                                                                }
                                                            }
                                                            await db.SaveChangesAsync();
                                                        }
                                                        break;
                                                    ///Nộp bài điền từ
                                                    case ExerciseType.Essay:
                                                        {
                                                            var myAnswer = exerciseSubmit.Answers.FirstOrDefault();
                                                            var answerResult = new tbl_AnswerResult
                                                            {
                                                                AnswerContent = myAnswer.AnswerContent,
                                                                VideoUrl = myAnswer.VideoUrl,
                                                                AnswerId = 0,
                                                                ExerciseResultId = exerciseResult.Id,
                                                                MyResult = true,
                                                                IsTrue = false,
                                                                MyAnswerContent = myAnswer?.AnswerContent ?? "",
                                                                MyAnswerId = myAnswer?.AnswerId ?? 0,
                                                                CreatedBy = user.FullName,
                                                                CreatedOn = DateTime.Now,
                                                                Enable = true,
                                                                ModifiedBy = user.FullName,
                                                                ModifiedOn = DateTime.Now,
                                                            };
                                                            db.tbl_AnswerResult.Add(answerResult);
                                                            await db.SaveChangesAsync();
                                                        }
                                                        break;
                                                }
                                                exerciseResult.IsResult = isResult;
                                                totalPoint += exercise.Point ?? 0;
                                                if (isResult)
                                                {
                                                    exerciseResult.Point = exercise.Point ?? 0;
                                                    mypoint += exercise.Point ?? 0;
                                                }
                                                await db.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        examResult.MyPoint = mypoint;
                        examResult.TotalPoint = totalPoint;
                        if (examResult.MyPoint >= exam.PassPoint)
                        {
                            examResult.IsPass = true;
                            await LessonVideoService.Completed(db, examResult.LessonVideoId, user, examResult.Id, examResult.TotalPoint);
                        }
                        await db.SaveChangesAsync();
                        tran.Commit();
                        return examResult;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(ExamResultSearch search, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_ExamResult @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@ExamId = {search.ExamId ?? 0}," +
                    $"@VideoCourseId = {search.VideoCourseId}," +
                    $"@Status = {search.Status}," +
                    $"@TeacherId = {(userLog.RoleId == ((int)RoleEnum.teacher) ? userLog.UserInformationId : search.TeacherId)}," +
                    $"@StudentId ={(userLog.RoleId == ((int)RoleEnum.student) ? userLog.UserInformationId : search.StudentId)}";
                var data = await db.Database.SqlQuery<Get_ExamResult>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_ExamResult(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public class ExamResultDetailResult : AppDomainResult
        {
            public string ExamName { get; set; }
            public string ExamCode { get; set; }
            public double PassPoint { get; set; }
            public bool IsPass { get; set; }
            public int LessonVideoId { get; set; }
            public int Type { get; set; }
            public int Status { get; set; }
            public string StatusName { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CreatedBy { get; set; }
            public int VideoCourseId { get; set; }
            /// <summary>
            /// Tổng số điểm
            /// </summary>
            public double TotalPoint { get; set; }
            /// <summary>
            /// Điểm
            /// </summary>
            public double MyPoint { get; set; }
        }
        public static async Task<ExamResultDetailResult> GetDetail(int examResultId)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_ExamResultDetail @PageIndex = 1," +
                    $"@PageSize = 9999," +
                    $"@ExamResultId = {examResultId}";
                var examResult = await db.tbl_ExamResult.SingleOrDefaultAsync(x => x.Id == examResultId);
                if (examResult == null)
                    return new ExamResultDetailResult { TotalRow = 0, Data = null };
                var exam = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == examResult.ExamId);
                var data = await db.Database.SqlQuery<Get_ExamResultDetail>(sql).ToListAsync();
                if (!data.Any()) return new ExamResultDetailResult { TotalRow = 0, Data = null };
                int indexInExam = 0;
                var result = data.GroupBy(es => new { es.ExamSectionResultId, es.ExamSectionName, es.Explanations, es.ExamSectionIndex }).OrderBy(x => x.Key.ExamSectionIndex)
                    .Select(es => new ExamSectionResultModel
                    {
                        Id = es.Key.ExamSectionResultId,
                        Name = es.Key.ExamSectionName,
                        Explanations = es.Key.Explanations,
                        Index = es.Key.ExamSectionIndex,
                        ExerciseResultGroups = es.GroupBy(eg => new
                        {
                            eg.Id,
                            eg.Content,
                            eg.Paragraph,
                            eg.ExerciseNumber,
                            eg.Level,
                            eg.LevelName,
                            eg.Type,
                            eg.TypeName,
                            eg.ExerciseGroupIndex,
                            eg.VideoUrl,
                        }).OrderBy(x => x.Key.ExerciseGroupIndex).Select(eg => new ExerciseGroupResultModel
                        {
                            Id = eg.Key.Id,
                            Content = eg.Key.Content,
                            Paragraph = eg.Key.Paragraph,
                            ExerciseNumber = eg.Key.ExerciseNumber,
                            Level = eg.Key.Level,
                            LevelName = eg.Key.LevelName,
                            Type = eg.Key.Type,
                            TypeName = eg.Key.TypeName,
                            IsExist = true,
                            Index = eg.Key.ExerciseGroupIndex,
                            VideoUrl = eg.Key.VideoUrl,
                            ExerciseResults = eg.GroupBy(e => new
                            {
                                e.ExerciseResultId,
                                e.ExerciseContent,
                                e.InputId,
                                e.DescribeAnswer,
                                e.ExerciseIndex,
                                e.Point,
                                e.IsResult,
                            }).OrderBy(x => x.Key.ExerciseIndex).Select(e => new ExerciseResultModel
                            {
                                Id = e.Key.ExerciseResultId,
                                Content = e.Key.ExerciseContent,
                                InputId = e.Key.InputId,
                                DescribeAnswer = e.Key.DescribeAnswer,
                                IndexInExam = indexInExam += 1,
                                Index = e.Key.ExerciseIndex,
                                Point = e.Key.Point,
                                IsResult = e.Key.IsResult,
                                AnswerResults = e.GroupBy(a => new
                                {
                                    a.AnswerResultId,
                                    a.AnswerContent,
                                    a.IsTrue,
                                    a.MyAnswerContent,
                                    a.MyAnswerId,
                                    a.MyResult,
                                    a.MyAnswerVideoUrl,
                                }).Select(a => new AnswerResultModel
                                {
                                    Id = a.Key.AnswerResultId,
                                    AnswerContent = a.Key.AnswerContent,
                                    IsTrue = a.Key.IsTrue,
                                    MyAnswerContent = a.Key.MyAnswerContent,
                                    MyAnswerId = a.Key.MyAnswerId,
                                    MyResult = a.Key.MyResult,
                                    VideoUrl = a.Key.MyAnswerVideoUrl,
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList();
                return new ExamResultDetailResult {
                    Data = result,
                    TotalPoint = examResult.TotalPoint,
                    MyPoint = examResult.MyPoint,
                    ExamCode = exam.Code,
                    ExamName = exam.Name,
                    IsPass = examResult.IsPass.Value,
                    LessonVideoId = examResult.LessonVideoId,
                    PassPoint = examResult.PassPoint,
                    Type = examResult.Type,
                    Status = examResult.Status,
                    StatusName = examResult.StatusName,
                    CreatedOn = examResult.CreatedOn.Value,
                    CreatedBy = examResult.CreatedBy,
                    VideoCourseId = examResult.VideoCourseId ?? 0,
                };
            }
        }
        public class GradingEssayModel
        {
            public int ExamResultId { get; set; }
            public List<GradingEssayItem> Items { get; set; }
            public GradingEssayModel()
            {
                Items = new List<GradingEssayItem>();
            }
        }
        public class GradingEssayItem
        {
            public int StandardId { get; set; }
            [JsonIgnore]
            public bool Enable { get; set; }
            public GradingEssayItem()
            {
                Enable = true;
            }
        }
        public static async Task<tbl_ExamResult> CreateGradingEssay(GradingEssayModel itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var examResult = await db.tbl_ExamResult.SingleOrDefaultAsync(x => x.Id == itemModel.ExamResultId);
                        if (examResult == null)
                            throw new Exception("Không tìm thấy kết quả làm bài");
                        if (examResult.Type == 1)
                            throw new Exception("Không thể chấm bài thi kiến thức");

                        double myPoint = 0;
                        //Xóa những câu trước đó đã chọn
                        var gradingEssays = await db.tbl_GradingEssay.Where(x => x.Enable == true && x.ExamResultId == examResult.Id).ToListAsync();
                        if (gradingEssays.Any())
                        {
                            foreach (var item in gradingEssays)
                            {
                                if (!itemModel.Items.Any(x => x.StandardId == item.StandardId))
                                    itemModel.Items.Add(new GradingEssayItem { StandardId = item.StandardId, Enable = false });
                            }
                        }

                        foreach (var item in itemModel.Items)
                        {
                            var gradingEssay = await db.tbl_GradingEssay
                                .FirstOrDefaultAsync(x => x.StandardId == item.StandardId && x.ExamResultId == examResult.Id && x.Enable == true);
                            if (gradingEssay == null)
                            {
                                gradingEssay = new tbl_GradingEssay
                                {
                                    CreatedBy = userLog.FullName,
                                    CreatedOn = DateTime.Now,
                                    Enable = item.Enable,
                                    ExamResultId = examResult.Id,
                                    ModifiedBy = userLog.FullName,
                                    ModifiedOn = DateTime.Now,
                                    StandardId = item.StandardId
                                };
                                db.tbl_GradingEssay.Add(gradingEssay);
                            }
                            else
                            {
                                gradingEssay.Enable = item.Enable;
                            }
                            await db.SaveChangesAsync();

                            var standard = await db.tbl_Standard.SingleOrDefaultAsync(x => x.Id == item.StandardId);
                            if (gradingEssay.Enable == true && standard != null)
                                myPoint += standard.Point;
                        }

                        examResult.MyPoint = myPoint;
                        examResult.Status = 2;
                        examResult.StatusName = "Đã chấm bài";
                        await db.SaveChangesAsync();
                        if (myPoint >= examResult.PassPoint)
                        {
                            examResult.IsPass = true;
                            var student = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == examResult.StudentId);
                            await LessonVideoService.Completed(db, examResult.LessonVideoId, student, examResult.Id, examResult.MyPoint);
                        }
                        await db.SaveChangesAsync();
                        tran.Commit();
                        return examResult;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public static async Task<List<tbl_GradingEssay>> GetGradingEssay(int examResultId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_GradingEssay.Where(x => x.ExamResultId == examResultId && x.Enable == true).ToListAsync();
                data = (from i in data
                        select new tbl_GradingEssay
                        {
                            CreatedBy = i.CreatedBy,
                            CreatedOn = i.CreatedOn,
                            Enable = i.Enable,
                            ExamResultId = i.ExamResultId,
                            Id = i.Id,
                            ModifiedBy = i.ModifiedBy,
                            ModifiedOn = i.ModifiedOn,
                            StandardId = i.StandardId,
                            StandardName = Task.Run(()=> GetStandard(db,i.StandardId)).Result?.Name,
                            StandardPoint = Task.Run(() => GetStandard(db, i.StandardId)).Result?.Point ?? 0,
                        }).ToList();
                return data;
            }
        }
        public static async Task<tbl_Standard> GetStandard(lmsDbContext dbContext, int standardId)
        {
            return await dbContext.tbl_Standard.SingleOrDefaultAsync(x => x.Id == standardId);
        }
        public class AddTeacherModel
        {
            public int TeacherId { get; set; }
            public List<int> ExamResultIds { get; set; }
        }
        public static async Task AddTeachers(AddTeacherModel itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var teacher = await db.tbl_UserInformation
                    .SingleOrDefaultAsync(x => x.RoleId == ((int)RoleEnum.teacher) && x.UserInformationId == itemModel.TeacherId && x.Enable == true);
                if (teacher == null)
                    throw new Exception("Không tìm thấy giáo viên");
                if (!itemModel.ExamResultIds.Any())
                    throw new Exception("Vui lòng chọn bài làm");
                foreach (var item in itemModel.ExamResultIds)
                {
                    var examResult = await db.tbl_ExamResult.SingleOrDefaultAsync(x => x.Id == item);
                    if (examResult == null)
                        continue;
                    examResult.TeacherId = teacher.UserInformationId;
                    examResult.ModifiedBy = userLog.FullName;
                    await db.SaveChangesAsync();
                }

                await NotificationService.Send(
                                    new tbl_Notification
                                    {
                                        UserId = teacher.UserInformationId,
                                        Title = "Bài tập cần chấm",
                                        Content = "Bạn có bài tập mới cần chấm, vui lòng kiểm tra"
                                    }, new tbl_UserInformation { FullName = "Tự động" });
            }
        }
        /// <summary>
        /// false - chưa hoàn thành bài kiểm tra kiến thức
        /// </summary>
        /// <param name="videoCourseId"></param>
        /// <param name="userLog"></param>
        /// <returns></returns>
        public static async Task<bool> KnowledgeExamCompleted(int videoCourseId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var sections = await db.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true).Select(x => x.Id).ToListAsync();
                if (sections.Any())
                {
                    foreach (var item in sections)
                    {
                        var lessons = await db.tbl_LessonVideo.Where(x => x.SectionId == item && x.Enable == true && x.Type == 2)
                            .Select(x => new { x.Id, x.ExamId }).ToListAsync();
                        if (lessons.Any())
                        {
                            foreach (var jteam in lessons)
                            {
                                var exam = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == jteam.ExamId);
                                if (exam.Type == 1)
                                {
                                    var hasResult = await db.tbl_ExamResult
                                        .AnyAsync(x => x.LessonVideoId == jteam.Id && x.StudentId == userLog.UserInformationId && x.Enable == true && x.IsPass == true);
                                    if (!hasResult)
                                        return false;
                                }
                            }
                        }
                    }
                }
                else return false;
                return true;

            }
        }
    }
}