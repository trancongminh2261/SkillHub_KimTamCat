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
    public class NotificationInVideoService
    {
        public static async Task<tbl_NotificationInVideo> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_NotificationInVideo.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_NotificationInVideo> Insert(NotificationInVideoCreate notificationInVideoCreate,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_NotificationInVideo(notificationInVideoCreate);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_NotificationInVideo.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        /// <summary>
        /// Tự động gửi thông báo
        /// </summary>
        /// <returns></returns>
        public static async Task SeenNotification()
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_NotificationInVideo.Where(x => x.IsSend == false && x.Enable == true)
                    .Select(x => x.Id).ToListAsync();
                if (entity.Any())
                {
                    foreach (var item in entity)
                    {
                        var notificationInVideo = await db.tbl_NotificationInVideo.SingleOrDefaultAsync(x => x.Id == item);
                        notificationInVideo.IsSend = true;
                        var sendUsers = await db.tbl_VideoCourseStudent
                            .Where(x => x.VideoCourseId == notificationInVideo.VideoCourseId && x.Enable == true).Select(x=>x.UserId).ToListAsync();
                        if (sendUsers.Any())
                        {
                            foreach (var jtem in sendUsers)
                            {
                                await NotificationService.Send(
                                    new tbl_Notification
                                    {
                                        UserId = jtem,
                                        Title = notificationInVideo.Title,
                                        Content = notificationInVideo.Content
                                    }, new tbl_UserInformation { FullName = "Tự động" });
                            }
                        }
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(NotificationInVideoSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_NotificationInVideo @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@VideoCourseId = N'{search.VideoCourseId ?? 0}'";
                var data = await db.Database.SqlQuery<Get_NotificationInVideo>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_NotificationInVideo(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
    }
}