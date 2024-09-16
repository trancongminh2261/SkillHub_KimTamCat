using LMS_Project.Areas.Attribute;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO;
using LMS_Project.DTO.OptionDTO;
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
    [RoutePrefix("api/VideoCourseAllow")]
    [ValidateModelState]
    public class VideoCourseAllowController : BaseController
    {
        private lmsDbContext dbContext;
        private VideoCourseAllowService domainService;
        public VideoCourseAllowController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new VideoCourseAllowService(this.dbContext);
        }
        [HttpGet]
        [Route("list-allow")]
        [SwaggerResponse(200, "OK", typeof(IList<VideoCourseAllowAvailableDTO>))]
        public async Task<HttpResponseMessage> GetAllowAvailable([FromUri] VideoCourseAllowAvaibleSearch baseSeach)
        {
            var data = await domainService.GetAllowAvailable(baseSeach);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        [HttpPut]
        [Route("delete-multi")]
        public async Task<HttpResponseMessage> DeleteMulti(List<int> ListId)
        {
            try
            {
                await domainService.DeleteMulti(ListId, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !"});
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPost]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(IList<VideoCourseAllowDTO>))]
        public async Task<HttpResponseMessage> Insert(VideoCourseAllowCreate model)
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
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<VideoCourseAllowDTO>))]
        public async Task<HttpResponseMessage> GetAll([FromUri] VideoCourseAllowSearch baseSearch)
        {
            var data = await domainService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
