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
    public class AnswerInVideoService
    {
        public static async Task<tbl_AnswerInVideo> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_AnswerInVideo.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_AnswerInVideo> Insert(AnswerInVideoCreate answerInVideoCreate,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var validate = await db.tbl_QuestionInVideo.AnyAsync(x => x.Id == answerInVideoCreate.QuestionInVideoId);
                    if (!validate)
                        throw new Exception("Không tìm thấy câu hỏi");
                    var model = new tbl_AnswerInVideo(answerInVideoCreate);
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    model.UserId = user.UserInformationId;
                    db.tbl_AnswerInVideo.Add(model);
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
                    var entity = await db.tbl_AnswerInVideo.SingleOrDefaultAsync(x => x.Id == id);
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
        public static async Task<List<AnswerInVideoModel>> GetByQuestion(int questionInVideoId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_AnswerInVideo
                    .Where(x => x.QuestionInVideoId == questionInVideoId && x.Enable == true).ToListAsync();
                var users = await db.tbl_UserInformation.Where(x => x.Enable == true).ToListAsync();
                var result = (from i in data
                              join u in users on i.UserId equals u.UserInformationId
                              select new AnswerInVideoModel
                              {
                                  Id = i.Id,
                                  UserId = u.UserInformationId,
                                  Avatar = u.Avatar,
                                  Content = i.Content,
                                  CreatedBy = i.CreatedBy,
                                  CreatedOn = i.CreatedOn,
                                  Enable = i.Enable,
                                  FullName = u.FullName,
                                  ModifiedBy = i.ModifiedBy,
                                  ModifiedOn = i.ModifiedOn,
                                  QuestionInVideoId = i.QuestionInVideoId,
                                  UserCode = u.UserCode,
                              }).ToList();
                return result;
            }
        }
    }
}