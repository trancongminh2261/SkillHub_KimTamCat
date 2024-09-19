using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.UserInExamPeriod
{
    public class UserInExamPeriodAvailable
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Avatar { get; set; }
    }
}