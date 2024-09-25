/*using LMS_Project.Areas.Attribute;
using LMS_Project.Areas.Models;
using LMS_Project.Models;
using LMS_Project.Services.Statistical;
using LMS_Project.Users;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static LMS_Project.Services.Statistical.StatisticalService;

namespace LMS_Project.Areas.ControllerAPIs.Statistical
{
    [ClaimsAuthorize]
    [RoutePrefix("api/Statistical")]
    [ValidateModelState]
    public class StatisticalController : BaseController
    {
        private lmsDbContext dbContext;
        private StatisticalService domainService;
        public StatisticalController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new StatisticalService(this.dbContext);
        }

        /// <summary>
        /// thống kê số liệu tổng quan
        /// </summary>
        /// <param name="baseSeach"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("overview")]
        [SwaggerResponse(200, "OK", typeof(IList<StatisticalModel>))]
        public async Task<HttpResponseMessage> Overview([FromUri] StatisticalSearch baseSeach)
        {
            var data = await domainService.Overview(baseSeach, GetCurrentUser());
            if (data == null || data.Count <= 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        
        /// <summary>
        /// thống kê tiến trình hội thảo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("webinar-progress")]
        [SwaggerResponse(200, "OK", typeof(IList<StatisticalModel>))]
        public async Task<HttpResponseMessage> WebinarProgress([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.WebinarProgress(baseSearch, GetCurrentUser());
            if (data == null || data.Count <= 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        [HttpGet]
        [Route("academic-rank")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<AcademicRankModel>))]
        public async Task<HttpResponseMessage> AcademicRank([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.AcademicRank(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("rate-pass-exam")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<AcademicRankModel>))]
        public async Task<HttpResponseMessage> RatePassExam([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.RatePassExam(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpGet]
        [Route("study-time")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<AcademicRankModel>))]
        public async Task<HttpResponseMessage> StudyTime([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.StudyTime(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
*/