using ExcelDataReader;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;
namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class NotificationController : BaseController
    {
        [HttpGet]
        [Route("api/Notification")]
        public async Task<HttpResponseMessage> GetAll([FromUri] SearchOptions search)
        {
            var data = await NotificationService.GetAll(search,GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpPut]
        [Route("api/Notification/Seen/{id}")]
        public async Task<HttpResponseMessage> Seen(int id)
        {
            await NotificationService.Seen(id,GetCurrentUser());
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
        }
        [HttpPut]
        [Route("api/Notification/SeenAll")]
        public async Task<HttpResponseMessage> SeenAll()
        {
            await NotificationService.SeenAll(GetCurrentUser());
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
        }
        /// <summary>
        /// true - gửi thông báo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Notification/maintenance")]
        public async Task<HttpResponseMessage> MaintenanceNotice()
        {
            var data = await NotificationService.MaintenanceNotice();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
    }
}
