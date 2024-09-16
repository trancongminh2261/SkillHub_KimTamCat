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
using System.Web.Http.Description;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.ExerciseGroupService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class ExerciseGroupController : BaseController
    {
        [HttpPost]
        [Route("api/ExerciseGroup")]
        public async Task<HttpResponseMessage> Insert(ExerciseGroupCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExerciseGroupService.Insert(model, GetCurrentUser());
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
        [HttpPut]
        [Route("api/ExerciseGroup")]
        public async Task<HttpResponseMessage> Update(ExerciseGroupUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ExerciseGroupService.Update(model, GetCurrentUser());
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
        [Route("api/ExerciseGroup")]
        [ResponseType(typeof(List<ExerciseGroupModel>))]
        public async Task<HttpResponseMessage> GetAll([FromUri] ExerciseGroupSearch search)
        {
            var data = await ExerciseGroupService.GetAll(search);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpDelete]
        [Route("api/ExerciseGroup/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await ExerciseGroupService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPut]
        [Route("api/ExerciseGroup/ChangeIndex")]
        public async Task<HttpResponseMessage> ChangeIndex(ExerciseGroupIndexModel model)
        {
            try
            {
                await ExerciseGroupService.ChangeIndex(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}
