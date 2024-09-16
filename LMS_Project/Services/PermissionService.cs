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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{
    public class PermissionService
    {
        public class RoleModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public static async Task<List<RoleModel>> GetRole()
        {
            var data = new List<RoleModel>()
            {
                new RoleModel { Id = ((int)RoleEnum.admin), Name = "Admin" },
                new RoleModel { Id = ((int)RoleEnum.teacher), Name = "Giáo viên" },
                new RoleModel { Id = ((int)RoleEnum.student), Name = "Học viên" },
                new RoleModel { Id = ((int)RoleEnum.manager), Name = "Quản lý" },
            };
            return data;
        }
        public static async Task<List<RoleModel>> GetRoleStaff()
        {
            var data = new List<RoleModel>()
            {
                new RoleModel { Id = ((int)RoleEnum.admin), Name = "Admin" },
                new RoleModel { Id = ((int)RoleEnum.teacher), Name = "Giáo viên" },
                new RoleModel { Id = ((int)RoleEnum.manager), Name = "Quản lý" },
            };
            return data;
        }
        public class PermissionCreate
        {
            public string Controller { get; set; }
            public string Action { get; set; }
            public string Description { get; set; }
        }
        public static async Task<tbl_Permission> Insert(PermissionCreate itemModel)
        {
            using (var db = new lmsDbContext())
            {
                var check = await db.tbl_Permission.AnyAsync(x => x.Controller.ToUpper() == itemModel.Controller.ToUpper()
                && x.Action.ToUpper() == itemModel.Action.ToUpper());
                if (check)
                    throw new Exception("Đã có");
                var model = new tbl_Permission
                {
                    Action = itemModel.Action,
                    Allowed = "",
                    Controller = itemModel.Controller,
                    Description = itemModel.Description
                };
                db.tbl_Permission.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }
        public class PermissionUpdate
        {
            public int Id { get; set; }
            /// <summary>
            /// danh sách Id quyền, mẫu 1,2,3
            /// </summary>
            public string Allowed { get; set; }
        }
        public static async Task<tbl_Permission> Update(PermissionUpdate model)
        {
            using (var db = new lmsDbContext())
            {
                var entity = await db.tbl_Permission.SingleOrDefaultAsync(x => x.Id == model.Id);
                if (entity == null)
                    throw new Exception("Không tìm thấy dữ liệu");
                entity.Allowed = model.Allowed;
                await db.SaveChangesAsync();
                return entity;
            }
        }
        public class PermissionSearch
        {
            public string search { get; set; }
        }
        public static async Task<List<PermissionModel>> GetAll(PermissionSearch baseSearch)
        {
            using (var db = new lmsDbContext())
            {
                if (baseSearch == null) baseSearch = new PermissionSearch();
                var data = await db.tbl_Permission.Where(x =>
               x.Controller.Contains(baseSearch.search)
               || x.Action.Contains(baseSearch.search)
               || string.IsNullOrEmpty(baseSearch.search))
                   .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToListAsync();
                var result = (from i in data
                              select new PermissionModel(i)).ToList();
                return result;
            }
        }

        public static void AutoMapRouter()
        {
            List<ControllerActionInfo> controllerActionInfoList = new List<ControllerActionInfo>();

            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(ApiController).IsAssignableFrom(type) && type.Name.EndsWith("Controller"));

            foreach (var controllerType in controllerTypes)
            {
                var controllerName = controllerType.Name.Replace("Controller", "");
                if (controllerName != "Base"
                    && controllerName != "AutoNoti"
                    && controllerName != "Permission"
                    )
                {
                    var actionNames = GetActionNamesForController(controllerType);
                    if (actionNames.Any())
                        actionNames.ForEach(item =>
                        {
                            controllerActionInfoList.Add(new ControllerActionInfo
                            {
                                ControllerName = controllerName,
                                ActionName = item
                            });
                        });
                }
            }
            using (var db = new lmsDbContext())
            {
                var dataMap = new List<tbl_Permission>();
                var permissions = db.tbl_Permission.ToList();
                foreach (var item in controllerActionInfoList)
                {
                    if (!permissions.Any(x => x.Controller == item.ControllerName && x.Action == item.ActionName))
                        dataMap.Add(new tbl_Permission
                        {
                            Controller = item.ControllerName,
                            Action = item.ActionName,
                            Allowed = ""
                        });
                }
                db.tbl_Permission.AddRange(dataMap);
                db.SaveChanges();

                //var users =  db.tbl_UserInformation.OrderBy(x=>x.UserInformationId).ToList();
                //foreach (var user in users)
                //{
                //    string baseCode = user.RoleId == ((int)RoleEnum.admin) ? "QTV"
                //                        : user.RoleId == ((int)RoleEnum.manager) ? "QL"
                //                        : user.RoleId == ((int)RoleEnum.teacher) ? "GV"
                //                        : user.RoleId == ((int)RoleEnum.student) ? "HV" : "";
                //    int count = db.tbl_UserInformation.Count(x => x.RoleId == user.RoleId
                //            && x.CreatedOn.Value.Year == user.CreatedOn.Value.Year
                //            && x.CreatedOn.Value.Month == user.CreatedOn.Value.Month
                //            && x.CreatedOn.Value.Day == user.CreatedOn.Value.Day
                //            && x.UserInformationId < user.UserInformationId);
                //    user.UserCode = AssetCRM.InitCode(baseCode, user.CreatedOn.Value, count + 1);
                //    db.SaveChanges();
                //}


                return;
            }
        }
        private static List<string> GetActionNamesForController(Type controllerType)
        {
            var actionNames = new List<string>();

            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RouteAttribute), true);
                if (attributes.Length > 0)
                {
                    var actionName = method.Name;
                    actionNames.Add(actionName);
                }
            }

            return actionNames;
        }
        public class ControllerActionInfo
        {
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
        }
    }
}