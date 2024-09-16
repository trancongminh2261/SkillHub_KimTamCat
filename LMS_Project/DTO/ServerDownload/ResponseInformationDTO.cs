using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO.ServerDownload
{
    public class ResponseInformationDTO
    {
        public string _id { get; set; }
        public string client_id { get; set; }
        public string product_id { get; set; }
        public string folder { get; set; }
        public string status { get; set; }
        public List<string> domains { get; set; }
        public double? used_size { get; set; }
        public double? start_date { get; set; }
        public double? end_date { get; set; }
        public double? next_due_date { get; set; }
        public Specs specs { get; set; }
        public string notes { get; set; }
        public double? created_at { get; set; }
        public string created_by { get; set; }
        public string client_name { get; set; }
        public string client_email { get; set; }
        public string client_phone_number { get; set; }
        public string product_name { get; set; }
        public bool? have_drm { get; set; }
        public string prefix { get; set; }
        /*public bool? dev_mode { get; set; }
        public string assignee_id { get; set; }
        public string assignee_name { get; set; }
        public string temp_renews { get; set; }*/
    }
    public class Specs
    {
        public double? storage { get; set; }
        public double? domain { get; set; }
    }
}