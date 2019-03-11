using EFClassLibrary;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;


namespace PhoneUI.Controllers
{
    public class GoodsController : Controller
    {
        //
        // GET: /Goods/

        D8MallEntities db = new D8MallEntities();
        public ActionResult Index(string GoodsClassId)
        {
            var listclass = db.pro_class.OrderByDescending(p => p.pro_class_order).ToList();
            ViewBag.listclass = listclass;
            var query = db.pro_sku;
            ViewBag.tiname = "所有产品";
            int listcount = 0;
            if (string.IsNullOrEmpty(GoodsClassId))
            {
                listcount = query.Where(s => s.pro_sku_status == "0").ToList().Count;
            }
            else
            {
                var querys = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault();
                listcount = query.Where(s => s.pro_sku_status == "0" & s.pro_sku_code == querys.pro_class_id).ToList().Count;
            }

            ViewBag.TotalPages = listcount;
            ViewBag.GoodsClassId = GoodsClassId;
            return View();
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
        /// 商品json读取
        /// </summary>
        /// <param name="CurPage"></param>
        /// <param name="GoodsClassId"></param>
        /// <returns></returns>
        /// 
        public JsonResult GoodsLists(int CurPage, string GoodsClassId)
        {
            try
            {
                var pagelist = PageView(CurPage, 20, GoodsClassId);
                return Json(pagelist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        #region 商品搜索

        public ActionResult GoodsSearch(string words)
        {
            try
            {
                var query = db.pro_sku;
                var querys = new pro_class();
                ViewBag.tiname = "所有产品";
                IList<pro_sku> lists = new List<pro_sku>();
                lists = query.Where(s => s.pro_sku_status == "0").ToList();
                lists = lists.Where(s => s.pro_sku_title.Contains(words)).ToList();
                IList<pro_skus> list = new List<pro_skus>();
                foreach (var item in lists)
                {
                    pro_skus sku = new pro_skus();

                    if (string.IsNullOrEmpty(item.pro_sku_covimg))
                    {
                        sku.pro_sku_covimg = "/file/img/default.jpg";
                    }
                    else
                    {
                        sku.pro_sku_covimg = item.pro_sku_covimg;
                    }

                    sku.pro_sku_code = item.pro_sku_code;
                    sku.pro_sku_arrspe = item.pro_sku_arrspe;
                    var pro_skuitem = db.pro_skuitem.Where(i => i.pro_sku_id == item.pro_sku_id).SingleOrDefault();
                    var pro_item = db.pro_item.Where(i => i.pro_item_id == item.pro_item_id).SingleOrDefault();
                    var pro_sku_title = string.Empty;
                    if (pro_item == null)
                    {
                        pro_sku_title = "";
                    }
                    else
                    {
                        pro_sku_title = pro_item.pro_brand_id;
                    }

                    #region 判断促销价格
                    var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == item.pro_sku_code && s.pro_sales_stastus == "1").SingleOrDefault();
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
                    sku.pro_sku_title = pro_sku_title;
                   
                    list.Add(sku);
                }

                IList<pro_skus> mPage = list;

                ViewBag.count = list.Count;
                ViewBag.words = words;
                return View(mPage);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult GoodsSearchList(int CurPage, string words)
        {
            try
            {
                var pagelist = PageView(CurPage, 18, "").Where(s=>s.pro_sku_title == words || s.pro_sku_arrspe == words).ToList();
                return Json(pagelist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region 商品详情
        public ActionResult GoodsDetails(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ViewBag.id = id;
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
                    ViewBag.pro_item_content = RegularExpression.GetImageUrl(Server.HtmlDecode(pro_item.pro_item_content), "http://www.cuckooshop.cn/");

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
                    #region 图片循环
                    var imglist = db.com_img.Where(i => i.com_img_fk == pro_sku.pro_sku_id).OrderByDescending(i => i.com_img_adddate).ToList();//图片list
                    var htmlstr = string.Empty;
                    if (imglist.Count > 0)
                    {
                        for (int i = 0; i < imglist.Count; i++)
                        {
                            StringBuilder sb = new StringBuilder();

                            sb.Append("<div class='swiper-slide' style='width:100%'><img src='http://www.cuckooshop.cn/" + imglist[i].com_img_src + "' alt='" + imglist[i].com_img_alt + "'  title='" + imglist[i].com_img_alt + "'  class='img-responsive' style='width:100%'/></div>");


                            htmlstr += sb.ToString();
                        }
                    }
                    ViewBag.htmlstr = htmlstr;

                    #endregion
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


        #endregion
        private IList<pro_skus> PageView(int pageIndx, int pageSize, string GoodsClassId)
        {

            IList<pro_sku> listcount = new List<pro_sku>();
            IList<pro_sku> lists = new List<pro_sku>();
            if (!string.IsNullOrEmpty(Request["GoodsClassId"]))
            {
                var query = db.pro_sku;
                var querys = new pro_class();
                querys = db.pro_class.Where(c => c.pro_class_page == GoodsClassId).SingleOrDefault();
                ViewBag.tiname = querys.pro_class_name;
                listcount = query.Where(s => s.pro_sku_status == "0" & s.pro_class_id == querys.pro_class_id).ToList();
                lists = query.Where(s => s.pro_sku_status == "0" & s.pro_class_id == querys.pro_class_id).OrderBy(s => s.pro_sku_order).Take(pageSize * pageIndx).Skip(pageSize * (pageIndx - 1)).ToList();
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
                var pro_item = db.pro_item.Where(i => i.pro_item_id == item.pro_item_id).SingleOrDefault();
                var pro_sku_title = string.Empty;
                if (pro_item == null)
                {
                    pro_sku_title = "";
                }
                else
                {
                    pro_sku_title = pro_item.pro_brand_id;
                }

               
                #region 判断促销价格
                var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == item.pro_sku_code && s.pro_sales_stastus == "1").SingleOrDefault();
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
                sku.pro_sku_title = pro_sku_title;
             
                list.Add(sku);
            }

            IList<pro_skus> mPage = list;
            return mPage;
        }


        public class com_banners : com_banner
        {
            public string com_img_src { get; set; }
            public string pro_brand_id { get; set; }

            public string pro_skuitem_price { get; set; }
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
                    ban.com_banner_url = new Uri(item.com_banner_url).AbsolutePath;
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
                    ban.com_banner_url = new Uri(item.com_banner_url).AbsolutePath;
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
                string str = string.Empty;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ilist8.Count; i++)
                {

                    if (i == 0)
                    {
                        sb.Append("<div class='col-xs-12 content_div'>");
                        sb.Append("<div class='col-xs-8 text-left content_div'><a href='" + ilist8[i].com_banner_url + "'><img src='http://www.cuckooshop.cn/" + ilist8[i].com_img_src + "' class='img-responsive center-block' /></a></div>");
                    }
                    else if (i == 1)
                    {
                        sb.Append("<div class='col-xs-4 text-left content_div'><a href='" + ilist8[i].com_banner_url + "'><img src='http://www.cuckooshop.cn/" + ilist8[i].com_img_src + "' class='img-responsive center-block' /></a></div>");
                        sb.Append("</div>");
                    }
                    else if (i == 2)
                    {
                        sb.Append("<div class='col-xs-12 content_div'>");
                        sb.Append("<div class='col-xs-4 text-left content_div'><a href='" + ilist8[i].com_banner_url + "'><img src='http://www.cuckooshop.cn/" + ilist8[i].com_img_src + "' class='img-responsive center-block' /></a></div>");
                    }
                    else if (i == 3)
                    {
                        sb.Append("<div class='col-xs-8 text-left content_div'><a href='" + ilist8[i].com_banner_url + "'><img src='http://www.cuckooshop.cn/" + ilist8[i].com_img_src + "' class='img-responsive center-block' /></a></div>");
                        sb.Append("</div>");
                    }
                    str = sb.ToString();

                }
                ViewBag.str = str;

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
                    sku.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString()).ToString("C");
                    listsku.Add(sku);
                }

                // IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageIndx, 12, nglist.Count, listsku);
                return View("GoodsNews", listsku);



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
                    ban.com_banner_url = new Uri(item.com_banner_url).AbsolutePath;
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
                    sku.pro_skuitem_price = Convert.ToDecimal(item.pro_sales_price.ToString()).ToString("C");
                    listsku.Add(sku);
                }
                //IList<pro_skus> mPage = PageCommon.PageList<pro_skus>(pageIndx, 16, sales.Count, listsku);
                return View("GoodsSales", listsku);
            }
            catch (Exception)
            {

                throw;
            }
        }



        #region 支付模块
        /// <summary>
        /// 订单支付中转模块
        /// </summary>
        /// <returns></returns>
        public JsonResult OrderTransfer()
        {
            try
            {
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var id = JSONHelper.JsonToString(jsonstr, "id");
                var num = JSONHelper.JsonToString(jsonstr, "num");
                string result = string.Empty;
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(num))
                {
                    var item = id + "|" + num;

                    result = item;
                }
                else
                {
                    result = "NO";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        #region 生成订单
        /// <summary>
        /// 填写订单
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [Personal]
        public ActionResult GoodsOrder(string items)
        {
            try
            {
                try
                {

                    #region 商品展示
                    var query = db.user_basic;
                    var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                    var upwd = HttpContext.Request.Cookies["value"].Value;
                    var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                    ViewBag.user_address = db.user_address.Where(u => u.user_basic_id == user_basic.user_basic_id).OrderByDescending(u => u.user_address_editdate).ToList();
                    var CY = string.Empty;//促销判断
                    string jsonstrs = Server.UrlDecode(items);
                    string[] arrpro = jsonstrs.Split('|');
                    string id = string.Empty;//商品ID
                    int num = 0;//数量
                    var tnum = 0;//总数
                    decimal tpirce = 0;//总价格
                    pro_skus sku = new pro_skus();//初始化商品
                    if (arrpro != null)//判断数组不为空
                    {
                        id = arrpro[0].ToString();
                        num = Convert.ToInt32(arrpro[1].ToString());
                        decimal price = 0;
                        var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == id).SingleOrDefault();
                        if (pro_sales != null)
                        {
                            CY = "CY";
                            price = Convert.ToDecimal(pro_sales.pro_sales_price);
                        }
                        else
                        {
                            var pro_sku = db.pro_sku.Where(s => s.pro_sku_code == id).SingleOrDefault();
                            var pro_skuitem = db.pro_skuitem.Where(pi => pi.pro_sku_id == pro_sku.pro_sku_id).SingleOrDefault();
                            price = Convert.ToDecimal(pro_skuitem.pro_skuitem_price);
                        }
                        sku.pro_sku_code = id;
                        sku.pro_sku_id = db.pro_sku.Where(p => p.pro_sku_code == id).SingleOrDefault().pro_sku_id;
                        sku.pro_sku_covimg = db.pro_sku.Where(p => p.pro_sku_code == id).SingleOrDefault().pro_sku_covimg;
                        var title = db.pro_sku.Where(p => p.pro_sku_code == id).SingleOrDefault().pro_sku_title;
                        sku.pro_sku_title = title.Length > 19 ? title.Substring(0, 19) + "…" : title;
                        sku.pro_skuitem_price = price.ToString("F2");
                        sku.pro_skuitem_mprice = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_mprice.ToString()).ToString("F2");
                        sku.shop_car_num = num;
                        sku.pro_skuitem_tprice = (Convert.ToDecimal(sku.pro_skuitem_price) * sku.shop_car_num).ToString();
                        sku.pro_skuitem_tmprice = (Convert.ToDecimal(sku.pro_skuitem_mprice) * sku.shop_car_num).ToString();
                        tnum = sku.shop_car_num;
                        tpirce = (Convert.ToDecimal(sku.pro_skuitem_price));

                    }
                    ViewBag.sku = sku;
                    ViewBag.tnum = tnum;
                    ViewBag.tpirce = (tpirce * tnum).ToString("F2");
                    ViewBag.pro_skuitem_id = id;
                    #endregion


                    //StringBuilder proitem = new StringBuilder();
                    //IList<pro_skus> list = new List<pro_skus>();
                    //var tnum = 0;
                    //decimal tpirce = 0;
                    //foreach (var item in arrpro)
                    //{
                    //    pro_skus sku = new pro_skus();
                    //    if (item != "")
                    //    {

                    //        string pro_skuitem_id = item.Substring(0, item.IndexOf(':'));
                    //        int index = item.IndexOf(':') + 1;
                    //        int last = item.LastIndexOf(':');
                    //        string num = item.Substring(index, (last - index));
                    //        string price = item.Substring((last + 1), (item.Length - 1) - last);
                    //        int shop_car_num = Convert.ToInt32(num);
                    //        decimal shop_car_price = Convert.ToDecimal(price);
                    //        var pro_sales = db.pro_sales.Where(s => s.pro_sku_code == pro_skuitem_id).SingleOrDefault();
                    //        if (pro_sales != null)
                    //        {
                    //            CY = "CY";
                    //        }
                    //        sku.pro_sku_code = pro_skuitem_id;
                    //        sku.pro_sku_id = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_id;
                    //        sku.pro_sku_covimg = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_covimg;
                    //        sku.pro_sku_title = db.pro_sku.Where(p => p.pro_sku_code == pro_skuitem_id).SingleOrDefault().pro_sku_title;
                    //        sku.pro_skuitem_price = shop_car_price.ToString("F2");
                    //        sku.pro_skuitem_mprice = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == sku.pro_sku_id).SingleOrDefault().pro_skuitem_mprice.ToString()).ToString("F2");
                    //        sku.shop_car_num = Convert.ToInt32(shop_car_num.ToString());
                    //        sku.pro_skuitem_tprice = (Convert.ToDecimal(sku.pro_skuitem_price) * sku.shop_car_num).ToString();
                    //        sku.pro_skuitem_tmprice = (Convert.ToDecimal(sku.pro_skuitem_mprice) * sku.shop_car_num).ToString();
                    //        tnum += sku.shop_car_num;
                    //        tpirce += (Convert.ToDecimal(sku.pro_skuitem_price));
                    //        sku.item = TDESHelper.EncryptString(item);
                    //        list.Add(sku);
                    //    }
                    //}
                    //ViewBag.list = list;



                    //#region 优惠券
                    //#region 控制优惠券码显示
                    //if (CY == "CY")
                    //{
                    //    ViewBag.dis = "style=display:none";
                    //}
                    //else
                    //{
                    //    ViewBag.dis = "";
                    //}
                    //#endregion

                    //IList<WebUI.Controllers.PersonalController.pro_coupons> pro_list = new List<WebUI.Controllers.PersonalController.pro_coupons>();
                    //IList<user_coupon> ulist = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
                    //foreach (var item in ulist)
                    //{
                    //    WebUI.Controllers.PersonalController.pro_coupons pros = new WebUI.Controllers.PersonalController.pro_coupons();
                    //    pros.pro_coupon_id = item.user_coupon_id.ToString();//用户优惠券所属ID
                    //    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;
                    //    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    //    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    //    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    //    pros.pro_coupon_num = "1";
                    //    pro_list.Add(pros);
                    //}
                    //var list_01 = pro_list.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠券").ToList();

                    //ViewBag.list_01 = list_01;
                    //#endregion


                    //#region 优惠码
                    //IList<WebUI.Controllers.PersonalController.pro_coupons> pro_lists = new List<WebUI.Controllers.PersonalController.pro_coupons>();
                    //IList<user_coupon> ulists = db.user_coupon.Where(u => u.user_basic_id == user_basic.user_basic_id & u.user_coupon_stastus == "0").ToList();
                    //foreach (var item in ulists)
                    //{
                    //    WebUI.Controllers.PersonalController.pro_coupons pros = new WebUI.Controllers.PersonalController.pro_coupons();
                    //    pros.pro_coupon_id = item.user_coupon_id.ToString();//用户优惠券所属ID
                    //    pros.pro_coupon_class = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_class;
                    //    pros.pro_coupon_discount = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_discount;
                    //    pros.pro_coupon_stadate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_stadate;
                    //    pros.pro_coupon_enddate = db.pro_coupon.Where(d => d.pro_coupon_id == item.pro_coupon_id).SingleOrDefault().pro_coupon_enddate;
                    //    pros.pro_coupon_num = "1";
                    //    pro_lists.Add(pros);
                    //}
                    //var list_02 = pro_lists.Where(l => l.pro_coupon_enddate >= DateTime.Now && l.pro_coupon_class == "优惠码").ToList();

                    //ViewBag.list_02 = list_02;


                    //#endregion
                    return View();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 配送地区查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
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

        /// <summary>
        /// 下单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Personal]
        public JsonResult GoodsOrderAdd()
        {
            try
            {
                int result = 0;//状态判断
                #region 初始化JSON 获取值
                var stre = HttpContext.Request.InputStream;
                var jsonstr = new StreamReader(stre).ReadToEnd();
                var addressid = JSONHelper.JsonToString(jsonstr, "addressid");//地址ID
                var shop_invoice_username = JSONHelper.JsonToString(jsonstr, "shop_invoice_username");//个人姓名
                var shop_invoice_usercode = JSONHelper.JsonToString(jsonstr, "shop_invoice_usercode");//身份证号
                var shop_invoice_company = JSONHelper.JsonToString(jsonstr, "shop_invoice_company");//公司名称
                var fapiao = JSONHelper.JsonToString(jsonstr, "fapiao");//公司名称
                var tnum = JSONHelper.JsonToString(jsonstr, "tnum");//总数
                var tpirce = JSONHelper.JsonToString(jsonstr, "tpirce");//总价格 
                var pro_skuitem_id = JSONHelper.JsonToString(jsonstr, "pro_skuitem_id");//商品ID
                #endregion

                #region 获取用户信息
                var query = db.user_basic;
                var uname = TDESHelper.DecryptString(HttpContext.Request.Cookies["keys"].Value);
                var upwd = HttpContext.Request.Cookies["value"].Value;
                var user_basic = query.Where(u => u.user_basic_login == uname & u.user_basic_pwd == upwd).SingleOrDefault();
                #endregion
                var guid = Guid.NewGuid().ToString("N");
                shop_order order = new shop_order();
                order.shop_order_id = guid;
                order.shop_order_code = DateTime.Now.Ticks.ToString();
                order.shop_order_status = "未支付";
                order.shop_order_totalmoeny = tpirce;
                var user_address_province = db.user_address.Where(a => a.user_address_id == addressid).SingleOrDefault().user_address_province;

                #region 地址分辨
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
                #endregion
                order.shop_order_waynum = tnum;
                order.user_address_id = addressid;
                order.user_basic_id = user_basic.user_basic_id;
                order.pro_coupon_id = "";//优惠折扣
                order.shop_order_buydate = DateTime.Now;
                order.shop_order_adddate = DateTime.Now;
                order.shop_order_editdate = DateTime.Now;
                db.shop_order.Add(order);
                result = db.SaveChanges();
                if (result > 0)
                {
                    decimal shop_price = Convert.ToDecimal(tpirce);
                    decimal shop_num = Convert.ToDecimal(tnum);
                    shop_orderdetail od = new shop_orderdetail();
                    od.shop_order_id = guid;
                    od.shop_orderdetail_id = Guid.NewGuid().ToString("N");
                    od.shop_orderdetail_price = Convert.ToDecimal(shop_price / shop_num);
                    od.pro_skuitem_id = pro_skuitem_id;
                    od.shop_orderdetail_total = shop_price;
                    od.shop_orderdetail_num = Convert.ToInt32(tnum);
                    db.shop_orderdetail.Add(od);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    result = db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }

                #region 发票处理
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
                    result = db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }
                #endregion
                if (result > 0)
                {
                    string url = string.Empty;
                    if (order.shop_order_remark == "YF")
                    {
                        url = "/Goods/GoodsOrderAddress?orid=" + guid;
                    }
                    else
                    {
                        url = "/Goods/GoodsOrderPay?orid=" + guid;
                    }
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
        #endregion

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

        #region 支付窗口


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
                var shop_order_totalmoeny = Convert.ToDecimal(so.shop_order_totalmoeny).ToString("C");
                var shop_order_code = so.shop_order_code;
                ViewBag.stock = _stock;

                #region 总价和订单号储存，用于微信
                Response.Cookies["shop_order_totalmoeny"].Value = TDESHelper.EncryptString(shop_order_totalmoeny);
                Response.Cookies["shop_order_totalmoeny"].Expires = DateTime.Now.AddHours(2);
                Response.Cookies["shop_order_code"].Value = TDESHelper.EncryptString(shop_order_code);
                Response.Cookies["shop_order_code"].Expires = DateTime.Now.AddHours(2);
                #endregion

                return View();
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
