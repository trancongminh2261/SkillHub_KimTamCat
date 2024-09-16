using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LMS_Project.Models
{
    public partial class tbl_UserInformation
    {
        [Key]
        public int UserInformationId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string UserCode { get; set; }
        public DateTime? DOB { get; set; } // ngày sinh
        public int? Gender { get; set; } = 2; //2 khác 1 là nam 0 là nữ
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Mobile { get; set; } // số điện thoại
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        public string Address { get; set; }
        public int? StatusId { get; set; } // 1 khóa 0 hoạt động
        public int? RoleId { get; set; } //1 admin 2 giáo viên 3 học viên
        public string RoleName { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        public string OneSignal_DeviceId { get; set; }
        public string Password { get; set; }

        public string KeyForgotPassword { get; set; }

        public DateTime? CreatedDateKeyForgot { get; set; }
        /// <summary>
        /// Ngày hoạt động
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string CMND { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        public string RefreshToken { get; set; }
        /// <summary>
        /// Hạn sử dụng refresh token
        /// </summary>
        public DateTime? RefreshTokenExpires { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// ngày dăng nhập gần nhất => để làm chức năng gửi thông báo không cần hiển thị
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
        /*/// <summary>
        /// thiết bị sử dụng
        /// 1 - Android
        /// 2 - IOS
        /// 3 - Máy tính
        /// </summary>
        public int? Device { get; set; }
        /// <summary>
        /// anh Chao Said: Cái này để múa
        /// </summary>
        public string Note { get; set; }*/
        public tbl_UserInformation() { }
        public tbl_UserInformation(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        me.SetValue(this, item.GetValue(model));
                    }
                }
            }
        }
    }
    public class Get_UserInformation
    {
        [Key]
        public int UserInformationId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public DateTime? DOB { get; set; } = DateTime.Now; // ngày sinh
        public int? Gender { get; set; } = 2; //2 khác 1 là nam 0 là nữ
        public string Mobile { get; set; } // số điện thoại
        public string Email { get; set; }
        public string Address { get; set; }
        public int? StatusId { get; set; } // 1 khóa 0 hoạt động
        public int? RoleId { get; set; } //1 admin 2 giáo viên 3 học viên
        public string RoleName { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } 
        public int? DistrictId { get; set; }
        public int? WardId { get; set; } 
        public string AreaName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string OneSignal_DeviceId { get; set; }
        public string Password { get; set; }

        public string KeyForgotPassword { get; set; }

        public DateTime? CreatedDateKeyForgot { get; set; }
        /// <summary>
        /// Ngày hoạt động
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string CMND { get; set; }
        public string NickName { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public string DepartmentName { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        public int TotalRow { get; set; }
    }
    public class UserInformationModel
    {
        public int UserInformationId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public DateTime? DOB { get; set; } // ngày sinh
        public int? Gender { get; set; } = 2; //2 khác 1 là nam 0 là nữ
        public string Mobile { get; set; } // số điện thoại
        public string Email { get; set; }
        public string Address { get; set; }
        public int? StatusId { get; set; } // 1 khóa 0 hoạt động
        public int? RoleId { get; set; } //1 admin 2 giáo viên 3 học viên
        public string RoleName { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        public string AreaName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string CMND { get; set; }
        public string NickName { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public string DepartmentName { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        public string OneSignal_DeviceId { get; set; }
        public UserInformationModel() { }
        public UserInformationModel(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        me.SetValue(this, item.GetValue(model));
                    }
                }
            }
        }
    }
}
