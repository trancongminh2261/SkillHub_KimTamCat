using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class StandardService : DomainService
    {
        public StandardService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<tbl_Standard> Insert(StandardCreate itemModel, tbl_UserInformation userLog)
        {
            if (itemModel.Type < 1 && itemModel.Type > 2)
                throw new Exception("Loại tiêu chí không phù hợp");
            if (itemModel.Type == 2)
            {
                var videoCourse = await dbContext.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Id == itemModel.VideoCourseId);
                if (videoCourse == null)
                    throw new Exception("Không tìm thấy khóa học này");
            }
            var model = new tbl_Standard(itemModel);
            model.CreatedBy = model.ModifiedBy = userLog.FullName;
            dbContext.tbl_Standard.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }
        public async Task<tbl_Standard> Update(StandardUpdate itemModel, tbl_UserInformation userLog)
        {
            var entity = await GetById(itemModel.Id.Value);
            if (entity == null)
                throw new Exception("Không tìm thấy dữ liệu");
            entity.Name = itemModel.Name ?? entity.Name;
            entity.Point = itemModel.Point ?? entity.Point;
            entity.Type = itemModel.Type ?? entity.Type;
            entity.VideoCourseId = itemModel.VideoCourseId ?? entity.VideoCourseId;
            if (entity.Type < 1 && entity.Type > 2)
                throw new Exception("Loại tiêu chí không phù hợp");
            if (entity.Type == 2)
            {
                var videoCourse = await dbContext.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Id == entity.VideoCourseId);
                if (videoCourse == null)
                    throw new Exception("Không tìm thấy khóa học này");
            }
            else
            {
                entity.VideoCourseId = 0;
            }
            entity.ModifiedBy = userLog.FullName;
            entity.ModifiedOn = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return entity;

        }
        public async Task<tbl_Standard> GetById(int id)
        {
            var data = await dbContext.tbl_Standard.SingleOrDefaultAsync(x => x.Id == id);
            data.VideoCourseName = (await dbContext.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Id == data.VideoCourseId))?.Name;
            return data;
        }
        public async Task Delete(int id)
        {
            var data = await GetById(id);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            data.Enable = false;
            await dbContext.SaveChangesAsync();
        }
        public async Task<AppDomainResult> GetAll(StandardSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new StandardSearch();
            var pg = await dbContext.tbl_Standard.Where(x => x.Enable == true
                && (x.VideoCourseId == baseSearch.VideoCourseId || baseSearch.VideoCourseId == 0 || x.Type == 1))
                .OrderByDescending(x => x.CreatedOn).Select(x => x.Id).ToListAsync();
            if (!pg.Any())
                return new AppDomainResult() { TotalRow = 0, Data = null };
            int totalRow = pg.Count();
            pg = pg.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
            var data = (from i in pg
                        select Task.Run(() => GetById(i)).Result).ToList();
            return new AppDomainResult() { TotalRow = totalRow, Data = data };

        }
    }
}