using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneUI.Controllers
{
    public class RegisterController : Controller
    {
        D8MallEntities db = new D8MallEntities();
        //
        // GET: /Register/

        public ActionResult Index()
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


        public JsonResult RegisterRegUserName()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_tel = JSONHelper.JsonToString(jsonstr, "user_basic_tel");
                int phone = db.user_basic.Where(u => u.user_basic_tel == user_basic_tel).Count();
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
        public JsonResult RegisterRegUserPhone()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_login = JSONHelper.JsonToString(jsonstr, "user_basic_login");
                int phone = db.user_basic.Where(u => u.user_basic_tel == user_basic_login).Count();
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
        [HttpPost]
        public JsonResult RegisterCreate()
        {
            try
            {
                string result = string.Empty;
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_login = JSONHelper.JsonToString(jsonstr, "user_basic_login");
                var user_basic_pwd = TDESHelper.EncryptString(JSONHelper.JsonToString(jsonstr, "user_basic_pwd"));
                var user_basic_tel = JSONHelper.JsonToString(jsonstr, "user_basic_tel");
                var user_detail_gender = JSONHelper.JsonToString(jsonstr, "user_detail_gender");
                var user_detail_birthday = JSONHelper.JsonToString(jsonstr, "user_detail_birthday");
                var user_basic_email = JSONHelper.JsonToString(jsonstr, "user_basic_email");
                var username = db.user_basic.Where(u => u.user_basic_login == user_basic_login).Count();
                var phone = db.user_basic.Where(u => u.user_basic_tel == user_basic_tel).Count();
                if (username > 0)
                {
                    result = "UNO";
                }
                else if (phone > 0)
                {
                    result = "PNO";
                }
                else
                {
                    user_basic basic = new user_basic();
                    string guid = Guid.NewGuid().ToString("N");
                    basic.user_basic_id = guid;
                    basic.user_basic_login = user_basic_login;
                    basic.user_basic_pwd = user_basic_pwd;
                    basic.user_basic_tel = user_basic_tel;
                    basic.user_basic_email = user_basic_email;
                    basic.user_basic_status = "0";
                    basic.user_basic_emails = "未认证";
                    basic.user_basic_tels = "已认证";
                    basic.user_basic_adddate = DateTime.Now;
                    basic.user_basic_editdate = DateTime.Now;
                    db.user_basic.Add(basic);
                    int result_basic = db.SaveChanges();

                    if (result_basic > 0)
                    {
                        user_detail detail = new user_detail();
                        detail.user_basic_id = guid;
                        detail.user_detail_id = Guid.NewGuid().ToString("N");
                        detail.user_detail_phone = user_basic_tel;
                        detail.user_detail_tel = user_basic_tel;
                        detail.user_detail_birthday = user_detail_birthday;
                        detail.user_detail_gender = user_detail_gender;

                        detail.user_detail_adddate = DateTime.Now;
                        db.user_detail.Add(detail);
                        int result_detail = db.SaveChanges();
                        if (result_detail > 0)
                        {
                            Response.Cookies["keys"].Value = TDESHelper.EncryptString(user_basic_login);
                            Response.Cookies["keys"].Expires = DateTime.Now.AddDays(1);
                            Response.Cookies["value"].Value = user_basic_pwd;
                            Response.Cookies["value"].Expires = DateTime.Now.AddDays(1);
                            result = "OK";
                        }
                        else
                        {
                            result = "NO";
                        }
                    }
                    else
                    {
                        result = "NO";
                    }
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
