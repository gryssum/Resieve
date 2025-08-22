using System.Linq;
using ReSieve.Filtering.ExpressionTrees;
using ReSieve.Filtering.Lexers;
using ReSieve.Mappings;

namespace ReSieve.Filtering
{
    public interface IFilterProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ReSieveFilterProcessor : IFilterProcessor
    {
        private readonly ReSieveMapper _mapper;

        public ReSieveFilterProcessor(ReSieveMapper mapper)
        {
            _mapper = mapper;
        }

        public IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            if (string.IsNullOrWhiteSpace(reSieveModel.Filters))
            {
                return source; // No filters to apply
            }

            // 1. Tokenize the filter string
            var lexer = new FilterLexer();
            var tokens = lexer.Tokenize(reSieveModel.Filters).ToList();

            // 2. Validate tokenns against mapped properties and throw
            
            // 3. Filter out custom properties not mapped for filtering
            
            // 4. Build expression tree from tokens
            var expressionBuilder = new ExpressionBuilder();
            var expression = expressionBuilder.BuildExpressionFromTokens<TEntity>(tokens);

            return source.Where(expression);
        }
    }

}