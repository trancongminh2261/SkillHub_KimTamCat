using LMS_Project.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.OptionDTO
{
    public class VideoCourseAllowAvailableDTO : DomainOptionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VideoCourseAllowAvailableDTO() : base() { }
        public VideoCourseAllowAvailableDTO(object model) : base(model) { }
    }
}