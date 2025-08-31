using System.Linq;

namespace Resieve.Mappings.Interfaces
{
    public interface IResieveCustomSort
    {
        IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> source, string propertyName, bool descending);
    }   
}