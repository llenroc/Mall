﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;

using Mall.Domain.Entities;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Mall.Extend;

namespace Mall.Cart
{

    #region 1.0 抽象接口 ICartAppService
    /// <summary>
    /// 购物车信息
    /// </summary>
    public interface ICartAppService
    {
        /// <summary>
        /// 获取购物车中的未完结产品的数量
        /// </summary>
        /// <returns></returns>
        Task<int> GetCartProjectNum();
        /// <summary>
        /// 1:向购物车中添加商品
        /// 2:返回当前购物车中的商品数量
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        Task<int> AddProductToCart(CartItemInput itemDto);
        /// <summary>
        /// 2:删除购物车中的东西
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task DelProduct(int productId);
        /// <summary>
        /// 3:更新商品
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        Task<bool> UpdateProductNums(CartItemInput itemDto);

        /// <summary>
        /// 付款结账
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        Task<bool> CheckOut();

        /// <summary>
        /// 获取当前人员的购物车信息
        /// </summary>
        /// <returns></returns>
        Task<PagedResultDto<CartItemDto>> GetMyCart(PageRequestDto input);

        /// <summary>
        /// 通过购物车的唯一标识来获取
        /// </summary>
        /// <returns></returns>
        Task<PagedResultDto<CartItemDto>> GetCartById(int cartId);
    }
    #endregion

    #region 2.0 具体实现 CartAppService
    /// <summary>
    /// 购物车
    /// </summary>
    public class CartAppService : MallAppServiceBase, ICartAppService
    {

        private IRepository<Mall_Cart> _cartRepository;
        private IRepository<Mall_CartItem> _cartItemRepository;
        private IRepository<Mall_Product> _productRepository;
        private IRepository<Mall_Order> _orderRepository;

        public CartAppService(IRepository<Mall_Cart> cartRepository,
                            IRepository<Mall_CartItem> cartItemRepository,
                            IRepository<Mall_Product> productRepository,
                            IRepository<Mall_Order> orderRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;

        }

        private async Task<Mall_Cart> GetCurrentCart()
        {
            return await _cartRepository.FirstOrDefaultAsync(u => u.ItemStatus == CartStatus.Init && u.CreatorUserId.Value.Equals(UserId));
        }

        /// <summary>
        /// 自动生成订单编号
        /// </summary>
        /// <returns></returns>
        private string GenerateOrderNo()
        {
            //var orders = await _orderRepository.GetAll().Where(u=>u.CreationTime.)
            //1：通过日期来
            return $"MO_{UserId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        }


        public async Task<int> AddProductToCart(CartItemInput itemDto)
        {
            //1:找到产品
            var product = await _productRepository.FirstOrDefaultAsync(u => u.Id.Equals(itemDto.ProductId));

            //2:找到购物车
            var cart = await GetCurrentCart();
            if (cart == null)
            {
                cart = new Mall_Cart();
                await _cartRepository.InsertAsync(cart);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            
            //3:检查购物车中是否已经存在该物品,如果存在,那么给该物品的数量添加1
            var cartItem = await _cartItemRepository.FirstOrDefaultAsync(u => u.ProductId.Equals(itemDto.ProductId) && u.CartId.Equals(cart.Id));
            if (cartItem != null)
            {
                cartItem.ItemNum++;
            }
            else
            {
                ObjectMapper.Map<Mall_CartItem>(itemDto);
                cartItem.CartId = cart.Id;
                cartItem.ItemPrice = product.Price;
                await _cartItemRepository.InsertAsync(cartItem);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            //4:获取购物中产品的数量
            var productNum = _cartItemRepository.GetAll().Where(u => u.CartId.Equals(cart.Id)).Count();
            return await Task.FromResult(productNum);
        }

        public async Task DelProduct(int productId)
        {
            //1：找到购物车
            var cart = await GetCurrentCart();
            //2：找到商品
            var cartItem = await _cartItemRepository.FirstOrDefaultAsync(u => u.CartId.Equals(cart.Id) && u.ProductId.Equals(productId));
            //3：删除商品
            await _cartItemRepository.DeleteAsync(cartItem);
        }

        public async Task<bool> UpdateProductNums(CartItemInput itemDto)
        {
            //1：找到购物车
            var cart = await GetCurrentCart();
            //2：找到商品
            var cartItem = await _cartItemRepository.FirstOrDefaultAsync(u => u.CartId.Equals(cart.Id) && u.ProductId.Equals(itemDto.ProductId));
            cartItem.ItemNum = itemDto.ItemNum;
            //3：更新商品
            return await _cartItemRepository.UpdateAsync(cartItem) != null;
        }

        public async Task<bool> CheckOut()
        {
            //1:获取购物车
            var cart = await GetCurrentCart();
            //2：设置状态
            cart.ItemStatus = CartStatus.Pay;
            //3：生成订单
            Mall_Order order = new Mall_Order();
            order.CartId = cart.Id;
            order.OrderStatus = OrderStatus.Init;
            //订单编号,自动生成
            order.OrderNo = GenerateOrderNo();
            //计算该订单的总金额
            order.AllPrice = _cartItemRepository.GetAll().Where(u => u.CartId.Equals(cart.Id)).Select(u => u.ItemNum * u.ItemPrice).Sum();
            //4：没有订单
            await _orderRepository.InsertAsync(order);
            //5：更新采购
            await _cartRepository.UpdateAsync(cart);
            return await Task.FromResult(true);
        }


        public async Task<int> GetCartProjectNum()
        {
            //1:获取购物车
            var cart = await GetCurrentCart();
            if (cart == null)
            {
                return await Task.FromResult(0);
            }
            var cartId = cart.Id;
            //2:通过购物车来找到产品数量
            var productNum = _cartItemRepository.GetAll().Where(u => u.CartId.Equals(cartId)).Count();
            //3:返回当前购物车中的产品数量
            return await Task.FromResult(productNum);
        }

        public async Task<PagedResultDto<CartItemDto>> GetMyCart(PageRequestDto input)
        {
            var cart = await GetCurrentCart();
            int cartId = cart.Id;
            //获取数据集
            var result = _cartItemRepository.GetAll().Where(u => u.CartId.Equals(cartId)).Skip(input.SkipCount).Take(input.Limit);
            //返回结果集
            return new PagedResultDto<CartItemDto>() { Items = result.MapTo<List<CartItemDto>>(), TotalCount = result.Count() };
        }

        public async Task<PagedResultDto<CartItemDto>> GetCartById(int cartId)
        {
            var carts = _cartItemRepository.GetAll().Where(u => u.CartId.Equals(cartId)).ToList();
            //返回结果集
            return await Task.FromResult(new PagedResultDto<CartItemDto>() { Items = carts.MapTo<List<CartItemDto>>(), TotalCount = carts.Count() });
        }
    }
    #endregion
}
