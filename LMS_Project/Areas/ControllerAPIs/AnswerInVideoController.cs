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

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class AnswerInVideoController : BaseController
    {
        [HttpPost]
        [Route("api/AnswerInVideo")]
        public async Task<HttpResponseMessage> Insert(AnswerInVideoCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await AnswerInVideoService.Insert(model, GetCurrentUser());
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
        [Route("api/AnswerInVideo/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await AnswerInVideoService.Delete(id, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/AnswerInVideo/{id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            var data = await AnswerInVideoService.GetById(Id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpGet]
        [Route("api/AnswerInVideo/GetByQuestion/{questionInVideoId}")]
        public async Task<HttpResponseMessage> GetByQuestion(int questionInVideoId)
        {
            var data = await AnswerInVideoService.GetByQuestion(questionInVideoId);
            if (data.Any())
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
