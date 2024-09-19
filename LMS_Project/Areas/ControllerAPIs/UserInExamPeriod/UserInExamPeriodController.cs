using LMS_Project.Areas.Attribute;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO.UserInExamPeriod;
using LMS_Project.Models;
using LMS_Project.Services.UserInUserInExamPeriod;
using LMS_Project.Users;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs.UserInExamPeriod
{
    [ClaimsAuthorize]
    [RoutePrefix("api/UserInExamPeriod")]
    [ValidateModelState]
    public class UserInExamPeriodController : BaseController
    {
        private lmsDbContext dbContext;
        private UserInExamPeriodService domainService;
        public UserInExamPeriodController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new UserInExamPeriodService(this.dbContext);
        }
        [HttpGet]
        [Route("{id:int}")]
        [SwaggerResponse(200, "OK", typeof(UserInExamPeriodDTO))]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await domainService.GetById(id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpPost]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(IList<UserInExamPeriodDTO>))]
        public async Task<HttpResponseMessage> Insert(UserInExamPeriodCreate model)
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
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<UserInExamPeriodDTO>))]
        public async Task<HttpResponseMessage> GetAll([FromUri] UserInExamPeriodSearch baseSearch)
        {
            var data = await domainService.GetAll(baseSearch);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }

        [HttpGet]
        [Route("user-available/{examPeriodId}")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<UserInExamPeriodDTO>))]
        public async Task<HttpResponseMessage> GetUserAvailable(int examPeriodId)
        {
            var data = await domainService.GetUserAvailable(examPeriodId);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
        }
    }
}
