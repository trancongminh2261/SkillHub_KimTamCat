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
using static LMS_Project.Services.LessonVideoService;
using static LMS_Project.Services.PermissionService;

namespace LMS_Project.Areas.ControllerAPIs
{
    public class PermissionController : BaseController
    {
        [HttpPost]
        [Route("api/Permission")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> Insert(PermissionCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await PermissionService.Insert(model);
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

        [HttpPut]
        [Route("api/Permission")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> Update(PermissionUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await PermissionService.Update(model);
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
        [Route("api/Permission/role")]
        public async Task<HttpResponseMessage> GetRole()
        {
            var data = await PermissionService.GetRole();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
        [HttpGet]
        [Route("api/Permission/role-staff")]
        public async Task<HttpResponseMessage> GetRoleStaff()
        {
            var data = await PermissionService.GetRoleStaff();
            data = data.Where(x => x.Id != ((int)RoleEnum.student)).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }

        [HttpGet]
        [Route("api/Permission")]
        [ClaimsAuthorize]
        public async Task<HttpResponseMessage> GetAll([FromUri] PermissionSearch baseSearch)
        {
            var data = await PermissionService.GetAll(baseSearch);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
    }
}
