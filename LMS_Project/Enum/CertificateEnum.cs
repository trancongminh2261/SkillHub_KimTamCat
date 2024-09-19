using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Enum
{
    public class CertificateEnum
    {
        public enum Status
        {
            /// Valid - còn hạn
            /// Expired - hết hạn
            /// Indefinitely - vô thời hạn
            Valid = 1,
            Expired,
            Indefinitely
        }
    }
}