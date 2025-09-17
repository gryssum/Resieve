using System.Linq;

namespace Resieve.Mappings.Interfaces
{
    public interface IResieveCustomSort<TEntity>
    {
        IOrderedQueryable<TEntity> Apply(IQueryable<TEntity> source, string propertyName, bool isDescending);
        IOrderedQueryable<TEntity> ApplyThenBy(IOrderedQueryable<TEntity> source, string propertyName, bool isDescending);
    }   
}