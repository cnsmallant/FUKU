using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using System.Text;

namespace WebUI.Areas.Admin.Controllers
{
    [AdminVerification]
    public class OrderController : Controller
    {
        //
        // GET: /Admin/Order/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {
            return View();
        }

        #region 订单管理
        /// <summary>
        /// 未支付订单
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderUnpaid(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "未支付").ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 20, op.Count, op);
                return View("OrderUnpaid", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult OrderRevisedPrice(string shop_order_id)
        {
            try
            {
                var shop_order = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                ViewBag.shop_order_ytotalmoeny = Convert.ToDecimal(shop_order.shop_order_totalmoeny).ToString("C");

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult OrderRevisedPriceE(string shop_order_id)
        {
            try
            {
                var shop_order = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                shop_order.shop_order_totalmoeny = Request.Form["shop_order_totalmoeny"];
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("OrderUnpaid", "Order");
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
        /// 未发货订单
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderUnfilled(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已支付").ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 20, op.Count, op);
                return View("OrderUnfilled", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public ActionResult OrderSta(string shop_order_id, string sta)
        {
            try
            {
                var or = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                or.shop_order_status = Server.UrlDecode(sta);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("OrderUnfilled", "Order");
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
        /// 执行发货
        /// </summary>
        /// <returns></returns>
        public ActionResult ExecutiveShip()
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
        /// 修改订单-执行发货
        /// </summary>
        /// <returns></returns>
        public ActionResult EditExecutiveShip(string shop_order_id)
        {
            try
            {
                SendSMS sms = new SendSMS();
                shop_order order = db.shop_order.Where(s => s.shop_order_id == shop_order_id).SingleOrDefault();
                order.shop_shipping_id = Request.Form["shop_shipping_id"];
                order.shop_order_shipmentnumber = Request.Form["shop_order_shipmentnumber"];
                order.shop_order_status = "已发货";
                order.shop_order_remark = "BY";
                order.shop_order_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    user_basic u = db.user_basic.Where(us => us.user_basic_id == order.user_basic_id).SingleOrDefault();
                    string content = "【福库商城】亲爱的用户，您的订单已发货，订单号：" + order.shop_order_code + "，" + order.shop_shipping_id + "快递单号：" + order.shop_order_shipmentnumber + "。请勿将此信息泄露给他人！";
                    sms.SendSms(u.user_basic_tel, content, "200");
                    return RedirectToAction("OrderUnfilled", "Order");
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
        /// 未签收订单
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderNosign(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已发货").ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 20, op.Count, op);
                return View("OrderNosign", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 已签收
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderReceipt(int pageIndx = 1)
        {
            try
            {
                IList<shop_order> ord = OrderPage();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == "已签收").ToList();
                IList<shop_order> mPage = PageCommon.PageList<shop_order>(pageIndx, 20, op.Count, op);
                return View("OrderReceipt", mPage);
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

            IList<shop_order> orderlist = db.shop_order.OrderByDescending(o => o.shop_order_editdate).ToList();
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
                var addre = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault();
                var address = string.Empty;
                var County = string.Empty;
                if (addre == null)
                {
                    address = "";
                    County = "";

                }
                else
                {
                    int user_address_province = Convert.ToInt32(addre.user_address_province);
                    int user_address_city = Convert.ToInt32(addre.user_address_city);
                    int user_address_county = Convert.ToInt32(addre.user_address_county);
                    var province = db.com_area.Where(a => a.com_area_id == user_address_province).SingleOrDefault().com_area_name;
                    var city = db.com_area.Where(a => a.com_area_id == user_address_city).SingleOrDefault().com_area_name;
                    var county = db.com_area.Where(a => a.com_area_id == user_address_county).SingleOrDefault().com_area_name;
                    address = "收货人：" + addre.user_address_name + "，收货地址：" + province + city + county + addre.user_address_detail + "，收货人手机：" + addre.user_address_tel + "，邮政编码：" + addre.user_address_ZipCode;
                    County = db.user_address.Where(ua => ua.user_address_id == item.user_address_id).SingleOrDefault().user_address_county;

                }
                so.user_address_id = address;
                so.shop_order_remark = item.shop_order_remark;

                so.shop_shipping_id = item.shop_shipping_id;
                so.shop_order_editdate = item.shop_order_editdate;
                sor.Add(so);
            }
            IList<shop_order> mPage = sor;
            return mPage;
        }

        #region 导出
        public FileResult ExportExcel(string shop_order_status, string stadate, string enddate)
        {

            try
            {

                string sta = Server.UrlDecode(shop_order_status);
                DateTime sdate = Convert.ToDateTime(Server.UrlDecode(stadate));
                DateTime edate = Convert.ToDateTime(Server.UrlDecode(enddate));
                IList<shop_order> ord = db.shop_order.ToList();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == sta && o.shop_order_editdate >= sdate && o.shop_order_editdate <= edate).OrderByDescending(o => o.shop_order_buydate).ToList();
                var sbHtml = new StringBuilder();
                var list = op;
                sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
                sbHtml.Append("<tr>");
                var lstTitle = new List<string> { "订单编号", "购买商品", "商品总数", "订单状态", "订单总价", "优惠类型", "优惠折扣", "支付价格", "支付方式", "购买时间", "会员帐号", "收货人", "收货地址", "收货人手机", "收货邮政编码", "发货快递名称", "发货快递单号", "订单发货时间" };

                foreach (var item in lstTitle)
                {
                    sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
                }
                sbHtml.Append("</tr>");
                foreach (var item in list)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_code);//订单编号

                    var list2 = db.shop_orderdetail.Where(so => so.shop_order_id == item.shop_order_id).ToList();
                    string s = string.Empty;
                    decimal pm = 0;
                    foreach (var items in list2)
                    {
                        pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == items.pro_skuitem_id).SingleOrDefault();
                        s += sku.pro_sku_title + "(" + items.shop_orderdetail_num + "件，商品单价：" + items.shop_orderdetail_price + "元)<br/>";
                        decimal pr = Convert.ToDecimal(items.shop_orderdetail_total);
                        pm += pr;
                    }
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", s);//购买商品
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_waynum);//商品总数
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_status);//订单状态
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", pm);//订单总价
                    if (!string.IsNullOrEmpty(item.pro_coupon_id))
                    {
                        int pro_coupon_id = Convert.ToInt32(item.pro_coupon_id);

                        var user_coupon = db.user_coupon.Where(u => u.user_coupon_id == pro_coupon_id).SingleOrDefault();
                        if (user_coupon == null)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠类型
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠折扣
                        }
                        else
                        {
                            var pro_coupon = db.pro_coupon.Where(pc => pc.pro_coupon_id == user_coupon.pro_coupon_id).SingleOrDefault();
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", pro_coupon.pro_coupon_class);//优惠类型
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", pro_coupon.pro_coupon_discount);//优惠折扣
                        }
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠类型
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠折扣
                    }

                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_totalmoeny);//支付价格
                    if (item.shop_pay_id == "wx")//支付方式
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "微信支付");
                    }
                    else if (item.shop_pay_id == "zfb")
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "支付宝支付");
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                    }
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_buydate);//购买时间
                    var username = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault();
                    if (username != null)
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", username.user_basic_login);//会员帐号
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                    }
                    var useradd = db.user_address.Where(ud => ud.user_address_id == item.user_address_id).SingleOrDefault();
                    if (useradd != null)
                    {
                        int user_address_province = Convert.ToInt32(useradd.user_address_province);
                        int user_address_city = Convert.ToInt32(useradd.user_address_city);
                        int user_address_county = Convert.ToInt32(useradd.user_address_county);
                        var province = db.com_area.Where(a => a.com_area_id == user_address_province).SingleOrDefault().com_area_name;
                        var city = db.com_area.Where(a => a.com_area_id == user_address_city).SingleOrDefault().com_area_name;
                        var county = db.com_area.Where(a => a.com_area_id == user_address_county).SingleOrDefault().com_area_name;
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", useradd.user_address_name);//收货人
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", province + city + county + useradd.user_address_detail);//收货地址
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", useradd.user_address_tel);//收货人手机
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", useradd.user_address_ZipCode);//收货人邮政编码
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");
                    }

                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_shipping_id);//发货快递名称
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_shipmentnumber);//发货快递单号
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_editdate);//发货时间
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</table>");

                byte[] fileContents = Encoding.UTF8.GetBytes(sbHtml.ToString());
                return File(fileContents, "application/ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + shop_order_status + ".xls");
            }
            catch (Exception)
            {

                throw;
            }
        }


        public FileResult ExportExcel02(string shop_order_status)
        {

            try
            {

                string sta = Server.UrlDecode(shop_order_status);
                IList<shop_order> ord = db.shop_order.ToList();
                IList<shop_order> op = ord.Where(o => o.shop_order_status == sta).OrderByDescending(o => o.shop_order_buydate).ToList();
                var sbHtml = new StringBuilder();
                var list = op;
                sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
                sbHtml.Append("<tr>");
                var lstTitle = new List<string> { "订单编号", "购买商品", "商品总数", "订单状态", "订单总价", "优惠类型", "优惠折扣", "支付价格", "支付方式", "购买时间", "会员帐号", "收货人", "收货地址", "收货人手机", "收货邮政编码", "发货快递名称", "发货快递单号", "订单发货时间" };

                foreach (var item in lstTitle)
                {
                    sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
                }
                sbHtml.Append("</tr>");
                foreach (var item in list)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_code);//订单编号

                    var list2 = db.shop_orderdetail.Where(so => so.shop_order_id == item.shop_order_id).ToList();
                    string s = string.Empty;
                    decimal pm = 0;
                    foreach (var items in list2)
                    {
                        pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == items.pro_skuitem_id).SingleOrDefault();
                        s += sku.pro_sku_title + "(" + items.shop_orderdetail_num + "件，商品单价：" + items.shop_orderdetail_price + "元)<br/>";
                        decimal pr = Convert.ToDecimal(items.shop_orderdetail_total.ToString());
                        pm += pr;
                    }
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", s);//购买商品
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_waynum);//商品总数
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_status);//订单状态
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", pm);//订单总价
                    if (!string.IsNullOrEmpty(item.pro_coupon_id))
                    {
                        int pro_coupon_id = Convert.ToInt32(item.pro_coupon_id);

                        var user_coupon = db.user_coupon.Where(u => u.user_coupon_id == pro_coupon_id).SingleOrDefault();
                        if (user_coupon == null)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠类型
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠折扣
                        }
                        else
                        {
                            var pro_coupon = db.pro_coupon.Where(pc => pc.pro_coupon_id == user_coupon.pro_coupon_id).SingleOrDefault();
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", pro_coupon.pro_coupon_class);//优惠类型
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", pro_coupon.pro_coupon_discount);//优惠折扣
                        }
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠类型
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");//优惠折扣
                    }

                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_totalmoeny);//支付价格
                    if (item.shop_pay_id == "wx")//支付方式
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "微信支付");
                    }
                    else if (item.shop_pay_id == "zfb")
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "支付宝支付");
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                    }
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_buydate);//购买时间
                    var username = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault();
                    if (username != null)
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", username.user_basic_login);//会员帐号
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                    }
                    var useradd = db.user_address.Where(ud => ud.user_address_id == item.user_address_id).SingleOrDefault();
                    if (useradd != null)
                    {
                        int user_address_province = Convert.ToInt32(useradd.user_address_province);
                        int user_address_city = Convert.ToInt32(useradd.user_address_city);
                        int user_address_county = Convert.ToInt32(useradd.user_address_county);
                        var province = db.com_area.Where(a => a.com_area_id == user_address_province).SingleOrDefault().com_area_name;
                        var city = db.com_area.Where(a => a.com_area_id == user_address_city).SingleOrDefault().com_area_name;
                        var county = db.com_area.Where(a => a.com_area_id == user_address_county).SingleOrDefault().com_area_name;
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", useradd.user_address_name);//收货人
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", province + city + county + useradd.user_address_detail);//收货地址
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", useradd.user_address_tel);//收货人手机
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", useradd.user_address_ZipCode);//收货人邮政编码
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", "----");
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", "----");
                    }

                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_shipping_id);//发货快递名称
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_shipmentnumber);//发货快递单号
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_editdate);//发货时间
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</table>");

                byte[] fileContents = Encoding.UTF8.GetBytes(sbHtml.ToString());
                return File(fileContents, "application/ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + shop_order_status + ".xls");
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion


        #endregion


        #region 退换货

        public ActionResult Returns(int pageid = 1)
        {
            try
            {
                var query = db.shop_choose;
                var lists = query.OrderByDescending(q => q.shop_choose_date).ToList();
                IList<shop_choose> list = new List<shop_choose>();
                foreach (var item in lists)
                {
                    shop_choose s = new shop_choose();
                    s.shop_choose_id = item.shop_choose_id;
                    s.shop_order_id = item.shop_order_id;
                    s.shop_choose_code = item.shop_choose_code;
                    s.shop_choose_date = item.shop_choose_date;
                    s.shop_choose_des = item.shop_choose_des;
                    s.shop_orderdetail_id = item.shop_orderdetail_id;
                    s.shop_choose_status = item.shop_choose_status;
                    s.shop_choose_service = item.shop_choose_service;
                    s.shop_choose_isget = item.shop_choose_isget;
                    s.user_basic_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_login;
                    s.user_address_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_tel;
                    s.shop_express_name = item.shop_express_name;
                    s.shop_express_code = item.shop_express_code;
                    list.Add(s);

                }
                IList<shop_choose> mPage = PageCommon.PageList<shop_choose>(pageid, 20, lists.Count, list);
                return View("Returns", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult StatusReturns(string Status, int pageid = 1)
        {
            try
            {
                string status = Server.UrlDecode(Status);
                var query = db.shop_choose;
                var lists = query.Where(q => q.shop_choose_status == status).OrderByDescending(q => q.shop_choose_date).ToList();
                IList<shop_choose> list = new List<shop_choose>();
                foreach (var item in lists)
                {
                    shop_choose s = new shop_choose();
                    s.shop_choose_id = item.shop_choose_id;
                    s.shop_order_id = item.shop_order_id;
                    s.shop_choose_code = item.shop_choose_code;
                    s.shop_choose_date = item.shop_choose_date;
                    s.shop_choose_des = item.shop_choose_des;
                    s.shop_orderdetail_id = item.shop_orderdetail_id;
                    s.shop_choose_status = item.shop_choose_status;
                    s.shop_choose_service = item.shop_choose_service;
                    s.shop_choose_isget = item.shop_choose_isget;
                    s.user_basic_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_login;
                    s.user_address_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_tel;
                    s.shop_express_name = item.shop_express_name;
                    s.shop_express_code = item.shop_express_code;
                    list.Add(s);

                }
                IList<shop_choose> mPage = PageCommon.PageList<shop_choose>(pageid, 20, lists.Count, list);
                return View("Returns", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult ReturnsStatus(string shop_choose_id, string shop_choose_status)
        {
            try
            {
                shop_choose ss = db.shop_choose.Where(s => s.shop_choose_id == shop_choose_id).SingleOrDefault();
                ss.shop_choose_status = Server.UrlDecode(shop_choose_status);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Returns", "Order");
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
        /// 查看订单商品
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckOrder(string shop_choose_id)
        {
            try
            {
                shop_choose shop = db.shop_choose.Where(s => s.shop_choose_id == shop_choose_id).SingleOrDefault();
                shop_orderdetail sd = db.shop_orderdetail.Where(d => d.shop_orderdetail_id == shop.shop_orderdetail_id).SingleOrDefault();
                pro_sku sku = db.pro_sku.Where(p => p.pro_sku_code == sd.pro_skuitem_id).SingleOrDefault();
                ViewBag.pro_sku_covimg = sku.pro_sku_covimg;
                ViewBag.pro_sku_title = sku.pro_sku_title;
                ViewBag.shop_orderdetail_total = sd.shop_orderdetail_total;
                ViewBag.shop_orderdetail_num = sd.shop_orderdetail_num;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public FileResult ExportExcelReturns(string Status)
        {

            try
            {

                string status = Server.UrlDecode(Status);
                var query = db.shop_choose;
                var lists = query.Where(q => q.shop_choose_status == status).OrderByDescending(q => q.shop_choose_date).ToList();
                IList<shop_choose> list = new List<shop_choose>();
                foreach (var item in lists)
                {
                    shop_choose s = new shop_choose();
                    s.shop_choose_id = item.shop_choose_id;
                    s.shop_order_id = item.shop_order_id;
                    s.shop_choose_code = item.shop_choose_code;
                    s.shop_choose_date = item.shop_choose_date;
                    s.shop_choose_des = item.shop_choose_des;
                    s.shop_orderdetail_id = item.shop_orderdetail_id;
                    s.shop_choose_status = item.shop_choose_status;
                    s.shop_choose_service = item.shop_choose_service;
                    s.shop_choose_isget = item.shop_choose_isget;
                    s.user_basic_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_login;
                    s.user_address_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_tel;
                    s.shop_express_name = item.shop_express_name;
                    s.shop_express_code = item.shop_express_code;
                    list.Add(s);

                }
                var sbHtml = new StringBuilder();

                sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
                sbHtml.Append("<tr>");
                var lstTitle = new List<string> { "用户", "联系方式", "类别", "服务编码", "状态", "快递名称", "快递单号", "时间" };

                foreach (var item in lstTitle)
                {
                    sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
                }
                sbHtml.Append("</tr>");
                foreach (var item in list)
                {
                    sbHtml.Append("<tr>");
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.user_basic_id);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.user_address_id);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_choose_service);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_choose_code);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_choose_status);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_express_name);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_express_code);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_choose_date);
                    //sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_shipping_id);
                    //sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;vnd.ms-excel.numberformat:@'>{0}</td>", item.shop_order_shipmentnumber);
                    //sbHtml.AppendFormat("<td style='font-size: 12px;height:auto;'>{0}</td>", item.shop_order_editdate);
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</table>");

                byte[] fileContents = Encoding.UTF8.GetBytes(sbHtml.ToString());
                return File(fileContents, "application/ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + status + ".xls");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region 评论
        public ActionResult OrderComment(int pageid = 1)
        {
            try
            {
                var List = db.pro_comment.OrderByDescending(c => c.pro_comment_date).ToList();
                IList<pro_comment> list = new List<pro_comment>();
                foreach (var item in List)
                {
                    pro_comment p = new pro_comment();
                    p.pro_comment_content = item.pro_comment_content;
                    p.pro_sku_code = db.pro_sku.Where(pro => pro.pro_sku_code == item.pro_sku_code).SingleOrDefault().pro_sku_title;
                    p.pro_comment_id = item.pro_comment_id;
                    list.Add(p);

                }
                var mPage = PageCommon.PageList(pageid, 20, list.Count, list);
                return View(mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult OrderCommentDel(string pro_comment_id)
        {
            try
            {
                pro_comment pro = db.pro_comment.Where(p => p.pro_comment_id == pro_comment_id).SingleOrDefault();
                db.pro_comment.Remove(pro);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("OrderComment", "Order");
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
        #endregion

    }
}
