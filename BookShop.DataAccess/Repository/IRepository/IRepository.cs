using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category
        // Find Category
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null,bool tracked = true);

        // For Listing
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        //For Add item
        void Add(T entity);
        
        // For Remove Items
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);

    }
}
