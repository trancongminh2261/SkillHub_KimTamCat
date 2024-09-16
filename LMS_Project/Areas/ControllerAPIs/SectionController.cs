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
using static LMS_Project.Services.SectionService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class SectionController : BaseController
    {
        [HttpPost]
        [Route("api/Section")]
        public async Task<HttpResponseMessage> Insert(SectionCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await SectionService.Insert(model, GetCurrentUser());
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
        [Route("api/Section")]
        public async Task<HttpResponseMessage> Update(SectionUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await SectionService.Update(model, GetCurrentUser());
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
        [HttpDelete]
        [Route("api/Section/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await SectionService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPut]
        [Route("api/Section/ChangeIndex")]
        public async Task<HttpResponseMessage> ChangeIndex([FromBody] ChangeIndexModel model)
        {
            try
            {
                await SectionService.ChangeIndex(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Section/GetByVideoCourse/{videoCourseId}")]
        public async Task<HttpResponseMessage> GetByVideoCourse(int videoCourseId)
        {
            try
            {
                var data = await SectionService.GetByVideoCourse(videoCourseId, GetCurrentUser());
                if (!data.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                double complete = await SectionService.GetComplete(videoCourseId,GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data, complete = complete });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}
