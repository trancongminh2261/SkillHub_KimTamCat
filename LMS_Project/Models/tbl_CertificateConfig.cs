namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class tbl_CertificateConfig : DomainEntity
    {
        /// <summary>
        /// Tên mẫu chứng chỉ
        /// </summary>
        public string Name { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Hình nền
        /// </summary>
        public string Background { get; set; }
        /// <summary>
        /// Mặt sau
        /// </summary>
        public string Backside { get; set; }
        public tbl_CertificateConfig() : base() { }
        public tbl_CertificateConfig(object model) : base(model) { }
    }
}