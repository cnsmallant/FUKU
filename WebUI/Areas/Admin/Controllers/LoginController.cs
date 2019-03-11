using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;

namespace WebUI.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Admin/Login/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserLogin()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var uname = JSONHelper.JsonToString(jsonstr, "uname");
                var upwd = TDESHelper.EncryptString(JSONHelper.JsonToString(jsonstr, "upwd"));
                var query = db.sys_admin;
                var sys_admin = query.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                string result = string.Empty;
                if (sys_admin != null)
                {
                    Response.Cookies["uname"].Value = TDESHelper.EncryptString(uname);
                    Response.Cookies["uname"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["upwd"].Value = upwd;
                    Response.Cookies["upwd"].Expires = DateTime.Now.AddDays(1);
                    Log.LogTxt("登录系统", sys_admin.sys_admin_name);
                    #region 处理过期订单
                    var ord = db.shop_order.Where(o => o.shop_order_status == "已发货").ToList();
                    foreach (var item in ord)
                    {
                        var date = Convert.ToDateTime(item.shop_order_editdate).AddDays(10);//10天后没有签收订单的默认签收
                        var datereg = DateTime.Now;
                        if (date <= datereg)
                        {
                            shop_order o = db.shop_order.Where(or => or.shop_order_id == item.shop_order_id).SingleOrDefault();
                            o.shop_order_status = "已签收";
                            db.Configuration.ValidateOnSaveEnabled = false;
                            int result_ord = db.SaveChanges();
                            db.Configuration.ValidateOnSaveEnabled = true;
                            if (result_ord > 0)
                            {
                                SendSMS sms = new SendSMS();
                                user_basic user = db.user_basic.Where(u => u.user_basic_id == o.user_basic_id).SingleOrDefault();
                                string content = "尊敬的顾客您好！您的福库商城订单：" + o.shop_order_code + "已经超过10天签收期，系统已经默认签收并默认评价。查看订单状态请登录：http://www.cuckooshop.cn/Personal/ClosedOrders 进行查看。如果您未收到商品，请您致电：0532-87905615转232进行咨询。";
                                sms.SendSms(user.user_basic_tel, content, "111");
                                var ords = db.shop_orderdetail.Where(od => od.shop_order_id == o.shop_order_id).ToList();
                                foreach (var items in ords)
                                {
                                    pro_comment pc = new pro_comment();
                                    pc.pro_comment_id = Guid.NewGuid().ToString("N");
                                    pc.pro_comment_star = "5";
                                    pc.pro_comment_content = "此商品已经超过签收日期，系统默认五星好评！";
                                    pc.pro_comment_date = DateTime.Now;
                                    pc.pro_sku_code = items.pro_skuitem_id;
                                    pc.user_basic_id = o.user_basic_id;
                                    db.pro_comment.Add(pc);
                                    db.Configuration.ValidateOnSaveEnabled = false;
                                    db.SaveChanges();
                                    db.Configuration.ValidateOnSaveEnabled = true;
                                }

                            }
                        }
                    }
                    #endregion


                    #region 处理客户已删除订单
                    var orddel = db.shop_order.Where(o => o.shop_order_status == "已删除").ToList();
                    foreach (var item in orddel)
                    {
                        var orded = db.shop_orderdetail.Where(s => s.shop_order_id == item.shop_order_id).ToList();
                        foreach (var items in orded)
                        {
                            var ordeds = db.shop_orderdetail.Where(s => s.shop_orderdetail_id == items.shop_orderdetail_id).SingleOrDefault();
                            db.shop_orderdetail.Remove(ordeds);
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                            db.Configuration.ValidateOnSaveEnabled = true;
                        }
                       
                        var orde = db.shop_order.Where(s => s.shop_order_id == item.shop_order_id).SingleOrDefault();
                        db.shop_order.Remove(orde);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        db.Configuration.ValidateOnSaveEnabled = true;
                    }
                  
                    #endregion
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
