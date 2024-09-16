using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_VideoConfigOption : DomainEntity
    {
        public int VideoConfigQuestionId { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public tbl_VideoConfigOption() : base() { }
        public tbl_VideoConfigOption(object model) : base(model) { }
    }
}