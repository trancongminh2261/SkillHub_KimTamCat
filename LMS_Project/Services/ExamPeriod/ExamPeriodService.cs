using LMS_Project.Areas.ControllerAPIs.ExamPeriod;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO.ExamPeriod;
using LMS_Project.Enum;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services.ExamPeriod
{
    public class ExamPeriodService : DomainService
    {
        public ExamPeriodService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<ExamPeriodDTO> GetById(int id)
        {
            var data = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data != null)
            {
                return new ExamPeriodDTO(data);
            }
            return null;
        }
        public async Task<tbl_Exam> GetExam(int examPeriodId)
        {
                var examPeriod = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Enable == true && x.Id == examPeriodId);
                if (examPeriod == null)
                    throw new Exception("Không tìm thấy thông tin kỳ thi");
                var data = await dbContext.tbl_Exam.SingleOrDefaultAsync(x => x.Id == examPeriod.ExamId);
                return data;
        }
        public async Task<ExamPeriodDTO> Insert(ExamPeriodCreate request, tbl_UserInformation currentUser)
        {
            var data = new tbl_ExamPeriod(request);
            var checkCode = await dbContext.tbl_ExamPeriod.AnyAsync(x => x.Enable == true && x.Code.Trim() == request.Code.Trim());
            if (checkCode)
                throw new Exception("Mã kỳ thi đã tồn tại");
            if(request.EndTime < request.StartTime)
                throw new Exception("Thời gian kết thúc không được lớn hơn thời gian bắt đầu");
            var checkVideoCourse = await dbContext.tbl_VideoCourse.AnyAsync(x => x.Enable == true && x.Id == request.VideoCourseId);
            if (!checkVideoCourse)
                throw new Exception("Chương trình học không tồn tại");
            var checkExam = await dbContext.tbl_Exam.AnyAsync(x => x.Enable == true && x.Id == request.ExamId);
            if (!checkExam)
                throw new Exception("Đề thi không tồn tại");
            data.CreatedBy = data.ModifiedBy = currentUser.FullName;
            dbContext.tbl_ExamPeriod.Add(data);
            await dbContext.SaveChangesAsync();
            return await GetById(data.Id);
        }
        public async Task<ExamPeriodDTO> Update(ExamPeriodUpdate request, tbl_UserInformation currentUser)
        {
            var data = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Id == request.Id && x.Enable == true);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            if (data.Status != ExamPeriodEnum.Status.UpComing.ToString())
                throw new Exception("Kỳ thi đã diễn ra không thể cập nhật");
            data.Code = request.Code ?? data.Code;
            data.Name = request.Name ?? data.Name;
            data.StartTime = request.StartTime ?? data.StartTime;
            data.EndTime = request.EndTime ?? data.EndTime;
            data.VideoCourseId = request.VideoCourseId ?? data.VideoCourseId;
            data.ExamId = request.ExamId ?? data.ExamId;
            data.ExtensionPeriod = request.ExtensionPeriod ?? data.ExtensionPeriod;
            //data.PassingScore = request.PassingScore ?? data.PassingScore;
            data.Description = request.Description ?? data.Description;
            data.MaxQuantity = request.MaxQuantity ?? data.MaxQuantity;
            data.ModifiedBy = currentUser.FullName;
            data.ModifiedOn = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return await GetById(data.Id);
        }
        public async Task Delete(int id, tbl_UserInformation currentUser)
        {
            var data = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            data.Enable = false;
            data.ModifiedBy = currentUser.FullName;
            data.ModifiedOn = DateTime.Now;
            await dbContext.SaveChangesAsync();
        }
        public async Task<AppDomainResult<ExamPeriodDTO>> GetAll(SearchOptions baseSearch)
        {
            if (baseSearch == null) return new AppDomainResult<ExamPeriodDTO> { TotalRow = 0, Data = null };
            string sql = $"Get_ExamPeriod @PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<ExamPeriodDTO>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult<ExamPeriodDTO> { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            return new AppDomainResult<ExamPeriodDTO> { TotalRow = totalRow ?? 0, Data = data };
        }

        public async Task<AppDomainResult<ExamPeriodStatistical>> GetStatistical(SearchOptions baseSearch)
        {
            var listExamPeriod = await dbContext.tbl_ExamPeriod.Where(x => x.Enable == true).OrderByDescending(x => x.Id).ToListAsync() ?? new List<tbl_ExamPeriod>();
            var totalRow = listExamPeriod.Count;
            listExamPeriod = listExamPeriod.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
            var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true && x.ExamPeriodId != null).ToListAsync() ?? new List<tbl_ExamResult>();
            var listUserInExamPeriod = await dbContext.tbl_UserInExamPeriod.Where(x => x.Enable == true).ToListAsync() ?? new List<tbl_UserInExamPeriod>();
            var result = new List<ExamPeriodStatistical>();
            if (listExamPeriod.Count > 0)
            {               
                foreach(var item in listExamPeriod)
                {
                    var data = new ExamPeriodStatistical();
                    data.Code = item.Code;
                    data.Name = item.Name;
                    data.StartTime = item.StartTime;
                    data.EndTime = item.EndTime;
                    data.TotalJoined = listUserInExamPeriod.Count(x => x.ExamPeriodId == item.ExtensionPeriod);
                    data.TotalSubmit = listExamResult.Count(x => x.ExamPeriodId == item.ExtensionPeriod);
                    data.TotalNotSubmit = data.TotalSubmit - data.TotalJoined;
                    data.TotalPass = listExamResult.Count(x => x.ExamPeriodId == item.ExtensionPeriod && x.IsPass == true);
                    data.TotalFail = data.TotalJoined - data.TotalSubmit;
                    data.RatePass = 0;
                    if (data.TotalJoined > 0)
                        data.RatePass = Math.Round(((double)data.TotalPass / (double)data.TotalJoined * 100), 2);
                    data.RateFail = 100 - data.RatePass;
                    result.Add(data);
                }
            }
            return new AppDomainResult<ExamPeriodStatistical> { TotalRow = totalRow, Data = result };
        }
        public async Task<AppDomainResult> GetDoingTest(int examPeriodId, bool isDoingTest, tbl_UserInformation user)
        {
            var examPeriod = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Enable == true && x.Id == examPeriodId);
            if (examPeriod == null)
                throw new Exception("Không tìm thấy thông tin kỳ thi");
            string sql = $"Get_ExamDetail @PageIndex = 1," +
                $"@PageSize = 9999," +
                $"@ExamId = {examPeriod.ExamId}";
            ///Nếu không phải là admin thì ẩn đáp án
            bool viewResult = false;
            if (user.RoleId != ((int)RoleEnum.student))
                viewResult = true;
            var data = await dbContext.Database.SqlQuery<Get_ExerciseGroup>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
            int indexInExam = 0;
            var result = new List<ExamSectionModel>();
            if (isDoingTest)
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
                        //.OrderBy(x => x.Key.ExerciseGroupIndex).Select(eg => new ExerciseGroupModel
                        .OrderBy(x => Guid.NewGuid()).Select(eg => new ExerciseGroupModel
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
                                 }).OrderBy(x => Guid.NewGuid()).ToList()
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
        public async Task<double> GetTotalPoint(int examPeriodId)
        {
            var examPeriod = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Enable == true && x.Id == examPeriodId);
            if (examPeriod == null)
                throw new Exception("Không tìm thấy thông tin kỳ thi");
            double totalPoint = 0;
            var examSectionIds = await dbContext.tbl_ExamSection.Where(x => x.ExamId == examPeriod.ExamId && x.Enable == true).Select(x => x.Id).ToListAsync();
            if (examSectionIds.Any())
            {
                foreach (var item in examSectionIds)
                {
                    var groupIds = await dbContext.tbl_ExerciseGroup.Where(x => x.ExamSectionId == item && x.Enable == true).Select(x => x.Id).ToListAsync();
                    if (groupIds.Any())
                    {
                        foreach (var groupId in groupIds)
                        {
                            var exercises = await dbContext.tbl_Exercise
                                .Where(x => x.ExerciseGroupId == groupId && x.Enable == true)
                                .ToListAsync();

                            totalPoint = exercises.Sum(x => x.Point ?? 0);
                        }
                    }
                }
            }
            return totalPoint;
        }
        //tự động cập nhật trạng thái kì thi khi đến giờ thi hoặc đến giờ kết thúc
        public static async Task AutoUpdateStatus()
        {
            using (var db = new lmsDbContext())
            {
                var timenow = DateTime.Now;
                //cập nhật cho các kỳ thi đang diễn ra
                var listExamPeriodOpening = await db.tbl_ExamPeriod
                    .Where(x => x.Enable == true && timenow <= x.EndTime)
                    .ToListAsync();
                if (listExamPeriodOpening.Count > 0)
                {
                    foreach (var item in listExamPeriodOpening)
                    {
                        item.Status = ExamPeriodEnum.Status.Opening.ToString();
                    }
                }

                //cập nhật lớp kết thúc
                var listExamPeriodClosed = await db.tbl_ExamPeriod
                    .Where(x => x.Enable == true && timenow > x.EndTime && x.Status == ExamPeriodEnum.Status.Closed.ToString())
                    .ToListAsync();
                if (listExamPeriodClosed.Count > 0)
                {
                    foreach (var item in listExamPeriodClosed)
                    {
                        item.Status = ExamPeriodEnum.Status.Closed.ToString();
                    }
                }
                await db.SaveChangesAsync();
            }
        }
    }
}