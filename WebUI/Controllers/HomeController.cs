using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFClassLibrary;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        D8MallEntities db = new D8MallEntities();

        public class com_banners : com_banner
        {
            public string com_img_src { get; set; }
            public string pro_brand_id { get; set; }

            public string pro_skuitem_price { get; set; }
        }

        public class pro_newsgoodss : pro_newsgoods
        {
            public string com_img_src { get; set; }


            public string pro_sku_title { get; set; }

            public string pro_sku_covimg { get; set; }

            public string pro_brand_id { get; set; }

            public string pro_skuitem_price { get; set; }
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
        public ActionResult Index()
        {

            var query = db.pro_class.Where(c => c.pro_parent == "0");
            ViewBag.list = query.OrderByDescending(c => c.pro_class_order).ToList();
            #region Banner
            var list = db.com_banner.Where(b => b.com_banner_class == "首页").OrderByDescending(b => b.com_banner_order).ToList();
            IList<com_banners> ilist = new List<com_banners>();
            foreach (var item in list)
            {
                com_banners ban = new com_banners();
                var com_img = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault();
                if (com_img != null)
                {
                    ban.com_banner_url = item.com_banner_url;
                    ban.com_img_src = com_img.com_img_src;
                    ilist.Add(ban);
                }


            }
            ViewBag.bannerlist = ilist;
            #endregion

            #region 推荐
            IList<pro_newsgoodss> listsku = new List<pro_newsgoodss>();
            var recommendlist = db.pro_newsgoods.Where(n => n.pro_newsgoods_status == "0").OrderByDescending(n => n.pro_newsgoods_order).ToList();
            foreach (var item in recommendlist)
            {
                pro_newsgoodss sku = new pro_newsgoodss();
                pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                sku.pro_sku_title = skus.pro_sku_title;
                sku.pro_sku_covimg = skus.pro_sku_covimg;
                sku.pro_sku_code = item.pro_sku_code;
                sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;
                sku.pro_skuitem_price = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_price.ToString()).ToString("F2");
                var nid = "cx" + item.pro_newsgoods_id;
                sku.com_img_src = db.com_img.Where(c => c.com_img_fk == nid).SingleOrDefault().com_img_src;
                listsku.Add(sku);
            }
            ViewBag.recommend = listsku;
            #endregion

            #region 超高压2.0电饭煲系列
            string pro_sku_code = string.Empty;
            var list2 = db.com_banner.Where(b => b.com_banner_class == "首页超高压2.0电饭煲系列").OrderByDescending(b => b.com_banner_order).ToList();
            IList<com_banners> ilist2 = new List<com_banners>();
            foreach (var item in list2)
            {
                com_banners ban = new com_banners();
                ban.com_banner_url = item.com_banner_url;
                pro_sku_code = item.com_banner_url;
                int index = pro_sku_code.LastIndexOf("/") + 1;
                int last = pro_sku_code.Length - index;
                pro_sku_code = pro_sku_code.Substring(index, last);
                var pro_sku = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
                var pro_skuitem = db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku.pro_sku_id).SingleOrDefault();
                if (pro_skuitem != null)
                {
                    var pro_skuitem_price = pro_skuitem.pro_skuitem_price;
                    ban.pro_skuitem_price = Convert.ToDecimal(pro_skuitem_price).ToString("C");
                }
                else
                {
                    ban.pro_skuitem_price = "0";
                }


                ban.pro_brand_id = db.pro_item.Where(i => i.pro_item_id == pro_sku.pro_item_id).SingleOrDefault().pro_brand_id;
                ban.com_img_src = db.com_img.Where(c => c.com_img_fk == item.com_banner_id).SingleOrDefault().com_img_src;
                ilist2.Add(ban);
            }
            ViewBag.list2 = ilist2;

            #endregion

            #region 原装进口IH高压电饭煲系列

            var list3 = db.com_banner.Where(b => b.com_banner_class == "首页原装进口IH高压电饭煲系列").OrderByDescending(b => b.com_banner_order).ToList();
            IList<com_banners> ilist3 = new List<com_banners>();
            foreach (var item in list3)
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
                ilist3.Add(ban);
            }
            ViewBag.list3 = ilist3;

            #endregion

            #region 安心空气净化器系列

            var list4_1 = db.com_banner.Where(b => b.com_banner_class == "首页安心空气净化器系列").OrderByDescending(b => b.com_banner_order).Skip(2 * (1 - 1)).Take(2).ToList();
            IList<com_banners> ilist4_1 = new List<com_banners>();
            foreach (var item in list4_1)
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
                ilist4_1.Add(ban);
            }
            ViewBag.list4_1 = ilist4_1;
            var list4_2 = db.com_banner.Where(b => b.com_banner_class == "首页安心空气净化器系列").OrderByDescending(b => b.com_banner_order).Skip(2 * (2 - 1)).Take(2).ToList();
            IList<com_banners> ilist4_2 = new List<com_banners>();
            foreach (var item in list4_2)
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
                ilist4_2.Add(ban);
            }
            ViewBag.list4_2 = ilist4_2;

            ViewBag.jhqadurl = db.com_banner.Where(b => b.com_banner_class == "首页净化器广告").SingleOrDefault().com_banner_url;
            var banner_id = db.com_banner.Where(b => b.com_banner_class == "首页净化器广告").SingleOrDefault().com_banner_id;
            ViewBag.jhqimg = db.com_img.Where(i => i.com_img_fk == banner_id).SingleOrDefault().com_img_src;
            #endregion


            #region 厨房家电系列

            #region 原汁机
            var list5 = db.com_banner.Where(b => b.com_banner_class == "首页原汁机").OrderByDescending(b => b.com_banner_order).ToList();
            IList<com_banners> ilist5 = new List<com_banners>();
            foreach (var item in list5)
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
                ilist5.Add(ban);
            }
            ViewBag.list5 = ilist5;


            ViewBag.yzjadurl = db.com_banner.Where(b => b.com_banner_class == "首页原汁机广告").SingleOrDefault().com_banner_url;
            var yzj_idad = db.com_banner.Where(b => b.com_banner_class == "首页原汁机广告").SingleOrDefault().com_banner_id;
            var yzj_id = db.com_banner.Where(b => b.com_banner_class == "首页原汁机1").SingleOrDefault().com_banner_id;
            ViewBag.yzjimg = db.com_img.Where(i => i.com_img_fk == yzj_idad).SingleOrDefault().com_img_src;
            ViewBag.yzj1 = db.com_banner.Where(b => b.com_banner_class == "首页原汁机1").SingleOrDefault().com_banner_url;
            var yzj_id1 = db.com_banner.Where(b => b.com_banner_class == "首页原汁机1").SingleOrDefault().com_banner_id;
            ViewBag.yzjimg1 = db.com_img.Where(i => i.com_img_fk == yzj_id).SingleOrDefault().com_img_src;
            int index_yzj = pro_sku_code.LastIndexOf("/") + 1;
            int last_yzj = pro_sku_code.Length - index_yzj;
            pro_sku_code = pro_sku_code.Substring(index_yzj, last_yzj);
            var pro_sku_zyj = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
            ViewBag.pro_skuitem_price_yzj = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku_zyj.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("C");
            ViewBag.pro_brand_id_yzj = db.pro_item.Where(i => i.pro_item_id == pro_sku_zyj.pro_item_id).SingleOrDefault().pro_brand_id;
            #endregion

            #region 重汤机
            var list6 = db.com_banner.Where(b => b.com_banner_class == "首页重汤机").OrderByDescending(b => b.com_banner_order).ToList();
            IList<com_banners> ilist6 = new List<com_banners>();
            foreach (var item in list6)
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
                ilist6.Add(ban);
            }
            ViewBag.list6 = ilist6;


            ViewBag.ztjadurl = db.com_banner.Where(b => b.com_banner_class == "首页重汤机广告").SingleOrDefault().com_banner_url;
            var ztj_idad = db.com_banner.Where(b => b.com_banner_class == "首页重汤机广告").SingleOrDefault().com_banner_id;
            var ztj_id = db.com_banner.Where(b => b.com_banner_class == "首页重汤机1").SingleOrDefault().com_banner_id;
            ViewBag.ztjimg = db.com_img.Where(i => i.com_img_fk == ztj_idad).SingleOrDefault().com_img_src;
            ViewBag.ztj1 = db.com_banner.Where(b => b.com_banner_class == "首页重汤机1").SingleOrDefault().com_banner_url;
            var ztj_id1 = db.com_banner.Where(b => b.com_banner_class == "首页重汤机1").SingleOrDefault().com_banner_id;
            ViewBag.ztjimg1 = db.com_img.Where(i => i.com_img_fk == ztj_id).SingleOrDefault().com_img_src;
            int index_ztj = pro_sku_code.LastIndexOf("/") + 1;
            int last_ztj = pro_sku_code.Length - index_ztj;
            pro_sku_code = pro_sku_code.Substring(index_ztj, last_ztj);
            var pro_sku_ztj = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
            ViewBag.pro_skuitem_price_ztj = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku_ztj.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("C");
            ViewBag.pro_brand_id_ztj = db.pro_item.Where(i => i.pro_item_id == pro_sku_ztj.pro_item_id).SingleOrDefault().pro_brand_id;

            #endregion

            #region 电烤盘
            var list7 = db.com_banner.Where(b => b.com_banner_class == "首页电烤盘").OrderByDescending(b => b.com_banner_order).ToList();
            IList<com_banners> ilist7 = new List<com_banners>();
            foreach (var item in list7)
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
                ilist7.Add(ban);
            }
            ViewBag.list7 = ilist7;


            ViewBag.dkpadurl = db.com_banner.Where(b => b.com_banner_class == "首页电烤盘广告").SingleOrDefault().com_banner_url;
            var dkp_idad = db.com_banner.Where(b => b.com_banner_class == "首页电烤盘广告").SingleOrDefault().com_banner_id;
            var dkp_id = db.com_banner.Where(b => b.com_banner_class == "首页电烤盘1").SingleOrDefault().com_banner_id;
            ViewBag.dkpimg = db.com_img.Where(i => i.com_img_fk == dkp_idad).SingleOrDefault().com_img_src;
            ViewBag.dkp1 = db.com_banner.Where(b => b.com_banner_class == "首页电烤盘1").SingleOrDefault().com_banner_url;
            var dkp_id1 = db.com_banner.Where(b => b.com_banner_class == "首页电烤盘1").SingleOrDefault().com_banner_id;
            ViewBag.dkpimg1 = db.com_img.Where(i => i.com_img_fk == dkp_id).SingleOrDefault().com_img_src;
            int index_dkp = pro_sku_code.LastIndexOf("/") + 1;
            int last_dkp = pro_sku_code.Length - index_dkp;
            pro_sku_code = pro_sku_code.Substring(index_dkp, last_dkp);
            var pro_sku_dkp = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
            ViewBag.pro_skuitem_price_dkp = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku_dkp.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("C");
            ViewBag.pro_brand_id_dkp = db.pro_item.Where(i => i.pro_item_id == pro_sku_dkp.pro_item_id).SingleOrDefault().pro_brand_id;
            #endregion

            #region 电水壶
            var list8 = db.com_banner.Where(b => b.com_banner_class == "首页电水壶").OrderByDescending(b => b.com_banner_order).ToList();
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


            ViewBag.dshadurl = db.com_banner.Where(b => b.com_banner_class == "首页电水壶广告").SingleOrDefault().com_banner_url;
            var dsh_idad = db.com_banner.Where(b => b.com_banner_class == "首页电水壶广告").SingleOrDefault().com_banner_id;
            var dsh_id = db.com_banner.Where(b => b.com_banner_class == "首页电水壶1").SingleOrDefault().com_banner_id;
            ViewBag.dshimg = db.com_img.Where(i => i.com_img_fk == dsh_idad).SingleOrDefault().com_img_src;
            ViewBag.dsh1 = db.com_banner.Where(b => b.com_banner_class == "首页电水壶1").SingleOrDefault().com_banner_url;
            var dsh_id1 = db.com_banner.Where(b => b.com_banner_class == "首页电水壶1").SingleOrDefault().com_banner_id;
            ViewBag.dshimg1 = db.com_img.Where(i => i.com_img_fk == dsh_id).SingleOrDefault().com_img_src;
            int index_dsh = pro_sku_code.LastIndexOf("/") + 1;
            int last_dsh = pro_sku_code.Length - index_dsh;
            pro_sku_code = pro_sku_code.Substring(index_dsh, last_dsh);
            var pro_sku_dsh = db.pro_sku.Where(p => p.pro_sku_code == pro_sku_code).SingleOrDefault();
            ViewBag.pro_skuitem_price_dsh = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == pro_sku_dsh.pro_sku_id).SingleOrDefault().pro_skuitem_price).ToString("C");
            ViewBag.pro_brand_id_dsh = db.pro_item.Where(i => i.pro_item_id == pro_sku_dsh.pro_item_id).SingleOrDefault().pro_brand_id;
            #endregion
            #endregion

            #region 特价

            IList<pro_skus> listskus = new List<pro_skus>();
            var sales = db.pro_sales.Where(p => p.pro_sales_stastus == "1").OrderByDescending(n => n.pro_sales_order).ToList().Take(15);
            foreach (var item in sales)
            {
                pro_skus sku = new pro_skus();
                pro_sku skus = db.pro_sku.Where(s => s.pro_sku_code == item.pro_sku_code).SingleOrDefault();
                sku.pro_sku_title = skus.pro_sku_title;
                sku.pro_sku_covimg = skus.pro_sku_covimg;
                sku.pro_sku_code = item.pro_sku_code;
                sku.pro_brand_id = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_brand_id;

                sku.pro_item_subtitle = db.pro_item.Where(s => s.pro_item_id == skus.pro_item_id).SingleOrDefault().pro_item_subtitle;
                sku.pro_skuitem_mprice = Convert.ToDecimal(db.pro_skuitem.Where(s => s.pro_sku_id == skus.pro_sku_id).SingleOrDefault().pro_skuitem_mprice).ToString("C");
                sku.pro_skuitem_price = Convert.ToDecimal(item.pro_sales_price.ToString()).ToString("F2");
                listskus.Add(sku);
            }
            ViewBag.goodes = listskus;

            #endregion

            return View();
        }


        [Serializable]
        public class Ad_info : ad_info
        {
            public string com_img_src { get; set; }
        }
        [HttpPost]
        public JsonResult Ad()
        {
            try
            {

                var d = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var ss = db.ad_info.Where(a=>a.ad_type == "pc").OrderBy(s => s.ad_date).ToList();
                var query = ss.Where(s => s.ad_sdate <= d & s.ad_edate>=d).OrderBy(s => s.ad_date).Take(1).SingleOrDefault();
                if (query == null)
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Ad_info ad = new Ad_info();

                    com_img img = db.com_img.Where(i => i.com_img_fk == query.ad_id).SingleOrDefault();
                    if (img == null)
                    {
                        ad.com_img_src = "http://www.cuckooshop.cn/file/img/default.jpg";
                    }
                    else
                    {
                        ad.com_img_src = "http://www.cuckooshop.cn/" + img.com_img_src;
                    }
                    ad.ad_title = query.ad_title;
                    ad.ad_img =  query.ad_img.AbsolutePath;
                    return Json(ad, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}


