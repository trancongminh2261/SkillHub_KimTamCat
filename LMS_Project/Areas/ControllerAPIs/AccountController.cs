using LMS_Project.Areas.ControllerAPIs;
using LMS_Project.Areas.Models;
using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using System.Web.Http.Filters;
using Facebook;
using static LMS_Project.Services.UserInformation;
using LMS_Project.Areas.Request;
using static LMS_Project.Models.lmsEnum;
using static LMS_Project.Services.Account;
using System.Web.Helpers;
using System.IO;

namespace LMS_Project.ControllerAPIs
{
    //
    public class AccountController : BaseController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Login()
        {
            string username = HttpContext.Current.Request.Form.Get("username");
            string password = HttpContext.Current.Request.Form.Get("password");
            TokenResult appDomainResult = await Account.Login(username, password);
            if (appDomainResult.ResultCode != ((int)HttpStatusCode.OK))
                return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
                {
                    message = appDomainResult.ResultMessage,
                });
            return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
            {
                message = appDomainResult.ResultMessage,
                token = appDomainResult.GenerateTokenModel.Token,
                refreshToken = appDomainResult.GenerateTokenModel.RefreshToken,
                refreshTokenExpires = appDomainResult.GenerateTokenModel.RefreshTokenExpires,
            });
        }

        [HttpPost]
        public async Task<HttpResponseMessage> LoginTest(string username, string password)
        {
            TokenResult appDomainResult = await Account.Login(username, password);
            if (appDomainResult.ResultCode != ((int)HttpStatusCode.OK))
                return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
                {
                    message = appDomainResult.ResultMessage,
                });
            return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
            {
                message = appDomainResult.ResultMessage,
                token = appDomainResult.GenerateTokenModel.Token,
                refreshToken = appDomainResult.GenerateTokenModel.RefreshToken,
                refreshTokenExpires = appDomainResult.GenerateTokenModel.RefreshTokenExpires,
            });
        }
        public class LoginDevModel
        { 
            public int Id { get; set; }
            public string PassDev { get; set; }
        }
        [HttpPost]
        [Route("api/LoginDev")]
        public async Task<HttpResponseMessage> LoginByDev(LoginDevModel model)
        {

            TokenResult appDomainResult = await Account.LoginByDev(model);
            if (appDomainResult.ResultCode != ((int)HttpStatusCode.OK))
                return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
                {
                    message = appDomainResult.ResultMessage,
                });
            return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
            {
                message = appDomainResult.ResultMessage,
                Token = appDomainResult.GenerateTokenModel.Token,
                refreshToken = appDomainResult.GenerateTokenModel.RefreshToken,
                refreshTokenExpires = appDomainResult.GenerateTokenModel.RefreshTokenExpires,
            });
        }

        [HttpPost]
        [Route("api/RefreshToken")]
        public async Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest itemModel)
        {
            TokenResult appDomainResult = await Account.RefreshToken(itemModel);
            if (appDomainResult.ResultCode == ((int)HttpStatusCode.Unauthorized))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    message = appDomainResult.ResultMessage,
                });
            }
            return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
            {
                message = appDomainResult.ResultMessage,
                token = appDomainResult.GenerateTokenModel.Token,
                refreshToken = appDomainResult.GenerateTokenModel.RefreshToken,
                refreshTokenExpires = appDomainResult.GenerateTokenModel.RefreshTokenExpires,
            });
        }
        [HttpGet]
        [Route("api/GetAccount")]
        public async Task<HttpResponseMessage> GetAccount()
        {
            try
            {
                var data = await Account.GetAccount();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công", data });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message + "||" + ex.InnerException });
            }
        }
        [HttpPost]
        [ClaimsAuthorize]
        [Route("api/ChangePassword")]
        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await Account.ChangePassword(model,GetCurrentUser());
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
        [HttpPost]
        [Route("api/Register")]
        public async Task<HttpResponseMessage> Register(RegisterModel model)
        {
            if(await Account.GetAllowRegister() == AllowRegister.UnAllow)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Chức năng này đã tắt" });
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await Account.Register(model);
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
        /// Tạo token mới
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ClaimsAuthorize]
        [Route("api/NewToken")]
        public async Task<HttpResponseMessage> NewToken()
        {

            TokenResult appDomainResult = await Account.NewToken(GetCurrentUser());
            return Request.CreateResponse((HttpStatusCode)appDomainResult.ResultCode, new
            {
                message = appDomainResult.ResultMessage,
                Token = appDomainResult.GenerateTokenModel.Token,
                refreshToken = appDomainResult.GenerateTokenModel.RefreshToken,
                refreshTokenExpires = appDomainResult.GenerateTokenModel.RefreshTokenExpires,
            });
        }
        [HttpPost]
        [ClaimsAuthorize]
        [Route("api/ChangeRegister/{value}")]
        public async Task<HttpResponseMessage> ChangeRegister(AllowRegister value)
        {
            try
            {
                await Account.ChangeRegister(value);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/AllowRegister")]
        public async Task<HttpResponseMessage> GetAllowRegister()
        {
            var result = await Account.GetAllowRegister();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = result.ToString() });
        }
        [HttpPost]
        [Route("api/KeyForgotPassword")]
        public async Task<HttpResponseMessage> KeyForgotPassword([FromBody] KeyForgotPasswordModel model)
        {
            try
            {
                await Account.KeyForgotPassword(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !"});
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpPost]
        [Route("api/ResetPassword")]
        public async Task<HttpResponseMessage> ResetPassword([FromBody] ResetPasswordModel model)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    await Account.ResetPassword(model);
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
        [HttpPost]
        [Route("api/Base/Upload")]
        public HttpResponseMessage Upload()
        {
            try
            {
                string link = "";
                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");
                if (file != null)
                {
                    WebImage img = new WebImage(file.InputStream);
                    //if (img.Width > 750)
                    //    img.Resize(750, 750);

                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string fileName = Guid.NewGuid() + ext; // getting File Name
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    var result = AssetCRM.isValIdFileCustom(ext); // ValIdate Header
                    if (result)
                    {
                        fileName = Guid.NewGuid() + ext;
                        var path = Path.Combine(httpContext.Server.MapPath("~/Upload/Images/"), fileName);
                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        link = strUrl + "Upload/Images/" + fileName;
                        img.Save(path);
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

        [HttpPost]
        [Route("api/Base/Upload/ResizeImage")]
        public HttpResponseMessage ResizeImage()
        {
            try
            {
                string linkOriginal = "";
                string linkResized = "";

                var httpContext = HttpContext.Current;
                var file = httpContext.Request.Files.Get("File");

                if (file != null)
                {
                    WebImage imgOriginal = new WebImage(file.InputStream);

                    //string ext = Path.GetExtension(file.FileName).ToLower();
                    string ext = imgOriginal.ImageFormat.ToLower();
                    if (!ext.StartsWith("."))
                    {
                        ext = "." + ext;
                    }

                    var result = AssetCRM.isValIdFileCustom(ext); // Validate Header

                    if (result)
                    {
                        string fileName = Path.ChangeExtension(Guid.NewGuid().ToString(), ext);
                        var pathOriginal = Path.Combine(httpContext.Server.MapPath("~/Upload/Images/"), fileName);
                        var pathResized = Path.Combine(httpContext.Server.MapPath("~/Upload/Images/"), "resized_" + fileName);

                        string strPathAndQuery = httpContext.Request.Url.PathAndQuery;
                        string strUrl = httpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                        linkOriginal = strUrl + "Upload/Images/" + fileName;
                        linkResized = strUrl + "Upload/Images/" + "resized_" + fileName;

                        imgOriginal.Save(pathOriginal);

                        WebImage imgResized = imgOriginal.Clone();
                        imgResized.Resize((int)(imgOriginal.Width * 0.7), (int)(imgOriginal.Height * 0.7));
                        imgResized.Save(pathResized);

                        if (!linkOriginal.Contains("https"))
                            linkOriginal = linkOriginal.Replace("http", "https");
                        if (!linkResized.Contains("https"))
                            linkResized = linkResized.Replace("http", "https");

                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            //originalImage = linkOriginal,
                            resizedImage = linkResized,
                            message = ApiMessage.SAVE_SUCCESS
                        });
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

        [HttpGet]
        [Route("api/Base/Upload/File")]
        public HttpResponseMessage GetFileBaseUpload()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { data = "jpg,jpeg,png,bmp", message = "Thành công" });
        }
    }
}
