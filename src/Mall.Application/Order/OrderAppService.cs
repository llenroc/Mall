﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Mall.Domain.Entities;

namespace Mall.Order
{
    #region 1.0 接口 IOrderAppService
    public interface IOrderAppService
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<OrderDto>> GetMyOrders(GetAllOrderInput input);
        /// <summary>
        /// 确认订单
        /// </summary>
        /// <returns></returns>
        Task<Mall_Order> AcceptOrder(int orderId);


    }
    #endregion

    #region 2.0 实现 OrderAppService
    /// <summary>
    /// 订单处理类
    /// </summary>
    public class OrderAppService : MallAppServiceBase, IOrderAppService
    {
        private IRepository<Mall_Order> _orderRepository;
        public OrderAppService(IRepository<Mall_Order> orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        /// <summary>
        /// 获取我的订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OrderDto>> GetMyOrders(GetAllOrderInput input)
        {

            var myOrders = _orderRepository.GetAll().Where(u => u.CreatorUserId.Equals(UserId));
            //1:处理OrderStatus
            myOrders = myOrders.WhereIf(input.OrderStatus.HasValue, u => u.OrderStatus.Equals(input.OrderStatus.Value));
            //2:获取对数据进行排序和分页处理
            myOrders = myOrders.OrderByDescending(u => u.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount);

            return await Task.FromResult(new PagedResultDto<OrderDto>() { TotalCount = myOrders.Count(), Items = myOrders.MapTo<List<OrderDto>>() });
        }

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Mall_Order> AcceptOrder(int orderId)
        {
            //1.找到订单
            var order = await _orderRepository.FirstOrDefaultAsync(u => u.Id.Equals(orderId));
            //2.更新订单状态
            order.OrderStatus = OrderStatus.Complete;
            //3.更新
            return await _orderRepository.UpdateAsync(order);
        }
    }
    #endregion

}
