using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class FileInVideoService
    {
        public static void PushNotiNewFile(int lessonId, tbl_UserInformation userLog)
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
                        string title = "Thông báo tài liệu mới";
                        string content = $"Bài học {lesson.Name} trong khóa {videoCourse.Name} đã được thêm tài liệu mới, vui lòng truy cập {href} để học";
                        string onesignalContent = $"Bài học {lesson.Name} trong khóa {videoCourse.Name} đã được thêm tài liệu mới, vui lòng kiểm tra lại thông tin";

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
        public static async Task<tbl_FileInVideo> Insert(FileInVideoCreate fileInVideoCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Id == fileInVideoCreate.LessonVideoId);
                    if (lessonVideo == null)
                        throw new Exception("Không tìm thấy bài học");
                    var model = new tbl_FileInVideo(fileInVideoCreate);
                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    db.tbl_FileInVideo.Add(model);
                    await db.SaveChangesAsync();
                    Thread pushnoti = new Thread(() =>
                        PushNotiNewFile(lessonVideo.Id, user));
                    pushnoti.Start();
                    return model;
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
                try
                {
                    var fileInVideo = await db.tbl_FileInVideo.SingleOrDefaultAsync(x => x.Id == id);
                    if (fileInVideo == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    fileInVideo.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<List<tbl_FileInVideo>> GetByLesson(int lessonVideoId)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_FileInVideo.Where(x => x.LessonVideoId == lessonVideoId && x.Enable == true).OrderBy(x => x.Id).ToListAsync();
                return data;
            }
        }
    }
}