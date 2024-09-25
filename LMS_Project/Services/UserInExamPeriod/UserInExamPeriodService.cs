using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO.UserInExamPeriod;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services.UserInUserInExamPeriod
{
    public class UserInExamPeriodService : DomainService
    {
        public UserInExamPeriodService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<UserInExamPeriodDTO> GetById(int id)
        {
            var data = await dbContext.tbl_UserInExamPeriod.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data != null)
            {
                return new UserInExamPeriodDTO(data);
            }
            return null;
        }

        public async Task<IList<UserInExamPeriodAvailable>> GetUserAvailable(UserInExamPeriodAvailableSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new UserInExamPeriodAvailableSearch();
            var listUserInExamPeriod = await dbContext.tbl_UserInExamPeriod.Where(x => x.ExamPeriodId == baseSearch.ExamPeriodId && x.Enable == true).Select(x => x.UserId).ToListAsync();
            var listUser = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.StatusId == 0 && x.RoleId == (int)RoleEnum.student && !listUserInExamPeriod.Contains(x.UserInformationId)).ToListAsync();
            
            var result = new List<UserInExamPeriodAvailable>();
            if(listUser.Count > 0)
            {
                if (baseSearch.DepartmentId == null || baseSearch.DepartmentId == 0)
                    listUser = listUser.Where(x => x.DepartmentId == baseSearch.DepartmentId).ToList();
                result = listUser.Select(x => new UserInExamPeriodAvailable
                {
                    UserId = x.UserInformationId,
                    UserCode = x.UserCode,
                    FullName = x.FullName,
                    Avatar = x.Avatar
                }).ToList();
                return result;
            }
            return null;
        }


        public async Task<IList<UserInExamPeriodDTO>> Insert(UserInExamPeriodCreate request, tbl_UserInformation currentUser)
        {
            var httpContext = HttpContext.Current;
            var pathViews = Path.Combine(httpContext.Server.MapPath("~/Views"));
            var result = new List<UserInExamPeriodDTO>();
            if (request.ListUserId.Count > 0)
            {
                var examPeriod = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Enable == true && x.Id == request.ExamPeriodId);
                if (examPeriod == null)
                    throw new Exception("Không tìm thấy đợt thi");
                var listUserInExamPeriod = await dbContext.tbl_UserInExamPeriod.Where(x => x.Enable == true && x.ExamPeriodId == request.ExamPeriodId).ToListAsync() ?? new List<tbl_UserInExamPeriod>();
                string domain = ConfigurationManager.AppSettings["DomainFE"].ToString();
                string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                string href = $"<a href=\"{domain}/testing-exam/\"><b style=\"color: blue;\">Tại đây</b></a>";
                string title = "Thông báo kỳ thi mới";
                string content = $"Bạn sắp có kỳ thi {examPeriod.Name} diễn ra từ {examPeriod.StartTime.ToString("HH:mm dd/MM/yyyy")} tới {examPeriod.EndTime.ToString("HH:mm dd/MM/yyyy")}, vui lòng truy cập {href} để xem thông tin";
                string contentEmail = System.IO.File.ReadAllText($"{pathViews}/Template/MailNewExamPeriod.html");

                contentEmail = contentEmail.Replace("[ProjectName]", projectName);
                contentEmail = contentEmail.Replace("[ExamPeriodName]", examPeriod.Name);
                contentEmail = contentEmail.Replace("[StartTime]", examPeriod.StartTime.ToString("HH:mm dd/MM/yyyy"));
                contentEmail = contentEmail.Replace("[EndTime]", examPeriod.EndTime.ToString("HH:mm dd/MM/yyyy"));
                contentEmail = contentEmail.Replace("[Url]", $"{domain}/testing-exam/");

                var listStudent = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync() ?? new List<tbl_UserInformation>();

                foreach (var item in request.ListUserId)
                {
                    var checkUserInExamPeriod = listUserInExamPeriod.Any(x => x.UserId == item);
                    if (checkUserInExamPeriod)
                        continue;
                    var student = listStudent.FirstOrDefault(x => x.UserInformationId == item) ?? new tbl_UserInformation();

                    var userInExamPeriod = new tbl_UserInExamPeriod();
                    userInExamPeriod.ExamPeriodId = request.ExamPeriodId;
                    userInExamPeriod.UserId = item;
                    userInExamPeriod.Enable = true;
                    userInExamPeriod.CreatedBy = currentUser.FullName;
                    userInExamPeriod.ModifiedBy = currentUser.FullName;
                    userInExamPeriod.CreatedOn = DateTime.Now;
                    userInExamPeriod.ModifiedOn = DateTime.Now;
                    dbContext.tbl_UserInExamPeriod.Add(userInExamPeriod);
                    await dbContext.SaveChangesAsync();

                    string mailToStudent = contentEmail;
                    mailToStudent = mailToStudent.Replace("[FullName]", student.FullName);
                    NotificationService.SendNotThread(dbContext,
                        new NotificationService.SendNotThreadModel
                        {
                            Content = content,
                            Email = student.Email,
                            EmailContent = mailToStudent,
                            OnesignalId = student.OneSignal_DeviceId,
                            Title = title,
                            UserId = student.UserInformationId,
                            OnesignalContent = "",
                            OnesignalUrl = ""
                        }
                        , currentUser);

                    /*await NotificationService.Send(
                                    new tbl_Notification
                                    {
                                        UserId = item,
                                        Title = "Kỳ thi sắp diễn ra",
                                        Content = $"Bạn sắp có kỳ thi {examPeriod.Name} diễn ra từ {examPeriod.StartTime.ToString("HH:mm dd/MM/yyyy")} tới {examPeriod.EndTime.ToString("HH:mm dd/MM/yyyy")}"
                                    }, new tbl_UserInformation { FullName = "Tự động" }, "");*/

                    result.Add(await GetById(userInExamPeriod.Id));
                }                
            }           
            return result;
        }
        public async Task Delete(int id, tbl_UserInformation currentUser)
        {
            var data = await dbContext.tbl_UserInExamPeriod.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            data.Enable = false;
            data.ModifiedBy = currentUser.FullName;
            data.ModifiedOn = DateTime.Now;
            await dbContext.SaveChangesAsync();
        }
        public async Task<AppDomainResult<UserInExamPeriodDTO>> GetAll(UserInExamPeriodSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new UserInExamPeriodSearch();
            string sql = $"Get_UserInExamPeriod @PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@ExamPeriodId = {baseSearch.ExamPeriodId}," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<UserInExamPeriodDTO>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult<UserInExamPeriodDTO> { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            return new AppDomainResult<UserInExamPeriodDTO> { TotalRow = totalRow ?? 0, Data = data };
        }
    }
}