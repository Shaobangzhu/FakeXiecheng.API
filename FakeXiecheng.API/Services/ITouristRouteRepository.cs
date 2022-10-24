using FakeXiecheng.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Services
{
    public interface ITouristRouteRepository
    {
        IEnumerable<TouristRoute> GetTouristRoutes(string keyword, string ratingOperator, int? ratingValue); // Get all the tourist routes
        IEnumerable<TouristRoute> GetTouristRoutesByIDList(IEnumerable<Guid> ids); // Get the tourist routes by IDs
        TouristRoute GetTouristRoute(Guid touristRouteId); // Get a specific tourist route through a route ID
        bool TouristRouteExists(Guid touristRouteId); // If one specific tourist route exists
        IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId); // Get tourist route picture by a specific ID
        TouristRoutePicture GetPicture(int pictureId); // Get a specific picture of a route from tourist route picture list
        void AddTouristRoute(TouristRoute touristRoute); // Add one tourist route to the repo
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture); // Add one tourist route picture to the repo
        bool Save(); // Write data into database
        void DeleteTouristRoute(TouristRoute touristRoute); // Delete a specific tourist route
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture); // Delete a specific tourist route picture
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes); // Delete a bunch of routes by Ids
    }
}
