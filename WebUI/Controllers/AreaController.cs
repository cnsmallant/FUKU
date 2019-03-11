using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;

namespace WebUI.Controllers
{
    public class AreaController : Controller
    {
        //
        // GET: /Area/
        D8MallEntities db = new D8MallEntities();
        [HttpGet]
        public JsonResult Cascade(string parentid)
        {
            try
            {
                var result = db.com_area.Where(c => c.com_area_parentid == parentid).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
