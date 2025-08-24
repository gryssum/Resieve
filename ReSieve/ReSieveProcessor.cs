using System.Linq;
using ReSieve.Filtering;
using ReSieve.Mappings;
using ReSieve.Pagination;
using ReSieve.Sorting;

namespace ReSieve
{
    public class ReSieveProcessor
    {
        private readonly ReSieveMapper _mapper;
        private readonly IPaginationProcessor _paginationProcessor = new ReSievePaginationProcessor();
        private readonly ISortingProcessor _sortingProcessor;
        private readonly IFilterProcessor _filterProcessor;

        public ReSieveProcessor(
            ReSieveMapper mapper,
            ISortingProcessor? sortingProcessor = null,
            IFilterProcessor? filterProcessor = null,
            IPaginationProcessor? paginationProcessor = null)
        {
            _mapper = mapper;

            _sortingProcessor = sortingProcessor ?? new ReSieveSortingProcessor(_mapper);
            _filterProcessor = filterProcessor ?? new ReSieveFilterProcessor(_mapper);
            
            if (paginationProcessor != null)
            {
                _paginationProcessor = paginationProcessor;
            }
        }

        public IQueryable<TEntity> Process<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            var filtered = _filterProcessor.Apply(reSieveModel, source);
            var sorted = _sortingProcessor.Apply(reSieveModel, filtered);
            var paged = _paginationProcessor.Apply(reSieveModel, sorted);
            return paged;
        }
    }

}