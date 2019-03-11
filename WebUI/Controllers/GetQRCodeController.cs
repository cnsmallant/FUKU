using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;
using EFClassLibrary;

namespace WebUI.Controllers
{
    public class GetQRCodeController : Controller
    {
        //
        // GET: /GetQRCode/

        D8MallEntities db = new D8MallEntities();
        /// <summary>
        ///模式二
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Index()
        {
            object objResult = "";
            var stre = HttpContext.Request.InputStream;
            var jsonstr = new StreamReader(stre).ReadToEnd();
            var time = JSONHelper.JsonToString(jsonstr, "time");
            var productId = JSONHelper.JsonToString(jsonstr, "productId");

            string strProductID = productId;
            string strQRCodeStr = GetPrePayUrl(strProductID);
            if (!string.IsNullOrWhiteSpace(strProductID))
            {
                objResult = new { result = true, str = strQRCodeStr };
            }
            else
            {
                objResult = new { result = false };
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        ///// <summary>
        ///// 返回结果
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult ResultCode()
        //{
        //    try
        //    {
        //        object objResult = "";
        //        var stre = HttpContext.Request.InputStream;
        //        var jsonstr = new StreamReader(stre).ReadToEnd();
        //        var time = JSONHelper.JsonToString(jsonstr, "time");
        //        var productId = JSONHelper.JsonToString(jsonstr, "productId");

        //        string strProductID = productId;
        //        string strQRCodeStr = GetResultCode(strProductID);
        //        if (strQRCodeStr == "SUCCESS")
        //        {
        //            objResult = "OK";
        //        }
        //        else
        //        {
        //            objResult = "NO";
        //        }
        //        return Json(objResult, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        /**
       * 生成扫描支付模式二URL
       * @param productId 商品ID
       * @return 模式二URL
       */
        public string GetPrePayUrl(string productId)
        {
            //WxPayData data = new WxPayData();
            //data.SetValue("appid", WxPayConfig.APPID);//公众帐号id
            //data.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            //data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            //data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            //data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            //data.SetValue("total_fee", 1);//总金额
            //data.SetValue("trade_type", "NATIVE");//交易类型
            //data.SetValue("product_id", productId);//商品ID
            //data.SetValue("sign", data.MakeSign());//签名
            //string str = ToUrlParams(data.GetValues());//转换为URL串
            //string url = "weixin://wxpay/bizpayurl?" + str;
            //return url;

            shop_order o = db.shop_order.Where(s => s.shop_order_code == productId).SingleOrDefault();
            decimal tol = Convert.ToDecimal(o.shop_order_totalmoeny) * 100;//金额换算
            int money = (int)tol;//金额类型转换
            string GenerateOutTradeNo = WxPayApi.GenerateOutTradeNo();
            Response.Cookies["GenerateOutTradeNo"].Value = TDESHelper.EncryptString(GenerateOutTradeNo);
            Response.Cookies["GenerateOutTradeNo"].Expires = DateTime.Now.AddMinutes(30);
            WxPayData data = new WxPayData();
            data.SetValue("body", o.shop_order_code);//商品描述
            data.SetValue("attach", o.shop_order_code);//附加数据
            data.SetValue("out_trade_no", GenerateOutTradeNo);//随机字符串
            data.SetValue("total_fee", money);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", o.shop_order_code);//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", productId);//商品ID


            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
            return url;

        }




        /***
        * 订单查询完整业务流程逻辑
        * @param transaction_id 微信订单号（优先使用）
        * @param out_trade_no 商户订单号
        * @return 订单查询结果（xml格式）
        */
        public JsonResult Run()
        {
            string out_trade_no01 = TDESHelper.DecryptString(HttpContext.Request.Cookies["GenerateOutTradeNo"].Value);
            if (HttpContext.Request.Cookies["GenerateOutTradeNo"] != null)
            {
                string out_trade_no = TDESHelper.DecryptString(HttpContext.Request.Cookies["GenerateOutTradeNo"].Value);
                WxPayData data = new WxPayData();
                data.SetValue("out_trade_no", out_trade_no);
                WxPayData result = WxPayApi.OrderQuery(data);//提交订单查询请求给API，接收返回数据
                var re = result.GetValue("trade_state").ToString();
                return Json(re, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NO", JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Pay()
        {
            var stre = HttpContext.Request.InputStream;
            var jsonstr = new StreamReader(stre).ReadToEnd();
            var productId = JSONHelper.JsonToString(jsonstr, "productId");
            shop_order sh = db.shop_order.Where(s => s.shop_order_code == productId).SingleOrDefault();
            sh.shop_order_status = "已支付";
            sh.shop_pay_id = "wx";
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
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NO", JsonRequestBehavior.AllowGet);
            }
        }

        // /**
        //* 参数数组转换为url格式
        //* @param map 参数名与参数值的映射表
        //* @return URL字符串
        //*/
        // private string ToUrlParams(SortedDictionary<string, object> map)
        // {
        //     string buff = "";
        //     foreach (KeyValuePair<string, object> pair in map)
        //     {
        //         buff += pair.Key + "=" + pair.Value + "&";
        //     }
        //     buff = buff.Trim('&');
        //     return buff;
        // }
    }
}
