using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project.Services
{
    public class StudyRouteService : DomainService
    {
        public StudyRouteService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<AppDomainResult> GetAll(StudyRouteSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new StudyRouteSearch();
            string sql = $"Get_StudyRoute " +
                $"@PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@DepartmentId = {baseSearch.DepartmentId ?? 0}," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<Get_StudyRoute>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            var result = data.Select(i => new tbl_StudyRoute(i)).ToList();
            return new AppDomainResult { TotalRow = totalRow, Data = result };
        }

        public async Task<tbl_StudyRoute> GetById(int id)
        {
            var data = await dbContext.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            return data;
        }

        public async Task<tbl_StudyRoute> Insert(StudyRouteCreate itemModel, tbl_UserInformation user)
        {
            var model = new tbl_StudyRoute(itemModel);
            var department = await dbContext.tbl_Department.SingleOrDefaultAsync(x => x.Enable == true && x.Id == itemModel.DepartmentId);
            if (department == null)
                throw new Exception("Không tìm thấy phòng ban");
            // 1 phòng ban 1 lộ trình học
            var checkDepartment = await dbContext.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Enable == true && x.DepartmentId == itemModel.DepartmentId);
            if (checkDepartment != null)
                throw new Exception("Phòng ban này đã có lộ trình học");
            model.CreatedBy = model.ModifiedBy = user.FullName;
            dbContext.tbl_StudyRoute.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task<tbl_StudyRoute> Update(StudyCourseUpdate itemModel, tbl_UserInformation user)
        {
            var entity = await dbContext.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Id == itemModel.Id && x.Enable == true);
            if (entity == null)
            {
                throw new Exception("Không tìm thấy lộ trình học");
            }
            entity.LearnInOrder = itemModel.LearnInOrder ?? entity.LearnInOrder;
            entity.ModifiedBy = user.FullName;
            entity.ModifiedOn = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(int id)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var entity = await dbContext.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                    if (entity == null)
                        throw new Exception("Không tìm thấy lộ trình học");
                    //xóa lộ trình thì xóa luôn chi tiết trong lộ trình
                    var listDetail = await dbContext.tbl_StudyRouteDetail.Where(x => x.Enable == true && x.StudyRouteId == id).ToListAsync();
                    foreach (var item in listDetail)
                    {
                        item.Enable = false;
                        await dbContext.SaveChangesAsync();
                    }
                    entity.Enable = false;
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}