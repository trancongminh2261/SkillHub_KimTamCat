using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Areas.Models
{
    public class AppDomainResult
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public int ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public bool NoContent { get; set; }
        public int TotalRow { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class AppDomainResult<T>
    {
        public bool Success { get; set; }
        public IList<T> Data { get; set; }
        public int ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public int TotalRow { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}