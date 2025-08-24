using System;
using System.Collections.Generic;
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

            // 2. Validate tokens against mapped properties and throw
            var allowedProperties = tokens.Where(x => x.Type == TokenType.Property);
            GuardAgainstUnmappedProperties<TEntity>(allowedProperties);
            
            // 3. Filter out custom properties not mapped for filtering

            // 4. Build expression tree from tokens
            var expressionBuilder = new ExpressionTreeBuilder();
            var expression = expressionBuilder.BuildFromTokens<TEntity>(tokens);

            return source.Where(expression);
        }
        
        private void GuardAgainstUnmappedProperties<TEntity>(IEnumerable<Token> filterTerms)
        {
            _mapper.PropertyMappings.TryGetValue(typeof(TEntity), out var mappedProperties);
            
            if (mappedProperties is null)
            {
                throw new ArgumentException("Not allowed to sort on this entity.");
            }

            if(!filterTerms.All(x => mappedProperties.Keys.Any(y => y.Equals(x.Value, StringComparison.OrdinalIgnoreCase) && mappedProperties[y].CanFilter)))
            {
                throw new ArgumentException("Not allowed to sort on this entity.");
            }
        }
    }

}