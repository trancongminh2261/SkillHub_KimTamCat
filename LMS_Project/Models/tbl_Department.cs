using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_Department : DomainEntity
    {
        /// <summary>
        /// tên phòng ban
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// xác định phòng ban cha ( nếu nó là thư mục gốc thì == null )
        /// </summary>
        public int? ParentDepartmentId { get; set; }
        /// <summary>
        /// đây là cờ phân biệt phòng ban xem nó có phải gốc không
        /// true = gốc => tự động set ParentDepartmentId = null = 0
        /// false = không phải gốc => nếu ParentDepartmentId = null = 0 thì báo lỗi
        /// </summary>
        public bool IsRoot { get; set; }
        public tbl_Department() : base() { }
        public tbl_Department(object model) : base(model) { }
    }
    public class Get_Department : DomainEntity
    {
        public string Name { get; set; }
        public int? ParentDepartmentId { get; set; }
        public bool IsRoot { get; set; }
        public int TotalRow { get; set; }
    }
}