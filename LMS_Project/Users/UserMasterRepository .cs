using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project
{
    
        public class UserMasterRepository : IDisposable  
        {
        // SECURITY_DBEntities it is your context class
        lmsDbContext context = new lmsDbContext();
        //This method is used to check and valIdate the user credentials
        public tbl_UserInformation ValIdateUser(string username, string password)
        {
            var encryptPassword = Encryptor.Encrypt(password);
            return context.tbl_UserInformation.FirstOrDefault(user =>
            user.Email == username
            && user.Password == encryptPassword);
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }  
}