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
using System.Data.Entity;
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
    [RoutePrefix("api/StudyRouteDetail")]
    public class StudyRouteDetailController : BaseController
    {
        private lmsDbContext dbContext;
        private StudyRouteDetailService domainService;
        public StudyRouteDetailController()
        {
            this.dbContext = new lmsDbContext();
            this.domainService = new StudyRouteDetailService(this.dbContext);
        }
        
        /// <summary>
        /// lấy danh sách khóa học
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ListVideoCourse")]
        public async Task<HttpResponseMessage> GetListVideoCourse(int StudyRouteId, string tags = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await domainService.GetListVideoCourse(StudyRouteId, tags);
                    if (!data.Any())
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        /// <summary>
        /// Tìm kiếm chi tiết lộ trình học theo StudyRouteId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{StudyRouteId}")]
        public async Task<HttpResponseMessage> GetByStudyRouteId(int StudyRouteId)
        {
            try
            {
                var data = await domainService.GetByStudyRouteId(StudyRouteId);
                if (data == null)
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data = data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }

        /// <summary>
        /// Thêm mới khóa học vào chi tiết lộ trình học
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Insert(StudyRouteDetailCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await domainService.Insert(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }

        /// <summary>
        /// Thêm mới nhiều khóa học vào chi tiết lộ trình học
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ImportListCourse")]
        public async Task<HttpResponseMessage> InsertMulti(MultiStudyRouteDetailCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await domainService.InsertMulti(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thàng công", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }

        /// <summary>
        /// sắp xếp thứ tự trong lộ trình học
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPut]
        [Route("Index")]
        public async Task<HttpResponseMessage> UpdateIndex(List<IndexUpdate> request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await domainService.UpdateIndex(request, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }

        /// <summary>
        /// xóa khóa học trong lộ trình học
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await domainService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}
