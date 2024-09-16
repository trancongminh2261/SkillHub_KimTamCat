using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace LMS_Project.Services
{
    public class ZoomRoomService
    {

        static readonly char[] padding = { '=' };
        ///Tạo phòng zoom
        public static async Task<AppDomainResult> CreateRoom(int seminarId, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var seminar = await db.tbl_Seminar.SingleOrDefaultAsync(x => x.Id == seminarId);
                    if (seminar == null)
                        return new AppDomainResult { Success = false, ResultMessage = "Không tìm thấy buổi Webinar" };
                    if (user.RoleId != ((int)RoleEnum.admin) && user.UserInformationId != seminar.LeaderId)
                        return new AppDomainResult { Success = false, ResultMessage = "Bạn không có quyền tạo phòng" };

                    var zoomConfig = db.tbl_ZoomConfig
                        .Where(x => x.Active == false && x.Enable == true)
                        .FirstOrDefault();

                    var zoomRoom = await db.tbl_ZoomRoom.FirstOrDefaultAsync(x => x.SeminarId == seminar.Id && x.Enable == true);
                    if (zoomRoom != null)
                    {
                        if (zoomRoom.IsOpenZoom = true)
                        {
                            zoomConfig = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == zoomRoom.ZoomConfigId);
                        }
                    }

                    if (zoomConfig == null)
                        return new AppDomainResult { Success = false, ResultMessage = "Không có tài khoản Zoom nào đang trống, vui lòng thêm tài khoản" };
                    
                    ZoomModel zoomModel = new ZoomModel
                    {
                        Status = true,
                        RoomId = "",
                        RoomPass = "",
                        ApiKey = zoomConfig.APIKey,
                        SignatureTeacher = "",
                        SignatureStudent = "",
                        UserName = user.FullName,
                    };
                    var tokenOject = JObject.Parse(AccessTokenWithServerToServer(zoomConfig.UserZoom, zoomConfig.APIKey, zoomConfig.APISecret));

                    string tokenString = tokenOject["access_token"].ToString();

                    var client = new RestClient("https://api.zoom.us/v2/users/me/meetings");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;

                    var dataBody = new
                    {
                        topic = seminar.Name,
                        type = "1",
                        settings = new
                        {
                            waiting_room = false
                        }
                    };

                    request.AddJsonBody(dataBody);
                    request.AddHeader("Authorization", String.Format("Bearer {0}", tokenString));
                    request.AddHeader("Content-Type", "application/json");

                    IRestResponse restResponse = client.Execute(request);
                    HttpStatusCode statusCode = restResponse.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    var jObject = JObject.Parse(restResponse.Content);


                    if (numericStatusCode == 201)
                    {
                        if (string.IsNullOrEmpty(jObject["id"].ToString()) || string.IsNullOrEmpty(jObject["encrypted_password"].ToString()))
                        {
                            throw new Exception("Tạo phòng không thành công");
                        }
                        else
                        {
                            zoomModel.RoomId = jObject["id"].ToString();
                            zoomModel.RoomPass = jObject["password"].ToString();
                            zoomModel.SignatureTeacher = jObject["start_url"].ToString();
                            zoomModel.SignatureStudent = jObject["join_url"].ToString();
                        }
                    }
                    else
                        throw new Exception("Tạo phòng không thành công");

                    if (zoomRoom == null)
                    {
                        db.tbl_ZoomRoom.Add(new tbl_ZoomRoom
                        {
                            CreatedBy = user.FullName,
                            CreatedOn = DateTime.Now,
                            Enable = true,
                            LeaderId = user.UserInformationId,
                            ModifiedBy = user.FullName,
                            ModifiedOn = DateTime.Now,
                            IsOpenZoom = true,
                            RoomId = zoomModel.RoomId,
                            RoomPass = zoomModel.RoomPass,
                            SeminarId = seminar.Id,
                            ZoomConfigId = zoomConfig.Id,
                            SignatureStudent = zoomModel.SignatureStudent,
                            SignatureTeacher = zoomModel.SignatureTeacher
                        });
                    }
                    else
                    {
                        zoomRoom.IsOpenZoom = true;
                        zoomRoom.RoomId = zoomModel.RoomId;
                        zoomRoom.RoomPass = zoomModel.RoomPass;
                        zoomRoom.ZoomConfigId = zoomConfig.Id;
                        zoomRoom.SignatureStudent = zoomModel.SignatureStudent;
                        zoomRoom.SignatureTeacher = zoomModel.SignatureTeacher;
                    }
                    zoomConfig.Active = true;
                    seminar.Status = 2;
                    seminar.StatusName = "Đang diễn ra";

                    await db.SaveChangesAsync();
                    return new AppDomainResult { Success = true, Data = zoomModel } ;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }
        ///Tắt phòng 
        public static async Task CloseRoom(int seminarId, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var seminar = await db.tbl_Seminar.SingleOrDefaultAsync(x => x.Id == seminarId);
                    if (seminar == null)
                        throw new Exception("Không tìm thấy buổi Webinar");

                    if (user.RoleId != ((int)RoleEnum.admin) && user.UserInformationId != seminar.LeaderId)
                        throw new Exception("Bạn không có quyền tạo phòng");
                    var zoomRoom = await db.tbl_ZoomRoom
                        .Where(x => x.SeminarId == seminar.Id && x.Enable ==true).FirstOrDefaultAsync();
                    if (zoomRoom != null)
                    {
                        var zoomConfig = await db.tbl_ZoomConfig.SingleOrDefaultAsync(x => x.Id == zoomRoom.ZoomConfigId);
                        if (zoomConfig != null)
                        {
                            zoomConfig.Active = false;

                            var tokenOject = JObject.Parse(AccessTokenWithServerToServer(zoomConfig.UserZoom, zoomConfig.APIKey, zoomConfig.APISecret));
                            string tokenString = tokenOject["access_token"].ToString();

                            var client = new RestClient($"https://api.zoom.us/v2/meetings/{zoomRoom.RoomId}/status");
                            var request = new RestRequest(Method.PUT);
                            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
                            request.AddParameter("application/json", "{\"action\":\"end\"}", ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            HttpStatusCode statusCode = response.StatusCode;
                            int numericStatusCode = (int)statusCode;
                        }
                        zoomRoom.IsOpenZoom = false;
                    }
                    seminar.Status = 3; 
                    seminar.StatusName = "Kết thúc";
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<AppDomainResult> GetActive(SearchOptions search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                string sql = $"Get_ZoomActive @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}";
                var data = await db.Database.SqlQuery<Get_ZoomActive>(sql).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                var result = data.Select(i => new tbl_ZoomRoom(i)).ToList();
                return new AppDomainResult { TotalRow = totalRow, Data = result };
            }
        }
        /// <summary>
        /// 30p sau khi hết giờ hệ thống tự động tắt phòng zoom nếu chưa tắt
        /// </summary>
        /// <returns></returns>
        public static async Task AutoCloseRoom()
        {
            using (var db = new lmsDbContext())
            {
                DateTime time = DateTime.Now.AddMinutes(-30);
                var seminars = await db.tbl_Seminar
                    .Where(x => x.Enable == true && x.Status != 3 && time > x.EndTime).ToListAsync();
                if (seminars.Any())
                {
                    foreach (var item in seminars)
                        await CloseRoom(item.Id, new tbl_UserInformation { RoleId = 1, FullName = "Tự động" });
                }
            }
        }
        /// <summary>
        /// Lấy bảng ghi buổi họp
        /// </summary>
        /// <param name="courseScheduleID"></param>
        /// <returns></returns>
        public static async Task<List<RecordingFiles>> GetRecord(int seminarId)
        {
            using (var db = new lmsDbContext())
            {
                var seminar = await db.tbl_Seminar.SingleOrDefaultAsync(x => x.Id == seminarId);
                if (seminar == null)
                    throw new Exception("Không tìm thấy buổi Webinar");
                var zoomRoom = await db.tbl_ZoomRoom.Where(x => x.SeminarId == seminar.Id).FirstOrDefaultAsync();
                if (zoomRoom == null)
                    return new List<RecordingFiles>();
                var zoomConfig = db.tbl_ZoomConfig.SingleOrDefault(x => x.Id == zoomRoom.ZoomConfigId);
                if (zoomConfig == null)
                    return new List<RecordingFiles>();

                List<RecordingFiles> rFile = new List<RecordingFiles>();
                try
                {
                    var tokenOject = JObject.Parse(AccessTokenWithServerToServer(zoomConfig.UserZoom, zoomConfig.APIKey, zoomConfig.APISecret));
                    string tokenString = tokenOject["access_token"].ToString();
                    var client = new RestClient($"https://api.zoom.us/v2/meetings/{zoomRoom.RoomId}/recordings");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
                    IRestResponse response = client.Execute(request);
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    //chỗ này hứng data nè
                    if (numericStatusCode == 200)
                    {
                        var jObject = JObject.Parse(response.Content);
                        if (jObject != null && jObject["recording_files"] != null)
                        {
                            rFile = JsonConvert.DeserializeObject<List<RecordingFiles>>(jObject["recording_files"].ToString());
                        }
                    }
                }
                catch { }

                if (!rFile.Any())
                    return new List<RecordingFiles>();
                else
                    return rFile;
            }
        }
        public static string AccessTokenWithServerToServer(string acountId, string clientId, string clientSecret)
        {
            var client = new RestClient("https://zoom.us/oauth/token?grant_type=account_credentials&account_id=" + acountId);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            var authenticationString = $"{(clientId.Trim())}:{(clientSecret.Trim())}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
            request.AddHeader("Authorization", "Basic " + base64EncodedAuthenticationString);

            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}