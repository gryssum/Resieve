using System.Linq;
using ReSieve.Models;

namespace ReSieve.Services
{
    public class ReSieveProcessor
    {
        private readonly ReSieveMapper _mapper;
        private readonly IPaginationProcessor _paginationProcessor= new DefaultPaginationProcessor();

        public ReSieveProcessor(ReSieveMapper mapper, IPaginationProcessor? paginationProcessor = null)
        {
            _mapper = mapper;
            
            if (paginationProcessor != null)
            {
                _paginationProcessor = paginationProcessor;
            }
        }

        public IQueryable<TEntity> Process<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            return _paginationProcessor.Apply(reSieveModel, source);
        }
    }
   
}