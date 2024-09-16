using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project
{
    public class tbl_VideoConfigQuestion : DomainEntity
    {
        public int VideoConfigId { get; set; }
        public string Content { get; set; }
        public int Index { get; set; }
        public tbl_VideoConfigQuestion() : base() { }
        public tbl_VideoConfigQuestion(object model) : base(model) { }
    }
}