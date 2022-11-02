using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public interface ITouristRouteRepository
    {
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string keyword, string ratingOperator, int? ratingValue, int pageSize, int pageNumber); // Get all the tourist routes
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids); // Get the tourist routes by IDs
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId); // Get a specific tourist route through a route ID
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId); // If one specific tourist route exists
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId); // Get tourist route picture by a specific ID
        Task<TouristRoutePicture> GetPictureAsync(int pictureId); // Get a specific picture of a route from tourist route picture list
        Task<bool> SaveAsync(); // Write data into database
        Task<ShoppingCart> GetShoppingCartByUserId(string userId); // 通过Id获得用户的购物车
        Task CreateShoppingCart(ShoppingCart shoppingCart); // 创建购物车
        Task AddShoppingCartItem(LineItem lineItem); // 往购物车里添加商品
        Task<LineItem> GetShoppingCartItemByItemId(int lineItemId); // 通过商品ID获取购物车中的商品
        Task<IEnumerable<LineItem>> GetShoppingCartsByIdListAsync(IEnumerable<int> ids); // 通过购物车商品ID列表获取商品列表
        Task<PaginationList<Order>> GetOrdersByUserId(string userId, int pageSize, int pageNumber); // 通过用户的ID获得用户订单
        Task<Order> GetOrderById(Guid orderId); // 通过订单的ID获取订单
        Task AddOrderAsync(Order order); // 添加订单
        void AddTouristRoute(TouristRoute touristRoute); // Add one tourist route to the repo
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture); // Add one tourist route picture to the repo
        void DeleteTouristRoute(TouristRoute touristRoute); // Delete a specific tourist route
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture); // Delete a specific tourist route picture
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes); // Delete a bunch of routes by Ids
        void DeleteShoppingCartItem(LineItem lineItem); // 删除购物车商品
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems); // 批量删除购物车商品
    }
}
