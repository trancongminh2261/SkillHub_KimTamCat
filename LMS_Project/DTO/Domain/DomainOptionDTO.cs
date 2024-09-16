using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LMS_Project.DTO.Domain
{
    public class DomainOptionDTO
    {
        public DomainOptionDTO() { }
        public DomainOptionDTO(object model)
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