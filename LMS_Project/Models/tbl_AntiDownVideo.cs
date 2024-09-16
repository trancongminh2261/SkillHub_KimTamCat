using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public class tbl_AntiDownVideo : DomainEntity
    {
        /// <summary>
        /// id sau khi upload video
        /// </summary>
        public string VideoUploadId { get; set; }
        /// <summary>
        /// tiêu đề video
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// thời lượng của video ( tính bằng giây )
        /// </summary>
        public int Minute { get; set; }
        /// <summary>
        /// ảnh bìa
        /// </summary>
        public string Thumbnail { get; set; }
        /// <summary>
        /// Link trả ra từ server
        /// </summary>
        public string VideoUrl { get; set; }
        public tbl_AntiDownVideo() : base() { }
        public tbl_AntiDownVideo(object model) : base(model) { }
    }
}