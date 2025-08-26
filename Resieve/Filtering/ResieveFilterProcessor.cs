using System;
using System.Collections.Generic;
using System.Linq;
using Resieve.Filtering.ExpressionTrees;
using Resieve.Filtering.Lexers;
using Resieve.Mappings;

namespace Resieve.Filtering
{
    public interface IResieveFilterProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ResieveFilterProcessor : IResieveFilterProcessor
    {
        private readonly IResieveMapper _mapper;

        public ResieveFilterProcessor(ResieveMapper mapper)
        {
            _mapper = mapper;
        }

        public IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source)
        {
            if (string.IsNullOrWhiteSpace(reSieveModel.Filters))
            {
                return source; // No filters to apply
            }

            // 1. Tokenize the filter string
            var tokens = FilterLexer.Tokenize(reSieveModel.Filters).ToList();

            // 2. Validate tokens against mapped properties and throw
            var allowedProperties = tokens.Where(x => x.Type == TokenType.Property);
            GuardAgainstUnmappedProperties<TEntity>(allowedProperties);

            // 3. Filter out custom properties not mapped for filtering

            // 4. Build expression tree from tokens
            var expression = ExpressionTreeBuilder.BuildFromTokens<TEntity>(tokens);

            return source.Where(expression);
        }

        private void GuardAgainstUnmappedProperties<TEntity>(IEnumerable<Token> filterTerms)
        {
            _mapper.PropertyMappings.TryGetValue(typeof(TEntity), out var mappedProperties);

            if (mappedProperties is null || !filterTerms
                    .All(x =>
                        mappedProperties.Keys.Any(y =>
                            y.Equals(x.Value, StringComparison.OrdinalIgnoreCase) && mappedProperties[y].CanFilter)))
            {
                throw new ArgumentException("Not allowed to sort on this entity.");
            }
        }
    }

}