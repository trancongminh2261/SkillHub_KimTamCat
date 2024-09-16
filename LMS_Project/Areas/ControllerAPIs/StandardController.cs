﻿using ExcelDataReader;
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
    public class StandardController : BaseController
    {
        private lmsDbContext dbContext;
        private StandardService standardService;
        public StandardController()
        {
            this.dbContext = new lmsDbContext();
            this.standardService = new StandardService(this.dbContext);
        }

        [HttpGet]
        [Route("api/Standard/{id:int}")]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            var data = await standardService.GetById(Id);
            if (data != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", data });
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        [HttpPut]
        [Route("api/Standard")]
        public async Task<HttpResponseMessage> Update(StandardUpdate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await standardService.Update(model, GetCurrentUser());
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
        [HttpPost]
        [Route("api/Standard")]
        public async Task<HttpResponseMessage> Insert(StandardCreate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await standardService.Insert(model, GetCurrentUser());
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
        [Route("api/Standard/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                await standardService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = e.Message });
            }
        }
        [HttpGet]
        [Route("api/Standard")]
        public async Task<HttpResponseMessage> GetAll([FromUri] StandardSearch baseSearch)
        {
            var data = await standardService.GetAll(baseSearch);
            if (data.TotalRow == 0)
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Thành công !", totalRow = data.TotalRow, data = data.Data });
        }
    }
}
