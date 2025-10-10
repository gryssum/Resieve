using System.Linq;
using Resieve.Filtering;
using Resieve.Pagination;
using Resieve.Sorting;

namespace Resieve
{
    public class ResieveProcessor(
        IResieveSortingProcessor resieveSortingProcessor,
        IResieveFilterProcessor resieveFilterProcessor,
        IResievePaginationProcessor resievePaginationProcessor
    ) : IResieveProcessor
    {
        public IQueryable<TEntity> Process<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source)
        {
            // TODO: Exceptions
            // TODO: Settings
            // TODO: Standard PaginatedResult
            var filtered = resieveFilterProcessor.Apply(reSieveModel, source);
            var sorted = resieveSortingProcessor.Apply(reSieveModel, filtered);
            var paged = resievePaginationProcessor.Apply(reSieveModel, sorted);
            return paged;
        }
    }

    public interface IResieveProcessor
    {
        IQueryable<TEntity> Process<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }
}