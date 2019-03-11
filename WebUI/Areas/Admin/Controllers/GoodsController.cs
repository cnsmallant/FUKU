using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using System.IO;
using System.Text;

namespace WebUI.Areas.Admin.Controllers
{
    [AdminVerification]
    public class GoodsController : Controller
    {

        D8MallEntities db = new D8MallEntities();
        //
        // GET: /Admin/Goods/
        [Purview]
        public ActionResult Index()
        {
            return View();
        }
        #region 类别管理
        /// <summary>
        /// 类别管理
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsClass(int pageid = 1)
        {

            var query = db.pro_class;
            var list = query.OrderByDescending(c => c.pro_class_order).ToList();
            IList<pro_class> listclass = new List<pro_class>();

            foreach (var item in list)
            {
                pro_class pro = new pro_class();

                pro.pro_class_id = item.pro_class_id;
                pro.pro_class_order = item.pro_class_order;
                pro.pro_class_name = item.pro_class_name;
                if (item.pro_parent == "0")
                {
                    pro.pro_parent = "-";
                }
                else
                {
                    pro.pro_parent = db.pro_class.Where(c => c.pro_parent == item.pro_class_id).SingleOrDefault().pro_class_name;
                }
                pro.pro_class_page = item.pro_class_page;
                com_img img = db.com_img.Where(i => i.com_img_fk == item.pro_class_id).SingleOrDefault();
                if (img == null)
                {
                    pro.pro_class_img = item.pro_class_img;
                }
                else
                {
                    pro.pro_class_img = img.com_img_src;
                }
                pro.pro_class_status = item.pro_class_status == "0" ? "正常" : "禁用";
                pro.pro_class_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_class_adduser).SingleOrDefault().sys_admin_name;
                pro.pro_class_adddate = item.pro_class_adddate;
                pro.pro_class_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_class_adduser).SingleOrDefault().sys_admin_name;
                pro.pro_class_editdate = item.pro_class_editdate;
                listclass.Add(pro);
            }
            IList<pro_class> mPage = PageCommon.PageList<pro_class>(pageid, 20, listclass.Count, listclass);
            return View("GoodsClass", mPage);
        }
        /// <summary>
        /// 商品类别
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsClassCreate(string pro_class_id)
        {
            try
            {
                if (string.IsNullOrEmpty(pro_class_id))
                {
                    ViewBag.pro_class_id = pro_class_id;
                }

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 执行添加
        /// </summary>
        /// <returns></returns>
        public ActionResult AddGoodsClass()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var guid = Guid.NewGuid().ToString("N");
                pro_class cl = new pro_class();
                cl.pro_class_id = guid;
                cl.pro_class_order = Convert.ToInt32(Request.Form["pro_class_order"].ToString());
                cl.pro_class_name = Request.Form["pro_class_name"];
                if (!string.IsNullOrEmpty(Request.Form["pro_class_id"]))
                {
                    cl.pro_parent = Request.Form["pro_class_id"];
                }
                else
                {
                    cl.pro_parent = guid;
                }
                cl.pro_class_img = "/file/img/default.jpg";
                cl.pro_class_page = Request.Form["pro_class_page"];
                cl.pro_class_status = "0";
                cl.pro_class_adduser = sys_admin.sys_admin_id;
                cl.pro_class_adddate = DateTime.Now;
                cl.pro_class_edituser = sys_admin.sys_admin_id;
                cl.pro_class_editdate = DateTime.Now;
                db.pro_class.Add(cl);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsClass", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("新建类别失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 验证页面名称是否有重复
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RegPage(string pagename)
        {
            try
            {
                var query = db.pro_class;
                pro_class pro = query.Where(c => c.pro_class_page == pagename).SingleOrDefault();
                if (pro == null)
                {
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsClassEdit(string pro_class_id)
        {
            try
            {
                var query = db.pro_class;
                pro_class cl = query.Where(c => c.pro_class_id == pro_class_id).SingleOrDefault();
                ViewBag.pro_class_name = cl.pro_class_name;
                ViewBag.pro_class_order = cl.pro_class_order;
                ViewBag.pro_class_id = cl.pro_class_id;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditGoodsClass()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var guid = Request.Form["pro_class_id"];
                var cl = db.pro_class.Where(c => c.pro_class_id == guid).SingleOrDefault();
                cl.pro_class_id = guid;
                cl.pro_class_order = Convert.ToInt32(Request.Form["pro_class_order"].ToString());
                cl.pro_class_name = Request.Form["pro_class_name"];
                cl.pro_class_edituser = sys_admin.sys_admin_id;
                cl.pro_class_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsClass", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑类别失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsStatus(string pro_class_id, string pro_class_status)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var cl = db.pro_class.Where(c => c.pro_class_id == pro_class_id).SingleOrDefault();
                cl.pro_class_id = pro_class_id;
                cl.pro_class_status = pro_class_status;
                cl.pro_class_edituser = sys_admin.sys_admin_id;
                cl.pro_class_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsClass", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑类别失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="pro_class_id"></param>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsUpload(string pro_class_id)
        {
            try
            {
                ViewBag.pro_class_id = pro_class_id;
                com_img imgs = db.com_img.Where(i => i.com_img_fk == pro_class_id).SingleOrDefault();
                if (imgs != null)
                {
                    ViewBag.com_img_title = imgs.com_img_title;
                    ViewBag.com_img_alt = imgs.com_img_alt;
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }




        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="com_img_src"></param>
        /// <returns></returns>
        public ActionResult UploadGoods(HttpPostedFileBase com_img_src)
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
                string pro_class_id = Request.Form["pro_class_id"];
                com_img imgs = db.com_img.Where(i => i.com_img_fk == pro_class_id).SingleOrDefault();
                if (imgs == null)
                {
                    pro_class cl = db.pro_class.Where(c => c.pro_class_id == pro_class_id).SingleOrDefault();
                    string pathstr = "/file/img/GoodsClass/" + cl.pro_class_page + "/";
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
                    img.com_img_title = Request.Form["com_img_title"];
                    img.com_img_alt = Request.Form["com_img_alt"];
                    img.com_img_src = pathstr + fileName;
                    img.com_img_fk = pro_class_id;
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
                        return RedirectToAction("GoodsClass", "Goods");
                    }
                    else
                    {
                        string msg = Server.UrlEncode("上传失败！");
                        return RedirectToAction("Index", "Message", new { mid = msg });
                    }
                }
                else
                {
                    pro_class cl = db.pro_class.Where(c => c.pro_class_id == pro_class_id).SingleOrDefault();
                    string pathstr = "/file/img/GoodsClass/" + cl.pro_class_page + "/";
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

                    com_img img = db.com_img.Where(i => i.com_img_id == imgs.com_img_id).SingleOrDefault();
                    img.com_img_id = imgs.com_img_id;
                    img.com_img_title = Request.Form["com_img_title"];
                    img.com_img_alt = Request.Form["com_img_alt"];
                    img.com_img_src = pathstr + fileName;
                    img.com_img_fk = pro_class_id;
                    img.com_img_edituser = sys_admin.sys_admin_id;
                    img.com_img_editdate = DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    int result = db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                    if (result > 0)
                    {
                        return RedirectToAction("GoodsClass", "Goods");
                    }
                    else
                    {
                        string msg = Server.UrlEncode("上传失败！");
                        return RedirectToAction("Index", "Message", new { mid = msg });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 读取父级类别
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetGoodsClass()
        {
            try
            {
                var query = db.pro_class.Where(c => c.pro_parent == "0").OrderByDescending(c => c.pro_class_order).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 按照父级获取数据
        /// </summary>
        /// <param name="pro_parent"></param>
        /// <param name="pageid"></param>
        /// <returns></returns>
        public ActionResult GetGoods(string pro_parent, int pageid = 1)
        {
            try
            {
                var query = db.pro_class;
                var list = query.Where(c => c.pro_parent == pro_parent).OrderByDescending(c => c.pro_class_order).ToList();
                IList<pro_class> listclass = new List<pro_class>();

                foreach (var item in list)
                {
                    pro_class pro = new pro_class();

                    pro.pro_class_id = item.pro_class_id;
                    pro.pro_class_order = item.pro_class_order;
                    pro.pro_class_name = item.pro_class_name;
                    if (item.pro_parent == "0")
                    {
                        pro.pro_parent = "-";
                    }
                    else
                    {
                        pro.pro_parent = db.pro_class.Where(c => c.pro_parent == item.pro_class_id).SingleOrDefault().pro_class_name;
                    }
                    pro.pro_class_page = item.pro_class_page;
                    com_img img = db.com_img.Where(i => i.com_img_fk == item.pro_class_id).SingleOrDefault();
                    if (img == null)
                    {
                        pro.pro_class_img = item.pro_class_img;
                    }
                    else
                    {
                        pro.pro_class_img = img.com_img_src;
                    }
                    pro.pro_class_status = item.pro_class_status == "0" ? "正常" : "禁用";
                    pro.pro_class_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_class_adduser).SingleOrDefault().sys_admin_name;
                    pro.pro_class_adddate = item.pro_class_adddate;
                    pro.pro_class_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_class_adduser).SingleOrDefault().sys_admin_name;
                    pro.pro_class_editdate = item.pro_class_editdate;
                    listclass.Add(pro);
                }
                IList<pro_class> mPage = PageCommon.PageList<pro_class>(pageid, 20, listclass.Count, listclass);
                return View("GoodsClass", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 关键字查询
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="pageid"></param>
        /// <returns></returns>
        public ActionResult GoodsKeyword(string keywords, int pageid = 1)
        {
            try
            {
                var query = db.pro_class;
                var list = query.Where(c => c.pro_class_name.Contains(keywords)).OrderByDescending(c => c.pro_class_order).ToList();
                IList<pro_class> listclass = new List<pro_class>();

                foreach (var item in list)
                {
                    pro_class pro = new pro_class();

                    pro.pro_class_id = item.pro_class_id;
                    pro.pro_class_order = item.pro_class_order;
                    pro.pro_class_name = item.pro_class_name;
                    if (item.pro_parent == "0")
                    {
                        pro.pro_parent = "-";
                    }
                    else
                    {
                        pro.pro_parent = db.pro_class.Where(c => c.pro_parent == item.pro_class_id).SingleOrDefault().pro_class_name;
                    }
                    pro.pro_class_page = item.pro_class_page;
                    com_img img = db.com_img.Where(i => i.com_img_fk == item.pro_class_id).SingleOrDefault();
                    if (img == null)
                    {
                        pro.pro_class_img = item.pro_class_img;
                    }
                    else
                    {
                        pro.pro_class_img = img.com_img_src;
                    }
                    pro.pro_class_status = item.pro_class_status == "0" ? "正常" : "禁用";
                    pro.pro_class_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_class_adduser).SingleOrDefault().sys_admin_name;
                    pro.pro_class_adddate = item.pro_class_adddate;
                    pro.pro_class_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_class_adduser).SingleOrDefault().sys_admin_name;
                    pro.pro_class_editdate = item.pro_class_editdate;
                    listclass.Add(pro);
                }
                IList<pro_class> mPage = PageCommon.PageList<pro_class>(pageid, 20, listclass.Count, listclass);
                return View("GoodsClass", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 规格管理
        /// <summary>
        /// 规格读取
        /// </summary>
        /// <param name="pageid"></param>
        /// <returns></returns>
        [Purview]

        public ActionResult Spe(int pageid = 1)
        {
            try
            {

                var query = db.pro_spe;
                var list = query.OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> listclass = new List<pro_spe>();
                foreach (var item in list)
                {
                    pro_spe spe = new pro_spe();
                    spe.pro_spe_id = item.pro_spe_id;
                    spe.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    if (item.pro_spe_parent == "0")
                    {
                        spe.pro_spe_parent = "-";
                    }
                    else
                    {
                        spe.pro_spe_parent = db.pro_spe.Where(s => s.pro_spe_id == item.pro_spe_parent).SingleOrDefault().pro_spe_name;
                    }

                    spe.pro_spe_name = item.pro_spe_name;
                    spe.pro_spe_status = item.pro_spe_status == "0" ? "正常" : "禁用";
                    spe.pro_spe_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_adduser).SingleOrDefault().sys_admin_name;
                    spe.pro_spe_adddate = item.pro_spe_adddate;
                    spe.pro_spe_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_edituser).SingleOrDefault().sys_admin_name; ;
                    spe.pro_spe_editdate = item.pro_spe_editdate;
                    spe.pro_spe_order = item.pro_spe_order;
                    spe.pro_spe_page = item.pro_spe_page;
                    listclass.Add(spe);
                }
                IList<pro_spe> mPage = PageCommon.PageList<pro_spe>(pageid, 20, listclass.Count, listclass);
                return View("Spe", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public JsonResult GetRegSpePage(string pagename)
        {
            try
            {
                var query = db.pro_spe;
                var pro = query.Where(s => s.pro_spe_page == pagename).SingleOrDefault();
                if (pro == null)
                {
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult SpeCreate()
        {
            try
            {
                ViewData["pro_class_id"] = from a in db.pro_class.Where(c => c.pro_parent == "0").OrderByDescending(c => c.pro_class_order).ToList()
                                           select new SelectListItem
                                           {
                                               Text = a.pro_class_name,
                                               Value = a.pro_class_id
                                           };

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 父级规格
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSpe()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var guid = Guid.NewGuid().ToString("N");
                pro_spe spe = new pro_spe();
                spe.pro_spe_id = guid;
                spe.pro_class_id = Request.Form["pro_class_name"];
                spe.pro_spe_parent = "0";
                spe.pro_subclass_id = "";
                spe.pro_spe_name = Request.Form["pro_spe_name"];
                spe.pro_spe_status = "0";
                spe.pro_spe_adduser = sys_admin.sys_admin_id;
                spe.pro_spe_adddate = DateTime.Now;
                spe.pro_spe_edituser = sys_admin.sys_admin_id;
                spe.pro_spe_editdate = DateTime.Now;
                spe.pro_spe_order = Convert.ToInt32(Request.Form["pro_spe_order"].ToString());
                spe.pro_spe_page = Request.Form["pro_spe_page"];
                db.pro_spe.Add(spe);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Spe", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("新建规格失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult SubSpeCreate(string pro_spe_id)
        {
            try
            {
                ViewBag.pro_spe_id = pro_spe_id;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 子级规格
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSubSpe()
        {
            try
            {

                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var guid = Guid.NewGuid().ToString("N");
                var pro_spe_id = Request.Form["pro_spe_id"];
                pro_spe spe = new pro_spe();
                spe.pro_spe_id = guid;
                spe.pro_class_id = db.pro_spe.Where(s => s.pro_spe_id == pro_spe_id).SingleOrDefault().pro_class_id;
                spe.pro_spe_parent = Request.Form["pro_spe_id"];
                spe.pro_subclass_id = "";
                spe.pro_spe_name = Request.Form["pro_spe_name"];
                spe.pro_spe_status = "0";
                spe.pro_spe_adduser = sys_admin.sys_admin_id;
                spe.pro_spe_adddate = DateTime.Now;
                spe.pro_spe_edituser = sys_admin.sys_admin_id;
                spe.pro_spe_editdate = DateTime.Now;
                spe.pro_spe_order = Convert.ToInt32(Request.Form["pro_spe_order"].ToString());
                spe.pro_spe_page = Request.Form["pro_spe_page"];
                db.pro_spe.Add(spe);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Spe", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("新建子级规格失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        [Purview]
        public ActionResult SpeEdit(string pro_spe_id)
        {
            try
            {
                pro_spe spe = db.pro_spe.Where(s => s.pro_spe_id == pro_spe_id).SingleOrDefault();
                var cl = db.pro_class.Where(c => c.pro_parent == "0").OrderByDescending(c => c.pro_class_order).ToList();
                SelectList selList = new SelectList(cl, "pro_class_id", "pro_class_name", spe.pro_class_id);
                ViewData["pro_class_id"] = selList;
                ViewBag.pro_spe_name = spe.pro_spe_name;

                ViewBag.pro_spe_order = spe.pro_spe_order;
                if (spe.pro_spe_id == spe.pro_spe_parent)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("SubSpeEdit", "Goods", new { pro_spe_id = pro_spe_id });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult EditSpe()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var pro_spe_id = Request.Form["pro_spe_id"];
                pro_spe spe = db.pro_spe.Where(s => s.pro_spe_id == pro_spe_id).SingleOrDefault();
                if (!string.IsNullOrEmpty(Request.Form["pro_class_name"]))
                {
                    spe.pro_class_id = Request.Form["pro_class_name"];
                    var sp = db.pro_spe.Where(s => s.pro_spe_parent == pro_spe_id).ToList();
                    foreach (var item in sp)
                    {
                        item.pro_class_id = Request.Form["pro_class_name"];

                    }



                }
                spe.pro_spe_name = Request.Form["pro_spe_name"];
                spe.pro_spe_order = Convert.ToInt32(Request.Form["pro_spe_order"].ToString());
                spe.pro_spe_id = pro_spe_id;
                spe.pro_spe_parent = "0";
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Spe", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑规格失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [Purview]
        public ActionResult SubSpeEdit(string pro_spe_id)
        {
            try
            {
                pro_spe spe = db.pro_spe.Where(s => s.pro_spe_id == pro_spe_id).SingleOrDefault();


                ViewBag.pro_spe_name = spe.pro_spe_name;

                ViewBag.pro_spe_order = spe.pro_spe_order;

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult EditSubSpe()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var pro_spe_id = Request.Form["pro_spe_id"];
                pro_spe spe = db.pro_spe.Where(s => s.pro_spe_id == pro_spe_id).SingleOrDefault();
                spe.pro_spe_name = Request.Form["pro_spe_name"];
                spe.pro_spe_order = Convert.ToInt32(Request.Form["pro_spe_order"].ToString());
                spe.pro_spe_id = pro_spe_id;
                var sp = db.pro_spe.Where(s => s.pro_spe_id == spe.pro_spe_parent).SingleOrDefault();
                spe.pro_class_id = sp.pro_class_id;

                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Spe", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑子级规格失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public JsonResult GetSpeClass()
        {
            try
            {
                var query = db.pro_spe.Where(s => s.pro_spe_parent == "0").OrderByDescending(s => s.pro_spe_order).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GetSpe(string pro_class_id, int pageid = 1)
        {
            try
            {
                var query = db.pro_spe;
                var list = query.Where(s => s.pro_class_id == pro_class_id).OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> listclass = new List<pro_spe>();
                foreach (var item in list)
                {
                    pro_spe spe = new pro_spe();
                    spe.pro_spe_id = item.pro_spe_id;
                    spe.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    if (item.pro_spe_parent == "0")
                    {
                        spe.pro_spe_parent = "-";
                    }
                    else
                    {
                        spe.pro_spe_parent = db.pro_spe.Where(s => s.pro_spe_id == item.pro_spe_parent).SingleOrDefault().pro_spe_name;
                    }
                    spe.pro_spe_name = item.pro_spe_name;
                    spe.pro_spe_status = item.pro_spe_status == "0" ? "正常" : "禁用";
                    spe.pro_spe_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_adduser).SingleOrDefault().sys_admin_name;
                    spe.pro_spe_adddate = item.pro_spe_adddate;
                    spe.pro_spe_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_edituser).SingleOrDefault().sys_admin_name; ;
                    spe.pro_spe_editdate = item.pro_spe_editdate;
                    spe.pro_spe_order = item.pro_spe_order;
                    spe.pro_spe_page = item.pro_spe_page;
                    listclass.Add(spe);
                }
                IList<pro_spe> mPage = PageCommon.PageList<pro_spe>(pageid, 20, listclass.Count, listclass);
                return View("Spe", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult GetSpes(string pro_spe_id, int pageid = 1)
        {
            try
            {
                var query = db.pro_spe;
                var list = query.Where(s => s.pro_spe_parent == pro_spe_id).OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> listclass = new List<pro_spe>();
                foreach (var item in list)
                {
                    pro_spe spe = new pro_spe();
                    spe.pro_spe_id = item.pro_spe_id;
                    spe.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    if (item.pro_spe_parent == "0")
                    {
                        spe.pro_spe_parent = "-";
                    }
                    else
                    {
                        spe.pro_spe_parent = db.pro_spe.Where(s => s.pro_spe_id == item.pro_spe_parent).SingleOrDefault().pro_spe_name;
                    }
                    spe.pro_spe_name = item.pro_spe_name;
                    spe.pro_spe_status = item.pro_spe_status == "0" ? "正常" : "禁用";
                    spe.pro_spe_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_adduser).SingleOrDefault().sys_admin_name;
                    spe.pro_spe_adddate = item.pro_spe_adddate;
                    spe.pro_spe_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_edituser).SingleOrDefault().sys_admin_name; ;
                    spe.pro_spe_editdate = item.pro_spe_editdate;
                    spe.pro_spe_order = item.pro_spe_order;
                    spe.pro_spe_page = item.pro_spe_page;
                    listclass.Add(spe);
                }
                IList<pro_spe> mPage = PageCommon.PageList<pro_spe>(pageid, 20, listclass.Count, listclass);
                return View("Spe", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult SpeKeyword(string keywords, int pageid = 1)
        {
            try
            {
                var query = db.pro_spe;
                var list = query.Where(s => s.pro_spe_name.Contains(keywords)).OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> listclass = new List<pro_spe>();
                foreach (var item in list)
                {
                    pro_spe spe = new pro_spe();
                    spe.pro_spe_id = item.pro_spe_id;
                    spe.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    if (item.pro_spe_parent == "0")
                    {
                        spe.pro_spe_parent = "-";
                    }
                    else
                    {
                        spe.pro_spe_parent = db.pro_spe.Where(s => s.pro_spe_id == item.pro_spe_parent).SingleOrDefault().pro_spe_name;
                    }
                    spe.pro_spe_name = item.pro_spe_name;
                    spe.pro_spe_status = item.pro_spe_status == "0" ? "正常" : "禁用";
                    spe.pro_spe_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_adduser).SingleOrDefault().sys_admin_name;
                    spe.pro_spe_adddate = item.pro_spe_adddate;
                    spe.pro_spe_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_spe_edituser).SingleOrDefault().sys_admin_name; ;
                    spe.pro_spe_editdate = item.pro_spe_editdate;
                    spe.pro_spe_order = item.pro_spe_order;
                    spe.pro_spe_page = item.pro_spe_page;
                    listclass.Add(spe);
                }
                IList<pro_spe> mPage = PageCommon.PageList<pro_spe>(pageid, 20, listclass.Count, listclass);
                return View("Spe", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Purview]
        public ActionResult SpeStatus(string pro_spe_id, string pro_spe_status)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var cl = db.pro_spe.Where(c => c.pro_spe_id == pro_spe_id).SingleOrDefault();
                cl.pro_spe_id = pro_spe_id;
                cl.pro_spe_status = pro_spe_status;
                cl.pro_spe_edituser = sys_admin.sys_admin_id;
                cl.pro_spe_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Spe", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion

        #region 商品管理
        public class pro_skus : pro_sku
        {
            public decimal? pro_skuitem_price { get; set; }
            public decimal? pro_skuitem_mprice { get; set; }
            public int? pro_skuitem_stock { get; set; }
            public string pro_brand_id { get; set; }
            public string date { get; set; }

            public string com_img_src { get; set; }

            public string pro_newsgoods_id { get; set; }
        }
        [Purview]
        public ActionResult GoodsList(int pageid = 1)
        {
            try
            {


                var query = db.pro_sku;
                var list = query.OrderByDescending(s => s.pro_sku_order).ToList();
                IList<pro_skus> listsku = new List<pro_skus>();
                foreach (var item in list)
                {
                    pro_skus sku = new pro_skus();
                    sku.pro_sku_id = item.pro_sku_id;
                    sku.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    sku.pro_sku_title = item.pro_sku_title;
                    sku.pro_sku_status = item.pro_sku_status == "0" ? "已上架" : "已下架";
                    if (string.IsNullOrEmpty(item.pro_sku_covimg))
                    {
                        sku.pro_sku_covimg = "/file/img/default.jpg";
                    }
                    else
                    {
                        sku.pro_sku_covimg = item.pro_sku_covimg;
                    }

                    sku.pro_brand_id = db.pro_item.Where(p => p.pro_item_id == item.pro_item_id).SingleOrDefault().pro_brand_id;
                    sku.pro_sku_code = item.pro_sku_code;
                    var pro_skuitem = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault();
                    if (pro_skuitem == null)
                    {
                        sku.pro_skuitem_mprice = 0;
                        sku.pro_skuitem_price = 0;
                        sku.pro_skuitem_stock = 0;
                    }
                    else
                    {
                        sku.pro_skuitem_mprice = pro_skuitem.pro_skuitem_mprice;
                        sku.pro_skuitem_price = pro_skuitem.pro_skuitem_price;
                        sku.pro_skuitem_stock = pro_skuitem.pro_skuitem_stock;
                    }


                    sku.pro_sku_arrspe = item.pro_sku_arrspe;
                    sku.pro_sku_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_sku_adduser).SingleOrDefault().sys_admin_name;
                    sku.pro_sku_adddate = item.pro_sku_adddate;
                    sku.pro_sku_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_sku_edituser).SingleOrDefault().sys_admin_name; ;
                    sku.pro_sku_editdate = item.pro_sku_editdate;
                    sku.pro_sku_order = item.pro_sku_order;

                    listsku.Add(sku);
                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageid, 20, listsku.Count, listsku);
                return View("GoodsList", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GoodsListClass(string pro_class_id, int pageid = 1)
        {
            try
            {


                var query = db.pro_sku;
                var list = query.Where(s => s.pro_class_id == pro_class_id).OrderByDescending(s => s.pro_sku_order).ToList();
                IList<pro_skus> listsku = new List<pro_skus>();
                foreach (var item in list)
                {
                    pro_skus sku = new pro_skus();
                    sku.pro_sku_id = item.pro_sku_id;
                    sku.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    sku.pro_sku_title = item.pro_sku_title;
                    sku.pro_sku_status = item.pro_sku_status == "0" ? "已上架" : "已下架";
                    sku.pro_sku_covimg = item.pro_sku_covimg;
                    sku.pro_brand_id = db.pro_item.Where(p => p.pro_item_id == item.pro_item_id).SingleOrDefault().pro_brand_id;
                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_skuitem_mprice = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_mprice;
                    sku.pro_skuitem_price = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_price;
                    sku.pro_skuitem_stock = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_stock;
                    sku.pro_sku_arrspe = item.pro_sku_arrspe;
                    sku.pro_sku_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_sku_adduser).SingleOrDefault().sys_admin_name;
                    sku.pro_sku_adddate = item.pro_sku_adddate;
                    sku.pro_sku_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_sku_edituser).SingleOrDefault().sys_admin_name; ;
                    sku.pro_sku_editdate = item.pro_sku_editdate;
                    sku.pro_sku_order = item.pro_sku_order;

                    listsku.Add(sku);
                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageid, 20, listsku.Count, listsku);
                return View("GoodsList", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult GoodsListKeys(string keywords, int pageid = 1)
        {
            try
            {


                var query = db.pro_sku;
                var list = query.Where(s => s.pro_sku_title.Contains(keywords)).OrderByDescending(s => s.pro_sku_order).ToList();
                IList<pro_skus> listsku = new List<pro_skus>();
                foreach (var item in list)
                {
                    pro_skus sku = new pro_skus();
                    sku.pro_sku_id = item.pro_sku_id;
                    sku.pro_class_id = db.pro_class.Where(c => c.pro_class_id == item.pro_class_id).SingleOrDefault().pro_class_name;
                    sku.pro_sku_title = item.pro_sku_title;
                    sku.pro_sku_status = item.pro_sku_status == "0" ? "已上架" : "已下架";
                    sku.pro_sku_covimg = item.pro_sku_covimg;
                    sku.pro_brand_id = db.pro_item.Where(p => p.pro_item_id == item.pro_item_id).SingleOrDefault().pro_brand_id;
                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_skuitem_mprice = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_mprice;
                    sku.pro_skuitem_price = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_price;
                    sku.pro_skuitem_stock = db.pro_skuitem.Where(s => s.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_stock;
                    sku.pro_sku_arrspe = item.pro_sku_arrspe;
                    sku.pro_sku_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_sku_adduser).SingleOrDefault().sys_admin_name;
                    sku.pro_sku_adddate = item.pro_sku_adddate;
                    sku.pro_sku_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_sku_edituser).SingleOrDefault().sys_admin_name; ;
                    sku.pro_sku_editdate = item.pro_sku_editdate;
                    sku.pro_sku_order = item.pro_sku_order;

                    listsku.Add(sku);
                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageid, 20, listsku.Count, listsku);
                return View("GoodsList", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Purview]
        public ActionResult GoodsCreate()
        {
            try
            {
                ViewBag.list = db.pro_class.Where(c => c.pro_parent == "0").OrderByDescending(c => c.pro_class_order).ToList();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult GoodsFirstStepCreate(string pro_class_id)
        {
            try
            {
                var spelist = db.pro_spe.Where(s => s.pro_class_id == pro_class_id & s.pro_spe_parent == "0").OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> spesublist = new List<pro_spe>();
                string htmlstr = string.Empty;
                foreach (var item in spelist)
                {
                    StringBuilder sb = new StringBuilder();
                    spesublist = db.pro_spe.Where(s => s.pro_spe_parent == item.pro_spe_id).OrderByDescending(s => s.pro_spe_order).ToList();
                    sb.Append("<tr>");
                    sb.Append("<td width='100' align='right'>" + item.pro_spe_name + "：</td>");
                    sb.Append(" <td width='700'>");
                    foreach (var itemsub in spesublist)
                    {
                        sb.Append("<label><input name='" + item.pro_spe_id + "' type='checkbox' value='" + itemsub.pro_spe_name + "' />" + itemsub.pro_spe_name + " </label>");
                    }

                    sb.Append("</td>");
                    sb.Append("</tr>");
                    htmlstr += sb.ToString();
                }
                ViewBag.pro_class_id = pro_class_id;
                ViewBag.htmlstr = htmlstr;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddGoodsFirstStep()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                pro_item item = new pro_item();
                var pro_spes = Request.Form["pro_spes"].ToString().TrimEnd(',');
                item.pro_item_id = Guid.NewGuid().ToString("N");
                item.pro_class_id = Request.Form["pro_class_id"];
                item.pro_item_title = Request.Form["pro_item_title"];
                item.pro_item_subtitle = Request.Form["pro_item_subtitle"];
                item.pro_brand_id = Request.Form["pro_brand_id"];
                item.pro_item_content = Server.HtmlEncode(Request.Form["pro_item_content"]);
                item.pro_item_adduser = sys_admin.sys_admin_id;
                item.pro_item_adddate = DateTime.Now;
                item.pro_item_edituser = sys_admin.sys_admin_id;
                item.pro_item_editdate = DateTime.Now;
                item.pro_item_status = 0;
                db.pro_item.Add(item);

                pro_sku sku = new pro_sku();
                sku.pro_sku_id = Guid.NewGuid().ToString("N");
                sku.pro_item_id = item.pro_item_id;
                sku.pro_sku_code = DateTime.Now.ToFileTime().ToString();
                sku.pro_class_id = Request.Form["pro_class_id"];
                sku.pro_sku_keywords = pro_spes;
                sku.pro_sku_description = pro_spes;
                sku.pro_sku_arrspe = pro_spes;
                sku.pro_sku_order = Convert.ToInt32(Request.Form["pro_sku_order"].ToString());
                sku.pro_sku_title = Request.Form["pro_item_title"];
                sku.pro_sku_status = "0";
                sku.pro_sku_adduser = sys_admin.sys_admin_id;
                sku.pro_sku_adddate = DateTime.Now;
                sku.pro_sku_edituser = sys_admin.sys_admin_id;
                sku.pro_sku_editdate = DateTime.Now;
                db.pro_sku.Add(sku);


                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsSecondStepCreate", "Goods", new { pro_class_id = item.pro_class_id, pro_sku_id = sku.pro_sku_id, pro_item_id = item.pro_item_id });
                }
                else
                {
                    string msg = Server.UrlEncode("第一步生成失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }


            }
            catch (Exception)
            {

                throw;
            }
        }

        [Purview]
        public ActionResult GoodsSecondStepCreate()
        {
            try
            {
                ViewBag.pro_class_id = Request["pro_class_id"];
                ViewBag.pro_sku_id = Request["pro_sku_id"]; ;
                ViewBag.pro_item_id = Request["pro_item_id "];
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult AddGoodsSecondStep(HttpPostedFileBase com_img_src)
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                string fileName = com_img_src.FileName;
                //转换只取得文件名 去掉路。
                if (fileName.LastIndexOf("\\") > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                }
                string pro_class_id = Request.Form["pro_class_id"];

                pro_class cl = db.pro_class.Where(c => c.pro_class_id == pro_class_id).SingleOrDefault();
                string pathstr = "/file/img/Goods/" + cl.pro_class_page + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "/";
                string path = Server.MapPath(pathstr);

                string src = string.Empty;
                if (FileHelper.CreateDir(path))
                {
                    src = path + fileName;
                    com_img_src.SaveAs(src);  //保存到相对路径下
                }
                var pro_sku_id = Request.Form["pro_sku_id"];
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                sku.pro_sku_id = pro_sku_id;
                sku.pro_sku_covimg = pathstr + fileName;
                sku.pro_sku_edituser = sys_admin.sys_admin_id;
                sku.pro_sku_editdate = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                int res = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (res > 0)
                {
                    pro_skuitem ski = new pro_skuitem();
                    ski.pro_skuitem_id = Guid.NewGuid().ToString("N");
                    ski.pro_sku_id = Request.Form["pro_sku_id"];
                    ski.pro_class_id = Request.Form["pro_class_id"];
                    ski.pro_item_id = Request.Form["pro_item_id"];
                    ski.pro_skuitem_mprice = Convert.ToDecimal(Request.Form["pro_skuitem_mprice"].ToString());
                    ski.pro_skuitem_price = Convert.ToDecimal(Request.Form["pro_skuitem_price"].ToString());
                    ski.pro_skuitem_stock = Convert.ToInt32(Request["pro_skuitem_stock"].ToString());
                    ski.pro_skuitem_adduser = sys_admin.sys_admin_id;
                    ski.pro_skuitem_adddate = DateTime.Now;
                    ski.pro_skuitem_edituser = sys_admin.sys_admin_id;
                    ski.pro_skuitem_editdate = DateTime.Now;
                    db.pro_skuitem.Add(ski);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    int result = db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                    if (result > 0)
                    {
                        return RedirectToAction("GoodsUploadImage", "Goods", new { pro_sku_id = pro_sku_id });
                    }
                    else
                    {
                        string msg = Server.UrlEncode("第二步生成失败！");
                        return RedirectToAction("Index", "Message", new { mid = msg });
                    }
                }
                else
                {
                    string msg = Server.UrlEncode("第二步生成失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 图片列表
        /// </summary>
        /// <param name="pro_sku_id"></param>
        /// <param name="pageid"></param>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsImageList(string pro_sku_id, int pageid = 1)
        {
            try
            {
                var query = db.com_img;
                var list = query.Where(c => c.com_img_fk == pro_sku_id).ToList();
                IList<com_img> listimg = new List<com_img>();
                foreach (var item in list)
                {
                    com_img img = new com_img();
                    img.com_img_id = item.com_img_id;
                    img.com_img_title = item.com_img_title;
                    img.com_img_alt = item.com_img_alt;
                    img.com_img_fk = db.pro_sku.Where(s => s.pro_sku_id == item.com_img_fk).SingleOrDefault().pro_sku_title;
                    img.com_img_src = item.com_img_src;
                    img.com_img_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.com_img_adduser).SingleOrDefault().sys_admin_name;
                    img.com_img_adddate = item.com_img_adddate;
                    img.com_img_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.com_img_edituser).SingleOrDefault().sys_admin_name;
                    img.com_img_editdate = item.com_img_editdate;

                    listimg.Add(img);
                }
                IList<com_img> mPage = PageCommon.PageList<com_img>(pageid, 20, listimg.Count, listimg);
                return View("GoodsImageList", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult GoodsUploadImage(string pro_sku_id)
        {
            try
            {
                ViewBag.pro_sku_id = pro_sku_id;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="com_img_src"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadGoodsImage(HttpPostedFileBase com_img_src)
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
                string pro_sku_id = Request.Form["pro_sku_id"];
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                string pathstr = "/file/img/Goods/" + sku.pro_sku_code + "/";
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
                img.com_img_title = Request.Form["com_img_title"];
                img.com_img_alt = Request.Form["com_img_alt"];
                img.com_img_src = pathstr + fileName;
                img.com_img_fk = pro_sku_id;
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
                    return RedirectToAction("GoodsImageList", "Goods", new { pro_sku_id = pro_sku_id });
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

        #region 修改
        [Purview]
        public ActionResult GoodsShelf(string pro_sku_id, string pro_sku_status)
        {
            try
            {
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                sku.pro_sku_status = pro_sku_status;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsList", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("状态操作失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult GoodsSpeEdit(string pro_sku_id)
        {
            try
            {
                ViewBag.pro_sku_id = pro_sku_id;
                var pro_sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                var spelist = db.pro_spe.Where(s => s.pro_class_id == pro_sku.pro_class_id & s.pro_spe_parent == "0").OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> spesublist = new List<pro_spe>();
                string htmlstr = string.Empty;
                foreach (var item in spelist)
                {
                    StringBuilder sb = new StringBuilder();
                    spesublist = db.pro_spe.Where(s => s.pro_spe_parent == item.pro_spe_id).OrderByDescending(s => s.pro_spe_order).ToList();
                    sb.Append("<tr>");
                    sb.Append("<td width='100' align='right'>" + item.pro_spe_name + "：</td>");
                    sb.Append(" <td width='700'>");
                    string[] str = StringPlusCommon.GetStrArray(pro_sku.pro_sku_arrspe, ',');
                    foreach (var itemsub in spesublist)
                    {
                        sb.Append("<label>");
                        sb.Append("<input name='" + item.pro_spe_id + "' type='checkbox'");
                        sb.Append("value='" + itemsub.pro_spe_name + "'");
                        for (int i = 0; i < str.Length; i++)
                        {


                            if (str[i] == itemsub.pro_spe_name)
                            {
                                sb.Append(" checked='checked'");
                            }


                        }

                        sb.Append("/>" + itemsub.pro_spe_name + " </label>");
                    }

                    sb.Append("</td>");
                    sb.Append("</tr>");
                    htmlstr += sb.ToString();
                }

                ViewBag.htmlstr = htmlstr;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult EditGoodsSpe()
        {
            try
            {
                var pro_sku_id = Request.Form["pro_sku_id"];
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                var arrspe = Request.Form["pro_spes"].ToString().TrimEnd(',');
                sku.pro_sku_arrspe = arrspe;
                sku.pro_sku_description = arrspe;
                sku.pro_sku_keywords = arrspe;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsList", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("状态操作失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult GoodsClassSpeEdit(string pro_sku_id)
        {
            try
            {

                ViewBag.list = db.pro_class.Where(c => c.pro_parent == "0").OrderByDescending(c => c.pro_class_order).ToList();
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GoodsFirstStepEdit(string pro_sku_id, string pro_class_id)
        {
            try
            {
                ViewBag.pro_sku_id = pro_sku_id;
                ViewBag.pro_class_id = pro_sku_id;
                var spelist = db.pro_spe.Where(s => s.pro_class_id == pro_class_id & s.pro_spe_parent == "0").OrderByDescending(s => s.pro_spe_order).ToList();
                IList<pro_spe> spesublist = new List<pro_spe>();
                string htmlstr = string.Empty;
                foreach (var item in spelist)
                {
                    StringBuilder sb = new StringBuilder();
                    spesublist = db.pro_spe.Where(s => s.pro_spe_parent == item.pro_spe_id).OrderByDescending(s => s.pro_spe_order).ToList();
                    sb.Append("<tr>");
                    sb.Append("<td width='100' align='right'>" + item.pro_spe_name + "：</td>");
                    sb.Append(" <td width='700'>");
                    foreach (var itemsub in spesublist)
                    {
                        sb.Append("<label><input name='" + item.pro_spe_id + "' type='checkbox' value='" + itemsub.pro_spe_name + "' />" + itemsub.pro_spe_name + " </label>");
                    }

                    sb.Append("</td>");
                    sb.Append("</tr>");
                    htmlstr += sb.ToString();
                }

                ViewBag.htmlstr = htmlstr;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult EditGoodsClassSpe()
        {
            try
            {
                var pro_sku_id = Request.Form["pro_sku_id"];
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                var arrspe = Request.Form["pro_spes"].ToString().TrimEnd(',');
                sku.pro_sku_arrspe = arrspe;
                sku.pro_sku_description = arrspe;
                sku.pro_sku_keywords = arrspe;
                sku.pro_class_id = Request.Form["pro_class_id"];
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsList", "Goods");
                }
                else
                {
                    string msg = Server.UrlEncode("状态操作失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 修改详细
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult GoodsEditDetails(string pro_sku_id)
        {
            try
            {
                ViewBag.pro_sku_id = pro_sku_id;
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                pro_item item = db.pro_item.Where(i => i.pro_item_id == sku.pro_item_id).SingleOrDefault();
                pro_skuitem ski = db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault();
                ViewBag.pro_item_title = item.pro_item_title;
                ViewBag.pro_item_subtitle = item.pro_item_subtitle;
                ViewBag.pro_brand_id = item.pro_brand_id;
                ViewBag.pro_item_content = Server.HtmlDecode(item.pro_item_content);
                ViewBag.pro_sku_order = sku.pro_sku_order;
                ViewBag.pro_skuitem_mprice = ski.pro_skuitem_mprice;
                ViewBag.pro_skuitem_price = ski.pro_skuitem_price;
                ViewBag.pro_skuitem_stock = ski.pro_skuitem_stock;

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditGoodsDetails()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var pro_sku_id = Request.Form["pro_sku_id"];
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                sku.pro_sku_title = Request.Form["pro_item_title"];
                sku.pro_sku_order = Convert.ToInt32(Request.Form["pro_sku_order"].ToString());
                sku.pro_sku_edituser = sys_admin.sys_admin_id;
                sku.pro_sku_editdate = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                int sku_result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (sku_result > 0)
                {
                    pro_item item = db.pro_item.Where(i => i.pro_item_id == sku.pro_item_id).SingleOrDefault();
                    item.pro_item_title = Request.Form["pro_item_title"];
                    item.pro_item_subtitle = Request.Form["pro_item_subtitle"];
                    item.pro_item_content = Server.HtmlEncode(Request.Form["pro_item_content"]);
                    item.pro_brand_id = Request.Form["pro_brand_id"];
                    db.Configuration.ValidateOnSaveEnabled = false;
                    int item_result = db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;

                    pro_skuitem ski = db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault();
                    ski.pro_skuitem_mprice = Convert.ToDecimal(Request.Form["pro_skuitem_mprice"]);
                    ski.pro_skuitem_price = Convert.ToDecimal(Request.Form["pro_skuitem_price"]);
                    ski.pro_skuitem_stock = Convert.ToInt32(Request.Form["pro_skuitem_stock"]);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    int ski_result = db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                    return RedirectToAction("GoodsList", "Goods");
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

        [Purview]
        public ActionResult GoodsImgaeEdit(string com_img_id, string pro_sku_id)
        {
            try
            {
                ViewBag.com_img_id = com_img_id;
                ViewBag.pro_sku_id = pro_sku_id;
                ViewBag.com_img_src = db.com_img.Where(c => c.com_img_id == com_img_id).SingleOrDefault().com_img_src;

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult EditGoodsImgae(HttpPostedFileBase com_img_src)
        {
            try
            {
                string fileName = com_img_src.FileName;
                //转换只取得文件名 去掉路。
                if (fileName.LastIndexOf("\\") > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                }
                string pro_sku_id = Request.Form["pro_sku_id"];
                string com_img_id = Request["com_img_id"].ToString();
                com_img img = db.com_img.Where(i => i.com_img_id == com_img_id).SingleOrDefault();
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                string pathstr = "/file/img/Goods/" + sku.pro_sku_code + "/";
                string path = Server.MapPath(pathstr);
                string src = string.Empty;
                string pathsrc = Server.MapPath(img.com_img_src);
                if (FileHelper.DelFile(pathsrc))
                {
                    src = path + fileName;
                    com_img_src.SaveAs(src);  //保存到相对路径下
                }
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                img.com_img_id = com_img_id;
                img.com_img_src = pathstr + fileName;
                img.com_img_edituser = sys_admin.sys_admin_id;
                img.com_img_editdate = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                int result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (result > 0)
                {
                    return RedirectToAction("GoodsImageList", "Goods", new { pro_sku_id = pro_sku_id });
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
        [Purview]
        public ActionResult GoodsImgaeDel(string pro_sku_id, string com_img_id)
        {
            try
            {
                com_img img = db.com_img.Where(i => i.com_img_id == com_img_id).SingleOrDefault();
                string pathsrc = Server.MapPath(img.com_img_src);
                if (FileHelper.DelFile(pathsrc))
                {
                    db.com_img.Remove(img);
                    db.SaveChanges();
                    return RedirectToAction("GoodsImageList", "Goods", new { pro_sku_id = pro_sku_id });
                }
                else
                {
                    string msg = Server.UrlEncode("删除失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [Purview]
        public ActionResult GoodsImgCoverEdit(string pro_sku_id)
        {
            try
            {
                ViewBag.pro_sku_id = pro_sku_id;
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                ViewBag.com_img_src = sku.pro_sku_covimg;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult EditGoodsImgCover(HttpPostedFileBase com_img_src)
        {
            try
            {

                string fileName = com_img_src.FileName;
                //转换只取得文件名 去掉路。
                if (fileName.LastIndexOf("\\") > -1)
                {
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                }
                string pro_sku_id = Request.Form["pro_sku_id"];
                pro_sku sku = db.pro_sku.Where(s => s.pro_sku_id == pro_sku_id).SingleOrDefault();
                pro_class cl = db.pro_class.Where(s => s.pro_class_id == sku.pro_class_id).SingleOrDefault();
                string pathstr = "/file/img/Goods/" + cl.pro_class_page + "/";
                string path = Server.MapPath(pathstr);
                string src = string.Empty;
                string pathsrc = Server.MapPath(sku.pro_sku_covimg);
                if (FileHelper.DelFile(pathsrc))
                {
                    src = path + fileName;
                    com_img_src.SaveAs(src);  //保存到相对路径下
                }
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                sku.pro_sku_covimg = pathstr + fileName;
                sku.pro_sku_edituser = sys_admin.sys_admin_id;
                sku.pro_sku_editdate = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                int result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (result > 0)
                {
                    return RedirectToAction("GoodsList", "Goods");
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
        /// <summary>
        /// 快递公司查询
        /// </summary>
        /// <returns></returns>
        public ActionResult Express()
        {
            try
            {
                var exp = db.pro_express.OrderByDescending(e => e.pro_express_editdate).ToList();

                IList<pro_express> list = new List<pro_express>();
                foreach (var item in exp)
                {
                    pro_express exps = new pro_express();
                    exps.pro_express_id = item.pro_express_id;
                    exps.pro_express_name = item.pro_express_name;
                    exps.pro_express_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_express_adduser).SingleOrDefault().sys_admin_name;
                    exps.pro_express_adddate = item.pro_express_adddate;
                    exps.pro_express_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_express_edituser).SingleOrDefault().sys_admin_name;
                    exps.pro_express_editdate = item.pro_express_editdate;
                    list.Add(exps);
                }
                ViewBag.list = list;
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ExpressCreate()
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


        public ActionResult ExpressEdit(string pro_express_id)
        {
            try
            {
                ViewBag.pro_express_id = pro_express_id;
                pro_express exp = db.pro_express.Where(e => e.pro_express_id == pro_express_id).SingleOrDefault();
                ViewBag.pro_express_name = exp.pro_express_name;
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult AddExpres()
        {
            try
            {
                var pro_express_name = Request.Form["pro_express_name"];
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                pro_express exp = new pro_express();
                exp.pro_express_id = Guid.NewGuid().ToString("N");
                exp.pro_express_name = pro_express_name;
                exp.pro_express_adduser = sys_admin.sys_admin_id;
                exp.pro_express_adddate = DateTime.Now;
                exp.pro_express_edituser = sys_admin.sys_admin_id;
                exp.pro_express_editdate = DateTime.Now;
                db.pro_express.Add(exp);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Express", "Goods");
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

        [HttpPost]
        public ActionResult EditExpres()
        {
            try
            {
                var pro_express_id = Request.Form["pro_express_id"];
                var pro_express_name = Request.Form["pro_express_name"];
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                pro_express exp = db.pro_express.Where(e => e.pro_express_id == pro_express_id).SingleOrDefault();

                exp.pro_express_name = pro_express_name;
                exp.pro_express_edituser = sys_admin.sys_admin_id;
                exp.pro_express_editdate = DateTime.Now;

                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Express", "Goods");
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

        public ActionResult ExpressDel(string pro_express_id)
        {
            try
            {
                var ex = db.pro_express.Where(e => e.pro_express_id == pro_express_id).SingleOrDefault();
                db.pro_express.Remove(ex);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Express", "Goods");
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
        /// 运费
        /// </summary>
        /// <returns></returns>
        public ActionResult Shipment(int pageid = 1)
        {
            try
            {
                var query = db.pro_shipment;
                var list = query.OrderByDescending(s => s.pro_shipment_editdate).ToList();
                IList<pro_shipment> listimg = new List<pro_shipment>();
                foreach (var item in list)
                {
                    pro_shipment sh = new pro_shipment();
                    int provinceid = Convert.ToInt32(item.pro_shipment_province);
                    int cityid = Convert.ToInt32(item.pro_shipment_city);
                    int countyid = Convert.ToInt32(item.pro_shipment_county);
                    var province = db.com_area.Where(a => a.com_area_id == provinceid).SingleOrDefault().com_area_name;
                    var city = db.com_area.Where(a => a.com_area_id == cityid).SingleOrDefault().com_area_name;
                    var county = db.com_area.Where(a => a.com_area_id == countyid).SingleOrDefault().com_area_name;
                    sh.pro_shipment_id = item.pro_shipment_id;
                    sh.pro_shipment_starcity = item.pro_shipment_starcity;
                    sh.pro_shipment_county = province + city + county;
                    sh.pro_shipment_price = item.pro_shipment_price;
                    sh.pro_shipment_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_shipment_adduser).SingleOrDefault().sys_admin_name;
                    sh.pro_shipment_adddate = item.pro_shipment_adddate;
                    sh.pro_shipment_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_shipment_edituser).SingleOrDefault().sys_admin_name;
                    sh.pro_shipment_editdate = item.pro_shipment_editdate;
                    listimg.Add(sh);
                }
                IList<pro_shipment> mPage = PageCommon.PageList<pro_shipment>(pageid, 20, listimg.Count, listimg);
                return View("Shipment", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult ShipmentCity(string cid, int pageid = 1)
        {
            try
            {
                var query = db.pro_shipment;
                var list = query.Where(s => s.pro_shipment_city == cid).OrderByDescending(s => s.pro_shipment_editdate).ToList();
                IList<pro_shipment> listimg = new List<pro_shipment>();
                foreach (var item in list)
                {
                    pro_shipment sh = new pro_shipment();
                    int provinceid = Convert.ToInt32(item.pro_shipment_province);
                    int cityid = Convert.ToInt32(item.pro_shipment_city);
                    int countyid = Convert.ToInt32(item.pro_shipment_county);
                    var province = db.com_area.Where(a => a.com_area_id == provinceid).SingleOrDefault().com_area_name;
                    var city = db.com_area.Where(a => a.com_area_id == cityid).SingleOrDefault().com_area_name;
                    var county = db.com_area.Where(a => a.com_area_id == countyid).SingleOrDefault().com_area_name;
                    sh.pro_shipment_id = item.pro_shipment_id;
                    sh.pro_shipment_starcity = item.pro_shipment_starcity;
                    sh.pro_shipment_county = province + city + county;
                    sh.pro_shipment_price = item.pro_shipment_price;
                    sh.pro_shipment_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_shipment_adduser).SingleOrDefault().sys_admin_name;
                    sh.pro_shipment_adddate = item.pro_shipment_adddate;
                    sh.pro_shipment_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.pro_shipment_edituser).SingleOrDefault().sys_admin_name;
                    sh.pro_shipment_editdate = item.pro_shipment_editdate;
                    listimg.Add(sh);
                }
                IList<pro_shipment> mPage = PageCommon.PageList<pro_shipment>(pageid, 20, listimg.Count, listimg);
                return View("Shipment", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ShipmentCreate()
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

        [HttpPost]
        public ActionResult AddShipment()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                pro_shipment sh = new pro_shipment();
                var stacity = Request.Form["pro_shipment_starcity"];

                sh.pro_shipment_id = Guid.NewGuid().ToString("N");
                sh.pro_shipment_price = Request.Form["pro_shipment_price"];
                if (string.IsNullOrEmpty(stacity))
                {
                    sh.pro_shipment_starcity = "青岛市";
                }
                else
                {
                    sh.pro_shipment_starcity = Request.Form["pro_shipment_starcity"];
                }
                sh.pro_shipment_province = Request.Form["pro_shipment_province"];
                sh.pro_shipment_city = Request.Form["pro_shipment_city"];
                sh.pro_shipment_county = Request.Form["pro_shipment_county"];
                sh.pro_shipment_adduser = sys_admin.sys_admin_id;
                sh.pro_shipment_adddate = DateTime.Now;
                sh.pro_shipment_edituser = sys_admin.sys_admin_id;
                sh.pro_shipment_editdate = DateTime.Now;
                db.pro_shipment.Add(sh);
                db.Configuration.ValidateOnSaveEnabled = false;
                int result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (result > 0)
                {
                    return RedirectToAction("Shipment", "Goods");
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
        /// 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipmentEdit(string pro_shipment_id)
        {
            try
            {
                var sh = db.pro_shipment.Where(s => s.pro_shipment_id == pro_shipment_id).SingleOrDefault();
                var ex = db.pro_express.OrderByDescending(e => e.pro_express_editdate).ToList();
                ViewBag.pro_shipment_starcity = sh.pro_shipment_starcity;
                var Province = db.com_area.Where(c => c.com_area_lev == "1").ToList();
                var City = db.com_area.Where(c => c.com_area_parentid == sh.pro_shipment_province).ToList();
                var County = db.com_area.Where(c => c.com_area_parentid == sh.pro_shipment_city).ToList();
                SelectList provinces = new SelectList(Province, "com_area_id", "com_area_name", sh.pro_shipment_province);
                SelectList citys = new SelectList(City, "com_area_id", "com_area_name", sh.pro_shipment_city);
                SelectList countys = new SelectList(County, "com_area_id", "com_area_name", sh.pro_shipment_county);
                ViewData["province"] = provinces;
                ViewData["city"] = citys;
                ViewData["county"] = countys;
                ViewBag.pro_shipment_price = sh.pro_shipment_price;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult EditShipment()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var pro_shipment_id = Request.Form["pro_shipment_id"];
                pro_shipment sh = db.pro_shipment.Where(s => s.pro_shipment_id == pro_shipment_id).SingleOrDefault();
                var stacity = Request.Form["pro_shipment_starcity"];

                sh.pro_shipment_price = Request.Form["pro_shipment_price"];
                if (string.IsNullOrEmpty(stacity))
                {
                    sh.pro_shipment_starcity = "青岛市";
                }
                else
                {
                    sh.pro_shipment_starcity = Request.Form["pro_shipment_starcity"];
                }

                if (!string.IsNullOrEmpty(Request.Form["pro_shipment_province"]))
                {
                    sh.pro_shipment_province = Request.Form["pro_shipment_province"];
                }
                if (!string.IsNullOrEmpty(Request.Form["pro_shipment_city"]))
                {
                    sh.pro_shipment_city = Request.Form["pro_shipment_city"];
                }
                if (!string.IsNullOrEmpty(Request.Form["pro_shipment_county"]))
                {
                    sh.pro_shipment_county = Request.Form["pro_shipment_county"];
                }

                sh.pro_shipment_edituser = sys_admin.sys_admin_id;
                sh.pro_shipment_editdate = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                int result = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                if (result > 0)
                {
                    return RedirectToAction("Shipment", "Goods");
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
        [HttpGet]
        public JsonResult Cascade(string parentid)
        {
            try
            {
                var result = db.com_area.Where(c => c.com_area_parentid == parentid).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public ActionResult GoodsNews(int pageid = 1)
        {
            try
            {
                IList<pro_skus> listsku = new List<pro_skus>();
                var nglist = db.pro_newsgoods.OrderByDescending(n => n.pro_newsgoods_order).ToList();
                foreach (var item in nglist)
                {
                    pro_skus sku = new pro_skus();
                    pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                    sku.pro_sku_id = item.pro_newsgoods_id.ToString();
                    sku.pro_sku_title = skus.pro_sku_title;
                    sku.pro_sku_status = item.pro_newsgoods_status == "0" ? "已推荐" : "未推荐";
                    sku.pro_sku_covimg = skus.pro_sku_covimg;
                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_skuitem_mprice = db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_mprice;
                    sku.pro_skuitem_price = db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_price;
                    sku.pro_skuitem_stock = db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_stock;
                    sku.pro_sku_adduser = db.sys_admin.Where(a => a.sys_admin_id == skus.pro_sku_adduser).SingleOrDefault().sys_admin_name;
                    sku.pro_sku_adddate = skus.pro_sku_adddate;
                    sku.pro_sku_edituser = db.sys_admin.Where(a => a.sys_admin_id == skus.pro_sku_edituser).SingleOrDefault().sys_admin_name; ;
                    sku.pro_sku_editdate = skus.pro_sku_editdate;
                    sku.pro_sku_order = item.pro_newsgoods_order;
                    var sid = "cx" + item.pro_newsgoods_id.ToString();
                    var com_img_src = db.com_img.Where(c => c.com_img_fk == sid).SingleOrDefault();
                    if (com_img_src != null)
                    {
                        sku.com_img_src = com_img_src.com_img_src;
                    }
                    else
                    {
                        sku.com_img_src = "/file/img/default.jpg";
                    }

                    sku.pro_newsgoods_id = "cx" + item.pro_newsgoods_id;
                    listsku.Add(sku);
                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageid, 20, nglist.Count, listsku);
                return View("GoodsNews", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GoodsNewsAdd(string str)
        {
            try
            {
                string text = Server.UrlDecode(str);
                string[] arrstr = str.Split(',');
                int result = 0;
                foreach (var item in arrstr)
                {
                    if (item != "")
                    {
                        pro_newsgoods ng = new pro_newsgoods();
                        ng.pro_sku_code = item;
                        ng.pro_newsgoods_order = 10;
                        ng.pro_newsgoods_status = "1";
                        db.pro_newsgoods.Add(ng);
                        result = db.SaveChanges();
                    }
                }
                if (result > 0)
                {
                    return RedirectToAction("GoodsNews", "Goods");
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


        public ActionResult GoodsNewsDel(int pro_newsgoods_id)
        {
            try
            {
                pro_newsgoods ng = db.pro_newsgoods.Where(n => n.pro_newsgoods_id == pro_newsgoods_id).SingleOrDefault();
                db.pro_newsgoods.Remove(ng);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsNews", "Goods");
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
        public ActionResult GoodsRecommend(int pro_newsgoods_id, string sta)
        {
            try
            {
                pro_newsgoods ng = db.pro_newsgoods.Where(n => n.pro_newsgoods_id == pro_newsgoods_id).SingleOrDefault();
                ng.pro_newsgoods_status = sta;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsNews", "Goods");
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
        public ActionResult GoodsNewOrd(int pro_newsgoods_id, int ord)
        {
            try
            {
                pro_newsgoods ng = db.pro_newsgoods.Where(n => n.pro_newsgoods_id == pro_newsgoods_id).SingleOrDefault();
                ng.pro_newsgoods_order = ord;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsNews", "Goods");
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
        public ActionResult GoodsSales(int pageid = 1)
        {
            try
            {
                IList<pro_skus> listsku = new List<pro_skus>();
                var salist = db.pro_sales.OrderByDescending(n => n.pro_sales_order).ToList();
                foreach (var item in salist)
                {

                    pro_skus sku = new pro_skus();
                    pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                    sku.pro_sku_id = item.pro_sales_id.ToString();
                    sku.pro_sku_title = skus.pro_sku_title;
                    sku.pro_sku_status = item.pro_sales_stastus == "0" ? "未启用" : "已启用";
                    sku.pro_sku_covimg = skus.pro_sku_covimg;
                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_skuitem_price = item.pro_sales_price;
                    sku.pro_skuitem_stock = db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_stock;
                    sku.pro_sku_order = item.pro_sales_order;
                    sku.date = Convert.ToDateTime(item.pro_sales_stadate.ToString()).ToString("yyyy-MM-dd") + "<=>" + Convert.ToDateTime(item.pro_sales_enddate.ToString()).ToString("yyyy-MM-dd");
                    listsku.Add(sku);

                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageid, 20, salist.Count, listsku);
                return View("GoodsSales", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult GoodsSalesAdd(string str)
        {
            try
            {
                string text = Server.UrlDecode(str);
                string[] arrstr = str.Split(',');
                int result = 0;
                foreach (var item in arrstr)
                {
                    if (item != "")
                    {
                        pro_sales sa = new pro_sales();
                        sa.pro_sku_code = item;
                        sa.pro_sales_stastus = "0";
                        sa.pro_sales_price = 0;
                        sa.pro_sales_order = 10;
                        sa.pro_sales_stadate = DateTime.Now;
                        sa.pro_sales_enddate = DateTime.Now;
                        db.pro_sales.Add(sa);
                        result = db.SaveChanges();
                    }
                }
                if (result > 0)
                {
                    return RedirectToAction("GoodsSales", "Goods");
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


        public ActionResult GoodsIsEnable(int pro_sales_id, string sta)
        {
            try
            {
                pro_sales sa = db.pro_sales.Where(n => n.pro_sales_id == pro_sales_id).SingleOrDefault();
                sa.pro_sales_stastus = sta;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsSales", "Goods");
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

        public ActionResult GoodsSalesDate(int pro_sales_id, string datetxt)
        {
            try
            {
                pro_sales sa = db.pro_sales.Where(n => n.pro_sales_id == pro_sales_id).SingleOrDefault();
                string str = Server.UrlDecode(datetxt);
                string[] strarr = str.Split(',');
                sa.pro_sales_stadate = Convert.ToDateTime(strarr[0].ToString());
                sa.pro_sales_enddate = Convert.ToDateTime(strarr[1].ToString());
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsSales", "Goods");
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

        public ActionResult GoodsSalesPrice(int pro_sales_id, decimal price)
        {
            try
            {
                pro_sales sa = db.pro_sales.Where(n => n.pro_sales_id == pro_sales_id).SingleOrDefault();
                sa.pro_sales_price = price;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsSales", "Goods");
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

        public ActionResult GoodsSalesDel(int pro_sales_id)
        {
            try
            {
                pro_sales ng = db.pro_sales.Where(n => n.pro_sales_id == pro_sales_id).SingleOrDefault();
                db.pro_sales.Remove(ng);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsSales", "Goods");
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

        public ActionResult GoodsCoupon(int pageid = 1)
        {
            try
            {
                var salist = db.pro_coupon.OrderByDescending(n => n.pro_coupon_date).ToList();
                IList<pro_coupon> mPage = PageCommon.PageList<pro_coupon>(pageid, 20, salist.Count, salist);
                return View("GoodsCoupon", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult GoodsCouponClass(string pro_coupon_class, int pageid = 1)
        {
            try
            {
                var salist = db.pro_coupon.Where(c => c.pro_coupon_class == pro_coupon_class).OrderByDescending(n => n.pro_coupon_date).ToList();
                IList<pro_coupon> mPage = PageCommon.PageList<pro_coupon>(pageid, 20, salist.Count, salist);
                return View("GoodsCoupon", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GoodsCouponCreate()
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

        public ActionResult AddGoodsCoupon()
        {
            try
            {
                pro_coupon c = new pro_coupon();
                c.pro_coupon_id = Guid.NewGuid().ToString("N");
                c.pro_coupon_name = Request.Form["pro_coupon_name"];
                c.pro_coupon_class = Request.Form["pro_coupon_class"];
                if (c.pro_coupon_class == "优惠券")
                {
                    c.pro_coupon_code = "YHQ" + DateTime.Now.Ticks.ToString();
                }
                else
                {
                    c.pro_coupon_code = "YHM" + DateTime.Now.Ticks.ToString();
                }
                c.pro_coupon_discount = Request.Form["pro_coupon_discount"];
                c.pro_coupon_enddate = Convert.ToDateTime(Request.Form["pro_coupon_enddate"]);
                c.pro_coupon_num = Request.Form["pro_coupon_num"];
                c.pro_coupon_date = DateTime.Now;
                c.pro_coupon_stadate = DateTime.Now;
                c.pro_coupon_stastus = "0";
                db.pro_coupon.Add(c);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsCoupon", "Goods");
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

        public ActionResult GoodsCouponEdit(string pro_coupon_id)
        {
            try
            {
                pro_coupon cp = db.pro_coupon.Where(c => c.pro_coupon_id == pro_coupon_id).SingleOrDefault();
                ViewBag.pro_coupon_id = cp.pro_coupon_id;
                ViewBag.pro_coupon_class = cp.pro_coupon_class;
                ViewBag.pro_coupon_name = cp.pro_coupon_name;
                ViewBag.pro_coupon_discount = cp.pro_coupon_discount;
                ViewBag.pro_coupon_num = cp.pro_coupon_num;
                ViewBag.pro_coupon_enddate = cp.pro_coupon_enddate;
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult EditGoodsCoupon()
        {
            try
            {
                var pro_coupon_id = Request.Form["pro_coupon_id"];
                pro_coupon cp = db.pro_coupon.Where(c => c.pro_coupon_id == pro_coupon_id).SingleOrDefault();
                cp.pro_coupon_name = Request.Form["pro_coupon_name"];
                cp.pro_coupon_class = Request.Form["pro_coupon_class"];
                if (cp.pro_coupon_class == "优惠券")
                {
                    cp.pro_coupon_code = "YHQ" + DateTime.Now.Ticks.ToString();
                }
                else
                {
                    cp.pro_coupon_code = "YHM" + DateTime.Now.Ticks.ToString();
                }
                cp.pro_coupon_discount = Request.Form["pro_coupon_discount"];
                cp.pro_coupon_enddate = Convert.ToDateTime(Request.Form["pro_coupon_enddate"]);
                cp.pro_coupon_num = Request.Form["pro_coupon_num"];
                cp.pro_coupon_date = DateTime.Now;
                cp.pro_coupon_stastus = "0";
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsCoupon", "Goods");
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

        public ActionResult GoodsCouponStastus(string pro_coupon_id, string pro_coupon_stastus)
        {
            try
            {
                pro_coupon cp = db.pro_coupon.Where(c => c.pro_coupon_id == pro_coupon_id).SingleOrDefault();
                cp.pro_coupon_stastus = pro_coupon_stastus;
                cp.pro_coupon_stadate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("GoodsCoupon", "Goods");
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
