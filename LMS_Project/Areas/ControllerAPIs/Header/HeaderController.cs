using LMS_Project.Areas.Attribute;
using LMS_Project.DTO.Header;
using LMS_Project.Models;
using LMS_Project.Services.Header;
using LMS_Project.Users;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs.Header
{
    [ClaimsAuthorize]
    [RoutePrefix("api/Header")]
    [ValidateModelState]
    public class HeaderController : BaseController
    {
        private lmsDbContext dbContext;
        private HearderService domainService;
        public HeaderController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new HearderService(this.dbContext);
        }
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(MenuCountDTO))]
        public async Task<HttpResponseMessage> GetMenuCount()
        {
            var data = await domainService.GetMenuCount(GetCurrentUser());
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
    }
}
