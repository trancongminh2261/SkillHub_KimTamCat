using LMS_Project.Areas.Models;
using LMS_Project.LMS;
using LMS_Project.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_Project.Services
{
    public class Area
    {
        public static async Task<tbl_Area> GetById(int Id)
        {
            tbl_Area createAcc = new tbl_Area();
            using (lmsDbContext _db = new lmsDbContext())
            {
                createAcc = await _db.tbl_Area.Where(c => c.Id == Id).SingleOrDefaultAsync();
            }
            return createAcc;
        }
        public static async Task<ObjectResult> GetAll(AreaSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new ObjectResult { obj = null, TotalRow = 0 }; 
                var l = await db.tbl_Area.Where(x=>x.Enable == true
                && (x.Name.Contains(search.Name) || string.IsNullOrEmpty(search.Name))).OrderBy(x=>x.Name).ToListAsync();
                int totalRow = l.Count();
                var result = l.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                return new ObjectResult { obj = result, TotalRow = totalRow };
            }
        }
    }
}