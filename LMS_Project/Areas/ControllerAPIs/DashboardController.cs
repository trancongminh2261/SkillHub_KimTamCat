using LMS_Project.Areas.ControllerAPIs.ExamPeriod;
using LMS_Project.Areas.Models;
using LMS_Project.DTO.ExamPeriod;
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
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.DashboardService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class DashboardController : BaseController
    {
        private lmsDbContext dbContext;
        private DashboardService domainService;
        public DashboardController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new DashboardService(this.dbContext);
        }

        /// <summary>
        /// thống kê số liệu tổng quan
        /// </summary>
        /// <param name="baseSeach"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/statistical-overview")]
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
        [Route("api/Dashboard/webinar-progress")]
        [SwaggerResponse(200, "OK", typeof(IList<StatisticalModel>))]
        public async Task<HttpResponseMessage> WebinarProgress([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.WebinarProgress(baseSearch, GetCurrentUser());
            if (data == null || data.Count <= 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }

        /// <summary>
        /// xếp hạng dựa trên thành tích học tập
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/academic-rank")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<AcademicRankModel>))]
        public async Task<HttpResponseMessage> AcademicRank([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.AcademicRank(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        /// <summary>
        /// tỷ lệ vượt qua bài kiểm tra
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/rate-pass-exam")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<AcademicRankModel>))]
        public async Task<HttpResponseMessage> RatePassExam([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.RatePassExam(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        /// <summary>
        /// thời gian học của học viên
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/study-time")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<AcademicRankModel>))]
        public async Task<HttpResponseMessage> StudyTime([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.StudyTime(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/Dashboard/statistical-exam-period")]
        [SwaggerResponse(200, "OK", typeof(AppDomainResult<ExamPeriodStatistical>))]
        public async Task<HttpResponseMessage> StatisticalExamPeriod([FromUri] StatisticalSearch baseSearch)
        {
            var data = await domainService.StatisticalExamPeriod(baseSearch, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        #region V2
        /*[HttpGet]
        [Route("api/Dashboard/OverviewV2")]
        public async Task<HttpResponseMessage> OverviewModelV2()
        {
            try
            {
                var data = await DashboardService.OverviewModelV2();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }*/

        /*[HttpGet]
        [Route("api/Dashboard/TotalDevice")]
        public async Task<HttpResponseMessage> TotalDevice()
        {
            try
            {
                var data = await DashboardService.TotalDevice();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }*/

        [HttpGet]
        [Route("api/Dashboard/TotalStaffCompleteAndNotCompleteCourse")]
        public async Task<HttpResponseMessage> TotalStaffCompleteAndNotCompleteCourse()
        {
            try
            {
                var data = await DashboardService.TotalStaffCompleteAndNotCompleteCourse();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }

        [HttpGet]
        [Route("api/Dashboard/TotalStaffStudyMoreOrLessThan5Hour")]
        public async Task<HttpResponseMessage> TotalStaffStudyMoreOrLessThan5Hour()
        {
            try
            {
                var data = await DashboardService.TotalStaffStudyMoreOrLessThan5Hour();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/AverageVideoViewingTime")]
        public async Task<HttpResponseMessage> AverageVideoViewingTime()
        {
            try
            {
                var data = await DashboardService.AverageVideoViewingTime();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }

        [HttpGet]
        [Route("api/Dashboard/AverageVideoViewingViews")]
        public async Task<HttpResponseMessage> AverageVideoViewingViews()
        {
            try
            {
                var data = await DashboardService.AverageVideoViewingViews();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }

        [HttpGet]
        [Route("api/Dashboard/CountViewOfVideo")]
        public async Task<HttpResponseMessage> CountViewOfVideo()
        {
            try
            {
                var data = await DashboardService.CountViewOfVideo();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        #endregion


        [HttpGet]
        [Route("api/Dashboard/Overview")]
        public async Task<HttpResponseMessage> GetAll()
        {
            try
            {
                var data = await DashboardService.OverviewModel();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
            
        }
        [HttpGet]
        [Route("api/Dashboard/StatisticGetInMonth")]
        public async Task<HttpResponseMessage> GetAllInMonth()
        {
            try
            {
                var data = await DashboardService.OverviewModelInMonth();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/StatisticAgeStudent")]
        public async Task<HttpResponseMessage> GetAllAgeStudent()
        {
            try
            {
                var data = await DashboardService.StatisticForAge();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/StatisticTopCourse")]
        public async Task<HttpResponseMessage> StatisticTopCourse()
        {
            try
            {
                var data = await DashboardService.StatisticTopCourse();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/OverviewTeacher")]
        public async Task<HttpResponseMessage> OverviewTeacher()
        {
            try
            {
                var data = await DashboardService.OverviewModelForTeacher(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/Dashboard/OverviewStudent")]
        public async Task<HttpResponseMessage> OverviewStudent()
        {
            try
            {
                var data = await DashboardService.StatisticCourseStudent(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Dashboard/Student/LearningDetails")]
        public async Task<HttpResponseMessage> LearningDetails()
        {
            try
            {
                var data = await DashboardService.LearningDetails(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê học tập của người dùng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/OverviewLearning")]
        public async Task<HttpResponseMessage> OverviewLearning([FromUri] OverviewSearch search)
        {
            try
            {
                var data = await DashboardService.OverviewLearning(search);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê khoá học của hệ thống
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/OverviewVideoCourse")]
        public async Task<HttpResponseMessage> OverviewVideoCourse([FromUri] OverviewSearch search)
        {
            try
            {
                var data = await DashboardService.OverviewVideoCourse(search);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Thống kê bài tập
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/OverviewExam")]
        public async Task<HttpResponseMessage> OverviewExam([FromUri] OverviewSearch search)
        {
            try
            {
                var data = await DashboardService.OverviewExam(search);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        //[HttpGet]
        //[ClaimsAuthorize(new RoleEnum[] {
        //    RoleEnum.admin,
        //    RoleEnum.teacher,
        //})]
        //[Route("api/Dashboard/OverviewUserInformation")]
        //public async Task<HttpResponseMessage> OverviewUserInformation()
        //{
        //    try
        //    {
        //        var data = await DashboardService.OverviewUserInformation();
        //        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
        //    }
        //}
        #region Dashboard For Student
        /// <summary>
        /// Chứng chỉ của học viên
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/certificate")]
        public async Task<HttpResponseMessage> GetCertificateInDashboard()
        {
            try
            {
                var data = await DashboardService.GetCertificateInDashboard(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        /// <summary>
        /// Đang học dở
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Dashboard/learning")]
        public async Task<HttpResponseMessage> GetLearningInDashboard()
        {
            try
            {
                var data = await DashboardService.GetLearningInDashboard(GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        #endregion
        [HttpGet]
        [Route("api/Dashboard/unfinished-grading")]
        public async Task<HttpResponseMessage> GetUnfinishedGrading()
        {
            try
            {
                var data = await DashboardService.GetUnfinishedGrading();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Dashboard/statistical-certificate")]
        public async Task<HttpResponseMessage> GetStatisticalCertificate()
        {
            try
            {
                var data = await DashboardService.GetStatisticalCertificate();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Dashboard/statistical-exam-result")]
        public async Task<HttpResponseMessage> GetStatisticalExamResult()
        {
            try
            {
                var data = await DashboardService.GetStatisticalExamResult();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công!", data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }



}
