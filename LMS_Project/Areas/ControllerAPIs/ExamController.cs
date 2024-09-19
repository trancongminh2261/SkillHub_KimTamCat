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
using static LMS_Project.Services.ExamService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class ExamController : BaseController
    {
        [HttpGet]
        [Route("api/Exam/{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await ExamService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPost]
        [Route("api/Exam")]
        public async Task<HttpResponseMessage> Insert(ExamCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ExamService.Insert(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpPut]
        [Route("api/Exam")]
        public async Task<HttpResponseMessage> Update(ExamUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ExamService.Update(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
        [HttpDelete]
        [Route("api/Exam/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await ExamService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Exam")]
        public async Task<HttpResponseMessage> GetAll([FromUri] ExamSearch baseSearch)
        {
            var data = await ExamService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/Exam/Detail/{examId}")]
        public async Task<HttpResponseMessage> GetDetail(int examId)
        {
            var data = await ExamService.GetDetail(examId,false,GetCurrentUser());
            if (data.Data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            double totalPoint = await ExamService.GetTotalPoint(examId);
            var exam = await ExamService.GetById(examId);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data, totalPoint, passPoint = exam.PassPoint , time = exam.Time });
        }
        [HttpGet]
        [Route("api/Exam/doing-test/{examId}")]
        public async Task<HttpResponseMessage> GetDoingTest(int examId)
        {
            var data = await ExamService.GetDetail(examId, true, GetCurrentUser());
            if (data.Data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            double totalPoint = await ExamService.GetTotalPoint(examId);
            var exam = await ExamService.GetById(examId);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data.Data, totalPoint, time = exam.Time });
        }      

        [HttpPost]
        [Route("api/Exam/AddExerciseGroup")]
        public async Task<HttpResponseMessage> AddExerciseGroup(AddExerciseGroupModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExamService.AddExerciseGroup(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
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
        /// Thêm câu hỏi tự động
        /// </summary>
        /// <remarks>
        /// type 1 - Đơn, 2 - Nhóm
        /// </remarks>
        /// <param name="examSectionId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Exam/{examSectionId}/AddRandom/{amount}/Type/{type}")]
        public async Task<HttpResponseMessage> AddRandom(int examSectionId, int amount,int type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExamService.AddRandom(examSectionId,amount,type, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }
    }
}
