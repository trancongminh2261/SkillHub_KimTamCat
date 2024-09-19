using LMS_Project.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.UserInExamPeriod
{
    public class UserInExamPeriodDTO : DomainDTO
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public int ExamPeriodId { get; set; }
        public UserInExamPeriodDTO() : base() { }
        public UserInExamPeriodDTO(object model) : base(model) { }
    }
}