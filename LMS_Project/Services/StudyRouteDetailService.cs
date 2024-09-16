using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
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
    public class StudyRouteDetailService : DomainService
    {
        public StudyRouteDetailService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        //code đánh index ở hàm Insert, Update Index, Delete đang là code cũ nhưng đã chạy rồi nên không điều chỉnh
        //code mới này viết để dùng cho những hàm khác
        public async Task<int> NewIndex(int studyRouteId)
        {
            var lastIndex = await dbContext.tbl_StudyRouteDetail
                .Where(x => x.StudyRouteId == studyRouteId && x.Enable == true)
                .OrderByDescending(x => x.Index)
                .FirstOrDefaultAsync();
            if (lastIndex == null)
                return 1;
            return lastIndex.Index + 1;
        }
        public async Task ReloadIndex(int studyRouteId)
        {
            var details = await dbContext.tbl_StudyRouteDetail
                .Where(x => x.StudyRouteId == studyRouteId && x.Enable == true)
                .OrderBy(x => x.Index)
                .ToListAsync();
            if (details.Any())
            {
                int index = 1;
                foreach (var item in details)
                {
                    item.Index = index;
                    index++;
                }
                await dbContext.SaveChangesAsync();
            }
        }
        public class VideoCourseModel
        {
            public int Id { get; set; }
            public string Name { get; set; }

        }
        public async Task<List<VideoCourseModel>> GetListVideoCourse(int StudyRouteId, string tags)
        {
            var tagsArray = tags.Split(',');
            var listTag = tagsArray.ToList();
            var videoCourses = new List<tbl_VideoCourse>();
            if (listTag.Any() && tags != null && tags != "")
            {
                videoCourses = await dbContext.tbl_VideoCourse
                .Where(x => x.Enable == true && listTag.Any(tag => x.Tags.Contains(tag)))
                .ToListAsync();
            }
            else
            {
                videoCourses = await dbContext.tbl_VideoCourse
                .Where(x => x.Enable == true)
                .ToListAsync();
            }

            var videoCourseInStudyRoute = await dbContext.tbl_StudyRouteDetail.Where(x => x.Enable == true && x.StudyRouteId == StudyRouteId).Select(x => x.VideoCourseId).ToListAsync();
            if (videoCourseInStudyRoute != null)
            {
                videoCourses = videoCourses.Where(x => !videoCourseInStudyRoute.Contains(x.Id)).ToList();
            }
            var ListVideoCourseModel = new List<VideoCourseModel>();
            //lấy ra danh sách khóa học trong lộ trình theo phòng ban
            foreach (var item in videoCourses)
            {
                var model = new VideoCourseModel
                {
                    Id = item.Id,
                    Name = item.Name
                };
                ListVideoCourseModel.Add(model);
            }
            return ListVideoCourseModel;
        }

        public async Task<List<tbl_StudyRouteDetail>> GetByStudyRouteId(int StudyRouteId)
        {
            try
            {
                var data = await (
                    from s in dbContext.tbl_StudyRouteDetail
                    join v in dbContext.tbl_VideoCourse on s.VideoCourseId equals v.Id
                    where s.StudyRouteId == StudyRouteId && s.Enable == true && v.Enable == true
                    select new
                    {
                        s.Id,
                        s.StudyRouteId,
                        s.VideoCourseId,
                        s.Index,
                        Name = v.Name,
                        s.Enable,
                        s.CreatedBy,
                        s.CreatedOn,
                        s.ModifiedBy,
                        s.ModifiedOn
                    }
                ).ToListAsync();

                // Tạo danh sách tbl_StudyRouteDetail từ kết quả
                var studyRouteDetails = data.Select(result => new tbl_StudyRouteDetail
                {
                    Id = result.Id,
                    StudyRouteId = result.StudyRouteId,
                    VideoCourseId = result.VideoCourseId,
                    Index = result.Index,
                    Name = result.Name,
                    Enable = result.Enable,
                    CreatedBy = result.CreatedBy,
                    CreatedOn = result.CreatedOn,
                    ModifiedBy = result.ModifiedBy,
                    ModifiedOn = result.ModifiedOn
                }).OrderBy(x=>x.Index).ToList();

                return studyRouteDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<tbl_StudyRouteDetail> Insert(StudyRouteDetailCreate itemModel, tbl_UserInformation user)
        {
            try
            {
                var model = new tbl_StudyRouteDetail(itemModel);
                var studyRoute = await dbContext.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Enable == true && x.Id == itemModel.StudyRouteId);
                if (studyRoute == null)
                    throw new Exception("Không tìm thấy lộ trình học");
                var checkVideoCourse = await dbContext.tbl_StudyRouteDetail.SingleOrDefaultAsync(x => x.Enable == true && x.VideoCourseId == itemModel.VideoCourseId && x.StudyRouteId == itemModel.StudyRouteId);
                if (checkVideoCourse != null)
                    throw new Exception("Đã tồn tại khóa này");
                var maxIndex = 0;
                var query = dbContext.tbl_StudyRouteDetail
                    .Where(x => x.Enable == true && x.StudyRouteId == itemModel.StudyRouteId);
                if (await query.AnyAsync())
                {
                    maxIndex = await query.MaxAsync(x => x.Index);
                }
                model.Index = maxIndex + 1;
                model.CreatedBy = model.ModifiedBy = user.FullName;
                dbContext.tbl_StudyRouteDetail.Add(model);
                await dbContext.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<tbl_StudyRouteDetail>> InsertMulti(MultiStudyRouteDetailCreate itemModel, tbl_UserInformation user)
        {
            try
            {
                var studyRoute = await dbContext.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Enable == true && x.Id == itemModel.StudyRouteId);
                if (studyRoute == null)
                    throw new Exception("Vui lòng chọn lộ trình học");
                List<tbl_StudyRouteDetail> listStudyRouteDetail = new List<tbl_StudyRouteDetail>();
                foreach (var item in itemModel.ListVideoCourseId)
                {
                    var videoCourse = await dbContext.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Enable == true && x.Id == item);
                    if (videoCourse == null)
                        throw new Exception("Vui lòng chọn khóa học video");
                    var checkVideoCourse = await dbContext.tbl_StudyRouteDetail.SingleOrDefaultAsync(x => x.Enable == true && x.VideoCourseId == item && x.StudyRouteId == itemModel.StudyRouteId);
                    if (checkVideoCourse != null)
                        throw new Exception("Đã tồn tại khóa học này");
                    var maxIndex = 0;
                    var query = dbContext.tbl_StudyRouteDetail
                        .Where(x => x.Enable == true && x.StudyRouteId == itemModel.StudyRouteId);
                    if (await query.AnyAsync())
                    {
                        maxIndex = await query.MaxAsync(x => x.Index);
                    }
                    var model = new tbl_StudyRouteDetail
                    {
                        StudyRouteId = itemModel.StudyRouteId,
                        VideoCourseId = item,
                        Index = maxIndex + 1,
                        Enable = true,
                        CreatedBy = user.FullName,
                        ModifiedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    };
                    dbContext.tbl_StudyRouteDetail.Add(model);
                    await dbContext.SaveChangesAsync();
                    listStudyRouteDetail.Add(model);
                }
                return listStudyRouteDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<tbl_StudyRouteDetail>> UpdateIndex(List<IndexUpdate> request, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                List<tbl_StudyRouteDetail> listStudyRouteDetail = new List<tbl_StudyRouteDetail>();
                if (!request.Any())
                    throw new Exception("Không tìm thấy dữ liệu");
                foreach (var item in request)
                {
                    var StudyRouteDetail = await db.tbl_StudyRouteDetail.SingleOrDefaultAsync(x => x.Id == item.Id && x.Enable == true);
                    if (StudyRouteDetail != null)
                    {
                        StudyRouteDetail.Index = item.Index ?? StudyRouteDetail.Index;
                        StudyRouteDetail.ModifiedBy = user.FullName;
                        StudyRouteDetail.ModifiedOn = DateTime.Now;
                        listStudyRouteDetail.Add(StudyRouteDetail);
                    }
                    await db.SaveChangesAsync();
                }
                return listStudyRouteDetail;
            }
        }
        public async Task Delete(int id)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var entity = await dbContext.tbl_StudyRouteDetail.SingleOrDefaultAsync(x => x.Id == id && x.Enable == true);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Enable = false;
                    await dbContext.SaveChangesAsync();
                    //xóa thì dịch chuyển index của các khóa học sau nó giảm 1 bậc
                    var listStudyRouteDetail = await dbContext.tbl_StudyRouteDetail.Where(x => x.Enable == true && x.StudyRouteId == entity.StudyRouteId).ToListAsync();
                    foreach (var item in listStudyRouteDetail)
                    {
                        if (item.Index > entity.Index)
                        {
                            item.Index--;
                            await dbContext.SaveChangesAsync();
                        }
                    }
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