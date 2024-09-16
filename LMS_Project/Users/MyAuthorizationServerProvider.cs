using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace LMS_Project
{
    public class MyAuthorizationServerProvIder : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        //public overrIde async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    using (UserMasterRepository _repo = new UserMasterRepository())
        //    {
        //        var user = _repo.ValIdateUser(context.UserName, context.Password);
        //        if (user == null)
        //        {
        //            context.SetError("invalId_grant", "ProvIded username and password is incorrect");
        //            return;
        //        }
        //        var Identity = new ClaimsIdentity(context.Options.AuthenticationType);  
        //        Identity.AddClaim(new Claim(ClaimTypes.Role, user.RoleName));
        //        Identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
        //        Identity.AddClaim(new Claim("Email", user.Email));
        //        context.ValIdated(Identity);
        //    }
        //}
    }
}