using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Resieve.Exceptions;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;

namespace Resieve.Sorting
{
    public interface IResieveSortingProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ResieveSortingProcessor : IResieveSortingProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IResieveMapper _mapper;
        
        public ResieveSortingProcessor(IServiceProvider serviceProvider, IResieveMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }
        
        public IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source)
        {
            var sortTerms = ResieveSortParser.ParseSorts(reSieveModel.Sorts);

            if (sortTerms.Count == 0)
            {
                return source;
            }
            
            _mapper.PropertyMappings.TryGetValue(typeof(TEntity), out var mappedProperties);
            GuardAgainstUnmappedProperties(sortTerms, mappedProperties);

            IOrderedQueryable<TEntity>? ordered = null;
            var registeredCustomSorts = _serviceProvider.GetService<IEnumerable<IResieveCustomSort<TEntity>>>()?.ToList() ?? new List<IResieveCustomSort<TEntity>>();
            
            for (var i = 0; i < sortTerms.Count; i++)
            {
                var sortTerm = sortTerms[i];

                var hasCustomSort = mappedProperties!.Single(x => x.Key.Equals(sortTerm.Name, StringComparison.OrdinalIgnoreCase)).Value;

                if (hasCustomSort.CustomSort != null)
                {
                    var customFilter = registeredCustomSorts.SingleOrDefault(x => x.GetType() == hasCustomSort.CustomSort);

                    if (customFilter == null)
                    {
                        throw new ResieveSortingException("");
                    }
                    
                    ordered = i == 0 ? 
                        customFilter.Apply(source, sortTerm.Name, sortTerm.Descending) : 
                        customFilter.ApplyThenBy(ordered!, sortTerm.Name, sortTerm.Descending);
                }
                else
                {
                    var lambda = CreateLambda<TEntity>(sortTerm.Name);
                    if (i == 0)
                    {
                        ordered = sortTerm.Descending
                            ? source.OrderByDescending(lambda)
                            : source.OrderBy(lambda);
                    }
                    else
                    {
                        ordered = sortTerm.Descending
                            ? ordered!.ThenByDescending(lambda)
                            : ordered!.ThenBy(lambda);
                    }
                }
            }

            return ordered ?? source;
        }

        private void GuardAgainstUnmappedProperties(List<ISortTerm> sortTerms, Dictionary<string, ResievePropertyMap>? mappedProperties)
        {
            if (mappedProperties is null)
            {
                throw new ResieveSortingException("Not allowed to sort on this entity.");
            }

            if(!sortTerms.All(x => mappedProperties.Keys.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase) && mappedProperties[y].CanSort)))
            {
                var unmappedProperties = sortTerms
                    .Where(x => !mappedProperties.Keys.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                    .Select(x => x.Name)
                    .ToList();
                
                var errorMessage = "Not allowed to sort on these properties";
                if (unmappedProperties.Any())
                {
                    errorMessage += $"Unmapped properties: {string.Join(", ", unmappedProperties)}.";
                }

                throw new ResieveSortingException(errorMessage);
            }
        }

        private static Expression<Func<TEntity, object>> CreateLambda<TEntity>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            Expression body = Expression.PropertyOrField(param, propertyName);
            if (body.Type.IsValueType)
            {
                body = Expression.Convert(body, typeof(object));
            }

            return Expression.Lambda<Func<TEntity, object>>(body, param);
        }
    }
}