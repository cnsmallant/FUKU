﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class D8MallEntities : DbContext
    {
        public D8MallEntities()
            : base("name=D8MallEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<com_area> com_area { get; set; }
        public DbSet<com_article> com_article { get; set; }
        public DbSet<com_banner> com_banner { get; set; }
        public DbSet<com_img> com_img { get; set; }
        public DbSet<pro_brand> pro_brand { get; set; }
        public DbSet<pro_class> pro_class { get; set; }
        public DbSet<pro_classsub> pro_classsub { get; set; }
        public DbSet<pro_comment> pro_comment { get; set; }
        public DbSet<pro_coupon> pro_coupon { get; set; }
        public DbSet<pro_express> pro_express { get; set; }
        public DbSet<pro_item> pro_item { get; set; }
        public DbSet<pro_newsgoods> pro_newsgoods { get; set; }
        public DbSet<pro_sales> pro_sales { get; set; }
        public DbSet<pro_shipment> pro_shipment { get; set; }
        public DbSet<pro_sku> pro_sku { get; set; }
        public DbSet<pro_skuitem> pro_skuitem { get; set; }
        public DbSet<pro_spe> pro_spe { get; set; }
        public DbSet<pro_spesub> pro_spesub { get; set; }
        public DbSet<shop_car> shop_car { get; set; }
        public DbSet<shop_choose> shop_choose { get; set; }
        public DbSet<shop_invoice> shop_invoice { get; set; }
        public DbSet<shop_order> shop_order { get; set; }
        public DbSet<shop_orderdetail> shop_orderdetail { get; set; }
        public DbSet<shop_pay> shop_pay { get; set; }
        public DbSet<shop_shipping> shop_shipping { get; set; }
        public DbSet<Sms> Sms { get; set; }
        public DbSet<sys_admin> sys_admin { get; set; }
        public DbSet<sys_log> sys_log { get; set; }
        public DbSet<sys_purview> sys_purview { get; set; }
        public DbSet<sys_role> sys_role { get; set; }
        public DbSet<user_address> user_address { get; set; }
        public DbSet<user_bank> user_bank { get; set; }
        public DbSet<user_basic> user_basic { get; set; }
        public DbSet<user_collect> user_collect { get; set; }
        public DbSet<user_coupon> user_coupon { get; set; }
        public DbSet<user_detail> user_detail { get; set; }
        public DbSet<user_info> user_info { get; set; }
        public DbSet<ad_info> ad_info { get; set; }
    }
}
