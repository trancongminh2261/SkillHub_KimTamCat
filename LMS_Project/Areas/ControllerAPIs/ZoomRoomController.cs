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
    public class ZoomRoomController : BaseController
    {
        public object LockObject = new object();
        [HttpPost]
        [Route("api/ZoomRoom/CreateRoom/{seminarId}")]
        public HttpResponseMessage CreateRoom(int seminarId)
        {
            ///Không cho tạo phòng cùng lúc
            lock (LockObject)
            {
                try
                {
                    var data = Task.Run(() => ZoomRoomService.CreateRoom(seminarId, GetCurrentUser())).Result;
                    if(data.Success == false) 
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = data.ResultMessage });
                    LockObject = new object();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data.Data });
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
                }
            }
        }
        [HttpPut]
        [Route("api/ZoomRoom/CloseRoom/{seminarId}")]
        public async Task<HttpResponseMessage> CloseRoom(int seminarId)
        {
            try
            {
                await ZoomRoomService.CloseRoom(seminarId, GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/ZoomRoom/GetActive")]
        public async Task<HttpResponseMessage> GetActive([FromUri] SearchOptions search)
        {
            var data = await ZoomRoomService.GetActive(search);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
        [HttpGet]
        [Route("api/ZoomRoom/GetRecord/{seminarId}")]
        public async Task<HttpResponseMessage> GetRecord(int seminarId)
        {
            var data = await ZoomRoomService.GetRecord(seminarId);
            if (!data.Any())
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data = data });
        }
    }
}
