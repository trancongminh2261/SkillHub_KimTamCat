using LMS_Project.Areas.Models;
using LMS_Project.Models;
using LMS_Project.Services;
using LMS_Project.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Areas.ControllerAPIs
{
    [RoutePrefix("api/Area")]
    public class AreaController : BaseController
    {
        [HttpGet]
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            tbl_Area data = await Area.GetById(Id);
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" , data });
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);

        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll([FromUri] AreaSearch search)
        {
            var data = await Area.GetAll(search);
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
