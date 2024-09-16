using LMS_Project.Areas.Models;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static LMS_Project.Services.WriteLogService;

namespace LMS_Project.Areas.ControllerAPIs
{
    public class WriteLogController : BaseController
    {
        [HttpPost]
        [Route("api/WriteLog")]
        public async Task<HttpResponseMessage> Insert([FromBody] WriteLogCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await WriteLogService.Insert(model);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpGet]
        [Route("api/WriteLog")]
        public async Task<HttpResponseMessage> GetAll()
        {
            var data = await WriteLogService.GetAll();
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
