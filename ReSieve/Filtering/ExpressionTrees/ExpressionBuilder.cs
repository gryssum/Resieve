using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReSieve.Filtering.Lexers;

namespace ReSieve.Filtering.ExpressionTrees
{
    public class ExpressionBuilder
    {
        public Expression<Func<TEntity, bool>> BuildExpressionFromTokens<TEntity>(List<Token> tokens)
        {
            var expressions = new Stack<Expression<Func<TEntity, bool>>>();
            var logicalOperators = new Stack<Token>();

            Token? currentProperty = null;
            Token? currentOperator = null;
            Token? currentValue = null;
            bool isOpenParenthesis = false;

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

                // Build single property - operator - value expression
                if (currentProperty != null && currentOperator != null && currentValue != null)
                {
                    // Build the expression for the current property, operator, and value
                    expressions.Push(BuildExpression<TEntity>(currentProperty, currentOperator, currentValue));

                    // Reset for next token set
                    currentProperty = null;
                    currentOperator = null;
                    currentValue = null;
                }

                // Combine two expressions with logical operators
                if (logicalOperators.Any() &&
                    (!isOpenParenthesis && expressions.Count == 2 ||
                     isOpenParenthesis && expressions.Count == 3))
                {
                    // Use a parameter replacer to ensure both expressions use the same parameter
                    var rightExpression = expressions.Pop();
                    var leftExpression = expressions.Pop();

                    var param = leftExpression.Parameters.Single();
                    var replacedBody = new ParameterReplacer(rightExpression.Parameters.Single(), param).Visit(rightExpression.Body);

                    var currentLogical = logicalOperators.Pop();
                    var logicalCombination = currentLogical.Type == TokenType.LogicalAnd
                        ? Expression.AndAlso(leftExpression.Body, replacedBody!)
                        : Expression.OrElse(leftExpression.Body, replacedBody!);

                    expressions.Push(Expression.Lambda<Func<TEntity, bool>>(logicalCombination, param));
                }
            }

            if (expressions.Count > 1)
            {
                throw new ArgumentException("Now we done did it. We have more than one expression left in the stack. This should not happen.");
            }

            return expressions.Pop();
        }


        private Expression<Func<TEntity, bool>> BuildExpression<TEntity>(Token property, Token @operator, Token value)
        {
            var param = Expression.Parameter(typeof(TEntity), "e");
            var prop = Expression.PropertyOrField(param, property.Value);
            var propType = prop.Type;

            // Use helper for conversion
            object typedValue = ConvertToPropertyType(propType, value.Value);
            var constant = Expression.Constant(typedValue, propType);

            Expression body = @operator.Value switch
            {
                "==" => Expression.Equal(prop, constant),
                "!=" => Expression.NotEqual(prop, constant),
                ">" => Expression.GreaterThan(prop, constant),
                ">=" => Expression.GreaterThanOrEqual(prop, constant),
                "<" => Expression.LessThan(prop, constant),
                "<=" => Expression.LessThanOrEqual(prop, constant),
                "@=" => Expression.Call(prop, typeof(string).GetMethod("Contains", new[] {typeof(string)})!, constant),
                "!@=" => Expression.Not(Expression.Call(prop, typeof(string).GetMethod("Contains", new[] {typeof(string)})!, constant)),
                "_=" => Expression.Call(prop, typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!, constant),
                "_-=" => Expression.Call(prop, typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!, constant),
                "!_=" => Expression.Not(Expression.Call(prop, typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!, constant)),
                "!_-=" => Expression.Not(Expression.Call(prop, typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!, constant)),
                "==*" => Expression.Equal(
                    Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                    Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                ),
                "!=*" => Expression.NotEqual(
                    Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                    Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                ),
                "@=*" => Expression.Call(
                    Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                    typeof(string).GetMethod("Contains", new[] {typeof(string)})!,
                    Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                ),
                "!@=*" => Expression.Not(
                    Expression.Call(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        typeof(string).GetMethod("Contains", new[] {typeof(string)})!,
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    )
                ),
                "_=*" => Expression.Call(
                    Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                    typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!,
                    Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                ),
                "_-=*" => Expression.Call(
                    Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                    typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!,
                    Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                ),
                "!_=*" => Expression.Not(
                    Expression.Call(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!,
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    )
                ),
                "!_-=*" => Expression.Not(
                    Expression.Call(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!,
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    )
                ),
                _ => throw new NotSupportedException($"Operator '{@operator.Value}' is not supported.")
            };

            return Expression.Lambda<Func<TEntity, bool>>(body, param);
        }

        private static object ConvertToPropertyType(Type propType, string value)
        {
            var targetType = Nullable.GetUnderlyingType(propType) ?? propType;
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value, ignoreCase: true);
            }
            return Convert.ChangeType(value, targetType);
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