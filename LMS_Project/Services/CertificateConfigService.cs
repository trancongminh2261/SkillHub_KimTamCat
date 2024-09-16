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
    public class CertificateConfigService
    {
        public static async Task<List<string>> GetGuide()
        {
            return await Task.Run(() =>
            {
                return new List<string> {
                "{TenChuongTrinh} : Tên chương trình",
                "{TenHocVien} : Tên học viên",
                "{MaHocVien} : Mã học viên",
                "{NgayHoanThanh} : Ngày hoàn thành",
                "{Ngay} : Ngày hiện tại",
                "{Thang} : Tháng hiện tại",
                "{Nam} : Năm hiện tại",
                };
            });
        }
        public static string ReplaceContent(string content,string videoCourseName, tbl_UserInformation user)
        {
            string day = DateTime.Now.Day < 10 ? $"0{DateTime.Now.Day}" : DateTime.Now.Day.ToString();
            string month = DateTime.Now.Month < 10 ? $"0{DateTime.Now.Month}" : DateTime.Now.Month.ToString();
            content = content.Replace("{TenChuongTrinh}", videoCourseName);
            content = content.Replace("{TenHocVien}", user.FullName);
            content = content.Replace("{MaHocVien}", user.UserCode);
            content = content.Replace("{NgayHoanThanh}", DateTime.Now.ToString("dd/MM/yyyy"));
            content = content.Replace("{Ngay}", day);
            content = content.Replace("{Thang}", month);
            content = content.Replace("{Nam}", DateTime.Now.Year.ToString());
            return content;
        }
        public static async Task<tbl_CertificateConfig> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_CertificateConfig.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_CertificateConfig> Insert(CertificateConfigCreate itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_CertificateConfig(itemModel);
                model.CreatedBy = model.ModifiedBy = userLog.FullName;
                db.tbl_CertificateConfig.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_CertificateConfig> Update(CertificateConfigUpdate itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_CertificateConfig.SingleOrDefaultAsync(x => x.Id == itemModel.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Name = itemModel.Name ?? entity.Name;
                entity.Content = itemModel.Content ?? entity.Content;
                entity.Background = itemModel.Background ?? entity.Background;
                entity.Backside = itemModel.Backside ?? entity.Backside;
                entity.ModifiedBy = userLog.FullName;
                entity.ModifiedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_CertificateConfig.SingleOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<AppDomainResult> GetAll(SearchOptions baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new SearchOptions();
                var l = await db.tbl_CertificateConfig.Where(x => x.Enable == true).OrderByDescending(x => x.Id).ToListAsync();
                int totalRow = l.Count();
                var result = l.Skip((baseSearch.PageIndex - 1) * baseSearch.PageSize).Take(baseSearch.PageSize).ToList();
                return new AppDomainResult { Data = result, TotalRow = totalRow };
            }
        }
    }
}