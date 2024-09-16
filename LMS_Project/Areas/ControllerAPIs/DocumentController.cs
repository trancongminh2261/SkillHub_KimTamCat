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
    public class DocumentController : BaseController
    {
        private lmsDbContext dbContext;
        private DocumentService documentService;
        private string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };
        public DocumentController()
        {
            this.dbContext = new lmsDbContext();
            this.documentService = new DocumentService(this.dbContext);
        }

        [HttpGet]
        [Route("api/Document/{id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            var data = await documentService.GetById(Id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/Document")]
        public async Task<HttpResponseMessage> GetAll([FromUri] DocumentSearch baseSearch)
        {
            var data = await documentService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpPut]
        [Route("api/Document")]
        public async Task<HttpResponseMessage> Update()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string link = "";
                    var httpContext = HttpContext.Current;
                    var file = httpContext.Request.Files.Get("File");
                    string topicId = httpContext.Request.Form.Get("TopicId");
                    string currentId = httpContext.Request.Form.Get("Id");
                    if (file != null && topicId != null && currentId != null)
                    {
                        DocumentUpdate request = new DocumentUpdate();
                        string ext = Path.GetExtension(file.FileName).ToLower();
                        
                        string fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/Documents/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/Documents/" + fileName;

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            file.InputStream.CopyTo(fileStream);
                        }
                        //update
                        request.Id = Int32.Parse(currentId);
                        request.TopicId = Int32.Parse(topicId);
                        request.FileName = file.FileName;
                        request.FileType = ext;
                        request.AbsolutePath = link;
                        var data = await documentService.Update(request, GetCurrentUser());
                        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });

                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.NOT_FOUND });
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
            var message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = message });
        }

        [HttpPost]
        [Route("api/Document")]
        public async Task<HttpResponseMessage> Insert()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string link = "";
                    var httpContext = HttpContext.Current;
                    var file = httpContext.Request.Files.Get("File");
                    string topicId = httpContext.Request.Form.Get("TopicId");
                    if (file != null && topicId != null)
                    {
                        DocumentCreate request = new DocumentCreate();
                        string ext = Path.GetExtension(file.FileName).ToLower();
                   
                        string fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/Documents/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/Documents/" + fileName;

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            file.InputStream.CopyTo(fileStream);
                        }

                        //insert document with id and fil e
                        request.TopicId = Int32.Parse(topicId);
                        request.FileName = file.FileName;
                        request.FileType = ext;
                        request.AbsolutePath = link;
                        var data = await documentService.Insert(request, GetCurrentUser());
                        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
                       
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.NOT_FOUND });
                    }
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
        [Route("api/Document/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await documentService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}
