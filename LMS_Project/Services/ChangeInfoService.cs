using LMS_Project.Areas.Models;
using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class ChangeInfoService
    {
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var user = await db.tbl_ChangeInfo.SingleOrDefaultAsync(x => x.Id == id);
                    if (user == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    user.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Approve(int id, ChangeInfoStatus status, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_ChangeInfo.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Status = ((int)status);
                    entity.StatusName = status == ChangeInfoStatus.Approve ? "Đã Duyệt"
                                        : status == ChangeInfoStatus.Cancel ? "Không duyệt" : "";
                    entity.Enable = false;
                    if (status == ChangeInfoStatus.Approve)
                    {
                        var userInfo = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == entity.UserInformationId);
                        if (userInfo == null)
                            throw new Exception("Không tin thấy thông tin người dùng");
                        userInfo.FullName = entity.FullName;
                        userInfo.UserName = entity.UserName;
                        userInfo.Email = entity.Email;
                        userInfo.DOB = entity.DOB;
                        userInfo.Gender = entity.Gender;
                        userInfo.Mobile = entity.Mobile;
                        userInfo.Address = entity.Address;
                        userInfo.Avatar = entity.Avatar;
                        userInfo.AreaId = entity.AreaId;
                        userInfo.DistrictId = entity.DistrictId;
                        userInfo.WardId = entity.WardId;
                        userInfo.NickName = entity.NickName;
                        userInfo.CMND = entity.CMND;
                    }
                    await db.SaveChangesAsync();
                    ///Gửi thông báo
                    string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                    string title = "Thay đổi thông tin";
                    string content = $"Thông tin cá nhân của bạn đã được thay đổi vui lòng kiểm tra lại, thông báo từ {projectName}";
                    if(status == ChangeInfoStatus.Cancel)
                        content = $"Yêu cầu thay đổi thông tin cá nhân của bạn không được duyệt, thông báo từ {projectName}";
                    await NotificationService.Send(
                                        new tbl_Notification
                                        {
                                            UserId = entity.UserInformationId,
                                            Title = title,
                                            Content = content
                                        }, new tbl_UserInformation { FullName = "Tự động" });
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(ChangeInfo search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_ChangeInfo @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@UserCode = N'{search.UserCode ?? ""}'," +
                    $"@Status = N'{search.Statuss ?? ""}'";
                var data = await db.Database.SqlQuery<Get_ChangeInfo>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_ChangeInfo {
                    Address = i.Address,
                    AreaId = i.AreaId,
                    AreaName = i.AreaName,
                    Avatar = i.Avatar,
                    CreatedBy = i.CreatedBy,
                    CreatedOn = i.CreatedOn,
                    DistrictId = i.DistrictId,
                    DistrictName = i.DistrictName,
                    DOB = i.DOB,
                    Enable = i.Enable,
                    FullName = i.FullName,
                    Gender = i.Gender,
                    Id = i.Id,
                    Mobile = i.Mobile,
                    ModifiedBy = i.ModifiedBy,
                    ModifiedOn = i.ModifiedOn,
                    Status = i.Status,
                    StatusName = i.StatusName,
                    UserCode = i.UserCode,
                    UserName = i.UserName,
                    Email = i.Email,
                    UserInformationId = i.UserInformationId,
                    WardId = i.WardId,
                    WardName = i.WardName,
                    CMND = i.CMND,
                    NickName = i.NickName,
                    Info = Task.Run(() => GetInfo(i.UserInformationId)).Result
                }).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task<object> GetInfo(int userInformationId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == userInformationId);
                var area = await db.tbl_Area.SingleOrDefaultAsync(x => x.Id == data.AreaId);
                var district = await db.tbl_District.SingleOrDefaultAsync(x => x.Id == data.DistrictId);
                var ward = await db.tbl_Ward.SingleOrDefaultAsync(x => x.Id == data.WardId);
                var result = new
                {
                    UserInformationId = data.UserInformationId,
                    FullName = data.FullName,
                    UserName = data.UserName,
                    UserCode = data.UserCode,
                    DOB = data.DOB,
                    Gender = data.Gender,
                    Mobile = data.Mobile,
                    Email = data.Email,
                    Address = data.Address,
                    Avatar = data.Avatar,
                    AreaId = data.AreaId,
                    AreaName = area?.Name,
                    DistrictId = data.UserInformationId,
                    DistrictName = district?.Name,
                    WardId = data.UserInformationId,
                    WardName = ward?.Name,
                    CMND = data.CMND,
                    NickName = data.NickName,
                };
                return result;
            }
        }
    }
}