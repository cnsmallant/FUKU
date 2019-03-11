using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneUI.Controllers
{
    [Personal]
    public class PersonalController : Controller
    {

        public class user_collects : user_collect
        {
            public string pro_skuitem_price { get; set; }
            public string pro_sku_covimg { get; set; }

            public string pro_sku_code { get; set; }
        }

        public class pro_skus : pro_sku
        {
            public string pro_skuitem_price { get; set; }
            public string pro_skuitem_mprice { get; set; }
            public string pro_skuitem_tmprice { get; set; }
            public string pro_skuitem_tprice { get; set; }
            public int shop_car_num { get; set; }
            public string item { get; set; }
            public string pro_brand_id { get; set; }

            public string pro_item_subtitle { get; set; }
        }
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

            #region 我的信息
            ViewBag.username = user_basic.user_basic_login;
            ViewBag.userid = user_basic.user_basic_code;
            ViewBag.user_basic_tels = user_basic.user_basic_tels;
            ViewBag.user_basic_emails = user_basic.user_basic_emails;


            #endregion


            #region 收藏

            var user_collects = db.user_collect.Where(c => c.user_basic_id == user_basic.user_basic_id).OrderByDescending(c => c.user_collect_adddate).ToList();
            IList<user_collects> user_collect_list = new List<user_collects>();
            foreach (var item in user_collects)
            {
                user_collects co = new user_collects();
                pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == item.pro_skuitem_id).SingleOrDefault();
                co.user_collect_id = item.user_collect_id;
                pro_item pitk = db.pro_item.Where(pi => pi.pro_item_id == sku.pro_item_id).SingleOrDefault();
                if (pitk == null)
                {
                    co.pro_skuitem_id = "-";
                }
                else
                {
                    co.pro_skuitem_id = pitk.pro_brand_id;
                }
                co.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("F2");
                co.pro_sku_covimg = sku.pro_sku_covimg;
                co.pro_sku_code = sku.pro_sku_code;
                user_collect_list.Add(co);
            }
            ViewBag.user_collect_list = user_collect_list;
            #endregion


            #region 特价商品
            IList<pro_skus> listskus = new List<pro_skus>();
            var sales = db.pro_sales.Where(p => p.pro_sales_stastus == "1").OrderByDescending(n => n.pro_sales_order).ToList().Take(15);
            foreach (var item in sales)
            {
                pro_skus sku = new pro_skus();
                pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                sku.pro_sku_title = skus.pro_sku_title;
                sku.pro_sku_covimg = skus.pro_sku_covimg;
                sku.pro_sku_code = item.pro_sku_code;
                sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;
                sku.pro_item_subtitle = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_item_subtitle;
                sku.pro_skuitem_mprice = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_mprice).ToString("C");
                sku.pro_skuitem_price = Convert.ToDecimal(item.pro_sales_price.ToString()).ToString("F2");
                listskus.Add(sku);
            }
            ViewBag.goodes = listskus;


            #endregion
            return View();
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
        #endregion

        #region 我的订单
        /// <summary>
        /// 订单
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalOrder()
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


        #region 未支付订单
        /// <summary>
        /// 未支付订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Unpaid()
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "未支付").OrderByDescending(o => o.shop_order_buydate).ToList();
                return View("Unpaid", op);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 已支付
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderPaid()
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已支付").OrderByDescending(o => o.shop_order_buydate).ToList();
                return View("OrderPaid", op);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 已发货订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Shipped()
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已发货").OrderByDescending(o => o.shop_order_buydate).ToList();
                return View("Shipped", op);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 已签收订单
        /// </summary>
        /// <returns></returns>
        public ActionResult ClosedOrders()
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已签收").OrderByDescending(o => o.shop_order_buydate).ToList();
                return View("ClosedOrders", op);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 订单签收
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
        #endregion

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
        #endregion

        #region 我的地址
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
        /// 添加地址
        /// </summary>
        /// <returns></returns>
        public ActionResult ShippingAddressAdd()
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

        /// <summary>
        /// 地址
        /// </summary>
        /// <returns></returns>
        public JsonResult AddShippingAddress()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                user_address ua = JSONHelper.DeserializeJsonToObject<user_address>(jsonstr);
                ua.user_address_id = Guid.NewGuid().ToString("N");
                ua.user_address_name = ua.user_address_name;
                ua.user_address_tel = ua.user_address_tel;
                ua.user_address_province = ua.user_address_province;
                ua.user_address_city = ua.user_address_city;
                ua.user_address_county = ua.user_address_county;
                ua.user_address_detail = ua.user_address_detail;
                ua.user_address_ZipCode = "";
                ua.user_address_tags = "";
                ua.user_basic_id = user_basic.user_basic_id;
                ua.user_address_adddate = DateTime.Now;
                ua.user_address_editdate = DateTime.Now;
                db.user_address.Add(ua);
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
        /// <summary>
        /// 编辑地址
        /// </summary>
        /// <returns></returns>
        public ActionResult ShippingAddressEdit(string user_address_id)
        {
            try
            {
                var query = db.user_basic;
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


        public JsonResult EditShippingAddress()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                user_address uas = JSONHelper.DeserializeJsonToObject<user_address>(jsonstr);
                var query = db.user_basic;
                var user_address_id = uas.user_address_id;
                user_address ua = db.user_address.Where(u => u.user_address_id == user_address_id).SingleOrDefault();
                ua.user_address_name = uas.user_address_name;
                ua.user_address_tel = uas.user_address_tel;
                if (!string.IsNullOrEmpty(uas.user_address_province))
                {
                    ua.user_address_province = uas.user_address_province;
                }
                if (!string.IsNullOrEmpty(uas.user_address_city))
                {
                    ua.user_address_city = uas.user_address_city;
                }
                if (!string.IsNullOrEmpty(uas.user_address_county))
                {
                    ua.user_address_county = uas.user_address_county;
                }
                ua.user_address_detail = uas.user_address_detail;

                ua.user_address_editdate = DateTime.Now;
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
        #endregion
        #region 我的收藏
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalCollect()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = db.user_basic.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var query = db.user_collect.Where(c => c.user_basic_id == user_basic.user_basic_id).OrderByDescending(c => c.user_collect_adddate).ToList();
                IList<user_collects> user_collect_list = new List<user_collects>();
                foreach (var item in query)
                {
                    user_collects co = new user_collects();
                    pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == item.pro_skuitem_id).SingleOrDefault();
                    co.user_collect_id = item.user_collect_id;
                    pro_item pitk = db.pro_item.Where(pi => pi.pro_item_id == sku.pro_item_id).SingleOrDefault();
                    if (pitk == null)
                    {
                        co.pro_skuitem_id = "-";
                    }
                    else
                    {
                        co.pro_skuitem_id = pitk.pro_brand_id;
                    }
                    co.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("F2");
                    co.pro_sku_covimg = sku.pro_sku_covimg;
                    co.pro_sku_code = sku.pro_sku_code;
                    user_collect_list.Add(co);
                }
                return View(user_collect_list);
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region 在线客服
        /// <summary>
        /// 在线客服
        /// </summary>
        /// <returns></returns>
        public ActionResult OnlineService()
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
        #endregion

        #region 系统设置
        /// <summary>
        /// 系统设置
        /// </summary>
        /// <returns></returns>
        public ActionResult SysSet()
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


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
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

        /// <summary>
        /// 关于福库
        /// </summary>
        /// <returns></returns>
        public ActionResult AboutUs()
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
        /// <summary>
        /// 账户退出
        /// </summary>
        /// <returns></returns>
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


        #region 我的消息

        /// <summary>
        /// 消息
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageCenter()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                IList<user_info> list = db.user_info.Where(i => i.user_basic_id == user_basic.user_basic_id).OrderByDescending(i => i.user_info_date).ToList();

                return View("MessageCenter", list);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 更改为已读状态
        /// </summary>
        /// <returns></returns>
        public JsonResult MessageCenterD()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_info_id = Convert.ToInt32(JSONHelper.JsonToString(jsonstr, "user_info_id"));
                var query = db.user_info.Where(u => u.user_info_id == user_info_id).SingleOrDefault();
                ViewBag.user_info_title = query.user_info_title;
                ViewBag.user_info_content = query.user_info_content;
                ViewBag.user_info_source = query.user_info_source;
                ViewBag.user_info_date = query.user_info_date;
                var query1 = db.user_info.Where(u => u.user_info_id == user_info_id).SingleOrDefault();
                query1.user_info_sta = "已读";
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
        #endregion
    }
}
