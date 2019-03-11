using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;



public class PersonalAttribute : ActionFilterAttribute
{
    D8MallEntities db = new D8MallEntities();
    public override void OnActionExecuting(ActionExecutingContext context)
    {

        if (HttpContext.Current.Request.Cookies["keys"] == null || HttpContext.Current.Request.Cookies["value"] == null)
        {
            context.Result = new RedirectResult("/Login");
            return;
        }
        else
        {
            var query = db.user_basic;

            var uname = TDESHelper.DecryptString(HttpContext.Current.Request.Cookies["keys"].Value);
            var upwd = HttpContext.Current.Request.Cookies["value"].Value;
            var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
            string result = string.Empty;
            if (user_basic == null)
            {
                context.Result = new RedirectResult("/Login");
                return;
            }
            else
            {
                if (user_basic.user_basic_status != "0")
                {
                    context.Result = new RedirectResult("/Login");
                    return;

                }
                else
                {
                    base.OnActionExecuting(context);
                }

            }
        }
    }

}
