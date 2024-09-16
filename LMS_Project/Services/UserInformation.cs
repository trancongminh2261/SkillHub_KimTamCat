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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class UserInformation
    {
        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static async Task<tbl_UserInformation> GetById(int userinfomationId)
        {
            tbl_UserInformation account = new tbl_UserInformation();
            using (lmsDbContext _db = new lmsDbContext())
            {
                account = await _db.tbl_UserInformation.Where(c => c.UserInformationId == userinfomationId).FirstOrDefaultAsync();
            }
            return account;
        }
        public static tbl_UserInformation GetByUserName(string username)
        {
            tbl_UserInformation account = new tbl_UserInformation();
            using (lmsDbContext _db = new lmsDbContext())
            {
                account = _db.tbl_UserInformation.Where(c => c.UserName.ToUpper() == username.ToUpper()).FirstOrDefault();
            }
            return account;
        }
        public static async Task<tbl_UserInformation> Insert(tbl_UserInformation user, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    user.CreatedBy = user.ModifiedBy = userLogin.FullName;
                    user.ActiveDate = DateTime.Now;
                    db.tbl_UserInformation.Add(user);
                    string baseCode = user.RoleId == ((int)RoleEnum.admin) ? "QTV"
                                        : user.RoleId == ((int)RoleEnum.manager) ? "QL"
                                        : user.RoleId == ((int)RoleEnum.teacher) ? "GV"
                                        : user.RoleId == ((int)RoleEnum.student) ? "HV" : "";
                    int count = await db.tbl_UserInformation.CountAsync(x => x.RoleId == user.RoleId
                            && x.CreatedOn.Value.Year == user.CreatedOn.Value.Year
                            && x.CreatedOn.Value.Month == user.CreatedOn.Value.Month
                            && x.CreatedOn.Value.Day == user.CreatedOn.Value.Day);
                    user.UserCode = AssetCRM.InitCode(baseCode, user.CreatedOn.Value, count + 1);
                    await db.SaveChangesAsync();

                    ////Tạo mã người dùng
                    //if (user.RoleId == ((int)RoleEnum.student))
                    //{
                    //    var codeIndex = await db.tbl_UserInformation.CountAsync(x => x.AreaId == area.Id && x.RoleId == ((int)RoleEnum.student));
                    //    user.UserCode = ($"{area.Abbreviation}0000")
                    //        .Remove((area.Abbreviation.Length + 4) - codeIndex.ToString().Length) + codeIndex.ToString();
                    //    await db.SaveChangesAsync();
                    //}
                    return user;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task ValidateUser(string userName, string email)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var checkUser = await db.tbl_UserInformation
                        .Where(x => x.UserName.ToUpper() == userName.ToUpper() && x.Enable == true).AnyAsync();
                    if (checkUser)
                        throw new Exception($"Tên đăng nhập {userName} đã tồn tại");
                    //var checkEmail = await db.tbl_UserInformation
                    //    .Where(x => x.Email.ToUpper() == email.ToUpper() && x.Enable == true).AnyAsync();
                    //if (checkEmail)
                    //    throw new Exception($"Email {email} đã tồn tại");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<tbl_UserInformation> Update(tbl_UserInformation user, tbl_UserInformation userLogin)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == user.UserInformationId);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    if (!string.IsNullOrEmpty(user.UserName) && user.UserName.ToUpper() != entity.UserName.ToUpper())
                    {
                        var validate = await db.tbl_UserInformation.Where(x => x.UserName.ToUpper() == user.UserName.ToUpper() && x.Enable == true).AnyAsync();
                        if (validate)
                            throw new Exception("Tên đăng nhập đã tồn tại");
                    }
                    //if (!string.IsNullOrEmpty(user.Email) && user.Email.ToUpper() != entity.Email.ToUpper())
                    //{
                    //    var validate = await db.tbl_UserInformation.Where(x => x.Email.ToUpper() == user.Email.ToUpper()).AnyAsync();
                    //    if (validate)
                    //        throw new Exception("Email đã tồn tại");
                    //}
                    //if (!string.IsNullOrEmpty(user.CMND) && user.CMND.ToUpper() != entity.CMND.ToUpper())
                    //{
                    //    var validate = await db.tbl_UserInformation.Where(x => x.CMND.ToUpper() == user.CMND.ToUpper() && x.Enable == true).AnyAsync();
                    //    if (validate)
                    //        throw new Exception("CMND đã tồn tại");
                    //    validate = await db.tbl_UserInformation.Where(x => x.UserName.ToUpper() == user.CMND.ToUpper() && x.Enable == true).AnyAsync();
                    //    if (validate)
                    //        throw new Exception("CMND đã tồn tại");
                    //}
                    //if (userLogin.RoleId != ((int)RoleEnum.student))
                    {
                        entity.FullName = user.FullName ?? entity.FullName;
                        entity.UserName = user.UserName ?? entity.UserName;
                        entity.Email = user.Email ?? entity.Email;
                        entity.DOB = user.DOB ?? entity.DOB;
                        entity.Gender = user.Gender ?? entity.Gender;
                        entity.Mobile = user.Mobile ?? entity.Mobile;
                        entity.Address = user.Address ?? entity.Address;
                        if (entity.StatusId == ((int)AccountStatus.inActive) && user.StatusId == ((int)AccountStatus.active))
                            entity.ActiveDate = DateTime.Now;
                        entity.StatusId = user.StatusId ?? entity.StatusId;
                        entity.Avatar = user.Avatar ?? entity.Avatar;
                        entity.AreaId = user.AreaId ?? entity.AreaId;
                        entity.DistrictId = user.DistrictId ?? entity.DistrictId;
                        entity.WardId = user.WardId ?? entity.WardId;
                        entity.DepartmentId = user.DepartmentId ?? entity.DepartmentId;
                        entity.NickName = user.NickName ?? entity.NickName;
                        entity.CMND = user.CMND ?? entity.CMND;
                        entity.Password = string.IsNullOrEmpty(user.Password) ? entity.Password : Encryptor.Encrypt(user.Password);
                        entity.ModifiedOn = user.ModifiedOn;
                        entity.ModifiedBy = userLogin.FullName;
                    }
                    //else
                    //{
                    //    var change = new tbl_ChangeInfo
                    //    {
                    //        Address = user.Address ?? entity.Address,
                    //        AreaId = user.AreaId ?? entity.AreaId,
                    //        Avatar = user.Avatar ?? entity.Avatar,
                    //        NickName = user.NickName ?? entity.NickName,
                    //        CreatedBy = userLogin.FullName,
                    //        CreatedOn = DateTime.Now,
                    //        DistrictId = user.DistrictId ?? entity.DistrictId,
                    //        DOB = user.DOB ?? entity.DOB,
                    //        Enable = true,
                    //        FullName = user.FullName ?? entity.FullName,
                    //        UserName = user.UserName ?? entity.UserName,
                    //        Email = user.Email ?? entity.Email,
                    //        Gender = user.Gender ?? entity.Gender,
                    //        Mobile = user.Mobile ?? entity.Mobile,
                    //        CMND = user.CMND ?? entity.CMND,
                    //        ModifiedBy = userLogin.FullName,
                    //        ModifiedOn = DateTime.Now,
                    //        Status = 1,
                    //        StatusName = "Chờ duyệt",
                    //        UserCode = entity.UserCode,
                    //        UserInformationId = entity.UserInformationId,
                    //        WardId = user.WardId ?? entity.WardId
                    //    };
                    //    if (change.Address != entity.Address ||
                    //        change.AreaId != entity.AreaId ||
                    //        change.Avatar != entity.Avatar ||
                    //        change.DistrictId != entity.DistrictId ||
                    //        change.DOB?.Date != entity.DOB?.Date ||
                    //        change.FullName != entity.FullName ||
                    //        change.UserName != entity.UserName ||
                    //        change.Email != entity.Email ||
                    //        change.Gender != entity.Gender ||
                    //        change.Mobile != entity.Mobile ||
                    //        change.CMND != entity.CMND ||
                    //        change.NickName != entity.NickName ||
                    //        user.WardId != entity.WardId)
                    //    db.tbl_ChangeInfo.Add(change);
                    //}
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int userInformationId)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == userInformationId);
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
        public static async Task<AppDomainResult> GetAll(UserSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new UserSearch();
                string sql = $"Get_User @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@FullName = N'{baseSearch.FullName ?? ""}'," +
                    $"@UserCode = N'{baseSearch.UserCode ?? ""}'," +
                    $"@RoleId = N'{baseSearch.RoleIds ?? ""}'," +
                    $"@Gender = N'{baseSearch.Genders ?? ""}'," +
                    $"@Sort = {baseSearch.Sort}," +
                    $"@DepartmentId = {baseSearch.DepartmentId ?? 0}," +
                    $"@StatusId = {baseSearch.StatusId ?? 2}," +
                    $"@SortType = {baseSearch.SortType}";
                var data = await db.Database.SqlQuery<Get_UserInformation>(sql).ToListAsync();
                if(!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new UserInformationModel(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        public static async Task ImportData(List<RegisterModel> model,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!model.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model)
                        {
                            var checkUser = await db.tbl_UserInformation
                                .Where(x => x.UserName.ToUpper() == item.UserName.ToUpper() && x.Enable == true).AnyAsync();
                            if (checkUser)
                                continue;
                            var newUser = new tbl_UserInformation(item);
                            newUser.CreatedBy = newUser.ModifiedBy = user.FullName;
                            newUser.ActiveDate = DateTime.Now;
                            db.tbl_UserInformation.Add(newUser);

                            string baseCode = newUser.RoleId == ((int)RoleEnum.admin) ? "QTV"
                                                : newUser.RoleId == ((int)RoleEnum.manager) ? "QL"
                                                : newUser.RoleId == ((int)RoleEnum.teacher) ? "GV"
                                                : newUser.RoleId == ((int)RoleEnum.student) ? "HV" : "";
                            int count = await db.tbl_UserInformation.CountAsync(x => x.RoleId == user.RoleId
                                    && x.CreatedOn.Value.Year == user.CreatedOn.Value.Year
                                    && x.CreatedOn.Value.Month == user.CreatedOn.Value.Month
                                    && x.CreatedOn.Value.Day == user.CreatedOn.Value.Day);
                            newUser.UserCode = AssetCRM.InitCode(baseCode, newUser.CreatedOn.Value, count + 1);
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
        public static async Task<bool> Update_OneSignal_DeviceId(string oneSignal_deviceId, tbl_UserInformation userInformation)
        {
            using (var db = new lmsDbContext())
            {
                if (!string.IsNullOrEmpty(oneSignal_deviceId))
                {
                    var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == userInformation.UserInformationId);
                    user.OneSignal_DeviceId = oneSignal_deviceId;
                    await db.SaveChangesAsync();
                }
                return true;
            }
        }
        //public static async Task AutoInActive()
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        var time = DateTime.Now.AddMonths(-3);
        //        var users = await db.tbl_UserInformation
        //            .Where(x => x.StatusId == ((int)AccountStatus.active) && x.Enable == true && x.ActiveDate < time && x.RoleId == ((int)RoleEnum.student))
        //            .Select(x=>x.UserInformationId).ToListAsync();
        //        if (users.Any())
        //        {
        //            foreach (var item in users)
        //            {
        //                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item);
        //                user.StatusId = ((int)AccountStatus.inActive);
        //                await db.SaveChangesAsync();
        //            }
        //        }
        //    }
        //}

        public class VideoCourseModel
        {
            public int VideoCourseId { get; set; }
            public string VideoCourseName { get; set; }
            /// <summary>
            /// 1 - chưa học
            /// 2 - đang học
            /// 3 - hoàn thành
            /// </summary>
            public int Status { get; set; }
            public string StatusName
            {
                get
                {
                    var result = "";
                    if (Status == 1)
                    {
                        result =  "Chưa học";
                    }
                    if (Status == 2)
                    {
                        result = "Đang học";
                    }
                    if (Status == 3)
                    {
                        result = "Hoàn thành";
                    }
                    return result;
                }
            }
        }
        public static async Task<List<VideoCourseModel>> LearningProgress(int StaffId)
        {
            using (var db = new lmsDbContext())
            {
                var result = new List<VideoCourseModel>();
                var staff = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.Enable == true && x.UserInformationId == StaffId);
                if (staff == null)
                    return result;
                var learningProgress = new List<VideoCourseModel>();
                var studyRoute = await db.tbl_StudyRoute.FirstOrDefaultAsync(x => x.Enable == true && x.DepartmentId == staff.DepartmentId);
                if (studyRoute == null)
                    return result;
                var studyRouteDetail = await db.tbl_StudyRouteDetail.Where(x => x.Enable == true && x.StudyRouteId == studyRoute.Id).OrderBy(x => x.Index).Select(x => x.VideoCourseId).ToListAsync();
                if (!studyRouteDetail.Any())
                    return result;
                foreach (var videoCourseId in studyRouteDetail)
                {
                    var videoCoursestudent = await db.tbl_VideoCourseStudent.FirstOrDefaultAsync(x => x.Enable == true && x.VideoCourseId == videoCourseId && x.UserId == StaffId);
                    if (videoCoursestudent != null)
                    {
                        var videoCourseModel = await db.tbl_VideoCourse
                            .Where(x => x.Enable == true && x.Id == videoCourseId)
                            .Select(x => new VideoCourseModel
                            {
                                VideoCourseId = videoCourseId,
                                VideoCourseName = x.Name,
                                Status = videoCoursestudent.Status,
                            })
                            .SingleOrDefaultAsync();
                        if (videoCourseModel != null)
                        {
                            learningProgress.Add(videoCourseModel);
                        }
                    }
                    else
                    {
                        var videoCourseModel = await db.tbl_VideoCourse
                            .Where(x => x.Enable == true && x.Id == videoCourseId)
                            .Select(x => new VideoCourseModel
                            {
                                VideoCourseId = videoCourseId,
                                VideoCourseName = x.Name,
                                Status = 1,
                            })
                            .SingleOrDefaultAsync();
                        if (videoCourseModel != null)
                        {
                            learningProgress.Add(videoCourseModel);
                        }
                    }
                }
                return learningProgress;
            }
        }
    }
}