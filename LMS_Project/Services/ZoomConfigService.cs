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
    public class ZoomConfigService
    {
        public static async Task<tbl_ZoomConfig> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == id);
                return data;
            }
        }
        public static async Task<tbl_ZoomConfig> Insert(ZoomConfigCreate zoomConfigCreate,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var model = new tbl_ZoomConfig(zoomConfigCreate);
                model.CreatedBy = model.ModifiedBy = user.FullName;
                db.tbl_ZoomConfig.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task<tbl_ZoomConfig> Update(ZoomConfigUpdate model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == model.Id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.UserZoom = model.UserZoom ?? entity.UserZoom;
                    entity.APIKey = model.APIKey ?? entity.APIKey;
                    entity.APISecret = model.APISecret ?? entity.APISecret;
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = model.ModifiedOn;
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<List<tbl_ZoomConfig>> GetAll()
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_ZoomConfig.Where(x => x.Enable == true).ToListAsync();
                return data;
            }
        }
    }
}