using LMS_Project.Areas.Attribute;
using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO.ExamPeriod;
using LMS_Project.Models;
using LMS_Project.Services.ExamPeriod;
using LMS_Project.Users;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs.ExamPeriod
{
    [ClaimsAuthorize]
    [RoutePrefix("api/ExamPeriod")]
    [ValidateModelState]
    public class ExamPeriodController : BaseController
    {
        private lmsDbContext dbContext;
        private ExamPeriodService domainService;
        public ExamPeriodController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new ExamPeriodService(this.dbContext);
        }
        [HttpGet]
        [Route("{id:int}")]
        [SwaggerResponse(200, "OK", typeof(ExamPeriodDTO))]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await domainService.GetById(id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpPut]
        [Route("")]
        [SwaggerResponse(200, "OK", typeof(ExamPeriodDTO))]
        public async Task<HttpResponseMessage> Update(ExamPeriodUpdate model)
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
        [SwaggerResponse(200, "OK", typeof(ExamPeriodDTO))]
        public async Task<HttpResponseMessage> Insert(ExamPeriodCreate model)
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
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<ExamPeriodDTO>))]
        public async Task<HttpResponseMessage> GetAll([FromUri] SearchOptions baseSearch)
        {
            var data = await domainService.GetAll(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("doing-test/{id}")]
        public async Task<HttpResponseMessage> GetDoingTest(int id)
        {
            var data = await domainService.GetDoingTest(id, true, GetCurrentUser());
            if (data.Data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            double totalPoint = await domainService.GetTotalPoint(id);
            var exam = await domainService.GetExam(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data, totalPoint, time = exam.Time });
        }
    }
}
