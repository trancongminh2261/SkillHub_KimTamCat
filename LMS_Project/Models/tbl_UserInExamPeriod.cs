using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_UserInExamPeriod : DomainEntity
    {
        public int UserId { get; set; }
        public int ExamPeriodId { get; set; }
        public tbl_UserInExamPeriod() : base() { }
        public tbl_UserInExamPeriod(object model) : base(model) { }
    }
}