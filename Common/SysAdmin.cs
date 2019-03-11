using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFClassLibrary;
using System.Web;



public class SysAdmin
{

    public static sys_admin sysAdmin()
    {
        D8MallEntities db = new D8MallEntities();
        var query = db.sys_admin;
        var uname = TDESHelper.DecryptString(HttpContext.Current.Request.Cookies["uname"].Value);
        var upwd = HttpContext.Current.Request.Cookies["upwd"].Value;
        sys_admin admin = query.Where(u => u.sys_admin_name == uname & u.sys_admin_pwd == upwd).SingleOrDefault();
        return admin;
    }
}

