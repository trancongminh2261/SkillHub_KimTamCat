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
using System.Threading;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;
namespace LMS_Project.Services
{
    public class NotificationService
    {
        public static async Task Send(tbl_Notification model, tbl_UserInformation userLog, string EmailContent = "")
        {
            using (var db = new lmsDbContext())
            {
                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == model.UserId);
                model.IsSeen = false;
                model.CreatedBy = model.ModifiedBy = userLog.FullName;
                model.CreatedOn = model.ModifiedOn = DateTime.Now;
                model.Enable = true;
                db.tbl_Notification.Add(model);
                await db.SaveChangesAsync();
                Thread send = new Thread(() =>
                {
                    ///Gửi mail
                    AssetCRM.SendMail(user.Email, model.Title, EmailContent == "" ? model.Content : EmailContent);
                    ///Thông báo desktop
                    //AssetCRM.OneSignalPushNotifications(model.Title, model.Content, user.OneSignal_DeviceId,"");
                    AssetCRM.OneSignalPushNotiV2(model.Title, model.Content, new List<string> { user.OneSignal_DeviceId }, "");
                });
                send.Start();
            }
        }
        public class SendNotThreadModel
        {
            public string Content { get; set; }
            public string Title { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }
            public string OnesignalId { get; set; }
            public string EmailContent { get; set; }
            public string OnesignalContent { get; set; }
            public string OnesignalUrl { get; set; }
        }
        public static void SendNotThread(lmsDbContext db, SendNotThreadModel itemModel, tbl_UserInformation userLog, bool? IsSendMail = true)
        {
            try
            {
                var model = new tbl_Notification
                {
                    Content = itemModel.Content,
                    CreatedBy = userLog.FullName,
                    CreatedOn = DateTime.Now,
                    Enable = true,
                    IsSeen = false,
                    ModifiedBy = userLog.FullName,
                    ModifiedOn = DateTime.Now,
                    Title = itemModel.Title,
                    UserId = itemModel.UserId
                };
                db.tbl_Notification.Add(model);
                db.SaveChanges();
                if(IsSendMail == true)
                {
                    ///Gửi mail
                    AssetCRM.SendMail(itemModel.Email, model.Title, string.IsNullOrEmpty(itemModel.EmailContent) ? model.Content : itemModel.EmailContent);
                }
                ///Thông báo desktop
                //AssetCRM.OneSignalPushNotifications(itemModel.Title, itemModel.OnesignalContent, itemModel.OnesignalId, itemModel.OnesignalUrl);
                AssetCRM.OneSignalPushNotiV2(model.Title, model.Content, new List<string> { itemModel.OnesignalId }, itemModel.OnesignalUrl);
            }
            catch
            {
                return;
            }
        }
        /// <summary>
        /// Xác nhận xem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task Seen(int id,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_Notification.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy thông báo");
                    entity.IsSeen = true;
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// Xác nhận xem tất cả
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task SeenAll(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var notifications = await db.tbl_Notification
                    .Where(x => x.UserId == user.UserInformationId && x.Enable == true && x.IsSeen == false)
                    .Select(x=>x.Id).ToListAsync();
                if (notifications.Any())
                {
                    foreach (var item in notifications)
                    {
                        var notification = await db.tbl_Notification.SingleOrDefaultAsync(x => x.Id == item);
                        notification.IsSeen = true;
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(SearchOptions search,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_Notification @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@UserId = N'{user.UserInformationId}'";
                var data = await db.Database.SqlQuery<Get_Notification>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_Notification(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result};
            }
        }
        public static async Task<tbl_Config> MaintenanceNotice()
        {
            using (var db = new lmsDbContext())
            {
                var config = await db.tbl_Config.FirstOrDefaultAsync(x => x.Code == "MaintenanceNotice");
                if (config == null)
                {
                    db.tbl_Config.Add(new tbl_Config
                    {
                        Code = "MaintenanceNotice",
                        Name = "Không có nội dung thông báo",
                        Value = "false"
                    });
                    await db.SaveChangesAsync();
                }
                return config;
            }
        }
    }
}