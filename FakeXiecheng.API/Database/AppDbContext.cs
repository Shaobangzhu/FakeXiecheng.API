using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser> // DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        #region 指明有哪些模型需要映射到数据库中
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Mock Tourist Route Data(COMMENTED)
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //    Id = Guid.NewGuid(),
            //    Title="ceshititle",
            //    Description="shuoming",
            //    OriginalPrice=0,
            //    CreateTime=DateTime.UtcNow
            //});
            #endregion

            // Add tourist route data
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            // Add tourist route picture data
            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutesPictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutesPictures);

            #region 初始化用户与角色的种子数据

            // 1.更新用户与角色的外键
            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles)
                .WithOne().HasForeignKey(ur => ur.UserId).IsRequired()
            );

            // 2.添加管理员角色
            var adminRoleId = "308660dc-ae51-480f-824d-7dca6714c3e2";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            );

            // 3.添加用户
            var adminUserId = "90184155-dee0-40c9-bb1e-b5ed07afc04e";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@fakexiecheng.com",
                NormalizedUserName = "admin@fakexiecheng.com".ToUpper(),
                Email = "admin@fakexiecheng.com",
                NormalizedEmail = "admin@fakexiecheng.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false
            };
            var ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // 4.给用户加入管理员角色
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
