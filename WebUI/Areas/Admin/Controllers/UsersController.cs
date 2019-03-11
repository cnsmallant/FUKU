using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using Webdiyer.WebControls.Mvc;
using System.Text;
using System.IO;

namespace WebUI.Areas.Admin.Controllers
{
    [AdminVerification]
    public class UsersController : Controller
    {
        //
        // GET: /Admin/Users/

        D8MallEntities db = new D8MallEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewUsers(int pageid = 1)
        {
            try
            {
                var query = db.user_basic;
                var lists = query.OrderByDescending(u => u.user_basic_adddate).ToList();
                IList<user_basic> list = new List<user_basic>();
                ViewBag.usertotal = lists.Count;
                ViewBag.usernormal = query.Where(u => u.user_basic_status == "0").Count();
                ViewBag.userillegal = query.Where(u => u.user_basic_status == "1").Count();
                foreach (var item in lists)
                {
                    user_basic user = new user_basic();
                    user.user_basic_id = item.user_basic_id;
                    user.user_basic_code = item.user_basic_code;
                    user.user_basic_login = item.user_basic_login;
                    user.user_basic_pwd = item.user_basic_pwd;
                    user.user_basic_status = item.user_basic_status == "0" ? "正常" : "注销";
                    user.user_basic_tel = item.user_basic_tel;
                    user.user_basic_adddate = item.user_basic_adddate;
                    user.user_basic_editdate = item.user_basic_editdate;
                    list.Add(user);

                }
                IList<user_basic> mPage = PageCommon.PageList<user_basic>(pageid, 20, list.Count, list);
                return View("ViewUsers", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ViewUsersStatus(string user_basic_status, int pageid = 1)
        {
            try
            {
                var query = db.user_basic;
                var lists = query.Where(u => u.user_basic_status == user_basic_status).OrderByDescending(u => u.user_basic_adddate).ToList();
                IList<user_basic> list = new List<user_basic>();
                ViewBag.usertotal = lists.Count;
                ViewBag.usernormal = query.Where(u => u.user_basic_status == "0").Count();
                ViewBag.userillegal = query.Where(u => u.user_basic_status == "1").Count();
                foreach (var item in lists)
                {
                    user_basic user = new user_basic();
                    user.user_basic_id = item.user_basic_id;
                    user.user_basic_code = item.user_basic_code;
                    user.user_basic_login = item.user_basic_login;
                    user.user_basic_pwd = item.user_basic_pwd;
                    user.user_basic_status = item.user_basic_status == "0" ? "正常" : "注销";
                    user.user_basic_tel = item.user_basic_tel;
                    user.user_basic_adddate = item.user_basic_adddate;
                    user.user_basic_editdate = item.user_basic_editdate;
                    list.Add(user);

                }
                IList<user_basic> mPage = PageCommon.PageList<user_basic>(pageid, 20, list.Count, list);
                return View("ViewUsers", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [Purview]
        public FileResult ExportExcel()
        {
            var sbHtml = new StringBuilder();
            var list = db.user_basic.OrderByDescending(l => l.user_basic_adddate).ToList();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "用户昵称", "电子邮箱", "生日", "手机号码" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            foreach (var item in list)
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.user_basic_login);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.user_basic_email);
                var ud = db.user_detail.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault();
                if (ud != null)
                {
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", ud.user_detail_birthday);
                }
                else
                {
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "--");
                }

                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.user_basic_tel);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");

            byte[] fileContents = Encoding.UTF8.GetBytes(sbHtml.ToString());
            return File(fileContents, "application/ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + "用户文件.xls");
        }
        public ActionResult ViewUsersLogin(string keywords, int pageid = 1)
        {
            try
            {
                var query = db.user_basic;
                var lists = query.Where(u => u.user_basic_login.Contains(keywords)).OrderByDescending(u => u.user_basic_adddate).ToList();
                IList<user_basic> list = new List<user_basic>();
                ViewBag.usertotal = lists.Count;
                ViewBag.usernormal = query.Where(u => u.user_basic_status == "0").Count();
                ViewBag.userillegal = query.Where(u => u.user_basic_status == "1").Count();
                foreach (var item in lists)
                {
                    user_basic user = new user_basic();
                    user.user_basic_id = item.user_basic_id;
                    user.user_basic_code = item.user_basic_code;
                    user.user_basic_login = item.user_basic_login;
                    user.user_basic_pwd = item.user_basic_pwd;
                    user.user_basic_status = item.user_basic_status == "0" ? "正常" : "注销";
                    user.user_basic_tel = item.user_basic_tel;
                    user.user_basic_adddate = item.user_basic_adddate;
                    user.user_basic_editdate = item.user_basic_editdate;
                    list.Add(user);

                }
                IList<user_basic> mPage = PageCommon.PageList<user_basic>(pageid, 20, list.Count, list);
                return View("ViewUsers", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult UserStatus(string userstasus, string user_basic_id)
        {
            try
            {
                user_basic user = db.user_basic.Where(u => u.user_basic_id == user_basic_id).SingleOrDefault();
                user.user_basic_status = userstasus;
                user.user_basic_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("ViewUsers", "Users");
                }
                else
                {
                    string msg = Server.UrlEncode("操作失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult UserDetail(string user_basic_id)
        {
            try
            {
                user_basic user = db.user_basic.Where(u => u.user_basic_id == user_basic_id).SingleOrDefault();
                user_detail userdetail = db.user_detail.Where(u => u.user_basic_id == user_basic_id).SingleOrDefault();
                ViewBag.user_basic_id = user_basic_id;
                ViewBag.user_basic_code = user.user_basic_code;
                ViewBag.user_basic_login = user.user_basic_login;
                if (!string.IsNullOrEmpty(userdetail.user_detail_firstname) || !string.IsNullOrEmpty(userdetail.user_detail_lastname))
                {
                    ViewBag.user_basic_name = userdetail.user_detail_firstname + userdetail.user_detail_lastname;
                }
                else
                {
                    ViewBag.user_basic_name = "--";

                }
                ViewBag.user_basic_status = user.user_basic_status == "0" ? "正常" : "注销";
                ViewBag.user_basic_tel = user.user_basic_tel;
                if (!string.IsNullOrEmpty(user.user_basic_email))
                {
                    ViewBag.user_basic_email = user.user_basic_email;
                }
                else
                {
                    ViewBag.user_basic_email = "--";
                }
                ViewBag.user_detail_birthday = userdetail.user_detail_birthday;
                int user_detail_province = Convert.ToInt32(userdetail.user_detail_province);
                int user_detail_city = Convert.ToInt32(userdetail.user_detail_city);
                int user_detail_county = Convert.ToInt32(userdetail.user_detail_county);
                if (user_detail_province == 0 || user_detail_city == 0 || user_detail_county == 0)
                {
                    ViewBag.user_detail_address = "--";
                }
                else
                {
                    var province = db.com_area.Where(a => a.com_area_id == user_detail_province).SingleOrDefault().com_area_name;
                    var city = db.com_area.Where(a => a.com_area_id == user_detail_city).SingleOrDefault().com_area_name;
                    var county = db.com_area.Where(a => a.com_area_id == user_detail_county).SingleOrDefault().com_area_name;
                    ViewBag.user_detail_address = province + city + county + userdetail.user_detail_address;
                }
                ViewBag.qwsy = coupon(userdetail.user_basic_id, "0", "优惠券");
                ViewBag.mwsy = coupon(userdetail.user_basic_id, "0", "优惠码");
                ViewBag.qysy = coupon(userdetail.user_basic_id, "1", "优惠券");
                ViewBag.mysy = coupon(userdetail.user_basic_id, "1", "优惠码");

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public class pro_coupons : pro_coupon
        {
            public string user_coupon_id { get; set; }

            public string user_basic_id { get; set; }
            public string user_coupon_stastus { get; set; }

        }

        private IList<pro_coupons> coupon(string user_basic_id, string user_coupon_stastus, string pro_coupon_class)
        {
            IList<pro_coupons> list = new List<pro_coupons>();
            IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic_id & u.user_coupon_stastus == user_coupon_stastus).ToList();
            foreach (var item in ulist)
            {
                pro_coupons pros = new pro_coupons();
                pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;
                pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                pros.pro_coupon_num = "1";
                list.Add(pros);
            }
            var list_01 = list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == pro_coupon_class).ToList();
            return list_01;
        }



        /// <summary>
        /// 发放优惠券
        /// </summary>
        /// <returns></returns>
        public ActionResult UsersCouponing(int pageid = 1)
        {
            try
            {


                var query = db.user_basic;
                var lists = query.Where(u => u.user_basic_status == "0").OrderByDescending(u => u.user_basic_adddate).ToList();
                IList<user_basic> list = new List<user_basic>();
                foreach (var item in lists)
                {
                    user_basic user = new user_basic();
                    user.user_basic_id = item.user_basic_id;
                    user.user_basic_code = item.user_basic_code;
                    user.user_basic_login = item.user_basic_login;
                    user.user_basic_pwd = item.user_basic_pwd;
                    user.user_basic_status = item.user_basic_status == "0" ? "正常" : "注销";
                    user.user_basic_tel = item.user_basic_tel;
                    // var c = db.user_coupon.Where(u => u.user_basic_id == item.user_basic_id).ToList();
                    //var str = string.Empty;
                    //foreach (var items in c)
                    //{
                    //    str += db.pro_coupon.Where(cc => cc.pro_coupon_id == items.pro_coupon_id).SingleOrDefault().pro_coupon_name + "<br/>";
                    //}
                    // user.user_basic_pwd = str;
                    user.user_basic_pwd = db.user_coupon.Where(u => u.user_basic_id == item.user_basic_id).Count().ToString();
                    user.user_basic_adddate = item.user_basic_adddate;
                    user.user_basic_editdate = item.user_basic_editdate;
                    list.Add(user);

                }
                IList<user_basic> mPage = PageCommon.PageList<user_basic>(pageid, 20, list.Count, list);
                return View("UsersCouponing", mPage);
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpGet]
        public JsonResult GetCoupon()
        {
            try
            {
                IList<pro_coupon> c = db.pro_coupon.Where(p => p.pro_coupon_stastus == "1" & p.pro_coupon_enddate >= DateTime.Now).ToList();

                return Json(c, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 全部发放
        /// </summary>
        /// <param name="pro_coupon_id"></param>
        /// <returns></returns>
        public ActionResult UsersCouponFull(string pro_coupon_id)
        {
            try
            {
                var couponclass = db.pro_coupon.Where(p => p.pro_coupon_id == pro_coupon_id).SingleOrDefault().pro_coupon_class;
                var userlist = db.user_basic.Where(u => u.user_basic_status == "0").ToList();
                foreach (var item in userlist)
                {
                    user_coupon uc = new user_coupon();
                    uc.pro_coupon_id = pro_coupon_id;
                    uc.user_basic_id = item.user_basic_id;
                    uc.user_coupon_stastus = "0";
                    uc.pro_coupon_class = couponclass;
                    db.user_coupon.Add(uc);
                }

                int result = db.SaveChanges();
                if (result > 0)
                {
                    var js = @"<script> alert('操作成功！'); window.location.href='/Admin/Users/UsersCouponing';</script>";
                    return Content(js);
                }
                else
                {
                    string msg = Server.UrlEncode("操作失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 选择发放
        /// </summary>
        /// <returns></returns>
        public ActionResult UsersCouponSelect(string pro_coupon_id, string text)
        {
            try
            {
                var couponclass = db.pro_coupon.Where(p => p.pro_coupon_id == pro_coupon_id).SingleOrDefault().pro_coupon_class;
                string txt = Server.UrlDecode(text);
                string[] str = txt.Split(',');
                foreach (var item in str)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        user_coupon uc = new user_coupon();
                        uc.pro_coupon_id = pro_coupon_id;
                        uc.user_basic_id = item;
                        uc.user_coupon_stastus = "0";
                        uc.pro_coupon_class = couponclass;
                        db.user_coupon.Add(uc);
                    }
                }
                int result = db.SaveChanges();
                if (result > 0)
                {
                    var js = @"<script> alert('操作成功！'); window.location.href='/Admin/Users/UsersCouponing';</script>";
                    return Content(js);
                }
                else
                {
                    string msg = Server.UrlEncode("操作失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region 我的消息
        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="pageid"></param>
        /// <returns></returns>
        public ActionResult MessageCenter(int pageid = 1)
        {
            try
            {

                IList<user_info> list = db.user_info.OrderByDescending(i => i.user_info_date).ToList();
                IList<user_info> mPage = PageCommon.PageList<user_info>(pageid, 20, list.Count, list);
                return View("MessageCenter", mPage);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult AddMessageCenter()
        {
            try
            {
                return View();

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult DelMessageCenter(int id)
        {
            try
            {
                var ui = db.user_info.Where(u => u.user_info_id == id).SingleOrDefault();
                db.user_info.Remove(ui);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("MessageCenter", "Users");
                    
                }
                else
                {
                    return RedirectToAction("MessageCenter", "Users");
                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        public JsonResult AddMessage()
        {

            try
            {
                bool r = false;

                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_info_title = JSONHelper.JsonToString(jsonstr, "user_info_title");
                var user_info_content = JSONHelper.JsonToString(jsonstr, "user_info_content");
                var tid = JSONHelper.JsonToString(jsonstr, "tid");
                if (tid == "01")
                {
                    var user = db.user_basic.ToList();
                    foreach (var item in user)
                    {
                        user_info u = new user_info();
                        u.user_info_title = user_info_title;
                        u.user_info_content = user_info_content;
                        u.user_info_source = "cuckoo商城";
                        u.user_basic_id = item.user_basic_id;
                        u.user_info_date = DateTime.Now;
                        u.user_info_sta = "未读";
                        db.user_info.Add(u);

                    }

                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;

                    }
                }
                else if (tid == "02")
                {
                    var user = db.user_basic.ToList();
                    bool rs = false;
                    var eml = string.Empty;
                    foreach (var item in user)
                    {

                        if (!string.IsNullOrEmpty(item.user_basic_email))
                        {
                            eml += item.user_basic_email + ",";
                        }

                    }
                    rs = SendEmailQUN(eml.TrimEnd(','), user_info_title, user_info_content);
                    if (rs == true)
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;
                    }
                }
                else if (tid == "03")
                {
                    var user = db.user_basic.ToList();
                    var phoestr = string.Empty;
                    foreach (var item in user)
                    {
                        string rs = "OK";
                        if (!string.IsNullOrEmpty(item.user_basic_tel))
                        {
                            phoestr += item.user_basic_tel + ",";
                        }

                        SendSMS sms = new SendSMS();
                        rs = sms.SendSms(phoestr, "【福库商城】" + user_info_content + "【回复TD确定退订】", "OK");
                        if (rs == "OK")
                        {
                            r = true;
                        }
                        else
                        {
                            r = false;
                        }
                    }
                }

                if (r == true)
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



        public JsonResult AddMessageSingle()
        {

            try
            {
                bool r = false;
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_id = JSONHelper.JsonToString(jsonstr, "user_basic_id");
                var user_info_title = JSONHelper.JsonToString(jsonstr, "user_info_title");
                var user_info_content = JSONHelper.JsonToString(jsonstr, "user_info_content");
                var tid = JSONHelper.JsonToString(jsonstr, "tid");
                user_basic user = db.user_basic.Where(u => u.user_basic_id == user_basic_id).SingleOrDefault();
                if (tid == "01")
                {
                    user_info u = new user_info();
                    u.user_info_title = user_info_title;
                    u.user_info_content = user_info_content;
                    u.user_info_source = "cuckoo商城";
                    u.user_basic_id = user_basic_id;
                    u.user_info_date = DateTime.Now;
                    u.user_info_sta = "未读";
                    db.user_info.Add(u);
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;

                    }
                }
                else if (tid == "02")
                {


                    bool rs = false;
                    if (!string.IsNullOrEmpty(user.user_basic_email))
                    {
                        rs = SendEmail(user.user_basic_email, user_info_title, user_info_content);
                    }
                    if (rs == true)
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;
                    }

                }
                else if (tid == "03")
                {

                    string rs = "OK";
                    if (!string.IsNullOrEmpty(user.user_basic_tel))
                    {
                        SendSMS sms = new SendSMS();
                        rs = sms.SendSms(user.user_basic_tel, user_info_content, "OK");
                    }
                    if (rs == "OK")
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;
                    }

                }

                if (r == true)
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

        /// <summary>
        /// 邮箱发送
        /// </summary>
        /// <param name="user_basic_email"></param>
        /// <param name="user_info_title"></param>
        /// <param name="user_info_content"></param>
        /// <returns></returns>
        public bool SendEmail(string user_basic_email, string user_info_title, string user_info_content)
        {
            try
            {
                string txtHost = "smtp.exmail.qq.com";
                string txtFrom = "cuckoo@cuckooshop.cn";
                string txtPass = "Cuckoo88kr";
                string txtTo = user_basic_email;
                string txtSubject = user_basic_email;
                string txtBody = user_info_content;
                string result = Mail.SendMail(txtHost, txtFrom, txtPass, txtTo, txtSubject, txtBody, true, System.Net.Mail.MailPriority.High, System.Text.Encoding.UTF8);
                if (result == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 群发邮箱
        /// </summary>
        /// <param name="user_basic_email"></param>
        /// <param name="user_info_title"></param>
        /// <param name="user_info_content"></param>
        /// <returns></returns>
        public bool SendEmailQUN(string user_basic_email, string user_info_title, string user_info_content)
        {
            try
            {
                string txtHost = "smtp.exmail.qq.com";
                string txtFrom = "cuckoo@cuckooshop.cn";
                string txtPass = "Cuckoo88kr";
                string txtTo = user_basic_email;
                string txtSubject = user_basic_email;
                string txtBody = user_info_content;
                string result = Mail.MassSendMail(txtHost, txtFrom, txtPass, txtTo, txtSubject, txtBody, true, System.Net.Mail.MailPriority.High, System.Text.Encoding.UTF8);
                if (result == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
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
