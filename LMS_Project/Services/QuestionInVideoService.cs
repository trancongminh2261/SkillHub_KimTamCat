using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class QuestionInVideoService
    {
        public static async Task<tbl_QuestionInVideo> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_QuestionInVideo.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_QuestionInVideo> Insert(QuestionInVideoCreate questionInVideoCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var validate = await db.tbl_VideoCourse.AnyAsync(x => x.Id == questionInVideoCreate.VideoCourseId);
                    if (!validate)
                        throw new Exception("Không tìm thấy khoá học");
                    //var hasLessonVideo = await db.tbl_LessonVideo.AnyAsync(x => x.Id == questionInVideoCreate.LessonVideoId && x.Enable == true);
                    //if (!hasLessonVideo)
                    //    throw new Exception("Không tìm thấy bài học");
                    var model = new tbl_QuestionInVideo(questionInVideoCreate);
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    model.UserId = user.UserInformationId;
                    db.tbl_QuestionInVideo.Add(model);
                    await db.SaveChangesAsync();
                    return model;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
        }
        public static async Task Delete(int id, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_QuestionInVideo.SingleOrDefaultAsync(x => x.Id == id);
                    if (user.RoleId != ((int)RoleEnum.admin) && entity.UserId != user.UserInformationId)
                        throw new Exception("Không có quyền xoá");
                    entity.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(QuestionInVideoSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_QuestionInVideo @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@UserId = {search.UserId}," +
                    $"@LessonVideoId = {search.LessonVideoId}," +
                    $"@VideoCourseId = {search.VideoCourseId}";
                var data = await db.Database.SqlQuery<Get_QuestionInVideo>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_QuestionInVideo(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}