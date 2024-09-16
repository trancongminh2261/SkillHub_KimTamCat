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
using static LMS_Project.Services.VideoCourseStudentService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class VideoCourseStudentController : BaseController
    {
        [HttpPost]
        [Route("api/VideoCourseStudent/AddVideoCourse/{videoCourseId}")]
        public async Task<HttpResponseMessage> AddVideoCourse(int videoCourseId)
        {
            try
            {
                await VideoCourseStudentService.AddVideoCourse(videoCourseId, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        ///// <summary>
        ///// Danh sách khoá học của mình
        ///// </summary>
        ///// <param name="search"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[ClaimsAuthorize(new RoleEnum[] {
        //    RoleEnum.admin,
        //    RoleEnum.student,
        //    RoleEnum.teacher
        //})]
        //[Route("api/VideoCourseStudent")]
        //public async Task<HttpResponseMessage> GetAll([FromUri] VideoCourseStudentSearch search)
        //{
        //    var data = await VideoCourseStudentService.GetAll(search, GetCurrentUser());
        //    if (data.TotalRow == 0)
        //        return Request.CreateResponse(HttpStatusCode.NoContent);
        //    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        //}
        [HttpGet]
        [Route("api/VideoCourseStudent/GetStudent")]
        public async Task<HttpResponseMessage> GetStudentInVideoCourse([FromUri] StudentInVideoCourseSearch search)
        {
            var data = await VideoCourseStudentService.GetStudentInVideoCourse(search);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/VideoCourseStudent/{videoCourseId}/GetLearningDetail/{userId}")]
        public async Task<HttpResponseMessage> GetLearningDetail(int videoCourseId, int userId)
        {
            var data = await VideoCourseStudentService.GetLearningDetail(videoCourseId,userId);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        /// <summary>
        /// Đánh giá 
        /// myRate: từ 1 => 5
        /// </summary>
        /// <param name="videoCourseId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/VideoCourseStudent/AddRate")]
        public async Task<HttpResponseMessage> AddRate(AddRateModel model)
        {
            try
            {
                await VideoCourseStudentService.AddRate(model, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/VideoCourseStudent/Rate/{videoCourseId}")]
        public async Task<HttpResponseMessage> GetRate(int videoCourseId)
        {
            var data = await VideoCourseStudentService.GetRate(videoCourseId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        /// <summary>
        /// Cấp chứng chỉ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/VideoCourseStudent/CreateCertificate")]
        //public async Task<HttpResponseMessage> CreateCertificate()
        //{
        //    try
        //    {
        //        await CertificateService.CreateCertificate(GetCurrentUser());
        //        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
        //    }
        //}
    }
}
