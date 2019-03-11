using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;
using System.Text;
using Webdiyer.WebControls.Mvc;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.IO;
using Com.Alipay;
using WxPayAPI;

namespace WebUI.Controllers
{
    public class GoodsController : Controller
    {
        //
        // GET: /Goods/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index(int pageIndx = 1)
        {


            var listclass = db.pro_class.OrderByDescending(p => p.pro_class_order).ToList();
            string htmlstrs = string.Empty;
            for (int i = 0; i < listclass.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<li class='li" + (i + 1) + "'><a href='/Goods?GoodsClassId=" + listclass[i].pro_class_page + "'>" + listclass[i].pro_class_name + "</a></li>");
                htmlstrs += sb.ToString();
            }
            ViewBag.htmlstrs = htmlstrs;
            if (!string.IsNullOrEmpty(Request["GoodsClassId"]) && !string.IsNullOrEmpty(Request["words"]))
            {
                var query = db.pro_spe;
                string GoodsClassId = Request["GoodsClassId"].ToString();
                string words = Request["words"].ToString();
                var pro_class_id = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault().pro_class_id;
                var list = query.Where(s => s.pro_class_id == pro_class_id & s.pro_spe_parent == "0").OrderByDescending(s => s.pro_spe_order).ToList();

                string htmlstr = string.Empty;

                for (int i = 0; i < list.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    var pro_spe_id = list[i].pro_spe_id;
                    sb.Append("<div class='kk'>");
                    var sublist = query.Where(s => s.pro_spe_parent == pro_spe_id).OrderByDescending(s => s.pro_spe_order).ToList();
                    sb.Append("<div class='kkl'>" + list[i].pro_spe_name + "：</div>");

                    sb.Append(" <div class='kkr'>");
                    if (i == 0)
                    {

                        sb.Append(" <span>");
                        for (int j = 0; j < sublist.Count; j++)
                        {
                            sb.Append("<a href='javascript:;' onclick='myonclick(this)' title='" + sublist[j].pro_spe_name + "'>" + sublist[j].pro_spe_name + "</a>");
                        }
                        sb.Append(" </span>");
                    }
                    else
                    {
                        sb.Append(" <ul>");
                        for (int x = 0; x < sublist.Count; x++)
                        {
                            sb.Append("<li><a href='javascript:;' onclick='myonclick(this)' title='" + sublist[x].pro_spe_name + "'>" + sublist[x].pro_spe_name + "</a></li>");
                        }
                        sb.Append(" </ul>");
                    }
                    sb.Append(" </div>");
                    sb.Append(" </div>");
                    htmlstr += sb.ToString();

                }

                ViewBag.htmlstr = htmlstr;
                ViewBag.GoodsClassId = GoodsClassId;
                return PageView(pageIndx, 20, GoodsClassId, words);
            }
            else
                if (!string.IsNullOrEmpty(Request["GoodsClassId"]))
                {
                    var query = db.pro_spe;
                    string GoodsClassId = Request["GoodsClassId"].ToString();
                    var pro_class_id = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault().pro_class_id;
                    var list = query.Where(s => s.pro_class_id == pro_class_id & s.pro_spe_parent == "0").OrderByDescending(s => s.pro_spe_order).ToList();

                    string htmlstr = string.Empty;

                    for (int i = 0; i < list.Count; i++)
                    {
                        StringBuilder sb = new StringBuilder();
                        var pro_spe_id = list[i].pro_spe_id;
                        sb.Append("<div class='kk'>");
                        var sublist = query.Where(s => s.pro_spe_parent == pro_spe_id).OrderByDescending(s => s.pro_spe_order).ToList();
                        sb.Append("<div class='kkl'>" + list[i].pro_spe_name + "：</div>");

                        sb.Append(" <div class='kkr'>");
                        if (i == 0)
                        {

                            sb.Append(" <span>");
                            for (int j = 0; j < sublist.Count; j++)
                            {
                                sb.Append("<a href='javascript:;' onclick='myonclick(this)' title='" + sublist[j].pro_spe_name + "'>" + sublist[j].pro_spe_name + "</a>");
                            }
                            sb.Append(" </span>");
                        }
                        else
                        {
                            sb.Append(" <ul>");
                            for (int x = 0; x < sublist.Count; x++)
                            {
                                sb.Append("<li><a href='javascript:;' onclick='myonclick(this)' title='" + sublist[x].pro_spe_name + "'>" + sublist[x].pro_spe_name + "</a></li>");
                            }
                            sb.Append(" </ul>");
                        }
                        sb.Append(" </div>");
                        sb.Append(" </div>");
                        htmlstr += sb.ToString();

                    }

                    ViewBag.htmlstr = htmlstr;
                    ViewBag.GoodsClassId = GoodsClassId;
                    return PageView(pageIndx, 20, GoodsClassId, "");
                }
                else
                    if (!string.IsNullOrEmpty(Request["words"]))
                    {
                        string words = Server.UrlDecode(Request["words"]);
                        return PageView(pageIndx, 20, "", words);
                    }
                    else
                    {
                        return PageView(pageIndx, 20, "", "");
                    }

        }
        public class pro_skus : pro_sku
        {
            public string pro_skuitem_price { get; set; }
            public string pro_skuitem_mprice { get; set; }
            public string pro_skuitem_tmprice { get; set; }
            public string pro_skuitem_tprice { get; set; }
            public int shop_car_num { get; set; }
            public string item { get; set; }
            public string pro_brand_id { get; set; }

            public string pro_item_subtitle { get; set; }


        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        private ActionResult PageView(int pageIndx, int pageSize, string GoodsClassId, string words)
        {

            IList<pro_sku> listcount = new List<pro_sku>();
            IList<pro_sku> lists = new List<pro_sku>();
            if (!string.IsNullOrEmpty(Request["GoodsClassId"]) && !string.IsNullOrEmpty(Request["words"]))
            {
                var query = from p in db.pro_sku select p;
                var querys = new pro_class();
                string str = Server.UrlDecode(words);
                str = str.TrimEnd(',');
                string[] word = str.Split(',');
                var g = GetConditionExpression<pro_sku>(word, "pro_sku_arrspe");

                querys = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault();
                ViewBag.tiname = querys.pro_class_name;
                listcount = query.Where(g).OrderBy(s => s.pro_sku_order).ToList().Where(s => s.pro_class_id == querys.pro_class_id & s.pro_sku_status == "0").ToList();
                lists = query.Where(g).OrderBy(s => s.pro_sku_order).OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList().Where(s => s.pro_class_id == querys.pro_class_id & s.pro_sku_status == "0").ToList(); ;
                ViewBag.goodscount = listcount.Count;
            }
            else if (!string.IsNullOrEmpty(Request["GoodsClassId"]))
            {
                var query = db.pro_sku;
                var querys = new pro_class();
                querys = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault();
                ViewBag.tiname = querys.pro_class_name;
                listcount = query.Where(s => s.pro_sku_status == "0" & s.pro_class_id == querys.pro_class_id).ToList();
                lists = query.Where(s => s.pro_sku_status == "0" & s.pro_class_id == querys.pro_class_id).OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList();
                ViewBag.goodscount = listcount.Count;
            }
            else if (!string.IsNullOrEmpty(Request["words"]))
            {
                var query = db.pro_sku;
                ViewBag.tiname = words;
                listcount = query.Where(s => s.pro_sku_status == "0" & s.pro_sku_title.Contains(words)).ToList();
                lists = query.Where(s => s.pro_sku_status == "0" & s.pro_sku_title.Contains(words)).OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList();
                ViewBag.goodscount = listcount.Count;
            }
            else
            {
                var query = db.pro_sku;
                var querys = new pro_class();
                ViewBag.tiname = "所有产品";
                listcount = query.Where(s => s.pro_sku_status == "0").ToList();
                lists = query.Where(s => s.pro_sku_status == "0").OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList();
                ViewBag.goodscount = listcount.Count;
            }

            IList<pro_skus> list = new List<pro_skus>();
            foreach (var item in lists)
            {
                pro_skus sku = new pro_skus();

                var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == item.pro_sku_code && s.pro_sales_stastus == "1").SingleOrDefault();
                //    var com_img = db.com_img.Where(i => i.com_img_fk == pro_sku.pro_sku_id).Take(1).SingleOrDefault();

                #region 判断促销标题
                if (pro_sales != null)
                {
                    sku.pro_sku_title = "[促]" + item.pro_sku_title;

                }
                else
                {
                    sku.pro_sku_title = item.pro_sku_title;
                }
                #endregion

                if (string.IsNullOrEmpty(item.pro_sku_covimg))
                {
                    sku.pro_sku_covimg = "/file/img/default.jpg";
                }
                else
                {
                    sku.pro_sku_covimg = item.pro_sku_covimg;
                }

                sku.pro_sku_code = item.pro_sku_code;

                var pro_skuitem = db.pro_skuitem.Where(i => i.pro_sku_id == item.pro_sku_id).SingleOrDefault();
              



                #region 判断促销价格

                if (pro_sales != null)
                {

                    sku.pro_skuitem_price = Convert.ToDecimal(pro_sales.pro_sales_price.ToString()).ToString("F2");
                }
                else
                {
                    if (pro_skuitem != null)
                    {
                        sku.pro_skuitem_price = Convert.ToDecimal(pro_skuitem.pro_skuitem_price.ToString()).ToString("F2");//现价
                    }
                    else
                    {
                        sku.pro_skuitem_price = "0.00";//现价
                    }

                }
                #endregion
             
                list.Add(sku);
            }

            IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageIndx, pageSize, listcount.Count, list);
            return View("Index", mPage);
        }

        //private ActionResult PageView(int pageIndx, int pageSize, string GoodsClassId, string words)
        //{

        //    IList<pro_sku> listcount = new List<pro_sku>();
        //    IList<pro_sku> lists = new List<pro_sku>();
        //    if (!string.IsNullOrEmpty(Request["GoodsClassId"]) && !string.IsNullOrEmpty(Request["words"]))
        //    {
        //        var query = from p in db.pro_sku select p;
        //        var querys = new pro_class();
        //        string str = Server.UrlDecode(words);
        //        str = str.TrimEnd(',');
        //        string[] word = str.Split(',');
        //        var g = GetConditionExpression<pro_sku>(word, "pro_sku_arrspe");

        //        querys = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault();
        //        ViewBag.tiname = querys.pro_class_name;
        //        listcount = query.Where(g).OrderBy(s => s.pro_sku_order).ToList().Where(s => s.pro_class_id == querys.pro_class_id & s.pro_sku_status == "0").ToList();
        //        lists = query.Where(g).OrderBy(s => s.pro_sku_order).OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList().Where(s => s.pro_class_id == querys.pro_class_id & s.pro_sku_status == "0").ToList(); ;
        //        ViewBag.goodscount = listcount.Count;
        //    }
        //    else if (!string.IsNullOrEmpty(Request["GoodsClassId"]))
        //    {
        //        var query = db.pro_sku;
        //        var querys = new pro_class();
        //        querys = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault();
        //        ViewBag.tiname = querys.pro_class_name;
        //        listcount = query.Where(s => s.pro_sku_status == "0" & s.pro_class_id == querys.pro_class_id).ToList();
        //        lists = query.Where(s => s.pro_sku_status == "0" & s.pro_class_id == querys.pro_class_id).OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList();
        //        ViewBag.goodscount = listcount.Count;
        //    }
        //    else
        //    {
        //        var query = db.pro_sku;
        //        var querys = new pro_class();
        //        ViewBag.tiname = "所有产品";
        //        listcount = query.Where(s => s.pro_sku_status == "0").ToList();
        //        lists = query.Where(s => s.pro_sku_status == "0").OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList();
        //        ViewBag.goodscount = listcount.Count;
        //    }

        //    IList<pro_skus> list = new List<pro_skus>();
        //    foreach (var item in lists)
        //    {
        //        pro_skus sku = new pro_skus();

        //        sku.pro_sku_title = item.pro_sku_title;
        //        sku.pro_sku_covimg = item.pro_sku_covimg;
        //        sku.pro_sku_code = item.pro_sku_code;
        //        decimal pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(i => i.pro_sku_id == item.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString());
        //        sku.pro_skuitem_price = pro_skuitem_price.ToString("C");
        //        list.Add(sku);
        //    }

        //    IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageIndx, pageSize, listcount.Count, list);
        //    return View("Index", mPage);
        //}


        /// <summary>
        /// 输出拼接字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetConditionExpression<T>(string[] options, string fieldName)
        {
            ParameterExpression left = Expression.Parameter(typeof(T), "c");//c=>
            Expression expression = Expression.Constant(true);
            foreach (var optionName in options)
            {
                Expression right = Expression.Call
                       (
                          Expression.Property(left, typeof(T).GetProperty(fieldName)),  //c.DataSourceName
                          typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),// 反射使用.Contains()方法                         
                         Expression.Constant(optionName)         // .Contains(optionName)
                       );
                expression = Expression.And(right, expression);//c.DataSourceName.contain("") || c.DataSourceName.contain("") 
            }
            Expression<Func<T, bool>> finalExpression
                = Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { left });
            return finalExpression;
        }
        /// <summary>
        /// 按照选择标记查询
        /// </summary>
        /// <returns></returns>
        public ActionResult GoodsMarkSearch()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["words"]) && !string.IsNullOrEmpty(Request["GoodsClassId"]))
                {
                    var words = Request["words"].ToString();
                    var GoodsClassId = Request["GoodsClassId"].ToString();
                    return RedirectToAction("Index", "Goods", new { GoodsClassId = GoodsClassId, words = words });
                }
                else if (!string.IsNullOrEmpty(Request["GoodsClassId"]))
                {
                    var GoodsClassId = Request["GoodsClassId"].ToString();
                    return RedirectToAction("Index", "Goods", new { GoodsClassId = GoodsClassId });
                }
                else if (!string.IsNullOrEmpty(Request["words"]))
                {
                    var words = Request["words"].ToString();
                    return RedirectToAction("Index", "Goods", new { words = words });
                }
                else
                {
                    return RedirectToAction("Index", "Goods");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GoodsDetails(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (Request.Cookies["GoodsScan"] != null)
                    {
                        var scan = TDESHelper.DecryptString(Request.Cookies["GoodsScan"].Value);
                        scan += id + ",";
                        Response.Cookies["GoodsScan"].Value = TDESHelper.EncryptString(scan);
                        Response.Cookies["GoodsScan"].Expires = DateTime.Now.AddDays(7);
                    }
                    else
                    {
                        var scan = id + ",";
                        Response.Cookies["GoodsScan"].Value = TDESHelper.EncryptString(scan);
                        Response.Cookies["GoodsScan"].Expires = DateTime.Now.AddDays(7);
                    }
                    ViewBag.id = id;
                    var pro_sku = db.pro_sku.Where(s => s.pro_sku_code == id).SingleOrDefault();
                    var pro_item = db.pro_item.Where(i => i.pro_item_id == pro_sku.pro_item_id).SingleOrDefault();
                    var pro_skuitem = db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku.pro_sku_id).SingleOrDefault();
                    var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == id && s.pro_sales_stastus == "1").SingleOrDefault();
                    //    var com_img = db.com_img.Where(i => i.com_img_fk == pro_sku.pro_sku_id).Take(1).SingleOrDefault();
                    ViewBag.pro_sku_id = id;
                    if (pro_sales != null)
                    {
                        ViewBag.pro_sku_title = "[促]" + pro_sku.pro_sku_title;
                        if (pro_sales.pro_sales_enddate <= DateTime.Now)
                        {
                            ViewBag.style = "style=display:none";
                            ViewBag.text = "活动已经结束";
                        }
                        else if (pro_sales.pro_sales_stadate >= DateTime.Now)
                        {
                            ViewBag.style = "style=display:none";
                            ViewBag.text = "活动还未开始，开始日期" + Convert.ToDateTime(pro_sales.pro_sales_stadate).ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        ViewBag.pro_sku_title = pro_sku.pro_sku_title;
                    }

                    ViewBag.pro_item_subtitle = pro_item.pro_item_subtitle;
                    ViewBag.pro_item_content = Server.HtmlDecode(pro_item.pro_item_content);

                    if (pro_sales != null)
                    {

                        ViewBag.pro_skuitem_price = Convert.ToDecimal(pro_sales.pro_sales_price.ToString()).ToString("F2");
                    }
                    else
                    {
                        if (pro_skuitem != null)
                        {
                            ViewBag.pro_skuitem_price = Convert.ToDecimal(pro_skuitem.pro_skuitem_price.ToString()).ToString("F2");//现价
                        }
                        else
                        {
                            ViewBag.pro_skuitem_price = "0.00";//现价
                        }

                    }

                    if (pro_skuitem != null)
                    {
                        ViewBag.pro_skuitem_mprice = Convert.ToDecimal(pro_skuitem.pro_skuitem_mprice.ToString()).ToString("C");//原价
                        ViewBag.pro_skuitem_stock = pro_skuitem.pro_skuitem_stock;
                    }
                    else
                    {
                        ViewBag.pro_skuitem_mprice = "0.00";//原价
                        ViewBag.pro_skuitem_stock = "0.00";
                    }

                    var imglist = db.com_img.Where(i => i.com_img_fk == pro_sku.pro_sku_id).OrderByDescending(i => i.com_img_adddate).ToList();//图片list


                    #region 已售出数量计算

                    int ysci = 0;
                    var xsl = db.shop_order.Where(o => o.shop_order_status != "未支付" && o.shop_order_status != "已删除").ToList();//正常的订单
                    foreach (var item in xsl)
                    {
                        var subo = db.shop_orderdetail.Where(s => s.shop_order_id == item.shop_order_id).ToList();//订单明细
                        foreach (var items in subo)
                        {
                            if (id == items.pro_skuitem_id)
                            {
                                ysci += Convert.ToInt32(items.shop_orderdetail_num);//统计数量
                            }
                        }
                    }

                    ViewBag.ysc = ysci;

                    //var ysc = db.shop_orderdetail.Where(o => o.pro_skuitem_id == id).ToList();
                    //int ysci = 0;
                    //foreach (var item in ysc)
                    //{
                    //    var ord = db.shop_order.Where(or => or.shop_order_id == item.shop_order_id).SingleOrDefault();
                    //    if (ord != null)
                    //    {
                    //        if (ord.shop_order_status != "未支付" && ord.shop_order_status != "已删除")
                    //        {

                    //            ysci=
                    //        }
                    //    }
                    //}

                    //ViewBag.ysc = ysci;
                    #endregion
                    var htmlstr = string.Empty;
                    if (imglist.Count > 0)
                    {
                        for (int i = 0; i < imglist.Count; i++)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (i == 0)
                            {
                                ViewBag.com_img_title = imglist[i].com_img_alt;
                                ViewBag.com_img_alt = imglist[i].com_img_alt;
                                ViewBag.com_img_src = imglist[i].com_img_src;
                                sb.Append("<li id='onlickImg'><img src='" + imglist[i].com_img_src + "' alt='" + imglist[i].com_img_alt + "'  title='" + imglist[i].com_img_alt + "' /></li>");
                            }
                            else
                            {
                                sb.Append("<li ><img src='" + imglist[i].com_img_src + "' alt='" + imglist[i].com_img_alt + "'  title='" + imglist[i].com_img_alt + "' /></li>");
                            }
                            htmlstr += sb.ToString();
                        }
                    }
                    ViewBag.htmlstr = htmlstr;

                    IList<pro_skus> listsku = new List<pro_skus>();
                    var nglist = db.pro_newsgoods.OrderByDescending(n => n.pro_newsgoods_order).ToList();
                    foreach (var item in nglist)
                    {
                        pro_skus sku = new pro_skus();
                        pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                        sku.pro_sku_title = skus.pro_sku_title;
                        sku.pro_sku_covimg = skus.pro_sku_covimg;
                        sku.pro_sku_code = item.pro_sku_code;
                        sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;
                        sku.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString()).ToString("F2");
                        listsku.Add(sku);
                    }
                    ViewBag.goodsnew = listsku;

                    IList<pro_skus> listskus = new List<pro_skus>();
                    var sales = db.pro_sales.Take(10).Where(p => p.pro_sales_stastus == "1" & p.pro_sales_stadate <= DateTime.Now & p.pro_sales_enddate >= DateTime.Now).OrderByDescending(n => n.pro_sales_order).ToList();
                    foreach (var item in sales)
                    {
                        pro_skus sku = new pro_skus();
                        pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                        sku.pro_sku_title = skus.pro_sku_title;
                        sku.pro_sku_covimg = skus.pro_sku_covimg;
                        sku.pro_sku_code = item.pro_sku_code;
                        sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;
                        sku.pro_item_subtitle = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_item_subtitle;
                        sku.pro_skuitem_price = Convert.ToDecimal(item.pro_sales_price.ToString()).ToString("F2");
                        listskus.Add(sku);
                    }
                    ViewBag.goodsSales = listskus;
                }




                #region 商品评价

                var com = db.pro_comment.Where(c => c.pro_sku_code == id).OrderByDescending(c => c.pro_comment_date).ToList();
                IList<pro_comment> list = new List<pro_comment>();
                foreach (var item in com)
                {
                    pro_comment cc = new pro_comment();
                    cc.pro_comment_id = item.pro_comment_id;
                    cc.pro_comment_content = item.pro_comment_content;
                    cc.user_basic_id = db.user_basic.Where(u => u.user_basic_id == item.user_basic_id).SingleOrDefault().user_basic_login;
                    cc.pro_comment_date = item.pro_comment_date;
                    cc.pro_comment_star = item.pro_comment_star;
                    list.Add(cc);
                }
                ViewBag.list = list;

                #endregion
                #region 收藏人气
                ViewBag.ucount = db.user_collect.Where(u => u.pro_skuitem_id == id).Count();
                #endregion
                #region 服务详情
                ViewBag.content = Server.HtmlDecode(db.com_article.Where(a => a.com_article_class == "returns").SingleOrDefault().com_article_content);
                #endregion

                #region 浏览喜欢
                IList<pro_sku> listscan = new List<pro_sku>();
                if (Request.Cookies["GoodsScan"] != null)
                {
                    var scan = TDESHelper.DecryptString(Request.Cookies["GoodsScan"].Value);
                    string[] str = scan.Split(',');

                    foreach (var item in str)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            pro_sku sku = new pro_sku();
                            sku.pro_sku_id = db.pro_sku.Where(s => s.pro_sku_code == item).SingleOrDefault().pro_sku_id;
                            var pro_item_id = db.pro_sku.Where(s => s.pro_sku_code == item).SingleOrDefault().pro_item_id;
                            sku.pro_sku_code = item;
                            sku.pro_sku_covimg = db.pro_sku.Where(s => s.pro_sku_code == item).SingleOrDefault().pro_sku_covimg;
                            sku.pro_item_id = db.pro_item.Where(i => i.pro_item_id == pro_item_id).SingleOrDefault().pro_brand_id;
                            sku.pro_sku_arrspe = Convert.ToDecimal(db.pro_skuitem.Where(sk => sk.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString()).ToString("F2");
                            listscan.Add(sku);
                        }
                    }

                }

                Random ran = new Random();
                int RandKey = ran.Next(1, listscan.Count);

                IList<pro_sku> mPage = PageCommon.PageList<pro_sku>(RandKey, 5, listscan.Count, listscan);
                ViewBag.mPage = mPage;
                #endregion
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public JsonResult GoodsProvince(string id)
        {
            try
            {
                var result = db.com_area.Where(c => c.com_area_parentid == id).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public JsonResult GoodsVal(string id)
        {
            try
            {
                pro_shipment sh = db.pro_shipment.Where(p => p.pro_shipment_county == id).SingleOrDefault();
                pro_shipment shi = new pro_shipment();
                string val = string.Empty;
                int iiid = Convert.ToInt32(id);
                if (sh == null)
                {
                    shi.pro_shipment_county = db.com_area.Where(s => s.com_area_id == iiid).SingleOrDefault().com_area_name;
                    shi.pro_shipment_price = "暂无配送信息";
                }
                else
                {
                    string pro_shipment_price = Convert.ToDecimal(sh.pro_shipment_price).ToString("C");
                    shi.pro_shipment_county = db.com_area.Where(s => s.com_area_id == iiid).SingleOrDefault().com_area_name;
                    shi.pro_shipment_price = "此地区可以配送";
                }

                return Json(shi, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult GoodsCart()
        {
            try
            {
                IList<pro_skus> list = new List<pro_skus>();

                if (Request.Cookies["shopstr"] != null)
                {
                    string shos = Request.Cookies["shopstr"].Value;
                    string jsonstrs = TDESHelper.DecryptString(shos);
                    string[] arrpro = jsonstrs.Split(',');
                    StringBuilder proitem = new StringBuilder();

                    foreach (var item in arrpro)
                    {
                        pro_skus sku = new pro_skus();
                        if (item != "")
                        {

                            string pro_skuitem_id = item.Substring(0, item.IndexOf(':'));
                            int index = item.IndexOf(':') + 1;
                            int last = item.LastIndexOf(':');
                            string num = item.Substring(index, (last - index));
                            string price = item.Substring((last + 1), (item.Length - 1) - last);
                            int shop_car_num = Convert.ToInt32(num);
                            decimal shop_car_price = Convert.ToDecimal(price);
                            sku.pro_sku_code = pro_skuitem_id;
                            sku.pro_sku_id = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_id;
                            sku.pro_sku_covimg = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                            sku.pro_sku_title = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_title;
                            sku.pro_skuitem_price = shop_car_price.ToString("F2");
                            sku.pro_skuitem_mprice = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_mprice.ToString()).ToString("F2");
                            sku.shop_car_num = Convert.ToInt32(shop_car_num.ToString());
                            sku.pro_skuitem_tprice = (Convert.ToDecimal(sku.pro_skuitem_price) * sku.shop_car_num).ToString();
                            sku.pro_skuitem_tmprice = (Convert.ToDecimal(sku.pro_skuitem_mprice) * sku.shop_car_num).ToString();
                            sku.item = TDESHelper.EncryptString(item);
                            list.Add(sku);
                        }
                    }

                }
                ViewBag.list = list;
                ViewBag.UrlReferrer = Request.UrlReferrer.ToString();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GoodsCartCreate()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                shop_car car = JSONHelper.DeserializeJsonToObject<shop_car>(jsonstr);
                string shopstr = string.Empty;
                string result = string.Empty;
                if (Request.Cookies["shopstr"] == null)
                {
                    shopstr = car.pro_skuitem_id + ":" + car.shop_car_num.ToString() + ":" + car.shop_car_price.ToString();
                    Response.Cookies["shopstr"].Value = TDESHelper.EncryptString(shopstr);
                    Response.Cookies["shopstr"].Expires = DateTime.Now.AddDays(1);
                    result = "OK";
                }
                else
                {
                    string shos = Request.Cookies["shopstr"].Value;
                    string jsonstrs = TDESHelper.DecryptString(shos);
                    string[] arrpro = jsonstrs.Split(',');
                    bool flg = false;
                    int pronum = 0;
                    StringBuilder sb = new StringBuilder(jsonstrs);

                    foreach (var item in arrpro)
                    {
                        if (item != "")
                        {
                            string proid = item.Substring(0, item.IndexOf(':'));
                            int index = item.IndexOf(':') + 1;
                            int last = item.LastIndexOf(':');
                            if (car.pro_skuitem_id == proid)
                            {
                                string num = item.Substring(index, (last - index));
                                pronum = Convert.ToInt32(num);
                                flg = true;
                                break;
                            }

                        }
                    }
                    if (flg == false)
                    {
                        shopstr = "," + car.pro_skuitem_id + ":" + car.shop_car_num.ToString() + ":" + car.shop_car_price.ToString();
                        sb.Append(shopstr);
                    }
                    else
                    {
                        string newstr = car.pro_skuitem_id + ":" + (car.shop_car_num + pronum).ToString() + ":" + car.shop_car_price.ToString();
                        string oldstr = car.pro_skuitem_id + ":" + pronum.ToString() + ":" + car.shop_car_price.ToString();
                        sb.Replace(oldstr, newstr);
                        sb.ToString();
                    }
                    Response.Cookies["shopstr"].Value = TDESHelper.EncryptString(sb.ToString());
                    Response.Cookies["shopstr"].Expires = DateTime.Now.AddDays(1);
                    result = "OK";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public ActionResult GoodsCartDel(string item)
        {
            try
            {
                var items = TDESHelper.DecryptString(item);
                if (Request.Cookies["shopstr"] != null)
                {
                    string shos = Request.Cookies["shopstr"].Value;
                    string jsonstrs = TDESHelper.DecryptString(shos);
                    StringBuilder proitem = new StringBuilder(jsonstrs);
                    proitem.Replace(items, "");
                    var newsitem = proitem.ToString();
                    Response.Cookies["shopstr"].Value = TDESHelper.EncryptString(newsitem.ToString());
                    Response.Cookies["shopstr"].Expires = DateTime.Now.AddDays(1);
                }
                return RedirectToAction("GoodsCart", "Goods");

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GoodsSelectDel(string items)
        {
            try
            {

                if (Request.Cookies["shopstr"] != null)
                {
                    items = Server.UrlDecode(items);
                    string shos = Request.Cookies["shopstr"].Value;
                    string jsonstrs = TDESHelper.DecryptString(shos);
                    StringBuilder proitem = new StringBuilder(jsonstrs);
                    proitem.Replace(items, "");
                    var newsitem = proitem.ToString();
                    Response.Cookies["shopstr"].Value = TDESHelper.EncryptString(newsitem.ToString());
                    Response.Cookies["shopstr"].Expires = DateTime.Now.AddDays(1);
                }
                return RedirectToAction("GoodsCart", "Goods");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Personal]
        public ActionResult GoodsOrder(string items)
        {
            try
            {

                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                ViewBag.user_address = db.user_address.Where(u => u.user_basic_id == user_basic.user_basic_id).OrderByDescending(u => u.user_address_editdate).ToList();
                ViewBag.items = items;
                var CY = string.Empty;//促销判断
                string jsonstrs = Server.UrlDecode(items);
                string[] arrpro = jsonstrs.Split(',');
                StringBuilder proitem = new StringBuilder();
                IList<pro_skus> list = new List<pro_skus>();
                var tnum = 0;
                decimal tpirce = 0;
                foreach (var item in arrpro)
                {
                    pro_skus sku = new pro_skus();
                    if (item != "")
                    {

                        string pro_skuitem_id = item.Substring(0, item.IndexOf(':'));
                        int index = item.IndexOf(':') + 1;
                        int last = item.LastIndexOf(':');
                        string num = item.Substring(index, (last - index));
                        string price = item.Substring((last + 1), (item.Length - 1) - last);
                        int shop_car_num = Convert.ToInt32(num);
                        decimal shop_car_price = Convert.ToDecimal(price);
                        var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == pro_skuitem_id).SingleOrDefault();
                        if (pro_sales != null)
                        {
                            CY = "CY";
                        }
                        sku.pro_sku_code = pro_skuitem_id;
                        sku.pro_sku_id = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_id;
                        sku.pro_sku_covimg = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                        sku.pro_sku_title = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_title;
                        sku.pro_skuitem_price = shop_car_price.ToString("F2");
                        sku.pro_skuitem_mprice = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_mprice.ToString()).ToString("F2");
                        sku.shop_car_num = Convert.ToInt32(shop_car_num.ToString());
                        sku.pro_skuitem_tprice = (Convert.ToDecimal(sku.pro_skuitem_price) * sku.shop_car_num).ToString();
                        sku.pro_skuitem_tmprice = (Convert.ToDecimal(sku.pro_skuitem_mprice) * sku.shop_car_num).ToString();
                        tnum += sku.shop_car_num;
                        tpirce += (Convert.ToDecimal(sku.pro_skuitem_price));
                        sku.item = TDESHelper.EncryptString(item);
                        list.Add(sku);
                    }
                }
                ViewBag.list = list;
                ViewBag.tnum = tnum;
                ViewBag.tpirce = tpirce.ToString("F2");


                #region 优惠券
                #region 控制优惠券码显示
                if (CY == "CY")
                {
                    ViewBag.dis = "style=display:none";
                }
                else
                {
                    ViewBag.dis = "";
                }
                #endregion

                IList<WebUI.Controllers.PersonalController.pro_coupons> pro_list = new List<WebUI.Controllers.PersonalController.pro_coupons>();
                IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
                foreach (var item in ulist)
                {
                    WebUI.Controllers.PersonalController.pro_coupons pros = new WebUI.Controllers.PersonalController.pro_coupons();
                    pros.pro_coupon_id = item.user_coupon_id.ToString();//用户优惠券所属ID
                    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;
                    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    pros.pro_coupon_num = "1";
                    pro_list.Add(pros);
                }
                var list_01 = pro_list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠券").ToList();

                ViewBag.list_01 = list_01;
                #endregion


                #region 优惠码
                IList<WebUI.Controllers.PersonalController.pro_coupons> pro_lists = new List<WebUI.Controllers.PersonalController.pro_coupons>();
                IList<user_coupon> ulists = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
                foreach (var item in ulists)
                {
                    WebUI.Controllers.PersonalController.pro_coupons pros = new WebUI.Controllers.PersonalController.pro_coupons();
                    pros.pro_coupon_id = item.user_coupon_id.ToString();//用户优惠券所属ID
                    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;
                    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    pros.pro_coupon_num = "1";
                    pro_lists.Add(pros);
                }
                var list_02 = pro_lists.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠码").ToList();

                ViewBag.list_02 = list_02;


                #endregion
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public JsonResult GoodsUserCoupon()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public JsonResult GoodsGetShipment()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var user_address_id = JSONHelper.JsonToString(jsonstr, "user_address_id");
                user_address ua = db.user_address.Where(u => u.user_address_id == user_address_id).SingleOrDefault();
                pro_shipment sh = db.pro_shipment.Where(s => s.pro_shipment_county == ua.user_address_county).SingleOrDefault();
                if (sh == null)
                {
                    return Json("暂无配送信息", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(sh.pro_shipment_price, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult GoodsOrderAdd()
        {
            try
            {
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var user_address_id = Request.Form["user_address_id"];
                var shop_order_totalmoeny = Request.Form["shop_order_totalmoeny"];
                var shop_order_waynum = Request.Form["shop_order_waynum"];
                var fapiao = Request.Form["fapiao"];
                var items = Request.Form["items"];
                var user_coupon_id = Request.Form["user_coupon_id"];
                var guid = Guid.NewGuid().ToString("N");
                var pro_coupon_id = string.Empty; ;
                decimal pro_coupon_discount = 0;

                if (!string.IsNullOrEmpty(user_coupon_id))
                {
                    int user_coupon_ids = Convert.ToInt32(user_coupon_id);
                    user_coupon uc = db.user_coupon.Where(u => u.user_coupon_id == user_coupon_ids).SingleOrDefault();
                    uc.user_coupon_stastus = "1";
                    pro_coupon_id = uc.pro_coupon_id;
                    var pro_coupon01 = db.pro_coupon.Where(p => p.pro_coupon_id == uc.pro_coupon_id).SingleOrDefault();
                    pro_coupon_discount = Convert.ToDecimal(pro_coupon01.pro_coupon_discount);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }


                shop_order order = new shop_order();
                order.shop_order_id = guid;
                order.shop_order_code = DateTime.Now.Ticks.ToString();
                order.shop_order_status = "未支付";
                //if (pro_coupon_discount != 0)
                //{
                //    decimal totalmoeny = Convert.ToDecimal(shop_order_totalmoeny);
                //    order.shop_order_totalmoeny = (totalmoeny * pro_coupon_discount).ToString();
                //}
                //else
                //{
                order.shop_order_totalmoeny = shop_order_totalmoeny;

                //}
                var user_address_province = db.user_address.Where(a => a.user_address_id == user_address_id).SingleOrDefault().user_address_province;
                int province = Convert.ToInt32(user_address_province);
                var com_area_shortname = db.com_area.Where(a => a.com_area_id == province).SingleOrDefault().com_area_shortname;
                if (com_area_shortname == "新疆" || com_area_shortname == "西藏" || com_area_shortname == "台湾" || com_area_shortname == "香港" || com_area_shortname == "澳门")
                {
                    order.shop_order_remark = "YF";//有运费
                }
                else
                {
                    order.shop_order_remark = "BY";//包邮
                }
                order.shop_order_waynum = shop_order_waynum;
                order.user_address_id = user_address_id;
                order.user_basic_id = user_basic.user_basic_id;
                order.pro_coupon_id = user_coupon_id;//优惠折扣
                order.shop_order_buydate = DateTime.Now;
                order.shop_order_adddate = DateTime.Now;
                order.shop_order_editdate = DateTime.Now;
                db.shop_order.Add(order);
                string jsonstrs = items;
                string[] arrpro = jsonstrs.Split(',');
                StringBuilder proitem = new StringBuilder();

                foreach (var item in arrpro)
                {

                    if (item != "")
                    {

                        string pro_skuitem_id = item.Substring(0, item.IndexOf(':'));
                        int index = item.IndexOf(':') + 1;
                        int last = item.LastIndexOf(':');
                        string num = item.Substring(index, (last - index));
                        string price = item.Substring((last + 1), (item.Length - 1) - last);
                        int shop_car_num = Convert.ToInt32(num);
                        decimal shop_car_price = Convert.ToDecimal(price);

                        shop_orderdetail od = new shop_orderdetail();
                        od.shop_order_id = guid;
                        od.shop_orderdetail_id = Guid.NewGuid().ToString("N");
                        od.shop_orderdetail_price = Convert.ToDecimal(shop_car_price / shop_car_num);
                        od.pro_skuitem_id = pro_skuitem_id;
                        od.shop_orderdetail_total = shop_car_price;
                        od.shop_orderdetail_num = Convert.ToInt32(num);
                        db.shop_orderdetail.Add(od);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        db.Configuration.ValidateOnSaveEnabled = true;
                    }
                }


                if (!string.IsNullOrEmpty(fapiao))
                {
                    shop_invoice si = new shop_invoice();
                    if (fapiao == "gr")
                    {

                        si.shop_order_id = guid;
                        si.shop_invoice_username = Request.Form["shop_invoice_username"];
                        si.shop_invoice_usercode = Request.Form["shop_invoice_usercode"];
                        si.shop_invoice_class = "gr";

                    }
                    else
                    {

                        si.shop_order_id = guid;
                        si.shop_invoice_company = Request.Form["shop_invoice_company"];
                        si.shop_invoice_class = "gs";

                    }
                    db.shop_invoice.Add(si);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }

                if (order.shop_order_remark == "YF")
                {
                    return RedirectToAction("GoodsOrderAddress", "Goods", new { orid = guid });
                }
                else
                {
                    return RedirectToAction("GoodsOrderPay", "Goods", new { orid = guid });

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        #region 地址验证
        public ActionResult GoodsOrderAddress(string orid)
        {
            try
            {
                shop_order so = db.shop_order.Where(s => s.shop_order_id == orid).SingleOrDefault();
                ViewBag.shop_order_totalmoeny = Convert.ToDecimal(so.shop_order_totalmoeny).ToString("C");
                ViewBag.shop_order_code = so.shop_order_code;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult OrderAddress(string orid)
        {
            try
            {
                shop_order so = db.shop_order.Where(s => s.shop_order_id == orid).SingleOrDefault();
                if (so.shop_order_remark == "YF")
                {
                    return RedirectToAction("GoodsOrderAddress", "Goods", new { orid = orid });
                }
                else
                {
                    return RedirectToAction("GoodsOrderPay", "Goods", new { orid = orid });

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region 支付


        public ActionResult GoodsOrderPay(string orid)
        {
            try
            {
                shop_order so = db.shop_order.Where(s => s.shop_order_id == orid).SingleOrDefault();
                #region 验证库存
                var sd = db.shop_orderdetail.Where(sds => sds.shop_order_id == so.shop_order_id).ToList();
                var _stock = string.Empty;
                foreach (var item in sd)
                {
                    pro_sku sku = db.pro_sku.Where(sk => sk.pro_sku_code == item.pro_skuitem_id).SingleOrDefault();
                    if (sku != null)
                    {
                        pro_skuitem skt = db.pro_skuitem.Where(pskt => pskt.pro_sku_id == sku.pro_sku_id).SingleOrDefault();
                        if (skt != null)
                        {
                            if (skt.pro_skuitem_stock > 0)
                            {
                                _stock = "Y";
                            }
                            else
                            {
                                _stock = "N";
                            }
                        }
                    }
                }
                #endregion
                ViewBag.shop_order_totalmoeny = Convert.ToDecimal(so.shop_order_totalmoeny).ToString("C");
                ViewBag.shop_order_code = so.shop_order_code;
                ViewBag.stock = _stock;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }


        #region 优惠码读取
        /// <summary>
        /// 优惠码
        /// </summary>
        /// <param name="yhm"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult YHM()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var yhm = JSONHelper.JsonToString(jsonstr, "yhm");
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                var pro_coupon = db.pro_coupon.Where(p => p.pro_coupon_code == yhm).SingleOrDefault();
                var user_coupon = db.user_coupon.Where(u => u.user_coupon_stastus == "0" & u.user_basic_id == user_basic.user_basic_id & u.pro_coupon_id == pro_coupon.pro_coupon_id).Take(1).SingleOrDefault();
                if (user_coupon != null)
                {
                    string s = string.Empty;
                    var pro_coupon01 = db.pro_coupon.Where(p => p.pro_coupon_id == user_coupon.pro_coupon_id).SingleOrDefault();
                    s = user_coupon.user_coupon_id + "|" + pro_coupon01.pro_coupon_discount;
                    return Json(s, JsonRequestBehavior.AllowGet);
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

        #endregion

        public void GoodsPayImmediately(string shop_order_code)
        {
            try
            {

                shop_order so = db.shop_order.Where(s => s.shop_order_code == shop_order_code).SingleOrDefault();
                ////////////////////////////////////////////请求参数////////////////////////////////////////////

                //商户订单号，商户网站订单系统中唯一订单号，必填
                string out_trade_no = so.shop_order_code;

                //订单名称，必填
                string subject = so.shop_order_code;

                //付款金额，必填
                string total_fee = so.shop_order_totalmoeny;

                //商品描述，可空
                string body = "";




                ////////////////////////////////////////////////////////////////////////////////////////////////

                //把请求参数打包成数组
                SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                sParaTemp.Add("service", Config.service);
                sParaTemp.Add("partner", Config.partner);
                sParaTemp.Add("seller_id", Config.seller_id);
                sParaTemp.Add("_input_charset", Config.input_charset.ToLower());
                sParaTemp.Add("payment_type", Config.payment_type);
                sParaTemp.Add("notify_url", Config.notify_url);
                sParaTemp.Add("return_url", Config.return_url);
                sParaTemp.Add("anti_phishing_key", Config.anti_phishing_key);
                sParaTemp.Add("exter_invoke_ip", Config.exter_invoke_ip);
                sParaTemp.Add("out_trade_no", out_trade_no);
                sParaTemp.Add("subject", subject);
                sParaTemp.Add("total_fee", total_fee);
                sParaTemp.Add("body", body);
                //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.O9yorI&treeId=62&articleId=103740&docType=1
                //如sParaTemp.Add("参数名","参数值");

                //建立请求
                string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                Response.Write(sHtmlText);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult WxPay(string shop_order_code)
        {
            try
            {
                ViewBag.shop_order_code = shop_order_code;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }



        #endregion



        public class com_banners : com_banner
        {
            public string com_img_src { get; set; }

            public string pro_skuitem_price { get; set; }

            public string pro_brand_id { get; set; }
        }
        /// <summary>
        /// 新品
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult GoodsNews(int pageIndx = 1)
        {
            try
            {
                var list = db.com_banner.Where(b => b.com_banner_class == "新品").OrderByDescending(b => b.com_banner_order).ToList();
                IList<com_banners> ilist = new List<com_banners>();
                foreach (var item in list)
                {
                    com_banners ban = new com_banners();
                    ban.com_banner_url = item.com_banner_url;
                    ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                    ilist.Add(ban);
                }
                ViewBag.bannerlist = ilist;

                string pro_sku_code = string.Empty;
                var list8 = db.com_banner.Where(b => b.com_banner_class == "新品广告").OrderByDescending(b => b.com_banner_order).ToList().Take(4);
                IList<com_banners> ilist8 = new List<com_banners>();
                foreach (var item in list8)
                {

                    com_banners ban = new com_banners();
                    ban.com_banner_url = item.com_banner_url;
                    pro_sku_code = item.com_banner_url;
                    int index = pro_sku_code.LastIndexOf("/") + 1;
                    int last = pro_sku_code.Length - index;
                    pro_sku_code = pro_sku_code.Substring(index, last);
                    var pro_sku = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
                    ban.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("C");
                    ban.pro_brand_id = db.pro_item.Where(i => i.pro_item_id == pro_sku.pro_item_id).SingleOrDefault().pro_brand_id;
                    ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                    ilist8.Add(ban);
                }
                ViewBag.list8 = ilist8;


                IList<pro_skus> listsku = new List<pro_skus>();
                var nglist = db.pro_newsgoods.OrderByDescending(n => n.pro_newsgoods_order).ToList();
                foreach (var item in nglist)
                {
                    pro_skus sku = new pro_skus();
                    pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                    sku.pro_sku_title = skus.pro_sku_title;
                    sku.pro_sku_covimg = skus.pro_sku_covimg;
                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;
                    sku.pro_skuitem_price = db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString();
                    listsku.Add(sku);
                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageIndx, 12, nglist.Count, listsku);
                return View("GoodsNews", mPage);



            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 促销
        /// </summary>
        /// <param name="pageIndx"></param>
        /// <returns></returns>
        public ActionResult GoodsSales(int pageIndx = 1)
        {
            try
            {
                var list = db.com_banner.Where(b => b.com_banner_class == "促销").OrderByDescending(b => b.com_banner_order).ToList();
                IList<com_banners> ilist = new List<com_banners>();
                foreach (var item in list)
                {
                    com_banners ban = new com_banners();
                    ban.com_banner_url = item.com_banner_url;
                    ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                    ilist.Add(ban);
                }
                ViewBag.bannerlist = ilist;
                string pro_sku_code = string.Empty;
                var list8 = db.com_banner.Where(b => b.com_banner_class == "促销广告").OrderByDescending(b => b.com_banner_order).ToList().Take(5);
                IList<com_banners> ilist8 = new List<com_banners>();
                foreach (var item in list8)
                {

                    com_banners ban = new com_banners();
                    ban.com_banner_url = item.com_banner_url;
                    pro_sku_code = item.com_banner_url;
                    int index = pro_sku_code.LastIndexOf("/") + 1;
                    int last = pro_sku_code.Length - index;
                    pro_sku_code = pro_sku_code.Substring(index, last);
                    var pro_sku = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
                    ban.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("C");
                    ban.pro_brand_id = db.pro_item.Where(i => i.pro_item_id == pro_sku.pro_item_id).SingleOrDefault().pro_brand_id;
                    ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                    ilist8.Add(ban);
                }
                ViewBag.list8 = ilist8;




                IList<pro_skus> listsku = new List<pro_skus>();
                var sales = db.pro_sales.Where(p => p.pro_sales_stastus == "1" & p.pro_sales_enddate >= DateTime.Now & p.pro_sales_stadate <= DateTime.Now).OrderByDescending(n => n.pro_sales_order).ToList();
                foreach (var item in sales)
                {
                    pro_skus sku = new pro_skus();
                    pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                    sku.pro_sku_title = skus.pro_sku_title;
                    sku.pro_sku_covimg = skus.pro_sku_covimg;
                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;
                    sku.pro_item_subtitle = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_item_subtitle;
                    sku.pro_skuitem_price = Convert.ToDecimal(item.pro_sales_price.ToString()).ToString("F2");
                    listsku.Add(sku);
                }
                IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageIndx, 16, sales.Count, listsku);
                return View("GoodsSales", mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
