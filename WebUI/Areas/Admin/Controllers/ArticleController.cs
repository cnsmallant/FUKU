using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;

namespace WebUI.Areas.Admin.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: /Admin/Article/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// 退换货政策
        /// </summary>
        /// <returns></returns>
        public ActionResult ArticleList(string classid)
        {
            try
            {
                ViewBag.title = ClassStr(classid);
                com_article arts = db.com_article.Where(a => a.com_article_class == classid).SingleOrDefault();
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddArticle(string classid)
        {
            try
            {
                int result = 0;
                var query = db.sys_admin;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = query.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                com_article art = db.com_article.Where(a => a.com_article_class == classid).SingleOrDefault();
                if (art == null)
                {
                    com_article arts = new com_article();
                    arts.com_article_id = Guid.NewGuid().ToString("N");
                    arts.com_article_class = classid;
                    arts.com_article_title = Request.Form["com_article_title"];
                    arts.com_article_content = Server.HtmlEncode(Request.Form["com_article_content"]);
                    arts.com_article_userid = sys_admin.sys_admin_id;
                    arts.com_article_date = DateTime.Now;
                    db.com_article.Add(arts);
                    result = db.SaveChanges();

                }
                else
                {
                    com_article arts = db.com_article.Where(a => a.com_article_class == classid).SingleOrDefault();
                    arts.com_article_title = Request.Form["com_article_title"];
                    arts.com_article_content = Server.HtmlEncode(Request.Form["com_article_content"]);
                    arts.com_article_userid = sys_admin.sys_admin_id;
                    arts.com_article_date = DateTime.Now;
                    result = db.SaveChanges();

                }
                if (result > 0)
                {
                    return RedirectToAction("ArticleList", "Article", new { classid = classid });
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
        /// 类别名称
        /// </summary>
        /// <returns></returns>
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
                    str = "关于退换货";
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
