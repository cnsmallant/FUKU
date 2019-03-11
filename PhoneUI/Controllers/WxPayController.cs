using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using WxPayAPI;
using LitJson;
using System.Xml.Linq;
using System.Net;
using System.Xml;

namespace PhoneUI.Controllers
{
    public class WxPayController : Controller
    {

        /// <summary>
        /// openid用于调用统一下单接口
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// access_token用于获取收货地址js函数入口参数
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 商品金额，用于统一下单
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxPayData unifiedOrderResult { get; set; }

        public static string wxJsApiParam { get; set; } //H5调起JS API参数
        //
        // GET: /WxPay/

        D8MallEntities db = new D8MallEntities();
        /// <summary>
        /// 支付授权
        /// </summary>
        /// <returns></returns>
        public ActionResult Authorize()
        {
            try
            {

                if (HttpContext.Request.Cookies["shop_order_totalmoeny"] != null || HttpContext.Request.Cookies["shop_order_code"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["code"]))
                    {
                        string code = Request.QueryString["code"];

                        GetOpenidAndAccessTokenFromCode(code);
                        var shop_order_totalmoeny = TDESHelper.DecryptString(HttpContext.Request.Cookies["shop_order_totalmoeny"].Value);
                        var shop_order_code = TDESHelper.DecryptString(HttpContext.Request.Cookies["shop_order_code"].Value);
                        string[] str = shop_order_totalmoeny.Split('￥');//去除￥
                        shop_order_totalmoeny = str[1].ToString();//获取纯数字字符串
                        total_fee = Convert.ToInt32((Convert.ToDecimal(shop_order_totalmoeny) * 100));
                        WxPayData unifiedOrderResult = GetUnifiedOrderResult();
                        wxJsApiParam = GetJsApiParameters();//获取H5调起JSAPI参数    
                        ViewBag.code = wxJsApiParam;
                        ViewBag.wxJsApiParam = wxJsApiParam;
                        var appId = JSONHelper.JsonToString(wxJsApiParam, "appId");
                        var nonceStr = JSONHelper.JsonToString(wxJsApiParam, "nonceStr");
                        var package = JSONHelper.JsonToString(wxJsApiParam, "package");
                        var paySign = JSONHelper.JsonToString(wxJsApiParam, "paySign");
                        var signType = JSONHelper.JsonToString(wxJsApiParam, "signType");
                        var timeStamp = JSONHelper.JsonToString(wxJsApiParam, "timeStamp");
                        //  string payparameter = appId + "|" + nonceStr + "|" + package + "|" + paySign + "|" + signType + "|" + timeStamp;
                        string pay = TDESHelper.EncryptString(wxJsApiParam);
                        return RedirectToAction("Pay", "WxPay", new { pay = pay });


                    }
                    else
                    {
                        return RedirectToAction("Index", "Personal");

                    }
                }
                else
                {
                    string js = "<script>alert('支付失效，请重新支付！');window.location.href ='/Goods/GoodsOrderPay';</script>";
                    return Content(js);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// 支付
        /// </summary>
        /// <returns></returns>
        public ActionResult Pay(string pay)
        {

            try
            {
                string wxJsApiParam = TDESHelper.DecryptString(pay);
                #region 组合H5起调JSAPI
                ViewBag.sss = wxJsApiParam;
                StringBuilder sb = new StringBuilder();
                sb.Append("<script>");
                sb.Append("function pay() {");
                sb.Append("WeixinJSBridge.invoke(");
                sb.Append("'getBrandWCPayRequest',");
                sb.Append(wxJsApiParam);
                sb.Append(",");
                sb.Append(" function (res) {");
                sb.Append("if (res.err_msg == 'get_brand_wcpay_request:ok') {");
                sb.Append("window.location.href ='/WxPay/OrderTracking';");
                sb.Append("} else if (res.err_msg == 'get_brand_wcpay_request:cancel') {");
                sb.Append("window.location.href ='/Personal/Unpaid';");
                sb.Append("} else {");
                sb.Append("alert(res.err_msg)");
                sb.Append("}});}</script>");
                string jsstr = sb.ToString();
                ViewBag.jsstr = jsstr;
                #endregion
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 微信订单跟踪 确保真实交易
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderTracking()
        {
            try
            {
                if (HttpContext.Request.Cookies["shop_order_totalmoeny"] != null && HttpContext.Request.Cookies["shop_order_code"] != null && HttpContext.Request.Cookies["GenerateOutTradeNo"] != null)
                {

                    var GenerateOutTradeNo = TDESHelper.DecryptString(HttpContext.Request.Cookies["GenerateOutTradeNo"].Value);
                    var shop_order_code = TDESHelper.DecryptString(HttpContext.Request.Cookies["shop_order_code"].Value);
                    string out_trade_no = TDESHelper.DecryptString(HttpContext.Request.Cookies["GenerateOutTradeNo"].Value);
                    WxPayData data = new WxPayData();
                    data.SetValue("out_trade_no", out_trade_no);
                    WxPayData result = WxPayApi.OrderQuery(data);//提交订单查询请求给API，接收返回数据
                    var return_code = result.GetValue("return_code").ToString();//return_code
                    if (return_code == "SUCCESS")
                    {
                        var result_code = result.GetValue("result_code").ToString();//业务结果
                        if (result_code == "SUCCESS")
                        {
                            var trade_state = result.GetValue("trade_state").ToString();//交易状态SUCCESS—支付成功REFUND—转入退款NOTPAY—未支付CLOSED—已关闭REVOKED—已撤销（刷卡支付）USERPAYING--用户支付中PAYERROR--支付失败(其他原因，如银行返回失败)
                            if (trade_state == "SUCCESS")
                            {

                                shop_order sh = db.shop_order.Where(s => s.shop_order_code == shop_order_code).SingleOrDefault();
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
                                    Response.Redirect("/PaySuccess/Index");
                                }
                            }
                            else
                            {
                                string js = "<script>alert('支付失败，请重新支付！');window.location.href ='/Personal/Unpaid';</script>";
                                return Content(js);

                            }
                        }
                        else
                        {
                            string js = "<script>alert('支付失败，请重新支付！');window.location.href ='/Personal/Unpaid';</script>";
                            return Content(js);
                        }

                    }
                    else
                    {
                        string js = "<script>alert('支付失败，请重新支付！');window.location.href ='/Personal/Unpaid';</script>";
                        return Content(js);
                    }

                }
                else
                {
                    string js = "<script>alert('支付失败，请重新支付！');window.location.href ='/Personal/Unpaid';</script>";
                    return Content(js);
                }



                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /**
       * 
       * 网页授权获取用户基本信息的全部过程
       * 详情请参看网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
       * 第一步：利用url跳转获取code
       * 第二步：利用code去获取openid和access_token
       * 
       */
        public void GetOpenidAndAccessToken()
        {
            if (!string.IsNullOrEmpty(Request["code"]))
            {
                //获取code码，以获取openid和access_token
                string code = Request["code"];
                WxPayAPI.Log.Debug(this.GetType().ToString(), "Get code : " + code);
                GetOpenidAndAccessTokenFromCode(code);
            }
            else
            {
                //构造网页授权获取code的URL
                string host = Request.Url.Host;
                string path = Request.Path;
                string redirect_uri = HttpUtility.UrlEncode("http://" + host + path);
                WxPayData data = new WxPayData();
                data.SetValue("appid", WxPayConfig.APPID);
                data.SetValue("redirect_uri", redirect_uri);
                data.SetValue("response_type", "code");
                data.SetValue("scope", "snsapi_base");
                data.SetValue("state", "STATE" + "#wechat_redirect");
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
                WxPayAPI.Log.Debug(this.GetType().ToString(), "Will Redirect to URL : " + url);
                try
                {
                    //触发微信返回code码         
                    Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                }
            }
        }
        /**
    * 
    * 通过code换取网页授权access_token和openid的返回数据，正确时返回的JSON数据包如下：
    * {
    *  "access_token":"ACCESS_TOKEN",
    *  "expires_in":7200,
    *  "refresh_token":"REFRESH_TOKEN",
    *  "openid":"OPENID",
    *  "scope":"SCOPE",
    *  "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
    * }
    * 其中access_token可用于获取共享收货地址
    * openid是微信支付jsapi支付接口统一下单时必须的参数
    * 更详细的说明请参考网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
    * @失败时抛异常WxPayException
    */
        public void GetOpenidAndAccessTokenFromCode(string code)
        {
            try
            {
                //构造获取openid及access_token的url
                WxPayData data = new WxPayData();
                data.SetValue("appid", WxPayConfig.APPID);
                data.SetValue("secret", WxPayConfig.APPSECRET);
                data.SetValue("code", code);
                data.SetValue("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?" + data.ToUrl();
                //  string resp = PushToWeb(url);

                //请求url以获取数据
                string result = HttpService.Get(url);

                WxPayAPI.Log.Debug(this.GetType().ToString(), "GetOpenidAndAccessTokenFromCode response : " + result);

                //保存access_token，用于收货地址获取
                JsonData jd = JsonMapper.ToObject(result);
                access_token = (string)jd["access_token"];

                //获取用户openid
                openid = (string)jd["openid"];

                WxPayAPI.Log.Debug(this.GetType().ToString(), "Get openid : " + openid);
                WxPayAPI.Log.Debug(this.GetType().ToString(), "Get access_token : " + access_token);

            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), ex.ToString());
                throw new WxPayException(ex.ToString());
            }
        }

        /**
         * 调用统一下单，获得下单结果
         * @return 统一下单结果
         * @失败时抛异常WxPayException
         */
        public WxPayData GetUnifiedOrderResult()
        {
            var shop_order_totalmoeny = TDESHelper.DecryptString(HttpContext.Request.Cookies["shop_order_totalmoeny"].Value);
            var shop_order_code = TDESHelper.DecryptString(HttpContext.Request.Cookies["shop_order_code"].Value);
            var GenerateOutTradeNo = WxPayApi.GenerateOutTradeNo();//储存微信订单号
            Response.Cookies["GenerateOutTradeNo"].Value = TDESHelper.EncryptString(GenerateOutTradeNo);
            Response.Cookies["GenerateOutTradeNo"].Expires = DateTime.Now.AddHours(2);
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", shop_order_code);
            data.SetValue("attach", shop_order_code);
            data.SetValue("out_trade_no", GenerateOutTradeNo);
            data.SetValue("total_fee", total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", shop_order_code);
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", openid);

            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }

            unifiedOrderResult = result;
            return result;
        }

        /**
    *  
    * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
    * 微信浏览器调起JSAPI时的输入参数格式如下：
    * {
    *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
    *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
    *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
    *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
    *   "signType" : "MD5",         //微信签名方式:    
    *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
    * }
    * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
    * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
    * 
    */
        public string GetJsApiParameters()
        {
            WxPayAPI.Log.Debug(this.GetType().ToString(), "JsApiPay::GetJsApiParam is processing...");

            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            string parameters = jsApiParam.ToJson();

            WxPayAPI.Log.Debug(this.GetType().ToString(), "Get jsApiParam : " + parameters);
            return parameters;
        }

        /***
       * 订单查询完整业务流程逻辑
       * @param transaction_id 微信订单号（优先使用）
       * @param out_trade_no 商户订单号
       * @return 订单查询结果（xml格式）
       */
        public static string Run(string transaction_id, string out_trade_no)
        {
            WxPayAPI.Log.Info("OrderQuery", "OrderQuery is processing...");

            WxPayData data = new WxPayData();
            if (!string.IsNullOrEmpty(transaction_id))//如果微信订单号存在，则以微信订单号为准
            {
                data.SetValue("transaction_id", transaction_id);
            }
            else//微信订单号不存在，才根据商户订单号去查单
            {
                data.SetValue("out_trade_no", out_trade_no);
            }

            WxPayData result = WxPayApi.OrderQuery(data);//提交订单查询请求给API，接收返回数据

            WxPayAPI.Log.Info("OrderQuery", "OrderQuery process complete, result : " + result.ToXml());
            return result.ToPrintStr();
        }


        private string PushToWeb(string Url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
