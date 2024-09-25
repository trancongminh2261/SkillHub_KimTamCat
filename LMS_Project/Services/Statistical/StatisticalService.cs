/*using LMS_Project.Areas.Models;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services.Statistical
{
    public class StatisticalService : DomainService
    {
        public StatisticalService(lmsDbContext dbContext) : base(dbContext) { }

        #region model + xử lý data
        public class Time
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public int LastMonth { get; set; }
            public int YearOfLastMonth { get; set; }
            public int LastYear { get; set; }
            public int Day { get; set; }
        }

        public static Time GetTimeModel(int? month, int? year)
        {
            DateTime timeNow = DateTime.Now;
            Time time = new Time();
            time.Month = month ?? DateTime.Now.Month;
            time.Year = year ?? DateTime.Now.Year;
            time.LastMonth = time.Month - 1 == 0 ? 12 : time.Month - 1;
            time.YearOfLastMonth = time.LastMonth == 12 ? time.Year - 1 : time.Year;
            time.LastYear = time.Year - 1;
            time.Day = timeNow.Day;
            return time;
        }
        public class StatisticalModel
        {
            public string Type { get; set; }
            public double Value { get; set; } = 0;
        }

        public class StatisticalDescriptionModel : StatisticalModel
        {
            public string Description { get; set; }
        }

        public class StatisticalDetailModel : StatisticalModel
        {
            //ví dụ value là tỉ lệ thì value detail là con số cụ thể
            public double ValueDetail { get; set; } = 0;
        }

        //dùng cho các biểu đồ show dữ liệu 12 tháng trong năm
        public class Statistical12MonthModel
        {
            public string Month { get; set; }
            public string Type { get; set; }
            public double Value { get; set; } = 0;
        }
        //dùng cho các biểu đồ so sánh dữ liệu năm được chọn và năm trước đó
        public class StatisticalCompareModel
        {
            /// <summary>
            /// tiêu đề => trong ant design tiêu đề là type nên đặt vậy luôn FE đỡ rối
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// dữ liệu năm được chọn 
            /// </summary>
            public List<StatisticalModel> DataInYear { get; set; }
            /// <summary>
            /// dữ liệu năm được chọn - 1
            /// </summary>
            public List<StatisticalModel> DataPreYear { get; set; }
        }

        // bổ sung thêm nhận xét tăng hay giảm so với năm trước
        public class StatisticalCommentModel : StatisticalModel
        {
            /// <summary>
            /// số lượng chênh lệch
            /// </summary>
            public double? DifferenceQuantity { get; set; }
            /// <summary>
            /// tỷ lệ chênh lệch
            /// </summary>
            public double? DifferenceValue { get; set; }
            /// <summary>
            /// 1 - tăng
            /// 2 - giảm
            /// 3 - không đổi
            /// </summary>
            public int? Status { get; set; }
            public string StatusName
            {
                get
                {
                    return Status == 1 ? "tăng"
                        : Status == 2 ? "giảm"
                        : Status == 3 ? "không đổi" : null;
                }
            }
        }
        public class CompareModel
        {
            /// <summary>
            /// số lượng chênh lệch
            /// </summary>
            public double? DifferenceQuantity { get; set; }
            /// <summary>
            /// tỷ lệ chênh lệch
            /// </summary>
            public double? DifferenceValue { get; set; }
            /// <summary>
            /// 1 - tăng
            /// 2 - giảm
            /// 3 - không đổi
            /// </summary>
            public int? Status { get; set; }
        }
        public static CompareModel CompareProgress(double thisMonth, double lastMonth)
        {
            double differenceQuantityValue = 0;
            double differenceRateValue = 0;
            int status = 3;
            //tháng này > 0 && tháng trước = 0 ( tăng 100% )
            if (thisMonth > 0 && lastMonth == 0)
            {
                differenceQuantityValue = Math.Abs(thisMonth);
                differenceRateValue = 100;
                status = 1;
            }
            //tháng này = 0 && tháng trước > 0 ( giảm 100% )
            if (thisMonth == 0 && lastMonth > 0)
            {
                differenceQuantityValue = Math.Abs(lastMonth);
                differenceRateValue = 100;
                status = 2;
            }

            //tháng này > 0 && tháng trước > 0
            if (thisMonth > 0 && lastMonth > 0)
            {
                //chênh lệch
                differenceQuantityValue = Math.Round(Math.Abs(thisMonth - lastMonth), 2);
                differenceRateValue = Math.Round(differenceQuantityValue / lastMonth * 100, 2);
                //tháng này > tháng trước ( tăng percent% )
                if (thisMonth > lastMonth)
                {
                    status = 1;
                }
                //tháng này < tháng trước (giảm percent% )
                if (thisMonth < lastMonth)
                {
                    status = 2;
                }
            }
            return new CompareModel { DifferenceQuantity = differenceQuantityValue, DifferenceValue = differenceRateValue, Status = status };
        }
        #endregion

        #region thống kê số liệu tổng quan
        public async Task<IList<StatisticalModel>> Overview(StatisticalSearch baseSearch, tbl_UserInformation currentUser)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.Month, baseSearch.Year);
            var result = new List<StatisticalModel>();
            var data = new StatisticalModel();
            double totalData = 0;
            double totalDataInMonth = 0;
            double totalDataPreMonth = 0;
            var compare = new CompareModel();

            #region admin
            if (currentUser.RoleId == (int)RoleEnum.admin)
            {
                var listUser = await dbContext.tbl_UserInformation.Where(x => x.Enable == true).ToListAsync() ?? new List<tbl_UserInformation>();
                //số lượng quản lý
                totalData = listUser.Count(x => x.RoleId == (int)RoleEnum.manager);
                data = new StatisticalModel
                {
                    Type = "Tổng quản lý",
                    Value = totalData
                };
                result.Add(data);

                //số lượng giáo viên
                totalData = listUser.Count(x => x.RoleId == (int)RoleEnum.teacher);
                data = new StatisticalModel
                {
                    Type = "Tổng giáo viên",
                    Value = totalData
                };
                result.Add(data);

                //số lượng học viên
                totalData = listUser.Count(x => x.RoleId == (int)RoleEnum.student);
                data = new StatisticalModel
                {
                    Type = "Tổng học viên",
                    Value = totalData
                };
                result.Add(data);

                //số lượng chứng chỉ đã đạt
                var listCertificate = await dbContext.tbl_Certificate.Where(x => x.Enable == true && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                totalData = listCertificate.Count;
                data = new StatisticalModel
                {
                    Type = "Tổng chứng chỉ",
                    Value = totalData
                };
                result.Add(data);

                //tỷ lệ vượt qua bài kiểm tra và bài thi
                var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                double totalExam = listExamResult.Count(x => x.ExamPeriodId == null);
                double totalPassExam = listExamResult.Count(x => x.ExamPeriodId == null && x.IsPass == true);
                double ratePassExam = 0;
                if (totalExam > 0)
                    ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua bài kiểm tra",
                    Value = ratePassExam
                };
                result.Add(data);

                double totalExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null);
                double totalPassExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null && x.IsPass == true);
                double ratePassExamPeriod = 0;
                if (totalExamPeriod > 0)
                    ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua kỳ thi",
                    Value = ratePassExamPeriod
                };
                result.Add(data);
            }
            #endregion

            #region quản lý
            if (currentUser.RoleId == (int)RoleEnum.manager)
            {
                var listUserInDepartment = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.DepartmentId == currentUser.DepartmentId).ToListAsync() ?? new List<tbl_UserInformation>();             
                //số lượng quản lý
                totalData = listUserInDepartment.Count(x => x.RoleId == (int)RoleEnum.manager);
                data = new StatisticalModel
                {
                    Type = "Tổng quản lý",
                    Value = totalData
                };
                result.Add(data);

                //số lượng giáo viên
                totalData = listUserInDepartment.Count(x => x.RoleId == (int)RoleEnum.teacher);
                data = new StatisticalModel
                {
                    Type = "Tổng giáo viên",
                    Value = totalData
                };
                result.Add(data);

                //số lượng học viên
                totalData = listUserInDepartment.Count(x => x.RoleId == (int)RoleEnum.student);
                data = new StatisticalModel
                {
                    Type = "Tổng học viên",
                    Value = totalData
                };
                result.Add(data);

                var listStudentId = listUserInDepartment.Where(x => x.RoleId == (int)RoleEnum.student).Select(x => x.UserInformationId).ToList();

                //số lượng chứng chỉ đã đạt
                var listCertificate = await dbContext.tbl_Certificate.Where(x => x.Enable == true && listStudentId.Contains(x.UserId ?? 0) && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                totalData = listCertificate.Count;
                data = new StatisticalModel
                {
                    Type = "Tổng chứng chỉ",
                    Value = totalData
                };
                result.Add(data);

                //tỷ lệ vượt qua bài kiểm tra và bài thi
                var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true && listStudentId.Contains(x.StudentId ?? 0) && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                double totalExam = listExamResult.Count(x => x.ExamPeriodId == null);
                double totalPassExam = listExamResult.Count(x => x.ExamPeriodId == null && x.IsPass == true);
                double ratePassExam = 0;
                if (totalExam > 0)
                    ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua bài kiểm tra",
                    Value = ratePassExam
                };
                result.Add(data);

                double totalExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null);
                double totalPassExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null && x.IsPass == true);
                double ratePassExamPeriod = 0;
                if (totalExamPeriod > 0)
                    ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua kỳ thi",
                    Value = ratePassExamPeriod
                };
                result.Add(data);
            }
            #endregion

            #region giáo viên
            if (currentUser.RoleId == (int)RoleEnum.teacher)
            {
                var listUserInDepartment = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.DepartmentId == currentUser.DepartmentId).ToListAsync() ?? new List<tbl_UserInformation>();

                //số lượng giáo viên
                totalData = listUserInDepartment.Count(x => x.RoleId == (int)RoleEnum.teacher);
                data = new StatisticalModel
                {
                    Type = "Tổng giáo viên",
                    Value = totalData
                };
                result.Add(data);

                //số lượng học viên
                totalData = listUserInDepartment.Count(x => x.RoleId == (int)RoleEnum.student);
                data = new StatisticalModel
                {
                    Type = "Tổng học viên",
                    Value = totalData
                };
                result.Add(data);

                var listStudentId = listUserInDepartment.Where(x => x.RoleId == (int)RoleEnum.student).Select(x => x.UserInformationId).ToList();

                //số lượng chứng chỉ đã đạt
                var listCertificate = await dbContext.tbl_Certificate.Where(x => x.Enable == true && listStudentId.Contains(x.UserId ?? 0) && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                totalData = listCertificate.Count;
                data = new StatisticalModel
                {
                    Type = "Tổng chứng chỉ",
                    Value = totalData
                };
                result.Add(data);

                //tỷ lệ vượt qua bài kiểm tra và bài thi
                var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true && listStudentId.Contains(x.StudentId ?? 0) && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                double totalExam = listExamResult.Count(x => x.ExamPeriodId == null);
                double totalPassExam = listExamResult.Count(x => x.ExamPeriodId == null && x.IsPass == true);
                double ratePassExam = 0;
                if (totalExam > 0)
                    ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua bài kiểm tra",
                    Value = ratePassExam
                };
                result.Add(data);

                double totalExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null);
                double totalPassExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null && x.IsPass == true);
                double ratePassExamPeriod = 0;
                if (totalExamPeriod > 0)
                    ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua kỳ thi",
                    Value = ratePassExamPeriod
                };
                result.Add(data);

                //số câu hỏi đã trả lời
                var listAnswerQuestion = await dbContext.tbl_AnswerInVideo
                    .Where(x => x.Enable == true && x.UserId == currentUser.UserInformationId && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year)
                    .GroupBy(x => x.QuestionInVideoId)
                    .Select(g => g.FirstOrDefault())
                    .ToListAsync() ?? new List<tbl_AnswerInVideo>();

                data = new StatisticalModel
                {
                    Type = "Số câu hỏi đã trả lời",
                    Value = listAnswerQuestion.Count
                };
                result.Add(data);
            }
            #endregion

            #region học viên
            if (currentUser.RoleId == (int)RoleEnum.student)
            {
                //số lượng chứng chỉ đã đạt
                var listCertificate = await dbContext.tbl_Certificate.Where(x => x.Enable == true && x.UserId == currentUser.UserInformationId && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                totalData = listCertificate.Count;
                data = new StatisticalModel
                {
                    Type = "Tổng chứng chỉ",
                    Value = totalData
                };
                result.Add(data);

                //tỷ lệ vượt qua bài kiểm tra và bài thi
                var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true && x.StudentId == currentUser.UserInformationId && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                double totalExam = listExamResult.Count(x => x.ExamPeriodId == null);
                double totalPassExam = listExamResult.Count(x => x.ExamPeriodId == null && x.IsPass == true);
                double ratePassExam = 0;
                if (totalExam > 0)
                    ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua bài kiểm tra",
                    Value = ratePassExam
                };
                result.Add(data);

                double totalExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null);
                double totalPassExamPeriod = listExamResult.Count(x => x.ExamPeriodId != null && x.IsPass == true);
                double ratePassExamPeriod = 0;
                if (totalExamPeriod > 0)
                    ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                data = new StatisticalModel
                {
                    Type = "Tỷ lệ vượt qua kỳ thi",
                    Value = ratePassExamPeriod
                };
                result.Add(data);
            }
            #endregion
            return result;
        }      
        #endregion       

        #region thống kê tỷ lệ hội thảo sắp diễn ra, đang diễn ra, và đã kết thúc
        public async Task<IList<StatisticalModel>> WebinarProgress(StatisticalSearch baseSearch, tbl_UserInformation userLogin)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.Month, baseSearch.Year);
            var result = new List<StatisticalModel>();
            var data = new StatisticalModel();
            double countData = 0;

            #region admin
            if (userLogin.RoleId == (int)RoleEnum.admin)
            {
                var listSeminar = await dbContext.tbl_Seminar.Where(x => x.Enable == true
                    && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                if (listSeminar.Count <= 0) return result;
                //lớp sắp diễn ra
                data = new StatisticalModel();
                data.Type = "Chưa diễn ra";
                countData = listSeminar.Count(x => x.Status == 1);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
                //lớp đang diễn ra
                data = new StatisticalModel();
                data.Type = "Đang diễn ra";
                countData = listSeminar.Count(x => x.Status == 2);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
                //lớp đã diễn ra
                data = new StatisticalModel();
                data.Type = "Kết thúc";
                countData = listSeminar.Count(x => x.Status == 3);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
            }
            #endregion

            #region quản lý
            if (userLogin.RoleId == (int)RoleEnum.manager)
            {
                var listSeminar = await dbContext.tbl_Seminar.Where(x => x.Enable == true
                    && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                if (listSeminar.Count <= 0) return result;
                //lớp sắp diễn ra
                data = new StatisticalModel();
                data.Type = "Chưa diễn ra";
                countData = listSeminar.Count(x => x.Status == 1);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
                //lớp đang diễn ra
                data = new StatisticalModel();
                data.Type = "Đang diễn ra";
                countData = listSeminar.Count(x => x.Status == 2);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
                //lớp đã diễn ra
                data = new StatisticalModel();
                data.Type = "Kết thúc";
                countData = listSeminar.Count(x => x.Status == 3);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
            }
            #endregion

            #region quản lý
            if (userLogin.RoleId == (int)RoleEnum.teacher)
            {
                var listSeminar = await dbContext.tbl_Seminar.Where(x => x.Enable == true
                    && x.LeaderId == userLogin.UserInformationId
                    && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync();
                if (listSeminar.Count <= 0) return result;
                //lớp sắp diễn ra
                data = new StatisticalModel();
                data.Type = "Chưa diễn ra";
                countData = listSeminar.Count(x => x.Status == 1);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
                //lớp đang diễn ra
                data = new StatisticalModel();
                data.Type = "Đang diễn ra";
                countData = listSeminar.Count(x => x.Status == 2);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
                //lớp đã diễn ra
                data = new StatisticalModel();
                data.Type = "Kết thúc";
                countData = listSeminar.Count(x => x.Status == 3);
                data.Value = Math.Round(countData / listSeminar.Count * 100, 2);
                result.Add(data);
            }
            #endregion
            return result;
        }
        #endregion

        #region xếp hạng học viên dựa trên thành tích học tập
        public class AcademicRankModel
        {
            public string UserCode { get; set; }
            public string FullName { get; set; }
            public string Avatar { get; set; }
            public double TotalPoint { get; set; } = 0;
        }
        public async Task<AppDomainResult<AcademicRankModel>> AcademicRank(StatisticalSearch baseSearch, tbl_UserInformation userLogin)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.Month, baseSearch.Year);
            var result = new List<AcademicRankModel>();
            var listStudent = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync() ?? new List<tbl_UserInformation>();
            var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true && x.MyPoint != null && x.MyPoint > 0).ToListAsync() ?? new List<tbl_ExamResult>();
            #region admin
            if (userLogin.RoleId == (int)RoleEnum.admin)
            {
                if (listStudent.Count <= 0) return new AppDomainResult<AcademicRankModel> { TotalRow = 0, Data = null };
                foreach (var item in listStudent)
                {
                    var data = new AcademicRankModel();
                    data.UserCode = item.UserCode;
                    data.FullName = item.FullName;
                    data.Avatar = item.Avatar;
                    var studentExamResults = listExamResult.Where(x => x.StudentId == item.UserInformationId).ToList();
                    if (studentExamResults.Count > 0)
                    {
                        data.TotalPoint = studentExamResults.Sum(x => x.MyPoint);
                    }
                    result.Add(data);
                }
            }
            #endregion
            #region quản lý
            if (userLogin.RoleId == (int)RoleEnum.manager)
            {
                listStudent = listStudent.Where(x => x.DepartmentId == userLogin.DepartmentId).ToList() ?? new List<tbl_UserInformation>();
                if (listStudent.Count <= 0) return new AppDomainResult<AcademicRankModel> { TotalRow = 0, Data = null };
                foreach (var item in listStudent)
                {
                    var data = new AcademicRankModel();
                    data.UserCode = item.UserCode;
                    data.FullName = item.FullName;
                    data.Avatar = item.Avatar;
                    var studentExamResults = listExamResult.Where(x => x.StudentId == item.UserInformationId).ToList();
                    if (studentExamResults.Count > 0)
                    {
                        data.TotalPoint = studentExamResults.Sum(x => x.MyPoint);
                    }
                    result.Add(data);
                }
            }
            #endregion
            #region giáo viên
            if (userLogin.RoleId == (int)RoleEnum.teacher)
            {
                listStudent = listStudent.Where(x => x.DepartmentId == userLogin.DepartmentId).ToList() ?? new List<tbl_UserInformation>();
                if (listStudent.Count <= 0) return new AppDomainResult<AcademicRankModel> { TotalRow = 0, Data = null };
                foreach (var item in listStudent)
                {
                    var data = new AcademicRankModel();
                    data.UserCode = item.UserCode;
                    data.FullName = item.FullName;
                    data.Avatar = item.Avatar;
                    var studentExamResults = listExamResult.Where(x => x.StudentId == item.UserInformationId).ToList();
                    if (studentExamResults.Count > 0)
                    {
                        data.TotalPoint = studentExamResults.Sum(x => x.MyPoint);
                    }
                    result.Add(data);
                }
            }
            #endregion
            var totalRow = result.Count;
            result = result.OrderByDescending(x => x.TotalPoint).ToList();
            // Phân trang
            int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            result = result.Skip(startIndex).Take(baseSearch.PageSize).ToList();
            return new AppDomainResult<AcademicRankModel> { TotalRow = totalRow, Data = result };
        }
        #endregion

        #region thống kê tỷ lệ vượt qua bài kiểm tra của từng nhân viên
        public class RatePassExamModel
        {
            public string FullName { get; set; }
            public string UserCode { get; set; }
            public string Avatar { get; set; }
            public double RatePassExam { get; set; }
            public double RatePassExamPeriod { get; set; }
        }
        public async Task<AppDomainResult<RatePassExamModel>> RatePassExam(StatisticalSearch baseSearch, tbl_UserInformation currentUser)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var listStudent = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).OrderBy(x => x.UserInformationId).ToListAsync() ?? new List<tbl_UserInformation>();
            var listExamResult = await dbContext.tbl_ExamResult.Where(x => x.Enable == true).ToListAsync() ?? new List<tbl_ExamResult>();
            var totalRow = 0;
            var result = new List<RatePassExamModel>();

            if (currentUser.RoleId == (int)RoleEnum.admin)
            {
                totalRow = listStudent.Count;
                // Phân trang
                int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                listStudent = listStudent.Skip(startIndex).Take(baseSearch.PageSize).ToList();
                foreach (var student in listStudent)
                {
                    double totalExam = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId == null);
                    double totalPassExam = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId == null && x.IsPass == true);
                    double ratePassExam = 0;
                    if (totalExam > 0)
                        ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                    double totalExamPeriod = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId != null);
                    double totalPassExamPeriod = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId != null && x.IsPass == true);
                    double ratePassExamPeriod = 0;
                    if (totalExamPeriod > 0)
                        ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                    var data = new RatePassExamModel();
                    data.FullName = student.FullName;
                    data.UserCode = student.UserCode;
                    data.Avatar = student.Avatar;
                    data.RatePassExam = ratePassExam;
                    data.RatePassExamPeriod = ratePassExamPeriod;
                    result.Add(data);
                }
            }

            if (currentUser.RoleId == (int)RoleEnum.manager)
            {
                listStudent = listStudent.Where(x => x.DepartmentId == currentUser.DepartmentId).ToList();
                totalRow = listStudent.Count;
                // Phân trang
                int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                listStudent = listStudent.Skip(startIndex).Take(baseSearch.PageSize).ToList();
                foreach (var student in listStudent)
                {
                    double totalExam = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId == null);
                    double totalPassExam = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId == null && x.IsPass == true);
                    double ratePassExam = 0;
                    if (totalExam > 0)
                        ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                    double totalExamPeriod = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId != null);
                    double totalPassExamPeriod = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId != null && x.IsPass == true);
                    double ratePassExamPeriod = 0;
                    if (totalExamPeriod > 0)
                        ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                    var data = new RatePassExamModel();
                    data.FullName = student.FullName;
                    data.UserCode = student.UserCode;
                    data.Avatar = student.Avatar;
                    data.RatePassExam = ratePassExam;
                    data.RatePassExamPeriod = ratePassExamPeriod;
                    result.Add(data);
                }
            }

            if (currentUser.RoleId == (int)RoleEnum.teacher)
            {
                listStudent = listStudent.Where(x => x.DepartmentId == currentUser.DepartmentId).ToList();
                totalRow = listStudent.Count;
                // Phân trang
                int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                listStudent = listStudent.Skip(startIndex).Take(baseSearch.PageSize).ToList();
                foreach (var student in listStudent)
                {
                    double totalExam = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId == null);
                    double totalPassExam = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId == null && x.IsPass == true);
                    double ratePassExam = 0;
                    if (totalExam > 0)
                        ratePassExam = Math.Round((totalPassExam / totalExam * 100), 2);
                    double totalExamPeriod = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId != null);
                    double totalPassExamPeriod = listExamResult.Count(x => x.StudentId != student.UserInformationId && x.ExamPeriodId != null && x.IsPass == true);
                    double ratePassExamPeriod = 0;
                    if (totalExamPeriod > 0)
                        ratePassExamPeriod = Math.Round((totalPassExamPeriod / totalExamPeriod * 100), 2);
                    var data = new RatePassExamModel();
                    data.FullName = student.FullName;
                    data.UserCode = student.UserCode;
                    data.Avatar = student.Avatar;
                    data.RatePassExam = ratePassExam;
                    data.RatePassExamPeriod = ratePassExamPeriod;
                    result.Add(data);
                }
            }

            return new AppDomainResult<RatePassExamModel> { TotalRow = totalRow, Data = result };
        }
        #endregion

        #region Api lấy trung bình thời gian mỗi nhân viên xem video trong tháng
        public class StudyTimeModel
        {
            public string FullName { get; set; }
            public string UserCode { get; set; }
            public string Avatar { get; set; }
            public double Time { get; set; }
        }
        public async Task<AppDomainResult<StudyTimeModel>> StudyTime(StatisticalSearch baseSearch, tbl_UserInformation currentUser)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.Month, baseSearch.Year);
            var result = new List<StudyTimeModel>();
            var totalRow = 0;
            var listLessonVideo = await dbContext.tbl_LessonVideo.Where(x => x.Enable == true).ToListAsync() ?? new List<tbl_LessonVideo>();
            var listLessonVideoCompleted = await dbContext.tbl_LessonCompleted.Where(x => x.Enable == true && x.LessonVideoId != null && x.LessonVideoId != 0 && x.CreatedOn.Value.Month == time.Month && x.CreatedOn.Value.Year == time.Year).ToListAsync() ?? new List<tbl_LessonCompleted>();
            var listStudent = await dbContext.tbl_UserInformation.Where(x => x.Enable == true && x.RoleId == (int)RoleEnum.student).ToListAsync() ?? new List<tbl_UserInformation>();
            if (currentUser.RoleId == (int)RoleEnum.admin)
            {             
                totalRow = listStudent.Count;
                // Phân trang
                int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                listStudent = listStudent.Skip(startIndex).Take(baseSearch.PageSize).ToList();
                foreach (var student in listStudent)
                {
                    double totalMinute = 0;
                    var studentVideoCompleted = listLessonVideoCompleted.Where(x => x.UserId == student.UserInformationId).Select(x => x.LessonVideoId).ToList();
                    if (studentVideoCompleted.Count > 0)
                    {
                        foreach (var lessonVideoId in studentVideoCompleted)
                        {
                            var lessonVideo = await dbContext.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Enable == true && x.Id == lessonVideoId);
                            if (lessonVideo != null)
                            {
                                totalMinute += lessonVideo.Minute;
                            }
                        }
                    }
                    double totalHour = Math.Round((totalMinute / 60), 4);
                    var data = new StudyTimeModel();
                    data.FullName = student.FullName;
                    data.Avatar = student.Avatar;
                    data.UserCode = student.UserCode;
                    data.Time = totalHour;
                    result.Add(data);
                }
            }
            if (currentUser.RoleId == (int)RoleEnum.manager)
            {
                listStudent = listStudent.Where(x => x.DepartmentId == currentUser.DepartmentId).ToList();
                totalRow = listStudent.Count;
                // Phân trang
                int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                listStudent = listStudent.Skip(startIndex).Take(baseSearch.PageSize).ToList();
                foreach (var student in listStudent)
                {
                    double totalMinute = 0;
                    var studentVideoCompleted = listLessonVideoCompleted.Where(x => x.UserId == student.UserInformationId).Select(x => x.LessonVideoId).ToList();
                    if (studentVideoCompleted.Count > 0)
                    {
                        foreach (var lessonVideoId in studentVideoCompleted)
                        {
                            var lessonVideo = await dbContext.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Enable == true && x.Id == lessonVideoId);
                            if (lessonVideo != null)
                            {
                                totalMinute += lessonVideo.Minute;
                            }
                        }
                    }
                    double totalHour = Math.Round((totalMinute / 60), 4);
                    var data = new StudyTimeModel();
                    data.FullName = student.FullName;
                    data.Avatar = student.Avatar;
                    data.UserCode = student.UserCode;
                    data.Time = totalHour;
                    result.Add(data);
                }
            }
            if (currentUser.RoleId == (int)RoleEnum.teacher)
            {
                listStudent = listStudent.Where(x => x.DepartmentId == currentUser.DepartmentId).ToList();
                totalRow = listStudent.Count;
                // Phân trang
                int startIndex = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                listStudent = listStudent.Skip(startIndex).Take(baseSearch.PageSize).ToList();
                foreach (var student in listStudent)
                {
                    double totalMinute = 0;
                    var studentVideoCompleted = listLessonVideoCompleted.Where(x => x.UserId == student.UserInformationId).Select(x => x.LessonVideoId).ToList();
                    if (studentVideoCompleted.Count > 0)
                    {
                        foreach (var lessonVideoId in studentVideoCompleted)
                        {
                            var lessonVideo = await dbContext.tbl_LessonVideo.SingleOrDefaultAsync(x => x.Enable == true && x.Id == lessonVideoId);
                            if (lessonVideo != null)
                            {
                                totalMinute += lessonVideo.Minute;
                            }
                        }
                    }
                    double totalHour = Math.Round((totalMinute / 60), 4);
                    var data = new StudyTimeModel();
                    data.FullName = student.FullName;
                    data.Avatar = student.Avatar;
                    data.UserCode = student.UserCode;
                    data.Time = totalHour;
                    result.Add(data);
                }
            }

            return new AppDomainResult<StudyTimeModel> { TotalRow = totalRow, Data = result };
        }
        #endregion
    }
}*/