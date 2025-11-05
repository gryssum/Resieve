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
    /// <summary>
    /// Interface for applying filtering to a queryable data source.
    /// </summary>
    public interface IResieveFilterProcessor
    {
        /// <summary>
        /// Applies filtering to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="reSieveModel">The model containing filter parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with filtering applied.</returns>
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    /// <summary>
    /// Provides filtering operations for queryable data sources using ResieveModel.
    /// </summary>
    public class ResieveFilterProcessor(IResieveMapper mapper, IExpressionTreeBuilder expressionTreeBuilder) : IResieveFilterProcessor
    {
        /// <inheritdoc/>
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

            mapper.PropertyMappings.TryGetValue(typeof(TEntity), out var mappedProperties);
            GuardAgainstUnmappedProperties(allowedProperties, mappedProperties!);

            // 4. Build expression tree from tokens
            var customFilters = mappedProperties!.Where(x => x.Value.CanFilter && x.Value.CustomFilter != null)
                .ToDictionary(x => x.Key, x => x.Value);
            var expression = expressionTreeBuilder.BuildFromTokens<TEntity>(tokens, customFilters);
            return source.Where(expression);
        }

        private void GuardAgainstUnmappedProperties(IEnumerable<Token> filterTerms, Dictionary<string, ResievePropertyMap> mappedProperties)
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

                var errorMessage = "Not allowed to filter on these properties.";
                if (unmappedProperties.Any())
                {
                    errorMessage += $"Unmapped properties: {string.Join(", ", unmappedProperties)}.";
                }

                throw new ResieveFilterException(errorMessage);
            }
        }
    }

}