using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Resieve.Filtering.Lexers;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;

namespace Resieve.Filtering.ExpressionTrees
{
    //TODO: Convert part of this to Specification pattern
    public class ExpressionTreeBuilder : IExpressionTreeBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpressionTreeBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Expression<Func<TEntity, bool>> BuildFromTokens<TEntity>(List<Token> tokens, Dictionary<string, ResievePropertyMap> customFilters)
        {
            var registeredCustomFilters = _serviceProvider.GetService<IEnumerable<IResieveCustomFilter<TEntity>>>()?.ToList() ?? new List<IResieveCustomFilter<TEntity>>();
            var expressions = new Stack<Expression<Func<TEntity, bool>>>();
            var logicalOperators = new Stack<Token>();

            Token? currentProperty = null;
            Token? currentOperator = null;
            Token? currentValue = null;
            var isOpenParenthesis = false;

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Property:
                        currentProperty = token;
                        break;
                    case TokenType.Operator:
                        currentOperator = token;
                        break;
                    case TokenType.Value:
                        currentValue = token;
                        break;
                    case TokenType.LogicalAnd:
                    case TokenType.LogicalOr:
                        logicalOperators.Push(token);
                        break;
                    case TokenType.OpenParen:
                        isOpenParenthesis = true;
                        break;
                    case TokenType.CloseParen:
                        isOpenParenthesis = false;
                        break;
                }

                if (currentProperty != null && currentOperator != null && currentValue != null)
                {
                    KeyValuePair<string, ResievePropertyMap> hasCustomFilter =
                        customFilters.SingleOrDefault(x => x.Key.Equals(currentProperty.Value, StringComparison.OrdinalIgnoreCase));

                    if (hasCustomFilter.Value?.CustomFilter != null)
                    {
                        var customFilter = registeredCustomFilters.SingleOrDefault(x => x.GetType() == hasCustomFilter.Value.CustomFilter);
                        if (customFilter == null)
                        {
                            throw new InvalidOperationException($"No custom filter registered for type {hasCustomFilter.Value.CustomFilter.Name}.");
                        }

                        var customQueryable = customFilter.GetWhereExpression(currentOperator.Value, currentValue.Value);
                        expressions.Push(customQueryable);
                    }
                    else
                    {
                        var expression = BuildFilterExpression<TEntity>(currentProperty, currentOperator, currentValue);
                        expressions.Push(expression);
                    }

                    currentProperty = null;
                    currentOperator = null;
                    currentValue = null;
                }

                if (logicalOperators.Any() &&
                    (!isOpenParenthesis && expressions.Count == 2 ||
                     isOpenParenthesis && expressions.Count == 3))
                {
                    var rightExpression = expressions.Pop();
                    var leftExpression = expressions.Pop();
                    var currentLogical = logicalOperators.Pop();
                    var combined = CombineLogicalExpressions<TEntity>(leftExpression, rightExpression, currentLogical);
                    expressions.Push((Expression<Func<TEntity, bool>>)combined);
                }
            }

            if (expressions.Count > 1)
            {
                throw new ArgumentException("More than one expression left in the stack. This should not happen.");
            }

            return expressions.Pop();
        }

        private static Expression<Func<TEntity, bool>> BuildFilterExpression<TEntity>(Token property, Token @operator, Token value)
        {
            var param = Expression.Parameter(typeof(TEntity), "e");
            var prop = Expression.PropertyOrField(param, property.Value);
            var propType = prop.Type;

            var typedValue = ConvertToPropertyType(propType, value.Value);
            var constant = Expression.Constant(typedValue, propType);

            Expression body = FilterOperators.GetExpression(@operator.Value, prop, constant);

            return Expression.Lambda<Func<TEntity, bool>>(body, param);
        }

        private static object ConvertToPropertyType(Type propType, string value)
        {
            var targetType = Nullable.GetUnderlyingType(propType) ?? propType;
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value, true);
            }
            return Convert.ChangeType(value, targetType);
        }

        private static LambdaExpression CombineLogicalExpressions<TEntity>(LambdaExpression leftExpression, LambdaExpression rightExpression, Token logicalOperator)
        {
            var param = leftExpression.Parameters.Single();
            var replacedBody = new ParameterReplacer(rightExpression.Parameters.Single(), param).Visit(rightExpression.Body);

            var combinedBody = logicalOperator.Type == TokenType.LogicalAnd
                ? Expression.AndAlso(leftExpression.Body, replacedBody!)
                : Expression.OrElse(leftExpression.Body, replacedBody!);
            return Expression.Lambda<Func<TEntity, bool>>(combinedBody, param);
        }

        // Helper for replacing parameters in expressions
        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _from;
            private readonly ParameterExpression _to;
            
            public ParameterReplacer(ParameterExpression from, ParameterExpression to)
            {
                _from = from;
                _to = to;
            }
            
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _from ? _to : base.VisitParameter(node);
            }
        }
    }
}