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
using static LMS_Project.Services.ExamResultService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class ExamResultController : BaseController
    {
        [HttpPost]
        [Route("api/ExamResult/Submit")]
        public async Task<HttpResponseMessage> Submit(ExamSubmit model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ExamResultService.Submit(model, GetCurrentUser());
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
        [HttpGet]
        [Route("api/ExamResult")]
        public async Task<HttpResponseMessage> GetAll([FromUri] ExamResultSearch search)
        {
            var data = await ExamResultService.GetAll(search,GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/ExamResult/Detail/{examResultId}")]
        public async Task<HttpResponseMessage> GetDetail(int examResultId)
        {
            var data = await ExamResultService.GetDetail(examResultId);
            if (data.Data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", 
                data = data.Data,
                totalPoint = data.TotalPoint,
                myPoint = data.MyPoint,
                examCode = data.ExamCode,
                examName = data.ExamName,
                isPass = data.IsPass,
                lessonVideoId = data.LessonVideoId,
                passPoint = data.PassPoint,
                type = data.Type,
                status = data.Status,
                statusName = data.StatusName,
                createdOn = data.CreatedOn,
                createdBy = data.CreatedBy,
                videoCourseId = data.VideoCourseId,
            });
        }
        [HttpPost]
        [Route("api/ExamResult/upload-video")]
        public HttpResponseMessage UploadVideo()
        {
            try
            {
                string link = "";
                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string fileName = Guid.NewGuid() + ext; // getting File Name
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    var result = AssetCRM.isValIdImageAndVIdeo(ext); // Validate Header                 
                    if (result)
                    {
                        fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/ExamResult/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/ExamResult/" + fileName;

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.InputStream.CopyTo(stream);
                        }
                        if (!link.Contains("https"))
                            link = link.Replace("http", "https");
                        return Request.CreateResponse(HttpStatusCode.OK, new { data = link, message = ApiMessage.SAVE_SUCCESS });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.INVALID_FILE });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.NOT_FOUND });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Chấm bài tự luận
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ExamResult/grading-essay")]
        public async Task<HttpResponseMessage> CreateGradingEssay(GradingEssayModel itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ExamResultService.CreateGradingEssay(itemModel, GetCurrentUser());
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
        /// <summary>
        /// Xem chi tiết chấm bài tự luận
        /// </summary>
        /// <param name="examResultId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ExamResult/grading-essay/{examResultId}")]
        public async Task<HttpResponseMessage> GetGradingEssay(int examResultId)
        {
            var data = await ExamResultService.GetGradingEssay(examResultId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                message = "Thành công !",
                data = data,
            });
        }
        [HttpPost]
        [Route("api/ExamResult/add-teachers")]
        public async Task<HttpResponseMessage> AddTeachers(AddTeacherModel itemModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExamResultService.AddTeachers(itemModel, GetCurrentUser());
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
        [HttpGet]
        [Route("api/ExamResult/knowledge-exam-completed/{videoCourseId}")]
        public async Task<HttpResponseMessage> KnowledgeExamCompleted(int videoCourseId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await ExamResultService.KnowledgeExamCompleted(videoCourseId, GetCurrentUser());
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
    }
}
