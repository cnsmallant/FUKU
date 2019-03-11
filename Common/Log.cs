using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EFClassLibrary;


/// <summary>
/// 添加日志
/// </summary>
public class Log
{
    /// <summary>
    /// 后台管理人员操作日志
    /// </summary>
    /// <param name="sys_log_ip"></param>
    /// <param name="sys_log_content"></param>
    /// <param name="sys_admin_name"></param>
    public static void LogTxt(string sys_log_content, string sys_admin_name)
    {
        D8MallEntities db = new D8MallEntities();

        sys_log log = new sys_log();
        log.sys_log_id = Guid.NewGuid().ToString("N");
        log.sys_log_ip = GetIP();
        log.sys_log_name = "LOG" + DateTime.Now.ToFileTime().ToString();
        log.sys_log_content = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒") + sys_admin_name + sys_log_content;
        log.sys_admin_id = sys_admin_name;
        log.sys_log_adddate = DateTime.Now;
        db.sys_log.Add(log);
        db.SaveChanges();
    }
    /// <summary>
    /// 获取IP
    /// </summary>
    /// <returns></returns>
    private static string GetIP()
    {
        string ip = string.Empty;
        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
            ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
        if (string.IsNullOrEmpty(ip))
            ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
        return ip;
    }
    ///// <summary>
    ///// 添加站内搜索
    ///// </summary>
    ///// <param name="SearchName">搜索标题</param>
    ///// <param name="SearchUrl">搜索链接</param>
    ///// <param name="SearchKey">关键字</param>
    ///// <param name="ProductTypeGuid">商品小类别</param>
    ///// <param name="ProductClassGuid">商品大类别</param>
    ///// <param name="ArticleTypeGuid">文章小类别</param>
    ///// <param name="ArticleClassGuid">文章大类别</param>
    //public static void SearchInfo(string SearchName,
    //    string SearchUrl, string SearchKey, string ProductTypeGuid,
    //    string ProductClassGuid, string ArticleTypeGuid, string ArticleClassGuid)
    //{
    //    Model.SearchInfo sea = new Model.SearchInfo();
    //    sea.SearchName = SearchName;
    //    sea.SearchUrl = SearchUrl;
    //    sea.SearchKey = SearchKey;
    //    sea.ProductTypeGuid = ProductTypeGuid;
    //    sea.ProductClassGuid = ProductClassGuid;
    //    sea.ArticleTypeGuid = ArticleTypeGuid;
    //    sea.ArticleClassGuid = ArticleClassGuid;
    //    BLL.SearchInfo.Add(sea);
    //}
}

