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
    public class CertificateConfigController : BaseController
    {
        [HttpGet]
        [Route("api/CertificateConfig/GetGuide")]
        public async Task<HttpResponseMessage> GetGuide()
        {
            var data = await CertificateConfigService.GetGuide();
            if (data.Any())
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpPost]
        [Route("api/CertificateConfig")]
        public async Task<HttpResponseMessage> Insert(CertificateConfigCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CertificateConfigService.Insert(model, GetCurrentUser());
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
        [Route("api/CertificateConfig")]
        public async Task<HttpResponseMessage> Update(CertificateConfigUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CertificateConfigService.Update(model, GetCurrentUser());
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
        [Route("api/CertificateConfig/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await CertificateConfigService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/CertificateConfig/{id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            var data = await CertificateConfigService.GetById(Id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpGet]
        [Route("api/CertificateConfig")]
        public async Task<HttpResponseMessage> GetAll([FromUri] SearchOptions search)
        {
            var data = await CertificateConfigService.GetAll(search);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
