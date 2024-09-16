namespace LMS_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Reflection;
    using System.Threading.Tasks;

    public class DomainEntity
    {

        [Key]
        public int Id { get; set; }
        public bool? Enable { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        public DomainEntity() { }
        public DomainEntity(object model)
        {
            foreach (PropertyInfo me in this.GetType().GetProperties())
            {
                foreach (PropertyInfo item in model.GetType().GetProperties())
                {
                    if (me.Name == item.Name)
                    {
                        if (me.PropertyType == typeof(string))
                        {
                            me.SetValue(this, item.GetValue(model) == null ? null : item.GetValue(model).ToString());
                        }
                        else
                        {
                            me.SetValue(this, item.GetValue(model));
                        }
                    }
                }
            }
        }
    }
}