using System;
using System.Linq;
using System.Linq.Expressions;
using ReSieve.Models;

namespace ReSieve.Services
{
    public interface ISortingProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class DefaultSortingProcessor : ISortingProcessor
    {
        public IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            if (reSieveModel.Sorts.Count == 0)
            {
                return source;
            }

            IOrderedQueryable<TEntity>? ordered = null;
            for (var i = 0; i < reSieveModel.Sorts.Count; i++)
            {
                if (!(reSieveModel.Sorts[i] is { } sortTerm))
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