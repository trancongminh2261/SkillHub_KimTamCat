using LMS_Project.DTO.ExamPeriod;
using LMS_Project.DTO.Header;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services.Header
{
    public class HearderService : DomainService
    {
        public HearderService(lmsDbContext dbContext) : base(dbContext) { }

        public async Task<MenuCountDTO> GetMenuCount(tbl_UserInformation currentUser)
        {
            var result = new MenuCountDTO();
            var count = 0;
            if (currentUser.RoleId == (int)RoleEnum.student)
            {
                count = 0;
                //kỳ thi chưa làm
                string sql = $"Get_ExamPeriod @PageIndex = {1}," +
                    $"@PageSize = {9999}," +
                    $"@UserId = {currentUser.UserInformationId}," +
                    $"@Search = N'{""}'";
                var data = await dbContext.Database.SqlQuery<ExamPeriodDTO>(sql).ToListAsync();
                if (data.Count > 0)
                {
                    data = data.Where(x => x.EndTime > DateTime.Now).ToList();
                    foreach(var item in data)
                    {
                        var checkExamResult = await dbContext.tbl_ExamResult.AnyAsync(x => x.Enable == true && x.StudentId == currentUser.UserInformationId && x.ExamPeriodId == item.Id);
                        if (!checkExamResult)
                            count++;
                    }
                }
                result.ExamPeriodCount = count;
            }
            return result;
        }
    }
}