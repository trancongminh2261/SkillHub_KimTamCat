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
    public class VideoCourseController : BaseController
    {
        [HttpPost]
        [Route("api/VideoCourse")]
        public async Task<HttpResponseMessage> Insert(VideoCourseCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await VideoCourseService.Insert(model, GetCurrentUser());
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
        [Route("api/VideoCourse")]
        public async Task<HttpResponseMessage> Update(VideoCourseUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await VideoCourseService.Update(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !",data });
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
        [Route("api/VideoCourse/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await VideoCourseService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/VideoCourse")]
        public async Task<HttpResponseMessage> GetAll([FromUri] VideoCourseSearch search)
        {
            var data = await VideoCourseService.GetAll(search,GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/VideoCourse/v2")]
        public async Task<HttpResponseMessage> GetAllV2([FromUri] VideoCourseSearchV2 search)
        {
            var data = await VideoCourseService.GetAllV2(search, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/VideoCourse/{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await VideoCourseService.GetById(id,GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("api/VideoCourse/Overview/{videoCourseId}")]
        public async Task<HttpResponseMessage> GetVideoCourseOverview(int videoCourseId)
        {
            var data = await VideoCourseService.GetVideoCourseOverview(videoCourseId);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("api/VideoCourse/list-id-detail/{videoCourseId}")]
        public async Task<HttpResponseMessage> GetListIdDetail(int videoCourseId)
        {
            var data = await VideoCourseService.GetListIdDetail(videoCourseId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpGet]
        [Route("api/VideoCourse/status")]
        public async Task<HttpResponseMessage> GetStatus([FromUri] VideoCourseSearchV2 search)
        {
            var data = await VideoCourseService.GetStatus(search, GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
