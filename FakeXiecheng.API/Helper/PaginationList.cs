using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Helper
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get; private set; } // 页面总量
        public int TotalCount { get; private set; } // 数据库的总数据量
        public bool HasPrevious => CurrentPage > 1; // 用来判断是否有上一页
        public bool HasNext => CurrentPage < TotalPages; // 用来判断是否有下一页

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        #region CONSTRUCTOR

        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        #endregion

        #region 创建PAGINATIOn(工厂模式)

        public static async Task<PaginationList<T>> CreateAsync(
            int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync(); // 第一次访问数据库来获取数据的总量

            // PAGINATION
            // 1.跳过一定量的数据
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            // 2.以pagesize为标准, 显示一定量的数据
            result = result.Take(pageSize);

            var item = await result.ToListAsync(); //第二次访问数据库来获得数据的列表

            return new PaginationList<T>(totalCount, currentPage, pageSize, item);
        }

        #endregion
    }
}
