//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFClassLibrary
{
    using System;
    using System.Collections.Generic;
    
    public partial class pro_sales
    {
        public int pro_sales_id { get; set; }
        public string pro_sku_code { get; set; }
        public Nullable<System.DateTime> pro_sales_stadate { get; set; }
        public Nullable<System.DateTime> pro_sales_enddate { get; set; }
        public string pro_sales_stastus { get; set; }
        public Nullable<int> pro_sales_order { get; set; }
        public Nullable<decimal> pro_sales_price { get; set; }
    }
}
