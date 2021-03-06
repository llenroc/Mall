﻿using Abp.EntityFrameworkCore;
using Mall.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mall.EntityFrameworkCore
{
    public class MallDbContext : AbpDbContext
    {
        //Add DbSet properties for your entities...
        #region 1.0  Add DbSet properties for your entities...
        /// <summary>
        /// 购物车
        /// </summary>
        public virtual DbSet<Mall_Cart> Mall_Cart { get; set; }
        /// <summary>
        /// 购物车明细
        /// </summary>
        public virtual DbSet<Mall_CartItem> Mall_CartItem { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public virtual DbSet<Mall_Order> Mall_Order { get; set; }

        public virtual DbSet<Mall_Category> Mall_Category { get; set; }
        /// <summary>
        /// 商品信息
        /// </summary>
        public virtual DbSet<Mall_Product> Mall_Product { get; set; }

        public virtual DbSet<Mall_Account> Mall_Account { get; set; }
        #endregion

        public MallDbContext(DbContextOptions<MallDbContext> options) 
            : base(options)
        {

        }
    }
}
