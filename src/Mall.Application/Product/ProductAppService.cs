﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Mall.Domain.Entities;

namespace Mall.Product
{
    #region 1.0 接口
    public interface IProductAppService : IAsyncCrudAppService<ProductDto, int, GetAllProductInput, CreateProductInput, UpdateProductInput>
    {
        //1:添加产品
        //2:作废产品
        //3:展示商品
        //PagedResultDto<ProudctListDto> GetProducts(GetAllProductInput input);
    }

    #endregion

    #region 2.0 具体实现类
    public class ProductAppService : AsyncCrudAppService<Mall_Product, ProductDto, int, GetAllProductInput, CreateProductInput, UpdateProductInput>, IProductAppService
    {

        private IRepository<Mall_Product> _productRepository;
        public ProductAppService(IRepository<Mall_Product> productRepository) : base(productRepository)
        {
            this._productRepository = productRepository;
        }

        /// <summary>
        /// 实现查询条件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Mall_Product> CreateFilteredQuery(GetAllProductInput input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.CategoryId.HasValue && input.CategoryId.Value != 0, u => u.CategoryId.Equals(input.CategoryId.Value))
                .WhereIf(!string.IsNullOrEmpty(input.Name), u => u.Name.Contains(input.Name));
        }
    }
    #endregion
}
