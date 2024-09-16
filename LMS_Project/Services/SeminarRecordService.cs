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
    public class SeminarRecordService
    {
        public static async Task<tbl_SeminarRecord> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_SeminarRecord.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_SeminarRecord> Insert(SeminarRecordCreate seminarRecordCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var seminar = await db.tbl_Seminar.AnyAsync(x=>x.Id == seminarRecordCreate.SeminarId);
                if (!seminar)
                    throw new Exception("Không tìm thấy Webinar");
                var model = new tbl_SeminarRecord(seminarRecordCreate);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_SeminarRecord.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_SeminarRecord> Update(SeminarRecordUpdate model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_SeminarRecord.SingleOrDefaultAsync(x => x.Id == model.Id);
                if (data == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                data.Name = model.Name ?? data.Name;
                data.VideoUrl = model.VideoUrl ?? data.VideoUrl;
                await db.SaveChangesAsync();
                return data;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_SeminarRecord.SingleOrDefaultAsync(x => x.Id == id);
                if (data == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                data.Enable = false;
                await db.SaveChangesAsync();
            }
        }
        public static async Task<List<tbl_SeminarRecord>> GetBySeminar(int seminarId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_SeminarRecord.Where(x => x.Enable == true && x.SeminarId == seminarId).OrderBy(x=>x.Name).ToListAsync();
                return data;
            }
        }
    }
}