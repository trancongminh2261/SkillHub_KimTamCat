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
    public  class TopicService : DomainService
    {
        public TopicService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<tbl_Topic> Insert(TopicCreate itemModel, tbl_UserInformation userLog)
        {
            var model = new tbl_Topic(itemModel);
            model.CreatedBy = model.ModifiedBy = userLog.FullName;
            dbContext.tbl_Topic.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }
        public async Task<tbl_Topic> Update(TopicUpdate itemModel, tbl_UserInformation userLog)
        {
            var entity = await GetById(itemModel.Id.Value);
            if (entity == null)
                throw new Exception("Không tìm thấy dữ liệu");
            entity.Name = itemModel.Name ?? entity.Name;
            entity.Code = itemModel.Code ?? entity.Code;
            itemModel.Description = itemModel.Description;
            await dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<tbl_Topic> GetById(int id)
        {
            return await dbContext.tbl_Topic.SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task Delete(int id)
        {
            var data = await GetById(id);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            data.Enable = false;
            await dbContext.SaveChangesAsync();
        }
        public async Task<AppDomainResult> GetAll(TopicSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new TopicSearch();
            var pg = dbContext.tbl_Topic.Where(x => x.Enable == true && (x.Name.Contains(baseSearch.Name) || string.IsNullOrEmpty(baseSearch.Name))).AsQueryable();
            switch (baseSearch.Sort)
            {
                case 0:
                    pg = baseSearch.SortType ? pg.OrderBy(x => x.CreatedOn) : pg.OrderByDescending(x => x.CreatedOn);
                    break;
                case 1:
                    pg = baseSearch.SortType ? pg.OrderBy(x => x.Name) : pg.OrderByDescending(x => x.Name);
                    break;
            }
            if (!pg.Any())
                return new AppDomainResult() { TotalRow = 0, Data = null };
            int totalRow = pg.Count();
            var data = await pg.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToListAsync();
            return new AppDomainResult() { TotalRow = totalRow, Data = data };
        }
    }
}