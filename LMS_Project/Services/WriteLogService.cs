using LMS_Project.Areas.Models;
using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace LMS_Project.Services
{
    public class WriteLogService
    {
        public class WriteLogCreate
        {
            public string Note { get; set; }
        }
        public static async Task<tbl_WriteLog> Insert(WriteLogCreate itemModel)
        {
            using (var db = new lmsDbContext())
            {
                var item = new tbl_WriteLog
                {
                    Note = itemModel.Note
                };
                db.tbl_WriteLog.Add(item);
                await db.SaveChangesAsync();
                return item;
            }
        }
        public static async Task<List<tbl_WriteLog>> GetAll()
        {
            using (var db = new lmsDbContext())
            {
                return await db.tbl_WriteLog.OrderByDescending(x => x.Id).ToListAsync();
            }
        }
    }
}