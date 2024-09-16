using LMS_Project.Areas.Attribute;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    [RoutePrefix("api/VideoConfig")]
    [ValidateModelState]
    public class VideoConfigController : BaseController
    {
        private lmsDbContext dbContext;
        private VideoConfigService domainService;
        public VideoConfigController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new VideoConfigService(this.dbContext);
        }
        [HttpGet]
        [Route("{id:int}")]
        [SwaggerResponse(200, "OK", typeof(VideoConfigDTO))]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await domainService.GetById(id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpPut]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(VideoConfigDTO))]
        public async Task<HttpResponseMessage> Update(VideoConfigUpdate model)
        {
            try
            {
                var data = await domainService.Update(model, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPost]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(VideoConfigDTO))]
        public async Task<HttpResponseMessage> Insert(VideoConfigCreate model)
        {
            try
            {
                var data = await domainService.Insert(model, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await domainService.Delete(id, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("by-lesson-video/{lessonVideoId}")]
        [SwaggerResponse(200, "OK", typeof(IList<VideoConfigDTO>))]
        public async Task<HttpResponseMessage> GetByLessonVideo(int lessonVideoId)
        {
            var data = await domainService.GetByLessonVideo(lessonVideoId);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }

        [HttpGet]
        [Route("detail/{id}")]
        [SwaggerResponse(200, "OK", typeof(VideoConfigDetailDTO))]
        public async Task<HttpResponseMessage> GetDetail(int id)
        {
            var data = await domainService.GetDetail(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
    }
}
