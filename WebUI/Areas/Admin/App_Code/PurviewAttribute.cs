using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;

/// <summary>
/// 权限验证
/// </summary>
public class PurviewAttribute : ActionFilterAttribute
{
    D8MallEntities db = new D8MallEntities();
    public override void OnActionExecuting(ActionExecutingContext context)
    {


        if (HttpContext.Current.Request.Cookies["uname"] == null || HttpContext.Current.Request.Cookies["upwd"] == null)
        {
            context.Result = new RedirectResult("/Admin/Login");
            return;
        }
        else
        {
            var query = db.sys_admin;

            var uname = TDESHelper.DecryptString(HttpContext.Current.Request.Cookies["uname"].Value);
            var upwd = HttpContext.Current.Request.Cookies["upwd"].Value;
            var sys_admin = query.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
            string result = string.Empty;
            if (sys_admin == null)
            {
                context.Result = new RedirectResult("/Admin/Login");
                return;
            }
            else
            {
                var sys_purview = db.sys_role.Where(r => r.sys_role_id == sys_admin.sys_admin_role).SingleOrDefault().sys_role_purview;
                int id = 0;
                string[] str = StringPlusCommon.GetStrArray(sys_purview, ',');
                string page = System.IO.Path.GetFileName(context.HttpContext.Request.PhysicalPath);
                for (int i = 0; i < str.Length; i++)
                {
                    id = Convert.ToInt32(str[i].ToString());
                    string pagename = db.sys_purview.Where(s => s.sys_purview_id == id).SingleOrDefault().sys_purview_page;
                    string purviewname = db.sys_purview.Where(s => s.sys_purview_id == id).SingleOrDefault().sys_purview_name;
                    if (pagename == page)
                    {
                        Log.LogTxt(purviewname, sys_admin.sys_admin_name);
                        base.OnActionExecuting(context);
                        return;
                    }
                    //else
                    //{
                    //    string msg = HttpContext.Current.Server.UrlEncode("您无权限操作，需要权限请联系管理员开通！");
                    //    context.Result = new RedirectResult("/Admin/Message?id=" + msg);
                    //    return;
                    //}

                }

                string msg = HttpContext.Current.Server.UrlEncode("您无权限操作，需要权限请联系管理员开通！");
                context.Result = new RedirectResult("/Admin/Message?mid=" + msg);
                return;
            }
        }
    }

}
