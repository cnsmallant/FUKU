using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;

namespace WebUI.Areas.Admin.Controllers
{
    [AdminVerification]
    public class ImagesController : Controller
    {
        //
        // GET: /Admin/Images/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {



            return View();
        }

        public ActionResult Create()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["com_img_fk"]) && !string.IsNullOrEmpty(Request["returnurl"]))
                {
                    ViewBag.com_img_fk = Request["com_img_fk"].ToString();
                    ViewBag.returnurl = Request["returnurl"].ToString();
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult AddImages(HttpPostedFileBase com_img_src)
        {
            try
            {
                string fileName = com_img_src.FileName;
                var guid = Guid.NewGuid().ToString("N");
                //转换只取得文件名 去掉路。
                if (fileName.LastIndexOf("\\") > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                }
                var com_img_fk = Request.Form["com_img_fk"];
                var returnurl = Server.UrlDecode(Request.Form["returnurl"]);
                string pathstr = "/file/img/" + com_img_fk + "/";
                string path = Server.MapPath(pathstr);

                string src = string.Empty;
                if (FileHelper.CreateDir(path))
                {
                    src = path + fileName;
                    com_img_src.SaveAs(src);  //保存到相对路径下
                }
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();

                com_img img = new com_img();
                img.com_img_id = guid;
                img.com_img_src = pathstr + fileName;
                img.com_img_fk = com_img_fk;
                img.com_img_adduser = sys_admin.sys_admin_id;
                img.com_img_adddate = DateTime.Now;
                img.com_img_edituser = sys_admin.sys_admin_id;
                img.com_img_editdate = DateTime.Now;
                db.com_img.Add(img);
                db.Configuration.ValidateOnSaveEnabled = false;
                int result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (result > 0)
                {
                    return Redirect(returnurl);
                }
                else
                {
                    string msg = Server.UrlEncode("上传失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult Edit()
        {

            if (!string.IsNullOrEmpty(Request["com_img_fk"]) && !string.IsNullOrEmpty(Request["returnurl"]))
            {
                var com_img_fk = Request["com_img_fk"].ToString();
                ViewBag.com_img_fk = com_img_fk;
                com_img img = db.com_img.Where(i => i.com_img_fk == com_img_fk).SingleOrDefault();
                ViewBag.com_img_srcs = img.com_img_src;

                ViewBag.returnurl = Request["returnurl"].ToString();
            }



            return View();
        }



        public ActionResult EditImages(HttpPostedFileBase com_img_src)
        {

            try
            {
                string fileName = com_img_src.FileName;
                //转换只取得文件名 去掉路。
                if (fileName.LastIndexOf("\\") > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                }
                var com_img_fk = Request.Form["com_img_fk"];
                var returnurl = Server.UrlDecode(Request.Form["returnurl"]);
                com_img imgs = db.com_img.Where(i => i.com_img_fk == com_img_fk).SingleOrDefault();
                string pathstr = "/file/img/" + com_img_fk + "/";
                string path = Server.MapPath(pathstr);
                string delsrc = Server.MapPath(imgs.com_img_src);
                string src = string.Empty;
                if (FileHelper.DelFile(delsrc))
                {
                    src = path + fileName;
                    com_img_src.SaveAs(src);  //保存到相对路径下
                }
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                imgs.com_img_src = pathstr + fileName;
                imgs.com_img_fk = com_img_fk;
                imgs.com_img_edituser = sys_admin.sys_admin_id;
                imgs.com_img_editdate = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                int result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (result > 0)
                {
                    return Redirect(returnurl);
                }
                else
                {
                    string msg = Server.UrlEncode("上传失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
