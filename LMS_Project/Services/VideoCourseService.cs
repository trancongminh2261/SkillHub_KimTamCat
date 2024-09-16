using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.DTO;
using LMS_Project.LMS;
using LMS_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
    public class VideoCourseService
    {
        public static async Task<tbl_VideoCourse> GetById(int id, tbl_UserInformation userlog)
        {
            using (var db = new lmsDbContext())
            {
                var data = await db.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Id == id);
                data.TotalLesson = await GetTotalLesson(data.Id);
                data.TotalSection= await GetTotalSection(data.Id);
                data.TotalMinute= await GetTotalMinute(data.Id);
                data.TagModels = await GetTag(data.Tags);
                if (userlog.RoleId == ((int)RoleEnum.student))
                {
                    var videoCourseStudent = await db.tbl_VideoCourseStudent.FirstOrDefaultAsync(x => x.VideoCourseId == data.Id && x.UserId == userlog.UserInformationId && x.Enable == true);
                    if (videoCourseStudent == null)
                        data.Status = 1;
                    else data.Status = videoCourseStudent.Status;
                }
                return data;

            }
        }
        public static async Task<int> GetTotalLesson(int videoCourseId)
        {
            using (var db = new lmsDbContext())
            {
                int result = 0;
                var sections = await db.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true).Select(x=>x.Id).ToListAsync();
                if (sections.Any())
                {
                    result += await db.tbl_LessonVideo.Where(x => sections.Contains(x.SectionId.Value) && x.Enable == true).CountAsync();
                }
                return result;
            }
        }
        public static async Task<int> GetTotalSection(int videoCourseId)
        {
            using (var db = new lmsDbContext())
            {
                int result = await db.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true).CountAsync();
                return result;
            }
        }
        public static async Task<int> GetTotalMinute(int videoCourseId)
        {
            using (var db = new lmsDbContext())
            {

                int result = 0;
                var sections = await db.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true).Select(x => x.Id).ToListAsync();
                if (sections.Any())
                {
                    var lesson = await db.tbl_LessonVideo.Where(x => sections.Contains(x.SectionId.Value) && x.Enable == true).Select(x=>x.Minute).ToListAsync();
                    if (lesson.Any())
                        result += lesson.Sum();
                }
                return result;
            }
        }
        public static async Task<List<TagModel>> GetTag(string tagIds)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var listTagIds = tagIds.Split(',').ToList();
                    var tags = await db.tbl_Tag.Where(x => listTagIds.Contains(x.Id.ToString())).ToListAsync();
                    if (!tags.Any())
                        return null;
                    return tags.Select(x => new TagModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();
                }
                catch {
                    return null;
                }
            }
        }
        public static async Task<tbl_VideoCourse> Insert(VideoCourseCreate videoCourseCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var model = new tbl_VideoCourse(videoCourseCreate);
                    var certificateConfig = await db.tbl_CertificateConfig.SingleOrDefaultAsync(x => x.Id == videoCourseCreate.CertificateConfigId);
                    if (certificateConfig == null)
                        throw new Exception("Không tìm thấy mẫu chứng chỉ");

                    model.CreatedBy = model.ModifiedBy = user.FullName;
                    model.TotalRate = 0;
                    model.TotalStudent = 0;
                    model.Active = false;
                    db.tbl_VideoCourse.Add(model);
                    await db.SaveChangesAsync();
                    return model;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task ReloadStudyRouteDetailIndex(int studyRouteId, lmsDbContext db)
        {
                var details = await db.tbl_StudyRouteDetail
                .Where(x => x.StudyRouteId == studyRouteId && x.Enable == true)
                .OrderBy(x => x.Index)
                .ToListAsync();
                if (details.Any())
                {
                    int index = 1;
                    foreach (var item in details)
                    {
                        item.Index = index;
                        index++;
                    }
                    await db.SaveChangesAsync();
                }
                
        }
        public static async Task<tbl_VideoCourse> Update(VideoCourseUpdate videoCourseUpdate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var model = new tbl_VideoCourse(videoCourseUpdate);
                    var entity = await db.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Id == model.Id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.CertificateConfigId = model.CertificateConfigId ?? entity.CertificateConfigId;

                    var certificateConfig = await db.tbl_CertificateConfig.SingleOrDefaultAsync(x => x.Id == entity.CertificateConfigId);
                    if (certificateConfig == null)
                        throw new Exception("Không tìm thấy mẫu chứng chỉ");

                    entity.Tags = model.Tags ?? entity.Tags;
                    entity.Name = model.Name ?? entity.Name;
                    entity.Thumbnail = model.Thumbnail ?? entity.Thumbnail;
                    entity.Description = model.Description ?? entity.Description;
                    entity.Active = model.Active ?? entity.Active;
                    entity.BeforeCourseId = videoCourseUpdate.BeforeCourseId ?? entity.BeforeCourseId;
                    var isChange = false;
                    if (model.IsPublic != null && model.IsPublic != entity.IsPublic)
                        isChange = true;
                    entity.IsPublic = model.IsPublic ?? entity.IsPublic;
                    await db.SaveChangesAsync();

                    //nếu khóa này không public thì xóa khỏi lộ trình học
                    if (isChange == true && entity.IsPublic == false)
                    {
                        var studyRouteDetails = await db.tbl_StudyRouteDetail.Where(x => x.VideoCourseId == entity.Id && x.Enable == true).ToListAsync();
                        if (studyRouteDetails.Count > 0)
                        {
                            foreach (var studyRouteDetail in studyRouteDetails)
                            {
                                studyRouteDetail.Enable = false;
                                await db.SaveChangesAsync();
                                await ReloadStudyRouteDetailIndex(studyRouteDetail.StudyRouteId, db);
                            }

                        }
                    }
                    //nếu cập nhật khóa học này cho phép tất cả phòng ban thì add vào lộ trình tất cả phòng ban
                    if (isChange == true && entity.IsPublic == true)
                    {
                        var listStudyRouteId = await db.tbl_StudyRoute.Where(x => x.Enable == true).Select(x => x.Id).ToListAsync();
                        if (listStudyRouteId.Count > 0)
                        {
                            var listStudyRouteDetail = await db.tbl_StudyRouteDetail.Where(x => x.Enable == true && x.VideoCourseId == entity.Id).ToListAsync();
                            var sqlBag = new ConcurrentBag<string>();
                            string datenow = DateTime.Now.ToString("yyyy-MM-dd");
                            Parallel.ForEach(listStudyRouteId, item =>
                            {
                                bool shouldInsert = true;
                                foreach (var detail in listStudyRouteDetail)
                                {
                                    if (detail.StudyRouteId == item && detail.VideoCourseId == entity.Id)
                                    {
                                        shouldInsert = false;
                                        break;
                                    }
                                }

                                if (shouldInsert)
                                {
                                    string insertSQL = $"INSERT INTO tbl_StudyRouteDetail(StudyRouteId, VideoCourseId, \"Index\", Enable, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn) " +
                                    $"VALUES ({item}, {entity.Id}, 9999, 1, N'{user.FullName}', '{datenow}', N'{user.FullName}', '{datenow}'); ";
                                    sqlBag.Add(insertSQL);
                                }
                            });
                            var sql = string.Join("", sqlBag);
                            if (sql != null && sql != "")
                            {
                                db.Database.ExecuteSqlCommand(sql);
                            }
                            foreach(var item in listStudyRouteId)
                            {
                                await ReloadStudyRouteDetailIndex(item, db);
                            }
                        }
                    }
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
                try
                {
                    var entity = await db.tbl_VideoCourse.SingleOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Enable = false;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<AppDomainResult> GetAll(VideoCourseSearch search,tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (search == null) return new AppDomainResult { TotalRow = 0, Data = null };
                if (user.RoleId == ((int)RoleEnum.student))
                    search.DepartmentIds = user.DepartmentId.ToString();
                string sql = $"Get_VideoCourse @PageIndex = {search.PageIndex}," +
                    $"@PageSize = {search.PageSize}," +
                    $"@Name = N'{search.Name ?? ""}'," +
                    $"@Tags = N'{search.Tags ?? ""}'," +
                    $"@RoleId = {user.RoleId ?? 0}," +
                    $"@Status = {search.Status ?? 0}," +
                    $"@Uid = {user.UserInformationId}," +
                    $"@UserId = {user.UserInformationId}," +
                    $"@DepartmentIds = N'{search.DepartmentIds}'," +
                    $"@Sort = {search.Sort}," +
                    $"@SortType = {search.SortType}";
                var data = await db.Database.SqlQuery<Get_VideoCourse>(sql).ToListAsync();
                var myCourses = await db.tbl_VideoCourseStudent
                    .Where(x => x.UserId == user.UserInformationId && x.Enable == true).Select(x => x.VideoCourseId).ToListAsync();
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                if (user.RoleId == ((int)RoleEnum.admin) || user.RoleId == ((int)RoleEnum.teacher))
                {
                    var result = (from i in data
                                  select new tbl_VideoCourse
                                  {
                                      Active = i.Active,
                                      BeforeCourseId = i.BeforeCourseId,
                                      BeforeCourseName = i.BeforeCourseName,
                                      CreatedBy = i.CreatedBy,
                                      CreatedOn = i.CreatedOn,
                                      Description = i.Description,
                                      Enable = i.Enable,
                                      Id = i.Id,
                                      ModifiedBy = i.ModifiedBy,
                                      ModifiedOn = i.ModifiedOn,
                                      Name = i.Name,
                                      Tags = i.Tags,
                                      Thumbnail = i.Thumbnail,
                                      IsPublic = i.IsPublic,
                                      TotalRate = i.TotalRate,
                                      TotalLesson = i.TotalLesson,
                                      TotalSection = i.TotalSection,
                                      TotalMinute = i.TotalMinute,
                                      TotalStudent = i.TotalStudent,
                                      TagModels = Task.Run(() => GetTag(i.Tags)).Result,
                                      Disable = false,
                                      CertificateConfigId = i.CertificateConfigId,
                                      CertificateConfigName = i.CertificateConfigName
                                  }).ToList();


                    return new AppDomainResult { TotalRow = totalRow, Data = result };
                }
                else
                {
                    var result = (from i in data
                                  join v in myCourses on i.BeforeCourseId equals v into pg
                                  from v in pg.DefaultIfEmpty()
                                  select new VideoCourseByStudent
                                  {
                                      //BeforeCourseId = i.BeforeCourseId,
                                      //BeforeCourseName = i.BeforeCourseName,
                                      CreatedBy = i.CreatedBy,
                                      CreatedOn = i.CreatedOn,
                                      Description = i.Description,
                                      Enable = i.Enable,
                                      Id = i.Id,
                                      ModifiedBy = i.ModifiedBy,
                                      ModifiedOn = i.ModifiedOn,
                                      Name = i.Name,
                                      Tags = i.Tags,
                                      Thumbnail = i.Thumbnail,
                                      IsPublic = i.IsPublic,
                                      TotalRate = i.TotalRate,
                                      TotalLesson = i.TotalLesson,
                                      TotalSection = i.TotalSection,
                                      TotalMinute = i.TotalMinute,
                                      TotalStudent = i.TotalStudent,
                                      TagModels = Task.Run(() => GetTag(i.Tags)).Result,
                                      Status = i.Status,
                                      Disable = user.RoleId != ((int)RoleEnum.student) ? false
                                      : i.BeforeCourseId == 0 ? false
                                      : v.HasValue ? false : true,
                                      CertificateConfigId = i.CertificateConfigId,
                                      CertificateConfigName = i.CertificateConfigName,
                                      CompletedPercent = i.CompletedPercent
                                  }).ToList();


                    return new AppDomainResult { TotalRow = totalRow, Data = result };
                }
            }
        }

        public static async Task<AppDomainResult> GetAllV2(VideoCourseSearchV2 baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) return new AppDomainResult { TotalRow = 0, Data = null };
                if (user.RoleId != ((int)RoleEnum.admin))
                    baseSearch.DepartmentId = user.DepartmentId;
                string sql = $"Get_VideoCourseV2 @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Tags = N'{baseSearch.Tags ?? ""}'," +
                    $"@Search = N'{baseSearch.Search ?? ""}'," +
                    $"@RoleId = {user.RoleId ?? 0}," +
                    $"@Status = {baseSearch.Status ?? 0}," +
                    $"@UserId = {user.UserInformationId}," +
                    $"@DepartmentId = {baseSearch.DepartmentId ?? 0}," +
                    $"@SortType = {baseSearch.SortType}";
                if (baseSearch.IsPublic.HasValue)
                    sql += $",@IsPublic = {baseSearch.IsPublic}";
                var data = await db.Database.SqlQuery<Get_VideoCourse>(sql).ToListAsync();
                /*var myCourses = await db.tbl_VideoCourseStudent
                    .Where(x => x.UserId == user.UserInformationId && x.Enable == true).Select(x => x.VideoCourseId).ToListAsync();*/
                if (!data.Any()) return new AppDomainResult { TotalRow = 0, Data = null };
                var totalRow = data[0].TotalRow;
                if (user.RoleId == ((int)RoleEnum.admin) || user.RoleId == ((int)RoleEnum.teacher))
                {
                    var result = (from i in data
                                  select new tbl_VideoCourse
                                  {
                                      Active = i.Active,
                                      BeforeCourseId = i.BeforeCourseId,
                                      BeforeCourseName = i.BeforeCourseName,
                                      CreatedBy = i.CreatedBy,
                                      CreatedOn = i.CreatedOn,
                                      Description = i.Description,
                                      Enable = i.Enable,
                                      Id = i.Id,
                                      ModifiedBy = i.ModifiedBy,
                                      ModifiedOn = i.ModifiedOn,
                                      Name = i.Name,
                                      Tags = i.Tags,
                                      Thumbnail = i.Thumbnail,
                                      IsPublic = i.IsPublic,
                                      TotalRate = i.TotalRate,
                                      TotalLesson = i.TotalLesson,
                                      TotalSection = i.TotalSection,
                                      TotalMinute = i.TotalMinute,
                                      TotalStudent = i.TotalStudent,
                                      TagModels = Task.Run(() => GetTag(i.Tags)).Result,
                                      Disable = false,
                                      CertificateConfigId = i.CertificateConfigId,
                                      CertificateConfigName = i.CertificateConfigName
                                  }).ToList();
                    return new AppDomainResult { TotalRow = totalRow, Data = result };
                }
                else
                {
                    var result = (from i in data                                
                                  select new VideoCourseByStudent
                                  {
                                      //BeforeCourseId = i.BeforeCourseId,
                                      //BeforeCourseName = i.BeforeCourseName,
                                      CreatedBy = i.CreatedBy,
                                      CreatedOn = i.CreatedOn,
                                      Description = i.Description,
                                      Enable = i.Enable,
                                      Id = i.Id,
                                      ModifiedBy = i.ModifiedBy,
                                      ModifiedOn = i.ModifiedOn,
                                      Name = i.Name,
                                      Tags = i.Tags,
                                      Thumbnail = i.Thumbnail,
                                      IsPublic = i.IsPublic,
                                      TotalRate = i.TotalRate,
                                      TotalLesson = i.TotalLesson,
                                      TotalSection = i.TotalSection,
                                      TotalMinute = i.TotalMinute,
                                      TotalStudent = i.TotalStudent,
                                      TagModels = Task.Run(() => GetTag(i.Tags)).Result,
                                      Status = i.Status,
                                      /*Disable = user.RoleId != ((int)RoleEnum.student) ? false
                                      : i.BeforeCourseId == 0 ? false
                                      : v.HasValue ? false : true,*/
                                      Disable = false,
                                      CertificateConfigId = i.CertificateConfigId,
                                      CertificateConfigName = i.CertificateConfigName,
                                      CompletedPercent = i.CompletedPercent
                                  }).ToList();


                    return new AppDomainResult { TotalRow = totalRow, Data = result };
                }
            }
        }

        

        public static async Task<VideoCourseStatusDTO> GetStatus(VideoCourseSearchV2 baseSearch, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new VideoCourseSearchV2();                
                if (user.RoleId != ((int)RoleEnum.admin))
                    baseSearch.DepartmentId = user.DepartmentId;
                string sql = $"Get_VideoCourseV2 @PageIndex = {baseSearch.PageIndex}," +
                    $"@PageSize = {baseSearch.PageSize}," +
                    $"@Tags = N'{baseSearch.Tags ?? ""}'," +
                    $"@Search = N'{baseSearch.Search ?? ""}'," +
                    $"@RoleId = {user.RoleId ?? 0}," +
                    $"@Status = {baseSearch.Status ?? 0}," +
                    $"@UserId = {user.UserInformationId}," +
                    $"@DepartmentId = {baseSearch.DepartmentId ?? 0}," +
                    $"@SortType = {baseSearch.SortType}";
                if (baseSearch.IsPublic.HasValue)
                    sql += $",@IsPublic = {baseSearch.IsPublic}";
                var data = await db.Database.SqlQuery<VideoCourseStatusDTO>(sql).FirstOrDefaultAsync();
                /*var myCourses = await db.tbl_VideoCourseStudent
                    .Where(x => x.UserId == user.UserInformationId && x.Enable == true).Select(x => x.VideoCourseId).ToListAsync();*/
                if (data == null) return new VideoCourseStatusDTO();
                return data;
            }
        }

        /// <summary>
        /// 1 - Chưa học 
        /// 2 - Đang học 
        /// 3 - Hoàn thành
        /// </summary>
        /// <param name="videoCourseId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<int> GetStatus(int videoCourseId, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                var result = 1;
                var entity = await db.tbl_VideoCourseStudent
                    .Where(x => x.UserId == user.UserInformationId && x.VideoCourseId == videoCourseId).FirstOrDefaultAsync();
                if(entity != null)
                    result = 2;
                var lastSection = await db.tbl_Section.Where(x => x.VideoCourseId == videoCourseId && x.Enable == true)
                    .OrderByDescending(x => x.Index).Select(x=>x.Id).FirstOrDefaultAsync();
                var completed = await db.tbl_SectionCompleted.AnyAsync(x => x.SectionId == lastSection && x.UserId == user.UserInformationId);
                if (completed)
                    result = 3;
                return result;
            }
        }
        public class VideoCourseOverview
        { 
            public int Id { get; set; }
            public string SectionName { get; set; }
            public string LessonVideoName { get; set; }
        }
        public static async Task<object> GetVideoCourseOverview(int videoCourseId)
        {
            using (var db = new lmsDbContext())
            {
                string sql = $"Get_VideoCourseOverview @VideoCourseId = {videoCourseId}";
                var data = await db.Database.SqlQuery<VideoCourseOverview>(sql).ToListAsync();
                if (!data.Any()) return null;
                var result = data.GroupBy(s => s.SectionName)
                    .Select(s => new
                    {
                        SectionName = s.Key,
                        LessonVideo = s.GroupBy(l =>l.LessonVideoName)
                        .Where(x=> !string.IsNullOrEmpty(x.Key))
                        .Select(l => new
                        {
                            LessonVideoName = l.Key
                        }).ToList()
                    }).ToList();
                return result;
            }
        }
        public class ListIdDetailDTO
        { 
            public int SectionId { get; set; }
            public List<int> LessonIds { get; set; }
        }
        public static async Task<List<ListIdDetailDTO>> GetListIdDetail(int videoCourseId)
        {
            using (var db = new lmsDbContext())
            {
                var result = new List<ListIdDetailDTO>();
                var sectionIds = await db.tbl_Section
                    .Where(x => x.VideoCourseId == videoCourseId && x.Enable == true)
                    .OrderBy(x => x.Index)
                    .Select(x => x.Id).ToListAsync();
                if (sectionIds.Any())
                {
                    foreach (var sectionId in sectionIds)
                    {
                        var lessonIds = await db.tbl_LessonVideo
                            .Where(x => x.SectionId == sectionId && x.Enable == true)
                            .OrderBy(x=>x.Index)
                            .Select(x=>x.Id).ToListAsync();
                        result.Add(new ListIdDetailDTO
                        {
                            SectionId = sectionId,
                            LessonIds = lessonIds
                        });
                    }
                }
                return result;
            }
        }
    }
}