using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Project.Controllers
{
    public class BaseController : Controller
    {

        protected void SetlinkInvoice(string url)
        {
            TempData["InvoiceURL"] = url;
        }
        protected void SetLinkOnePay(string url)
        {
            TempData["OnePayURL"] = url;
        }
    }
}
