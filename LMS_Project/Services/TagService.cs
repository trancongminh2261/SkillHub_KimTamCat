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
    public class TagService
    {
        public static async Task<tbl_Tag> Insert(TagCreate JobCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var checkExist = await db.tbl_Tag.AnyAsync(x => x.Name.ToUpper() == JobCreate.Name.ToUpper() && x.Enable == true);
                if (checkExist)
                    throw new Exception("Đã tồn tại");
                var model = new tbl_Tag(JobCreate);
                model.ModifiedBy = model.CreatedBy = user.FullName;
                db.tbl_Tag.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_Tag.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    var videoCourses = await db.tbl_VideoCourse.Where(x => x.Enable == true && x.Tags != null && x.Tags != "").ToListAsync();
                    if (videoCourses.Any())
                    {
                        foreach (var item in videoCourses)
                        {
                            var tags = item.Tags.Split(',').ToList();
                            if (tags.Any(x => x == id.ToString()))
                            {
                                tags.Remove(id.ToString());
                                item.Tags = string.Join(",", tags);
                            }
                        }
                    }
                    entity.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<List<tbl_Tag>> GetAll()
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_Tag.Where(x => x.Enable == true).OrderBy(x => x.Type).OrderBy(x => x.Id).ToListAsync();
                return data;
            }
        }
    }
}