using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using Webdiyer.WebControls.Mvc;
using System.IO;

namespace WebUI.Controllers
{
    [Personal]
    public class PersonalController : Controller
    {
        //
        // GET: /Personal/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {
            var query = db.user_basic;
            var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
            var upwd = HttpContext.Request.Cookies["value"].Value;
            var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
            var shop_order = db.shop_order;
            var user_collect = db.user_collect;
            ViewBag.PendingPay = shop_order.Where(s => s.shop_order_status == "未支付" & s.user_basic_id == user_basic.user_basic_id).ToList().Count;
            ViewBag.Inbound = shop_order.Where(s => s.shop_order_status == "已发货" & s.user_basic_id == user_basic.user_basic_id).ToList().Count;
            ViewBag.Collect = user_collect.Where(u => u.user_basic_id == user_basic.user_basic_id).ToList().Count;
            //我的消息
            IList<WebUI.Controllers.PersonalController.pro_coupons> pro_list = new List<WebUI.Controllers.PersonalController.pro_coupons>();
            IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
            foreach (var item in ulist)
            {
                WebUI.Controllers.PersonalController.pro_coupons pros = new WebUI.Controllers.PersonalController.pro_coupons();
                pros.pro_coupon_id = item.user_coupon_id.ToString();//用户优惠券所属ID
                pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;
                pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                pros.pro_coupon_num = "1";
                pro_list.Add(pros);
            }
            ViewBag.coupon = pro_list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠券").ToList().Count;
            ViewBag.PromoCode = pro_list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠码").ToList().Count;
            ViewBag.ms = db.user_info.Where(u => u.user_info_sta == "未读" & u.user_basic_id == user_basic.user_basic_id).Count();
            ViewBag.username = user_basic.user_basic_login;
            ViewBag.userid = user_basic.user_basic_code;
            ViewBag.user_basic_tels = user_basic.user_basic_tels;
            ViewBag.user_basic_emails = user_basic.user_basic_emails;


            #region 收藏

            var user_collects = db.user_collect.Where(c => c.user_basic_id == user_basic.user_basic_id).OrderByDescending(c => c.user_collect_adddate).ToList();
            IList<user_collects> user_collect_list = new List<user_collects>();
            foreach (var item in user_collects)
            {
                user_collects co = new user_collects();
                pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == item.pro_skuitem_id).SingleOrDefault();
                co.user_collect_id = item.user_collect_id;
                co.pro_skuitem_id = sku.pro_sku_title;
                co.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("F2");
                co.pro_sku_covimg = sku.pro_sku_covimg;
                co.pro_sku_code = sku.pro_sku_code;
                user_collect_list.Add(co);
            }
            ViewBag.user_collect_list = user_collect_list;
            #endregion


            #region 订单信息

            ViewBag.list01 = OrderListTop5("已删除");
            ViewBag.list02 = OrderListTop5("未支付");
            ViewBag.list03 = OrderListTop5("已支付");
            ViewBag.list04 = OrderListTop5("已发货");
            ViewBag.list05 = OrderListTop5("已签收");

            #endregion


            return View();
        }

        private IList<shop_order> OrderListTop5(string shop_order_status)
        {

            IList<shop_order> ord = OrderPage().OrderByDescending(l => l.shop_order_editdate).ToList();
            IList<shop_order> op;
            if (shop_order_status == "已删除")
            {
                op = ord.Where(o => o.shop_order_status != shop_order_status).ToList();
            }
            else
            {
                op = ord.Where(o => o.shop_order_status == shop_order_status).ToList();
            }
            IList<shop_order> mPage = PageCommon.PageList<shop_order>(1, 10, op.Count, op);
            return mPage;
        }

        #region 账户信息
        /// <summary>
        /// 账户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountInformation()
        {
            try
            {

                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                user_detail uds = db.user_detail.Where(ud => ud.user_basic_id == user_basic.user_basic_id).SingleOrDefault();
                ViewBag.username = uds.user_detail_firstname + uds.user_detail_lastname;
                ViewBag.user_basic_code = user_basic.user_basic_code;
                ViewBag.user_detail_gender = uds.user_detail_gender;
                ViewBag.user_detail_birthday = uds.user_detail_birthday;
                ViewBag.user_basic_email = user_basic.user_basic_email;
                ViewBag.user_basic_emails = user_basic.user_basic_emails;
                ViewBag.user_basic_tel = user_basic.user_basic_tel;
                ViewBag.user_basic_tels = user_basic.user_basic_tels;
                var province = uds.user_detail_province;
                var city = uds.user_detail_city;
                var county = uds.user_detail_county;
                if (string.IsNullOrEmpty(province) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(county))
                {
                    ViewBag.address = "";
                }
                else
                {
                    int provinces = Convert.ToInt32(province);
                    province = db.com_area.Where(c => c.com_area_id == provinces).SingleOrDefault().com_area_name;
                    int citys = Convert.ToInt32(city);
                    city = db.com_area.Where(c => c.com_area_id == citys).SingleOrDefault().com_area_name;
                    int countys = Convert.ToInt32(county);
                    county = db.com_area.Where(c => c.com_area_id == countys).SingleOrDefault().com_area_name;
                    ViewBag.address = province + city + county + uds.user_detail_address;
                }
                ViewBag.user_detail_zipcode = uds.user_detail_zipcode;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult ModifyInformation()
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
        public ActionResult EditModifyInformation()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                user_detail uds = db.user_detail.Where(ud => ud.user_basic_id == user_basic.user_basic_id).SingleOrDefault();
                uds.user_detail_province = Request.Form["user_detail_province"];
                uds.user_detail_city = Request.Form["user_detail_city"];
                uds.user_detail_county = Request.Form["user_detail_county"];
                uds.user_detail_firstname = Request.Form["user_detail_firstname"];
                uds.user_detail_lastname = Request.Form["user_detail_lastname"];
                if (!string.IsNullOrEmpty(Request.Form["user_detail_gender"]))
                {
                    uds.user_detail_gender = Request.Form["user_detail_gender"];
                }
                uds.user_detail_address = Request["user_detail_address"];
                if (!string.IsNullOrEmpty(Request.Form["user_detail_zipcode"]))
                {
                    uds.user_detail_zipcode = Request.Form["user_detail_zipcode"];
                }
                if (!string.IsNullOrEmpty(Request.Form["user_detail_birthday"]))
                {
                    uds.user_detail_birthday = Request.Form["user_detail_birthday"];
                }
                int result = db.SaveChanges();

                return RedirectToAction("AccountInformation", "Personal");
            }
            catch (Exception)
            {

                throw;
            }

        }

        public JsonResult Uploadify(HttpPostedFileBase com_img_src)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                string fileName = com_img_src.FileName;
                var guid = Guid.NewGuid().ToString("N");
                var com_img_fk = user_basic.user_basic_code.ToString();
                com_img imgs = db.com_img.Where(i => i.com_img_fk == com_img_fk).SingleOrDefault();
                if (com_img_src != null)
                {
                    if (imgs == null)
                    {
                        //转换只取得文件名 去掉路。
                        if (fileName.LastIndexOf("\\") > -1)
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                        }

                        var returnurl = Server.UrlDecode(Request.Form["returnurl"]);
                        string pathstr = "/file/img/" + com_img_fk + "/";
                        string path = Server.MapPath(pathstr);

                        string src = string.Empty;
                        if (FileHelper.CreateDir(path))
                        {
                            src = path + fileName;
                            com_img_src.SaveAs(src);  //保存到相对路径下
                        }


                        com_img img = new com_img();
                        img.com_img_id = guid;
                        img.com_img_src = pathstr + fileName;
                        img.com_img_fk = com_img_fk;
                        img.com_img_adduser = user_basic.user_basic_id;
                        img.com_img_adddate = DateTime.Now;
                        img.com_img_edituser = user_basic.user_basic_id;
                        img.com_img_editdate = DateTime.Now;
                        db.com_img.Add(img);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        int result = db.SaveChanges();
                        db.Configuration.ValidateOnSaveEnabled = true;
                        if (result > 0)
                        {
                            var json = JSONHelper.SerializeObject(img.com_img_src);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("NO", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //转换只取得文件名 去掉路。
                        if (fileName.LastIndexOf("\\") > -1)
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                        }

                        var returnurl = Server.UrlDecode(Request.Form["returnurl"]);
                        string pathstr = "/file/img/" + com_img_fk + "/";
                        string path = Server.MapPath(pathstr);
                        string delpath = Server.MapPath(imgs.com_img_src);
                        string src = string.Empty;
                        if (FileHelper.DelFile(delpath))
                        {
                            src = path + fileName;
                            com_img_src.SaveAs(src);  //保存到相对路径下
                        }


                        com_img img = db.com_img.Where(i => i.com_img_fk == com_img_fk).SingleOrDefault();
                        img.com_img_src = pathstr + fileName;
                        img.com_img_edituser = user_basic.user_basic_id;
                        img.com_img_editdate = DateTime.Now;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        int result = db.SaveChanges();
                        db.Configuration.ValidateOnSaveEnabled = true;
                        if (result > 0)
                        {
                            var json = JSONHelper.SerializeObject(img.com_img_src);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("NO", JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else
                {
                    return Json("NF", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        #region 账户安全

        public ActionResult PersonalSafe()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var user_basic_pwd = Request.Form["user_basic_pwd"];
                user_basic user = db.user_basic.Where(us => us.user_basic_id == user_basic.user_basic_id).SingleOrDefault();
                ViewBag.mailbox = user.user_basic_email;
                ViewBag.mailboxs = user.user_basic_emails;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #endregion

        #region 密码修改
        [HttpPost]
        public JsonResult OldPassword()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_basic_ypwd = JSONHelper.JsonToString(jsonstr, "user_basic_ypwd");
                var pwd = TDESHelper.EncryptString(user_basic_ypwd);
                int phone = db.user_basic.Where(u => u.user_basic_pwd == pwd).Count();
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
        public ActionResult PersonalPassword()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                ;
                var user_basic_pwd = Request.Form["user_basic_pwd"];
                user_basic user = db.user_basic.Where(us => us.user_basic_id == user_basic.user_basic_id).SingleOrDefault();
                user.user_basic_pwd = TDESHelper.EncryptString(user_basic_pwd);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    Response.Cookies["keys"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["value"].Expires = DateTime.Now.AddDays(-1);
                    return RedirectToAction("Index", "Login");

                }
                else
                {
                    return RedirectToAction("PersonalSafe", "Personal");

                }

            }
            catch (Exception)
            {

                throw;
            }
        }




        #endregion

        #region 邮箱操作

        /// <summary>
        /// 验证原邮箱
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyOldMailbox()
        {
            try
            {
                ViewBag.VerifyMailbox = "Old";
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult VerifyNewMailbox()
        {
            try
            {
                ViewBag.VerifyMailbox = "New";
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult IsOldMailbox(string phone)
        {
            try
            {
                try
                {
                    var query = db.user_basic;
                    var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                    var upwd = HttpContext.Request.Cookies["value"].Value;
                    var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                    var user_basic_email = user_basic.user_basic_email;
                    string result = string.Empty;
                    if (string.IsNullOrEmpty(user_basic_email))
                    {
                        result = "NO";
                    }
                    else if (phone == user_basic_email)
                    {
                        result = "OK";
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public ActionResult SendEmail()
        {
            try
            {
                string VerifyMailbox = Request.Form["VerifyMailbox"];
                string user_basic_email = Request.Form["user_basic_email"];
                string txtHost = "smtp.exmail.qq.com";
                string txtFrom = "cuckoo@cuckooshop.cn";
                string txtPass = "Cuckoo88kr";
                string txtTo = user_basic_email;
                string txtSubject = "邮箱验证【福库商城】";
                string url = "http://" + Request.Url.Authority + "/Personal/VerifyMail?VerifyMailbox=" + VerifyMailbox + "&mail=" + Server.UrlEncode(user_basic_email);
                string txtBody = "请点击<a href='" + url + "'>" + url + "</a>,验证邮件，请勿回复！";
                string result = Mail.SendMail(txtHost, txtFrom, txtPass, txtTo, txtSubject, txtBody, true, System.Net.Mail.MailPriority.High, System.Text.Encoding.UTF8);
                if (result == "OK")
                {
                    int j = (user_basic_email.Length - 1) - user_basic_email.LastIndexOf('@');
                    int l = user_basic_email.LastIndexOf('@') + 1;
                    string mail = user_basic_email.Substring(l, j);
                    string mailurl = "http://mail." + mail;
                    string msg = Server.UrlEncode("邮件发送成功");
                    string urlstr = Server.UrlEncode(mailurl);
                    return RedirectToAction("VerifyMailResult", "Personal", new { msg = msg, url = urlstr });
                }
                else
                {
                    string msg = Server.UrlEncode("邮件发送失败");
                    return RedirectToAction("VerifyMailResult", "Personal", new { msg = msg, url = "" });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult VerifyMailResult(string msg, string url)
        {
            try
            {
                ViewBag.msg = Server.UrlDecode(msg);
                if (string.IsNullOrEmpty(url))
                {
                    ViewBag.urlstr = Server.UrlDecode(msg);
                }
                else
                {
                    string urlsstr = Server.UrlDecode(url);
                    ViewBag.urlstr = "请登录<a href='" + urlsstr + "' target='_blank'>" + urlsstr + "</a>进行邮箱验证！";
                }

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult VerifyMail(string VerifyMailbox, string mail)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                if (VerifyMailbox == "New")
                {
                    user_basic.user_basic_email = mail;
                    user_basic.user_basic_emails = "已认证";
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        return RedirectToAction("AccountInformation", "Personal");
                    }
                }
                else
                {
                    return RedirectToAction("VerifyNewMailbox", "Personal");
                }

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 安全手机
        public ActionResult SecurityPhone()
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


        public ActionResult VerifySecurityPhone()
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


        public JsonResult RegSecurityPhone(string phone)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var phone1 = user_basic.user_basic_tel;
                string result = string.Empty;
                if (phone == phone1)
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


        public JsonResult ModifySecurityPhone(string phone)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                user_basic.user_basic_tel = phone;
                user_basic.user_basic_tels = "已认证";
                int reg = db.SaveChanges();
                string result = string.Empty;
                if (reg > 0)
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
        #endregion

        #region 注销
        public ActionResult UserLogout()
        {
            try
            {
                Response.Cookies["keys"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["value"].Expires = DateTime.Now.AddDays(-1);
                string url = Request.Url.ToString();
                return Redirect(url);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region 收货地址
        /// <summary>
        /// 我的地址
        /// </summary>
        /// <returns></returns>
        public ActionResult ShippingAddress()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                List<user_address> user_address = db.user_address.Where(u => u.user_basic_id == user_basic.user_basic_id).OrderByDescending(u => u.user_address_editdate).ToList();
                return View("ShippingAddress", user_address);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        /// <returns></returns>
        public ActionResult AddShippingAddress()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                user_address ua = new user_address();
                ua.user_address_id = Guid.NewGuid().ToString("N");
                ua.user_address_name = Request.Form["user_address_name"];
                ua.user_address_tel = Request.Form["user_address_tel"];
                ua.user_address_province = Request.Form["user_address_province"];
                ua.user_address_city = Request.Form["user_address_city"];
                ua.user_address_county = Request.Form["user_address_county"];
                ua.user_address_detail = Request.Form["user_address_detail"];
                ua.user_address_ZipCode = Request.Form["user_address_ZipCode"];
                ua.user_address_tags = Request.Form["user_address_tags"];
                ua.user_basic_id = user_basic.user_basic_id;
                ua.user_address_adddate = DateTime.Now;
                ua.user_address_editdate = DateTime.Now;
                db.user_address.Add(ua);
                int result = db.SaveChanges();
                return RedirectToAction("ShippingAddress", "Personal");
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult ShippingAddressEdit(string user_address_id)
        {
            try
            {
                user_address ua = db.user_address.Where(u => u.user_address_id == user_address_id).SingleOrDefault();
                var Provinces = db.com_area.Where(c => c.com_area_lev == "1").ToList();
                var Citys = db.com_area.Where(c => c.com_area_parentid == ua.user_address_province).ToList();
                var Countys = db.com_area.Where(c => c.com_area_parentid == ua.user_address_city).ToList();
                SelectList provinces = new SelectList(Provinces, "com_area_id", "com_area_name", ua.user_address_province);
                SelectList citys = new SelectList(Citys, "com_area_id", "com_area_name", ua.user_address_city);
                SelectList countys = new SelectList(Countys, "com_area_id", "com_area_name", ua.user_address_county);
                ViewData["Province"] = provinces;
                ViewData["City"] = citys;
                ViewData["County"] = countys;
                ViewBag.user_address_name = ua.user_address_name;
                ViewBag.user_address_tel = ua.user_address_tel;
                ViewBag.user_address_detail = ua.user_address_detail;
                ViewBag.user_address_id = user_address_id;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult EditShippingAddress()
        {
            try
            {
                var query = db.user_basic;
                var user_address_id = Request.Form["user_address_id"];
                user_address ua = db.user_address.Where(u => u.user_address_id == user_address_id).SingleOrDefault();
                ua.user_address_name = Request.Form["user_address_name"];
                ua.user_address_tel = Request.Form["user_address_tel"];
                if (!string.IsNullOrEmpty(Request.Form["user_address_province"]))
                {
                    ua.user_address_province = Request.Form["user_address_province"];
                }
                if (!string.IsNullOrEmpty(Request.Form["user_address_city"]))
                {
                    ua.user_address_city = Request.Form["user_address_city"];
                }
                if (!string.IsNullOrEmpty(Request.Form["user_address_county"]))
                {
                    ua.user_address_county = Request.Form["user_address_county"];
                }
                ua.user_address_detail = Request.Form["user_address_detail"];
                ua.user_address_ZipCode = Request.Form["user_address_ZipCode"];
                ua.user_address_tags = Request.Form["user_address_tags"];
                ua.user_address_editdate = DateTime.Now;
                int result = db.SaveChanges();
                return RedirectToAction("ShippingAddress", "Personal");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ShippingAddressDel(string user_address_id)
        {
            try
            {
                user_address ua = db.user_address.Where(u => u.user_address_id == user_address_id).SingleOrDefault();
                db.user_address.Remove(ua);
                db.SaveChanges();
                return RedirectToAction("ShippingAddress", "Personal");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 订单管理
        /// <summary>
        /// 我的订单
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalOrder(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status != "已删除").OrderByDescending(o => o.shop_order_buydate).ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, op.Count, op);
                return View("PersonalOrder", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 已支付
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult OrderPaid(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已支付").OrderByDescending(o => o.shop_order_buydate).ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, op.Count, op);
                return View("OrderPaid", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 未支付
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult Unpaid(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "未支付").OrderByDescending(o => o.shop_order_buydate).ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, op.Count, op);
                return View("Unpaid", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 删除订单，更改状态为"已删除"
        /// </summary>
        /// <param name="shop_order_id"></param>
        /// <returns></returns>
        public ActionResult DeleteOrders(string shop_order_id)
        {

            try
            {
                shop_order or = db.shop_order.Where(o => o.shop_order_id == shop_order_id).SingleOrDefault();
                or.shop_order_status = "已删除";
                if (!string.IsNullOrEmpty(or.pro_coupon_id))
                {
                    int pro_coupon_id = Convert.ToInt32(or.pro_coupon_id.ToString());
                    user_coupon pc = db.user_coupon.Where(p => p.user_coupon_id == pro_coupon_id).SingleOrDefault();
                    if (pc != null)
                    {
                        pc.user_coupon_stastus = "0";
                    }
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }
                int result = db.SaveChanges();
                return RedirectToAction("Unpaid");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult Shipped(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已发货").ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, op.Count, op);
                return View("Shipped", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 已关闭
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult ClosedOrders(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已签收").ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, op.Count, op);
                return View("ClosedOrders", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmationReceipt(string shop_order_id)
        {
            try
            {
                shop_order or = db.shop_order.Where(o => o.shop_order_id == shop_order_id).SingleOrDefault();
                or.shop_order_status = "已签收";
                int result = db.SaveChanges();

                return RedirectToAction("ClosedOrders");


            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 订单分页
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        private IList<shop_order> OrderPage()
        {
            var query = db.user_basic;
            var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
            var upwd = HttpContext.Request.Cookies["value"].Value;
            var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
            IList<shop_order> orderlist = db.shop_order.OrderBy(o => o.shop_order_editdate).Where(o => o.user_basic_id == user_basic.user_basic_id).ToList();
            IList<shop_order> sor = new List<shop_order>();
            foreach (var item in orderlist)
            {
                shop_order so = new shop_order();
                so.shop_order_id = item.shop_order_id;
                so.shop_order_code = item.shop_order_code;
                so.shop_order_status = item.shop_order_status;
                so.shop_order_buydate = item.shop_order_buydate;
                so.shop_shipping_id = item.shop_shipping_id;
                so.shop_order_shipmentnumber = item.shop_order_shipmentnumber;
                so.shop_order_waynum = item.shop_order_waynum;
                so.shop_order_totalmoeny = item.shop_order_totalmoeny;
                var addre = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault();
                var address = string.Empty;
                var County = string.Empty;
                if (addre == null)
                {
                    address = "";
                    County = "";
                    so.shop_order_remark = "0";
                }
                else
                {
                    //address = "收货人：" + addre.user_address_name + "，收货地址：" + addre.user_address_province + addre.user_address_city + addre.user_address_county + addre.user_address_detail + "，收货人手机：" + addre.user_address_tel + "，邮政编码：" + addre.user_address_ZipCode;
                    //County = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_county;
                    int user_address_province = Convert.ToInt32(addre.user_address_province);
                    int user_address_city = Convert.ToInt32(addre.user_address_city);
                    int user_address_county = Convert.ToInt32(addre.user_address_county);
                    var province = db.com_area.Where(a => a.com_area_id == user_address_province).SingleOrDefault().com_area_name;
                    var city = db.com_area.Where(a => a.com_area_id == user_address_city).SingleOrDefault().com_area_name;
                    var county = db.com_area.Where(a => a.com_area_id == user_address_county).SingleOrDefault().com_area_name;
                    address = "收货人：" + addre.user_address_name + "，收货地址：" + province + city + county + addre.user_address_detail + "，收货人手机：" + addre.user_address_tel + "，邮政编码：" + addre.user_address_ZipCode;
                    County = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_county;
                    so.shop_order_remark = db.pro_shipment.Where(sh => sh.pro_shipment_county == County).SingleOrDefault().pro_shipment_price;
                }
                so.user_address_id = address;


                sor.Add(so);
            }
            IList<shop_order> mPage = sor;
            return mPage;
        }



        /// <summary>
        /// 订单详细
        /// </summary>
        /// <param name="shop_order_id"></param>
        /// <returns></returns>
        public ActionResult PersonalOrderDetails(string shop_order_id)
        {
            try
            {
                shop_order ord = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                var useraddress = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault();

                if (useraddress != null)
                {

                    ViewBag.user_address_name = useraddress.user_address_name;
                    ViewBag.user_address_tel = useraddress.user_address_tel;
                    int user_address_province = Convert.ToInt32(useraddress.user_address_province);
                    int user_address_city = Convert.ToInt32(useraddress.user_address_city);
                    int user_address_county = Convert.ToInt32(useraddress.user_address_county);
                    ViewBag.address = db.com_area.Where(c => c.com_area_id == user_address_province).SingleOrDefault().com_area_name + db.com_area.Where(c => c.com_area_id == user_address_city).SingleOrDefault().com_area_name + db.com_area.Where(c => c.com_area_id == user_address_county).SingleOrDefault().com_area_name + useraddress.user_address_detail;
                    var yunfeiid = useraddress.user_address_county;
                    ViewBag.shop_order_code = ord.shop_order_code;
                    ViewBag.shop_order_status = ord.shop_order_status;
                    ViewBag.shop_order_waynum = ord.shop_order_waynum;
                    ViewBag.shop_order_totalmoeny = ord.shop_order_totalmoeny;
                    ViewBag.yunfei = db.pro_shipment.Where(c => c.pro_shipment_county == yunfeiid).SingleOrDefault().pro_shipment_price;
                    ViewBag.shop_order_buydate = ord.shop_order_buydate;
                    ViewBag.shop_order_shipmentnumber = ord.shop_order_shipmentnumber;
                    ViewBag.shop_pay_id = ord.shop_pay_id;
                    ViewBag.user_address_name = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault().user_address_name;
                    var list = db.shop_orderdetail.Where(or => or.shop_order_id == shop_order_id).ToList();
                    decimal? tprice = 0;
                    IList<shop_orderdetail> olist = new List<shop_orderdetail>();
                    foreach (var item in list)
                    {
                        shop_orderdetail so = new shop_orderdetail();
                        so.pro_skuitem_id = item.pro_skuitem_id;
                        so.shop_order_id = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                        so.shop_orderdetail_id = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault().pro_sku_title;
                        so.shop_orderdetail_num = item.shop_orderdetail_num;
                        so.shop_orderdetail_total = Convert.ToDecimal(Convert.ToDecimal(item.shop_orderdetail_total.ToString()).ToString("F2"));
                        tprice += item.shop_orderdetail_total;

                        olist.Add(so);
                    }
                    shop_invoice inv = db.shop_invoice.Where(i => i.shop_order_id == ord.shop_order_id).SingleOrDefault();
                    if (inv == null)
                    {

                        ViewBag.fptt = "无";

                    }
                    else
                    {
                        if (inv.shop_invoice_class == "gr")
                        {
                            ViewBag.fptt = "个人";
                        }
                        else
                        {
                            ViewBag.fptt = "企业";
                        }

                    }
                    ViewBag.shop_order_id = ord.shop_order_id;
                    ViewBag.tprice = tprice;
                    ViewBag.olist = olist;
                    return View();
                }
                else
                {
                    return Content("<script>alert('订单无效！');window.history.go(-1);</script>");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }





        #endregion


        #region 退换货
        /// <summary>
        /// 选择订单
        /// </summary>
        /// <returns></returns>
        public ActionResult ChooseOrder(int pageIndx = 1)
        {
            try
            {
                try
                {
                    var query = db.user_basic;
                    var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                    var upwd = HttpContext.Request.Cookies["value"].Value;
                    var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                    IList<shop_order> orderlist = db.shop_order.OrderBy(o => o.shop_order_editdate).Where(o => o.user_basic_id == user_basic.user_basic_id & o.shop_order_status == "已签收").ToList();
                    IList<shop_order> sor = new List<shop_order>();
                    foreach (var item in orderlist)
                    {
                        shop_order so = new shop_order();
                        so.shop_order_id = item.shop_order_id;
                        so.shop_order_code = item.shop_order_code;
                        so.shop_order_status = item.shop_order_status;
                        so.shop_order_buydate = item.shop_order_buydate;
                        so.shop_order_shipmentnumber = item.shop_order_shipmentnumber;
                        so.shop_order_waynum = item.shop_order_waynum;
                        so.shop_order_totalmoeny = item.shop_order_totalmoeny;

                        so.user_address_id = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_name;
                        var County = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_county;
                        so.shop_order_remark = db.pro_shipment.Where(sh => sh.pro_shipment_county == County).SingleOrDefault().pro_shipment_price;
                        sor.Add(so);
                    }
                    IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, sor.Count, sor);
                    return View("ChooseOrder", mPage);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult ChooseOrderDetails(string shop_order_id)
        {
            try
            {
                shop_order ord = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                var useraddress = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault();
                ViewBag.user_address_name = useraddress.user_address_name;
                ViewBag.user_address_tel = useraddress.user_address_tel;
                int user_address_province = Convert.ToInt32(useraddress.user_address_province);
                int user_address_city = Convert.ToInt32(useraddress.user_address_city);
                int user_address_county = Convert.ToInt32(useraddress.user_address_county);
                ViewBag.address = db.com_area.Where(c => c.com_area_id == user_address_province).SingleOrDefault().com_area_name + db.com_area.Where(c => c.com_area_id == user_address_city).SingleOrDefault().com_area_name + db.com_area.Where(c => c.com_area_id == user_address_county).SingleOrDefault().com_area_name + useraddress.user_address_detail;


                var yunfeiid = useraddress.user_address_county;
                ViewBag.shop_order_code = ord.shop_order_code;
                ViewBag.shop_order_status = ord.shop_order_status;
                ViewBag.shop_order_waynum = ord.shop_order_waynum;
                ViewBag.shop_order_totalmoeny = ord.shop_order_totalmoeny;
                ViewBag.yunfei = db.pro_shipment.Where(c => c.pro_shipment_county == yunfeiid).SingleOrDefault().pro_shipment_price;
                ViewBag.shop_order_buydate = ord.shop_order_buydate;
                ViewBag.shop_order_shipmentnumber = ord.shop_order_shipmentnumber;
                ViewBag.user_address_name = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault().user_address_name;
                var list = db.shop_orderdetail.Where(or => or.shop_order_id == shop_order_id).ToList();
                decimal? tprice = 0;
                IList<shop_orderdetail> olist = new List<shop_orderdetail>();
                foreach (var item in list)
                {
                    shop_orderdetail so = new shop_orderdetail();
                    so.pro_skuitem_id = item.shop_orderdetail_id;//订单详细ID
                    so.shop_order_id = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                    so.shop_orderdetail_id = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault().pro_sku_title;
                    so.shop_orderdetail_num = item.shop_orderdetail_num;
                    so.shop_orderdetail_total = Convert.ToDecimal(Convert.ToDecimal(item.shop_orderdetail_total.ToString()).ToString("F2"));
                    tprice += item.shop_orderdetail_total;
                    olist.Add(so);
                }
                ViewBag.tprice = tprice;
                ViewBag.olist = olist;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// 选择服务
        /// </summary>
        /// <returns></returns>
        public ActionResult ChooseSelectServices(string pro_skuitem_id, string shop_order_code)
        {
            try
            {
                shop_order ord = db.shop_order.Where(ords => ords.shop_order_code == shop_order_code).SingleOrDefault();
                var useraddress = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault();
                ViewBag.shop_order_code = shop_order_code;
                ViewBag.user_address_name = useraddress.user_address_name;
                ViewBag.user_address_tel = useraddress.user_address_tel;
                ViewBag.shop_order_buydate = ord.shop_order_buydate;
                ViewBag.user_address_id = ord.user_address_id;
                shop_orderdetail so = db.shop_orderdetail.Where(sp => sp.shop_orderdetail_id == pro_skuitem_id).SingleOrDefault();
                ViewBag.pro_skuitem_id = so.shop_orderdetail_id;//订单详细ID
                ViewBag.shop_order_id = db.pro_sku.Where(s => s.pro_sku_code == so.pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                ViewBag.shop_orderdetail_id = db.pro_sku.Where(s => s.pro_sku_code == so.pro_skuitem_id).SingleOrDefault().pro_sku_title;
                ViewBag.shop_orderdetail_num = so.shop_orderdetail_num;
                ViewBag.shop_orderdetail_total = Convert.ToDecimal(Convert.ToDecimal(so.shop_orderdetail_total.ToString()).ToString("F2"));
                ViewBag.Id = so.pro_skuitem_id;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public JsonResult AddChooseSelectServices()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var shop_order_code = JSONHelper.JsonToString(jsonstr, "shop_order_code");
                var user_address_id = JSONHelper.JsonToString(jsonstr, "user_address_id");
                var pro_skuitem_id = JSONHelper.JsonToString(jsonstr, "pro_skuitem_id");
                var thh = JSONHelper.JsonToString(jsonstr, "thh");
                var sh = JSONHelper.JsonToString(jsonstr, "sh");
                var des = JSONHelper.JsonToString(jsonstr, "des");
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var shop_choose = db.shop_choose.Where(s => s.shop_orderdetail_id == pro_skuitem_id).SingleOrDefault();
                if (shop_choose == null)
                {
                    shop_choose sc = new shop_choose();
                    sc.shop_choose_id = Guid.NewGuid().ToString("N");
                    sc.shop_orderdetail_id = pro_skuitem_id;
                    sc.shop_order_id = shop_order_code;
                    sc.shop_choose_service = thh;
                    sc.shop_choose_isget = sh;
                    sc.shop_choose_status = "申请中";
                    sc.shop_choose_des = des;
                    sc.user_address_id = user_address_id;
                    sc.user_basic_id = user_basic.user_basic_id;
                    sc.shop_choose_date = DateTime.Now;
                    db.shop_choose.Add(sc);
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        shop_choose c = db.shop_choose.Where(ss => ss.shop_choose_id == sc.shop_choose_id).SingleOrDefault();
                        return Json(c.shop_choose_code, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json("NO", JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json("Y", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult ChooseRevocation(string shop_choose_id)
        {
            try
            {
                ViewBag.shop_choose_code = shop_choose_id;
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult DelChooseRevocation(int shop_choose_code)
        {
            try
            {
                shop_choose s = db.shop_choose.Where(ss => ss.shop_choose_code == shop_choose_code).SingleOrDefault();
                db.shop_choose.Remove(s);
                int result = db.SaveChanges();
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

        public ActionResult ChooseList(int pageIndx = 1)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var list = db.shop_choose.Where(c => c.user_basic_id == user_basic.user_basic_id).OrderByDescending(s => s.shop_choose_date).ToList();
                IList<shop_choose> mPage = PageCommon.PageList<shop_choose>(pageIndx, 20, list.Count, list);
                return View("ChooseList", mPage);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ChooseDetails(int shop_choose_code)
        {
            try
            {
                shop_choose s = db.shop_choose.Where(ss => ss.shop_choose_code == shop_choose_code).SingleOrDefault();
                ViewBag.shop_choos_service = s.shop_choose_service;
                ViewBag.shop_choose_code = s.shop_choose_code;
                ViewBag.shop_choose_status = s.shop_choose_status;
                ViewBag.shop_choose_isget = s.shop_choose_isget;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public JsonResult ChooseExpress()
        {
            try
            {
                //<p>开户姓名：</p><input id="Text3"
                //<p>开户帐号：</p><input id="Text4"
                //<p>开户银行：</p><input id="Text5"
                //<p>银行地址：</p><input id="Text6"
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                int shop_choose_code = Convert.ToInt32(JSONHelper.JsonToString(jsonstr, "shop_choose_code"));
                var type = JSONHelper.JsonToString(jsonstr, "type");
                shop_choose s = db.shop_choose.Where(ss => ss.shop_choose_code == shop_choose_code).SingleOrDefault();
                if (type == "001")
                {
                    s.shop_express_name = JSONHelper.JsonToString(jsonstr, "Text1");
                    s.shop_express_code = JSONHelper.JsonToString(jsonstr, "Text2");
                    s.user_bank_username = JSONHelper.JsonToString(jsonstr, "Text3");
                    s.user_bank_name = JSONHelper.JsonToString(jsonstr, "Text4");
                    s.user_bank_code = JSONHelper.JsonToString(jsonstr, "Text5");
                    s.user_bank_address = JSONHelper.JsonToString(jsonstr, "Text6");
                    s.shop_choose_status = "未收货退款申请";
                }

                if (type == "002")
                {
                    s.user_bank_username = JSONHelper.JsonToString(jsonstr, "Text3");
                    s.user_bank_name = JSONHelper.JsonToString(jsonstr, "Text4");
                    s.user_bank_code = JSONHelper.JsonToString(jsonstr, "Text5");
                    s.user_bank_address = JSONHelper.JsonToString(jsonstr, "Text6");
                    s.shop_choose_status = "已收货退款申请";
                }
                if (type == "003")
                {
                    s.shop_express_name = JSONHelper.JsonToString(jsonstr, "Text1");
                    s.shop_express_code = JSONHelper.JsonToString(jsonstr, "Text2");
                    s.shop_choose_status = "换货已申请";
                }
                int result = db.SaveChanges();
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


        public ActionResult AfterSales()
        {
            try
            {

                com_article arts = db.com_article.Where(a => a.com_article_class == "THHZC").SingleOrDefault();
                if (arts != null)
                {
                    ViewBag.com_article_title = arts.com_article_title;
                    ViewBag.com_article_content = Server.HtmlDecode(arts.com_article_content);
                }

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 商品评价
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductReview(int pageIndx = 1)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<shop_order> orderlist = db.shop_order.OrderBy(o => o.shop_order_editdate).Where(o => o.user_basic_id == user_basic.user_basic_id & o.shop_order_status == "已签收").ToList();
                IList<shop_order> sor = new List<shop_order>();
                foreach (var item in orderlist)
                {
                    shop_order so = new shop_order();
                    so.shop_order_id = item.shop_order_id;
                    so.shop_order_code = item.shop_order_code;
                    so.shop_order_status = item.shop_order_status;
                    so.shop_order_buydate = item.shop_order_buydate;
                    so.shop_order_shipmentnumber = item.shop_order_shipmentnumber;
                    so.shop_order_waynum = item.shop_order_waynum;
                    so.shop_order_totalmoeny = item.shop_order_totalmoeny;

                    so.user_address_id = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_name;
                    var County = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_county;
                    so.shop_order_remark = db.pro_shipment.Where(sh => sh.pro_shipment_county == County).SingleOrDefault().pro_shipment_price;
                    sor.Add(so);
                }
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 5, sor.Count, sor);
                return View("ProductReview", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult Reviews(string shop_order_id)
        {
            try
            {
                shop_order ord = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                var useraddress = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault();
                ViewBag.user_address_name = useraddress.user_address_name;
                ViewBag.user_address_tel = useraddress.user_address_tel;
                int user_address_province = Convert.ToInt32(useraddress.user_address_province);
                int user_address_city = Convert.ToInt32(useraddress.user_address_city);
                int user_address_county = Convert.ToInt32(useraddress.user_address_county);
                ViewBag.address = db.com_area.Where(c => c.com_area_id == user_address_province).SingleOrDefault().com_area_name + db.com_area.Where(c => c.com_area_id == user_address_city).SingleOrDefault().com_area_name + db.com_area.Where(c => c.com_area_id == user_address_county).SingleOrDefault().com_area_name + useraddress.user_address_detail;


                var yunfeiid = useraddress.user_address_county;
                ViewBag.shop_order_code = ord.shop_order_code;
                ViewBag.shop_order_status = ord.shop_order_status;
                ViewBag.shop_order_waynum = ord.shop_order_waynum;
                ViewBag.shop_order_totalmoeny = ord.shop_order_totalmoeny;
                ViewBag.yunfei = db.pro_shipment.Where(c => c.pro_shipment_county == yunfeiid).SingleOrDefault().pro_shipment_price;
                ViewBag.shop_order_buydate = ord.shop_order_buydate;
                ViewBag.shop_order_shipmentnumber = ord.shop_order_shipmentnumber;
                ViewBag.user_address_name = db.user_address.Where(ua => ua.user_address_id == ord.user_address_id).SingleOrDefault().user_address_name;
                var list = db.shop_orderdetail.Where(or => or.shop_order_id == shop_order_id).ToList();
                decimal? tprice = 0;
                IList<shop_orderdetail> olist = new List<shop_orderdetail>();
                foreach (var item in list)
                {
                    shop_orderdetail so = new shop_orderdetail();

                    so.pro_skuitem_id = item.pro_skuitem_id;
                    so.shop_order_id = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                    so.shop_orderdetail_id = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault().pro_sku_title;
                    so.shop_orderdetail_num = item.shop_orderdetail_num;
                    so.shop_orderdetail_total = Convert.ToDecimal(Convert.ToDecimal(item.shop_orderdetail_total.ToString()).ToString("F2"));
                    tprice += item.shop_orderdetail_total;
                    olist.Add(so);
                }
                ViewBag.tprice = tprice;
                ViewBag.olist = olist;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult ReviewsDetails(string shop_orderdetail_id, string shop_order_code)
        {
            try
            {
                shop_order ord = db.shop_order.Where(s => s.shop_order_code == shop_order_code).SingleOrDefault();
                ViewBag.shop_order_code = shop_order_code;
                pro_sku pk = db.pro_sku.Where(p => p.pro_sku_code == shop_orderdetail_id).SingleOrDefault();
                ViewBag.pro_sku_covimg = pk.pro_sku_covimg;
                ViewBag.pro_sku_title = pk.pro_sku_title;
                ViewBag.pro_sku_code = pk.pro_sku_code;
                ViewBag.reviewscount = db.pro_comment.Where(c => c.pro_sku_code == shop_orderdetail_id).Count();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 异步提交评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Assess()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var pro_comment_star = JSONHelper.JsonToString(jsonstr, "pro_comment_star");
                var pro_comment_content = JSONHelper.JsonToString(jsonstr, "pro_comment_content");
                var pro_sku_code = JSONHelper.JsonToString(jsonstr, "pro_sku_code");
                var guid = Guid.NewGuid().ToString("N");
                pro_comment com = new pro_comment();
                com.pro_comment_id = guid;
                com.pro_comment_star = pro_comment_star;
                com.pro_comment_content = pro_comment_content;
                com.pro_sku_code = pro_sku_code;
                com.user_basic_id = user_basic.user_basic_id;
                com.pro_comment_date = DateTime.Now;
                db.pro_comment.Add(com);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return Json(guid, JsonRequestBehavior.AllowGet);
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
        /// 异步提交晒图图片
        /// </summary>
        /// <param name="com_img_src"></param>
        /// <returns></returns>
        public JsonResult Blueprint(HttpPostedFileBase com_img_src, string com_img_fk)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                string fileName = com_img_src.FileName;
                var guid = Guid.NewGuid().ToString("N");

                com_img imgs = db.com_img.Where(i => i.com_img_fk == com_img_fk).SingleOrDefault();
                if (com_img_src != null)
                {
                    if (imgs == null)
                    {
                        //转换只取得文件名 去掉路。
                        if (fileName.LastIndexOf("\\") > -1)
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                        }

                        var returnurl = Server.UrlDecode(Request.Form["returnurl"]);
                        string pathstr = "/file/img/" + com_img_fk + "/";
                        string path = Server.MapPath(pathstr);

                        string src = string.Empty;
                        if (FileHelper.CreateDir(path))
                        {
                            src = path + fileName;
                            com_img_src.SaveAs(src);  //保存到相对路径下
                        }


                        com_img img = new com_img();
                        img.com_img_id = guid;
                        img.com_img_src = pathstr + fileName;
                        img.com_img_fk = com_img_fk;
                        img.com_img_adduser = user_basic.user_basic_id;
                        img.com_img_adddate = DateTime.Now;
                        img.com_img_edituser = user_basic.user_basic_id;
                        img.com_img_editdate = DateTime.Now;
                        db.com_img.Add(img);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        int result = db.SaveChanges();
                        db.Configuration.ValidateOnSaveEnabled = true;
                        if (result > 0)
                        {
                            var json = JSONHelper.SerializeObject(img.com_img_src);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("NO", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //转换只取得文件名 去掉路。
                        if (fileName.LastIndexOf("\\") > -1)
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                        }

                        var returnurl = Server.UrlDecode(Request.Form["returnurl"]);
                        string pathstr = "/file/img/" + com_img_fk + "/";
                        string path = Server.MapPath(pathstr);
                        string delpath = Server.MapPath(imgs.com_img_src);
                        string src = string.Empty;
                        if (FileHelper.DelFile(delpath))
                        {
                            src = path + fileName;
                            com_img_src.SaveAs(src);  //保存到相对路径下
                        }


                        com_img img = db.com_img.Where(i => i.com_img_fk == com_img_fk).SingleOrDefault();
                        img.com_img_src = pathstr + fileName;
                        img.com_img_edituser = user_basic.user_basic_id;
                        img.com_img_editdate = DateTime.Now;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        int result = db.SaveChanges();
                        db.Configuration.ValidateOnSaveEnabled = true;
                        if (result > 0)
                        {
                            var json = JSONHelper.SerializeObject(img.com_img_src);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("NO", JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else
                {
                    return Json("NF", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public class pro_comments : pro_comment
        {
            public string com_img_src { get; set; }
            public pro_sku pro_sku { get; set; }
        }
        /// <summary>
        /// 已评价商品
        /// </summary>
        /// <returns></returns>
        public ActionResult BeenEvaluated(int pageIndex = 1)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var cm = db.pro_comment.Where(c => c.user_basic_id == user_basic.user_basic_id);
                var list = cm.ToList().OrderByDescending(c => c.pro_comment_date);
                IList<pro_comments> newlist = new List<pro_comments>();
                foreach (var item in list)
                {
                    pro_comments cms = new pro_comments();
                    cms.pro_comment_id = item.pro_comment_id;
                    cms.pro_comment_star = item.pro_comment_star;
                    cms.pro_comment_content = item.pro_comment_content;
                    cms.pro_sku = db.pro_sku.Where(p => p.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                    var com_img = db.com_img.Where(c => c.com_img_fk == item.pro_comment_id).SingleOrDefault();
                    if (com_img == null)
                    {
                        cms.com_img_src = "/file/img/default.jpg";
                    }
                    else
                    {
                        cms.com_img_src = com_img.com_img_src;
                    }

                    cms.pro_comment_date = item.pro_comment_date;
                    newlist.Add(cms);
                }
                IList<pro_comments> mPage = PageCommon.PageList<pro_comments>(pageIndex, 5, newlist.Count, newlist);
                return View("BeenEvaluated", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 浏览记录
        /// <summary>
        /// 浏览记录
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult PersonalScan(int pageIndx = 1)
        {
            try
            {
                IList<pro_sku> list = new List<pro_sku>();
                if (Request.Cookies["GoodsScan"] != null)
                {
                    var scan = TDESHelper.DecryptString(Request.Cookies["GoodsScan"].Value);
                    string[] str = scan.Split(',');

                    foreach (var item in str)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            pro_sku sku = new pro_sku();
                            sku.pro_sku_id = db.pro_sku.Where(s => s.pro_sku_code == item).SingleOrDefault().pro_sku_id;
                            var pro_item_id = db.pro_sku.Where(s => s.pro_sku_code == item).SingleOrDefault().pro_item_id;
                            sku.pro_sku_code = item;
                            sku.pro_sku_covimg = db.pro_sku.Where(s => s.pro_sku_code == item).SingleOrDefault().pro_sku_covimg;
                            sku.pro_item_id = db.pro_item.Where(i => i.pro_item_id == pro_item_id).SingleOrDefault().pro_brand_id;
                            sku.pro_sku_arrspe = Convert.ToDecimal(db.pro_skuitem.Where(sk => sk.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString()).ToString("F2");
                            list.Add(sku);
                        }
                    }

                }
                IList<pro_sku> mPage = PageCommon.PageList<pro_sku>(pageIndx, 12, list.Count, list);
                return View("PersonalScan", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 我的收藏
        public class user_collects : user_collect
        {
            public string pro_skuitem_price { get; set; }
            public string pro_sku_covimg { get; set; }

            public string pro_sku_code { get; set; }
        }
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult PersonalCollect(int pageIndx = 1)
        {
            var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
            var upwd = HttpContext.Request.Cookies["value"].Value;
            var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
            var query = db.user_collect.Where(c => c.user_basic_id == user_basic.user_basic_id).OrderByDescending(c => c.user_collect_adddate).ToList();
            IList<user_collects> list = new List<user_collects>();
            foreach (var item in query)
            {
                user_collects co = new user_collects();
                pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == item.pro_skuitem_id).SingleOrDefault();
                co.user_collect_id = item.user_collect_id;
                co.pro_skuitem_id = sku.pro_sku_title;
                co.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("F2");
                co.pro_sku_covimg = sku.pro_sku_covimg;
                co.pro_sku_code = sku.pro_sku_code;
                list.Add(co);
            }
            IList<user_collects> mPage = PageCommon.PageList<user_collects>(pageIndx, 16, query.Count, list);

            return View("PersonalCollect", mPage);
        }
        public ActionResult PersonalCollectDel(string user_collect_id)
        {
            try
            {
                user_collect uc = db.user_collect.Where(u => u.user_collect_id == user_collect_id).SingleOrDefault();
                db.user_collect.Remove(uc);
                int result = db.SaveChanges();
                return RedirectToAction("PersonalCollect", "Personal");
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 购物车收藏
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ActionResult PersonalCollectCreate(string item)
        {
            var query = db.user_basic;
            var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
            var upwd = HttpContext.Request.Cookies["value"].Value;
            var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
            string shos = item;
            string jsonstrs = TDESHelper.DecryptString(shos);
            string[] arrpro = jsonstrs.Split(',');
            foreach (var items in arrpro)
            {
                if (items != "")
                {
                    string pro_skuitem_id = items.Substring(0, items.IndexOf(':'));
                    user_collect co = new user_collect();
                    co.user_collect_id = Guid.NewGuid().ToString("N");
                    co.pro_skuitem_id = pro_skuitem_id;
                    co.user_basic_id = user_basic.user_basic_id;
                    co.user_collect_adddate = DateTime.Now;
                    co.user_collect_editdate = DateTime.Now;
                    db.user_collect.Add(co);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("PersonalCollect", "Personal");

        }
        /// <summary>
        /// 详细页面收藏功能
        /// </summary>
        /// <param name="pro_skuitem_id"></param>
        /// <returns></returns>
        public ActionResult PersonalCollectCreates(string pro_skuitem_id)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();

                user_collect co = new user_collect();
                co.user_collect_id = Guid.NewGuid().ToString("N");
                co.pro_skuitem_id = pro_skuitem_id;
                co.user_basic_id = user_basic.user_basic_id;
                co.user_collect_adddate = DateTime.Now;
                co.user_collect_editdate = DateTime.Now;
                db.user_collect.Add(co);
                db.SaveChanges();


                return RedirectToAction("PersonalCollect", "Personal");
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region 优惠券

        public class pro_coupons : pro_coupon
        {
            public string user_coupon_id { get; set; }

            public string user_basic_id { get; set; }
            public string user_coupon_stastus { get; set; }

        }
        /// <summary>
        /// 可使用
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalCoupon(int pageIndx = 1)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<pro_coupons> list = new List<pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
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
                var list_01 = list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠券").ToList();
                IList<pro_coupons> mPage = PageCommon.PageList<pro_coupons>(pageIndx, 5, list_01.Count, list_01);
                return View("PersonalCoupon", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 已过期优惠券
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult PersonalCouponExpired(int pageIndx = 1)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<pro_coupons> list = new List<pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id).ToList();
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

                var list_01 = list.Where(l => l.pro_coupon_enddate < DateTime.Now && l.pro_coupon_class == "优惠券").ToList();
                IList<pro_coupons> mPage = PageCommon.PageList<pro_coupons>(pageIndx, 5, list_01.Count, list_01);
                return View("PersonalCoupon", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 已使用优惠券
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult PersonalCouponUsed(int pageIndx = 1)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<pro_coupons> list = new List<pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "1").ToList();
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
                var list_01 = list.Where(l => l.pro_coupon_class == "优惠券").ToList();
                IList<pro_coupons> mPage = PageCommon.PageList<pro_coupons>(pageIndx, 5, list_01.Count, list_01);
                return View("PersonalCoupon", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region 优惠码



        /// <summary>
        /// 可使用优惠码
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalPromoCode(int pageIndx = 1)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<pro_coupons> list = new List<pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
                foreach (var item in ulist)
                {
                    pro_coupons pros = new pro_coupons();
                    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;

                    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    pros.pro_coupon_num = "1";
                    pros.pro_coupon_code = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_code;
                    list.Add(pros);
                }
                var list_01 = list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠码").ToList();
                IList<pro_coupons> mPage = PageCommon.PageList<pro_coupons>(pageIndx, 5, list_01.Count, list_01);
                return View("PersonalPromoCode", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 已过期优惠码
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult PersonalPromoCodeExpired(int pageIndx = 1)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<pro_coupons> list = new List<pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id).ToList();
                foreach (var item in ulist)
                {
                    pro_coupons pros = new pro_coupons();
                    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;

                    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    pros.pro_coupon_num = "1";
                    pros.pro_coupon_code = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_code;

                    list.Add(pros);
                }

                var list_01 = list.Where(l => l.pro_coupon_enddate < DateTime.Now && l.pro_coupon_class == "优惠码").ToList();
                IList<pro_coupons> mPage = PageCommon.PageList<pro_coupons>(pageIndx, 5, list_01.Count, list_01);
                return View("PersonalPromoCode", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 已使用优惠码
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult PersonalPromoCodeUsed(int pageIndx = 1)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<pro_coupons> list = new List<pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "1").ToList();
                foreach (var item in ulist)
                {
                    pro_coupons pros = new pro_coupons();
                    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;

                    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    pros.pro_coupon_num = "1";
                    pros.pro_coupon_code = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_code;

                    list.Add(pros);
                }
                var list_01 = list.Where(l => l.pro_coupon_class == "优惠码").ToList();
                IList<pro_coupons> mPage = PageCommon.PageList<pro_coupons>(pageIndx, 5, list_01.Count, list_01);
                return View("PersonalPromoCode", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 消息中心
        public ActionResult MessageCenter(int pageIndx = 1)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<user_info> list = db.user_info.Where(i => i.user_basic_id == user_basic.user_basic_id).OrderByDescending(i => i.user_info_date).ToList();
                IList<user_info> mPage = PageCommon.PageList<user_info>(pageIndx, 20, list.Count, list);
                return View("MessageCenter", mPage);

            }
            catch (Exception)
            {

                throw;
            }

        }



        public ActionResult ReadMessages(int pageIndx = 1)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<user_info> list = db.user_info.Where(i => i.user_info_sta == "已读" & i.user_basic_id == user_basic.user_basic_id).OrderByDescending(i => i.user_info_date).ToList();
                IList<user_info> mPage = PageCommon.PageList<user_info>(pageIndx, 20, list.Count, list);
                return View("ReadMessages", mPage);

            }
            catch (Exception)
            {

                throw;
            }

        }
        public ActionResult UnreadMessages(int pageIndx = 1)
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<user_info> list = db.user_info.Where(i => i.user_info_sta == "未读" & i.user_basic_id == user_basic.user_basic_id).OrderByDescending(i => i.user_info_date).ToList();
                IList<user_info> mPage = PageCommon.PageList<user_info>(pageIndx, 20, list.Count, list);
                return View("UnreadMessages", mPage);

            }
            catch (Exception)
            {

                throw;
            }

        }


        public ActionResult MessageCenterD(int user_info_id)
        {
            try
            {
                var query = db.user_info.Where(u => u.user_info_id == user_info_id).SingleOrDefault();
                ViewBag.user_info_title = query.user_info_title;
                ViewBag.user_info_content = query.user_info_content;
                ViewBag.user_info_source = query.user_info_source;
                ViewBag.user_info_date = query.user_info_date;
                var query1 = db.user_info.Where(u => u.user_info_id == user_info_id).SingleOrDefault();
                query1.user_info_sta = "已读";
                db.SaveChanges();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
