using LMS_Project.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using LMS_Project.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using LMS_Project.LMS;
using System.Threading.Tasks;

namespace LMS_Project.Users
{
    public class JWTManager
    {
        private static string Secret = "bG1zLm1vbmFtZWRpYS52bg==";
        public static async Task<string> GenerateToken(tbl_UserInformation user)
        {
            try
            {
                return await Task.Run(() =>
                {
                    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
                    SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                    {

                        Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.NameIdentifier, Encryptor.Encrypt(user.Email)),
                      new Claim(ClaimTypes.Name, user.FullName),
                      new Claim(ClaimTypes.Role, user.RoleName),
                      new Claim("roleId", user.RoleId.ToString() ?? ""),
                      new Claim("Gender", user.Gender.ToString() ?? ""),
                      new Claim("DOB", user.DOB.ToString() ?? ""),
                      new Claim("Email", user.Email.ToString() ?? ""),
                      new Claim("Mobile", user.Mobile.ToString() ?? ""),
                      new Claim("Address", user.Address.ToString() ?? ""),
                      new Claim("FullName", user.FullName.ToString() ?? ""),
                      new Claim("Avatar", user.Avatar.ToString() ?? "")
                }
                        ),
                        Expires = DateTime.UtcNow.AddDays(1),
                        //Expires = DateTime.UtcNow.AddMinutes(1),
                        SigningCredentials = new SigningCredentials(securityKey,
                        SecurityAlgorithms.HmacSha256Signature)
                    };

                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
                    return handler.WriteToken(token);
                });
            }
            catch (Exception exception)
            {
                await AssetCRM.WriteLog(new AssetCRM.LogModel() { function = "GenerateToken", exception = exception });
                return string.Empty;
            }
        }
        //public static async Task<string> GenerateToken(int userInformationId)
        //{
        //    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        //    var user = await UserInformation.GetById(userInformationId);
        //    SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        //    {

        //        Subject = new ClaimsIdentity(new[] {
        //              new Claim(ClaimTypes.NameIdentifier, Encryptor.Encrypt(user.UserInformationId.ToString())),
        //              new Claim(ClaimTypes.Name, user.FullName),
        //              new Claim(ClaimTypes.Role, user.RoleName),
        //              new Claim("UserInformationId", user.UserInformationId.ToString()),
        //              new Claim("FullName", user.FullName.ToString()),
        //              new Claim("UserCode", user.UserCode ?? ""),
        //              new Claim("RoleId", user.RoleId?.ToString() ?? ""),
        //              new Claim("Gender", user.Gender?.ToString() ?? ""),
        //              new Claim("DOB", user.DOB == null ? "" : user.DOB.ToString()),
        //              new Claim("Email", user.Email ?? ""),
        //              new Claim("Mobile", user.Mobile ?? ""),
        //              new Claim("Address", user.Address ?? ""),
        //              new Claim("Avatar", user.Avatar ?? ""),
        //              new Claim("AreaId", user.AreaId.ToString()),
        //              new Claim("DistrictId", user.DistrictId.ToString()),
        //              new Claim("WardId", user.WardId.ToString()),
        //        }
        //        ),
        //        Expires = DateTime.UtcNow.AddDays(1),
        //        SigningCredentials = new SigningCredentials(securityKey,
        //        SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        //    JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
        //    return handler.WriteToken(token);
        //}

        public class GenerateTokenModel
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            /// <summary>
            /// Hạn sử dụng refresh token
            /// </summary>
            public DateTime? RefreshTokenExpires { get; set; }
        }
        public static async Task<GenerateTokenModel> GenerateToken(int userInformationId)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var user = await UserInformation.GetById(userInformationId);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.NameIdentifier, Encryptor.Encrypt(user.UserInformationId.ToString())),
                      new Claim(ClaimTypes.Name, user.FullName),
                      new Claim(ClaimTypes.Role, user.RoleName),
                      new Claim("UserInformationId", user.UserInformationId.ToString()),
                      new Claim("FullName", user.FullName.ToString()),
                      new Claim("UserCode", user.UserCode ?? ""),
                      new Claim("UserName", user.UserName ?? ""),
                      new Claim("RoleId", user.RoleId?.ToString() ?? ""),
                      new Claim("Gender", user.Gender?.ToString() ?? ""),
                      new Claim("DOB", user.DOB == null ? "" : user.DOB.ToString()),
                      new Claim("Email", user.Email ?? ""),
                      new Claim("Mobile", user.Mobile ?? ""),
                      new Claim("Address", user.Address ?? ""),
                      new Claim("Avatar", user.Avatar ?? ""),
                      new Claim("AreaId", user.AreaId.ToString()),
                      new Claim("DistrictId", user.DistrictId.ToString()),
                      new Claim("WardId", user.WardId.ToString()),
                      new Claim("NickName", user.NickName ?? ""),
                      new Claim("CMND", user.CMND ?? ""),
                }
                ),

                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };
            string refreshToken = Guid.NewGuid().ToString();
            DateTime refreshTokenExpires = DateTime.Now.AddDays(3);

            if (DateTime.Now < user.RefreshTokenExpires)
                refreshToken = user.RefreshToken;
            await Account.AddRefreshToken(new Account.AddRefreshTokenRequest
            {
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpires,
                UserId = userInformationId
            });

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //return handler.WriteToken(token);
            return new GenerateTokenModel
            {
                Token = handler.WriteToken(token),
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpires
            };
        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(hmac.Key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static string ValIdateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity Identity = null;
            try
            {
                Identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim usernameClaim = Identity.FindFirst(ClaimTypes.NameIdentifier);
            username = usernameClaim.Value;
            return username;
        }

    }
}