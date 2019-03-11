
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using EFClassLibrary;
using System.Xml.Linq;
using System.Xml;

namespace WebUI.Controllers
{
    public class XMLController : Controller
    {
        //
        // GET: /XML/
        D8MallEntities db = new D8MallEntities();
        public ActionResult Index()
        {
            var list = db.com_area.ToList();
            ViewBag.xml = CollectionToXml(list);
            return View();
        }
        #region 序列化XML文件

        public static string CollectionToXml(IList<com_area> config)
        {
            string xml = "<?xml version='1.0' encoding='utf-8' ?><root> ";
            var province = config.Where(p => p.com_area_lev == "1").ToList();
            foreach (var item in province)
            {
              
                xml += "<key>" + item.com_area_id + "</key>";
                xml += "<dict>";
                xml += "<key>" + item.com_area_name + "</key>";
                xml += "<dict>";
                string com_area_id = item.com_area_id.ToString();
                var city = config.Where(c => c.com_area_parentid == com_area_id).ToList();
                foreach (var item01 in city)
                {
                    string com_area_id01 = item01.com_area_id.ToString();
                
                   
                    xml += "<key>" + item01.com_area_id + "</key>";
                    xml += "<dict>";
                    xml += "<key>" + item01.com_area_name + "</key>";
                    xml += "<dict>";
                    var county = config.Where(c => c.com_area_parentid == com_area_id01).ToList();
                    foreach (var item02 in county)
                    {

                      
                        xml += "<dict>";
                        xml += "<key>" + item02.com_area_id + "</key>";

                        xml += "<string>" + item02.com_area_name + "</string>";

                        xml += "</dict>";
                     
                    }
                    xml += "</dict>";
                    xml += "</dict>";
                 
                 
                }
                xml += "</dict>";
                xml += "</dict>";
              
            }
            xml += "</root>";
            return xml;
        }

        #endregion
    }
}
