using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO;
using LMS_Project.Enum;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project.Services
{
    public class VideoConfigService : DomainService
    {
        public VideoConfigService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<int> NewIndex(int videoConfigId)
        {
            var lastIndex = await dbContext.tbl_VideoConfigQuestion
                .Where(x => x.VideoConfigId == videoConfigId && x.Enable == true)
                .OrderByDescending(x => x.Index)
                .FirstOrDefaultAsync();
            if (lastIndex == null)
                return 1;
            return lastIndex.Index + 1;
        }
        public async Task ReloadIndex(int videoConfigId)
        {
            var details = await dbContext.tbl_VideoConfigQuestion
                .Where(x => x.VideoConfigId == videoConfigId && x.Enable == true)
                .OrderBy(x => x.Index)
                .ToListAsync();
            if (details.Any())
            {
                int index = 1;
                foreach (var item in details)
                {
                    item.Index = index;
                    index++;
                }
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task<VideoConfigDTO> GetById(int id)
        {
            var data = await dbContext.tbl_VideoConfig.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data != null)
            {
                return new VideoConfigDTO(data);
            }
            return null;
        }
        public async Task<VideoConfigDTO> Insert(VideoConfigCreate request, tbl_UserInformation currentUser)
        {
            using (var tran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var lessonVideo = await dbContext.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == request.LessonVideoId && x.Enable == true);
                    if (lessonVideo == null)
                        throw new Exception("Không tìm thấy bài học");
                    var checkTime = await dbContext.tbl_VideoConfig.SingleOrDefaultAsync(x => x.Enable == true && x.LessonVideoId == request.LessonVideoId && x.StopInMinute == request.StopInMinute);
                    if (checkTime != null)
                        throw new Exception("Đã có cấu hình tại thời gian dừng video này");
                    var model = new tbl_VideoConfig(request);
                    model.CreatedBy = model.ModifiedBy = currentUser.FullName;
                    dbContext.tbl_VideoConfig.Add(model);
                    await dbContext.SaveChangesAsync();

                    if (request.Type == VideoConfigEnum.Type.AnswerQuestion && request.VideoConfigQuestions != null && request.VideoConfigQuestions.Any())
                    {
                        foreach (var question in request.VideoConfigQuestions)
                        {
                            var videoConfigQuestion = new tbl_VideoConfigQuestion(question);
                            videoConfigQuestion.ModifiedBy = videoConfigQuestion.CreatedBy = currentUser.FullName;
                            videoConfigQuestion.CreatedOn = DateTime.Now;
                            videoConfigQuestion.ModifiedOn = DateTime.Now;
                            videoConfigQuestion.VideoConfigId = model.Id;
                            videoConfigQuestion.Index = await NewIndex(model.Id);
                            dbContext.tbl_VideoConfigQuestion.Add(videoConfigQuestion);
                            await dbContext.SaveChangesAsync();
                            if (question.VideoConfigOptions.Any())
                            {
                                foreach (var option in question.VideoConfigOptions)
                                {
                                    var videoConfigOption = new tbl_VideoConfigOption(option);
                                    videoConfigOption.VideoConfigQuestionId = videoConfigQuestion.Id;
                                    videoConfigOption.ModifiedBy = videoConfigOption.CreatedBy = currentUser.FullName;
                                    videoConfigOption.IsCorrect = videoConfigOption.IsCorrect;
                                    dbContext.tbl_VideoConfigOption.Add(videoConfigOption);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    tran.Commit();
                    return await GetById(model.Id);
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw e;
                }
            }
        }
        public async Task<VideoConfigDTO> Update(VideoConfigUpdate request, tbl_UserInformation currentUser)
        {
            using (var tran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var entity = await dbContext.tbl_VideoConfig.SingleOrDefaultAsync(x => x.Id == request.Id && x.Enable == true);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    var checkTime = await dbContext.tbl_VideoConfig.SingleOrDefaultAsync(x => x.Enable == true && x.LessonVideoId == entity.LessonVideoId && x.StopInMinute == request.StopInMinute && request.StopInMinute != entity.StopInMinute);
                    if (checkTime != null)
                        throw new Exception("Đã có cấu hình tại thời gian dừng video này");
                    entity.StopInMinute = request.StopInMinute ?? entity.StopInMinute;
                    entity.Type = request.Type != null ? request.Type.ToString() : entity.Type;
                    entity.ModifiedBy = currentUser.FullName;
                    entity.ModifiedOn = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                    //xóa hết danh sách câu hỏi và câu trả lời
                    var listQuestion = await dbContext.tbl_VideoConfigQuestion.Where(x => x.Enable == true && x.VideoConfigId == entity.Id).ToListAsync();
                    if(listQuestion.Count > 0)
                    {
                        foreach(var question in listQuestion)
                        {                      
                            var listOption = await dbContext.tbl_VideoConfigOption.Where(x => x.Enable == true && x.VideoConfigQuestionId == question.Id).ToListAsync();
                            if (listOption.Count > 0)
                            {
                                dbContext.tbl_VideoConfigOption.RemoveRange(listOption);
                            }
                            dbContext.tbl_VideoConfigQuestion.Remove(question);
                            await dbContext.SaveChangesAsync();
                        }
                    }

                    //tạo danh sách câu hỏi và lựa chọn mới mà FE gửi xuống
                    if (request.VideoConfigQuestions != null && request.VideoConfigQuestions.Any())
                    {
                        foreach (var question in request.VideoConfigQuestions)
                        {
                            var videoConfigQuestion = new tbl_VideoConfigQuestion(question);
                            videoConfigQuestion.ModifiedBy = videoConfigQuestion.CreatedBy = currentUser.FullName;
                            videoConfigQuestion.CreatedOn = DateTime.Now;
                            videoConfigQuestion.ModifiedOn = DateTime.Now;
                            videoConfigQuestion.VideoConfigId = entity.Id;
                            videoConfigQuestion.Index = await NewIndex(entity.Id); ;
                            dbContext.tbl_VideoConfigQuestion.Add(videoConfigQuestion);
                            await dbContext.SaveChangesAsync();
                            if (question.VideoConfigOptions.Any())
                            {
                                foreach (var option in question.VideoConfigOptions)
                                {
                                    var videoConfigOption = new tbl_VideoConfigOption(option);
                                    videoConfigOption.VideoConfigQuestionId = videoConfigQuestion.Id;
                                    videoConfigOption.ModifiedBy = videoConfigOption.CreatedBy = currentUser.FullName;
                                    videoConfigOption.IsCorrect = videoConfigOption.IsCorrect;
                                    dbContext.tbl_VideoConfigOption.Add(videoConfigOption);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    tran.Commit();
                    return await GetById(entity.Id);
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw e;
                }
            }
        }
        public async Task Delete(int id, tbl_UserInformation currentUser)
        {
            var data = await dbContext.tbl_VideoConfig.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            data.Enable = false;
            data.ModifiedBy = currentUser.FullName;
            data.ModifiedOn = DateTime.Now;
            await dbContext.SaveChangesAsync();
        }
        public async Task<VideoConfigDetailDTO> GetDetail(int id)
        {
            var data = await dbContext.tbl_VideoConfig.SingleOrDefaultAsync(x => x.Enable == true && x.Id == id);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            var videoConfigDetailDTO = new VideoConfigDetailDTO();
            videoConfigDetailDTO.LessonVideoId = data.LessonVideoId;
            videoConfigDetailDTO.StopInMinute = data.StopInMinute;
            videoConfigDetailDTO.Type = data.Type;
            //khởi tạo nội dung: detail
            List<VideoConfigQuestionDTO> videoConfigQuestions = new List<VideoConfigQuestionDTO>();
            var questions = await dbContext.tbl_VideoConfigQuestion.Where(x => x.Enable == true && x.VideoConfigId == data.Id).OrderBy(x => x.Index).ToListAsync();
            if (questions.Count > 0)
            {
                foreach (var q in questions)
                {
                    VideoConfigQuestionDTO videoConfigQuestion = new VideoConfigQuestionDTO();
                    videoConfigQuestion.Id = q.Id;
                    videoConfigQuestion.Content = q.Content;
                    videoConfigQuestion.Index = q.Index;
                    // ds đáp án
                    List<VideoConfigOptionDTO> listOption = new List<VideoConfigOptionDTO>();
                    var options = await dbContext.tbl_VideoConfigOption.Where(x => x.Enable == true && x.VideoConfigQuestionId == q.Id).ToListAsync();
                    if (options.Count > 0)
                    {
                        foreach (var o in options)
                        {
                            VideoConfigOptionDTO videoConfigOption = new VideoConfigOptionDTO();
                            videoConfigOption.Id = o.Id;
                            videoConfigOption.Content = o.Content;
                            videoConfigOption.IsCorrect = o.IsCorrect;
                            listOption.Add(videoConfigOption);
                        }
                    }
                    videoConfigQuestion.VideoConfigOptions = listOption;
                    videoConfigQuestions.Add(videoConfigQuestion);
                }
                videoConfigDetailDTO.VideoConfigQuestions = videoConfigQuestions;
            }
            return videoConfigDetailDTO;
        }

        public async Task<IList<VideoConfigDTO>> GetByLessonVideo(int lessonVideoId)
        {
            var data = await dbContext.tbl_VideoConfig.Where(x => x.Enable == true && x.LessonVideoId == lessonVideoId)
                    .Select(x => new VideoConfigDTO
                    {
                        LessonVideoId = x.LessonVideoId,
                        Type = x.Type,
                        StopInMinute = x.StopInMinute,
                        TotalQuestion = 0,
                        Id = x.Id,
                        CreatedBy = x.CreatedBy,
                        CreatedOn = x.CreatedOn,
                        ModifiedBy = x.ModifiedBy,
                        ModifiedOn = x.ModifiedOn
                    }).ToListAsync();
            foreach (var item in data)
            {
                if (item.Type == VideoConfigEnum.Type.AnswerQuestion.ToString())
                {
                    var totalQuestion = await dbContext.tbl_VideoConfigQuestion.CountAsync(x => x.Enable == true && x.VideoConfigId == item.Id);
                    item.TotalQuestion = totalQuestion;
                }
            }
            return data;
        }
    }
}