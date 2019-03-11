using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneUI.Controllers
{
    public class LoginController : Controller
    {
        D8MallEntities db = new D8MallEntities();
        //
        // GET: /Login/

        public ActionResult Index()
        {
            try
            {
                var list = db.com_banner.Where(b => b.com_banner_class == "登录Banner").OrderByDescending(b => b.com_banner_order).ToList();
                IList<GoodsController.com_banners> ilist = new List<GoodsController.com_banners>();
                foreach (var item in list)
                {
                    Controllers.GoodsController.com_banners ban = new Controllers.GoodsController.com_banners();
                    ban.com_banner_url = new Uri(item.com_banner_url).AbsolutePath;
                    ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                    ilist.Add(ban);
                }
                ViewBag.bannerlist = ilist;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserLogin()
        {
            try
            {

                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var uname = JSONHelper.JsonToString(jsonstr, "user_basic_login");
                var upwd = TDESHelper.EncryptString(JSONHelper.JsonToString(jsonstr, "user_basic_pwd"));
                var query = db.user_basic;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                string result = string.Empty;
                if (user_basic != null)
                {
                    Response.Cookies["keys"].Value = TDESHelper.EncryptString(uname);
                    Response.Cookies["keys"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["value"].Value = upwd;
                    Response.Cookies["value"].Expires = DateTime.Now.AddDays(1);
                    result = "OK";
                }
                else
                {
                    result = "NO";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
