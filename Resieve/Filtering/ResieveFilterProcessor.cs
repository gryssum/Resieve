using System;
using System.Collections.Generic;
using System.Linq;
using Resieve.Exceptions;
using Resieve.Filtering.ExpressionTrees;
using Resieve.Filtering.Lexers;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;

namespace Resieve.Filtering
{
    public interface IResieveFilterProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ResieveFilterProcessor : IResieveFilterProcessor
    {
        private readonly IResieveMapper _mapper;
        private readonly IExpressionTreeBuilder _expressionTreeBuilder;

        public ResieveFilterProcessor(ResieveMapper mapper, IExpressionTreeBuilder expressionTreeBuilder)
        {
            _mapper = mapper;
            _expressionTreeBuilder = expressionTreeBuilder;
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

            _mapper.PropertyMappings.TryGetValue(typeof(TEntity), out var mappedProperties);
            GuardAgainstUnmappedProperties<TEntity>(allowedProperties, mappedProperties!);

            // 4. Build expression tree from tokens
            var customFilters = mappedProperties!.Where(x => x.Value.CanFilter && x.Value.CustomFilter != null)
                .ToDictionary(x => x.Key, x => x.Value);
            var expression = _expressionTreeBuilder.BuildFromTokens<TEntity>(tokens, customFilters);
            return source.Where(expression);
        }

        private void GuardAgainstUnmappedProperties<TEntity>(IEnumerable<Token> filterTerms, Dictionary<string, ResievePropertyMap> mappedProperties)
        {
            if (mappedProperties == null)
            {
                throw new ResieveFilterException("Not allowed to filter on this entity.");
            }

            var filterTermsList = filterTerms.ToList();
            if (!filterTermsList
                    .All(x =>
                        mappedProperties.Keys.Any(y =>
                            y.Equals(x.Value, StringComparison.OrdinalIgnoreCase) && mappedProperties[y].CanFilter)))
            {
                var unmappedProperties = filterTermsList
                    .Where(x => !mappedProperties.Keys.Any(y => y.Equals(x.Value, StringComparison.OrdinalIgnoreCase)))
                    .Select(x => x.Value)
                    .ToList();

                var errorMessage = "Not allowed to filter on this entity.";
                if (unmappedProperties.Any())
                {
                    errorMessage += $"Unmapped properties: {string.Join(", ", unmappedProperties)}.";
                }

                throw new ResieveFilterException(errorMessage);
            }
        }
    }

}