using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using PuppeteerSharp.PageCoverage;
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

namespace LMS_Project.Services
{
    public class DashboardService
    {
        #region dashboard V2
        /// <summary>
        /// Api lấy tổng số lượng nhân viên.
        /// </summary>
        /// <returns></returns>
        /*public static async Task<List<CountModel>> OverviewModelV2()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                var ListStaff = await db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync();

                //số lượng nhân viên
                var CountStaff = ListStaff.Count;
                var TotalAndroid = ListStaff.Where(x => x.Device == 1).Count();
                var TotalIOS = ListStaff.Where(x => x.Device == 2).Count();
                var TotalComputer = ListStaff.Where(x => x.Device == 3).Count();

                data.Add(new CountModel { Type = "Nhân viên", Count = CountStaff });
                data.Add(new CountModel { Type = "Android", Count = TotalAndroid });
                data.Add(new CountModel { Type = "IOS", Count = TotalIOS });
                data.Add(new CountModel { Type = "Máy tính", Count = TotalComputer });
                return data;
            }
        }*/

        /// <summary>
        /// Api lấy: số nv sử dụng điện android, số nv sử dụng ios, số nv sử dụng máy tính (cái này api update thông tin, cho thêm 1 field "device", 
        /// FE truyền lên cho. Cho thêm cái "note" để ngài Chao múa nha).
        /// </summary>
        /*public static async Task<List<CountModel>> TotalDevice()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                var ListStaff = await db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync();
                var TotalAndroid = ListStaff.Where(x => x.Device == 1).Count();
                var TotalIOS = ListStaff.Where(x => x.Device == 2).Count();
                var TotalComputer = ListStaff.Where(x => x.Device == 3).Count();

                data.Add(new CountModel { Type = "Android", Count = TotalAndroid });
                data.Add(new CountModel { Type = "IOS", Count = TotalIOS });
                data.Add(new CountModel { Type = "Máy tính", Count = TotalComputer });
                return data;
            }
        }*/

        /// <summary>
        /// Api lấy số nhân viên đã hoàn thành hết các khoá học trong danh sách khoá học của nó, số nv không hoàn thành.
        /// </summary>
        public static async Task<List<CountModel>> TotalStaffCompleteAndNotCompleteCourse()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                var ListStaff = await db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync();

                //số nhân viên đã và chưa hoàn thành hết tất cả khóa học
                var TotalStaffCompletedCourse = 0;
                var TotalStaffNotCompletedCourse = 0;
                if (ListStaff.Count > 0)
                {
                    foreach (var staff in ListStaff)
                    {
                        bool IsCompleted = true;
                        if (staff.DepartmentId.HasValue)
                        {
                            var studyRoute = await db.tbl_StudyRoute.SingleOrDefaultAsync(x => x.Enable == true && x.DepartmentId == staff.DepartmentId);
                            if (studyRoute != null)
                            {
                                var listVideoCourseId = await db.tbl_StudyRouteDetail.Where(x => x.Enable == true && x.StudyRouteId == studyRoute.Id).Select(x => x.VideoCourseId).ToListAsync();
                                if (listVideoCourseId.Count > 0)
                                {
                                    foreach (var videoCourseId in listVideoCourseId)
                                    {
                                        var checkCompleted = await db.tbl_VideoCourseStudent.SingleOrDefaultAsync(x => x.Enable == true && x.VideoCourseId == videoCourseId && x.UserId == staff.UserInformationId && x.Status == 3);
                                        if (checkCompleted == null)
                                        {
                                            IsCompleted = false;
                                            break;
                                        }
                                    }
                                    if (IsCompleted == false)
                                    {
                                        TotalStaffNotCompletedCourse += 1;
                                    }
                                    else
                                    {
                                        TotalStaffCompletedCourse += 1;
                                    }

                                }
                            }
                        }
                    }
                }
                data.Add(new CountModel { Type = "Nhân viên hoàn thành khóa học", Count = TotalStaffCompletedCourse });
                data.Add(new CountModel { Type = "Nhân viên không hoàn thành khóa học", Count = TotalStaffNotCompletedCourse });
                return data;
            }
        }

        /// <summary>
        /// Api lấy số nhân viên có tổng thời gian học dưới 5 tiếng, số nhân viên học trên 5 tiếng (có thể cộng thời gian tất cả các video nó đã hoàn thành).
        /// </summary>
        public static async Task<List<CountModel>> TotalStaffStudyMoreOrLessThan5Hour()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                var ListStaff = await db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync();

                //số nhân viên có tổng thời gian học dưới 5h và trên 5h
                var TotalStaffStudyMore5Hour = 0;
                var TotalStaffStudyLess5Hour = 0;
                if (ListStaff.Count > 0)
                {
                    foreach (var staff in ListStaff)
                    {
                        double totalMinute = 0;
                        var ListVideoCompleted = await db.tbl_LessonCompleted.Where(x => x.Enable == true && x.ExamResultId == 0 && x.TotalPoint == 0 && x.UserId == staff.UserInformationId).Select(x => x.LessonVideoId).ToListAsync();
                        if (ListVideoCompleted.Count > 0)
                        {
                            foreach (var lessonVideoId in ListVideoCompleted)
                            {
                                var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Enable == true && x.Id == lessonVideoId);
                                if(lessonVideo != null)
                                {
                                    totalMinute += lessonVideo.Minute;
                                }
                            }
                        }
                        double totalHour = totalMinute / 60;
                        if (totalHour >= 5)
                            TotalStaffStudyMore5Hour += 1;
                        else
                            TotalStaffStudyLess5Hour += 1;
                    }
                }

                data.Add(new CountModel { Type = "Số nhân viên học trên 5h", Count = TotalStaffStudyMore5Hour });
                data.Add(new CountModel { Type = "Số nhân viên học dưới 5h", Count = TotalStaffStudyLess5Hour });
                return data;
            }
        }

        /// <summary>
        /// Api lấy trung bình thời gian mỗi nhân viên xem video (như ở trên).
        /// </summary>
        public static async Task<List<AverageModel>> AverageVideoViewingTime()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<AverageModel>();
                var ListStaff = await db.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync();

                double TotalStaff = 0;
                double TotalHour = 0;
                if (ListStaff.Count > 0)
                {
                    foreach (var staff in ListStaff)
                    {
                        double totalMinute = 0;
                        var ListVideoCompleted = await db.tbl_LessonCompleted.Where(x => x.Enable == true && x.ExamResultId == 0 && x.TotalPoint == 0 && x.UserId == staff.UserInformationId).Select(x => x.LessonVideoId).ToListAsync();
                        if (ListVideoCompleted.Count > 0)
                        {
                            foreach (var lessonVideoId in ListVideoCompleted)
                            {
                                var lessonVideo = await db.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Enable == true && x.Id == lessonVideoId);
                                if (lessonVideo != null)
                                {
                                    totalMinute += lessonVideo.Minute;
                                }
                            }
                        }
                        double totalHour = Math.Round((totalMinute / 60), 4);
                        TotalStaff += 1;
                        TotalHour += totalHour;
                    }
                }
                double average = 0;
                if (TotalStaff > 0)
                    average = Math.Round((TotalHour / TotalStaff), 2);
                else
                    average = 0;
                data.Add(new AverageModel { Type = "Trung bình thời gian xem video", Average = average });
                return data;
            }
        }

        /// <summary>
        /// Api lấy trung bình mỗi video được xem bao nhiêu lần (lấy số nhân viên hoàn thành mỗi video rồi chia cho tổng số video nhe em yếu).
        /// </summary>
        public static async Task<List<AverageModel>> AverageVideoViewingViews()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<AverageModel>();
                double TotalLessonVideo = await db.tbl_LessonVideo.Where(x => x.Enable == true && x.Type == 1).CountAsync();
                if(TotalLessonVideo <= 0)
                {
                    data.Add(new AverageModel { Type = "Trung bình lượt xem video", Average = 0 });
                    return data;
                }
                double TotalLessonVideoComplete = await db.tbl_LessonCompleted.Where(x => x.Enable == true && x.ExamResultId == 0 && x.TotalPoint == 0).CountAsync();
                
                double average = Math.Round((TotalLessonVideoComplete / TotalLessonVideo), 2);
                data.Add(new AverageModel { Type = "Trung bình lượt xem video", Average = average });
                return data;
            }
        }
        /// <summary>
        /// Api lấy (số lượt xem, tên, khoá) của video có lượt xem cao nhất và thấp nhất.
        /// </summary>
        public class CountViewModel
        {
            public string Type { get; set; }
            public string LessonVideoName { get; set; }
            public string VideoCourseName { get; set; }
            public int Count { get; set; }
        }
        public static async Task<List<CountViewModel>> CountViewOfVideo()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountViewModel>();
                var lessonVideoWithMaxViews = await db.tbl_LessonVideo
                    .Where(x => x.Enable == true && x.Type == 1 &&
                                db.tbl_Section.Any(s => s.Enable == true && s.Id == x.SectionId &&
                                                          db.tbl_VideoCourse.Any(vc => vc.Enable == true && vc.Id == s.VideoCourseId)))
                    .Select(lessonVideo => new
                    {
                        LessonVideoName = lessonVideo.Name,
                        TotalView = db.tbl_LessonCompleted
                            .Where(completed => completed.Enable == true && completed.LessonVideoId == lessonVideo.Id)
                            .Count(),
                        VideoCourseName = db.tbl_Section
                            .Where(s => s.Id == lessonVideo.SectionId)
                            .Select(s => db.tbl_VideoCourse
                                .Where(vc => vc.Id == s.VideoCourseId && vc.Enable == true)
                                .Select(vc => vc.Name)
                                .FirstOrDefault())
                            .FirstOrDefault()
                    })
                    .OrderByDescending(result => result.TotalView)
                    .FirstOrDefaultAsync();

                var lessonVideoName = lessonVideoWithMaxViews?.LessonVideoName ?? "";
                var views = lessonVideoWithMaxViews?.TotalView ?? 0;
                var videoCourseName = lessonVideoWithMaxViews?.VideoCourseName ?? "";        
                
                data.Add(new CountViewModel { Type = "Lượt xem cao nhất", LessonVideoName = lessonVideoName, VideoCourseName = videoCourseName, Count = views });

                var lessonVideoWithMinViews = await db.tbl_LessonVideo
                    .Where(x => x.Enable == true && x.Type == 1 &&
                                db.tbl_Section.Any(s => s.Enable == true && s.Id == x.SectionId &&
                                                          db.tbl_VideoCourse.Any(vc => vc.Enable == true && vc.Id == s.VideoCourseId)))
                    .Select(lessonVideo => new
                    {
                        LessonVideoName = lessonVideo.Name,
                        TotalView = db.tbl_LessonCompleted
                            .Where(completed => completed.Enable == true && completed.LessonVideoId == lessonVideo.Id)
                            .Count(),
                        VideoCourseName = db.tbl_Section
                            .Where(s => s.Id == lessonVideo.SectionId)
                            .Select(s => db.tbl_VideoCourse
                                .Where(vc => vc.Id == s.VideoCourseId && vc.Enable == true)
                                .Select(vc => vc.Name)
                                .FirstOrDefault())
                            .FirstOrDefault()
                    })
                    .OrderBy(result => result.TotalView)
                    .FirstOrDefaultAsync();

                lessonVideoName = lessonVideoWithMinViews?.LessonVideoName ?? "";
                views = lessonVideoWithMinViews?.TotalView ?? 0;
                videoCourseName = lessonVideoWithMinViews?.VideoCourseName ?? "";

                data.Add(new CountViewModel { Type = "Lượt xem thấp nhất", LessonVideoName = lessonVideoName, VideoCourseName = videoCourseName, Count = views });
                return data;
            }
        }
        #endregion

        public class CountModel
        {
            public string Type { get; set; }
            public int Count { get; set; }
        }
        public class AverageModel
        {
            public string Type { get; set; }
            public double Average { get; set; }
        }
        #region Thống kê học viên
        public static async Task<List<CountModel>> OverviewModel()
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                ///
                var CountStudent = await db.tbl_UserInformation.CountAsync(x => x.RoleId == ((int)RoleEnum.student) && x.Enable == true);
                var CountCourse = await db.tbl_VideoCourse.CountAsync(x => x.Enable == true);
                var CountCertificate = await db.tbl_Certificate.CountAsync(x => x.Enable == true);
                var HoiThaoChuaDienRa = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.ChuaDienRa && x.Enable == true);
                var HoiThaoDangDienRa = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.DangDienRa && x.Enable == true);
                var HoiThaoDaKetThuc = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.KetThuc && x.Enable == true);

                data.Add(new CountModel { Type = "Học viên", Count = CountStudent });
                data.Add(new CountModel { Type = "Khóa học", Count = CountCourse });
                data.Add(new CountModel { Type = "Chứng chỉ", Count = CountCertificate });
                //data.Add(new CountModel { Type = "Webinar chưa diễn ra", Count = HoiThaoChuaDienRa });
                //data.Add(new CountModel { Type = "Webinar đang diễn ra", Count = HoiThaoDangDienRa });
                //data.Add(new CountModel { Type = "Webinar đã kết thúc", Count = HoiThaoDaKetThuc });
                return data;

            }
        }

        //Danh sách thống kê mà giáo viên có thể xem
        public static async Task<List<CountModel>> OverviewModelForTeacher(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                ///
                var countStudent = await db.tbl_UserInformation.CountAsync(x => x.RoleId == ((int)RoleEnum.student) && x.Enable == true);
                var hoiThaoChuaDienRa = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.ChuaDienRa && x.Enable == true && x.LeaderId == user.UserInformationId);
                var hoiThaoDangDienRa = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.DangDienRa && x.Enable == true && x.LeaderId == user.UserInformationId);
                var hoiThaoDaKetThuc = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.KetThuc && x.Enable == true && x.LeaderId == user.UserInformationId);
                var hoiThaoCuaGV = await db.tbl_Seminar.CountAsync(x => x.LeaderId == user.UserInformationId && x.Enable == true && x.LeaderId == user.UserInformationId);

                data.Add(new CountModel { Type = "Học viên", Count = countStudent });
                //data.Add(new CountModel { Type = "Webinar chưa diễn ra", Count = hoiThaoChuaDienRa });
                //data.Add(new CountModel { Type = "Webinar đang diễn ra", Count = hoiThaoDangDienRa });
                //data.Add(new CountModel { Type = "Webinar đã kết thúc", Count = hoiThaoDaKetThuc });
                //data.Add(new CountModel { Type = "Tổng Webinar của tôi", Count = hoiThaoCuaGV });

                ///
                return data;

            }
        }

        public class CountModelInMonth
        {
            public string Type { get; set; }
            public int Count { get; set; }
            public string Note { get; set; }
        }

        //Thống kê theo tháng
        public static async Task<List<CountModelInMonth>> OverviewModelInMonth()
        {
            var data = new List<CountModelInMonth>();
            using (var db = new lmsDbContext())
            {
                var CountStudentInMonth = await db.tbl_UserInformation.CountAsync(x => x.RoleId == ((int)RoleEnum.student) && x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month);
                var CountStudentPreMonth = await db.tbl_UserInformation.CountAsync(x => x.RoleId == ((int)RoleEnum.student) && x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month - 1);
                var CountCertificateInMonth = await db.tbl_Certificate.CountAsync(x => x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month);
                var CountCertificatePreMonth = await db.tbl_Certificate.CountAsync(x => x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month - 1);
                var CountCourse = await db.tbl_VideoCourse.CountAsync(x => x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month);
                var HoiThaoChuaDienRa = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.ChuaDienRa && x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month);
                var HoiThaoDangDienRa = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.DangDienRa && x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month);
                var HoiThaoDaKetThuc = await db.tbl_Seminar.CountAsync(x => x.Status == (int)SeminarStatus.KetThuc && x.Enable == true && x.CreatedOn.Value.Month == DateTime.Now.Month);
                double StatisticStudent = Math.Abs(CountStudentInMonth - CountStudentPreMonth);
                double StatisticCertificate = Math.Abs(CountCertificateInMonth - CountCertificatePreMonth);
                if (CountStudentInMonth < CountStudentPreMonth || CountCertificateInMonth < CountCertificatePreMonth)
                {
                    data.Add(new CountModelInMonth { Type = "Học viên mới trong tháng", Count = CountStudentInMonth, Note = $"Giảm {StatisticStudent} học viên so với tháng trước" });
                    data.Add(new CountModelInMonth { Type = "Chứng chỉ mới trong tháng", Count = CountCertificateInMonth, Note = $"Giảm {StatisticCertificate} chứng chỉ so với tháng trước" });
                }
                else
                {
                    data.Add(new CountModelInMonth { Type = "Học viên mới trong tháng", Count = CountStudentInMonth, Note = StatisticStudent == 0 ? "" : $"Tăng {StatisticStudent} học viên so với tháng trước" });
                    data.Add(new CountModelInMonth { Type = "Chứng chỉ mới trong tháng", Count = CountCertificateInMonth, Note = StatisticCertificate == 0 ? "" : $"Tăng {StatisticCertificate} chứng chỉ so với tháng trước" });
                }
                return data;
            }

        }
        public class CountAgeStudent
        {
            /// <summary>
            /// 2 - tên loại thống kê
            /// 3 - số lượng trên hệ thống
            /// </summary>
            public int Type { get; set; }
            public string Note { get; set; }
            public int Count { get; set; }
        }

        //Thống kê học viên theo độ tuổi
        public static async Task<List<CountAgeStudent>> StatisticForAge()
        {
            var data = new List<CountAgeStudent>();
            using (var db = new lmsDbContext())
            {
                var model = new tbl_UserInformation();
                var Under18 = await (from age in db.tbl_UserInformation where (DateTime.Now.Year - age.DOB.Value.Year < 18 && age.Enable == true && age.RoleId == (int)RoleEnum.student) select age.DOB).CountAsync();
                var Over18 = await (from age in db.tbl_UserInformation where ((DateTime.Now.Year - age.DOB.Value.Year) >= 18 && (DateTime.Now.Year - age.DOB.Value.Year) < 25 && age.Enable == true && age.RoleId == (int)RoleEnum.student) select age.DOB).CountAsync();
                var Over25 = await (from age in db.tbl_UserInformation where ((DateTime.Now.Year - age.DOB.Value.Year) >= 25 && (DateTime.Now.Year - age.DOB.Value.Year) < 35 && age.Enable == true && age.RoleId == (int)RoleEnum.student) select age.DOB).CountAsync();
                var Over35 = await (from age in db.tbl_UserInformation where ((DateTime.Now.Year - age.DOB.Value.Year) >= 35 && (DateTime.Now.Year - age.DOB.Value.Year) < 45 && age.Enable == true && age.RoleId == (int)RoleEnum.student) select age.DOB).CountAsync();
                var Over45 = await (from age in db.tbl_UserInformation where ((DateTime.Now.Year - age.DOB.Value.Year) >= 45 && age.Enable == true && age.RoleId == (int)RoleEnum.student) select age.DOB).CountAsync();
                data.Add(new CountAgeStudent { Type = 1, Note = "Học viên dưới 18", Count = Under18 });
                data.Add(new CountAgeStudent { Type = 2, Note = "Học viên từ 18 đến 25", Count = Over18 });
                data.Add(new CountAgeStudent { Type = 3, Note = "Học viên từ 25 đến 35", Count = Over25 });
                data.Add(new CountAgeStudent { Type = 4, Note = "Học viên từ 35 đến 45", Count = Over35 });
                data.Add(new CountAgeStudent { Type = 5, Note = "Học viên trên 45", Count = Over45 });
                return data;
            }
        }
        public class Get_CountTopStudentInCourse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Total { get; set; }
        }

        //Thống kê top 5 khóa học có nhiều học viên nhất
        public static async Task<List<Get_CountTopStudentInCourse>> StatisticTopCourse()
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_CountTopCouseStudent";
                var data = await db.Database.SqlQuery<Get_CountTopStudentInCourse>(sql).ToListAsync();
                return data;
            }
        }

        public class Get_CompleteCousre
        {
            public int Id { get; set; }
        }
        #endregion
        //Thống kê khóa học của học viên
        public static async Task<List<CountModel>> StatisticCourseStudent(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var data = new List<CountModel>();
                var hoiThaoDangDienRa = await (from ht in db.tbl_Seminar where (ht.Status == (int)SeminarStatus.DangDienRa && ht.Enable == true) select ht.Id).CountAsync();
                var tongKhoaDangHoc = await (from kch in db.tbl_VideoCourseStudent
                                             join c in db.tbl_VideoCourse on kch.VideoCourseId equals c.Id
                                             where (kch.Enable == true && c.Id == kch.VideoCourseId && c.Enable == true && kch.UserId == user.UserInformationId && c.Active == true)
                                             select c.Id).CountAsync();
                var tongKhoaHoc = await (from kh in db.tbl_VideoCourse where (kh.Enable == true && kh.Active == true) select kh.Id).CountAsync();
                var tongCauHoi = await (from ch in db.tbl_QuestionInVideo where (ch.Enable == true && ch.UserId == user.UserInformationId) select ch.Id).CountAsync();
                int tongKhoaChuaHoc = tongKhoaHoc - tongKhoaDangHoc;
                data.Add(new CountModel { Type = "Webinar đang diễn ra", Count = hoiThaoDangDienRa });
                data.Add(new CountModel { Type = "Tổng khóa đang học", Count = tongKhoaDangHoc });
                data.Add(new CountModel { Type = "Tổng khóa chưa học", Count = tongKhoaChuaHoc });
                string sql = $"Get_CompleteCousre @UserId = {user.UserInformationId}";
                var data2 = await db.Database.SqlQuery<Get_CompleteCousre>(sql).CountAsync();
                data.Add(new CountModel { Type = "Tổng khóa đã hoàn thành", Count = data2 });
                data.Add(new CountModel { Type = "Tổng câu hỏi đã đặt", Count = tongCauHoi });
                return data;

            }
        }
        public class Get_Dashboard_LearningDetails
        {
            public int Id { get; set; }
            public int? UserId { get; set; }
            public string Name { get; set; }
            public double Completed { get; set; }
            public double Lesson { get; set; }
            private double SetPercent { get; set; }
            public double Percent {
                get { return Math.Round(SetPercent, 2); }
                set { SetPercent = value; } }
        }
        public static async Task<List<Get_Dashboard_LearningDetails>> LearningDetails(tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_Dashboard_LearningDetails @UserId = {user.UserInformationId}";
                var data = await db.Database.SqlQuery<Get_Dashboard_LearningDetails>(sql).ToListAsync();
                return data;
            }
        }
        public class Get_Dashboard_OverviewLearning
        {
            public int UserInformationId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string RoleName { get; set; }
            public int Course { get; set; }
            public int Completed { get; set; }
            public double Point { get; set; }
            public DateTime? CreatedOn { get; set; }
            public int TotalExam { get; set; }
            public int TotalCourse { get; set; }
            public int TotalCourseCompleted { get; set; }
            public int TotalRow { get; set; }
            public string MarriageName { get; set; }
            public string AcademicLevelName { get; set; }
            public string JobName { get; set; }
            public string MonthlyIncomeName { get; set; }
            public string JobOfFatherName { get; set; }
            public string JobOfMotherName { get; set; }
            public string JobOfSpouseName { get; set; }
            public string IncomeOfFamilyName { get; set; }
        }
        public class Dashboard_OverviewLearningModel
        {
            public int UserInformationId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string RoleName { get; set; }
            public int Course { get; set; }
            public int Completed { get; set; }
            public double Point { get; set; }
            public DateTime? CreatedOn { get; set; }
            public string MarriageName { get; set; }
            public string AcademicLevelName { get; set; }
            public string JobName { get; set; }
            public string MonthlyIncomeName { get; set; }
            public string JobOfFatherName { get; set; }
            public string JobOfMotherName { get; set; }
            public string JobOfSpouseName { get; set; }
            public string IncomeOfFamilyName { get; set; }
        }
        public class OverviewLearningModel
        {
            public int TotalStudent { get; set; } = 0;
            public int TotalExam { get; set; } = 0;
            public int TotalCourseCompleted { get; set; } = 0;
            public int TotalCourse { get; set; } = 0;
            public int TotalRow { get; set; } = 0;
            public List<Dashboard_OverviewLearningModel> Data { get; set; }
        }
        public static async Task<OverviewLearningModel> OverviewLearning(OverviewSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) search = new OverviewSearch();
                string sql = $"Get_Dashboard_OverviewLearning @PageIndex = {search.PageIndex}, @PageSize = {search.PageSize}, @Search = {search.Search ?? "''"}";
                var data = await db.Database.SqlQuery<Get_Dashboard_OverviewLearning>(sql).ToListAsync();
                if (!data.Any()) return new OverviewLearningModel();
                var result = new OverviewLearningModel
                {
                    TotalStudent = data[0].TotalRow,
                    TotalCourse = data[0].TotalCourse,
                    TotalCourseCompleted = data[0].TotalCourseCompleted,
                    TotalRow = data[0].TotalRow,
                    TotalExam = data[0].TotalExam,
                    Data = data.Select(x => new Dashboard_OverviewLearningModel
                    {
                        Completed = x.Completed,
                        Course = x.Course,
                        Email = x.Email,
                        FullName = x.FullName,
                        Point = x.Point,
                        RoleName = x.RoleName,
                        CreatedOn = x.CreatedOn,
                        UserInformationId = x.UserInformationId,
                        AcademicLevelName = x.AcademicLevelName,
                        IncomeOfFamilyName = x.IncomeOfFamilyName,
                        JobName = x.JobName,
                        JobOfFatherName = x.JobOfFatherName,
                        JobOfMotherName = x.JobOfMotherName,
                        JobOfSpouseName = x.JobOfSpouseName,
                        MarriageName = x.MarriageName,
                        MonthlyIncomeName = x.MonthlyIncomeName,
                    }).ToList()
                };
                return result;
            }
        }
        public class Get_Dashboard_OverviewVideoCourse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int VideoCourse { get; set; }
            public int Completed { get; set; }
            public int TotalStudent { get; set; }
            public int TotalCompleted { get; set; }
            public int TotalRow { get; set; }
        }
        public class Dashboard_OverviewVideoCourseModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int VideoCourse { get; set; }
            public int Completed { get; set; }
        }
        public class OverviewVideoCourseModel
        {
            public int TotalVideoCourse { get; set; }
            public int TotalStudent { get; set; }
            public int TotalCompleted { get; set; }
            public int TotalRow { get; set; }
            public List<Dashboard_OverviewVideoCourseModel> Data { get; set; }
        }
        public static async Task<OverviewVideoCourseModel> OverviewVideoCourse(OverviewSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) search = new OverviewSearch();
                string sql = $"Get_Dashboard_OverviewVideoCourse @PageIndex = {search.PageIndex}, @PageSize = {search.PageSize}, @Search = {search.Search ?? "''"}";
                var data = await db.Database.SqlQuery<Get_Dashboard_OverviewVideoCourse>(sql).ToListAsync();
                if (!data.Any()) return new OverviewVideoCourseModel();
                var result = new OverviewVideoCourseModel
                {
                    TotalVideoCourse = data[0].TotalRow,
                    TotalStudent = data[0].TotalStudent,
                    TotalCompleted = data[0].TotalCompleted,
                    TotalRow = data[0].TotalRow,
                    Data = data.Select(x => new Dashboard_OverviewVideoCourseModel
                    {
                        Completed = x.Completed,
                        VideoCourse = x.VideoCourse,
                        Id = x.Id,
                        Name = x.Name
                    }).ToList()
                };
                return result;
            }
        }
        public class Get_Dashboard_OverviewExam
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string VideoCourseName { get; set; }
            public int Completed { get; set; }
            public int Pass { get; set; }
            public double Medium { get; set; }
            public int TotalExam { get; set; }
            public int TotalCompleted { get; set; }
            public int TotalPass { get; set; }
            public double TotalMedium { get; set; }
            public int TotalRow { get; set; }
        }
        public class Dashboard_OverviewExam
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string VideoCourseName { get; set; }
            public int Completed { get; set; }
            public int Pass { get; set; }
            public double Medium { get; set; }
        }
        public class OverviewExamModel
        {
            public int TotalExam { get; set; }
            public int TotalCompleted { get; set; }
            public int TotalPass { get; set; }
            public double TotalMedium { get; set; }
            public int TotalRow { get; set; }
            public List<Dashboard_OverviewExam> Data { get; set; }
        }
        public static async Task<OverviewExamModel> OverviewExam(OverviewSearch search)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) search = new OverviewSearch();
                string sql = $"Get_Dashboard_OverviewExam @PageIndex = {search.PageIndex}, @PageSize = {search.PageSize}, @Search = {search.Search ?? "''"}";
                var data = await db.Database.SqlQuery<Get_Dashboard_OverviewExam>(sql).ToListAsync();
                if (!data.Any()) return new OverviewExamModel();
                var result = new OverviewExamModel
                {
                    TotalExam = data[0].TotalRow,
                    TotalMedium = Math.Round(data[0].TotalMedium, 2),
                    TotalPass = data[0].TotalPass,
                    TotalCompleted = data[0].TotalCompleted,
                    TotalRow = data[0].TotalRow,
                    Data = data.Select(x => new Dashboard_OverviewExam
                    {
                        Completed = x.Completed,
                        Medium = Math.Round(x.Medium, 2),
                        Pass = x.Pass,
                        VideoCourseName = x.VideoCourseName,
                        Id = x.Id,
                        Name = x.Name
                    }).ToList()
                };
                return result;
            }
        }
        public class OverviewUserInformationModel
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        //public static async Task<List<OverviewUserInformationModel>> OverviewUserInformation()
        //{
        //    using (var db = new lmsDbContext())
        //    {
        //        var result = new List<OverviewUserInformationModel>();
        //        var academicLevels = await db.tbl_AcademicLevel.Where(x=>x.Enable == true).Select(x=> new { x.Id,x.Name}).ToListAsync();
        //        if (academicLevels.Any())
        //        {
        //            foreach (var item in academicLevels)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.AcademicLevelId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Trình độ học vấn: " + item.Name, Value = count });
        //            }
        //        }
        //        var incomeOfFamilys = await db.tbl_IncomeOfFamily.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (incomeOfFamilys.Any())
        //        {
        //            foreach (var item in incomeOfFamilys)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.IncomeOfFamilyId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Thu nhập trung bình của gia đình: " + item.Name, Value = count });
        //            }
        //        }
        //        var jobs = await db.tbl_Job.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (jobs.Any())
        //        {
        //            foreach (var item in jobs)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.JobId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Công việc: " + item.Name, Value = count });
        //            }
        //        }
        //        var jobOfFathers = await db.tbl_JobOfFather.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (jobOfFathers.Any())
        //        {
        //            foreach (var item in jobOfFathers)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.JobOfFatherId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Công việc của bố: " + item.Name, Value = count });
        //            }
        //        }
        //        var jobOfMothers = await db.tbl_JobOfMother.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (jobOfMothers.Any())
        //        {
        //            foreach (var item in jobOfMothers)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.JobOfMotherId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Công việc của mẹ: " + item.Name, Value = count });
        //            }
        //        }
        //        var jobOfSpouses = await db.tbl_JobOfSpouse.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (jobOfSpouses.Any())
        //        {
        //            foreach (var item in jobOfSpouses)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.JobOfSpouseId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Công việc của vợ hoặc chồng: " + item.Name, Value = count });
        //            }
        //        }
        //        var marriages = await db.tbl_Marriage.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (marriages.Any())
        //        {
        //            foreach (var item in marriages)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.MarriageId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Tình trạng hôn nhân: " + item.Name, Value = count });
        //            }
        //        }
        //        var monthlyIncomes = await db.tbl_MonthlyIncome.Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
        //        if (monthlyIncomes.Any())
        //        {
        //            foreach (var item in monthlyIncomes)
        //            {
        //                var count = await db.tbl_UserInformation.CountAsync(x => x.Enable == true && x.MonthlyIncomeId == item.Id);
        //                result.Add(new OverviewUserInformationModel { Name = "Thu nhập hằng tháng: " + item.Name, Value = count });
        //            }
        //        }
        //        return result;
        //    }
        //}


        #region Dashboard For Student
        public class CertificateInDashboard
        {
            /// <summary>
            /// Số lượng chứng chỉ
            /// </summary>
            public int Amount { get; set; }
            public List<CertificateInDashboardItem> Items { get; set; }
            public CertificateInDashboard()
            {
                Items = new List<CertificateInDashboardItem>();
            }
        }
        public class CertificateInDashboardItem
        {
            public int CertificateId { get; set; }
            public string VideoCourseName { get; set; }
            public string VideoCourseThumbnail { get; set; }
            public string PDFUrl { get; set; }
        }
        public static async Task<CertificateInDashboard> GetCertificateInDashboard(tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var result = new CertificateInDashboard();

                var data = await db.tbl_Certificate.Where(x => x.UserId == userLog.UserInformationId)
                    .Select(x => new { x.Id, x.PDFUrl, x.VideoCourseId }).OrderByDescending(x => x.Id).ToListAsync();
                if (data.Any())
                {
                    result.Amount = data.Count();
                    data = data.Take(5).ToList();
                    foreach (var item in data)
                    {
                        var videoCourse = await db.tbl_VideoCourse.Where(x => x.Id == item.VideoCourseId)
                            .Select(x => new { x.Name, x.Thumbnail }).FirstOrDefaultAsync();
                        result.Items.Add(new CertificateInDashboardItem
                        {
                            CertificateId = item.Id,
                            VideoCourseName = videoCourse.Name,
                            VideoCourseThumbnail = videoCourse.Thumbnail,
                            PDFUrl = item.PDFUrl
                        });
                    }
                }

                return result;
            }
        }
        public class LearningInDashboard
        {
            /// <summary>
            /// Số lượng khóa đang học
            /// </summary>
            public int Amount { get; set; }
            /// <summary>
            /// Số lượng chương trình đã hoàn thành
            /// </summary>
            public int VideoCourseCompleted { get; set; }
            /// <summary>
            /// Số lượng bài kiểm tra đã vượt qua
            /// </summary>
            public int ExamPass { get; set; }
            public List<LearningInDashboardItem> Items { get; set; }
            public LearningInDashboard()
            {
                Items = new List<LearningInDashboardItem>();
            }
        }
        public class LearningInDashboardItem
        {
            public int VideoCourseId { get; set; }
            public string VideoCourseName { get; set; }
            public string VideoCourseThumbnail { get; set; }
            public double CompletedPercent { get; set; }
        }
        public static async Task<LearningInDashboard> GetLearningInDashboard(tbl_UserInformation userLog)
        {
            using (var db = new lmsDbContext())
            {
                var result = new LearningInDashboard();

                var data = await db.tbl_VideoCourseStudent.Where(x => x.UserId == userLog.UserInformationId && x.Enable == true && x.Status == 2)
                    .Select(x => new { x.VideoCourseId, x.CompletedPercent, x.Status }).ToListAsync();
                result.VideoCourseCompleted = await db.tbl_VideoCourseStudent.CountAsync(x => x.UserId == userLog.UserInformationId && x.Enable == true && x.Status == 3);
                result.ExamPass = await db.tbl_ExamResult.CountAsync(x => x.StudentId == userLog.UserInformationId && x.Enable == true);
                if (data.Any())
                {
                    result.Amount = data.Count();
                    foreach (var item in data)
                    {
                        var videoCourse = await db.tbl_VideoCourse
                            .Where(x => x.Id == item.VideoCourseId && x.Enable == true)
                            .Select(x => new { x.Id, x.Name, x.Thumbnail }).FirstOrDefaultAsync();
                        if (videoCourse != null)
                        {
                            result.Items.Add(new LearningInDashboardItem
                            {
                                CompletedPercent = item.CompletedPercent,
                                VideoCourseId = videoCourse.Id,
                                VideoCourseName = videoCourse.Name,
                                VideoCourseThumbnail = videoCourse.Thumbnail
                            });
                        }
                    }

                }
                return result;
            }
        }
        #endregion

        public class UnfinishedGradingModel
        {
            public int VideoCourseId { get; set; }
            public string VideoCourseName { get; set; }
            public int Amount { get; set; }
        }
        public static async Task<List<UnfinishedGradingModel>> GetUnfinishedGrading()
        {
            using (var db = new lmsDbContext())
            {
                var result = new List<UnfinishedGradingModel>();
                var videoCourses = await db.tbl_VideoCourse
                    .Where(x => x.Enable == true).Select(x => new { x.Id, x.Name }).ToListAsync();
                if (videoCourses.Any())
                {
                    foreach (var item in videoCourses)
                    {
                        int amount = await db.tbl_ExamResult
                            .CountAsync(x => x.VideoCourseId == item.Id && x.Enable == true && x.Status == 1);
                        result.Add(new UnfinishedGradingModel
                        {
                            Amount = amount,
                            VideoCourseId = item.Id,
                            VideoCourseName = item.Name
                        });
                    }
                }
                return result.Where(x => x.Amount > 0).ToList();
            }
        }
        public class StatisticalCertificate
        {
            public int VideoCourseId { get; set; }
            public string VideoCourseName { get; set; }
            public int Amount { get; set; }
        }
        public static async Task<List<StatisticalCertificate>> GetStatisticalCertificate()
        {
            using (var db = new lmsDbContext())
            {
                var result = new List<StatisticalCertificate>();

                var videoCourses = await db.tbl_VideoCourse.Where(x => x.Enable == true)
                    .Select(x=> new { x.Id,x.Name }).ToListAsync();
                if (videoCourses.Any())
                {
                    foreach (var item in videoCourses)
                    {
                        var amount = await db.tbl_Certificate.CountAsync(x => x.VideoCourseId == item.Id && x.Enable == true);
                        result.Add(new StatisticalCertificate
                        {
                            VideoCourseId = item.Id,
                            VideoCourseName = item.Name,
                            Amount = amount
                        });
                    }
                }
                return result.Where(x => x.Amount > 0).OrderByDescending(x => x.Amount).ToList();
            }
        }
        public class StatisticalExamResult
        {
            public int ExamId { get; set; }
            public string ExamName { get; set; }
            /// <summary>
            /// Số lượng học viên đã làm bài
            /// </summary>
            public int Amount { get; set; }
        }
        public static async Task<List<StatisticalExamResult>> GetStatisticalExamResult()
        {
            using (var db = new lmsDbContext())
            {
                var result = new List<StatisticalExamResult>();
                var exams = await db.tbl_Exam.Where(x => x.Enable == true)
                    .Select(x => new { x.Id, x.Name }).ToListAsync();
                if (exams.Any())
                { 
                    foreach (var item in exams)
                    {
                        var amount = await db.tbl_ExamResult.CountAsync(x => x.ExamId == item.Id && x.Enable == true && x.VideoCourseId != 0);
                        result.Add(new StatisticalExamResult
                        {
                            ExamId = item.Id,
                            ExamName = item.Name,
                            Amount = amount
                        });
                    }
                }
                return result.Where(x => x.Amount > 0).OrderByDescending(x => x.Amount).ToList();
            }
        }
    }
}
