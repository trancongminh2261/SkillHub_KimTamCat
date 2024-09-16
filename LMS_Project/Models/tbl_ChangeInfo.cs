using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace LMS_Project.Models
{
    public class tbl_ChangeInfo : DomainEntity
    {
        public int UserInformationId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public DateTime? DOB { get; set; } // ngày sinh
        public int? Gender { get; set; } = 2; //2 khác 1 là nam 0 là nữ
        public string Mobile { get; set; } // số điện thoại
        public string Address { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        [NotMapped]
        public string AreaName { get; set; }
        [NotMapped]
        public string DistrictName { get; set; }
        [NotMapped]
        public string WardName { get; set; }
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Đã duyệt
        /// 3 - Không duyệt
        /// </summary>
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string CMND { get; set; }
        [NotMapped]
        public object Info { get; set; }
    }
    public class Get_ChangeInfo : DomainEntity
    {
        public int UserInformationId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? DOB { get; set; } // ngày sinh
        public int? Gender { get; set; } = 2; //2 khác 1 là nam 0 là nữ
        public string Mobile { get; set; } // số điện thoại
        public string Address { get; set; }
        public string Avatar { get; set; }
        public int? AreaId { get; set; } // tình/tp
        public int? DistrictId { get; set; }//quận/huyện
        public int? WardId { get; set; } // phường/xã 
        public string AreaName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        /// <summary>
        /// 1 - Chờ duyệt
        /// 2 - Đã duyệt
        /// 3 - Không duyệt
        /// </summary>
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Số chứng minh nhân dân
        /// </summary>
        public string CMND { get; set; }
        public string NickName { get; set; }

        public int TotalRow { get; set; }
    }
}