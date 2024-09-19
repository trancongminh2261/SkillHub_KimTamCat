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
    public class UserInformationController : BaseController
    { 
        //[HttpPost]
        //[Route("api/UserInformation/Upload")]
        //[ClaimsAuthorize(new lmsEnum.RoleEnum[] { 
        //    lmsEnum.RoleEnum.admin,
        //    lmsEnum.RoleEnum.student,
        //    lmsEnum.RoleEnum.teacher
        //})]
        //public HttpResponseMessage Upload()
        //{
        //    try
        //    {
        //        string link = "";
        //        var httpContext = HttpContext.Current;
        //        var file = httpContext.Request.Files.Get("File");
        //        if (file != null)
        //        {
        //            WebImage img = new WebImage(file.InputStream);
        //            if (img.Width > 750)
        //                img.Resize(750, 750);

        //            string ext = Path.GetExtension(file.FileName).ToLower();
        //            string fileName = Guid.NewGuid() + ext; // getting File Name
        //            string fileExtension = Path.GetExtension(fileName).ToLower();
        //            var result = AssetCRM.isValIdFileCustom(ext); // ValIdate Header
        //            if (result)
        //            {
        //                fileName = Guid.NewGuid() + ext;
        //                var path = Path.Combine(httpContext.Server.MapPath("~/Upload/Images/"), fileName);
        //                string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
        //                string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
        //                link = strUrl + "Upload/Images/" + fileName;
        //                img.Save(path);
        //                return Request.CreateResponse(HttpStatusCode.OK, new { data = link, message = ApiMessage.SAVE_SUCCESS });
        //            }
        //            else
        //            {
        //                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.INVALID_FILE });
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ApiMessage.NOT_FOUND });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
        //    }
        //}
        [HttpPost]
        [Route("api/UserInformation")]
        public async Task<HttpResponseMessage> Insert(UserCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await UserInformation.ValidateUser(model.UserName, model.Email);
                    var data = await UserInformation.Insert(new tbl_UserInformation(model), GetCurrentUser());
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
        [Route("api/UserInformation")]
        public async Task<HttpResponseMessage> Update(UserUpdate model)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser();
                if(user.RoleId != ((int)RoleEnum.admin) && user.UserInformationId != model.UserInformationId)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Không được phép thực hiện!"});
                try
                {
                    var data = await UserInformation.Update(new tbl_UserInformation(model), user);
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data = data });
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
        [Route("api/UserInformation/{userInformationId}")]
        public async Task<HttpResponseMessage> Delete(int userInformationId)
        {
            try
            {
                await UserInformation.Delete(userInformationId);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }

        }
        [HttpGet]
        [Route("api/UserInformation")]
        public async Task<HttpResponseMessage> GetAll([FromUri] UserSearch baseSearch)
        {
            var data = await UserInformation.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/UserInformation/{userInformationId}")]
        public async Task<HttpResponseMessage> GetById(int userInformationId)
        {
            var data = await UserInformation.GetById(userInformationId);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !",data = new UserInformationModel(data) });
        }
        public class OneSignalModel
        { 
            public string OneSignal_deviceId { get; set; }
        }
        [HttpPut]
        [Route("api/Update_OneSignal_DeviceId")]
        public async Task<HttpResponseMessage> Update_OneSignal_DeviceId([FromBody] OneSignalModel model)
        {
            var data = await UserInformation.Update_OneSignal_DeviceId(model.OneSignal_deviceId, GetCurrentUser());
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("api/UserInformation/ImportStudent")]
        public async Task<HttpResponseMessage> ImportStudent()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                using (var db = new lmsDbContext())
                {
                    var model = new List<RegisterModel>();
                    if (httpRequest.Files.Count > 0)
                    {
                        Inputfile = httpRequest.Files.Get("File");
                        FileStream = Inputfile.InputStream;
                        if (Inputfile != null && FileStream != null)
                        {
                            if (Inputfile.FileName.EndsWith(".xls"))
                                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                            else if (Inputfile.FileName.EndsWith(".xlsx"))
                                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                            else
                                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Không đúng định dạng." });
                            dsexcelRecords = reader.AsDataSet();
                            reader.Close();
                            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                                for (int i = 2; i < dtStudentRecords.Rows.Count; i++)
                                {
                                    var item = new RegisterModel
                                    {
                                        FullName = dtStudentRecords.Rows[i][0].ToString(),
                                        UserName = dtStudentRecords.Rows[i][1].ToString(),
                                        Email = dtStudentRecords.Rows[i][2].ToString(),
                                        Mobile = dtStudentRecords.Rows[i][3].ToString(),
                                        Password = Encryptor.Encrypt(dtStudentRecords.Rows[i][4].ToString())
                                    };
                                    if (string.IsNullOrEmpty(item.FullName) ||  string.IsNullOrEmpty(item.UserName))
                                    {
                                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Vui lòng điền đầy đủ họ tên và tài khoản đăng nhập" });
                                    }
                                    model.Add(item);
                                }
                            }
                            else
                                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Không có dữ liệu." });
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "File lỗi." });
                    }
                    await UserInformation.ImportData(model, GetCurrentUser());
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thêm thành công" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/UserInformation/learning-progress/{userId}")]
        public async Task<HttpResponseMessage> LearningProgress(int userId)
        {
            try
            { 
                var data = await UserInformation.LearningProgress(userId);
                if (data == null)
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data = data });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
    }
}
