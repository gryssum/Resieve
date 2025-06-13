using System.Linq;
using ReSieve.Models;

namespace ReSieve.Services
{
    public interface IPaginationProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class DefaultPaginationProcessor : IPaginationProcessor
    {
        public IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            int page = reSieveModel.Page;
            int pageSize = reSieveModel.PageSize;

            // Treat page <= 0 as 1
            if (page <= 0)
            {
                page = 1;
            }

            // Treat pageSize <= 0 as 'all'
            if (pageSize <= 0)
            {
                return source;
            }

            int skip = (page - 1) * pageSize;
            return source.Skip(skip).Take(pageSize);
        }
    }
}