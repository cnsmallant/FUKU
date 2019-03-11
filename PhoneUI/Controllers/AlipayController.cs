using Com.Alipay;
using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneUI.Controllers
{
    public class AlipayController : Controller
    {
        //
        // GET: /Alipay/

        D8MallEntities db = new D8MallEntities();

        public ActionResult NotifyUrl()
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];


                    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                    }
                    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //付款完成后，支付宝系统发送该交易状态通知
                    }
                    else
                    {
                    }

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    Response.Write("success");  //请不要修改或删除

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("无通知参数");
            }

            return View();
        }


        public ActionResult ReturnUrl()
        {

            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.QueryString["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];


                    if (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序
                    }
                    else
                    {
                        Response.Write("trade_status=" + Request.QueryString["trade_status"]);
                    }

                    //打印页面
                    //Response.Write("验证成功<br />");
                    shop_order sh = db.shop_order.Where(s => s.shop_order_code == out_trade_no).SingleOrDefault();
                    sh.shop_order_status = "已支付";
                    sh.shop_pay_id = "zfb";
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        var sd = db.shop_orderdetail.Where(s => s.shop_order_id == sh.shop_order_id).ToList();
                        foreach (var item in sd)
                        {
                            pro_sku sku = db.pro_sku.Where(s => s.pro_sku_code == item.pro_skuitem_id).SingleOrDefault();
                            pro_skuitem psk = db.pro_skuitem.Where(sk => sk.pro_sku_id == sku.pro_sku_id).SingleOrDefault();
                            int stock = Convert.ToInt32(psk.pro_skuitem_stock);
                            int num = Convert.ToInt32(item.shop_orderdetail_num);
                            stock = (stock - num);
                            psk.pro_skuitem_stock = stock;
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                            db.Configuration.ValidateOnSaveEnabled = true;
                        }
                        Response.Redirect("/PaySuccess/Index");

                        //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    else//验证失败
                    {
                        Response.Write("验证失败");
                    }
                }
                else
                {
                    Response.Write("无返回参数");
                }
             
            }
            return View();
        }
        /// <summary>
        /// 支付
        /// </summary>
        public void Pay(string shop_order_code)
        {
            try
            {
                ////////////////////////////////////////////请求参数////////////////////////////////////////////
                shop_order so = db.shop_order.Where(s => s.shop_order_code == shop_order_code).SingleOrDefault();//订单查询
                shop_orderdetail sord = db.shop_orderdetail.Where(o => o.shop_order_id == so.shop_order_id).SingleOrDefault();

                //商户订单号，商户网站订单系统中唯一订单号，必填
                string out_trade_no = shop_order_code;

                //订单名称，必填
                string subject = shop_order_code;

                //付款金额，必填
                string total_fee = so.shop_order_totalmoeny;

                //收银台页面上，商品展示的超链接，必填
                string show_url = "http://" + Request.Url.Host + "/Goods/GoodsDetails/" + sord.pro_skuitem_id;

                //商品描述，可空
                string body = "";



                ////////////////////////////////////////////////////////////////////////////////////////////////

                //把请求参数打包成数组
                SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                sParaTemp.Add("partner", Config.partner);
                sParaTemp.Add("seller_id", Config.seller_id);
                sParaTemp.Add("_input_charset", Config.input_charset.ToLower());
                sParaTemp.Add("service", Config.service);
                sParaTemp.Add("payment_type", Config.payment_type);
                sParaTemp.Add("notify_url", Config.notify_url);
                sParaTemp.Add("return_url", Config.return_url);
                sParaTemp.Add("out_trade_no", out_trade_no);
                sParaTemp.Add("subject", subject);
                sParaTemp.Add("total_fee", total_fee);
                sParaTemp.Add("show_url", show_url);
                sParaTemp.Add("app_pay", "Y");//启用此参数可唤起钱包APP支付。
                sParaTemp.Add("body", body);
                //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.2Z6TSk&treeId=60&articleId=103693&docType=1
                //如sParaTemp.Add("参数名","参数值");

                //建立请求
                string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                Response.Write(sHtmlText);
            }
            catch (Exception)
            {

                throw;
            }
        }





        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }



        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }
    }
}
