using LMS_Project.LMS;
using LMS_Project.Models;
using LMS_Project.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace LMS_Project.Users
{
    public class ClaimsAuthorizeAttribute : Attribute, IAuthenticationFilter
    {
        public lmsEnum.RoleEnum[] AllowRoles { get; set; }

        public bool AllowMultiple => false;

        public ClaimsAuthorizeAttribute()
        {
            AllowRoles = null;
        }
        public ClaimsAuthorizeAttribute(lmsEnum.RoleEnum[] AllowRoles)
        {
            this.AllowRoles = AllowRoles;
        }

        //public overrIde Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        string token = HttpContext.Current.Request.Headers.Get("token");
        //        var principal = JWTManager.GetPrincipal(token);
        //        int roleId = int.Parse(principal.FindFirst("roleId").Value);

        //        if (!principal.Identity.IsAuthenticated)
        //            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = ApiMessage.EXPIRE_TOKEN });

        //        if (AllowRoles != null && !AllowRoles.Contains((lmsEnum.RoleEnum)roleId))
        //            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = ApiMessage.UNAUTHORIZED });
        //        //var userName = principal.FindFirst(ClaimTypes.Name).Value;
        //        //var userAllowedTime = principal.FindFirst("BranchId").Value;

        //        //if (userAllowedTime != null)
        //        //{
        //        //    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Not allowed to access...bla bla");
        //        //    return Task.FromResult<object>(null);
        //        //}

        //        //User is Authorized, complete execution

        //        return Task.FromResult<object>(null);
        //    }
        //    catch
        //    {
        //        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = ApiMessage.EXPIRE_TOKEN });
        //        return Task.FromResult<object>(null);
        //    }

        //}

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            try
            {
                string token = request.Headers.GetValues("token").FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    context.ErrorResult = new AuthenticationFailureResult("Hết hạn đăng nhập", request);
                    return;
                }
                var controllerName = context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                var actionName = context.ActionContext.ActionDescriptor.ActionName;
                //var token = authorization.Parameter;
                var principal = JWTManager.GetPrincipal(token);
                if (principal == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Hết hạn đăng nhập", request);
                    return;
                }
                if (!principal.Identity.IsAuthenticated)
                    context.ErrorResult = new AuthenticationFailureResult(ApiMessage.EXPIRE_TOKEN, request);
                else
                {
                    int roleId = int.Parse(principal.FindFirst("RoleId").Value);
                    var allow = await Account.HasPermission(roleId, controllerName, actionName);
                    if (!allow)
                        context.ErrorResult = new AuthenticationFailureResult(ApiMessage.UNAUTHORIZED, request);
                    else
                    {
                        context.Principal = principal;
                    }
                }
            }
            catch
            {
                context.ErrorResult = new AuthenticationFailureResult("Hết hạn đăng nhập", request);
                return;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        //public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        //{
        //    Challenge(context);
        //    return Task.FromResult(0);
        //}

        //private void Challenge(HttpAuthenticationChallengeContext context)
        //{
        //    string parameter = null;

        //    if (AllowRoles != null)
        //        parameter = "allowRoles=\"" + AllowRoles. + "\"";

        //    context.ChallengeWith("Bearer", parameter);
        //}
    }
}