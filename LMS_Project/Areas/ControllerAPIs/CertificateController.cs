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
using static LMS_Project.Services.LessonVideoService;

namespace LMS_Project.Areas.ControllerAPIs
{
    [ClaimsAuthorize]
    public class CertificateController : BaseController
    {
        [HttpGet]
        [Route("api/Certificate/{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var data = await CertificateService.GetById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
        [HttpPut]
        [Route("api/Certificate")]
        public async Task<HttpResponseMessage> Update(CertificateUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await CertificateService.Update(model, GetCurrentUser());
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
        [Route("api/Certificate")]
        public async Task<HttpResponseMessage> GetAll([FromUri] CertificateSearch search)
        {
            var data = await CertificateService.GetAll(search, GetCurrentUser());
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }

        [HttpPost]
        [Route("api/Certificate/{videoCourseId}/for-student/{studentId}")]
        public async Task<HttpResponseMessage> CreateCertificate(int videoCourseId, int studentId)
        {
            try
            {
                using (var dbContext = new lmsDbContext())
                {
                    if (GetCurrentUser().RoleId == ((int)RoleEnum.student))
                        studentId = GetCurrentUser().UserInformationId;
                    var check = await dbContext.tbl_VideoCourseStudent
                        .AnyAsync(x => x.VideoCourseId == videoCourseId && x.UserId == studentId && x.Enable == true && x.Status == 3);
                    if (!check)
                        throw new Exception("Chưa hoàn thành chương trình học");

                    var hasCertificate = await dbContext.tbl_Certificate.AnyAsync(x => x.UserId == studentId && x.VideoCourseId == videoCourseId && x.Enable == true);
                    if (hasCertificate)
                        throw new Exception("Học viên đã được cấp chứng chỉ");

                    await CertificateService.CreateCertificate(dbContext,videoCourseId,studentId);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        public class ExportPdf
        {
            public int Id { get; set; }
            public string Content { get; set; }
        }
        [Route("api/Certificate/export-pdf")]
        public async Task<HttpResponseMessage> ExportPDF(ExportPdf itemModel)
        {
            using (lmsDbContext dbContext = new lmsDbContext())
            {
                try
                {
                    var httpContext = HttpContext.Current;
                    var path = Path.Combine(httpContext.Server.MapPath("~/Upload"));
                    string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                    string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                    var pathViews = Path.Combine(httpContext.Server.MapPath("~/Views"));
                    var data = await CertificateService.ExportPDF(dbContext, itemModel.Id, itemModel.Content, path, strUrl);
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        message = "Thành công",
                        data = data
                    });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
        }
    }
}
