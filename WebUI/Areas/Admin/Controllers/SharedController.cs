using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Admin.Controllers
{
    public class SharedController : Controller
    {
        //
        // GET: /Admin/Shared/

        public ActionResult toper()
        {
            return View();
        }


        public ActionResult footer()
        {
            return View();
        }
    }
}
