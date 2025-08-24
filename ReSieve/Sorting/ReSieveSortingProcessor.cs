using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReSieve.Mappings;

namespace ReSieve.Sorting
{
    public interface ISortingProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ReSieveSortingProcessor : ISortingProcessor
    {
        private readonly ReSieveMapper _mapper;
        
        public ReSieveSortingProcessor(ReSieveMapper mapper)
        {
            _mapper = mapper;
        }
        
        public IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            var sortTerms = ReSieveSortParser.ParseSorts(reSieveModel.Sorts);

            GuardAgainstUnmappedProperties<TEntity>(sortTerms);

            if (sortTerms.Count == 0)
            {
                return source;
            }

            IOrderedQueryable<TEntity>? ordered = null;
            for (var i = 0; i < sortTerms.Count; i++)
            {
                if (!(sortTerms[i] is { } sortTerm))
                {
                    continue;
                }

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

            return ordered ?? source;
        }

        private void GuardAgainstUnmappedProperties<TEntity>(List<ISortTerm> sortTerms)
        {
            _mapper.PropertyMappings.TryGetValue(typeof(TEntity), out var mappedProperties);
            
            if (mappedProperties is null)
            {
                throw new ArgumentException("Not allowed to sort on this entity.");
            }

            if(!sortTerms.All(x => mappedProperties.Keys.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase) && mappedProperties[y].CanSort)))
            {
                throw new ArgumentException("Not allowed to sort on this entity.");
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