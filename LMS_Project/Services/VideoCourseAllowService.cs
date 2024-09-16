using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO;
using LMS_Project.DTO.OptionDTO;
using LMS_Project.Enum;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class VideoCourseAllowService : DomainService
    {
        public VideoCourseAllowService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        //lấy danh sách nhân viên thuộc phòng ban và chức vụ của đợt thi
        public async Task<IList<VideoCourseAllowAvailableDTO>> GetAllowAvailable(VideoCourseAllowAvaibleSearch baseSeach)
        {
            if (baseSeach == null) baseSeach = new VideoCourseAllowAvaibleSearch();
            var videoCourseAllows = await dbContext.tbl_VideoCourseAllow.Where(x => x.Type == baseSeach.Type.ToString() && x.Enable == true && x.VideoCourseId == baseSeach.VideoCourseId).Select(x => x.ValueId).ToListAsync();
            if (baseSeach.Type == VideoCourseAllowEnum.Type.Department)
            {
                var listData = await dbContext.tbl_Department.Where(x => x.Enable == true && !videoCourseAllows.Contains(x.Id))
                    .Select(x => new VideoCourseAllowAvailableDTO
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToListAsync();

                return listData;
            }
            else
            {
                var listUser = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student && !videoCourseAllows.Contains(x.UserInformationId)).ToListAsync();

                var listData = listUser.Select(x => new VideoCourseAllowAvailableDTO
                {
                    Id = x.UserInformationId,
                    Name = $"{x.FullName} - {x.UserCode}"
                }).ToList();

                return listData;
            }
            return null;

        }
        public async Task<VideoCourseAllowDTO> GetById(int id)
        {
            var data = await dbContext.tbl_VideoCourseAllow.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
            if (data != null)
            {
                return new VideoCourseAllowDTO(data);
            }
            return null;
        }
        
        public async Task<IList<VideoCourseAllowDTO>> Insert(VideoCourseAllowCreate request, tbl_UserInformation currentUser)
        {
            using(var tran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var result = new List<VideoCourseAllowDTO>();
                    if (request.ListValueId.Count > 0)
                    {
                        if(request.Type == VideoCourseAllowEnum.Type.Department)
                        {
                            var studyRouteDetailService = new StudyRouteDetailService(dbContext);
                            foreach (var item in request.ListValueId)
                            {
                                var data = new tbl_VideoCourseAllow(request);
                                var hasData = await dbContext.tbl_VideoCourseAllow.AnyAsync(x => x.Enable == true && x.VideoCourseId == request.VideoCourseId && x.Type == request.Type.ToString() && x.ValueId == item);
                                if (hasData)
                                    throw new Exception("Phòng ban đã tồn tại không thể thêm trùng");
                                data.ValueId = item;
                                //data.Type = request.Type.ToString();
                                data.CreatedBy = data.ModifiedBy = currentUser.FullName;
                                dbContext.tbl_VideoCourseAllow.Add(data);
                                await dbContext.SaveChangesAsync();
                                result.Add(await GetById(data.Id));
                                //nếu là phòng ban thì thêm khóa học này vào lộ trình học của phòng ban đó
                                var studyRoute = await dbContext.tbl_StudyRoute.FirstOrDefaultAsync(x => x.Enable == true && x.DepartmentId == item);
                                if(studyRoute == null)
                                {
                                    //tạo lộ trình học cho phòng ban
                                    studyRoute = new tbl_StudyRoute();
                                    studyRoute.DepartmentId = item;
                                    studyRoute.LearnInOrder = false;
                                    studyRoute.Enable = true;
                                    studyRoute.CreatedBy = currentUser.FullName;
                                    studyRoute.CreatedOn = DateTime.Now;
                                    studyRoute.ModifiedBy = currentUser.FullName;
                                    studyRoute.ModifiedOn = DateTime.Now;
                                    dbContext.tbl_StudyRoute.Add(studyRoute);
                                    await dbContext.SaveChangesAsync();
                                }

                                var hasStudyRouteDetail = await dbContext.tbl_StudyRouteDetail.AnyAsync(x => x.Enable == true && x.StudyRouteId == studyRoute.Id && x.VideoCourseId == request.VideoCourseId);
                                if (!hasStudyRouteDetail)
                                {
                                    var studyRouteDetail = new tbl_StudyRouteDetail();
                                    studyRouteDetail.StudyRouteId = studyRoute.Id;
                                    studyRouteDetail.VideoCourseId = request.VideoCourseId;                                  
                                    studyRouteDetail.Index = await studyRouteDetailService.NewIndex(studyRoute.Id);
                                    studyRouteDetail.Enable = true;
                                    studyRouteDetail.CreatedBy = studyRouteDetail.ModifiedBy = currentUser.FullName;
                                    studyRouteDetail.CreatedOn = DateTime.Now;
                                    studyRouteDetail.ModifiedOn = DateTime.Now;
                                    dbContext.tbl_StudyRouteDetail.Add(studyRouteDetail);
                                    await dbContext.SaveChangesAsync();
                                }                               
                            }
                        }
                        else
                        {
                            foreach (var item in request.ListValueId)
                            {
                                var data = new tbl_VideoCourseAllow(request);
                                var hasData = await dbContext.tbl_VideoCourseAllow.AnyAsync(x => x.Enable == true && x.VideoCourseId == request.VideoCourseId && x.Type == request.Type.ToString() && x.ValueId == item);
                                if (hasData)
                                    throw new Exception("Nhân viên đã tồn tại không thể thêm trùng");
                                data.ValueId = item;
                                //data.Type = request.Type.ToString();
                                data.CreatedBy = data.ModifiedBy = currentUser.FullName;
                                dbContext.tbl_VideoCourseAllow.Add(data);
                                await dbContext.SaveChangesAsync();
                                result.Add(await GetById(data.Id));
                            }
                        }
                    }
                    tran.Commit();
                    return result;
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }                  
        }

        public async Task DeleteMulti(List<int> ListId, tbl_UserInformation currentUser)
        {
            using (var tran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (ListId.Count > 0)
                    {
                        var studyRouteDetailService = new StudyRouteDetailService(dbContext);
                        foreach (var id in ListId)
                        {
                            var data = await dbContext.tbl_VideoCourseAllow.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                            if (data == null)
                                throw new Exception("Không tìm thấy dữ liệu");
                            data.Enable = false;
                            data.ModifiedBy = currentUser.FullName;
                            data.ModifiedOn = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            if(data.Type == VideoCourseAllowEnum.Type.Department.ToString())
                            {
                                var studyRouteDetail = await dbContext.tbl_StudyRouteDetail.FirstOrDefaultAsync(x => x.Enable == true && x.VideoCourseId == data.VideoCourseId && dbContext.tbl_StudyRoute.Any(y => y.Enable == true && y.DepartmentId == data.ValueId && y.Id == x.StudyRouteId));
                                if(studyRouteDetail != null)
                                {
                                    studyRouteDetail.Enable = false;
                                    await dbContext.SaveChangesAsync();
                                    await studyRouteDetailService.ReloadIndex(studyRouteDetail.StudyRouteId);
                                }
                            }
                        }
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }          
        }
        public async Task<AppDomainResult<VideoCourseAllowDTO>> GetAll(VideoCourseAllowSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new VideoCourseAllowSearch();
            string sql = $"Get_VideoCourseAllow " +
                $"@PageIndex = {baseSearch.PageIndex}," +
                $"@PageSize = {baseSearch.PageSize}," +
                $"@VideoCourseId = {baseSearch.VideoCourseId}," +
                $"@Type = '{baseSearch.Type.ToString()}'," +
                $"@Search = N'{baseSearch.Search ?? ""}'";
            var data = await dbContext.Database.SqlQuery<VideoCourseAllowDTO>(sql).ToListAsync();
            if (!data.Any()) return new AppDomainResult<VideoCourseAllowDTO> { TotalRow = 0, Data = null };
            var totalRow = data[0].TotalRow;
            return new AppDomainResult<VideoCourseAllowDTO> { TotalRow = totalRow ?? 0, Data = data };
        }
    }
}