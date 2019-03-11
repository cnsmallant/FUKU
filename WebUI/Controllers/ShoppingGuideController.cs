using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class ShoppingGuideController : Controller
    {
        //
        // GET: /ShoppingGuide/
        D8MallEntities db = new D8MallEntities();

        public ActionResult Index(string classid)
        {
            ViewBag.classid = classid;
            ViewBag.title = ClassStr(classid);
            com_article arts = db.com_article.Where(a => a.com_article_class == classid).SingleOrDefault();
            if (arts != null)
            {
                ViewBag.com_article_title = arts.com_article_title;
                ViewBag.com_article_content = Server.HtmlDecode(arts.com_article_content);
            }

            return View();
        }

        private string ClassStr(string classid)
        {
            try
            {
                string str = string.Empty;
                if (classid == "about")
                {
                    str = "关于我们";
                }
                if (classid == "member")
                {
                    str = "会员介绍";
                }
                if (classid == "process")
                {
                    str = "购物流程";
                }
                if (classid == "returns")
                {
                    str = "关于售后";
                }
                if (classid == "maintain")
                {
                    str = "关于维修";
                }
                if (classid == "invoice")
                {
                    str = "关于发票";
                }
                if (classid == "express")
                {
                    str = "关于快递";
                }
                if (classid == "THHZC")
                {
                    str = "退换货政策";
                }
                return str;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
