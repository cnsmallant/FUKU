using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneUI.Controllers
{
    public class SMSController : Controller
    {
        //
        // GET: /SMS/
        D8MallEntities db = new D8MallEntities();
        [HttpGet]
        public JsonResult Index(string phone)
        {
            try
            {
                string code;
                Random ran = new Random();
                int RandKey = ran.Next(100000, 1000000);
                code = RandKey.ToString();
                SendSMS sms = new SendSMS();
                string content = "【福库商城】亲爱的用户，您的验证码为" + code + "，10分钟内有效，请勿将验证码泄露给他人！";
                var result = sms.SendSms(phone, content, code);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
