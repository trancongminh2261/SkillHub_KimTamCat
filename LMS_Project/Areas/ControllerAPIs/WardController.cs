using LMS_Project.Areas.Models;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Ward")]
    public class WardController : BaseController
    {
       
        [HttpGet]
        [Route("{Id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            tbl_Ward data = await Ward.GetbyId(Id);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Thất bại !" });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" ,data});
        }
        [HttpGet]
        [Route("")]
        //Lấy danh sách quận huyện
        public async Task<HttpResponseMessage> GetAll([FromUri] WardSearch search)
        {
            var data = await Ward.GetAll(search);
            int totalRow = data.TotalRow;
            if (totalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            if (data != null)

            {
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow, data = data.obj });

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Thất bại !" });
        }
    }
}
