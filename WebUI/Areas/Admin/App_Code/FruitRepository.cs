using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFClassLibrary;


    public class FruitRepository
    {

      
        /// <summary>
        /// for get fruit for specific id
        /// </summary>
        public static Fruit Get(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id.Equals(id));
        }

        /// <summary>
        /// for get all fruits
        /// </summary>
        public static IEnumerable<Fruit> GetAll()
        {
            D8MallEntities db = new D8MallEntities();
            var query = db.sys_purview;
            IList<sys_purview> list = query.OrderBy(s => s.sys_purview_id).ToList();
            IList<Fruit> flist = new List<Fruit>();
         
            foreach (var item in list)
            {
                Fruit f = new Fruit();
                f.Id = item.sys_purview_id;
                f.Name = item.sys_purview_name;
                flist.Add(f);
            }
            return flist;
        }
    }
