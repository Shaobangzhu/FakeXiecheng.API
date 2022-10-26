using FakeXiecheng.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public interface ITouristRouteRepository
    {
        Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword, string ratingOperator, int? ratingValue); // Get all the tourist routes
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids); // Get the tourist routes by IDs
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId); // Get a specific tourist route through a route ID
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId); // If one specific tourist route exists
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId); // Get tourist route picture by a specific ID
        Task<TouristRoutePicture> GetPictureAsync(int pictureId); // Get a specific picture of a route from tourist route picture list
        Task<bool> SaveAsync(); // Write data into database
        void AddTouristRoute(TouristRoute touristRoute); // Add one tourist route to the repo
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture); // Add one tourist route picture to the repo
        void DeleteTouristRoute(TouristRoute touristRoute); // Delete a specific tourist route
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture); // Delete a specific tourist route picture
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes); // Delete a bunch of routes by Ids
    }
}
