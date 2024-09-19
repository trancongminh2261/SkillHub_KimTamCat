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

namespace LMS_Project.Services
{
    public class ExamService
    {
        public static async Task<tbl_Exam> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
       
        public static async Task<tbl_Exam> Insert(ExamCreate examCreate,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var model = new tbl_Exam(examCreate);
                    var validateCode = await db.tbl_Exam.AnyAsync(x => x.Code.ToUpper() == model.Code.ToUpper() && x.Enable == true);
                    if (validateCode)
                        throw new Exception("Đã tồn tại mã đề này");
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    db.tbl_Exam.Add(model);
                    await db.SaveChangesAsync();
                    if (model.Type == 2)
                    {
                        db.tbl_ExamSection.Add(new tbl_ExamSection
                        {
                            CreatedBy = user.FullName,
                            CreatedOn = DateTime.Now,
                            Enable = true,
                            ExamId = model.Id,
                            Explanations = "",
                            Index = 1,
                            ModifiedBy = user.FullName,
                            ModifiedOn = DateTime.Now,
                            Name = "",
                        });
                        await db.SaveChangesAsync();
                    }
                    return model;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<tbl_Exam> Update(ExamUpdate examUpdate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == examUpdate.Id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy đề");
                    var validateCode = await db.tbl_Exam.AnyAsync(x => x.Code.ToUpper() == examUpdate.Code.ToUpper() && x.Id != entity.Id);
                    if (validateCode)
                        throw new Exception("Đã tồn tại mã đề này");
                    entity.Name = examUpdate.Name ?? entity.Name;
                    entity.Code = examUpdate.Code ?? entity.Code;
                    entity.Description = examUpdate.Description ?? entity.Description;
                    entity.Time = examUpdate.Time ?? entity.Time;
                    entity.PassPoint = examUpdate.PassPoint ?? entity.PassPoint;
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = DateTime.Now;
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy đề thi");
                    entity.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(ExamSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_Exam @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Search = N'{baseSearch.Search ?? ""}'";
                var data = await db.Database.SqlQuery<Get_Exam>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Exam(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task<AppDomainResult> GetDetail(int examId, bool isDoingTest,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_ExamDetail @PageIndex = 1," +
                    $"@PageSize = 9999," +
                    $"@ExamId = {examId}";
                ///Nếu không phải là admin thì ẩn đáp án
                bool viewResult = false;
                if (user.RoleId != ((int)RoleEnum.student))
                    viewResult = true;
                var data = await db.Database.SqlQuery<Get_ExerciseGroup>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                int indexInExam = 0;
                var result = new List<ExamSectionModel>();
                if(isDoingTest)
                    result = data.GroupBy(es => new { es.ExamSectionId, es.ExamSectionName, es.Explanations ,es.ExamSectionIndex}).OrderBy(x=>x.Key.ExamSectionIndex)
                        .Select(es => new ExamSectionModel
                        {
                            Id = es.Key.ExamSectionId,
                            Name = es.Key.ExamSectionName,
                            Explanations = es.Key.Explanations,
                            Index = es.Key.ExamSectionIndex,
                            ExerciseGroups = es.GroupBy(eg => new
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
                                eg.Enable,
                                eg.VideoUrl,
                            }).Where(x=>x.Key.Id.HasValue && x.Key.Enable == true)
                            //.OrderBy(x => x.Key.ExerciseGroupIndex).Select(eg => new ExerciseGroupModel
                            .OrderBy(x =>Guid.NewGuid()).Select(eg => new ExerciseGroupModel
                            {
                                Id = eg.Key.Id,
                                Content = eg.Key.Content,
                                Paragraph = eg.Key.Paragraph,
                                ExerciseNumber = eg.Key.ExerciseNumber,
                                Level = eg.Key.Level,
                                LevelName = eg.Key.LevelName,
                                Type = eg.Key.Type,
                                TypeName = eg.Key.TypeName,
                                Index = eg.Key.ExerciseGroupIndex,
                                VideoUrl = eg.Key.VideoUrl,
                                Exercises = eg.GroupBy(e => new
                                {
                                    e.ExerciseId,
                                    e.ExerciseContent,
                                    e.InputId,
                                    e.DescribeAnswer,
                                    e.ExerciseIndex,
                                    e.Point,
                                    e.ExerciseEnable,
                                }).Where(x => x.Key.ExerciseId.HasValue && x.Key.ExerciseEnable == true)
                                //.OrderBy(x => x.Key.ExerciseIndex).Select(e => new ExerciseModel
                                .OrderBy(x => Guid.NewGuid()).Select(e => new ExerciseModel
                                {
                                    Id = e.Key.ExerciseId,
                                    Content = e.Key.ExerciseContent,
                                    InputId = e.Key.InputId,
                                    DescribeAnswer = e.Key.DescribeAnswer,
                                    IndexInExam = indexInExam += 1,
                                    Index = e.Key.ExerciseIndex,
                                    Point = e.Key.Point ?? 0,
                                    Correct = e.GroupBy(a => new //Số câu đúng
                                    {
                                        a.AnswerId,
                                        a.IsTrue,
                                        a.AnswerEnable,
                                    }).Count(x => x.Key.AnswerId.HasValue && x.Key.AnswerEnable == true && x.Key.IsTrue == true),
                                    Answers = e.GroupBy(a => new
                                    {
                                        a.AnswerId,
                                        a.AnswerContent,
                                        a.IsTrue,
                                        a.AnswerEnable,
                                    }).Where(x => x.Key.AnswerId.HasValue && x.Key.AnswerEnable == true).Select(a => new AnswerModel
                                    {
                                        Id = a.Key.AnswerId,
                                        AnswerContent = a.Key.AnswerContent,
                                        IsTrue = viewResult ? a.Key.IsTrue : null,
                                    }).OrderBy(x=> Guid.NewGuid()).ToList()
                                }).ToList()

                            }).ToList()
                        }).ToList();
                else
                    result = data.GroupBy(es => new { es.ExamSectionId, es.ExamSectionName, es.Explanations, es.ExamSectionIndex }).OrderBy(x => x.Key.ExamSectionIndex)
                        .Select(es => new ExamSectionModel
                        {
                            Id = es.Key.ExamSectionId,
                            Name = es.Key.ExamSectionName,
                            Explanations = es.Key.Explanations,
                            Index = es.Key.ExamSectionIndex,
                            ExerciseGroups = es.GroupBy(eg => new
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
                                eg.Enable,
                                eg.VideoUrl,
                            }).Where(x => x.Key.Id.HasValue && x.Key.Enable == true)
                            .OrderBy(x => x.Key.ExerciseGroupIndex).Select(eg => new ExerciseGroupModel
                            {
                                Id = eg.Key.Id,
                                Content = eg.Key.Content,
                                Paragraph = eg.Key.Paragraph,
                                ExerciseNumber = eg.Key.ExerciseNumber,
                                Level = eg.Key.Level,
                                LevelName = eg.Key.LevelName,
                                Type = eg.Key.Type,
                                TypeName = eg.Key.TypeName,
                                Index = eg.Key.ExerciseGroupIndex,
                                VideoUrl = eg.Key.VideoUrl,
                                Exercises = eg.GroupBy(e => new
                                {
                                    e.ExerciseId,
                                    e.ExerciseContent,
                                    e.InputId,
                                    e.DescribeAnswer,
                                    e.ExerciseIndex,
                                    e.Point,
                                    e.ExerciseEnable,
                                }).Where(x => x.Key.ExerciseId.HasValue && x.Key.ExerciseEnable == true)
                                .OrderBy(x => x.Key.ExerciseIndex).Select(e => new ExerciseModel
                                {
                                    Id = e.Key.ExerciseId,
                                    Content = e.Key.ExerciseContent,
                                    InputId = e.Key.InputId,
                                    DescribeAnswer = e.Key.DescribeAnswer,
                                    IndexInExam = indexInExam += 1,
                                    Index = e.Key.ExerciseIndex,
                                    Point = e.Key.Point ?? 0,
                                    Correct = e.GroupBy(a => new //Số câu đúng
                                {
                                        a.AnswerId,
                                        a.IsTrue,
                                        a.AnswerEnable,
                                    }).Count(x => x.Key.AnswerId.HasValue && x.Key.AnswerEnable == true && x.Key.IsTrue == true),
                                    Answers = e.GroupBy(a => new
                                    {
                                        a.AnswerId,
                                        a.AnswerContent,
                                        a.IsTrue,
                                        a.AnswerEnable,
                                    }).Where(x => x.Key.AnswerId.HasValue && x.Key.AnswerEnable == true).Select(a => new AnswerModel
                                    {
                                        Id = a.Key.AnswerId,
                                        AnswerContent = a.Key.AnswerContent,
                                        IsTrue = viewResult ? a.Key.IsTrue : null,
                                    }).ToList()
                                }).ToList()

                            }).ToList()
                        }).ToList();
                return new AppDomainResult { Data = result };
            }
        }
        
        public static async Task<double> GetTotalPoint(int examId)
        {
            using (var db = new lmsDbContext())
            {
                double totalPoint = 0;
                var examSectionIds = await db.tbl_ExamSection.Where(x => x.ExamId == examId && x.Enable == true).Select(x=>x.Id).ToListAsync();
                if (examSectionIds.Any())
                {
                    foreach (var item in examSectionIds)
                    {
                        var groupIds = await db.tbl_ExerciseGroup.Where(x => x.ExamSectionId == item && x.Enable == true).Select(x => x.Id).ToListAsync();
                        if (groupIds.Any())
                        {
                            foreach (var groupId in groupIds)
                            {
                                var exercises = await db.tbl_Exercise
                                    .Where(x => x.ExerciseGroupId == groupId && x.Enable == true)
                                    .ToListAsync();

                                totalPoint = exercises.Sum(x => x.Point ?? 0);
                            }
                        }
                    }
                }
                return totalPoint;
            }
        }
        
        public class AddExerciseGroupModel
        {
            [Required(ErrorMessage = "Vui lòng chọn phần")]
            public int? ExamSectionId { get; set; }
            public List<ExerciseGroupItem> Items { get; set; }
        }
        public class ExerciseGroupItem
        {
            [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]
            public int Id { get; set; }
            public List<ExerciseItem> ExerciseItems  { get;set;}
        }
        public class ExerciseItem
        {
            [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]
            public int Id { get; set; }
            [Required(ErrorMessage = "Vui lòng nhập điểm")]
            public double Point { get; set; }
        }
        /// <summary>
        /// Thêm câu hỏi vào đề
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task AddExerciseGroup(AddExerciseGroupModel model,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var examSection = await db.tbl_ExamSection.SingleOrDefaultAsync(x => x.Id == model.ExamSectionId);
                        if (examSection == null)
                            throw new Exception("Không tìm thấy phần");
                        if (!model.Items.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model.Items)
                        {
                            var exerciseGroup = await db.tbl_ExerciseGroup.SingleOrDefaultAsync(x => x.Id == item.Id);
                            if (exerciseGroup == null)
                                throw new Exception("Không tìm thấy câu hỏi");
                            if (exerciseGroup.ExamId == examSection.ExamId)
                                throw new Exception("Đã tồn tại câu hỏi trong đề");
                            exerciseGroup.ExamId = examSection.ExamId;
                            exerciseGroup.ExamSectionId = examSection.Id;
                            var groupIndex = 1;
                            var lastGroup = await db.tbl_ExerciseGroup
                                .Where(x => x.ExamSectionId == model.ExamSectionId && x.Enable == true).OrderByDescending(x => x.Index).FirstOrDefaultAsync();
                            if (lastGroup != null)
                                groupIndex = lastGroup.Index.Value + 1;
                            exerciseGroup.ModifiedBy = exerciseGroup.CreatedBy = user.FullName;
                            exerciseGroup.ModifiedOn = exerciseGroup.CreatedOn = DateTime.Now;
                            exerciseGroup.Index = groupIndex;
                            db.tbl_ExerciseGroup.Add(exerciseGroup);
                            await db.SaveChangesAsync();

                            if (item.ExerciseItems.Any())
                            {
                                int exerciseIndex = 1;
                                foreach (var etem in item.ExerciseItems)
                                {
                                    var exercise = await db.tbl_Exercise.SingleOrDefaultAsync(x => x.Id == etem.Id);
                                    exercise.ModifiedBy = exercise.CreatedBy = user.FullName;
                                    exercise.ModifiedOn = exercise.CreatedOn = DateTime.Now;
                                    exercise.ExerciseGroupId = exerciseGroup.Id;
                                    exercise.ExamSectionId = examSection.Id;
                                    exercise.ExamId = examSection.ExamId;
                                    exercise.Index = exerciseIndex;
                                    exercise.Point = etem.Point;
                                    db.tbl_Exercise.Add(exercise);
                                    await db.SaveChangesAsync();
                                    var answers = await db.tbl_Answer.Where(x => x.ExerciseId == etem.Id && x.Enable == true).ToListAsync();
                                    if(answers.Any())
                                    foreach (var answer in answers)
                                    {
                                        answer.ExerciseId = exercise.Id;
                                        answer.ModifiedBy = answer.CreatedBy = user.FullName;
                                        db.tbl_Answer.Add(answer);
                                    }
                                    await db.SaveChangesAsync();
                                    exerciseIndex += 1;
                                }
                            }
                        }
                        await ExerciseGroupService.UpdateExerciseNumber(db,examSection.ExamId.Value);
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// Thêm câu hỏi tự động
        /// Mặc định mỗi câu 1 điểm
        /// </summary>
        /// <param name="ExamSectionId"></param>
        /// <param name="amount"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task AddRandom(int examSectionId, int amount,int type, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var examSection = await db.tbl_ExamSection.SingleOrDefaultAsync(x => x.Id == examSectionId);
                    if (examSection == null)
                        throw new Exception("Không tìm thấy phần");
                    var model = new AddExerciseGroupModel
                    {
                        ExamSectionId = examSectionId,
                        Items = new List<ExerciseGroupItem>()
                    };
                    var exerciseGroups = await db.tbl_ExerciseGroup
                        .Where(x => x.ExamId != examSection.ExamId && x.Enable == true && x.SourceId == x.Id).ToListAsync();
                    var newExerciseGroups = exerciseGroups;
                    if (newExerciseGroups.Any())
                    {
                        foreach (var item in newExerciseGroups.ToList())
                        {
                            var existInExam = await db.tbl_ExerciseGroup
                                .Where(x => x.ExamId == examSection.ExamId && x.SourceId == item.Id && x.Enable == true).AnyAsync();
                            if (existInExam)
                                exerciseGroups.Remove(item);
                        }
                    }
                    if (type == 1)
                        exerciseGroups = exerciseGroups.Where(x => x.ExerciseNumber == 1).ToList();
                    else
                        exerciseGroups = exerciseGroups.Where(x => x.ExerciseNumber > 1).ToList();

                    if (exerciseGroups.Count() < amount)
                        throw new Exception("Không đủ số lượng câu hỏi");

                    var rd = new Random();
                    var validates = new List<int>();
                    for (int i = 0; i < amount; i++)
                    {
                        exerciseGroups = exerciseGroups.Where(x => !validates.Contains(x.Id)).ToList();
                        var count = exerciseGroups.Count() - 1;
                        var exerciseGroup = exerciseGroups[rd.Next(0, count)];
                        validates.Add(exerciseGroup.Id);
                        var exercises = await db.tbl_Exercise.Where(x => x.ExerciseGroupId == exerciseGroup.Id && x.Enable == true).Select(x => x.Id).ToListAsync();

                        var item = new ExerciseGroupItem
                        {
                            Id = exerciseGroup.Id,
                            ExerciseItems = new List<ExerciseItem>()
                        };
                        exercises.ForEach(x =>
                        {
                            item.ExerciseItems.Add(new ExerciseItem { Id = x, Point = 1 });
                        });
                        model.Items.Add(item);
                    }
                    await AddExerciseGroup(model, user);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

    }
}