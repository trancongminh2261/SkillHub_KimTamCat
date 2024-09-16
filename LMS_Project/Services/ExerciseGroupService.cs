using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class ExerciseGroupService
    {
        public static async Task Insert(ExerciseGroupCreate model, tbl_UserInformation user)
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
                        var exam = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == examSection.ExamId);
                        if (exam == null)
                            throw new Exception("Không tìm thấy đề thi");
                        if (exam.Type == 1 && model.Type == ExerciseType.Essay)
                            throw new Exception("Không thể thêm câu hỏi tự luận trong đề trắc nghiệm");
                        if (exam.Type == 2 && model.Type != ExerciseType.Essay)
                            throw new Exception("Không thể thêm câu hỏi trắc nghiệm trong đề tự luận");

                        var groupIndex = 1;
                        var lastGroup = await db.tbl_ExerciseGroup
                            .Where(x => x.ExamSectionId == model.ExamSectionId && x.Enable == true).OrderByDescending(x => x.Index).FirstOrDefaultAsync();
                        if (lastGroup != null)
                            groupIndex = lastGroup.Index.Value + 1;

                        var exerciseGroup = new tbl_ExerciseGroup(model);
                        exerciseGroup.ModifiedBy = exerciseGroup.CreatedBy = user.FullName;
                        exerciseGroup.ExerciseNumber = model.ExerciseCreates.Count();
                        exerciseGroup.ExamSectionId = examSection.Id;
                        exerciseGroup.Index = groupIndex;
                        exerciseGroup.ExamId = examSection.ExamId;
                        db.tbl_ExerciseGroup.Add(exerciseGroup);
                        await db.SaveChangesAsync();
                        exerciseGroup.SourceId = exerciseGroup.Id;
                        await db.SaveChangesAsync();

                        if (model.ExerciseCreates.Any())
                        {
                            int exerciseIndex = 1;
                            foreach (var item in model.ExerciseCreates)
                            {
                                var exercise = new tbl_Exercise(item);
                                exercise.ModifiedBy = exercise.CreatedBy = user.FullName;
                                exercise.ExerciseGroupId = exerciseGroup.Id;
                                exercise.ExamSectionId = examSection.Id;
                                exercise.ExamId = examSection.ExamId;
                                exercise.Index = exerciseIndex;
                                exercise.Point = item.Point;
                                db.tbl_Exercise.Add(exercise);
                                await db.SaveChangesAsync();

                                if (item.AnswerCreates.Any())
                                {
                                    foreach (var jtem in item.AnswerCreates)
                                    {
                                        var answer = new tbl_Answer(jtem);
                                        answer.ExerciseId = exercise.Id;
                                        answer.ModifiedBy = answer.CreatedBy = user.FullName;
                                        db.tbl_Answer.Add(answer);
                                    }
                                    await db.SaveChangesAsync();
                                }
                                exerciseIndex += 1;
                            }
                        }
                        await UpdateExerciseNumber(db,examSection.ExamId.Value);
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
        /// Cập nhật lại số câu hỏi trong đề
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public static async Task UpdateExerciseNumber(lmsDbContext dbContext ,int examId)
        {
            var exam = await dbContext.tbl_Exam.SingleOrDefaultAsync(x => x.Id == examId);
            if (exam != null)
            {
                var exerciseGroups = await dbContext.tbl_ExerciseGroup.Where(x => x.ExamId == exam.Id && x.Enable == true).ToListAsync();
                if (exerciseGroups.Any())
                {
                    exam.NumberExercise = exerciseGroups.Sum(x => x.ExerciseNumber);
                    exam.DifficultExercise = exerciseGroups.Where(x => x.Level == ExerciseLevel.Difficult).Any() == false ? 0
                        : exerciseGroups.Where(x => x.Level == ExerciseLevel.Difficult).Sum(x => x.ExerciseNumber);
                    exam.NormalExercise = exerciseGroups.Where(x => x.Level == ExerciseLevel.Normal).Any() == false ? 0
                        : exerciseGroups.Where(x => x.Level == ExerciseLevel.Normal).Sum(x => x.ExerciseNumber);
                    exam.EasyExercise = exerciseGroups.Where(x => x.Level == ExerciseLevel.Easy).Any() == false ? 0
                        : exerciseGroups.Where(x => x.Level == ExerciseLevel.Easy).Sum(x => x.ExerciseNumber);
                }
                else
                {
                    exam.NumberExercise = 0;
                    exam.DifficultExercise = 0;
                    exam.NormalExercise = 0;
                    exam.EasyExercise = 0;
                }
                await dbContext.SaveChangesAsync();
            }
        }
        public static async Task Update(ExerciseGroupUpdate model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var exerciseGroup = await db.tbl_ExerciseGroup.SingleOrDefaultAsync(x => x.Id == model.Id);
                        if (exerciseGroup == null)
                            throw new Exception("Không tìm thấy dữ liệu");
                        exerciseGroup.Content = model.Content ?? exerciseGroup.Content;
                        exerciseGroup.Paragraph = model.Paragraph ?? exerciseGroup.Paragraph;
                        exerciseGroup.Level = model.Level ?? exerciseGroup.Level;
                        exerciseGroup.LevelName = model.Level == null ? exerciseGroup.LevelName : model.LevelName;
                        exerciseGroup.VideoUrl = model.VideoUrl ?? exerciseGroup.VideoUrl;
                        exerciseGroup.ModifiedBy = user.FullName;
                        exerciseGroup.ModifiedOn = DateTime.Now;
                        await db.SaveChangesAsync();
                        if (model.ExerciseUpdates.Any())
                        {
                            foreach (var item in model.ExerciseUpdates)
                            {
                                var exercise = await db.tbl_Exercise.SingleOrDefaultAsync(x => x.Id == item.Id);
                                if (exercise == null)
                                {
                                    exercise = new tbl_Exercise(item);
                                    exercise.ModifiedBy = exercise.CreatedBy = user.FullName;
                                    exercise.ExerciseGroupId = exerciseGroup.Id;
                                    exercise.ModifiedOn = exercise.CreatedOn = DateTime.Now;
                                    exercise.ExamId = exerciseGroup.ExamId;
                                    exercise.ExamId = exerciseGroup.ExamSectionId;
                                    exercise.Enable = true;
                                    db.tbl_Exercise.Add(exercise);
                                    exerciseGroup.ExerciseNumber += 1;
                                }
                                else
                                {
                                    exercise.Content = item.Content ?? exercise.Content;
                                    exercise.Index = item.Index ?? exercise.Index;
                                    exercise.InputId = item.InputId ?? exercise.InputId;
                                    exercise.DescribeAnswer = item.DescribeAnswer ?? exercise.DescribeAnswer;
                                    exercise.Point = item.Point ?? exercise.Point;
                                    exercise.Enable = item.Enable ?? exercise.Enable;
                                    if (exercise.Enable == false)
                                    {
                                        exerciseGroup.ExerciseNumber -= 1;
                                    }
                                }
                                await db.SaveChangesAsync();
                                if (item.AnswerUpdates.Any())
                                {
                                    foreach (var jtem in item.AnswerUpdates)
                                    {
                                        var answer = await db.tbl_Answer.SingleOrDefaultAsync(x => x.Id == jtem.Id);
                                        if (answer == null)
                                        {
                                            answer = new tbl_Answer(jtem);
                                            answer.ExerciseId = exercise.Id;
                                            answer.ModifiedBy = answer.CreatedBy = user.FullName;
                                            answer.ModifiedOn = answer.CreatedOn = DateTime.Now;
                                            answer.Enable = true;
                                            db.tbl_Answer.Add(answer);
                                        }
                                        else
                                        {
                                            answer.AnswerContent = jtem.AnswerContent ?? answer.AnswerContent;
                                            answer.IsTrue = jtem.IsTrue ?? answer.IsTrue;
                                            answer.Enable = jtem.Enable ?? answer.Enable;
                                        }
                                        await db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        await UpdateExerciseNumber(db,exerciseGroup.ExamId.Value);
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
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await db.tbl_ExerciseGroup.SingleOrDefaultAsync(x => x.Id == id);
                        if (entity == null)
                            throw new Exception("Không tìm thấy dữ liệu");
                        entity.Enable = false;
                        var exercises = await db.tbl_Exercise.Where(x => x.ExerciseGroupId == entity.Id)
                                         .Select(x => x.Id).ToListAsync();
                        if(exercises.Any())
                            foreach (var item in exercises)
                            {
                                var exercise = await db.tbl_Exercise.SingleOrDefaultAsync(x => x.Id == item);
                                exercise.Enable = false;
                                await db.SaveChangesAsync();
                            }
                        await db.SaveChangesAsync();
                        await UpdateExerciseNumber(db,entity.ExamId.Value);
                        await ReloadIndex(db,entity.ExamSectionId.Value);
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
        public class ExerciseGroupIndexModel
        {
            public List<ExerciseGroupIndexItem> Items { get; set; }
        }
        public class ExerciseGroupIndexItem
        {
            public int Id { get; set; }
            public int Index { get; set; }
        }
        public static async Task ChangeIndex(ExerciseGroupIndexModel model)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!model.Items.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model.Items)
                        {
                            var exerciseGroup = await db.tbl_ExerciseGroup.SingleOrDefaultAsync(x => x.Id == item.Id);
                            exerciseGroup.Index = item.Index;
                            await db.SaveChangesAsync();
                        }
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
        /// Cập nhật lại vị trí
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public static async Task ReloadIndex(lmsDbContext dbContext,int examSectionId)
        {
            var exerciseGroups = await dbContext.tbl_ExerciseGroup.Where(x => x.ExamSectionId == examSectionId && x.Enable == true).OrderBy(x => x.Index).ToListAsync();
            if (exerciseGroups.Any())
            {
                int index = 1;
                foreach (var item in exerciseGroups)
                {
                    var exerciseGroup = await dbContext.tbl_ExerciseGroup.SingleOrDefaultAsync(x => x.Id == item.Id);
                    exerciseGroup.Index = index;
                    await dbContext.SaveChangesAsync();
                    index += 1;
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(ExerciseGroupSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_ExerciseGroup @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@ExerciseGroupId = N'{search.Id}'," +
                    $"@NotExistInExam = N'{search.NotExistInExam}'," +
                    $"@Level = N'{((int)(search.Level ?? 0))}'," +
                    $"@Type = N'{((int)(search.Type ?? 0))}'";
                var data = await db.Database.SqlQuery<Get_ExerciseGroup>(sql).ToListAsync();

                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                int indexInExam = 0;
                var result = data.GroupBy(eg => new
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
                }).Select(eg => new ExerciseGroupModel
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
                    }).Where(x=>x.Key.ExerciseEnable == true && x.Key.ExerciseId.HasValue).Select(e => new ExerciseModel
                    {
                        Id = e.Key.ExerciseId,
                        Content = e.Key.ExerciseContent,
                        InputId = e.Key.InputId,
                        DescribeAnswer = e.Key.DescribeAnswer,
                        IndexInExam = indexInExam += 1,
                        Index = e.Key.ExerciseIndex,
                        Point = e.Key.Point,
                        Answers = e.GroupBy(a => new
                        {
                            a.AnswerId,
                            a.AnswerContent,
                            a.IsTrue,
                            a.AnswerEnable,
                        }).Where(x => x.Key.AnswerEnable == true && x.Key.AnswerId.HasValue).Select(a => new AnswerModel
                        {
                            Id = a.Key.AnswerId,
                            AnswerContent = a.Key.AnswerContent,
                            IsTrue = a.Key.IsTrue,
                        }).ToList()
                    }).ToList()

                }).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}