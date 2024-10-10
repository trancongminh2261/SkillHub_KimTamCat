using LMS_Project.Areas.Attribute;
using LMS_Project.Areas.Models;
using LMS_Project.DTO.VideoCourseCompletedHistory;
using LMS_Project.Models;
using LMS_Project.Services.VideoCourseCompletedHistory;
using LMS_Project.Users;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs.VideoCourseCompletedHistory
{
    [ClaimsAuthorize]
    [RoutePrefix("api/VideoCourseCompletedHistory")]
    [ValidateModelState]
    public class VideoCourseCompletedHistoryController : BaseController
    {
        private lmsDbContext dbContext;
        private VideoCourseCompletedHistoryService domainService;
        public VideoCourseCompletedHistoryController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new VideoCourseCompletedHistoryService(this.dbContext);
        }
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<VideoCourseCompletedHistoryDTO>))]
        public async Task<HttpResponseMessage> GetAll([FromUri] VideoCourseCompletedHistorySearch baseSearch)
        {
            var data = await domainService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
