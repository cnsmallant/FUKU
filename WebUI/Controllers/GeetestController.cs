using GeetestSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    /// <summary>
    /// 极验验证
    /// </summary>
    public class GeetestController : Controller
    {
        //
        // GET: /Geetest/

        public ActionResult Index()
        {
            string result = getCaptcha();

            return Content(result, "application/json");
        }

        private string getCaptcha()
        {
            try
            {
                GeetestLib geetest = new GeetestLib("6e082c1b538c4d0b79685ab07c74a431", "99164f4d16263dc0de27640df33bc4e0");
                Byte gtServerStatus = geetest.preProcess();
                Session[GeetestLib.gtServerStatusSessionKey] = gtServerStatus;
                return geetest.getResponseStr();
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult CheckGeeTestResult()
        {
            GeetestLib geetest = new GeetestLib("6e082c1b538c4d0b79685ab07c74a431", "99164f4d16263dc0de27640df33bc4e0");
            Byte gt_server_status_code = (Byte)Session[GeetestLib.gtServerStatusSessionKey];
            String userID = (String)Session["userID"];
            var stre = HttpContext.Request.InputStream;
            var jsonstr = new StreamReader(stre).ReadToEnd();
            var geetest_challenge = JSONHelper.JsonToString(jsonstr, "geetest_challenge");
            var geetest_validate = JSONHelper.JsonToString(jsonstr, "geetest_validate");
            var geetest_seccode = JSONHelper.JsonToString(jsonstr, "geetest_seccode");
            int result = 0;
            String challenge = geetest_challenge;
            String validate = geetest_validate;
            String seccode = geetest_seccode;
            if (gt_server_status_code == 1)
            {
                result = geetest.enhencedValidateRequest(challenge, validate, seccode, userID);
            }
            else
            {
                result = geetest.failbackValidateRequest(challenge, validate, seccode);
            }
            if (result != 1)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
