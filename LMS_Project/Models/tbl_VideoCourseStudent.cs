namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Reflection;

    public class tbl_VideoCourseStudent : DomainEntity
    {
        public int? UserId { get; set; }
        public int? VideoCourseId { get; set; }
        public int? MyRate { get; set; }
        public string RateComment { get; set; }
        /// <summary>
        /// 1 - Chưa học 
        /// 2 - Đang học
        /// 3 - Hoàn thành
        /// </summary>
        public int Status { get; set; }
        public string StatusName { get; set; }
        public double CompletedPercent { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Thumbnail { get; set; }
        [NotMapped]
        public string Stag { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [NotMapped]
        public double TotalRate { get; set; }
        [NotMapped]
        public double TotalStudent { get; set; }
        public tbl_VideoCourseStudent() : base() { }
        public tbl_VideoCourseStudent(object model) : base(model) { }
    }
    public class Get_VideoCourseStudent : DomainEntity
    {
        public int? UserId { get; set; }
        public int? VideoCourseId { get; set; }
        public int? MyRate { get; set; }
        public string Name { get; set; }
	    public string Thumbnail { get; set; }
        public string Stag { get; set; }
		public string Description { get; set; }
        public double TotalRate { get; set; }
        public double TotalStudent { get; set; }
        public int TotalRow { get; set; }
    }
    public class Get_StudentInVideoCourse : DomainEntity
    {
        public int? UserId { get; set; }
        public int? VideoCourseId { get; set; }
        public int? MyRate { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public int Gender { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
        public int TotalRow { get; set; }
    }
    public class StudentInVideoCourseModel : DomainEntity
    {
        public int? UserId { get; set; }
        public int? VideoCourseId { get; set; }
        public int? MyRate { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public int Gender { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public StudentInVideoCourseModel() { }
        public StudentInVideoCourseModel(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        me.SetValue(this, item.GetValue(model));
                    }
                }
            }
        }
    }
}