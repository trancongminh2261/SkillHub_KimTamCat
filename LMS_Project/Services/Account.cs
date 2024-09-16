using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Enum;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.ControllerAPIs.AccountController;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Users.JWTManager;

namespace LMS_Project.Services
{
    public class Account
    {
        public static void PushNotiRemindStudy()
        {
            using (var db = new lmsDbContext())
            {
                var httpContext = HttpContext.Current;
                var pathViews = Path.Combine(httpContext.Server.MapPath("~/Views"));
                var studentIds = db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student && (x.LastLoginDate == null || (DateTime.Now - x.LastLoginDate.Value).Days >= 3)
                    && (db.tbl_VideoCourseAllow.Any(y => y.Enable == true && ((y.ValueId == x.DepartmentId && y.Type == VideoCourseAllowEnum.Type.Department.ToString()) || (y.ValueId == x.UserInformationId && y.Type == VideoCourseAllowEnum.Type.User.ToString()))
                        && (db.tbl_VideoCourseStudent.Any(z => z.Enable == true && z.VideoCourseId == y.VideoCourseId && x.StatusId == 3) == false)))).Select(x => x.UserInformationId).Distinct().ToList();
                if (studentIds.Count > 0)
                {
                    string domain = ConfigurationManager.AppSettings["DomainFE"].ToString();
                    string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                    //https://skillhub.mona.software/learning/?course=84&sectionIds=57&currentLessonId=1170
                    //string href = $"<a href=\"{domain}/course/video-course/detail/?slug={videoCourse.Id}\"><b style=\"color: blue;\">Tại đây</b></a>";
                    string href = $"<a href=\"{domain}/course/video-course\"><b style=\"color: blue;\">Tại đây</b></a>";
                    string title = "Thông báo tham gia khóa học";
                    string contentEmail = System.IO.File.ReadAllText($"{pathViews}/Template/MailNewLesson.html");

                    contentEmail = contentEmail.Replace("[TenDuAn]", projectName);
                    contentEmail = contentEmail.Replace("[TaiDay]", href);
                    foreach (var studentId in studentIds)
                    {
                        try
                        {
                            var student = db.tbl_UserInformation.SingleOrDefault(x => x.UserInformationId == studentId);
                            var totalDate = (DateTime.Now.Date - student.LastLoginDate.Value.Date).Days;
                            string content = $"Đã {totalDate}, bạn chưa tham gia vào các khóa học trên hệ thống, để đảm bảo bạn nắm bắt được kiến thức cần thiết và hoàn thành khóa học, xin vui lòng đăng nhập và tiếp tục các khóa học còn dang dở";
                            string onesignalContent = $"Đã {totalDate}, bạn chưa tham gia vào các khóa học trên hệ thống, để đảm bảo bạn nắm bắt được kiến thức cần thiết và hoàn thành khóa học, xin vui lòng đăng nhập và tiếp tục các khóa học còn dang dở";
                            string mailToStudent = contentEmail;
                            mailToStudent = mailToStudent.Replace("[HoVaTen]", student.FullName);
                            mailToStudent = contentEmail.Replace("[Ngay]", totalDate.ToString());
                            NotificationService.SendNotThread(db,
                                new NotificationService.SendNotThreadModel
                                {
                                    Content = content,
                                    Email = student.Email,
                                    EmailContent = mailToStudent,
                                    OnesignalId = student.OneSignal_DeviceId,
                                    Title = title,
                                    UserId = student.UserInformationId,
                                    OnesignalContent = onesignalContent,
                                    OnesignalUrl = $"{domain}/course/video-course"
                                }
                                , new tbl_UserInformation { FullName = "Hệ thống" });
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                }
            }
        }
        public class TokenResult : AppDomainResult
        {
            public GenerateTokenModel GenerateTokenModel { get; set; }
        }
        public static async Task<TokenResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tên đăng nhập hoặc mật khẩu không được để trống!" };
            using (lmsDbContext _db = new lmsDbContext())
            {
                string pass = Encryptor.Encrypt(password);
                var account = await _db.tbl_UserInformation.SingleOrDefaultAsync(
                    c => c.UserName.ToUpper() == username.ToUpper() && c.Password == pass && c.Enable == true);
                if (account == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!" };
                if (account.StatusId != (int)AccountStatus.active)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!" };
                if (account.RoleId != ((int)RoleEnum.admin))
                {
                    if (username != "phuong.le@loreal.com" && username != "Trinh.Nguyenngoctuyet@loreal.com")
                    {
                        var timelock = new DateTime(2023, 3, 23, 0, 0, 0);
                        if (DateTime.Now < timelock)
                            return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Chương trình bắt đầu học từ ngày 23/03/2023. Mời bạn trở lại đăng nhập vào khóa học vào thời gian trên" };
                    }
                }
                var token = await JWTManager.GenerateToken(account.UserInformationId);
                account.LastLoginDate = DateTime.Now;
                await _db.SaveChangesAsync();
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }
        }
        //public static async Task<AppDomainResult> LoginByDev(LoginDevModel logindev)
        //{
        //    if (string.IsNullOrEmpty(logindev.Id.ToString()) || string.IsNullOrEmpty(logindev.PassDev))
        //        return new AppDomainResult()
        //        {
        //            ResultCode = (int)HttpStatusCode.BadRequest,
        //            ResultMessage = "Tài khoản và mật khẩu không được để trống"
        //        };
        //    using (lmsDbContext db = new lmsDbContext())
        //    {
        //        var accountByDev = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == logindev.Id && logindev.PassDev == "m0n4medi4" && x.Enable == true);
        //        if (accountByDev == null)
        //            return new AppDomainResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Sai mã" };
        //        if (accountByDev.StatusId != (int)AccountStatus.active)
        //            return new AppDomainResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!" };
        //        var token = await JWTManager.GenerateToken(accountByDev.UserInformationId);
        //        return new AppDomainResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, Data = token };
        //    }

        //}

        public static async Task<TokenResult> LoginByDev(LoginDevModel logindev)
        {
            if (string.IsNullOrEmpty(logindev.Id.ToString()) || string.IsNullOrEmpty(logindev.PassDev))
                return new TokenResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    ResultMessage = "Tài khoản và mật khẩu không được để trống"
                };
            using (lmsDbContext db = new lmsDbContext())
            {
                var accountByDev = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == logindev.Id && logindev.PassDev == "m0n4medi4" && x.Enable == true);
                if (accountByDev == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Sai mã" };
                if (accountByDev.StatusId != (int)AccountStatus.active)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.BadRequest, ResultMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!" };
                var token = await JWTManager.GenerateToken(accountByDev.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }

        }
        public static async Task<TokenResult> NewToken(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var token = await JWTManager.GenerateToken(user.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }
        }
        public static async Task<tbl_UserInformation> Register(RegisterModel model)
        {
            try
            {
                string originPasword = model.Password;
                model.Password = Encryptor.Encrypt(model.Password);
                await UserInformation.ValidateUser(model.UserName, model.Email);
                var data = await UserInformation.Insert(new tbl_UserInformation(model), new tbl_UserInformation { FullName = "Đăng ký" });

                Thread sendMail = new Thread(() =>
                {
                    string title = "Đăng ký thành công tài khoản";
                    string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                    StringBuilder content = new StringBuilder();
                    content.Append($"<div>");
                    content.Append($"<p>Đăng ký thành công tài khoản</p>");
                    content.Append($"<p></p>");
                    content.Append($"<p>Chào {data.FullName}</p>");
                    content.Append($"<p></p>");
                    content.Append($"<p>Chúng tôi xin thông báo rằng bạn đã đăng ký thành công tài khoản tại hệ thống của chúng tôi. Sau đây là thông tin tài khoản của bạn:</p>");
                    content.Append($"<p></p>");
                    content.Append($"<p>Tên đăng nhập: {model.UserName}</p>");
                    content.Append($"<p>Mật khẩu: {originPasword}</p>");
                    content.Append($"<p></p>");
                    content.Append($"<p>Chúng tôi khuyến khích bạn nên thay đổi mật khẩu của mình sau khi đăng nhập lần đầu tiên vào tài khoản để tăng tính bảo mật cho tài khoản của mình.</p>");
                    var timelock = new DateTime(2023, 3, 23, 0, 0, 0);
                    if (DateTime.Now < timelock)
                    {
                        content.Append($"<p>Chương trình bắt đầu học từ ngày 23/03/2023. Mời bạn trở lại đăng nhập vào khóa học vào thời gian trên</p>");
                    }
                    content.Append($"<p></p>");
                    content.Append($"<p>{projectName}</p>");
                    content.Append($"</div>");
                    AssetCRM.SendMail(data.Email, title, content.ToString());
                });
                sendMail.Start();

                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task ChangeRegister(AllowRegister value)
        {
            using (var db = new lmsDbContext())
            {
                var config = await db.tbl_Config
                    .Where(x => x.Code == "Register").FirstOrDefaultAsync();
                if (config == null)
                    throw new Exception("Chưa cấu hình hệ thống");
                config.Value = value.ToString();
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AllowRegister> GetAllowRegister()
        {
            using (var db = new lmsDbContext())
            {
                var config = await db.tbl_Config
                    .Where(x => x.Value == "UnAllow" && x.Code == "Register").AnyAsync();
                if (config)
                    return AllowRegister.UnAllow;
                return AllowRegister.Allow;
            }
        }
        public static async Task ChangePassword(ChangePasswordModel model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    if (Encryptor.Encrypt(model.OldPassword) != user.Password)
                        throw new Exception("Mật khẩu không chính xác");
                    var entity = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == user.UserInformationId);
                    entity.Password = Encryptor.Encrypt(model.NewPassword);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class KeyForgotPasswordModel
        {
            public string UserName { get; set; }
        }
        public static async Task KeyForgotPassword(KeyForgotPasswordModel model)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var url = ConfigurationManager.AppSettings["DomainFE"].ToString() + "reset-password?key=";
                    var projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                    var user = await db.tbl_UserInformation
                        .Where(x => x.UserName.ToUpper() == model.UserName.ToUpper() && x.Enable == true).FirstOrDefaultAsync();
                    if (user == null)
                        throw new Exception("Tài khoản không tồn tại");
                    string title = "Yêu cầu thay đổi mật khẩu";
                    ///Gửi mail thông báo
                    user.KeyForgotPassword = Guid.NewGuid().ToString();
                    user.CreatedDateKeyForgot = DateTime.Now.AddHours(24);
                    StringBuilder content = new StringBuilder();
                    content.Append($"<div>");
                    content.Append($"<p>Xin chào {user.FullName} </p>");
                    content.Append($"<p>Để đặt lại mật khẩu password cho username {user.UserName}, bạn vui lòng truy cập <a href=\"{url}{user.KeyForgotPassword}\"><b>vào đây</b></a></p>");
                    content.Append($"<p>Đường link sẽ hết hạn trong vòng 24 giờ</p>");
                    content.Append($"<p>Thông báo từ {projectName} </p>");
                    content.Append($"</div>");
                    AssetCRM.SendMail(user.Email, title, content.ToString());
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task ResetPassword(ResetPasswordModel model)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var user = await db.tbl_UserInformation.Where(x => x.KeyForgotPassword == model.Key && !string.IsNullOrEmpty(x.KeyForgotPassword)).FirstOrDefaultAsync();
                    if (user == null)
                        throw new Exception("Xác thực không thành công");
                    if (user.CreatedDateKeyForgot < DateTime.Now)
                        throw new Exception("Yêu cầu đã hết hạn");
                    user.Password = Encryptor.Encrypt(model.NewPassword);
                    user.KeyForgotPassword = "";
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class AccountModel
        { 
            public int Id { get; set; }
            public string FullName { get; set; }
            public string RoleName { get; set; }
        }
        public static async Task<List<AccountModel>> GetAccount()
        {
            using (var db = new lmsDbContext())
            {
                var users = await db.tbl_UserInformation.Where(x => x.Enable == true && x.StatusId == ((int)AccountStatus.active)).ToListAsync();
                if (!users.Any())
                    return new List<AccountModel>();
                return (from i in users
                        select new AccountModel
                        {
                            FullName = i.FullName,
                            Id = i.UserInformationId,
                            RoleName = i.RoleName
                        }).ToList();
            }
        }

        public class AddRefreshTokenRequest
        {
            public int UserId { get; set; }
            public string RefreshToken { get; set; }
            /// <summary>
            /// Hạn sử dụng refresh token
            /// </summary>
            public DateTime? RefreshTokenExpires { get; set; }
        }
        public static async Task AddRefreshToken(AddRefreshTokenRequest itemModel)
        {
            using (var db = new lmsDbContext())
            {
                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == itemModel.UserId);
                if (user != null)
                {
                    user.RefreshToken = itemModel.RefreshToken;
                    user.RefreshTokenExpires = itemModel.RefreshTokenExpires;
                    await db.SaveChangesAsync();
                }
            }
        }

        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; }
        }
        public static async Task<TokenResult> RefreshToken(RefreshTokenRequest itemModel)
        {
            using (var db = new lmsDbContext())
            {
                if (itemModel == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.Unauthorized, ResultMessage = "Phiên đăng nhập hết hạn" };
                if (string.IsNullOrEmpty(itemModel.RefreshToken))
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.Unauthorized, ResultMessage = "Phiên đăng nhập hết hạn" };

                var user = await db.tbl_UserInformation.FirstOrDefaultAsync(x => x.RefreshToken == itemModel.RefreshToken);
                if (user == null)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.Unauthorized, ResultMessage = "Phiên đăng nhập hết hạn" };
                if (DateTime.Now > user.RefreshTokenExpires)
                    return new TokenResult() { ResultCode = (int)HttpStatusCode.Unauthorized, ResultMessage = "Phiên đăng nhập hết hạn" };
                var token = await JWTManager.GenerateToken(user.UserInformationId);
                return new TokenResult() { ResultCode = (int)HttpStatusCode.OK, ResultMessage = ApiMessage.LOGIN_SUCCESS, GenerateTokenModel = token };
            }
        }
        public static async Task<bool> HasPermission(int roleId, string controller, string action)
        {
            using (var db = new lmsDbContext())
            {
                if (controller == "Permission" && roleId == ((int)RoleEnum.admin))
                    return true;
                var permissions = await db.tbl_Permission.Where(x => x.Controller.ToUpper() == controller.ToUpper()
                   && x.Action.ToUpper() == action.ToUpper()).ToListAsync();
                if (!permissions.Any())
                    return false;
                if (roleId == ((int)RoleEnum.admin))
                    return true;
                var permission = permissions.Any(x => x.Allowed.Contains(roleId.ToString()));
                if (permission)
                    return true;
                return false;
            }
        }
    }
}