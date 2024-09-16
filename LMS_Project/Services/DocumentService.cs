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
    public  class DocumentService : DomainService
    {
        public DocumentService(lmsDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<tbl_Document> Insert(DocumentCreate itemModel, tbl_UserInformation userLog)
        {
            var model = new tbl_Document(itemModel);
            model.CreatedBy = model.ModifiedBy = userLog.FullName;
            dbContext.tbl_Document.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }
        public async Task<tbl_Document> Update(DocumentUpdate itemModel, tbl_UserInformation userLog)
        {
            var entity = await GetById(itemModel.Id.Value);
            if (entity == null)
                throw new Exception("Không tìm thấy dữ liệu");
            if (entity.TopicId == null)
                throw new Exception("Không tìm thấy chủ đề");
            entity.TopicId = itemModel.TopicId ?? entity.TopicId;
            entity.FileType = itemModel.FileType ?? entity.FileType;
            entity.FileName = itemModel.FileName ?? entity.FileName;
            entity.AbsolutePath = itemModel.AbsolutePath ?? entity.AbsolutePath;
            await dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<tbl_Document> GetById(int id)
        {
            return await dbContext.tbl_Document.SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task Delete(int id)
        {
            var data = await GetById(id);
            if (data == null)
                throw new Exception("Không tìm thấy dữ liệu");
            data.Enable = false;
            await dbContext.SaveChangesAsync();
        }
        public async Task<AppDomainResult> GetAll(DocumentSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new DocumentSearch();
            var pg = dbContext.tbl_Document.Where(x => x.Enable == true).AsQueryable();
            switch (baseSearch.Sort)
            {
                case 1:
                    pg = baseSearch.SortType ? pg.OrderBy(x => x.FileName) : pg.OrderByDescending(x => x.FileName);
                    break;
                default:
                    pg = baseSearch.SortType ? pg.OrderBy(x => x.CreatedOn) : pg.OrderByDescending(x => x.CreatedOn);
                    break;
            }
            if (!string.IsNullOrEmpty(baseSearch.Name))
                pg = pg.Where(x => x.FileName.Contains(baseSearch.Name));
            if (baseSearch.TopicId != null)
                pg = pg.Where(x => x.TopicId == baseSearch.TopicId);
            
            if (!pg.Any())
                return new AppDomainResult() { TotalRow = 0, Data = null };
            int totalRow = pg.Count();
            var data = await pg.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToListAsync();
            return new AppDomainResult() { TotalRow = totalRow, Data = data };
        }
    }
}