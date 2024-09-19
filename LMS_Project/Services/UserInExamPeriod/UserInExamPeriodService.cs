using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO.UserInExamPeriod;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<IList<UserInExamPeriodAvailable>> GetUserAvailable(int examPeriodId)
        {
            var listUserInExamPeriod = await dbContext.tbl_UserInExamPeriod.Where(x => x.ExamPeriodId == examPeriodId && x.Enable == true).Select(x => x.UserId).ToListAsync();
            var listUser = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.StatusId == 0 && !listUserInExamPeriod.Contains(x.UserInformationId)).ToListAsync();
            var result = new List<UserInExamPeriodAvailable>();
            if(listUser.Count > 0)
            {
                result = listUser.Select(x => new UserInExamPeriodAvailable
                {
                    UserId = x.UserInformationId,
                    UserCode = x.UserCode,
                    FullName = x.FullName,
                    Avatar = x.Avatar
                }).ToList();
            }
            return null;
        }


        public async Task<IList<UserInExamPeriodDTO>> Insert(UserInExamPeriodCreate request, tbl_UserInformation currentUser)
        {
            var result = new List<UserInExamPeriodDTO>();
            if (request.ListUserId.Count > 0)
            {
                var examPeriod = await dbContext.tbl_ExamPeriod.SingleOrDefaultAsync(x => x.Enable == true && x.Id == request.ExamPeriodId);
                if (examPeriod == null)
                    throw new Exception("Không tìm thấy đợt thi");
                var listUserInExamPeriod = await dbContext.tbl_UserInExamPeriod.Where(x => x.Enable == true && x.ExamPeriodId == request.ExamPeriodId).ToListAsync() ?? new List<tbl_UserInExamPeriod>();
                foreach(var item in request.ListUserId)
                {
                    var checkUserInExamPeriod = listUserInExamPeriod.Any(x => x.UserId == item);
                    if (checkUserInExamPeriod)
                        continue;
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

                    await NotificationService.Send(
                                    new tbl_Notification
                                    {
                                        UserId = item,
                                        Title = "Kỳ thi sắp diễn ra",
                                        Content = $"Bạn sắp có kỳ thi {examPeriod.Name} diễn ra từ {examPeriod.StartTime.ToString("HH:mm dd/MM/yyyy")} tới {examPeriod.EndTime.ToString("HH:mm dd/MM/yyyy")}"
                                    }, new tbl_UserInformation { FullName = "Tự động" });

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
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<UserInExamPeriodDTO>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult<UserInExamPeriodDTO> { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            return new AppDomainResult<UserInExamPeriodDTO> { TotalRow = totalRow ?? 0, Data = data };
        }
    }
}