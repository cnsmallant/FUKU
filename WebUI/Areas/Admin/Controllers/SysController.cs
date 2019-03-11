using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using System.Text;
using System.IO;


namespace WebUI.Areas.Admin.Controllers
{
    [AdminVerification]
    public class SysController : Controller
    {
        //
        // GET: /Admin/Sys/
        D8MallEntities db = new D8MallEntities();


        [Purview]
        public ActionResult Index()
        {
            return View();
        }
        [Purview]
        public ActionResult Log()
        {
            var query = db.sys_log;
            ViewBag.list = query.OrderByDescending(l => l.sys_log_adddate).ToList();
            return View();

        }


        [Purview]
        public ActionResult DelLog()
        {
            var query = db.sys_log.ToList();

            foreach (var item in query)
            {
                var log = db.sys_log.Where(s => s.sys_log_id == item.sys_log_id).SingleOrDefault();
                db.sys_log.Remove(log);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
            }

            return RedirectToAction("Log", "sys");
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [Purview]
        public FileResult ExportExcel()
        {
            var sbHtml = new StringBuilder();
            var list = db.sys_log.OrderByDescending(l => l.sys_log_adddate).ToList();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "日志编码", "操作IP", "日志内容", "操作日期" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            foreach (var item in list)
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.sys_log_name);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.sys_log_ip);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.sys_log_content);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", item.sys_log_adddate);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");

            byte[] fileContents = Encoding.Default.GetBytes(sbHtml.ToString());
            return File(fileContents, "application/ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + "日志文件.xls");
        }


        #region 权限管理
        /// <summary>
        /// 读取权限视图&执行
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult Purview()
        {
            var query = db.sys_purview;
            ViewBag.list = query.OrderBy(s => s.sys_purview_id).ToList();
            return View();
        }
        /// <summary>
        /// 新建权限视图
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult PurviewCreate()
        {
            return View();
        }
        /// <summary>
        /// 执行新建
        /// </summary>
        /// <returns></returns>

        [HttpPost]

        public ActionResult AddPurview()
        {
            try
            {

                var sys_purview_name = Request.Form["purviewname"];
                var sys_purview_page = Request.Form["purviewpage"];
                sys_purview pur = new sys_purview();
                pur.sys_purview_name = sys_purview_name;
                pur.sys_purview_page = sys_purview_page;
                pur.sys_purview_adduser = SysAdmin.sysAdmin().sys_admin_id;
                pur.sys_purview_edituser = SysAdmin.sysAdmin().sys_admin_id;
                pur.sys_purview_adddate = DateTime.Now;
                pur.sys_purview_editdate = DateTime.Now;
                db.sys_purview.Add(pur);
                int result = db.SaveChanges();
                if (result > 0)
                {

                    return RedirectToAction("Purview", "Sys");

                }
                else
                {
                    string msg = Server.UrlEncode("新建权限失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 编辑权限视图&执行读取
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult PurviewEdit(string sys_purview_id)
        {

            try
            {
                if (!string.IsNullOrEmpty(sys_purview_id))
                {
                    int pid = Convert.ToInt32(sys_purview_id.ToString());
                    var query = db.sys_purview.Where(p => p.sys_purview_id == pid).SingleOrDefault();
                    ViewBag.purviewid = query.sys_purview_id;
                    ViewBag.purviewname = query.sys_purview_name;
                    ViewBag.purviewpage = query.sys_purview_page;
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 执行编辑
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public ActionResult EditPurview()
        {

            try
            {

                var id = Convert.ToInt32(Request.Form["purviewid"].ToString());
                var purviewname = Request.Form["purviewname"];
                var purviewpage = Request.Form["purviewpage"];
                var pur = db.sys_purview.Where(p => p.sys_purview_id == id).SingleOrDefault();
                pur.sys_purview_name = purviewname;
                pur.sys_purview_page = purviewpage;
                pur.sys_purview_edituser = SysAdmin.sysAdmin().sys_admin_id;
                pur.sys_purview_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {

                    return RedirectToAction("Purview", "Sys");

                }
                else
                {
                    string msg = Server.UrlEncode("编辑权限失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 角色管理
        /// <summary>
        /// 读取角色
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult Role()
        {
            try
            {
                var query = db.sys_role;
                ViewBag.list = query.OrderBy(r => r.sys_role_editdate).ToList();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult RoleCreate(PostedFruits postedFruits)
        {
            try
            {
                return View(GetFruitsModel(postedFruits));

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult AddRole()
        {
            try
            {
                sys_role r = new sys_role();
                r.sys_role_id = Guid.NewGuid().ToString("N");
                r.sys_role_name = Request.Form["rolewname"];
                string chestr = Request.Form["chestr"];
                r.sys_role_purview = chestr.TrimEnd(',');
                r.sys_role_adduser = SysAdmin.sysAdmin().sys_admin_id;
                r.sys_role_addate = DateTime.Now;
                r.sys_role_edituser = SysAdmin.sysAdmin().sys_admin_id;
                r.sys_role_editdate = DateTime.Now;
                db.sys_role.Add(r);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Role", "Sys");
                }
                else
                {
                    string msg = Server.UrlEncode("新建角色失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private FruitViewModel GetFruitsModel(PostedFruits postedFruits)
        {
            // setup properties
            var model = new FruitViewModel();
            var selectedFruits = new List<Fruit>();
            var postedFruitIds = new string[0];
            if (postedFruits == null) postedFruits = new PostedFruits();

            // if a view model array of posted fruits ids exists
            // and is not empty,save selected ids
            if (postedFruits.FruitIds != null && postedFruits.FruitIds.Any())
            {
                postedFruitIds = postedFruits.FruitIds;
            }

            // if there are any selected ids saved, create a list of fruits
            if (postedFruitIds.Any())
            {
                selectedFruits = FruitRepository.GetAll()
                 .Where(x => postedFruitIds.Any(s => x.Id.ToString().Equals(s)))
                 .ToList();
            }

            //setup a view model
            model.AvailableFruits = FruitRepository.GetAll().ToList();
            model.SelectedFruits = selectedFruits;
            model.PostedFruits = postedFruits;
            return model;
        }
        private FruitViewModel GetFruitsInitialModel()
        {
            //setup properties
            var model = new FruitViewModel();
            var selectedFruits = new List<Fruit>();

            //setup a view model
            model.AvailableFruits = FruitRepository.GetAll().ToList();
            model.SelectedFruits = selectedFruits;
            return model;
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <returns></returns>
        [Purview]
        public ActionResult RoleEdit(string sys_role_id, PostedFruits postedFruits)
        {
            try
            {
                var query = db.sys_role;
                var role = query.OrderBy(r => r.sys_role_editdate).Where(r => r.sys_role_id == sys_role_id).SingleOrDefault();
                ViewBag.rolewname = role.sys_role_name;
                ViewBag.roleid = role.sys_role_id;
                return View(GetFruitsModel(postedFruits));
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postedFruits"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditRole()
        {
            try
            {
                string roleid = Request.Form["roleid"];
                sys_role role = db.sys_role.Where(r => r.sys_role_id == roleid).SingleOrDefault();
                role.sys_role_id = roleid;
                role.sys_role_name = Request.Form["rolewname"];
                string chestr = Request.Form["chestr"];
                role.sys_role_purview = chestr.TrimEnd(',');
                role.sys_role_edituser = SysAdmin.sysAdmin().sys_admin_id;
                role.sys_role_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Role", "Sys");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑角色失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion


        #region 管理人员

        public class sys_admins : sys_admin
        {
            public string sys_admin_rolename { get; set; }

            public string sys_admin_satatu { get; set; }
        }
        [Purview]
        public ActionResult Admin()
        {
            try
            {
                var query = db.sys_admin;
                var list = query.OrderBy(a => a.sys_admin_editdate);
                IList<sys_admins> lists = new List<sys_admins>();
                foreach (var item in list)
                {
                    sys_admins admins = new sys_admins();
                    admins.sys_admin_id = item.sys_admin_id;
                    admins.sys_admin_name = item.sys_admin_name;
                    admins.sys_admin_pwd = item.sys_admin_pwd;
                    admins.sys_admin_rolename = db.sys_role.Where(r => r.sys_role_id == item.sys_admin_role).SingleOrDefault().sys_role_name;
                    admins.sys_admin_satatu = item.sys_admin_satatus == 1 ? "正常" : "注销";
                    admins.sys_admin_addadduser = item.sys_admin_addadduser;
                    admins.sys_admin_adddate = item.sys_admin_adddate;
                    admins.sys_admin_edituser = item.sys_admin_edituser;
                    admins.sys_admin_editdate = item.sys_admin_editdate;
                    lists.Add(admins);
                }
                ViewBag.list = lists;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult AdminLogout(string sys_admin_id, string satat)
        {
            try
            {
                if (!string.IsNullOrEmpty(sys_admin_id) && !string.IsNullOrEmpty(satat))
                {
                    sys_admin a = db.sys_admin.Where(ad => ad.sys_admin_id == sys_admin_id).SingleOrDefault();
                    a.sys_admin_id = sys_admin_id;
                    a.sys_admin_satatus = Convert.ToInt32(satat);
                    a.sys_admin_edituser = SysAdmin.sysAdmin().sys_admin_id;
                    a.sys_admin_editdate = DateTime.Now;
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        return RedirectToAction("Admin", "Sys");
                    }
                    else
                    {
                        string msg = Server.UrlEncode("操作管理人员状态失败！");
                        return RedirectToAction("Index", "Message", new { mid = msg });
                    }
                }
                else
                {
                    string msg = Server.UrlEncode("操作管理人员失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult AdminCreate()
        {
            try
            {
                ViewData["role"] = from a in db.sys_role.OrderBy(r => r.sys_role_id).ToList()
                                   select new SelectListItem
                                   {
                                       Text = a.sys_role_name,
                                       Value = a.sys_role_id
                                   };
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult AddAdmin()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                sys_admin a = new sys_admin();
                a.sys_admin_id = Guid.NewGuid().ToString("N");
                a.sys_admin_name = Request.Form["sys_admin_name"];
                a.sys_admin_pwd = TDESHelper.EncryptString(Request.Form["sys_admin_pwd"]);
                a.sys_admin_role = Request.Form["sys_admin_role"];
                a.sys_admin_satatus = 1;
                a.sys_admin_addadduser = sys_admin.sys_admin_id;
                a.sys_admin_adddate = DateTime.Now;
                a.sys_admin_edituser = sys_admin.sys_admin_id;
                a.sys_admin_editdate = DateTime.Now;

                db.sys_admin.Add(a);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Admin", "Sys");
                }
                else
                {
                    string msg = Server.UrlEncode("新建管理人员失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Purview]
        public ActionResult AdminEdit(string sys_admin_id)
        {
            try
            {
                var query = db.sys_admin;
                var admin = query.Where(a => a.sys_admin_id == sys_admin_id).SingleOrDefault();
                var role = db.sys_role.ToList();
                ViewBag.sys_admin_name = admin.sys_admin_name;
                ViewBag.sys_admin_id = admin.sys_admin_id;
                SelectList selList = new SelectList(role, "sys_role_id", "sys_role_name", admin.sys_admin_role);
                ViewData["role"] = selList;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult EditAdmin()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                string sys_admin_id = Request.Form["sys_admin_id"];
                sys_admin a = db.sys_admin.Where(ad => ad.sys_admin_id == sys_admin_id).SingleOrDefault();
                a.sys_admin_id = sys_admin_id;
                a.sys_admin_name = Request.Form["sys_admin_name"];
                a.sys_admin_role = Request.Form["sys_admin_role"];
                a.sys_admin_edituser = sys_admin.sys_admin_id;
                a.sys_admin_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Admin", "Sys");
                }
                else
                {
                    string msg = Server.UrlEncode("编辑管理人员失败！");
                    return RedirectToAction("Index", "Message", new { mid = msg });
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult AdminPassword()
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

        public ActionResult EditAdminPWD()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                sys_admin a = db.sys_admin.Where(ad => ad.sys_admin_id == sys_admin.sys_admin_id).SingleOrDefault();
                a.sys_admin_pwd = TDESHelper.EncryptString(Request.Form["sys_admin_pwd"]);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    Response.Cookies["uname"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["upwd"].Expires = DateTime.Now.AddDays(-1);
                    return RedirectToAction("Index", "Login");
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

        public ActionResult ExitSystem()
        {
            try
            {
                Response.Cookies["uname"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["upwd"].Expires = DateTime.Now.AddDays(-1);
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion


        #region banner管理

        public class com_banners : com_banner
        {
            public string com_img_src { get; set; }
        }

        public ActionResult Banner(int pageid = 1)
        {
            try
            {

                var query = db.com_banner;
                var list = query.OrderByDescending(u => u.com_banner_adddate).ToList();
                IList<com_banners> lists = new List<com_banners>();
                foreach (var item in list)
                {
                    com_banners ban = new com_banners();

                    var com_img = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault();
                    if (com_img != null)
                    {
                        ban.com_banner_id = item.com_banner_id;
                        ban.com_banner_class = item.com_banner_class;
                        ban.com_banner_order = item.com_banner_order;
                        ban.com_banner_url = item.com_banner_url;
                        ban.com_img_src = com_img.com_img_src;
                        ban.com_banner_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.com_banner_adduser).SingleOrDefault().sys_admin_name;
                        ban.com_banner_adddate = item.com_banner_adddate;
                        ban.com_banner_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.com_banner_edituser).SingleOrDefault().sys_admin_name;
                        ban.com_banner_editdate = item.com_banner_editdate;
                        lists.Add(ban);
                    }


                }
                IList<com_banners> mPage = PageCommon.PageList<com_banners>(pageid, 20, list.Count, lists);
                return View("Banner", mPage);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult BannerClass(string com_banner_class, int pageid = 1)
        {
            try
            {

                var query = db.com_banner;
                var list = query.Where(b => b.com_banner_class == com_banner_class).OrderByDescending(u => u.com_banner_adddate).ToList();
                IList<com_banners> lists = new List<com_banners>();
                foreach (var item in list)
                {
                    com_banners ban = new com_banners();
                    ban.com_banner_id = item.com_banner_id;
                    ban.com_banner_class = item.com_banner_class;
                    ban.com_banner_order = item.com_banner_order;
                    ban.com_banner_url = item.com_banner_url;
                    ban.com_img_src = db.com_img.Where(i => i.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                    ban.com_banner_adduser = db.sys_admin.Where(a => a.sys_admin_id == item.com_banner_adduser).SingleOrDefault().sys_admin_name;
                    ban.com_banner_adddate = item.com_banner_adddate;
                    ban.com_banner_edituser = db.sys_admin.Where(a => a.sys_admin_id == item.com_banner_edituser).SingleOrDefault().sys_admin_name;
                    ban.com_banner_editdate = item.com_banner_editdate;
                    lists.Add(ban);
                }
                IList<com_banners> mPage = PageCommon.PageList<com_banners>(pageid, 20, list.Count, lists);
                return View("Banner", mPage);

            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult BannerCreate()
        {
            try
            {
                ViewBag.returnurl = Request.UrlReferrer.ToString();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult AddBanner()
        {
            try
            {
                var returnurl = Request.Form["returnurl"];
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                com_banner ban = new com_banner();
                string guid = Guid.NewGuid().ToString("N");
                ban.com_banner_id = guid;
                ban.com_banner_class = Request.Form["com_banner_class"];
                ban.com_banner_order = Request.Form["com_banner_order"];
                ban.com_banner_url = Request.Form["com_banner_url"];
                ban.com_banner_adduser = sys_admin.sys_admin_id;
                ban.com_banner_adddate = DateTime.Now;
                ban.com_banner_edituser = sys_admin.sys_admin_id;
                ban.com_banner_editdate = DateTime.Now;
                db.com_banner.Add(ban);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Create", "Images", new { com_img_fk = guid, returnurl = Server.UrlEncode(returnurl) });
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


        public ActionResult BannerEdit(string com_banner_id)
        {
            try
            {
                com_banner ban = db.com_banner.Where(b => b.com_banner_id == com_banner_id).SingleOrDefault();
                ViewBag.com_banner_order = ban.com_banner_order;
                ViewBag.com_banner_url = ban.com_banner_url;
                ViewBag.com_banner_id = ban.com_banner_id;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult EditBanner()
        {
            try
            {
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["uname"].Value);
                var upwd = HttpContext.Request.Cookies["upwd"].Value;
                var sys_admin = db.sys_admin.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
                var com_banner_id = Request.Form["com_banner_id"];
                com_banner ban = db.com_banner.Where(b => b.com_banner_id == com_banner_id).SingleOrDefault();
                ban.com_banner_order = Request.Form["com_banner_order"];
                ban.com_banner_url = Request.Form["com_banner_url"];
                ban.com_banner_edituser = sys_admin.sys_admin_id;
                ban.com_banner_editdate = DateTime.Now;
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Banner", "Sys");
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



        public ActionResult BannerDel(string com_banner_id)
        {
            try
            {
                com_banner ban = db.com_banner.Where(b => b.com_banner_id == com_banner_id).SingleOrDefault();
                db.com_banner.Remove(ban);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Banner", "Sys");
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



        #region 巨幕广告

        [Serializable]
        public class Ad_info : ad_info
        {
            public string com_img_src { get; set; }
        }

        /// <summary>
        /// 巨幕广告
        /// </summary>
        /// <returns></returns>
        public ActionResult Ad()
        {
            try
            {
                var query = db.ad_info;
                var list = query.OrderByDescending(u => u.ad_date).ToList();
                IList<Ad_info> lists = new List<Ad_info>();
                foreach (var item in list)
                {
                    Ad_info ad = new Ad_info();
                    ad.ad_id = item.ad_id;
                    ad.ad_title = item.ad_title;
                    ad.ad_type = item.ad_type;
                    ad.ad_img = item.ad_img;
                    ad.ad_sdate = item.ad_sdate;
                    ad.ad_edate = item.ad_edate;
                    ad.ad_date = item.ad_date;
                    com_img img = db.com_img.Where(i => i.com_img_fk == item.ad_id).SingleOrDefault();
                    if (img == null)
                    {
                        ad.com_img_src = "/file/img/default.jpg";
                    }
                    else
                    {
                        ad.com_img_src = img.com_img_src;
                    }
                    lists.Add(ad);
                }
                return View(lists);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 新建广告
        /// </summary>
        /// <returns></returns>
        public ActionResult AdCreate()
        {
            try
            {
                ViewBag.returnurl = Request.UrlReferrer.ToString();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 添加广告
        /// </summary>
        /// <returns></returns>
      [HttpPost]
        
        public JsonResult AddAd()
        {
            try
            {

                                    //ad_title: ad_title.val(),
                                    //ad_sdate: ad_sdate.val(),
                                    //ad_edate: ad_edate.val(),
                                    //ad_img: ad_img.val(),
                                    //ad_type: ad_type.val(),
                                    //returnurl: returnurl.val(),

                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var ad_title = JSONHelper.JsonToString(jsonstr, "ad_title");
                var ad_sdate = JSONHelper.JsonToString(jsonstr, "ad_sdate");
                var ad_edate = JSONHelper.JsonToString(jsonstr, "ad_edate");
                var ad_img = JSONHelper.JsonToString(jsonstr, "ad_img");
                var ad_type = JSONHelper.JsonToString(jsonstr, "ad_type");
                var returnurl = JSONHelper.JsonToString(jsonstr, "returnurl");
                ad_info ad = new ad_info();
                string guid = Guid.NewGuid().ToString("N");
                ad.ad_id = guid;
                ad.ad_type = ad_type;
                ad.ad_title = ad_title;
                ad.ad_img = ad_img;
                ad.ad_sdate = Convert.ToDateTime(ad_sdate);
                ad.ad_edate = Convert.ToDateTime(ad_edate);
                ad.ad_date = DateTime.Now;
                db.ad_info.Add(ad);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    string url = "/Admin/Images/Create?com_img_fk=" + guid + "&returnurl=" + Server.UrlEncode(returnurl);
                    return Json(url, JsonRequestBehavior.AllowGet);
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
        /// 删除
        /// </summary>
        /// <param name="com_banner_id"></param>
        /// <returns></returns>
        public ActionResult AdDel(string com_banner_id)
        {
            try
            {
                ad_info ban = db.ad_info.Where(b => b.ad_id == com_banner_id).SingleOrDefault();
                db.ad_info.Remove(ban);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Ad", "Sys");
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
        #endregion
    }
}
