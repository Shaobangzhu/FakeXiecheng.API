﻿using FakeXiecheng.API.Database;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        // DATABASE: 上下文关系对象
        private readonly AppDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRouteRepository(
            AppDbContext context,
            IPropertyMappingService propertyMappingService
        )
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        #region 所有的异步操作
        // Get a specific picture from tourist picture list by its ID
        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.Where(p => p.Id == pictureId).FirstOrDefaultAsync();
        }

        // Get picture list by tourist route ID
        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures
                .Where(p => p.TouristRouteId == touristRouteId).ToListAsync(); // p represent picture
        }

        // Get specific tourist route by its tourist route ID
        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }

        // Get the tourist route list (PAGINATION ENABLED)
        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword, 
            string ratingOperator, 
            int? ratingValue,
            int pageSize,
            int pageNumber,
            string orderBy
        )
        {
            IQueryable<TouristRoute> result = _context
                .TouristRoutes
                .Include(t => t.TouristRoutePictures);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword)); // t means tourist route
            }

            if (ratingValue >= 0)
            {
                result = ratingOperator switch
                {
                    "largerThan" => result.Where(t => t.Rating > ratingValue),
                    "lessThan" => result.Where(t => t.Rating < ratingValue),
                    _ => result.Where(t => t.Rating == ratingValue),
                };
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                #region HARDCODED价格排序
                //if(orderBy.ToLowerInvariant() == "originalprice")
                //{
                //    result = result.OrderBy(t => t.OriginalPrice);
                //}
                #endregion

                var touristRouteMappingDictionary = _propertyMappingService
                    .GetPropertyMapping<TouristRouteDto, TouristRoute>();

                result = result.ApplySort(orderBy, touristRouteMappingDictionary);
            }

            #region PAGINATION(MOVED TO PAGINATIONLIST IN HELPER)
            // 1.跳过一定量的数据
            //var skip = (pageNumber - 1) * pageSize;
            //result = result.Skip(skip);
            // 2.以pagesize为标准, 显示一定量的数据
            //result = result.Take(pageSize);
            #endregion

            return await PaginationList<TouristRoute>.CreateAsync(pageNumber, pageSize, result);

            // Include is used to connect two different tables.(Eager Load)
            // We use Include here, because in the front page, when we want to demo a recommended tourist route, we want to also show its pictures.
            //return _context.TouristRoutes.Include(t => t.TouristRoutePictures);
        }

        // 通过多个ID获得一组tourist routes
        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        // If a specific tourist route exists
        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId); // t represent tourist route
        }

        // 将数据写入数据库
        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        // 通过Id来获取用户的购物车信息
        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User)
                .Include(s => s.ShoppingCartItems).ThenInclude(li => li.TouristRoute)
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }

        // 创建用户购物车
        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        // 添加商品到购物车
        public async Task AddShoppingCartItem(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }

        // 获取购物车中商品的ID
        public async Task<LineItem> GetShoppingCartItemByItemId(int lineItemId)
        {
            return await _context.LineItems
                .Where(li => li.Id == lineItemId)
                .FirstOrDefaultAsync();
        }

        // 通过购物车商品ID列表获取商品列表
        public async Task<IEnumerable<LineItem>> GetShoppingCartsByIdListAsync(IEnumerable<int> ids)
        {
            return await _context.LineItems
                .Where(li => ids.Contains(li.Id))
                .ToListAsync();
        }

        // 添加新订单
        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        // 通过用户ID获取订单
        public async Task<PaginationList<Order>> GetOrdersByUserId(
            string userId, int pageSize, int pageNumber)
        {
            //return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
            IQueryable<Order> result = _context.Orders.Where(o => o.UserId == userId);
            return await PaginationList<Order>.CreateAsync(pageNumber, pageSize, result);
        }

        // 通过订单ID获取订单
        public async Task<Order> GetOrderById(Guid orderId)
        {
            // o: order oi: orderItem
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.TouristRoute)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }
        #endregion

        // 向上下文关系对象AppDbContext中添加tourist route数据
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
        }

        // 向上下文关系对象AppDbContext中添加tourist route picture数据
        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRouteId));
            }
            if (touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        // 将tourist route数据从数据库中删除
        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        // 将tourist route picture数据从数据库中删除
        public void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture)
        {
            _context.TouristRoutePictures.Remove(touristRoutePicture);
        }

        // 通过多个ID删除一组tourist routes
        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        // 删除购物车商品
        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }

        // 批量删除购物车商品
        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }
    }
}
