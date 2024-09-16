using LMS_Project.Areas.Models;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs
{
    public class BaseController : ApiController
    {
        
        [NonAction]
        public string ParseToDate(string value)
        {
            try
            {
                System.DateTime td = System.DateTime.ParseExact(value, "dd/MM/yyyy", null);
                value = td.Date.ToString("yyyy-MM-dd 23:59:59.999");
            }
            catch
            {
                value = null;
            }
            return value;
        }
        [NonAction]
        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        [NonAction]
        public static tbl_UserInformation GetCurrentUser()
        {
            try
            {
                if (ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier) == null) return null;
                var name = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                var username = Encryptor.Decrypt(name);
                var user_information = Task.Run(() =>UserInformation.GetById(int.Parse(username))).Result;
                return user_information;
            }
            catch { return null; }

        }
    }
}
