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
    [RoutePrefix("api/District")]
    public class DistrictController : BaseController
    {
        [HttpGet]
        [Route("{Id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            tbl_District data = await District.GetById(Id);
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new {  message = "Thành công !" , data });

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Thất bại !" });
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] DistrictSearch search)
        {
            var data = await District.GetAll(search);
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

