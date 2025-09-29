using System.Linq;
using Resieve.Filtering;
using Resieve.Pagination;
using Resieve.Sorting;

namespace Resieve
{
    public class ResieveProcessor : IResieveProcessor
    {
        private readonly IResievePaginationProcessor _resievePaginationProcessor;
        private readonly IResieveSortingProcessor _resieveSortingProcessor;
        private readonly IResieveFilterProcessor _resieveFilterProcessor;

        public ResieveProcessor(
            IResieveSortingProcessor resieveSortingProcessor,
            IResieveFilterProcessor resieveFilterProcessor,
            IResievePaginationProcessor resievePaginationProcessor)
        {
            _resieveSortingProcessor = resieveSortingProcessor;
            _resieveFilterProcessor = resieveFilterProcessor;
            _resievePaginationProcessor = resievePaginationProcessor;
        }

        public IQueryable<TEntity> Process<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source)
        {
            // TODO: Exceptions
            // TODO: Settings
            // TODO: Custom Filtering, Sorting, Pagination
            // TODO: Standard PaginatedResult
            // TODO: Merge Included with aggregate
            var filtered = _resieveFilterProcessor.Apply(reSieveModel, source);
            var sorted = _resieveSortingProcessor.Apply(reSieveModel, filtered);
            var paged = _resievePaginationProcessor.Apply(reSieveModel, sorted);
            return paged;
        }
    }

    public interface IResieveProcessor
    {
        IQueryable<TEntity> Process<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }
}