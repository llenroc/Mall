﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Mall.Domain.Entities;

namespace Mall.Cart
{

    public class CartItemDto : CartItemInput, IEntityDto<int>
    {
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public string Name { get; set; }


        public string ItemSpecs { get; set; }
        /// <summary>
        /// 产品价格
        /// </summary>
        public decimal ItemPrice { get; set; }

    }

    [AutoMapTo(typeof(Mall_CartItem))]
    public class CartItemInput
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ItemNum { get; set; }
        
    }


    
}
