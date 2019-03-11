using GeetestSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using System.IO;

namespace WebUI.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {
            var list = db.com_banner.Where(b => b.com_banner_class == "登录Banner").OrderByDescending(b => b.com_banner_order).ToList();
            IList<WebUI.Controllers.GoodsController.com_banners> ilist = new List<WebUI.Controllers.GoodsController.com_banners>();
            foreach (var item in list)
            {
                WebUI.Controllers.GoodsController.com_banners ban = new WebUI.Controllers.GoodsController.com_banners();
                ban.com_banner_url = item.com_banner_url;
                ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                ilist.Add(ban);
            }
            ViewBag.bannerlist = ilist;
            return View();
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
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


        #region 找回密码
        public ActionResult RetrievePassword()
        {
            try
            {
                var list = db.com_banner.Where(b => b.com_banner_class == "登录Banner").OrderByDescending(b => b.com_banner_order).ToList();
                IList<WebUI.Controllers.GoodsController.com_banners> ilist = new List<WebUI.Controllers.GoodsController.com_banners>();
                foreach (var item in list)
                {
                    WebUI.Controllers.GoodsController.com_banners ban = new WebUI.Controllers.GoodsController.com_banners();
                    ban.com_banner_url = item.com_banner_url;
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
        /// 验证用户
        /// </summary>
        /// <returns></returns>
        public JsonResult RegUser()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_login = JSONHelper.JsonToString(jsonstr, "user_basic_login");
                int phone = db.user_basic.Where(u => u.user_basic_login == user_basic_login).Count();
                string result = string.Empty;
                if (phone > 0)
                {
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

        public JsonResult RegPhone()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_pwd = JSONHelper.JsonToString(jsonstr, "user_basic_pwd");
                int phone = db.user_basic.Where(u => u.user_basic_tel == user_basic_pwd).Count();
                string result = string.Empty;
                if (phone > 0)
                {
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

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        public JsonResult ResetPassword()
        {
            try
            {
                int result = 0;
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_pwd = JSONHelper.JsonToString(jsonstr, "user_basic_pwd");
                var user_basic_login = JSONHelper.JsonToString(jsonstr, "user_basic_login");
                var user = db.user_basic.Where(u => u.user_basic_login == user_basic_login).SingleOrDefault();
                user.user_basic_pwd = TDESHelper.EncryptString(user_basic_pwd);
                result = db.SaveChanges();
                if (result > 0)
                {
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
