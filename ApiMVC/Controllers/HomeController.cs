using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApiMVC.Controllers
{
    public class HomeController : Controller
    { 
        public ActionResult Call()
        {
            return View();
        }

        public ActionResult CallResponseSuccess()
        {
            return View();
        }

        public ActionResult CallResponseFailed()
        {
            return View();
        }
    }
}
