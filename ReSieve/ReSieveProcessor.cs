using System.Linq;
using ReSieve.Mappings;
using ReSieve.Paginations;
using ReSieve.Sorting;

namespace ReSieve
{
    public class ReSieveProcessor
    {
        private readonly ReSieveMapper _mapper;
        private readonly IPaginationProcessor _paginationProcessor = new ReSievePaginationProcessor();
        private readonly ISortingProcessor _sortingProcessor = new ReSieveSortingProcessor();

        public ReSieveProcessor(
            ReSieveMapper mapper,
            ISortingProcessor? sortingProcessor = null,
            IPaginationProcessor? paginationProcessor = null)
        {
            _mapper = mapper;

            if (sortingProcessor != null)
            {
                _sortingProcessor = sortingProcessor;
            }

            if (paginationProcessor != null)
            {
                _paginationProcessor = paginationProcessor;
            }
        }

        public IQueryable<TEntity> Process<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            var sorted = _sortingProcessor.Apply(reSieveModel, source);
            var paged = _paginationProcessor.Apply(reSieveModel, sorted);
            return paged;
        }
    }

}