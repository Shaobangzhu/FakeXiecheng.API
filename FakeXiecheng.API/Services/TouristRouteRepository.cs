using FakeXiecheng.API.Database;
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

        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get a specific picture from tourist picture list by its ID
        public TouristRoutePicture GetPicture(int pictureId)
        {
            return _context.TouristRoutePictures.Where(p => p.Id == pictureId).FirstOrDefault();
        }

        // Get picture list by tourist route ID
        public IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId)
        {
            return _context.TouristRoutePictures
                .Where(p => p.TouristRouteId == touristRouteId).ToList(); // p represent picture
        }

        // Get specific tourist route by its tourist route ID
        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
            return _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefault(n => n.Id == touristRouteId);
        }

        // Get the tourist route list
        public IEnumerable<TouristRoute> GetTouristRoutes(string keyword, string ratingOperator, int? ratingValue)
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

            return result.ToList();

            // Include is used to connect two different tables.(Eager Load)
            // We use Include here, because in the front page, when we want to demo a recommended tourist route, we want to also show its pictures.
            //return _context.TouristRoutes.Include(t => t.TouristRoutePictures);
        }

        // If a specific tourist route exists
        public bool TouristRouteExists(Guid touristRouteId)
        {
            return _context.TouristRoutes.Any(t => t.Id == touristRouteId); // t represent tourist route
        }

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

        // 将数据写入数据库
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
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

        // 通过多个ID获得一组tourist routes
        public IEnumerable<TouristRoute> GetTouristRoutesByIDList(IEnumerable<Guid> ids)
        {
            return _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToList();
        }

        // 通过多个ID删除一组tourist routes
        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }
    }
}
