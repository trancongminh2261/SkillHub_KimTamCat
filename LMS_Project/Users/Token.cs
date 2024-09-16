using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Users
{
    public class Token
    {
        
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public string Error { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
       
    }
}
