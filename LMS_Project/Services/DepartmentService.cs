using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace LMS_Project.Services
{
    public class DepartmentService : DomainService
    {
        public DepartmentService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public class TreeDataModel
        {
            public int departmentId { get; set; }
            public string departmentName { get; set; }
            public List<TreeDataModel> children { get; set; }
        }

        public async Task<List<TreeDataModel>> TreeViewDepartment()
        {
            var departments = await dbContext.tbl_Department.Where(x => x.Enable == true).ToListAsync(); // Lấy danh sách phòng ban từ CSDL

            // Gọi hàm đệ quy để chuyển đổi từ danh sách phòng ban sang danh sách TreeDataModel
            List<TreeDataModel> treeData = departments
                .Where(d => d.ParentDepartmentId == null && d.Enable == true) // Lọc ra các phòng ban gốc
                .Select(d => ConvertToTreeDataModel(d, departments))
                .ToList();

            return treeData;
        }

        private static TreeDataModel ConvertToTreeDataModel(tbl_Department department, List<tbl_Department> allDepartments)
        {
            TreeDataModel treeModel = new TreeDataModel
            {
                departmentId = department.Id,
                departmentName = department.Name,
                children = allDepartments
                    .Where(d => d.ParentDepartmentId == department.Id && d.Enable == true) // Lọc ra các phòng ban con
                    .Select(d => ConvertToTreeDataModel(d, allDepartments)) // Đệ quy để lấy danh sách con
                    .ToList()
            };
            return treeModel;
        }

        public async Task<AppDomainResult> GetAll(SearchOptions baseSearch)
        {
            if (baseSearch == null) baseSearch = new SearchOptions();
            string sql = $"Get_Department " +
                $"@PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<Get_Department>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            var result = data.Select(i => new tbl_Department(i)).ToList();
            return new AppDomainResult { TotalRow = totalRow, Data = result };
        }

        public async Task<AppDomainResult> GetAllRoot(SearchOptions baseSearch)
        {
            if (baseSearch == null) baseSearch = new SearchOptions();
            string sql = $"Get_DepartmentRoot " +
                $"@PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<Get_Department>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            var result = data.Select(i => new tbl_Department(i)).ToList();
            return new AppDomainResult { TotalRow = totalRow, Data = result };
        }

        public async Task<tbl_Department> GetById(int id)
        {
            try
            {
                return await dbContext.tbl_Department.SingleOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<tbl_Department> InsertRoot(RootDepartmentCreate itemModel, tbl_UserInformation user)
        {
            using (var tran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = new tbl_Department(itemModel);
                    model.ParentDepartmentId = null;
                    model.IsRoot = true;
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    dbContext.tbl_Department.Add(model);
                    await dbContext.SaveChangesAsync();
                    //tạo lộ trình học cho phòng ban
                    var studyRoute = new tbl_StudyRoute
                    {
                        DepartmentId = model.Id,
                        LearnInOrder = false,
                        Enable = true,
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now
                    };
                    dbContext.tbl_StudyRoute.Add(studyRoute);
                    await dbContext.SaveChangesAsync();
                    tran.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }
        public async Task<tbl_Department> Insert(DepartmentCreate itemModel, tbl_UserInformation user)
        {
            using (var tran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = new tbl_Department(itemModel);
                    var parentDepartment = await dbContext.tbl_Department.Where(x => x.Enable == true && x.Id == itemModel.parentDepartmentId).FirstOrDefaultAsync();
                    if (parentDepartment == null)
                        throw new Exception("Không tì thấy phòng ban");
                    model.ParentDepartmentId = itemModel.parentDepartmentId;
                    model.IsRoot = false;
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    dbContext.tbl_Department.Add(model);
                    await dbContext.SaveChangesAsync();
                    //tạo lộ trình học cho phòng ban
                    var studyRoute = new tbl_StudyRoute
                    {
                        DepartmentId = model.Id,
                        LearnInOrder = false,
                        Enable = true,
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now
                    };
                    dbContext.tbl_StudyRoute.Add(studyRoute);
                    await dbContext.SaveChangesAsync();
                    tran.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        //kiểm tra xem isRoot = true hay false
        //nếu bằng true thì set parentDepartmentId = null
        //nếu bằng false thì đệ quy kiểm tra xem parentDepartmentId mới có phải con của nó không
        public async Task<tbl_Department> Update(DepartmentUpdate itemModel, tbl_UserInformation user)
        {
            try
            {
                var entity = await dbContext.tbl_Department.SingleOrDefaultAsync(x => x.Id == itemModel.Id && x.Enable == true);
                if (entity == null)
                    throw new Exception("Không tìm thấy phòng ban");
                entity.Name = itemModel.Name ?? entity.Name;
                entity.IsRoot = itemModel.IsRoot ?? entity.IsRoot;
                entity.ModifiedBy = user.FullName;
                entity.ModifiedOn = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //xóa phòng ban thì xóa các phòng ban con
        //kiểm tra các nhân viên thuộc phòng ban đó set phòng ban == null
        public async Task Delete(int id)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    await DeleteDepartmentAndChildren(id);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi xảy ra, hủy bỏ transaction
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private async Task DeleteDepartmentAndChildren(int id)
        {
            var entity = await dbContext.tbl_Department.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (entity == null)
                throw new Exception("Không tìm thấy phòng ban");

            entity.Enable = false;
            await dbContext.SaveChangesAsync();

            //sau khi xóa phòng ban tiến hành set phòng ban của nhân viên == null
            var ListUser = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.DepartmentId == id).ToListAsync();
            if (ListUser.Count > 0)
            {
                foreach (var item in ListUser)
                {
                    item.DepartmentId = null;
                }
                await dbContext.SaveChangesAsync();
            }

            // Xóa các phòng ban con
            var childDepartments = await dbContext.tbl_Department.Where(x => x.ParentDepartmentId == id && x.Enable == true).ToListAsync();
            if (childDepartments.Any())
            {
                foreach (var childDepartment in childDepartments)
                {
                    await DeleteDepartmentAndChildren(childDepartment.Id);
                }
            }
        }
    }
}