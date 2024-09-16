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
using WMPLib;
using System.Threading;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using LMS_Project.DTO.ServerDownload;

namespace LMS_Project.Services
{
    public class LessonVideoService
    {
        public static string serverDownload_Api_Key = ConfigurationManager.AppSettings["ServerDownload_API_Key"].ToString();
        public static string serverDownload_Video_Protection_Id = ConfigurationManager.AppSettings["ServerDownload_Video_Protection_Id"].ToString();
        public static async Task<tbl_LessonVideo> GetById(int id)
        {
            using (var db = new lmsDbContext())
            {
                return await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == id);
            }
        }

        /*public static async Task<ResponseGetIframeDTO> GetIframe (string videoUploadId)
        {
            using (HttpClient client = new HttpClient())
            {
                // Thiết lập URL với videoUploadId
                string url = $"https://app-api.mona.host/v1/videos/{videoUploadId}/iframe";

                // Thêm các tiêu đề yêu cầu
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-MONA-KEY", serverDownload_Api_Key);

                // Gửi yêu cầu GET
                HttpResponseMessage response = await client.GetAsync(url);

                // Kiểm tra phản hồi
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Có lỗi xảy ra trong quá trình tải video lên hệ thống");
                // Đọc nội dung phản hồi
                string responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseGetIframeDTO>(responseContent);
                return result;
            }
        }*/

        public static async Task<ResponseInformationDTO> GetDiskUsage()
        {
            using (HttpClient client = new HttpClient())
            {
                // Thiết lập URL với videoUploadId
                string url = $"https://app-api.mona.host/v1/video-protections/{serverDownload_Video_Protection_Id}/disk-usage";

                // Thêm các tiêu đề yêu cầu
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-MONA-KEY", serverDownload_Api_Key);

                // Gửi yêu cầu GET
                HttpResponseMessage response = await client.GetAsync(url);

                // Kiểm tra phản hồi
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Có lỗi xảy ra trong quá trình tải video lên hệ thống");
                // Đọc nội dung phản hồi
                string responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseInformationDTO>(responseContent);
                return result;
            }
        }

        public static async Task<tbl_LessonVideo> Insert(LessonVideoCreate lessonVideoCreate, tbl_UserInformation user, string mapPath,string pathViews)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var section = await db.tbl_Section.SingleOrDefaultAsync(x => x.Id == lessonVideoCreate.SectionId);
                        if (section == null)
                            throw new Exception("Không tìm thấy phần này");
                        var model = new tbl_LessonVideo(lessonVideoCreate);
                        model.CreatedBy = model.ModifiedBy = user.FullName;
                        var prevIndex = await db.tbl_LessonVideo
                            .Where(x => x.SectionId == model.SectionId)
                            .OrderByDescending(x => x.Index).FirstOrDefaultAsync();
                        model.Minute = lessonVideoCreate.Minute;
                        model.Index = prevIndex == null ? 1 : (prevIndex.Index + 1);
                        if (model.FileType == LessonFileType.FileUpload && !string.IsNullOrEmpty(model.VideoUrl))
                        {
                            ///đọc thời gian video tại đây
                            string[] videoUrls = model.VideoUrl.Split('/');
                            if (videoUrls.Length > 0)
                            {
                                try
                                {
                                    string mapUrl = $"{mapPath}/{videoUrls[videoUrls.Length - 1]}";
                                    var player = new WindowsMediaPlayer();
                                    var clip = player.newMedia(mapUrl);
                                    double time = clip.duration;
                                    model.Minute = (int)(time / 60);
                                }
                                catch { }
                            }
                        }
                        db.tbl_LessonVideo.Add(model);
                        await db.SaveChangesAsync();
                        var userCompleteds = await db.tbl_SectionCompleted.Where(x => x.SectionId == lessonVideoCreate.SectionId && x.Enable == true)
                            .Select(x => x.UserId).ToListAsync();
                        if (userCompleteds.Any())
                        {
                            foreach (var item in userCompleteds)
                            {
                                var userCompleted = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item);
                                await CompletedSection(db, lessonVideoCreate.SectionId.Value, userCompleted);
                            }
                        }
                        tran.Commit();

                        Thread pushnoti = new Thread(() =>
                        PushNotiNewLesson(model.Id, pathViews, user));
                        pushnoti.Start();
                        //PushNotiNewLesson(model.Id, pathViews, user);

                        return model;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }

        public static async Task<tbl_LessonVideo> InsertV2(LessonVideoCreateV2 request, tbl_UserInformation user, string mapPath, string pathViews)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var section = await db.tbl_Section.SingleOrDefaultAsync(x => x.Id == request.SectionId);
                        if (section == null)
                            throw new Exception("Không tìm thấy phần này");
                        var model = new tbl_LessonVideo(request);
                        model.CreatedBy = model.ModifiedBy = user.FullName;
                        var prevIndex = await db.tbl_LessonVideo
                            .Where(x => x.SectionId == model.SectionId)
                            .OrderByDescending(x => x.Index).FirstOrDefaultAsync();
                        model.Minute = request.Minute;
                        model.Index = prevIndex == null ? 1 : (prevIndex.Index + 1);
                        if (model.FileType == LessonFileType.FileUpload && !string.IsNullOrEmpty(model.VideoUrl))
                        {
                            ///đọc thời gian video tại đây
                            string[] videoUrls = model.VideoUrl.Split('/');
                            if (videoUrls.Length > 0)
                            {
                                try
                                {
                                    string mapUrl = $"{mapPath}/{videoUrls[videoUrls.Length - 1]}";
                                    var player = new WindowsMediaPlayer();
                                    var clip = player.newMedia(mapUrl);
                                    double time = clip.duration;
                                    //model.Minute = (int)(time / 60);
                                    model.Minute = (int)(time);
                                }
                                catch { }
                            }
                        }
                        if(model.FileType == LessonFileType.AntiDownload)
                        {
                            if (string.IsNullOrEmpty(request.VideoUploadId))
                                throw new Exception("Vui lòng tải video lên");
                            /*var getIframe = await GetIframe(request.VideoUploadId);
                            if(getIframe == null)
                                throw new Exception("Có lỗi xảy ra trong quá trình tải video lên hệ thống");*/
                            var antiDownVideo = new tbl_AntiDownVideo();
                            antiDownVideo.VideoUploadId = request.VideoUploadId;
                            antiDownVideo.Minute = request.Minute;
                            antiDownVideo.Title = request.Name;
                            antiDownVideo.Thumbnail = request.Thumbnail;
                            antiDownVideo.VideoUrl = request.VideoUrl;
                            antiDownVideo.Enable = true;
                            antiDownVideo.CreatedBy = user.FullName;
                            antiDownVideo.ModifiedBy = user.FullName;
                            antiDownVideo.ModifiedOn = DateTime.Now;
                            antiDownVideo.CreatedOn = DateTime.Now;
                            db.tbl_AntiDownVideo.Add(antiDownVideo);
                            await db.SaveChangesAsync();
                            model.AntiDownVideoId = antiDownVideo.Id;
                        }
                        db.tbl_LessonVideo.Add(model);
                        await db.SaveChangesAsync();
                        var userCompleteds = await db.tbl_SectionCompleted.Where(x => x.SectionId == request.SectionId && x.Enable == true)
                            .Select(x => x.UserId).ToListAsync();
                        if (userCompleteds.Any())
                        {
                            foreach (var item in userCompleteds)
                            {
                                var userCompleted = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item);
                                await CompletedSection(db, request.SectionId.Value, userCompleted);
                            }
                        }
                        tran.Commit();

                        Thread pushnoti = new Thread(() =>
                        PushNotiNewLesson(model.Id, pathViews, user));
                        pushnoti.Start();
                        //PushNotiNewLesson(model.Id, pathViews, user);

                        return model;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }

        public static void PushNotiNewLesson(int newLessonId, string pathViews, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var newLesson = db.tbl_LessonVideo.SingleOrDefault(x => x.Id == newLessonId);
                    if (newLesson == null)
                        return;

                    var section = db.tbl_Section.SingleOrDefault(x => x.Id == newLesson.SectionId);
                    if (section == null)
                        return;

                    var videoCourse = db.tbl_VideoCourse.SingleOrDefault(x => x.Id == section.VideoCourseId);
                    if (videoCourse == null)
                        return;

                    var studentIds = db.tbl_VideoCourseStudent.Where(x => x.VideoCourseId == videoCourse.Id && x.Enable == true)
                        .Select(x => x.UserId).Distinct().ToList();
                    if (studentIds.Any())
                    {
                        string domain = ConfigurationManager.AppSettings["DomainFE"].ToString();
                        string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                        //https://skillhub.mona.software/learning/?course=84&sectionIds=57&currentLessonId=1170
                        //string href = $"<a href=\"{domain}/course/video-course/detail/?slug={videoCourse.Id}\"><b style=\"color: blue;\">Tại đây</b></a>";
                        string href = $"<a href=\"{domain}/learning/?course={videoCourse.Id}&sectionIds={section.Id}&currentLessonId={newLesson.Id}\"><b style=\"color: blue;\">Tại đây</b></a>";
                        string title = "Thông báo bài học mới";
                        string content = $"Bạn có bài học mới trong khóa {videoCourse.Name}, vui lòng truy cập {href} để học";
                        string onesignalContent = $"Bạn có bài học mới trong khóa {videoCourse.Name}";
                        string contentEmail = System.IO.File.ReadAllText($"{pathViews}/Template/MailNewLesson.html");

                        contentEmail = contentEmail.Replace("[TenDuAn]", projectName);
                        contentEmail = contentEmail.Replace("[KhoaHoc]", videoCourse.Name);
                        contentEmail = contentEmail.Replace("[Chuong]", section.Name);
                        contentEmail = contentEmail.Replace("[BaiHoc]", newLesson.Name);
                        contentEmail = contentEmail.Replace("[TaiDay]", href);
                        foreach (var studentId in studentIds)
                        {
                            var student = db.tbl_UserInformation.SingleOrDefault(x => x.UserInformationId == studentId); 
                            string mailToStudent = contentEmail;
                            mailToStudent = mailToStudent.Replace("[HoVaTen]",student.FullName);
                            NotificationService.SendNotThread(db,
                                new NotificationService.SendNotThreadModel
                                { 
                                    Content = content,
                                    Email = student.Email,
                                    EmailContent = mailToStudent,
                                    OnesignalId = student.OneSignal_DeviceId,
                                    Title = title,
                                    UserId = student.UserInformationId,
                                    OnesignalContent = onesignalContent,
                                    OnesignalUrl = $"{domain}/learning/?course={videoCourse.Id}&sectionIds={section.Id}&currentLessonId={newLesson.Id}"
                                }
                                , userLog);
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        public static void PushNotiUpdateVideo(int lessonId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var lesson = db.tbl_LessonVideo.SingleOrDefault(x => x.Id == lessonId);
                    if (lesson == null)
                        return;

                    var section = db.tbl_Section.SingleOrDefault(x => x.Id == lesson.SectionId);
                    if (section == null)
                        return;

                    var videoCourse = db.tbl_VideoCourse.SingleOrDefault(x => x.Id == section.VideoCourseId);
                    if (videoCourse == null)
                        return;

                    var studentIds = db.tbl_VideoCourseStudent.Where(x => x.VideoCourseId == videoCourse.Id && x.Enable == true)
                        .Select(x => x.UserId).Distinct().ToList();
                    if (studentIds.Any())
                    {
                        string domain = ConfigurationManager.AppSettings["DomainFE"].ToString();
                        string projectName = ConfigurationManager.AppSettings["ProjectName"].ToString();
                        //https://skillhub.mona.software/learning/?course=84&sectionIds=57&currentLessonId=1170
                        //string href = $"<a href=\"{domain}/course/video-course/detail/?slug={videoCourse.Id}\"><b style=\"color: blue;\">Tại đây</b></a>";
                        string href = $"<a href=\"{domain}/learning/?course={videoCourse.Id}&sectionIds={section.Id}&currentLessonId={lesson.Id}\"><b style=\"color: blue;\">Tại đây</b></a>";
                        string title = "Thông báo cập nhật bài học";
                        string content = $"Bài học {lesson.Name} trong khóa {videoCourse.Name} đã được cập nhật nội dung, vui lòng truy cập {href} để học";
                        string onesignalContent = $"Bài học {lesson.Name} trong khóa {videoCourse.Name} đã được cập nhật nội dung, vui lòng kiểm tra lại thông tin";

                        foreach (var studentId in studentIds)
                        {
                            var student = db.tbl_UserInformation.SingleOrDefault(x => x.UserInformationId == studentId);
                            NotificationService.SendNotThread(db,
                                new NotificationService.SendNotThreadModel
                                {
                                    Content = content,
                                    Email = "",
                                    EmailContent = "",
                                    OnesignalId = student.OneSignal_DeviceId,
                                    Title = title,
                                    UserId = student.UserInformationId,
                                    OnesignalContent = onesignalContent,
                                    OnesignalUrl = $"{domain}/learning/?course={videoCourse.Id}&sectionIds={section.Id}&currentLessonId={lesson.Id}"
                                }
                                , userLog, false);
                        }
                    }
                }
                catch
                {
                    return;
                }
            }

        }
        public static async Task<tbl_LessonVideo> Update(LessonVideoUpdate model, tbl_UserInformation user, string mapPath)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == model.Id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Name = model.Name ?? entity.Name;
                    entity.ExamId = model.ExamId ?? entity.ExamId;
                    entity.VideoUrl = model.VideoUrl ?? entity.VideoUrl;
                    entity.Type = model.Type ?? entity.Type;
                    entity.TypeName = model.TypeName ?? entity.TypeName;
                    entity.FileType = model.FileType ?? entity.FileType;
                    entity.Minute = model.Minute ?? entity.Minute;
                    entity.Thumbnail = model.Thumbnail ?? entity.Thumbnail;
                    if (entity.FileType == LessonFileType.FileUpload && !string.IsNullOrEmpty(model.VideoUrl))
                    {
                        ///đọc thời gian video tại đây
                        string[] videoUrls = model.VideoUrl.Split('/');
                        if (videoUrls.Length > 0)
                        {
                            try
                            {
                                string mapUrl = $"{mapPath}/{videoUrls[videoUrls.Length - 1]}";
                                var player = new WindowsMediaPlayer();
                                var clip = player.newMedia(mapUrl);
                                double time = clip.duration;
                                entity.Minute = (int)(time / 60);
                            }
                            catch { }
                        }
                    }
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = model.ModifiedOn;
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static async Task<tbl_LessonVideo> UpdateV2(LessonVideoUpdateV2 model, tbl_UserInformation user, string mapPath)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == model.Id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    
                    entity.Name = model.Name ?? entity.Name;
                    entity.ExamId = model.ExamId ?? entity.ExamId;
                    entity.VideoUrl = model.VideoUrl ?? entity.VideoUrl;
                    entity.Type = model.Type ?? entity.Type;
                    entity.TypeName = model.TypeName ?? entity.TypeName;
                    entity.FileType = model.FileType ?? entity.FileType;
                    entity.Minute = model.Minute ?? entity.Minute;
                    entity.Thumbnail = model.Thumbnail ?? entity.Thumbnail;
                    if(entity.FileType == LessonFileType.AntiDownload)
                    {
                        var antiDownVideo = await db.tbl_AntiDownVideo.SingleOrDefaultAsync(x => x.Id == entity.AntiDownVideoId);
                        if (!string.IsNullOrEmpty(model.VideoUploadId))
                        {
                            /*var getIframe = await GetIframe(model.VideoUploadId);
                            if (getIframe == null)
                                throw new Exception("Có lỗi xảy ra trong quá trình tải video lên hệ thống");*/                         
                            if (antiDownVideo != null)
                            {
                                antiDownVideo.VideoUploadId = model.VideoUploadId;
                                antiDownVideo.Thumbnail = entity.Thumbnail;
                                antiDownVideo.Minute = entity.Minute;
                                antiDownVideo.VideoUrl = entity.VideoUrl;
                            }
                            else
                            {
                                antiDownVideo = new tbl_AntiDownVideo();
                                antiDownVideo.VideoUploadId = model.VideoUploadId;
                                antiDownVideo.Minute = entity.Minute;
                                antiDownVideo.Title = entity.Name;
                                antiDownVideo.Thumbnail = entity.Thumbnail;
                                antiDownVideo.VideoUrl = entity.VideoUrl;
                                antiDownVideo.Enable = true;
                                antiDownVideo.CreatedBy = user.FullName;
                                antiDownVideo.ModifiedBy = user.FullName;
                                antiDownVideo.ModifiedOn = DateTime.Now;
                                antiDownVideo.CreatedOn = DateTime.Now;
                                db.tbl_AntiDownVideo.Add(antiDownVideo);
                                await db.SaveChangesAsync();
                                entity.AntiDownVideoId = antiDownVideo.Id;
                                await db.SaveChangesAsync();
                            }                           
                            Thread pushnoti = new Thread(() =>
                                PushNotiUpdateVideo(entity.Id, user));
                            pushnoti.Start();
                        }
                        entity.VideoUploadId = antiDownVideo == null ? "" : antiDownVideo.VideoUploadId;
                    }
                    
                    if (entity.FileType == LessonFileType.FileUpload && !string.IsNullOrEmpty(model.VideoUrl))
                    {
                        ///đọc thời gian video tại đây
                        string[] videoUrls = model.VideoUrl.Split('/');
                        if (videoUrls.Length > 0)
                        {
                            try
                            {
                                string mapUrl = $"{mapPath}/{videoUrls[videoUrls.Length - 1]}";
                                var player = new WindowsMediaPlayer();
                                var clip = player.newMedia(mapUrl);
                                double time = clip.duration;
                                entity.Minute = (int)(time / 60);
                            }
                            catch { }
                        }
                        Thread pushnoti = new Thread(() =>
                                PushNotiUpdateVideo(entity.Id, user));
                        pushnoti.Start();
                    }
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = model.ModifiedOn;
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == id);
                        if (entity == null)
                            throw new Exception("Không tìm thấy dữ liệu");
                        entity.Enable = false;
                        await db.SaveChangesAsync();
                        //Cập nhật lại vi trí
                        var lessonVideos = await db.tbl_LessonVideo
                            .Where(x => x.SectionId == entity.SectionId && x.Enable == true && x.Index > entity.Index)
                            .ToListAsync();
                        if (lessonVideos.Any())
                        {
                            foreach (var item in lessonVideos)
                            {
                                var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == item.Id);
                                lessonVideo.Index -= 1;
                                await db.SaveChangesAsync();
                            }
                        }
                        var lessonCompleteds = await db.tbl_LessonCompleted.Where(x => x.LessonVideoId == id)
                            .Select(x => x.Id).ToListAsync();
                        if (lessonCompleteds.Any())
                        {
                            foreach (var item in lessonCompleteds)
                            {
                                var lessonCompleted = await db.tbl_LessonCompleted.SingleOrDefaultAsync(x => x.Id == item);
                                lessonCompleted.Enable = false;
                                await db.SaveChangesAsync();
                                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == lessonCompleted.UserId);
                                await CompletedSection(db, entity.SectionId.Value, user);
                            }
                        }
                        if (entity.FileType == LessonFileType.AntiDownload)
                        {
                            var saveUploadVideo = await db.tbl_AntiDownVideo.SingleOrDefaultAsync(x => x.Id == entity.AntiDownVideoId);
                            if (saveUploadVideo != null)
                            {
                                var httpClient = new HttpClient();
                                // Xóa video trên server
                                var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"https://app-api.mona.host/v1/videos/{saveUploadVideo.VideoUploadId}");
                                deleteRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                deleteRequest.Headers.Add("X-MONA-KEY", serverDownload_Api_Key);

                                var deleteResponse = await httpClient.SendAsync(deleteRequest);
                                if (!deleteResponse.IsSuccessStatusCode)
                                {
                                    var deleteErrorContent = await deleteResponse.Content.ReadAsStringAsync();
                                    throw new Exception("Xóa video thất bại!");
                                }
                            }                                                    
                        }
                        tran.Commit();

                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public class ChangeLessonIndexModel
        {
            public List<ChangeLessonIndexItem> Items { get; set; }
        }
        public class ChangeLessonIndexItem
        {
            public int Id { get; set; }
            public int Index { get; set; }
            public int SectionId { get; set; }
        }
        public static async Task ChangeIndex(ChangeLessonIndexModel model)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!model.Items.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model.Items)
                        {
                            var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == item.Id);
                            if (lessonVideo == null)
                                throw new Exception("Không tìm thấy dữ liệu");
                            lessonVideo.Index = item.Index;
                            if (lessonVideo.SectionId != item.SectionId && item.SectionId != 0)
                            {
                                lessonVideo.SectionId = item.SectionId;
                                var lessonInSS = await db.tbl_LessonVideo
                                    .Where(x => x.SectionId == item.SectionId && x.Enable == true)
                                    .OrderBy(x => x.Index).ToListAsync();
                                if (lessonInSS.Any())
                                {
                                    int index = 1;
                                    foreach (var ls in lessonInSS)
                                    {
                                        ls.Index = index;
                                        index++;
                                    }
                                }
                            }
                            await db.SaveChangesAsync();
                        }
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }

            }
        }
        public static async Task<List<LessonVideoModel>> GetBySection(int sectionId, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var completeds = await db.tbl_LessonCompleted
                    .Where(x => x.SectionId == sectionId && x.UserId == user.UserInformationId)
                    .Select(x => x.LessonVideoId).Distinct().ToListAsync();
                var data = await db.tbl_LessonVideo.Where(x => x.SectionId == sectionId && x.Enable == true).OrderBy(x => x.Index).ToListAsync();
                var dataIds = data.Select(x => x.AntiDownVideoId).ToList();
                var antiDownVideos = await db.tbl_AntiDownVideo.Where(x => x.Enable == true && dataIds.Contains(x.Id)).ToListAsync();
                if (antiDownVideos == null)
                {
                    antiDownVideos = new List<tbl_AntiDownVideo>();
                }
                var result = (from i in data
                              join completed in completeds on i.Id equals completed into pg
                              from completed in pg.DefaultIfEmpty()
                              join antiDownVideo in antiDownVideos on i.AntiDownVideoId equals antiDownVideo.Id into s
                              from antiDownVideo in s.DefaultIfEmpty()
                              select new LessonVideoModel
                              {
                                  ExamId = i.ExamId,
                                  Id = i.Id,
                                  Index = i.Index,
                                  Name = i.Name,
                                  SectionId = i.SectionId,
                                  Type = i.Type,
                                  TypeName = i.TypeName,
                                  VideoUrl = i.VideoUrl,
                                  isCompleted = completed.HasValue ? true : false,
                                  FileType = i.FileType,
                                  Minute = i.Minute,
                                  Thumbnail = i.Thumbnail,
                                  AntiDownVideoId = antiDownVideo == null ? 0 : antiDownVideo.Id,
                                  VideoUploadId = antiDownVideo == null ? "" : antiDownVideo.VideoUploadId,
                                  HasFile = Task.Run(() => GetHasFile(i.Id)).Result
                              }).ToList();
                return result;
            }
        }
        public static async Task<bool> GetHasFile(int id)
        {
            using (var db = new lmsDbContext())
            {
                var result = await db.tbl_FileInVideo.AnyAsync(x => x.LessonVideoId == id && x.Enable == true);
                return result;
            }
        }
        /// <summary>
        /// Đối với bài kiểm tra sẽ lưu lại điểm - hoàn thành theo thứ tự
        /// </summary>
        /// <param name="lessonVideoId"></param>
        /// <param name="user"></param>
        /// <param name="examResultId"></param>
        /// <param name="totalPoint"></param>
        /// <returns></returns>
        //public static async Task Completed(int lessonVideoId, tbl_UserInformation user, int examResultId = 0, double totalPoint = 0)
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        using (var tran = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == lessonVideoId);
        //                if (lessonVideo == null)
        //                    throw new Exception("Không tìm thấy nội dung bài học");
        //                ///Phải hoàn thành bài trước đó
        //                if (lessonVideo.Index != 1)
        //                {
        //                    var obligatoryCompleted = await db.tbl_LessonVideo
        //                        .Where(x => x.SectionId == lessonVideo.SectionId && x.Index == (lessonVideo.Index - 1) && x.Enable == true)
        //                        .FirstOrDefaultAsync();
        //                    if (obligatoryCompleted != null)
        //                    {
        //                        var validateIndex = await db.tbl_LessonCompleted.AnyAsync(x => x.LessonVideoId == obligatoryCompleted.Id && x.UserId == user.UserInformationId);
        //                        if (!validateIndex)
        //                            throw new Exception("Chưa học đến bài này");
        //                    }
        //                }
        //                var validate = await db.tbl_LessonCompleted.AnyAsync(x => x.LessonVideoId == lessonVideoId && x.UserId == user.UserInformationId);
        //                if (validate)
        //                    return;
        //                var model = new tbl_LessonCompleted
        //                {
        //                    CreatedBy = user.FullName,
        //                    CreatedOn = DateTime.Now,
        //                    Enable = true,
        //                    LessonVideoId = lessonVideoId,
        //                    ExamResultId = examResultId,
        //                    TotalPoint = totalPoint,
        //                    ModifiedBy = user.FullName,
        //                    ModifiedOn = DateTime.Now,
        //                    SectionId = lessonVideo.SectionId,
        //                    UserId = user.UserInformationId
        //                };
        //                db.tbl_LessonCompleted.Add(model);
        //                //await CompletedSection(model.SectionId.Value, user);
        //                await db.SaveChangesAsync();
        //                var section = await db.tbl_Section.SingleOrDefaultAsync(x => x.Id == model.SectionId.Value);
        //                var sectionCompled = await db.tbl_SectionCompleted.AnyAsync(x => x.SectionId == section.Id && x.UserId == user.UserInformationId && x.Enable == true);
        //                if (section != null && !sectionCompled)
        //                {
        //                    bool completed = true;
        //                    var lessonVideos = await db.tbl_LessonVideo.Where(x => x.SectionId == section.Id && x.Enable == true).ToListAsync();
        //                    if (lessonVideos.Any())
        //                    {
        //                        foreach (var item in lessonVideos)
        //                        {
        //                            var lessonCompleted = await db.tbl_LessonCompleted.AnyAsync(x => x.LessonVideoId == item.Id && x.UserId == user.UserInformationId && x.Enable == true);
        //                            if (!lessonCompleted)
        //                                completed = false;
        //                        }
        //                    }
        //                    if (completed)
        //                    {
        //                        db.tbl_SectionCompleted.Add(new tbl_SectionCompleted
        //                        {
        //                            CreatedBy = user.FullName,
        //                            CreatedOn = DateTime.Now,
        //                            Enable = true,
        //                            ModifiedBy = user.FullName,
        //                            ModifiedOn = DateTime.Now,
        //                            SectionId = model.SectionId.Value,
        //                            VideoCourseId = section.VideoCourseId,
        //                            UserId = user.UserInformationId
        //                        });
        //                        await db.SaveChangesAsync();
        //                    }
        //                }
        //                tran.Commit();
        //            }
        //            catch (Exception e)
        //            {
        //                tran.Rollback();
        //                throw e;
        //            }
        //        }        
        //    }
        //}
        /// <summary>
        /// Đối với bài kiểm tra sẽ lưu lại điểm - hoàn thành không theo thứ tự
        /// </summary>
        /// <param name="lessonVideoId"></param>
        /// <param name="user"></param>
        /// <param name="examResultId"></param>
        /// <param name="totalPoint"></param>
        /// <returns></returns>
        public static async Task Completed(lmsDbContext dbContext, int lessonVideoId, tbl_UserInformation user, int examResultId = 0, double totalPoint = 0)
        {
            try
            {
                var lessonVideo = await dbContext.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == lessonVideoId);
                if (lessonVideo == null)
                    return;

                var validate = await dbContext.tbl_LessonCompleted.AnyAsync(x => x.LessonVideoId == lessonVideoId && x.UserId == user.UserInformationId);
                if (validate)
                    return; //đã hoàn thành
                var model = new tbl_LessonCompleted
                {
                    CreatedBy = user.FullName,
                    CreatedOn = DateTime.Now,
                    Enable = true,
                    LessonVideoId = lessonVideoId,
                    ExamResultId = examResultId,
                    TotalPoint = totalPoint,
                    ModifiedBy = user.FullName,
                    ModifiedOn = DateTime.Now,
                    SectionId = lessonVideo.SectionId,
                    UserId = user.UserInformationId
                };
                dbContext.tbl_LessonCompleted.Add(model);
                await dbContext.SaveChangesAsync();

                var section = await dbContext.tbl_Section.SingleOrDefaultAsync(x => x.Id == model.SectionId);
                var videoCourseStudent = await dbContext.tbl_VideoCourseStudent
                    .FirstOrDefaultAsync(x => x.VideoCourseId == section.VideoCourseId && x.UserId == user.UserInformationId && x.Enable == true);
                if(videoCourseStudent != null)
                    videoCourseStudent.CompletedPercent = await GetComplete(dbContext, section.VideoCourseId.Value, user);

                await dbContext.SaveChangesAsync();
                await CompletedSection(dbContext, lessonVideo.SectionId.Value, user);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task CompletedSection(lmsDbContext dbContext,int sectionId, tbl_UserInformation user)
        {
            var section = await dbContext.tbl_Section.SingleOrDefaultAsync(x => x.Id == sectionId);
            if (section != null)
            {
                bool completed = true;
                var lessonVideos = await dbContext.tbl_LessonVideo.Where(x => x.SectionId == section.Id && x.Enable == true)
                    .Select(x=>x.Id).ToListAsync();
                if (lessonVideos.Any())
                {
                    foreach (var item in lessonVideos)
                    {
                        var lessonCompleted = await dbContext.tbl_LessonCompleted
                            .AnyAsync(x => x.LessonVideoId == item && x.UserId == user.UserInformationId && x.Enable == true);
                        if (!lessonCompleted)
                            completed = false;
                    }
                }
                if (completed)
                {
                    dbContext.tbl_SectionCompleted.Add(new tbl_SectionCompleted
                    {
                        CreatedBy = user.FullName,
                        CreatedOn = DateTime.Now,
                        Enable = true,
                        ModifiedBy = user.FullName,
                        ModifiedOn = DateTime.Now,
                        SectionId = sectionId,
                        VideoCourseId = section.VideoCourseId,
                        UserId = user.UserInformationId
                    });
                }
                else
                {
                    var sectionCompleted = await dbContext.tbl_SectionCompleted
                        .FirstOrDefaultAsync(x => x.SectionId == sectionId && x.VideoCourseId == section.VideoCourseId && x.UserId == user.UserInformationId && x.Enable == true);
                    if (sectionCompleted != null)
                        sectionCompleted.Enable = false;
                }
                await dbContext.SaveChangesAsync();

                var videoCourseStudent = await dbContext.tbl_VideoCourseStudent
                    .FirstOrDefaultAsync(x => x.VideoCourseId == section.VideoCourseId && x.UserId == user.UserInformationId && x.Enable == true);
                if (videoCourseStudent != null)
                    videoCourseStudent.CompletedPercent = await GetComplete(dbContext, section.VideoCourseId.Value, user);
                await dbContext.SaveChangesAsync();
                await CompletedVideoCourse(dbContext, section.VideoCourseId.Value, user);
            }
        }
        public static async Task CompletedVideoCourse(lmsDbContext dbContext, int videoCourseId, tbl_UserInformation user)
        {
            var videoCourseStudent = await dbContext.tbl_VideoCourseStudent
                .FirstOrDefaultAsync(x => x.VideoCourseId == videoCourseId && x.UserId == user.UserInformationId && x.Enable == true);
            if (videoCourseStudent != null)
            {
                bool completed = true;
                var sections = await dbContext.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true)
                    .Select(x => x.Id).ToListAsync();
                if (sections.Any())
                {
                    foreach (var item in sections)
                    {
                        var sectionCompleted = await dbContext.tbl_SectionCompleted
                            .AnyAsync(x => x.SectionId == item && x.UserId == user.UserInformationId && x.Enable == true);
                        if (!sectionCompleted)
                            completed = false;
                    }
                }
                videoCourseStudent.CompletedPercent = await GetComplete(dbContext, videoCourseId, user);
                if (completed)
                {
                    videoCourseStudent.Status = 3;
                    videoCourseStudent.StatusName = "Hoàn thành";

                    //Cấp chứng chỉ
                    await CertificateService.CreateCertificate(dbContext, videoCourseId, user.UserInformationId);
                }
                else
                {
                    videoCourseStudent.Status = 2;
                    videoCourseStudent.StatusName = "Đang học";
                }
                await dbContext.SaveChangesAsync();
            }
        }
        public static async Task<double> GetComplete(lmsDbContext dbContext,int videoCourseId, tbl_UserInformation user)
        {
            double result = 0;
            double completed = 0;
            double total = 0;
            var section = await dbContext.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true)
                .Select(x => x.Id).Distinct().ToListAsync();
            if (section.Any())
            {
                foreach (var item in section)
                {
                    var lesson = await dbContext.tbl_LessonVideo.Where(x => x.SectionId == item && x.Enable == true)
                        .Select(x => x.Id).Distinct().ToListAsync();
                    total += lesson.Count();
                    completed += await dbContext.tbl_LessonCompleted
                        .Where(x => x.SectionId == item && x.UserId == user.UserInformationId && x.Enable == true && lesson.Contains(x.LessonVideoId ?? 0))
                        .Select(x => x.LessonVideoId).Distinct().CountAsync();
                }
                if (completed == 0)
                    return 0;
                result = (completed * 100) / total;
            }
            return Math.Round(result, 0);
        }
        //public static async Task TestCompleteVideo()
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        var data = await db.tbl_VideoCourseStudent.Where(x => x.Enable == true).ToListAsync();
        //        if(data.Any())
        //        {
        //            foreach (var item in data)
        //            {
        //                var user = await db.tbl_UserInformation.SingleOrDefaultAsync(x => x.UserInformationId == item.UserId);
        //                await CompletedVideoCourse(db, item.VideoCourseId.Value, user);
        //            }
        //        }
        //    }
        //}
        public class SaveTimeWatchingVideoModel
        {
            public int LessonVideoId { get; set; }
            /// <summary>
            /// Tổng thời gian đã xem
            /// </summary>
            public double TotalSecond { get; set; }
        }
        public static async Task SaveTimeWatchingVideo(SaveTimeWatchingVideoModel itemModel, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_TimeWatchingVideo
                    .FirstOrDefaultAsync(x => x.LessonVideoId == itemModel.LessonVideoId && x.UserId == userLog.UserInformationId && x.Enable == true);
                if (data == null)
                {
                    data = new tbl_TimeWatchingVideo
                    {
                        CreatedBy = userLog.FullName,
                        Enable = true,
                        CreatedOn = DateTime.Now,
                        LessonVideoId = itemModel.LessonVideoId,
                        ModifiedBy = userLog.ModifiedBy,
                        ModifiedOn = DateTime.Now,
                        TotalSecond = itemModel.TotalSecond,
                        UserId = userLog.UserInformationId
                    };
                    db.tbl_TimeWatchingVideo.Add(data);
                }
                else
                {
                    data.TotalSecond = itemModel.TotalSecond;
                }
                await db.SaveChangesAsync();
            }
        }
        public static async Task<tbl_TimeWatchingVideo> GetTimeWatchingVideo(int lessonVideoId, tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_TimeWatchingVideo
                    .FirstOrDefaultAsync(x => x.LessonVideoId == lessonVideoId && x.UserId == userLog.UserInformationId && x.Enable == true);
                return data;
            }
        }
    }
}