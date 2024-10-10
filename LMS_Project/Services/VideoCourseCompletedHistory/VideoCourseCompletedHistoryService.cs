using LMS_Project.Areas.Models;
using LMS_Project.DTO.VideoCourseCompletedHistory;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project.Services.VideoCourseCompletedHistory
{
    public class VideoCourseCompletedHistoryService : DomainService
    {
        public VideoCourseCompletedHistoryService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<AppDomainResult<VideoCourseCompletedHistoryDTO>> GetAll(VideoCourseCompletedHistorySearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new VideoCourseCompletedHistorySearch();
            string sql = $"Get_VideoCourseCompletedHistory " +
                $"@PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@UserId = {baseSearch.UserId}," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<VideoCourseCompletedHistoryDTO>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult<VideoCourseCompletedHistoryDTO> { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            return new AppDomainResult<VideoCourseCompletedHistoryDTO> { TotalRow = totalRow ?? 0, Data = data };
        }
    }
}