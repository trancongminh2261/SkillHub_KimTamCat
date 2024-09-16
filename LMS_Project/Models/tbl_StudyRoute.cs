using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_StudyRoute : DomainEntity
    {
        public int DepartmentId { get; set; }
        /// <summary>
        /// Lựa chọn có học bài theo thứ tự hay không
        /// true - có
        /// false - không
        /// </summary>
        public bool LearnInOrder { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }

        public tbl_StudyRoute() : base() { }
        public tbl_StudyRoute(object model) : base(model) { }
    }
    public class Get_StudyRoute : DomainEntity
    {
        //phòng ban
        public int DepartmentId { get; set; }
        /// <summary>
        /// Lựa chọn có học bài theo thứ tự hay không
        /// true - có
        /// false - không
        /// </summary>
        public bool LearnInOrder { get; set; }
        public string DepartmentName { get; set; }
        public int TotalRow { get; set; }
    }
}