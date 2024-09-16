using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace LMS_Project.Areas.Attribute
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var message = actionContext.ModelState.Values
                    .FirstOrDefault(x => x.Errors.Count > 0);
                if (message != null)
                {
                    string mess = string.IsNullOrEmpty(message.Errors.FirstOrDefault().ErrorMessage)
                        ? message.Errors.FirstOrDefault().Exception.Message
                        : message.Errors.FirstOrDefault().ErrorMessage;
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new { message = mess });
                }
            }
        }
    }
}